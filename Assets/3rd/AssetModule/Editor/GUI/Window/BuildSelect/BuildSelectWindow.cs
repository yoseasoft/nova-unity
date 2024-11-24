using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using AssetModule.Editor.Build;
using System.Collections.Generic;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 打包选择窗口
    /// </summary>
    public class BuildSelectWindow : EditorWindow
    {
        public static void Open()
        {
            var win = GetWindow<BuildSelectWindow>(true, "资源构建");
            win.maxSize = win.minSize = new Vector2(520, 325);
        }

        /// <summary>
        /// 滑动列表滚动位置
        /// </summary>
        Vector2 _scrollPos;

        /// <summary>
        /// 清单配置列表
        /// </summary>
        List<ManifestConfig> _manifestConfigs;

        /// <summary>
        /// 构建完成后是否打开版本对比窗口
        /// </summary>
        bool _isOpenVersionContrastWindowAfterBuild;

        /// <summary>
        /// 版本记录文件数量(已有记录文件时才显示查看差异选项)
        /// </summary>
        int _versionRecordFileCount;

        /// <summary>
        /// 记录构建完成是否打开对比窗口的Key值
        /// </summary>
        const string OpenWindowRecordKey = "AssetModuleBuildSelectWindowIsOpenWindowAfterBuild";

        /// <summary>
        /// 构建完成是否打开打包目录
        /// </summary>
        bool _isOpenFolderAfterBuild;

        /// <summary>
        /// 记录构建完成是否打开对比窗口的Key值
        /// </summary>
        const string OpenFolderRecordKey = "AssetModuleBuildSelectWindowIsOpenFolderAfterBuild";

        void OnEnable()
        {
            _manifestConfigs = BuildUtils.GetAllManifestConfigs();
            _versionRecordFileCount = VersionContrastUtils.GetVersionFileCount();
            _isOpenVersionContrastWindowAfterBuild = _versionRecordFileCount > 0 && EditorPrefs.GetBool(OpenWindowRecordKey, false);
            _isOpenFolderAfterBuild = EditorPrefs.GetBool(OpenFolderRecordKey, false);
        }

        void OnGUI()
        {
            if (GUILayout.Button("全部构建", GUILayout.Height(50)))
            {
                Close();

                if (BuildExecuter.BuildAllManifests())
                {
                    // 显示版本对比
                    if (_isOpenVersionContrastWindowAfterBuild)
                        OpenVersionContrastWindow();

                    // 打开打包目录
                    if (_isOpenFolderAfterBuild)
                        EditorUtility.OpenWithDefaultApp(BuildUtils.PlatformBuildPath);
                }
            }

            EditorGUILayout.Space();
            GUILayout.Label("请选择本次需要打包的资源清单:");

            ManifestConfig selectedManifest = null;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(158));
            {
                foreach (ManifestConfig config in _manifestConfigs)
                {
                    if (GUILayout.Button(config.name, GUILayout.Height(50)))
                    {
                        selectedManifest = config;
                        break;
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            if (selectedManifest)
            {
                Close();

                if (BuildExecuter.BuildManifest(selectedManifest))
                {
                    // 显示版本对比
                    if (_isOpenVersionContrastWindowAfterBuild)
                        OpenVersionContrastWindow();

                    // 打开打包目录
                    if (_isOpenFolderAfterBuild)
                        EditorUtility.OpenWithDefaultApp(BuildUtils.PlatformBuildPath);
                }
            }

            bool isOpen;
            if (_versionRecordFileCount > 0)
            {
                Rect openContrastWindowToggleRect = new Rect(5, 276, 480, 20);
                isOpen = EditorGUI.Toggle(openContrastWindowToggleRect, "构建完成查看版本差异?", _isOpenVersionContrastWindowAfterBuild);
                if (isOpen != _isOpenVersionContrastWindowAfterBuild)
                {
                    _isOpenVersionContrastWindowAfterBuild = isOpen;
                    EditorPrefs.SetBool(OpenWindowRecordKey, isOpen);
                }
            }

            Rect openFolderWindowToggleRect = new Rect(5, 301, 480, 20);
            isOpen = EditorGUI.Toggle(openFolderWindowToggleRect, "完成后打开构建目录?", _isOpenFolderAfterBuild);
            if (isOpen != _isOpenFolderAfterBuild)
            {
                _isOpenFolderAfterBuild = isOpen;
                EditorPrefs.SetBool(OpenFolderRecordKey, isOpen);
            }
        }

        /// <summary>
        /// 打开版本对比窗口
        /// </summary>
        void OpenVersionContrastWindow()
        {
            string buildRecordFolderPath = BuildUtils.TranslateToBuildPath(BuildUtils.BuildRecordFolderName);
            DirectoryInfo directoryInfo = new DirectoryInfo(buildRecordFolderPath);
            if (!directoryInfo.Exists)
                return;

            DateTime nowTime = DateTime.Now;
            FileInfo[] fileInfoList = directoryInfo.GetFiles("*.json");
            foreach (FileInfo fileInfo in fileInfoList)
            {
                string fileName = fileInfo.Name;
                if (!fileName.StartsWith(BuildUtils.BuildRecordFilePrefix) || !fileName.EndsWith(".json"))
                    continue;

                // 通过比较时间发现有最新的文件才打开对比, 资源没有变化时不打开对比窗口
                if (new TimeSpan(nowTime.Ticks - fileInfo.CreationTime.Ticks).TotalSeconds <= 2)
                {
                    VersionContrastWindow win = VersionContrastWindow.Open();
                    win.RefreshVersionFileNameList();
                    win.StartContrastNewestVersions(true);
                    break;
                }
            }
        }
    }
}