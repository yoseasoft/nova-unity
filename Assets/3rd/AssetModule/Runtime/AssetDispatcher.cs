using UnityEngine;

namespace AssetModule
{
    /// <summary>
    /// 资源管理调度器
    /// </summary>
    public sealed class AssetDispatcher : MonoBehaviour
    {
        /// <summary>
        /// 进入游戏时先找到或创建Updater
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            AssetDispatcher dispatcher = FindObjectOfType<AssetDispatcher>();
            if (dispatcher)
                return;

            dispatcher = new GameObject(typeof(AssetDispatcher).Name).AddComponent<AssetDispatcher>();
            DontDestroyOnLoad(dispatcher);
        }

        /// <summary>
        /// 调度实例
        /// </summary>
        public static AssetDispatcher Instance { get; private set; }

        /// <summary>
        /// 每次Update最大时间, 大于等于此时间视为繁忙, 不再做处理
        /// </summary>
        static float BusyTime => UnityEngine.Application.backgroundLoadingPriority != ThreadPriority.High ? 0.01f : 0.06f;

        /// <summary>
        /// 当前时间
        /// </summary>
        float _curTime;

        /// <summary>
        /// 程序是否繁忙(用于资源加载分帧处理, 保证流畅)
        /// </summary>
        public bool IsBusy => Time.realtimeSinceStartup - _curTime >= BusyTime;

        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            _curTime = Time.realtimeSinceStartup;

            LoadableHandler.UpdateAllLoadables();
            OperationHandler.UpdateAllOperations();
            DownloadHandler.Update();
        }

        void OnDestroy()
        {
            Clear();
        }

        /// <summary>
        /// 清理所有下载和加载内容
        /// </summary>
        public void Clear()
        {
            DownloadHandler.ClearAllDownloads();
            LoadableHandler.ClearAllLoadables();
        }
    }
}