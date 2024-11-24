using UnityEngine;

namespace AssetModule
{
    /// <summary>
    /// 需要网络下载的资源包对象
    /// </summary>
    public sealed class DownloadBundle : Bundle
    {
        /// <summary>
        /// 下载过程对整个加载进度的占比
        /// (因下载受网络影响较大, 且本地加载ab包速度极快, 所以给下载过程80%的占比)
        /// </summary>
        private const float DownloadProportion = 0.8f;

        /// <summary>
        /// Download对象
        /// </summary>
        Download _download;

        /// <summary>
        /// 下载是否完成
        /// </summary>
        bool _isDownloadCompleted;

        /// <summary>
        /// ab包加载请求
        /// </summary>
        AssetBundleCreateRequest _request;

        /// <summary>
        /// 加密文件流
        /// </summary>
        CryptoAssetBundleStream _stream;

        protected override void OnLoad()
        {
            string savePath = AssetPath.TranslateToDownloadDataPath(bundleInfo.SaveFileName);
            _download = DownloadHandler.DownloadAsync(address, savePath, OnDownloadCompleted, bundleInfo.Size, bundleInfo.Hash);
        }

        /// <summary>
        /// 下载完成处理
        /// </summary>
        void OnDownloadCompleted(Download download)
        {
            _isDownloadCompleted = true;

            if (download.Status == DownloadStatus.Failed)
            {
                Finish(download.Error);
                return;
            }

            if (BundleHandler.IsBundleEncrypt)
                _request = BundleHandler.LoadAssetBundleFromStreamAsync(download.downloadInfo.savePath, bundleInfo, out _stream);
            else
                _request = AssetBundle.LoadFromFileAsync(download.downloadInfo.savePath, 0, (ulong)BundleHandler.BundleOffset);
        }

        protected override void OnLoadImmediately()
        {
            // 下载的资源包同步加载时会一直等到下载完成, 下载速度慢容易导致程序卡住, 慎用
            // 此处不能直接使用download.IsDone, 因为OnDownloadCompleted比download.IsDone = true晚调用
            while (!_isDownloadCompleted)
                DownloadHandler.Update();

            // 没有request代表下载失败了
            if (_request == null)
                return;

            // 异步加载过程中(即request.isDone = false时)直接访问request.assetBundle, 会立即变成同步加载并返回加载的ab包
            // 文档:https://docs.unity.cn/cn/current/ScriptReference/AssetBundleCreateRequest-assetBundle.html
            OnBundleLoaded(_request.assetBundle);
        }

        protected override void OnUpdate()
        {
            if (Status != LoadableStatus.Loading)
                return;

            // 加载进度 = 下载进度 * downloadProportion + 本地加载进度 * (1 - downloadProportion)

            if (!_isDownloadCompleted)
            {
                Progress = _download.DownloadedBytes / _download.downloadInfo.size * DownloadProportion;
                return;
            }

            if (_request == null)
                return;

            Progress = DownloadProportion + (1 - DownloadProportion) * _request.progress;

            if (_request.isDone)
            {
                OnBundleLoaded(_request.assetBundle);
                _request = null;
            }
        }

        protected override void OnUnload()
        {
            // 先卸载ab, 再卸载stream, stream的生命周期要比ab长
            // https://docs.unity.cn/cn/current/ScriptReference/AssetBundle.LoadFromStreamAsync.html
            base.OnUnload();
            _stream?.Dispose();
            _stream = null;
        }
    }
}