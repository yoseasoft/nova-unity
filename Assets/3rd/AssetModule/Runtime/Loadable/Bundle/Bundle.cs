using UnityEngine;

namespace AssetModule
{
    /// <summary>
    /// 资源包对象基类, 包含资源包的信息和持有一个其对应的ab包
    /// </summary>
    public class Bundle : Loadable
    {
        /// <summary>
        /// 包信息
        /// </summary>
        public ManifestBundleInfo bundleInfo;

        /// <summary>
        /// 资源包对应的ab包
        /// </summary>
        public AssetBundle assetBundle;

        /// <summary>
        /// ab包加载完成时由子类调用
        /// </summary>
        /// <param name="bundle"></param>
        protected void OnBundleLoaded(AssetBundle bundle)
        {
            assetBundle = bundle;
            Finish(assetBundle == null ? "ab包为空？？" : null);
            if (assetBundle)
                BundleHandler.AddAssetBundleRecord(bundleInfo.Name, assetBundle);
        }

        protected override void OnUnload()
        {
            BundleHandler.RemoveCache(bundleInfo.NameWithHash);

            if (!assetBundle)
                return;

            assetBundle.Unload(true);
            assetBundle = null;
            BundleHandler.RemoveAssetBundleRecord(bundleInfo.Name);
        }
    }
}