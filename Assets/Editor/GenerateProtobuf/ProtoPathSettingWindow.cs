using System.IO;
using UnityEngine;
using UnityEditor;

namespace GenerateProtobuf
{
    /// <summary>
    /// 协议目录配置窗口
    /// </summary>
    public class ProtoPathSettingWindow : EditorWindow
    {
        /// <summary>
        /// 窗口宽度
        /// </summary>
        const float WindowWidth = 480;

        /// <summary>
        /// Protobuf文件目录
        /// </summary>
        string _protobufPath;

        /// <summary>
        /// 打开设置窗口
        /// </summary>
        public static void Open()
        {
            var window = GetWindow<ProtoPathSettingWindow>(true, "Protobuf目录设置");
            window.minSize = window.maxSize = new Vector2(WindowWidth, 180);
        }

        void OnEnable()
        {
            _protobufPath = EditorPrefs.GetString(Proto2CsHelper.SaveKey, Proto2CsHelper.defaultPath.Replace('/', '\\'));
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField(@"请填写Protobuf所在目录（例如：C:\Users\Public\Documents\Project\Common\Proto）");
                _protobufPath = EditorGUILayout.TextField(_protobufPath);

                bool guiEnable = GUI.enabled;

                if (!string.IsNullOrEmpty(_protobufPath))
                {
                    EditorGUILayout.Space(20);
                    if (Directory.Exists(_protobufPath))
                    {
                        if (Directory.GetFiles(_protobufPath, "*.proto", SearchOption.AllDirectories).Length > 0)
                        {
                            EditorGUILayout.LabelField("目录状态:正常");
                        }
                        else
                        {
                            EditorGUILayout.LabelField("目录状态:");
                            EditorGUILayout.LabelField("检测不到proto文件！");
                            GUI.enabled = false;
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("错误提示:");
                        EditorGUILayout.LabelField("目录不存在, 请检查目录配置！");
                        GUI.enabled = false;
                    }
                }

                if (GUI.Button(new Rect(WindowWidth * 0.5f - 130, 145, 260, 30), "生成"))
                {
                    EditorPrefs.SetString(Proto2CsHelper.SaveKey, _protobufPath);
                    Close();
                    Proto2CsHelper.Start();
                }

                GUI.enabled = guiEnable;
            }
            EditorGUILayout.EndVertical();
        }
    }
}