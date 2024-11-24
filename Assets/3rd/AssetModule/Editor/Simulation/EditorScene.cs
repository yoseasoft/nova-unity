using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace AssetModule.Editor.Simulation
{
    /// <summary>
    /// 编辑器下场景加载
    /// </summary>
    public class EditorScene : Scene
    {
        /// <summary>
        /// 依赖
        /// </summary>
        EditorDependency _dependency;

        /// <summary>
        /// 创建EditorScene
        /// </summary>
        internal static EditorScene Create(string assetPath, bool isAdditive)
        {
            if (!File.Exists(assetPath))
            {
                Debug.LogError($"场景文件不存在:{assetPath}");
                return null;
            }

            if (!ManifestHandler.IsContainAsset(assetPath))
            {
                Debug.LogError($"场景加载失败, 因所有资源清单中都不存在此资源:{assetPath}");
                return null;
            }

            LoadSceneMode loadSceneMode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            return new EditorScene { address = assetPath, loadSceneMode = loadSceneMode };
        }

        protected override void OnLoad()
        {
            _dependency = new EditorDependency { address = address };
            _dependency.Load();

            LoadSceneParameters loadSceneParameters = new LoadSceneParameters { loadSceneMode = loadSceneMode };
            if (isSyncLoad)
            {
                _dependency.LoadImmediately();
                EditorSceneManager.LoadSceneInPlayMode(address, loadSceneParameters);
                Finish();
            }
            else
                asyncOperation = EditorSceneManager.LoadSceneAsyncInPlayMode(address, loadSceneParameters);
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

            if (!_dependency.IsDone)
                return;

            Finish();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _dependency.Release();
            _dependency = null;
        }
    }
}