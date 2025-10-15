using System.IO;
using UnityEditor;
using UnityEngine;

namespace SVNUnityExtension
{
    /// <summary>
    /// 设置安装目录窗口
    /// </summary>
    internal class SetSvnInstallPathWindow : EditorWindow
    {
        /// <summary>
        /// 窗口宽度
        /// </summary>
        const float WindowWidth = 430;

        /// <summary>
        /// Svn执行文件名
        /// </summary>
        const string SvnExeFileName = "TortoiseProc.exe";

        /// <summary>
        /// 打开目录设置窗口
        /// </summary>
        internal static void Open()
        {
            var window = GetWindow<SetSvnInstallPathWindow>(true, "设置");
            window.minSize = window.maxSize = new Vector2(WindowWidth, 180);
        }

        /// <summary>
        /// 安装目录
        /// </summary>
        string _installPath;

        void OnEnable()
        {
            _installPath = EditorPrefs.GetString(SvnHelper.SvnInstallPathKey);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField(@"请填写SVN所在目录:(例:C:\Program Files\TortoiseSVN\bin)");
                _installPath = EditorGUILayout.TextField(_installPath);

                bool guiEnable = GUI.enabled;

                if (!string.IsNullOrEmpty(_installPath))
                {
                    if (!Directory.Exists(_installPath))
                    {
                        EditorGUILayout.Space(20);
                        EditorGUILayout.LabelField("错误提示:");
                        EditorGUILayout.LabelField("目录不存在, 请检查目录配置");
                        GUI.enabled = false;
                    }
                    else if (!File.Exists(Path.Combine(_installPath, SvnExeFileName)))
                    {
                        EditorGUILayout.Space(20);
                        EditorGUILayout.LabelField("错误提示:");
                        EditorGUILayout.LabelField($"该目录不存在文件:{SvnExeFileName}, 请重新输入");
                        GUI.enabled = false;
                    }
                }

                if (GUI.Button(new Rect(WindowWidth * 0.5f - 130, 145, 260, 30), "保存"))
                {
                    EditorPrefs.SetString(SvnHelper.SvnInstallPathKey, Path.Combine(_installPath, SvnExeFileName));
                    Close();
                }

                GUI.enabled = guiEnable;
            }
            EditorGUILayout.EndVertical();
        }
    }
}