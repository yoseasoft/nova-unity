using System;
using UnityEngine;

namespace AssetModule
{
    /// <summary>
    /// ab包里的资源加载
    /// </summary>
    public sealed class BundledAsset : Asset
    {
        /// <summary>
        /// 依赖
        /// </summary>
        Dependency _dependency;

        /// <summary>
        /// ab包加载资源请求
        /// </summary>
        AssetBundleRequest _request;

        /// <summary>
        /// 单个包加载占比
        /// </summary>
        private const float SingleBundleLoadProportion = 0.1f;

        /// <summary>
        /// 依赖加载最大占比(因ab包加载速度较快, 所以最多占40%, 剩余60%占比留给AssetBundle.LoadAssetAsync())
        /// </summary>
        private const float DependencyLoadMaxProportion = 0.4f;

        protected override void OnLoad()
        {
            _dependency = DependencyHandler.LoadAsync(address);
            Status = LoadableStatus.DependentLoading;
        }

        protected override void OnLoadImmediately()
        {
            if (!_dependency.IsDone)
                _dependency.LoadImmediately();

            if (!_dependency.AssetBundle)
            {
                Finish("资源所在的ab包加载失败");
                return;
            }

            OnAssetLoaded(_dependency.AssetBundle.LoadAsset(address, type));
        }

        protected override void OnUpdate()
        {
            switch (Status)
            {
                case LoadableStatus.DependentLoading:
                    OnDependentLoadingUpdate();
                    break;
                case LoadableStatus.Loading:
                    OnLoadingUpdate();
                    break;
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _dependency?.Release();
            result = null;
            _request = null;
            _dependency = null;
        }

        /// <summary>
        /// 依赖加载占比计算
        /// </summary>
        float DependencyProportion => Mathf.Min(DependencyLoadMaxProportion, SingleBundleLoadProportion * _dependency.BundleCount);

        /// <summary>
        /// 依赖加载Update处理
        /// </summary>
        void OnDependentLoadingUpdate()
        {
            Progress = _dependency.Progress * DependencyProportion;

            if (!_dependency.IsDone)
                return;

            AssetBundle assetBundle = _dependency.AssetBundle;
            if (!assetBundle)
            {
                Finish("AssetBundle加载失败");
                return;
            }

            _request = assetBundle.LoadAssetAsync(address, type);
            Status = LoadableStatus.Loading;
        }

        /// <summary>
        /// 资源加载Update处理
        /// </summary>
        void OnLoadingUpdate()
        {
            /*
            后台高速加载时直接使用同步加载资源
            (为何要这样处理？
            同时异步加载数量太多时(通常在Loading界面会同时加载很多), 若此时关闭游戏, 则会出现以下报错:
            AssetBundle.unload could not complete because the asset bundle still has an async load operation in progress.
            而Unity又没有提供可以取消AssetBundleRequest的接口, 约定在设置Application.backgroundLoadingPriority = High后(Loading界面可这样设置), 直接同步加载;
            曾试过在OnUnload中先访问asset(即切换为同步加载), 但Unity崩溃了, 没有深究原因, 但可能因为Stream加载在应用退出那个时间点导致, 所以最后选择这种办法)
            (注意：此种处理方式仅可解决资源集中预加载的情况(即最容易出现报错而且是大量错误的情况),
            平时游戏进行中的不可避免, 但游戏进行时本身就很难卡到那个点关游戏, 就算卡到了可能就一两个, 而且报错后几乎没影响(毕竟都关游戏了), 可以忽略)
            */
            if (_request.isDone || UnityEngine.Application.backgroundLoadingPriority == ThreadPriority.High)
            {
                // 若request未完成, 直接访问asset视为取消异步加载, 直接同步获取加载结果
                // 参考:https://docs.unity.cn/cn/2021.3/ScriptReference/AssetBundleRequest-asset.html
                OnAssetLoaded(_request.asset);
                _request = null;
            }
            else
                Progress = DependencyProportion + (1 - DependencyProportion) * _request.progress;
        }
    }
}