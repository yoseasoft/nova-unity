using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 资源清单
    /// </summary>
    public class Manifest : ScriptableObject
    {
        /// <summary>
        /// 清单所在的文件名(带Hash,即保持唯一)
        /// </summary>
        internal string fileName;

        /// <summary>
        /// 资源包信息列表, 需public, 由Json覆盖写入
        /// </summary>
        public List<ManifestBundleInfo> manifestBundleInfoList = new();

        /// <summary>
        /// 资源包名字和资源包的对照字典
        /// </summary>
        Dictionary<string, ManifestBundleInfo> _nameToBundleInfo = new();

        /// <summary>
        /// 资源的真实路径和资源包的对照字典
        /// </summary>
        Dictionary<string, ManifestBundleInfo> _assetPathToBundleInfo = new();

        /// <summary>
        /// 根据新清单对象覆盖清单原有配置
        /// </summary>
        public void OverrideMainifest(Manifest manifest)
        {
            manifestBundleInfoList = manifest.manifestBundleInfoList;
            _nameToBundleInfo = manifest._nameToBundleInfo;
            _assetPathToBundleInfo = manifest._assetPathToBundleInfo;
        }

        /// <summary>
        /// 重载字典记录
        /// </summary>
        public void Reload()
        {
            _nameToBundleInfo.Clear();
            _assetPathToBundleInfo.Clear();

            foreach (ManifestBundleInfo bundleInfo in manifestBundleInfoList)
            {
                _nameToBundleInfo[bundleInfo.Name] = bundleInfo;
                foreach (string path in bundleInfo.AssetPathList)
                {
                    _assetPathToBundleInfo[path] = bundleInfo;
                    AssetPath.RecordCustomLoadPath(path);
                }
            }
        }

        /// <summary>
        /// 判断此清单是否包含某个资源
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        public bool IsContainAsset(string assetPath)
        {
            return _assetPathToBundleInfo.ContainsKey(assetPath);
        }

        /// <summary>
        /// 根据Bundle名称获取该包的信息
        /// </summary>
        /// <param name="bundleName">Bundle名字</param>
        public ManifestBundleInfo GetBundleInfoByBundleName(string bundleName)
        {
            return _nameToBundleInfo.GetValueOrDefault(bundleName);
        }

        /// <summary>
        /// 根据资源的真实路径获取资源所在的包的信息
        /// </summary>
        /// <param name="assetPath">资源的真实路径</param>
        public ManifestBundleInfo GetBundleInfo(string assetPath)
        {
            return _assetPathToBundleInfo.GetValueOrDefault(assetPath);
        }

        /// <summary>
        /// 空清单包信息列表
        /// </summary>
        static readonly ManifestBundleInfo[] EmptyBundleInfoList = Array.Empty<ManifestBundleInfo>();

        /// <summary>
        /// 根据清单包信息获取该包的依赖包信息列表
        /// </summary>
        public ManifestBundleInfo[] GetDependentBundleInfoList(ManifestBundleInfo bundle)
        {
            return bundle?.DependentBundleIDList == null ? EmptyBundleInfoList : Array.ConvertAll(bundle.DependentBundleIDList, index => manifestBundleInfoList[index]);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 空清单包信息对象
        /// </summary>
        static readonly ManifestBundleInfo EmptyBundleInfo = new();

        /// <summary>
        /// 记录资源路径, 但使用空的清单包信息
        /// (主要在编辑器模拟运行时使用, 因加载资源时需判断IsContainAsset(assetPath), 所以此处记录后就可以通过检测)
        /// (为什么可以使用空的打包信息？ 因模拟运行时不会使用打包文件加载, 会直接使用AssetDatabase.LoadAssetAtPath()加载(可查看EditorAsset.cs), 所以不需要真正的打包信息)
        /// </summary>
        public void RecordAssetButUseEmptyBundleInfo(string assetPath, ManifestBundleInfo manifestBundleInfo = null)
        {
            _assetPathToBundleInfo[assetPath] = manifestBundleInfo ?? EmptyBundleInfo;
            AssetPath.RecordCustomLoadPath(assetPath);
        }
#endif
    }
}