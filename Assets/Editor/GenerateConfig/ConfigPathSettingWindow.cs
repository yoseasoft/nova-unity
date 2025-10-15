using System.IO;
using UnityEngine;
using UnityEditor;

namespace GenerateConfig
{
    /// <summary>
    /// 配置目录设置窗口
    /// </summary>
    public class ConfigPathSettingWindow : EditorWindow
    {
        /// <summary>
        /// 窗口宽度
        /// </summary>
        const float WindowWidth = 430;

        /// <summary>
        /// Excel所在目录
        /// </summary>
        string _configPath;

        /// <summary>
        /// 打开设置窗口
        /// </summary>
        public static void Open()
        {
            var window = GetWindow<ConfigPathSettingWindow>(true, "Excel目录设置");
            window.minSize = window.maxSize = new Vector2(WindowWidth, 180);
        }

        void OnEnable()
        {
            _configPath = GenerateConfigHelper.ConfigPath;
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField(@"请填写配置所在目录:(例:D:\Tiny\Main\Common\Excel)");
                _configPath = EditorGUILayout.TextField(_configPath);

                bool guiEnable = GUI.enabled;

                if (!string.IsNullOrEmpty(_configPath))
                {
                    EditorGUILayout.Space(20);
                    if (Directory.Exists(_configPath))
                    {
                        bool hasFullClientFile = File.Exists(Path.Combine(_configPath, GenerateConfigHelper.GenFullClientFileName));
                        if (hasFullClientFile)
                        {
                            EditorGUILayout.LabelField("目录状态:正常");
                        }
                        else
                        {
                            EditorGUILayout.LabelField("目录状态:");
                            EditorGUILayout.LabelField("检测不到GenClient.bat或GenClientData.bat文件");
                            GUI.enabled = false;
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("错误提示:");
                        EditorGUILayout.LabelField("目录不存在, 请检查目录配置");
                        GUI.enabled = false;
                    }
                }

                if (GUI.Button(new Rect(WindowWidth * 0.5f - 130, 145, 260, 30), "生成"))
                {
                    GenerateConfigHelper.ConfigPath = _configPath;
                    Close();
                    GenerateConfigHelper.Start();
                }

                GUI.enabled = guiEnable;
            }
            EditorGUILayout.EndVertical();
        }
    }
}