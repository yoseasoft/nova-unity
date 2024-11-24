namespace AssetModule
{
    /// <summary>
    /// 下载信息
    /// </summary>
    public class DownloadInfo
    {
        /// <summary>
        /// 文件大小, 单位:字节(B)
        /// </summary>
        public long size;

        /// <summary>
        /// 下载地址url
        /// </summary>
        public string url;

        /// <summary>
        /// 文件Hash值
        /// </summary>
        public string hash;

        /// <summary>
        /// 保存文件位置
        /// </summary>
        public string savePath;
    }
}