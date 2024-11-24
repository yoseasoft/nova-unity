using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace AssetModule
{
    /// <summary>
    /// 场景对象基类
    /// 也可直接使用此类进行加载, 若用此类直接加载视为加载ScenesInBuild下的场景
    /// </summary>
    public class Scene : Loadable, IEnumerator
    {
        /// <summary>
        /// 是否同步加载
        /// </summary>
        public bool isSyncLoad;

        /// <summary>
        /// 场景名字
        /// </summary>
        public string sceneName;

        /// <summary>
        /// 叠加场景所在的主场景
        /// (此字段仅叠加场景使用)
        /// </summary>
        internal Scene parent;

        /// <summary>
        /// 主场景拥有的叠加场景列表
        /// (此字段仅主场景使用)
        /// </summary>
        internal readonly List<Scene> additiveSceneList = new();

        /// <summary>
        /// 完成回调
        /// </summary>
        public Action<Scene> completed;

        /// <summary>
        /// 加载场景的异步操作
        /// </summary>
        public AsyncOperation asyncOperation;

        /// <summary>
        /// 加载场景方式
        /// </summary>
        public LoadSceneMode loadSceneMode;

        /// <summary>
        /// 提供给await使用
        /// </summary>
        public Task<Scene> Task
        {
            get
            {
                TaskCompletionSource<Scene> tcs = new();
                completed += _ => tcs.SetResult(this);
                return tcs.Task;
            }
        }

        protected override void OnLoad()
        {
            if (!isSyncLoad)
                asyncOperation = SceneHandler.UnityLoadSceneAsync(sceneName, loadSceneMode);
            else
            {
                SceneManager.LoadScene(sceneName, loadSceneMode);
                Finish();
            }
        }

        protected override void OnUpdate()
        {
            if (Status != LoadableStatus.Loading)
                return;

            Progress = asyncOperation.allowSceneActivation ? asyncOperation.progress : asyncOperation.progress / 0.9f;

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

        protected override void OnComplete()
        {
            if (completed == null)
                return;

            var func = completed;
            completed = null;
            func?.Invoke(this);
        }

        protected override void OnUnused()
        {
            completed = null;
        }

        protected override void OnUnload()
        {
            if (loadSceneMode == LoadSceneMode.Single)
            {
                foreach (Scene addtiveScene in additiveSceneList)
                {
                    addtiveScene.Release();
                    addtiveScene.parent = null;
                }

                additiveSceneList.Clear();

                // Single模式场景不需卸载, 切换另一个Single场景时会自动卸载
                // 强行卸载最后一个场景会报Warning:Unloading the last loaded scene Assets/Scenes/SampleScene.unity(build index: 0), is not supported. Please use SceneManager.LoadScene()/EditorSceneManager.OpenScene() to switch to another scene.
            }
            else
            {
                parent?.RemoveAdditiveScene(this);
                parent = null;

                if (!HasError) // 有错误代表没有加载成功, 不再卸载
                    SceneHandler.UnityUnloadSceneAsync(sceneName);
            }
        }

        /// <summary>
        /// 移除叠加场景记录
        /// </summary>
        void RemoveAdditiveScene(Scene scene)
        {
            additiveSceneList.Remove(scene);
        }

        #region IEnumerator

        public object Current => null;

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
        }

        #endregion
    }
}