using System;
using System.IO;
using System.Net;
using UnityEngine;
using System.Threading;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace AssetModule
{
    /// <summary>
    /// 下载类对象
    /// </summary>
    public class Download : CustomYieldInstruction
    {
        /// <summary>
        /// 下载失败最大重试次数
        /// </summary>
        public const int MaxRetryTimes = 3;

        /// <summary>
        /// 读取buffer的缓冲区大小
        /// </summary>
        public const uint ReadBufferSize = 1024 * 4;

        /// <summary>
        /// 缓冲区
        /// </summary>
        readonly byte[] _readBuffer = new byte[ReadBufferSize];

        /// <summary>
        /// 当前带宽(每秒下载的字节数)
        /// </summary>
        long _bandwidth;

        /// <summary>
        /// 失败重试次数
        /// </summary>
        int _retryTimes;

        /// <summary>
        /// 下载文件流写入器
        /// </summary>
        FileStream _writer;

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsDone => Status == DownloadStatus.Successful || Status == DownloadStatus.Failed || Status == DownloadStatus.Canceled;

        /// <summary>
        /// CustomYieldInstruction需实现字段, 判断是否继续进行协程
        /// </summary>
        public override bool keepWaiting => !IsDone;

        /// <summary>
        /// 下载所需信息
        /// </summary>
        public DownloadInfo downloadInfo;

        /// <summary>
        /// 当前下载状态
        /// </summary>
        public DownloadStatus Status { get; private set; }

        /// <summary>
        /// 错误原因
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// 每帧回调
        /// </summary>
        public Action<Download> updated;

        /// <summary>
        /// 完成回调
        /// </summary>
        public Action<Download> completed;

        /// <summary>
        /// 下载进度
        /// </summary>
        public float Progress => (float)DownloadedBytes / downloadInfo.size;

        /// <summary>
        /// 已下载大小(单位:字节(B))
        /// </summary>
        public long DownloadedBytes { get; private set; }

        /// <summary>
        /// 开始前初始化参数
        /// </summary>
        internal void InitBeforeStart()
        {
            Status = DownloadStatus.Waiting;
            _retryTimes = 0;
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        internal void Start()
        {
            if (Status != DownloadStatus.Waiting)
                return;

            Logger.Info("开始下载:{0}", downloadInfo.url);
            Status = DownloadStatus.Downloading;
            FileInfo file = new FileInfo(downloadInfo.savePath);
            if (file.Exists && file.Length > 0)
            {
                if (downloadInfo.size > 0 && file.Length == downloadInfo.size)
                {
                    Status = DownloadStatus.Successful;
                    return;
                }

                // 文件已存在就继续写入, 即断线重连后继续下载
                _writer = File.OpenWrite(downloadInfo.savePath);
                DownloadedBytes = _writer.Length - 1;
                if (DownloadedBytes > 0)
                    _writer.Seek(-1, SeekOrigin.End);
            }
            else
            {
                string dir = Path.GetDirectoryName(downloadInfo.savePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                _writer = File.Create(downloadInfo.savePath);
                DownloadedBytes = 0;
            }

            // 开启线程执行下载操作
            Thread thread = new Thread(Run) { IsBackground = true };
            thread.Start();
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        public void Restart()
        {
            Status = DownloadStatus.Waiting;
            Start();
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        public void Cancel()
        {
            Error = "取消";
            Status = DownloadStatus.Canceled;
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public void Pause()
        {
            Status = DownloadStatus.Waiting;
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public void Resume()
        {
            if (Status == DownloadStatus.Waiting)
                Restart();
        }

        /// <summary>
        /// 完成通知
        /// </summary>
        internal void Complete()
        {
            if (completed == null)
                return;

            var func = completed;
            completed = null;
            func.Invoke(this);
        }

        /// <summary>
        /// 尝试重新开启下载
        /// </summary>
        /// <returns>返回是否尝试成功</returns>
        bool TryToRestart()
        {
            if (_retryTimes < MaxRetryTimes)
            {
                _retryTimes++;
                Logger.Warning($"下载失败 url:{downloadInfo.url}, 原因:{Error}, 现在自动重试第{_retryTimes}次");
                Thread.Sleep(1000);
                Restart();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 下载线程运行逻辑
        /// </summary>
        void Run()
        {
            try
            {
                Downloading();
                CloseWrite();
                if (Status is DownloadStatus.Failed or DownloadStatus.Canceled or DownloadStatus.Waiting)
                    return;

                if (DownloadedBytes != downloadInfo.size)
                {
                    Error = $"下载大小({DownloadedBytes})和下载信息里的下载大小({downloadInfo.size})不匹配";

                    // 下载大小比所需文件大小大时才删除文件, 否则下次继续下载(因ReadToEnd = true时, 有可能仅是网络异常, 保留文件下次依然可以继续下载)
                    if (DownloadedBytes > downloadInfo.size)
                        File.Delete(downloadInfo.savePath);

                    if (TryToRestart())
                        return;

                    Status = DownloadStatus.Failed;
                    return;
                }

                if (!string.IsNullOrEmpty(downloadInfo.hash))
                {
                    string hash = Utility.ComputeHash(downloadInfo.savePath);
                    if (downloadInfo.hash != hash)
                    {
                        File.Delete(downloadInfo.savePath);
                        Error = $"下载文件的Hash({hash})和下载信息里的Hash({downloadInfo.hash})不匹配";
                        if (TryToRestart())
                            return;

                        Status = DownloadStatus.Failed;
                        return;
                    }
                }

                Status = DownloadStatus.Successful;
            }
            catch (Exception e)
            {
                CloseWrite();
                Error = e.Message;
                if (TryToRestart())
                    return;

                Status = DownloadStatus.Failed;
            }
        }

        /// <summary>
        /// 关闭文件写入流
        /// </summary>
        void CloseWrite()
        {
            if (_writer == null)
                return;

            _writer.Flush();
            _writer.Close();
            _writer = null;
        }

        static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors spe)
        {
            return true;
        }

        /// <summary>
        /// 下载处理
        /// </summary>
        void Downloading()
        {
            WebRequest request = CreateWebRequest();
            using WebResponse response = request.GetResponse();
            if (response.ContentLength > 0)
            {
                if (downloadInfo.size == 0)
                    downloadInfo.size = response.ContentLength + DownloadedBytes;

                using Stream reader = response.GetResponseStream();
                if (DownloadedBytes < downloadInfo.size)
                {
                    DateTime startTime = DateTime.Now;
                    while (Status == DownloadStatus.Downloading)
                    {
                        if (ReadToEnd(reader))
                            break;

                        CheckBandwidthLimit(ref startTime);
                    }
                }
            }
            else
                Status = DownloadStatus.Successful;
        }

        /// <summary>
        /// 将下载的流写入文件, 直至没有下载到流数据
        /// 返回true时, 通常证明已下载完成, 但也有可能仅是网络异常导致下载不了
        /// </summary>
        bool ReadToEnd(Stream reader)
        {
            int len = reader.Read(_readBuffer, 0, _readBuffer.Length);
            if (len > 0)
            {
                _writer.Write(_readBuffer, 0, len);
                DownloadedBytes += len;
                _bandwidth += len;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查带宽限制, 若超过限制带宽, 则对此进程休眠等待
        /// </summary>
        void CheckBandwidthLimit(ref DateTime startTime)
        {
            // 这一秒已运行时间(单位:毫秒), 每秒清零
            double time = (DateTime.Now - startTime).TotalMilliseconds;

            if (time < 1000 && DownloadHandler.maxBandwidth > 0 && Status == DownloadStatus.Downloading &&
                _bandwidth >= DownloadHandler.maxBandwidth / DownloadHandler.DownloadingList.Count)
            {
                // 若未到1秒已经达到下载限制, 则休眠此进程(休眠时间:这一秒剩下的时间, 例如限速5M/s, 下载到0.3秒已经5M了, 则休眠0.7秒)
                Thread.Sleep((int)(1000 - time));
                time = (DateTime.Now - startTime).TotalMilliseconds;
            }

            if (time < 1000)
                return;

            startTime = DateTime.Now;
            _bandwidth = 0L;
        }

        /// <summary>
        /// 创建网络请求
        /// </summary>
        WebRequest CreateWebRequest()
        {
            WebRequest request;
            if (downloadInfo.url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                request = GetHttpWebRequest();
            }
            else if (downloadInfo.url.StartsWith("ftp", StringComparison.OrdinalIgnoreCase))
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(downloadInfo.url);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                if (!string.IsNullOrEmpty(DownloadHandler.ftpUserID))
                    ftpWebRequest.Credentials = new NetworkCredential(DownloadHandler.ftpUserID, DownloadHandler.ftpPassword);

                if (DownloadedBytes > 0)
                    ftpWebRequest.ContentOffset = DownloadedBytes; // 跳过已下载的大小, 然后继续下载

                request = ftpWebRequest;
            }
            else
                request = GetHttpWebRequest();

            return request;
        }

        /// <summary>
        /// 获取HTTP网络请求
        /// </summary>
        WebRequest GetHttpWebRequest()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(downloadInfo.url);
            httpWebRequest.ProtocolVersion = HttpVersion.Version10;
            if (DownloadedBytes > 0)
                httpWebRequest.AddRange(DownloadedBytes); // 跳过已下载的大小, 然后继续下载
            return httpWebRequest;
        }
    }
}