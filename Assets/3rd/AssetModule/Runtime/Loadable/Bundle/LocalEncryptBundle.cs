using UnityEngine;
using UnityEngine.Networking;

namespace AssetModule
{
    /// <summary>
    /// 本地加密资源包对象
    /// </summary>
    public sealed class LocalEncryptBundle : Bundle
    {
        /// <summary>
        /// 解压过程对整个加载进度的占比
        /// (因涉及IO操作, 且本地加载ab包速度极快, 所以给解压过程80%的占比)
        /// </summary>
        private const float UnpackProportion = 0.8f;

        /// <summary>
        /// ab文件复制请求
        /// </summary>
        UnityWebRequest _unpackFileRequest;

        /// <summary>
        /// ab包加载请求
        /// </summary>
        AssetBundleCreateRequest _request;

        /// <summary>
        /// 是否需要解压
        /// </summary>
        internal bool isNeedUnpack;

        /// <summary>
        /// AssetBundle文件读取和保存位置
        /// </summary>
        string _savePath;

        /// <summary>
        /// 加密文件流
        /// </summary>
        CryptoAssetBundleStream _stream;

        protected override void OnLoad()
        {
            string nameWithHash = bundleInfo.NameWithHash;

            if (AssetManagement.isOfflineWindows)
            {
                // Windows端的离线模式不用移动到外部目录, 直接加载即可
                isNeedUnpack = false;
                _savePath = AssetPath.TranslateToLocalDataPath(nameWithHash);
            }
            else
                _savePath = AssetPath.TranslateToDownloadDataPath(nameWithHash);

            if (!isNeedUnpack)
            {
                // 已下载的直接读取
                address = _savePath; // 此处address赋值为错误时提供打印用
                _request = BundleHandler.LoadAssetBundleFromStreamAsync(_savePath, bundleInfo, out _stream);
            }
            else
            {
                // 从StreamingAssets目录解压到下载目录再进行加载, 需URL格式
                address = AssetPath.TranslateToLocalDataPathUrl(nameWithHash);
                _unpackFileRequest = UnityWebRequest.Get(address);
                _unpackFileRequest.downloadHandler = new DownloadHandlerFile(_savePath);
                _unpackFileRequest.SendWebRequest();
                Status = LoadableStatus.Unpacking;
            }
        }

        protected override void OnLoadImmediately()
        {
            if (_unpackFileRequest != null)
            {
                // 同步加载时while循环等待到解压结束
                while (!_unpackFileRequest.isDone)
                {
                }

                if (!OnUnpackCompleted())
                    return;
            }

            // 异步加载过程中(即request.isDone = false时)直接访问request.assetBundle, 会立即变成同步加载并返回加载的ab包
            // 文档:https://docs.unity.cn/cn/current/ScriptReference/AssetBundleCreateRequest-assetBundle.html
            OnBundleLoaded(_request.assetBundle);
            _request = null;
        }

        /// <summary>
        /// 解压完成处理
        /// </summary>
        /// <returns>返回处理是否成功</returns>
        bool OnUnpackCompleted()
        {
            if (!string.IsNullOrEmpty(_unpackFileRequest.error))
            {
                Finish($"从StreamingAsset中解压到下载目录失败:{_unpackFileRequest.error}");
                _unpackFileRequest.Dispose();
                _unpackFileRequest = null;
                return false;
            }

            _unpackFileRequest.Dispose();
            _unpackFileRequest = null;
            _request = BundleHandler.LoadAssetBundleFromStreamAsync(_savePath, bundleInfo, out _stream);
            return true;
        }

        protected override void OnUpdate()
        {
            switch (Status)
            {
                case LoadableStatus.Unpacking:
                    OnUnpackingUpdate();
                    break;
                case LoadableStatus.Loading:
                    OnLoadingUpdate();
                    break;
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

        /// <summary>
        /// 解压状态刷新
        /// </summary>
        void OnUnpackingUpdate()
        {
            Progress = _unpackFileRequest.downloadProgress * UnpackProportion;

            if (_unpackFileRequest.isDone && OnUnpackCompleted())
                Status = LoadableStatus.Loading;
        }

        /// <summary>
        /// 加载状态刷新
        /// </summary>
        void OnLoadingUpdate()
        {
            if (_request == null)
            {
                Finish("LoadFromStreamAsync返回的request为空, 请检查Bundle文件是否正确");
                return;
            }

            if (!isNeedUnpack)
                Progress = _request.progress;
            else
                Progress = UnpackProportion + _request.progress * (1 - UnpackProportion);

            if (_request.isDone)
            {
                OnBundleLoaded(_request.assetBundle);
                _request = null;
            }
        }
    }
}