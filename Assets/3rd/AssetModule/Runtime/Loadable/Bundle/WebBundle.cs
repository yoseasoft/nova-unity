using UnityEngine;
using UnityEngine.Networking;

namespace AssetModule
{
    /// <summary>
    /// WebGL平台资源包对象
    /// </summary>
    public sealed class WebBundle : Bundle
    {
        UnityWebRequest _request;

        AsyncOperation _asyncOperation;

        protected override void OnLoad()
        {
            _request = UnityWebRequestAssetBundle.GetAssetBundle(address);
            _asyncOperation = _request.SendWebRequest();
        }

        protected override void OnUpdate()
        {
            if (Status != LoadableStatus.Loading)
                return;

            Progress = _asyncOperation.progress;
            if (!_asyncOperation.isDone)
                return;

            if (_request.result == UnityWebRequest.Result.Success)
                OnBundleLoaded(DownloadHandlerAssetBundle.GetContent(_request));
            else
                Finish(_request.error);

            DisposeRequest();
        }

        /// <summary>
        /// 销毁请求
        /// </summary>
        void DisposeRequest()
        {
            _request.Dispose();
            _request = null;
            _asyncOperation = null;
        }
    }
}