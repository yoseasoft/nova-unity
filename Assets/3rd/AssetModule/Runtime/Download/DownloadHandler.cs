using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 下载类管理容器
    /// </summary>
    public static class DownloadHandler
    {
        /// <summary>
        /// 最大同时下载数量
        /// </summary>
        public static uint maxDownloadNum = 10;

        /// <summary>
        /// 最大下载带宽(即下载速度)(0为不作限制)
        /// </summary>
        public static long maxBandwidth = 0;

        /// <summary>
        /// 等待开始下载的下载对象列表
        /// </summary>
        static readonly List<Download> PreparedList = new();

        /// <summary>
        /// 下载中的对象列表, 最大数量受maxDownloadNum限制
        /// </summary>
        public static readonly List<Download> DownloadingList = new();

        /// <summary>
        /// 下载对象缓存区, 防止下载时重复下载, 全部下载操作完成后会清空
        /// key:下载地址url, value:Download对象
        /// </summary>
        static readonly Dictionary<string, Download> Cache = new();

        /// <summary>
        /// Ftp下载的用户ID(Https下载不会用到)
        /// </summary>
        public static string ftpUserID;

        /// <summary>
        /// Ftp下载的用户密码(Https下载不会用到)
        /// </summary>
        public static string ftpPassword;

        /// <summary>
        /// 最后采样时间, 用于下载大小记录刷新计算
        /// </summary>
        static float s_lastSampleTime;

        /// <summary>
        /// 最后记录的总共下载大小(单位:字节(B)), 每秒记录一次
        /// </summary>
        static long s_lastTotalDownloadedBytes;

        /// <summary>
        /// 下载速度记录(单位:字节(B)), 每秒记录一次
        /// </summary>
        static long s_downloadSpeed;

        /// <summary>
        /// 是否有在下载中
        /// </summary>
        public static bool Working => DownloadingList.Count > 0;

        /// <summary>
        /// 当前总共已下载大小(单位:字节(B))
        /// </summary>
        public static long TotalDownloadedBytes
        {
            get
            {
                long bytes = 0L;
                foreach (var item in Cache)
                    bytes += item.Value.DownloadedBytes;
                return bytes;
            }
        }

        /// <summary>
        /// 总共需要下载的大小
        /// </summary>
        public static long TotalSize
        {
            get
            {
                long bytes = 0L;
                foreach (var item in Cache)
                    bytes += item.Value.downloadInfo.size;
                return bytes;
            }
        }

        /// <summary>
        /// 当前带宽
        /// </summary>
        public static long TotalBandWidth => TotalDownloadedBytes - s_lastTotalDownloadedBytes;

        /// <summary>
        /// 下载速度
        /// </summary>
        public static long DownloadSpeed => s_downloadSpeed == 0 ? TotalBandWidth : s_downloadSpeed;

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">网页url</param>
        /// <param name="savePath">文件保存路径</param>
        /// <param name="completed">完成回调</param>
        /// <param name="size">文件大小</param>
        /// <param name="hash">文件Hash</param>
        public static Download DownloadAsync(string url, string savePath, Action<Download> completed = null, long size = 0, string hash = "")
        {
            return DownloadAsync(new DownloadInfo { url = url, savePath = savePath, hash = hash, size = size }, completed);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="info">下载信息</param>
        /// <param name="completed">完成回调</param>
        public static Download DownloadAsync(DownloadInfo info, Action<Download> completed = null)
        {
            if (!Cache.TryGetValue(info.url, out var download))
            {
                download = new Download { downloadInfo = info };
                PreparedList.Add(download);
                Cache.Add(info.url, download);
            }
            else
                Logger.Warning($"已有相同地址正在下载:{info.url}");

            if (completed != null)
                download.completed += completed;

            return download;
        }

        /// <summary>
        /// 重试指定的Download对象
        /// </summary>
        public static void Retry(Download download)
        {
            if (!PreparedList.Contains(download))
                PreparedList.Add(download);

            Cache.TryAdd(download.downloadInfo.url, download);
        }

        /// <summary>
        /// 处理下载内容
        /// </summary>
        public static void Update()
        {
            if (PreparedList.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(PreparedList.Count, maxDownloadNum - DownloadingList.Count); i++)
                {
                    Download download = PreparedList[i];
                    PreparedList.RemoveAt(i);
                    i--;
                    DownloadingList.Add(download);
                    download.InitBeforeStart();
                    download.Start();
                }
            }

            if (DownloadingList.Count > 0)
            {
                for (int i = 0; i < DownloadingList.Count; i++)
                {
                    Download download = DownloadingList[i];
                    download.updated?.Invoke(download);
                    if (download.IsDone)
                    {
                        if (download.Status == DownloadStatus.Successful)
                            Logger.Info($"成功下载:{download.downloadInfo.url}");
                        else
                            Logger.Error($"下载失败:{download.downloadInfo.url}, 原因:{download.Error}");

                        DownloadingList.RemoveAt(i);
                        i--;
                        download.Complete();
                    }
                }

                // 每秒记录一次下载大小, 方便获取下载速度
                if (Time.realtimeSinceStartup - s_lastSampleTime >= 1)
                {
                    s_downloadSpeed = TotalDownloadedBytes - s_lastTotalDownloadedBytes;
                    s_lastTotalDownloadedBytes = TotalDownloadedBytes;
                    s_lastSampleTime = Time.realtimeSinceStartup;
                }
            }
            else if (Cache.Count > 0)
            {
                Cache.Clear();
                s_downloadSpeed = 0;
                s_lastTotalDownloadedBytes = 0;
                s_lastSampleTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// 暂停所有下载
        /// </summary>
        public static void PauseAllDownloads()
        {
            foreach (Download download in DownloadingList)
                download.Pause();
        }

        /// <summary>
        /// 继续所有已暂停的下载
        /// </summary>
        public static void ResumeAllDownloads()
        {
            foreach (Download download in DownloadingList)
                download.Resume();
        }

        /// <summary>
        /// 清除所有下载
        /// </summary>
        public static void ClearAllDownloads()
        {
            foreach (Download download in DownloadingList)
                download.Cancel();

            PreparedList.Clear();
            DownloadingList.Clear();
            Cache.Clear();
        }
    }
}