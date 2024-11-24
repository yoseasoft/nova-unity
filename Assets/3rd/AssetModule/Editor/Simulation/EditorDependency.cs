using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace AssetModule.Editor.Simulation
{
    /// <summary>
    /// 编辑器下依赖资源记录和加载
    /// AssetDatabase.LoadAsset会自动读取依赖, 但发现加载的Prefab和场景中带有的Prefab的材质和所用贴图会在镜头第一次渲染时才会真正读取文件加载
    /// 导致虽然已经预加载了但游戏运行时还是会突然卡顿, 故如果是Prefab和场景的话会先加载依赖资源, 让预加载完美
    /// </summary>
    public class EditorDependency : Loadable
    {
        /// <summary>
        /// 资源加载的index
        /// </summary>
        int _loadingIndex;

        /// <summary>
        /// 依赖资源路径
        /// </summary>
        string[] _dependencies;

        /// <summary>
        /// 需要加载的的依赖资源路径
        /// </summary>
        List<string> _needLoadDependencyList;

        /// <summary>
        /// 依赖的资源列表
        /// </summary>
        List<Object> _dependentAssetList;

        protected override void OnLoadImmediately()
        {
            if (_dependencies == null)
                GetDependenciesAndAddReference();

            if (_needLoadDependencyList == null)
                return;

            for (int i = _loadingIndex; i < _needLoadDependencyList.Count; i++)
                _dependentAssetList.Add(AssetDatabase.LoadAssetAtPath(_needLoadDependencyList[i], typeof(Object)));

            Finish();
        }

        protected override void OnUnused()
        {
            if (_dependencies != null)
                foreach (string assetPath in _dependencies)
                    EditorAssetReference.ReleaseReference(assetPath);

            if (_dependentAssetList != null)
                for (int i = 0; i < _dependentAssetList.Count; i++)
                    if (!EditorAssetReference.IsUsing(_needLoadDependencyList[i]) && _dependentAssetList[i])
                        Resources.UnloadAsset(_dependentAssetList[i]);

            _dependentAssetList = null;
            _needLoadDependencyList = null;
        }

        protected override void OnUpdate()
        {
            if (Status != LoadableStatus.Loading)
                return;

            if (_dependencies == null)
            {
                GetDependenciesAndAddReference();
                return;
            }

            for (int i = _loadingIndex; i < _needLoadDependencyList.Count; i++)
            {
                _dependentAssetList.Add(AssetDatabase.LoadAssetAtPath(_needLoadDependencyList[i], typeof(Object)));
                _loadingIndex++;

                if (AssetDispatcher.Instance.IsBusy)
                    break;
            }

            if (_loadingIndex == _needLoadDependencyList.Count)
                Finish();
        }

        /// <summary>
        /// 获取依赖,添加引用并计算需要加载的资源
        /// </summary>
        void GetDependenciesAndAddReference()
        {
            _dependencies = AssetDatabase.GetDependencies(address); // 此接口会包含资源本身

            // 添加引用记录时包含资源本身
            foreach (string assetPath in _dependencies)
                EditorAssetReference.AddReference(assetPath);

            // Prefab和场景才预加载其依赖, 其他资源仅添加记录, 资源依赖仅有一个时就是其自身, 也不进行其他处理
            if (!(address.EndsWith(".prefab") || address.EndsWith(".unity")) || _dependencies.Length == 1)
            {
                Finish();
                return;
            }

            _needLoadDependencyList = new List<string>();

            // 只预加载材质和贴图
            foreach (string assetPath in _dependencies)
                if (assetPath.EndsWith(".mat") || assetPath.EndsWith(".png") || assetPath.EndsWith(".tga") || assetPath.EndsWith(".psd") || assetPath.EndsWith(".jpg"))
                    _needLoadDependencyList.Add(assetPath);

            if (_needLoadDependencyList.Count == 0)
            {
                _needLoadDependencyList = null;
                Finish();
                return;
            }

            _dependentAssetList = new List<Object>(_needLoadDependencyList.Count);
        }
    }
}