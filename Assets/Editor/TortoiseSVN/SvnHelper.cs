using System;
using System.IO;
using System.Text;
using UnityEditor;
using System.Diagnostics;

namespace SVNUnityExtension
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class SvnHelper
    {
        /// <summary>
        /// svn安装目录编辑器本地记录key
        /// </summary>
        internal const string svnInstallPathKey = "SvnInstallPath";

        /// <summary>
        /// 更新文件命令行文件名字
        /// </summary>
        const string updateBatFileName = "updateFile.bat";

        /// <summary>
        /// 提交文件命令行文件名字
        /// </summary>
        const string submitBatFileName = "submitFile.bat";

        /// <summary>
        /// 获取已配置的svn安装目录, 若未则打开设置窗口
        /// </summary>
        public static string GetSvnInstallPathOrOpenSettingWindow()
        {
            string svnExePath = EditorPrefs.GetString(svnInstallPathKey, "C:/Program Files/TortoiseSVN/bin/TortoiseProc.exe");
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
        public static void Update(string path, bool isRefresh = true)
        {
            string svnExePath = GetSvnInstallPathOrOpenSettingWindow();
            if (string.IsNullOrEmpty(svnExePath))
                return;

            string batFilePath = Path.Combine(Environment.CurrentDirectory, updateBatFileName);
            if (File.Exists(batFilePath))
                File.Delete(batFilePath);

            EditorUtility.DisplayProgressBar("SVN", "SVN更新中...", 0.8f);

            // "\r\n代表使用CRLF格式换行"
            string writeFileContent =
            "@echo off\r\n\r\n" +

            "%SVN安装路径%\r\n" +
            $"set svn_path={svnExePath}\r\n\r\n" +

            "%更新路径%\r\n" +
            $"set update_path={path}\r\n\r\n" +

            "\"%svn_path%\" /command:update /path:%update_path%";

            File.WriteAllText(batFilePath, writeFileContent, Encoding.GetEncoding("GB2312"));//GB2312相当于文本文档中的ANSI编码

            // bat需使用CRLF换行和ANSI编码才能正确运行
            Process pro = Process.Start(new ProcessStartInfo() { FileName = batFilePath, WindowStyle = ProcessWindowStyle.Hidden });
            pro.WaitForExit();
            pro.Close();

            File.Delete(batFilePath);
            EditorUtility.ClearProgressBar();

            if (isRefresh)
                AssetDatabase.Refresh();
        }

        /// <summary>
        /// 提交指定的文件
        /// </summary>
        public static void Commit(string path)
        {
            string svnExePath = GetSvnInstallPathOrOpenSettingWindow();
            if (string.IsNullOrEmpty(svnExePath))
                return;

            string batFilePath = Path.Combine(Environment.CurrentDirectory, submitBatFileName);
            if (File.Exists(batFilePath))
                File.Delete(batFilePath);

            EditorUtility.DisplayProgressBar("SVN", "SVN提交中...", 0.8f);

            // "\r\n代表使用CRLF格式换行"
            string writeFileContent =
            "@echo off\r\n\r\n" +

            "%SVN安装路径%\r\n" +
            $"set svn_path={svnExePath}\r\n\r\n" +

            "%提交路径%\r\n" +
            $"set commit_path={path}\r\n\r\n" +

            "\"%svn_path%\" /command:commit /path:%commit_path%";

            File.WriteAllText(batFilePath, writeFileContent, Encoding.GetEncoding("GB2312"));//GB2312相当于文本文档中的ANSI编码

            // bat需使用CRLF换行和ANSI编码才能正确运行
            Process pro = Process.Start(new ProcessStartInfo() { FileName = batFilePath, WindowStyle = ProcessWindowStyle.Hidden });
            pro.WaitForExit();
            pro.Close();

            File.Delete(batFilePath);
            EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();
        }
    }
}