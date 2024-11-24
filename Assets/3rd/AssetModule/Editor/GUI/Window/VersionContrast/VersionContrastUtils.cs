using System;
using System.IO;
using AssetModule.Editor.Build;
using System.Collections.Generic;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 版本对比工具类
    /// </summary>
    internal static class VersionContrastUtils
    {
        /// <summary>
        /// 获取排序后的版本名字列表
        /// </summary>
        internal static void GetVersionFileNameList(List<string> versionFileNameList)
        {
            versionFileNameList.Clear();

            string buildRecordFolderPath = BuildUtils.TranslateToBuildPath(BuildUtils.BuildRecordFolderName);
            DirectoryInfo directoryInfo = new DirectoryInfo(buildRecordFolderPath);
            if (!directoryInfo.Exists)
                return;

            FileInfo[] fileInfoList = directoryInfo.GetFiles("*.json");
            foreach (FileInfo fileInfo in fileInfoList)
            {
                string fileName = fileInfo.Name;
                if (!fileName.StartsWith(BuildUtils.BuildRecordFilePrefix) || !fileName.EndsWith(".json"))
                    continue;

                string replaceFileName = fileName.Replace(".json", string.Empty);
                replaceFileName = replaceFileName.Replace(BuildUtils.BuildRecordFilePrefix, string.Empty);
                string[] sep = replaceFileName.Split('_');
                if (sep.Length == 2 && int.TryParse(sep[0], out int _) && long.TryParse(sep[1], out long _))
                    versionFileNameList.Add(fileName);
            }

            // 按最新时间排序
            versionFileNameList.Sort((a, b) =>
            {
                long aTime = long.Parse(a.Replace(".json", string.Empty).Replace(BuildUtils.BuildRecordFilePrefix, string.Empty).Split('_')[1]);
                long bTime = long.Parse(b.Replace(".json", string.Empty).Replace(BuildUtils.BuildRecordFilePrefix, string.Empty).Split('_')[1]);
                return aTime < bTime ? 1 : -1;
            });
        }

        /// <summary>
        /// 获取当前打包目录下的版本记录文件数量
        /// </summary>
        internal static int GetVersionFileCount()
        {
            string buildRecordFolderPath = BuildUtils.TranslateToBuildPath(BuildUtils.BuildRecordFolderName);
            DirectoryInfo directoryInfo = new DirectoryInfo(buildRecordFolderPath);
            if (!directoryInfo.Exists)
                return 0;

            int fileCount = 0;
            FileInfo[] fileInfoList = directoryInfo.GetFiles("*.json");
            foreach (FileInfo fileInfo in fileInfoList)
            {
                string fileName = fileInfo.Name;
                if (!fileName.StartsWith(BuildUtils.BuildRecordFilePrefix) || !fileName.EndsWith(".json"))
                    continue;

                string replaceFileName = fileName.Replace(".json", string.Empty);
                replaceFileName = replaceFileName.Replace(BuildUtils.BuildRecordFilePrefix, string.Empty);
                string[] sep = replaceFileName.Split('_');
                if (sep.Length == 2 && int.TryParse(sep[0], out int _) && long.TryParse(sep[1], out long _))
                    fileCount++;
            }

            return fileCount;
        }

        /// <summary>
        /// 文件名字转换成显示名字
        /// </summary>
        internal static string ToShowName(string fileName, Dictionary<string, string> fileNameToComment = null)
        {
            string comment = null;
            fileNameToComment?.TryGetValue(fileName, out comment);
            fileName = fileName.Replace(".json", string.Empty);
            fileName = fileName.Replace(BuildUtils.BuildRecordFilePrefix, string.Empty);
            string[] sep = fileName.Split('_');
            DateTime dateTime = DateTime.FromFileTime(long.Parse(sep[1])).ToLocalTime();
            if (string.IsNullOrEmpty(comment))
                return $"v{int.Parse(sep[0])}({dateTime:yyyy-MM-dd HH:mm:ss})";

            return $"v{int.Parse(sep[0])}({dateTime:yyyy-MM-dd HH:mm:ss})({comment})";
        }

        /// <summary>
        /// 根据记录文件名获取版本号
        /// </summary>
        internal static int GetBuildVersion(string fileName)
        {
            fileName = fileName.Replace(".json", string.Empty);
            fileName = fileName.Replace(BuildUtils.BuildRecordFilePrefix, string.Empty);
            return int.Parse(fileName.Split('_')[0]);
        }

        /// <summary>
        /// 根据记录文件名获取版本构建时间
        /// </summary>
        internal static long GetVersionBuildTime(string fileName)
        {
            fileName = fileName.Replace(".json", string.Empty);
            fileName = fileName.Replace(BuildUtils.BuildRecordFilePrefix, string.Empty);
            return long.Parse(fileName.Split('_')[1]);
        }

        /// <summary>
        /// 根据构建版本和时间戳转换成记录文件名字
        /// </summary>
        internal static string TranslateToVersionFileName(int version, long timestamp)
        {
            return $"{BuildUtils.BuildRecordFilePrefix}{version}_{timestamp}.json";
        }

        /// <summary>
        /// 加载注释文件并初始化备注字典
        /// </summary>
        internal static void LoadCommentDataAndRefreshCommentDictionary(Dictionary<string, string> recordFileNameToComment)
        {
            recordFileNameToComment.Clear();

            RecordComment recordComment = RecordComment.LoadRecordComment();
            if (recordComment == null)
                return;

            string[] nameList = recordComment.recordFileNameList;
            string[] commentList = recordComment.recordCommentList;
            for (int i = 0; i < nameList.Length; i++)
                recordFileNameToComment.Add(nameList[i], commentList[i]);
        }
    }
}