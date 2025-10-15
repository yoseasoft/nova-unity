using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

namespace SVNUnityExtension
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class SvnHelper
    {
        /// <summary>
        /// svn安装目录编辑器本地记录key
        /// </summary>
        internal const string SvnInstallPathKey = "SvnInstallPath";

        /// <summary>
        /// 更新文件命令行文件名字
        /// </summary>
        const string UpdateBatFileName = "updateFile.bat";

        /// <summary>
        /// 提交文件命令行文件名字
        /// </summary>
        const string SubmitBatFileName = "submitFile.bat";

        /// <summary>
        /// 是否正在更新
        /// </summary>
        static bool s_isUpdating;

        /// <summary>
        /// Svn更新开始时间
        /// </summary>
        static float s_svnUpdateStartTime;

        /// <summary>
        /// 更新完成回调
        /// </summary>
        static Action s_onUpdateComplete;

        /// <summary>
        /// 获取已配置的svn安装目录, 若未则打开设置窗口
        /// </summary>
        static string GetSvnInstallPathOrOpenSettingWindow()
        {
            string svnExePath = EditorPrefs.GetString(SvnInstallPathKey, "C:/Program Files/TortoiseSVN/bin/TortoiseProc.exe");
            if (!File.Exists(svnExePath))
            {
                SetSvnInstallPathWindow.Open();
                return string.Empty;
            }

            return svnExePath;
        }

        /// <summary>
        /// 更新指定的文件
        /// </summary>
        /// <param name="path">需要更新的目录, 若需要同时更新多个目录可用*号作为分隔符</param>
        /// <param name="isRefresh">更新完是否刷新Unity文件数据(即AssetDatabase.Refresh)</param>
        public static void Update(string path, Action onComplete = null)
        {
            if (s_isUpdating)
            {
                return;
            }

            string svnExePath = GetSvnInstallPathOrOpenSettingWindow();
            if (string.IsNullOrEmpty(svnExePath))
            {
                return;
            }

            string batFilePath = Path.Combine(Environment.CurrentDirectory, UpdateBatFileName);
            if (File.Exists(batFilePath))
            {
                File.Delete(batFilePath);
            }

            EditorUtility.DisplayProgressBar("SVN", "SVN更新中...", 0.8f);

            // "\r\n代表使用CRLF格式换行"
            string writeFileContent =
                "@echo off\r\n\r\n" +
                "%SVN安装路径%\r\n" +
                $"set svn_path={svnExePath}\r\n\r\n" +
                "%更新路径%\r\n" +
                $"set update_path={path}\r\n\r\n" +
                "\"%svn_path%\" /command:update /path:%update_path%";

            File.WriteAllText(batFilePath, writeFileContent, Encoding.GetEncoding("GB2312")); //GB2312相当于文本文档中的ANSI编码

            s_isUpdating = true;
            s_onUpdateComplete = onComplete;
            s_svnUpdateStartTime = Time.realtimeSinceStartup;
            EditorApplication.update += RefreshUpdateProgress;

            // bat需使用CRLF换行和ANSI编码才能正确运行
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo { FileName = batFilePath, WindowStyle = ProcessWindowStyle.Hidden },
                EnableRaisingEvents = true
            };
            process.Exited += (_, _) => { s_isUpdating = false; };
            process.Start();
        }

        [InitializeOnLoadMethod]
        static void OnInit()
        {
            EditorApplication.update += RefreshUpdateProgress;
        }

        /// <summary>
        /// 刷新更新进度条
        /// </summary>
        static void RefreshUpdateProgress()
        {
            if (s_isUpdating)
            {
                EditorUtility.DisplayProgressBar("SVN", "SVN更新中...", Mathf.Min((Time.realtimeSinceStartup - s_svnUpdateStartTime) * 1.5f, 0.95f));
                return;
            }

            EditorUtility.ClearProgressBar();
            EditorApplication.update -= RefreshUpdateProgress;
            AssetDatabase.Refresh();
            File.Delete(Path.Combine(Environment.CurrentDirectory, UpdateBatFileName));

            Action onComplete = s_onUpdateComplete;
            s_onUpdateComplete = null;
            onComplete?.Invoke();
        }

        /// <summary>
        /// 提交指定的文件
        /// </summary>
        public static void Commit(string path)
        {
            string svnExePath = GetSvnInstallPathOrOpenSettingWindow();
            if (string.IsNullOrEmpty(svnExePath))
                return;

            string batFilePath = Path.Combine(Environment.CurrentDirectory, SubmitBatFileName);
            if (File.Exists(batFilePath))
                File.Delete(batFilePath);

            EditorUtility.DisplayProgressBar("SVN", "SVN提交中...", 0.8f);

            // "\r\n代表使用CRLF格式换行"
            string writeFileContent =
                "@echo off\r\n\r\n" +
                "%SVN安装路径%\r\n" +
                $"set svn_path={svnExePath}\r\n\r\n" +
                "%提交路径%\r\n" +
                $"set commit_path={Path.GetFullPath(path)}\r\n\r\n" +
                "\"%svn_path%\" /command:commit /path:%commit_path%";

            File.WriteAllText(batFilePath, writeFileContent, Encoding.GetEncoding("GB2312")); //GB2312相当于文本文档中的ANSI编码

            // bat需使用CRLF换行和ANSI编码才能正确运行
            Process process = new Process { StartInfo = new ProcessStartInfo { FileName = batFilePath, WindowStyle = ProcessWindowStyle.Hidden } };
            process.Start();
            process.WaitForExit();
            process.Close();

            File.Delete(batFilePath);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}