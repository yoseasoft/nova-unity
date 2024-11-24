using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace AssetModule
{
    /// <summary>
    /// 原始资源文件加载;(此类用完不需要Release)
    /// 完成后可自行根据此类的保存路径加载;
    /// 原文件不允许和普通资源有引用关系, 通常用于文本文件，独立的AB包或类似Wwise导出的音乐包
    /// </summary>
    public class RawFile : Loadable
    {
        /// <summary>
        /// 文件原打包路径('Assets/_Resources/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')
        /// </summary>
        public string filePath;

        /// <summary>
        /// 资源信息
        /// </summary>
        ManifestBundleInfo _bundleInfo;

        /// <summary>
        /// 保存路径, 加载完成后可使用此路径加载文件
        /// </summary>
        public string savePath;

        /// <summary>
        /// 下载请求
        /// </summary>
        UnityWebRequest _request;

        /// <summary>
        /// 完成回调
        /// </summary>
        public Action<RawFile> completed;

        /// <summary>
        /// 提供给await使用
        /// </summary>
        public Task<RawFile> Task
        {
            get
            {
                TaskCompletionSource<RawFile> tcs = new();
                completed += _ => tcs.SetResult(this);
                return tcs.Task;
            }
        }

        protected override void OnLoad()
        {
            ManifestHandler.GetMainBundleInfoAndDependencies(filePath, out _bundleInfo, out _);
            address = GetRawFileLoadPath(_bundleInfo);
            if (AssetManagement.isOfflineWindows)
                savePath = AssetPath.TranslateToLocalDataPath(_bundleInfo.Name); // Windows下的离线模式直接使用首包目录
            else
            {
                savePath = AssetPath.TranslateToDownloadDataPath(_bundleInfo.Name); // 原始资源下载后使用打包时设置的文件名保存, 不使用hash值

                string dir = Path.GetDirectoryName(savePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }

            Status = LoadableStatus.CheckingVersion;
        }

        protected override void OnLoadImmediately()
        {
            FileInfo fileInfo = new FileInfo(savePath);
            if (fileInfo.Exists)
            {
                if (_bundleInfo.Size == fileInfo.Length && Utility.ComputeHash(savePath) == _bundleInfo.Hash)
                {
                    Finish();
                    return;
                }

                if (AssetManagement.isOfflineWindows)
                {
                    Finish($"文件{filePath}和资源信息(大小或Hash)不匹配");
                    return;
                }

                File.Delete(savePath);
            }
            else
            {
                if (AssetManagement.isOfflineWindows)
                {
                    Finish($"文件{filePath}不存在");
                    return;
                }
            }

            _request = UnityWebRequest.Get(address);
            _request.downloadHandler = new DownloadHandlerFile(savePath);
            _request.certificateHandler = new AcceptAllCertificatesHandler(); // 必须验证, 具体原因可进入AcceptAllCertificatesHandler查看
            _request.SendWebRequest();

            // 同步加载时while循环等待到下载结束
            while (!_request.isDone)
            {
            }

            if (_request.result == UnityWebRequest.Result.ConnectionError || _request.result == UnityWebRequest.Result.ProtocolError)
            {
                Finish($"文件{filePath}, 下载失败:{_request.error}");
                return;
            }

            Finish();
        }

        protected override void OnUpdate()
        {
            if (Status == LoadableStatus.CheckingVersion)
                OnCheckVersionUpdate();
            else if (Status == LoadableStatus.Loading)
                OnLoadingUpdate();
        }

        protected override void OnUnused()
        {
            completed = null;
        }

        protected override void OnComplete()
        {
            if (completed == null)
                return;

            var func = completed;
            completed = null;
            func?.Invoke(this);
        }

        /// <summary>
        /// 检查版本状态Update
        /// </summary>
        void OnCheckVersionUpdate()
        {
            FileInfo fileInfo = new FileInfo(savePath);
            if (fileInfo.Exists)
            {
                if (_bundleInfo.Size == fileInfo.Length && Utility.ComputeHash(savePath) == _bundleInfo.Hash)
                {
                    Finish();
                    return;
                }

                if (AssetManagement.isOfflineWindows)
                {
                    Finish($"文件{filePath}和资源信息(大小或Hash)不匹配");
                    return;
                }

                File.Delete(savePath);
            }
            else
            {
                if (AssetManagement.isOfflineWindows)
                {
                    Finish($"文件{filePath}不存在");
                    return;
                }
            }

            _request = UnityWebRequest.Get(address);
            _request.downloadHandler = new DownloadHandlerFile(savePath);
            _request.certificateHandler = new AcceptAllCertificatesHandler(); // 必须验证, 具体原因可进入AcceptAllCertificatesHandler查看
            _request.SendWebRequest();
            Status = LoadableStatus.Loading;
        }

        /// <summary>
        /// Loading状态Update
        /// </summary>
        void OnLoadingUpdate()
        {
            Progress = _request.downloadProgress;

            if (!_request.isDone)
                return;

            if (_request.result == UnityWebRequest.Result.ConnectionError || _request.result == UnityWebRequest.Result.ProtocolError)
            {
                Finish($"文件{filePath}, 下载失败:{_request.error}");
                return;
            }

            Finish();
        }

        /// <summary>
        /// 获取原始文件加载路径
        /// </summary>
        /// <param name="bundleInfo">清单资源包信息</param>
        static string GetRawFileLoadPath(ManifestBundleInfo bundleInfo)
        {
            if (AssetManagement.offlineMode || AssetManagement.IsBuildInFile(bundleInfo.NameWithHash))
                return AssetPath.TranslateToLocalDataPathUrl(bundleInfo.Name); // 原始文件使用打包时设置的名字放到打包目录, 下载路径需要使用URL格式

            if (AssetManagement.IsBundleFileAlreadyInDownloadPath(bundleInfo))
                return $"AlreadyDownload:{bundleInfo.Name}"; // 已下载的原文件不会再进行加载, 此处仅为了对address赋值

            // 下载使用带Hash的名字下载
            return AssetPath.TranslateToDownloadUrl(bundleInfo.NameWithHash);
        }
    }
}