using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace AssetModule.Editor.Simulation
{
    /// <summary>
    /// 编辑器下资源加载
    /// </summary>
    public class EditorAsset : Asset
    {
        /// <summary>
        /// 依赖
        /// </summary>
        EditorDependency _dependency;

        /// <summary>
        /// 创建EditorAsset
        /// </summary>
        internal static EditorAsset Create(string assetPath, Type type)
        {
            if (!File.Exists(assetPath))
            {
                Debug.LogError($"资源不存在{assetPath}");
                return null;
            }

            return new EditorAsset { address = assetPath, type = type };
        }

        protected override void OnLoad()
        {
            _dependency = new EditorDependency { address = address };
            _dependency.Load();
        }

        protected override void OnLoadImmediately()
        {
            _dependency.LoadImmediately();
            OnAssetLoaded(AssetDatabase.LoadAssetAtPath(address, type));
        }

        protected override void OnUpdate()
        {
            if (Status == LoadableStatus.Loading && _dependency.IsDone)
                OnAssetLoaded(AssetDatabase.LoadAssetAtPath(address, type));
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _dependency.Release();
            _dependency = null;

            if (!result)
                return;

            // Resources.UnloadAsset仅能释放非GameObject和Component的资源，比如Texture、Mesh等真正的资源。对于由Prefab加载出来的Object或Component，则不能通过该函数来进行释放。
            // UnloadAsset may only be used on individual assets and can not be used on GameObject's/Components or AssetBundles
            if (!EditorAssetReference.IsUsing(address) && result is not GameObject)
                Resources.UnloadAsset(result);

            result = null;
        }
    }
}