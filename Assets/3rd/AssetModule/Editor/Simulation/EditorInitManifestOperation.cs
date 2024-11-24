using UnityEngine;
using AssetModule.Editor.Build;
using System.Collections.Generic;

namespace AssetModule.Editor.Simulation
{
    /// <summary>
    /// 编辑器下清单初始化
    /// </summary>
    public class EditorInitManifestOperation : InitManifestOperation
    {
        /// <summary>
        /// 创建编辑器初始化清单操作方法
        /// </summary>
        public static EditorInitManifestOperation Create()
        {
            return new EditorInitManifestOperation();
        }

        protected override void OnStart()
        {
            foreach (ManifestConfig config in BuildUtils.GetAllManifestConfigs())
            {
                Manifest manifest = ScriptableObject.CreateInstance<Manifest>();
                manifest.name = config.name;
                foreach (Group group in config.groups)
                {
                    if (!group.IsNeedBuild)
                        continue;

                    foreach (BuildAssetInfo assetInfo in group.CollectAssets())
                    {
                        if (assetInfo.isExternalFile)
                        {
                            // 外部原始文件放入加载路径，提供给EditorRawFile加载
                            string loadPath = BuildUtils.GetExternalRawFileLoadPath(assetInfo.path, assetInfo.originalExternalPath, assetInfo.placeFolderName);
                            ManifestBundleInfo bundleInfo = new ManifestBundleInfo
                            {
                                Name = assetInfo.path,
                                AssetPathList = new List<string> { loadPath }
                            };
                            manifest.RecordAssetButUseEmptyBundleInfo(loadPath, bundleInfo);
                        }
                        else
                            manifest.RecordAssetButUseEmptyBundleInfo(assetInfo.path);
                    }
                }

                ManifestHandler.RefreshGlobalManifest(manifest);
            }

            Finish();
        }

        protected override void OnUpdate()
        {
            // do nothing
        }
    }
}