using UnityEngine;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 资源依赖加载
    /// </summary>
    public sealed class Dependency : Loadable
    {
        /// <summary>
        /// 资源所在的ab包
        /// </summary>
        public AssetBundle AssetBundle => _bundleList.Count > 0 ? _bundleList[0].assetBundle : null;

        /// <summary>
        /// 资源包对象列表(包含其所在的资源包和所有依赖的资源包);
        /// 规定第一个(index = 0)为主资源包(即资源本身所在的资源包)
        /// </summary>
        readonly List<Bundle> _bundleList = new();

        /// <summary>
        /// 需加载的资源包数量
        /// </summary>
        internal int BundleCount => _bundleList.Count;

        protected override void OnLoad()
        {
            if (!ManifestHandler.GetMainBundleInfoAndDependencies(address, out var bundleInfo, out var dependentBundleInfoList))
            {
                Finish("清单中没有此资源");
                return;
            }

            if (bundleInfo == null)
            {
                Finish("获取资源包信息失败");
                return;
            }

            _bundleList.Add(BundleHandler.LoadAsync(bundleInfo));

            if (dependentBundleInfoList is { Length: > 0 })
                foreach (ManifestBundleInfo dependentBundleInfo in dependentBundleInfoList)
                    _bundleList.Add(BundleHandler.LoadAsync(dependentBundleInfo));
        }

        protected override void OnLoadImmediately()
        {
            foreach (Bundle bundle in _bundleList)
                bundle.LoadImmediately();
        }

        protected override void OnUpdate()
        {
            if (Status != LoadableStatus.Loading)
                return;

            bool allDone = true;
            float totalProgress = 0;

            foreach (Bundle bundle in _bundleList)
            {
                totalProgress += bundle.Progress;

                if (!bundle.IsDone)
                    allDone = false;
                else if (bundle.HasError)
                {
                    Finish(bundle.Error);
                    return;
                }
            }

            if (allDone)
            {
                Finish(AssetBundle ? null : "依赖加载失败");
                return;
            }

            Progress = totalProgress / _bundleList.Count;
        }

        protected override void OnUnload()
        {
            DependencyHandler.RemoveCache(address);

            if (_bundleList.Count == 0)
                return;

            foreach (Bundle bundle in _bundleList)
                bundle.Release();

            _bundleList.Clear();
        }
    }
}