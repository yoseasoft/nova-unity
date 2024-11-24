using UnityEngine;

namespace AssetModule
{
    /// <summary>
    /// 本地资源包对象
    /// </summary>
    public sealed class LocalBundle : Bundle
    {
        /// <summary>
        /// ab包加载请求
        /// </summary>
        AssetBundleCreateRequest _request;

        protected override void OnLoad()
        {
            _request = AssetBundle.LoadFromFileAsync(address, 0, (ulong)BundleHandler.BundleOffset);
        }

        protected override void OnLoadImmediately()
        {
            // 异步加载过程中(即request.isDone = false时)直接访问request.assetBundle, 会立即变成同步加载并返回加载的ab包
            // 文档:https://docs.unity.cn/cn/current/ScriptReference/AssetBundleCreateRequest-assetBundle.html
            OnBundleLoaded(_request.assetBundle);
            _request = null;
        }

        protected override void OnUpdate()
        {
            if (Status != LoadableStatus.Loading)
                return;

            Progress = _request.progress;

            if (_request.isDone)
            {
                OnBundleLoaded(_request.assetBundle);
                _request = null;
            }
        }
    }
}