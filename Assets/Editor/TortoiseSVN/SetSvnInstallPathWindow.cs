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
        static float windowWidth = 430;

        /// <summary>
        /// 打开目录设置窗口
        /// </summary>
        internal static void Open()
        {
            var window = GetWindow<SetSvnInstallPathWindow>(true, "设置");
            window.minSize = window.maxSize = new Vector2(windowWidth, 180);
            window.position = new Rect(Screen.width * 0.5f - windowWidth * 0.5f, Screen.height * 0.5f - 180, windowWidth, 180);
        }

        /// <summary>
        /// 安装目录
        /// </summary>
        string installPath;

        void OnEnable()
        {
            installPath = EditorPrefs.GetString(SvnHelper.svnInstallPathKey);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("请填写SVN所在目录:(例:C:\\Program Files\\TortoiseSVN\\bin\\TortoiseProc.exe)");
                installPath = EditorGUILayout.TextField(installPath);

                bool guiEnable = GUI.enabled;

                if (!string.IsNullOrEmpty(installPath))
                {
                    if (!File.Exists(installPath))
                    {
                        EditorGUILayout.Space(20);
                        EditorGUILayout.LabelField("错误提示:");
                        EditorGUILayout.LabelField("目录不存在, 请检查目录配置");
                        GUI.enabled = false;
                    }
                    else if (!installPath.EndsWith("TortoiseProc.exe"))
                    {
                        EditorGUILayout.Space(20);
                        EditorGUILayout.LabelField("错误提示:");
                        EditorGUILayout.LabelField("请以TortoiseProc.exe结尾");
                        GUI.enabled = false;
                    }
                }

                if (GUI.Button(new Rect(windowWidth * 0.5f - 130, 145, 260, 30), "保存"))
                {
                    EditorPrefs.SetString(SvnHelper.svnInstallPathKey, installPath);
                    Close();
                }

                GUI.enabled = guiEnable;
            }
            EditorGUILayout.EndVertical();
        }
    }
}