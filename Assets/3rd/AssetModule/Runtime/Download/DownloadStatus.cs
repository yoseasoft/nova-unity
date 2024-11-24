namespace AssetModule
{
    /// <summary>
    /// 下载状态
    /// </summary>
    public enum DownloadStatus
    {
        /// <summary>
        /// 等待下载
        /// </summary>
        Waiting,

        /// <summary>
        /// 下载中
        /// </summary>
        Downloading,

        /// <summary>
        /// 下载成功
        /// </summary>
        Successful,

        /// <summary>
        /// 下载失败
        /// </summary>
        Failed,

        /// <summary>
        /// 被取消
        /// </summary>
        Canceled,
    }
}