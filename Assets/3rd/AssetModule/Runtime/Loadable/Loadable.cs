using System.IO;

namespace AssetModule
{
    /// <summary>
    /// 可加载对象基类
    /// </summary>
    public class Loadable
    {
        /// <summary>
        /// 引用计数
        /// </summary>
        public readonly Reference reference = new();

        /// <summary>
        /// 当前加载状态
        /// </summary>
        public LoadableStatus Status { get; protected set; } = LoadableStatus.Init;

        /// <summary>
        /// 资源地址(本地路径或网络url)
        /// </summary>
        public string address;

        /// <summary>
        /// 错误原因
        /// </summary>
        public string Error { get; protected set; }

        /// <summary>
        /// 当前是否存在错误
        /// </summary>
        public bool HasError => !string.IsNullOrEmpty(Error);

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsDone => Status == LoadableStatus.LoadSuccessful || Status == LoadableStatus.LoadFailed || Status == LoadableStatus.Unloaded;

        /// <summary>
        /// 加载进度(取值范围:0~1)
        /// </summary>
        public float Progress { get; protected set; }

#if UNITY_EDITOR
        /// <summary>
        /// 是否正在退出播放模式
        /// </summary>
        static bool s_isExitingPlayMode;

        /// <summary>
        /// Unity初始化完成后添加播放模式改变监听
        /// </summary>
        [UnityEditor.InitializeOnLoadMethod]
        static void AddPlayModeStateChangedListener()
        {
            UnityEditor.EditorApplication.playModeStateChanged += state => { s_isExitingPlayMode = state == UnityEditor.PlayModeStateChange.ExitingPlayMode; };
        }
#endif

        /// <summary>
        /// 加载, 加引用
        /// </summary>
        public void Load()
        {
            // 已不使用时再加载, 就从无用列表中移出
            if (Status != LoadableStatus.Init && reference.IsUnused)
                LoadableHandler.RemoveUnused(this);

            reference.Increase();
            LoadableHandler.AddLoadable(this);

            if (Status != LoadableStatus.Init)
                return;

            Logger.Info($"开始加载 {GetType().Name} {Path.GetFileName(address)}");
            Status = LoadableStatus.Loading;
            Progress = 0;
            OnLoad();
        }

        /// <summary>
        /// 让当前加载立刻加载完
        /// 基类需重写OnLoadImmediately方法实现
        /// </summary>
        public void LoadImmediately()
        {
            if (Status == LoadableStatus.Init)
            {
                Logger.Warning("请确保已调用Load后再调用LoadImmediately");
                return;
            }

            if (!IsDone)
                OnLoadImmediately();
        }

        /// <summary>
        /// 立刻加载完(让当前加载切换成同步加载, 基类需重写此方法实现)
        /// </summary>
        protected virtual void OnLoadImmediately()
        {
            Logger.Error($"{GetType().Name}并不支持同步加载, 请检查调用");
        }

        internal void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// 完成加载(加载成功和失败都需由基类调用此接口)
        /// </summary>
        /// <param name="errorCode">加载失败原因</param>
        protected void Finish(string errorCode = null)
        {
            Error = errorCode;
            Status = string.IsNullOrEmpty(errorCode) ? LoadableStatus.LoadSuccessful : LoadableStatus.LoadFailed;
            Progress = 1;
        }

        /// <summary>
        /// 完成通知, 基类调用Finish后, Complete就会被Handler调用
        /// </summary>
        internal void Complete()
        {
            if (Status == LoadableStatus.LoadFailed)
            {
                FullyRelease(); // 加载失败时自动释放所有引用

                if (!string.IsNullOrEmpty(Error))
                    Logger.Error($"加载{GetType().Name}失败, 资源地址:{address}, 原因:{Error}");
            }

            if (Status == LoadableStatus.LoadSuccessful)
                Logger.Info($"{GetType().Name}:{address}加载完成");

            OnComplete();
        }

        /// <summary>
        /// 释放, 减引用
        /// </summary>
        public void Release()
        {
            if (reference.IsUnused)
            {
#if UNITY_EDITOR
                // 正在退出导致的重复释放不打印, 因可能每个资源手动释放后再调用全部释放
                if (s_isExitingPlayMode)
                    return;
#endif

                // 有错误后触发的重复释放不打印Warning(因为加载错误时会自动释放资源, 这时再手动释放就会触发重复释放)
                if (!HasError)
                    Logger.Warning($"已释放资源{GetType().Name}, 资源地址:{address}, 请检查重复调用的原因");
                return;
            }

            reference.Decrease();

            if (reference.IsUnused)
            {
                LoadableHandler.AddUnused(this);
                OnUnused();
            }
        }

        /// <summary>
        /// 完全释放引用
        /// </summary>
        internal void FullyRelease()
        {
            reference.Reset();
            LoadableHandler.AddUnused(this);
            OnUnused();
        }

        /// <summary>
        /// 卸载通知(通常不被引用后就会被调用Unload)
        /// </summary>
        internal void Unload()
        {
            if (Status == LoadableStatus.Unloaded)
                return;

            Logger.Info($"卸载 {GetType().Name} {address}");
            OnUnload();
            Status = LoadableStatus.Unloaded;
        }

        // -------------------- 子类方法 --------------------

        /// <summary>
        /// 加载通知, 由子类继承实现
        /// </summary>
        protected virtual void OnLoad()
        {
        }

        /// <summary>
        /// Update通知, 由子类继承实现
        /// </summary>
        protected virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 加载完成通知, 由子类继承实现
        /// </summary>
        protected virtual void OnComplete()
        {
        }

        /// <summary>
        /// 不再被引用通知, 由子类继承实现
        /// </summary>
        protected virtual void OnUnused()
        {
        }

        /// <summary>
        /// 卸载通知, 由子类继承实现
        /// </summary>
        protected virtual void OnUnload()
        {
        }
    }
}