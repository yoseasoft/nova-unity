using System;
using CoreEngine;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace GenerateConfig
{
    /// <summary>
    /// 生成配置
    /// </summary>
    public static class GenerateConfigHelper
    {
        /// <summary>
        /// 保存配置目录的文件路径
        /// </summary>
        const string SavePathFilePath = "Library/LubanGenerateConfigPath";

        /// <summary>
        /// 客户端完整转换的bat文件名
        /// </summary>
        internal const string GenFullClientFileName = "GenClient.bat";

        /// <summary>
        /// 服务端完整转换的bat文件名
        /// </summary>
        const string GenFullServerFileName = "GenServer.bat";

        /// <summary>
        /// 客户端快速转换的bat文件名
        /// </summary>
        const string QuickGenClientFileName = "GenClientQuickly.bat";

        /// <summary>
        /// EditorPrefs储存值(是否运行服务端转表)
        /// </summary>
        internal const string IsGenerateServerConfigKey = "_IsGenerateServerConfig_";

        /// <summary>
        /// EditorPrefs储存值(是否使用本地快速转表)
        /// </summary>
        internal const string IsGenLocalConfigQuicklyKey = "_IsGenLocalConfigQuicklyKey_";

        /// <summary>
        /// 默认目录
        /// </summary>
        static readonly string s_defaultPath = (Application.dataPath + @"\..\..\common\excel\").Replace('/', '\\');

        /// <summary>
        /// 转表开始时间
        /// </summary>
        static float s_generateConfigStartTime;

        /// <summary>
        /// 进程是否完成运行
        /// </summary>
        static bool s_isProcessFinished;

        /// <summary>
        /// 是否正在快速转表
        /// </summary>
        static bool s_isQuickGenerating;

        /// <summary>
        /// 配置目录
        /// </summary>
        public static string ConfigPath
        {
            get
            {
                string configPath = File.Exists(SavePathFilePath) ? File.ReadAllText(SavePathFilePath) : s_defaultPath;
                if (!Directory.Exists(configPath))
                {
                    ConfigPathSettingWindow.Open();
                    Debug.LogError("请先正确配置Excel目录");
                    return null;
                }

                return configPath;
            }
            set => File.WriteAllText(SavePathFilePath, value.Replace('/', '\\'));
        }

        /// <summary>
        /// 关卡配置目录
        /// </summary>
        public const string LevelConfigPath = "Assets/_Resources/Config/Level";

        /// <summary>
        /// 开始转表
        /// </summary>
        public static void Start()
        {
            string configPath = ConfigPath;
            if (string.IsNullOrEmpty(configPath))
            {
                return;
            }

            // 先覆盖关卡配置文件到Checker工程, 然后再开始转表
            if (Directory.Exists(LevelConfigPath))
            {
                string checkerLevelConfigPath = Path.Combine(ConfigPath, "Checker", "ConfigChecker", "Data", "Level");
                FileUtil.DeleteFileOrDirectory(checkerLevelConfigPath);
                FileUtil.CopyFileOrDirectory(LevelConfigPath, checkerLevelConfigPath);
                foreach (string metaFiles in Directory.GetFiles(checkerLevelConfigPath, "*.meta", SearchOption.AllDirectories))
                {
                    File.Delete(metaFiles);
                }
            }

            s_isQuickGenerating = false;
            s_generateConfigStartTime = Time.realtimeSinceStartup;
            EditorUtility.DisplayProgressBar("Excel", "Excel转换中...", 0);
            EditorApplication.update += UpdateProgress;

            s_isProcessFinished = false;
            bool isGenServer = EditorPrefs.GetBool(IsGenerateServerConfigKey) && File.Exists(Path.Combine(configPath, GenFullServerFileName));
            Process process = CreateProcess(GenFullClientFileName, configPath, Application.dataPath, isGenServer);
            process.Exited += (_, _) => { s_isProcessFinished = true; };
            process.Start();
        }

        /// <summary>
        /// 客户端快速转表
        /// </summary>
        public static void StartQuickly()
        {
            string configPath = ConfigPath;
            if (string.IsNullOrEmpty(configPath))
            {
                return;
            }

            s_isQuickGenerating = true;
            s_generateConfigStartTime = Time.realtimeSinceStartup;
            EditorUtility.DisplayProgressBar("Excel", "Excel快速转换中...", 0);
            EditorApplication.update += UpdateProgress;

            s_isProcessFinished = false;
            Process process = CreateProcess(QuickGenClientFileName, configPath, Application.dataPath, false);
            process.Exited += (_, _) => { s_isProcessFinished = true; };
            process.Start();
        }

        /// <summary>
        /// 根据一个预估的固定时间显示进度
        /// </summary>
        static void UpdateProgress()
        {
            if (!s_isProcessFinished)
            {
                float maxTime = s_isQuickGenerating ? 3.8f : 22;
                EditorUtility.DisplayProgressBar("Excel", "Excel转换中...", Mathf.Min((Time.realtimeSinceStartup - s_generateConfigStartTime) / maxTime, 0.98f));
                return;
            }

            OnProcessFinished();
        }

        /// <summary>
        /// 进程完成处理
        /// </summary>
        static void OnProcessFinished()
        {
            EditorApplication.update -= UpdateProgress;
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            Debug.Log("导出完成");

            // 重载配置
            if (Application.isPlaying)
            {
                AppStart.ReloadConfigure();
            }
        }

        /// <summary>
        /// 创建运行进程
        /// </summary>
        static Process CreateProcess(string cmd, string workDirectory, string assetFolderPath, bool isGenServer)
        {
            const string app = "cmd.exe";
            const string arguments = "/c";
            Process process = new()
            {
                StartInfo = new ProcessStartInfo(app)
                {
                    Arguments = $"{arguments} \"{cmd}\" {assetFolderPath} {(isGenServer ? "1" : "")}",
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    WorkingDirectory = workDirectory
                },
                EnableRaisingEvents = true
            };
            return process;
        }
    }
}