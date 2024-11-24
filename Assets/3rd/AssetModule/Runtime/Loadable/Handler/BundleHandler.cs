using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// Bundle对象管理
    /// </summary>
    public static class BundleHandler
    {
        /// <summary>
        /// ab包是否使用算法加密
        /// </summary>
        public static bool IsBundleEncrypt => Utility.CurrentPlatform != RuntimePlatform.WebGLPlayer;

        /// <summary>
        /// ab包不使用算法加密时的简单文件偏移, 0为不进行偏移
        /// </summary>
        public static int BundleOffset => Utility.CurrentPlatform != RuntimePlatform.WebGLPlayer ? 8 : 0;

        /// <summary>
        /// 流文件读取缓冲区大小(此处使用Unity默认的32KB)
        /// </summary>
        private const int StreamBufferSize = 32 * 1024;

        /// <summary>
        /// Bundle对象缓存
        /// </summary>
        public static readonly Dictionary<string, Bundle> Cache = new();

        /// <summary>
        /// 当前运行的AB包列表
        /// key:Bundle信息名字, value:ab包
        /// </summary>
        static readonly Dictionary<string, AssetBundle> RunningAssetBundleMap = new();

        /// <summary>
        /// 添加到ab包记录列表
        /// </summary>
        internal static void AddAssetBundleRecord(string bundleInfoName, AssetBundle assetBundle)
        {
            RunningAssetBundleMap[bundleInfoName] = assetBundle;
        }

        /// <summary>
        /// 移除ab包记录
        /// </summary>
        internal static void RemoveAssetBundleRecord(string bundleInfoName)
        {
            RunningAssetBundleMap.Remove(bundleInfoName);
        }

        /// <summary>
        /// 加载Bundle对象
        /// </summary>
        /// <param name="bundleInfo">Bundle信息</param>
        internal static Bundle LoadAsync(ManifestBundleInfo bundleInfo)
        {
            if (bundleInfo == null)
            {
                Logger.Error("LoadBundle传入的Bundle信息为空");
                return null;
            }

            if (!Cache.TryGetValue(bundleInfo.NameWithHash, out Bundle bundle))
            {
                // 游戏运行中手动更新了资源清单有可能导致原来的ab已经过期了, 需要加载新的ab， 故先卸载再加载
                if (RunningAssetBundleMap.TryGetValue(bundleInfo.Name, out AssetBundle assetBundle))
                {
                    assetBundle.Unload(false);
                    RunningAssetBundleMap.Remove(bundleInfo.Name);
                }

                string address = GetAssetBundleLoadPath(bundleInfo);

                if (Utility.CurrentPlatform != RuntimePlatform.WebGLPlayer)
                {
                    if (address.StartsWith("http://") || address.StartsWith("https://") || address.StartsWith("ftp://"))
                        bundle = new DownloadBundle { address = address, bundleInfo = bundleInfo };
                    else
                    {
                        if (IsBundleEncrypt)
                        {
                            bundle = new LocalEncryptBundle
                            {
                                bundleInfo = bundleInfo,
                                // 此处为了避免再计算一次文件是否存在, 直接使用加载文件记录作为判断
                                // 因加密模式下buildIn地址不会进行缓存(看方法:GetAssetBundleLoadPath)
                                // 所以若此记录存在, 则证明文件在下载目录存在
                                isNeedUnpack = !AssetBundleFileLoadPathRecord.ContainsKey(bundleInfo.NameWithHash)
                            };
                        }
                        else
                        {
                            bundle = new LocalBundle { address = address, bundleInfo = bundleInfo };
                        }
                    }
                }
                else
                {
                    // WebGL平台
                    bundle = new WebBundle { address = address, bundleInfo = bundleInfo };
                }

                Cache.Add(bundleInfo.NameWithHash, bundle);
            }

            bundle.Load();
            return bundle;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        internal static void RemoveCache(string bundleNameWithHash)
        {
            Cache.Remove(bundleNameWithHash);
        }

        /// <summary>
        /// 清除缓存并全部卸载
        /// </summary>
        internal static void ClearCache()
        {
            Bundle[] bundles = Cache.Values.ToArray();
            foreach (Bundle bundle in bundles)
            {
                bundle.FullyRelease();
                bundle.Unload();
            }

            Cache.Clear();
        }

        /// <summary>
        /// 加载文件路径记录, 记录后获取时不用每次计算
        /// </summary>
        static readonly Dictionary<string, string> AssetBundleFileLoadPathRecord = new();

        /// <summary>
        /// 获取ab包文件加载路径
        /// </summary>
        /// <param name="bundleInfo">清单资源包信息</param>
        static string GetAssetBundleLoadPath(ManifestBundleInfo bundleInfo)
        {
            string fileNameWithHash = bundleInfo.NameWithHash;
            if (AssetBundleFileLoadPathRecord.TryGetValue(fileNameWithHash, out string loadPath))
                return loadPath;

            if (IsBundleEncrypt)
            {
                // 加密模式优先读取下载目录, 因加密模式下就算首包的资源也需解压到下载目录
                if (AssetManagement.IsBundleFileAlreadyInDownloadPath(bundleInfo))
                {
                    loadPath = AssetPath.TranslateToDownloadDataPath(fileNameWithHash);
                    AssetBundleFileLoadPathRecord[fileNameWithHash] = loadPath;
                    return loadPath;
                }

                // Windows下的离线模式除外, 直接读取首包目录
                if (AssetManagement.isOfflineWindows)
                {
                    loadPath = AssetPath.TranslateToLocalDataPath(fileNameWithHash);
                    AssetBundleFileLoadPathRecord[fileNameWithHash] = loadPath;
                    return loadPath;
                }

                // 加密模式buildIn地址不进行缓存, 没缓存的本地Bundle视为需要解压
                if (AssetManagement.offlineMode || AssetManagement.IsBuildInFile(fileNameWithHash))
                {
                    loadPath = AssetPath.TranslateToLocalDataPath(fileNameWithHash);
                    return loadPath;
                }
            }
            else
            {
                // 非加密模式优先读取首包资源目录
                if (AssetManagement.offlineMode || AssetManagement.IsBuildInFile(fileNameWithHash))
                {
                    loadPath = AssetPath.TranslateToLocalDataPath(fileNameWithHash);
                    AssetBundleFileLoadPathRecord[fileNameWithHash] = loadPath;
                    return loadPath;
                }

                if (AssetManagement.IsBundleFileAlreadyInDownloadPath(bundleInfo))
                {
                    loadPath = AssetPath.TranslateToDownloadDataPath(fileNameWithHash);
                    AssetBundleFileLoadPathRecord[fileNameWithHash] = loadPath;
                    return loadPath;
                }
            }

            return AssetPath.TranslateToDownloadUrl(fileNameWithHash);
        }

        static readonly byte[] CryptoBytes = { 113, 57, 109, 108, 83, 109, 97, 88, 110, 50, 56, 74, 112, 56, 71, 67, 75, 52, 50, 50, 97, 75, 67, 99, 114, 105, 75, 74, 103, 113, 121, 84 };

        /// <summary>
        /// 创建ab包加密文件流
        /// </summary>
        public static CryptoAssetBundleStream NewCryptoStream(string path, FileMode mode, FileAccess access, ManifestBundleInfo bundleInfo)
        {
            return new CryptoAssetBundleStream(path, mode, access, FileShare.None, StreamBufferSize, CryptoBytes, bundleInfo.NameWithHash.ToUpper()[2..]);
        }

        /// <summary>
        /// 开始异步加载ab包
        /// </summary>
        internal static AssetBundleCreateRequest LoadAssetBundleFromStreamAsync(string filePath, ManifestBundleInfo bundleInfo, out CryptoAssetBundleStream stream)
        {
            stream = NewCryptoStream(filePath, FileMode.Open, FileAccess.Read, bundleInfo);
            return AssetBundle.LoadFromStreamAsync(stream);
        }
    }
}