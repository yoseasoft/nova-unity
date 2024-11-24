using System;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 下载操作
    /// </summary>
    public sealed class DownloadOperation : Operation
    {
        /// <summary>
        /// 需要下载的信息列表
        /// </summary>
        public readonly List<DownloadInfo> needDownloadInfoList = new();

        /// <summary>
        /// 正在下载的Download对象列表
        /// </summary>
        readonly List<Download> _downloadingList = new();

        /// <summary>
        /// 下载成功的Download对象列表
        /// </summary>
        readonly List<Download> _successfulDownloadList = new();

        /// <summary>
        /// 下载失败的Download对象列表
        /// </summary>
        readonly List<Download> _failedDownloadList = new();

        /// <summary>
        /// 是否正在重试
        /// </summary>
        bool _isRetrying;

        /// <summary>
        /// 每帧刷新监听
        /// </summary>
        public Action<DownloadOperation> updated;

        /// <summary>
        /// 下载总大小(单位:字节(B))
        /// </summary>
        public long TotalSize { get; private set; }

        /// <summary>
        /// 已下载的大小(单位:字节(B))
        /// </summary>
        public long DownloadedBytes { get; private set; }

        /// <summary>
        /// 下载操作
        /// </summary>
        /// <param name="downloadInfoList">需要下载的下载信息列表</param>
        public DownloadOperation(List<DownloadInfo> downloadInfoList)
        {
            needDownloadInfoList.AddRange(downloadInfoList);
        }

        protected override void OnStart()
        {
            if (_isRetrying)
                return;

            DownloadedBytes = 0;
            foreach (DownloadInfo downloadInfo in needDownloadInfoList)
                TotalSize += downloadInfo.size;

            if (needDownloadInfoList.Count > 0)
                foreach (DownloadInfo downloadInfo in needDownloadInfoList)
                    _downloadingList.Add(DownloadHandler.DownloadAsync(downloadInfo));
            else
                Finish();
        }

        /// <summary>
        /// 下载失败后重试
        /// </summary>
        public void Retry()
        {
            _isRetrying = true;
            Start();
            foreach (Download download in _failedDownloadList)
            {
                DownloadHandler.Retry(download);
                _downloadingList.Add(download);
            }

            _failedDownloadList.Clear();
        }

        protected override void OnUpdate()
        {
            if (Status != OperationStatus.Processing)
                return;

            int downloadingCount = _downloadingList.Count;
            if (downloadingCount > 0)
            {
                long downloadedBytes = 0;

                for (int i = 0; i < downloadingCount; i++)
                {
                    Download download = _downloadingList[i];
                    if (download.IsDone)
                    {
                        _downloadingList.RemoveAt(i);
                        i--;
                        downloadingCount--;
                        if (download.Status == DownloadStatus.Successful)
                            _successfulDownloadList.Add(download);
                        else
                            _failedDownloadList.Add(download);
                    }
                    else
                        downloadedBytes += download.DownloadedBytes;
                }

                foreach (Download download in _successfulDownloadList)
                    downloadedBytes += download.DownloadedBytes;

                foreach (Download download in _failedDownloadList)
                    downloadedBytes += download.DownloadedBytes;

                DownloadedBytes = downloadedBytes;
                Progress = (float)downloadedBytes / TotalSize;
                updated?.Invoke(this);
            }
            else
            {
                updated = null;
                Finish(_failedDownloadList.Count > 0 ? $"有{_failedDownloadList.Count}个文件下载失败, 首个文件失败原因:{_failedDownloadList[0].Error}" : null);
            }
        }
    }
}