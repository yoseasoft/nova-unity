using System.IO;
using UnityEngine;
using UnityEditor;
using AssetModule.Editor.Build;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 修改打包版本窗口
    /// </summary>
    public class ChangeBuildVersionWindow : EditorWindow
    {
        public static void Open()
        {
            var win = GetWindow<ChangeBuildVersionWindow>(true, "手动修改打包版本");
            win.maxSize = win.minSize = new Vector2(500, 300);
        }

        /// <summary>
        /// 输入版本
        /// </summary>
        int _inputVersion;

        /// <summary>
        /// 版本文件目录
        /// </summary>
        string _versionFilePath;

        /// <summary>
        /// 清单版本容器
        /// </summary>
        ManifestVersionContainer _versionContainer;

        void OnEnable()
        {
            _versionFilePath = BuildUtils.TranslateToBuildPath(ManifestHandler.VersionFileName);
            if (File.Exists(_versionFilePath))
                _versionContainer = ManifestHandler.LoadManifestVersionContainer(_versionFilePath);
            if (!_versionContainer)
                _versionContainer = CreateInstance<ManifestVersionContainer>();

            _inputVersion = _versionContainer.Version;
        }

        void OnGUI()
        {
            GUILayout.Label($"当前平台:{BuildUtils.GetActiveBuildPlatformName()}, 当前资源版本号:{_versionContainer.Version}");

            GUILayout.Space(10);

            _inputVersion = EditorGUILayout.IntField("请填写修改后的版本", _inputVersion, GUILayout.Width(230));
            if (_inputVersion < 0)
                _inputVersion = 0;

            GUILayout.Space(10);

            if (GUILayout.Button("保存", GUILayout.Height(30), GUILayout.Width(232)))
            {
                if (_inputVersion != _versionContainer.Version)
                {
                    ChangeVersion();
                    Close();
                    Debug.Log($"已成功将版本修改到:{_inputVersion}");
                }
                else
                {
                    Close();
                    Debug.Log($"版本相同, 无需修改");
                }
            }
        }

        /// <summary>
        /// 修改版本
        /// </summary>
        void ChangeVersion()
        {
            int oldVersion = _versionContainer.Version;
            _versionContainer.Version = _inputVersion;
            string json = ManifestHandler.ManifestObjectToJson(_versionContainer);

            // 原文件
            File.WriteAllText(_versionFilePath, json);

            string oldVersionFileWithVersionName = ManifestHandler.GetVersionFileNameWithVersion(oldVersion);
            string newVersionFileWithVersionName = ManifestHandler.GetVersionFileNameWithVersion(_inputVersion);

            // 带版本号文件(打包目录)
            string oldVersionFileWithVersionPath = BuildUtils.TranslateToBuildPath(oldVersionFileWithVersionName);
            string newVersionFileWithVersionPath = BuildUtils.TranslateToBuildPath(newVersionFileWithVersionName);
            File.Delete(oldVersionFileWithVersionPath);
            File.WriteAllText(newVersionFileWithVersionPath, json);

            // 带版本号文件(上传目录)
            string oldVersionFileWithVersionUploadPath = AssetPath.CombinePath(BuildUtils.PlatformUploadPath, oldVersionFileWithVersionName);
            string newVersionFileWithVersionUploadPath = AssetPath.CombinePath(BuildUtils.PlatformUploadPath, newVersionFileWithVersionName);
            File.Delete(oldVersionFileWithVersionUploadPath);
            File.WriteAllText(newVersionFileWithVersionUploadPath, json);

            // 版本信息记录文件
            string buildRecordFolderPath = BuildUtils.TranslateToBuildPath(BuildUtils.BuildRecordFolderName);
            DirectoryInfo directoryInfo = new DirectoryInfo(buildRecordFolderPath);
            if (directoryInfo.Exists)
            {
                string oldRecordFileName = VersionContrastUtils.TranslateToVersionFileName(oldVersion, _versionContainer.Timestamp);
                string oldRecordFilePath = AssetPath.CombinePath(buildRecordFolderPath, oldRecordFileName);
                if (File.Exists(oldRecordFilePath))
                {
                    BuildRecord buildRecord = JsonUtility.FromJson<BuildRecord>(File.ReadAllText(oldRecordFilePath));
                    if (buildRecord != null)
                    {
                        buildRecord.version = _inputVersion;
                        string newRecordFileName = VersionContrastUtils.TranslateToVersionFileName(_inputVersion, _versionContainer.Timestamp);
                        string newRecordFilePath = AssetPath.CombinePath(buildRecordFolderPath, newRecordFileName);
                        File.WriteAllText(newRecordFilePath, JsonUtility.ToJson(buildRecord));
                        File.Delete(oldRecordFilePath);
                    }
                }
            }

            // 新的统计文件
            string newUploadStatisticsFilePath = BuildUtils.GetUploadStatisticsFilePath(_inputVersion);
            if (File.Exists(newUploadStatisticsFilePath))
                File.Delete(newUploadStatisticsFilePath);
            FileStream newFileStream = new FileStream(newUploadStatisticsFilePath, FileMode.CreateNew);
            StreamWriter newStreamWriter = new StreamWriter(newFileStream);

            string oldUploadStatisticsFilePath = BuildUtils.GetUploadStatisticsFilePath(oldVersion);
            if (File.Exists(oldUploadStatisticsFilePath))
            {
                // 读取旧的统计文件写入到新的统计文件(仅改变版本文件名字所在的行)
                FileStream oldFileStream = new FileStream(oldUploadStatisticsFilePath, FileMode.Open);
                StreamReader oldStreamReader = new StreamReader(oldFileStream);

                while (!oldStreamReader.EndOfStream)
                {
                    string str = oldStreamReader.ReadLine();
                    if (!str.Equals(oldVersionFileWithVersionName))
                        newStreamWriter.WriteLine(str);
                    else
                        newStreamWriter.WriteLine(newVersionFileWithVersionName);
                }

                oldStreamReader.Dispose();
                oldFileStream.Dispose();
                File.Delete(oldUploadStatisticsFilePath);
            }
            else
            {
                // 没有旧的统计文件时, 仅写上版本文件
                newStreamWriter.WriteLine(newVersionFileWithVersionName);
            }

            newStreamWriter.Dispose();
            newFileStream.Dispose();
        }
    }
}