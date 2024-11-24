using UnityEngine.SceneManagement;

namespace AssetModule
{
    /// <summary>
    /// ab包里的场景加载
    /// </summary>
    public sealed class BundledScene : Scene
    {
        /// <summary>
        /// ab包相关依赖
        /// </summary>
        Dependency _dependency;

        /// <summary>
        /// 场景依赖包加载占比, ab包加载速度较快, 所以占个30%
        /// </summary>
        const float DependencyLoadProportion = 0.3f;

        protected override void OnLoad()
        {
            _dependency = DependencyHandler.LoadAsync(address);
            if (isSyncLoad)
            {
                _dependency.LoadImmediately();
                SceneManager.LoadScene(sceneName, loadSceneMode);
                Finish();
            }
            else
                Status = LoadableStatus.DependentLoading;
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
            _dependency = null;
        }

        /// <summary>
        /// 依赖资源包加载中Update处理
        /// </summary>
        void OnDependentLoadingUpdate()
        {
            Progress = DependencyLoadProportion * _dependency.Progress;

            if (!_dependency.IsDone)
                return;

            if (!_dependency.AssetBundle)
            {
                Finish($"加载场景时, 其ab包加载失败, address:{address}");
                return;
            }

            asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            Status = LoadableStatus.Loading;
        }

        /// <summary>
        /// 加载Update处理
        /// </summary>
        void OnLoadingUpdate()
        {
            float loadProgress = asyncOperation.allowSceneActivation ? asyncOperation.progress : asyncOperation.progress / 0.9f;
            Progress = DependencyLoadProportion + (1 - DependencyLoadProportion) * loadProgress;

            if (asyncOperation.allowSceneActivation)
            {
                if (!asyncOperation.isDone)
                    return;
            }
            else
            {
                // 不允许场景自动激活时会停在0.9(但实际已完成加载), 直至设置allowSceneActivation = true
                // 文档:https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
                if (asyncOperation.progress < 0.9f)
                    return;
            }

            Finish();
        }
    }
}