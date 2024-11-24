using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace AssetModule
{
    /// <summary>
    /// Scene对象管理
    /// </summary>
    public static class SceneHandler
    {
        /// <summary>
        /// 创建场景对象方法, 可进行自定义覆盖, 默认为DefaultCreateSceneFunc
        /// </summary>
        public static Func<string, bool, Scene> CreateSceneFunc { get; set; } = DefaultCreateSceneFunc;

        /// <summary>
        /// 当前激活的主场景对象
        /// </summary>
        static Scene s_mainScene;

        /// <summary>
        /// 进行中的异步操作列表
        /// </summary>
        static readonly List<AsyncOperation> s_runningAsyncOperationList = new();

        /// <summary>
        /// 是否有加载中或卸载中的场景
        /// </summary>
        public static bool HasLoadingOrUnloadingScene => s_runningAsyncOperationList.Count > 0;

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="address">资源地址(名字或路径)</param>
        /// <param name="isAdditive">是否使用叠加方式加载</param>
        internal static Scene Load(string address, bool isAdditive = false)
        {
            Scene scene = CreateSceneAndReleaseCurrentMainScene(address, isAdditive);
            scene.isSyncLoad = true;
            scene.Load();
            return scene;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="address">资源地址(名字或路径)</param>
        /// <param name="isAdditive">是否使用叠加方式加载</param>
        /// <param name="completed">加载完成回调</param>
        internal static Scene LoadAsync(string address, bool isAdditive = false, Action<Scene> completed = null)
        {
            Scene scene = CreateSceneAndReleaseCurrentMainScene(address, isAdditive);
            if (completed != null)
                scene.completed += completed;
            scene.Load();
            return scene;
        }

        /// <summary>
        /// 使用Unity接口异步加载场景
        /// </summary>
        internal static AsyncOperation UnityLoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            s_runningAsyncOperationList.Add(asyncOperation);
            return asyncOperation;
        }

        /// <summary>
        /// 使用Unity接口异步卸载场景
        /// </summary>
        internal static AsyncOperation UnityUnloadSceneAsync(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            s_runningAsyncOperationList.Add(asyncOperation);
            return asyncOperation;
        }

        /// <summary>
        /// 刷新异步操作列表并获取是否有场景正在加载或正在卸载
        /// 小道消息:AsyncOperation.completed有低概率不触发(但没有找到相应的issue链接), 所以还是Update判断isDone再移除
        /// </summary>
        internal static bool UpdateAsyncOperationListAndGetHasLoadingOrUnloadingScene()
        {
            if (s_runningAsyncOperationList.Count == 0)
                return false;

            for (int i = s_runningAsyncOperationList.Count - 1; i >= 0; i--)
                if (s_runningAsyncOperationList[i].isDone)
                    s_runningAsyncOperationList.RemoveAt(i);

            return s_runningAsyncOperationList.Count > 0;
        }

        /// <summary>
        /// 创建场景并对基本参数赋值, 释放当前场景对象
        /// </summary>
        static Scene CreateSceneAndReleaseCurrentMainScene(string address, bool isAdditive)
        {
            string assetPath = AssetPath.GetActualPath(address);
            Scene newScene = CreateSceneFunc(assetPath, isAdditive);
            newScene.sceneName = Path.GetFileNameWithoutExtension(assetPath);
            if (!isAdditive)
            {
                s_mainScene?.Release();
                s_mainScene = newScene;
            }
            else
            {
                if (s_mainScene != null)
                {
                    s_mainScene.additiveSceneList.Add(newScene);
                    newScene.parent = s_mainScene;
                }
            }

            return newScene;
        }

        /// <summary>
        /// 默认创建场景对象的方法
        /// </summary>
        static Scene DefaultCreateSceneFunc(string assetPath, bool isAdditive)
        {
            LoadSceneMode loadSceneMode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;

            // 清单中有的使用打包场景加载
            if (ManifestHandler.IsContainAsset(assetPath))
                return new BundledScene { address = assetPath, loadSceneMode = loadSceneMode };

            // 清单中没有的视为使用ScenesInBuild的场景加载
            return new Scene { address = assetPath, loadSceneMode = loadSceneMode };
        }
    }
}