using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 总调度接口类
    /// </summary>
    public static class AssetManagement
    {
        /// <summary>
        /// 离线模式
        /// </summary>
        public static bool offlineMode;

        /// <summary>
        /// 是否在Windows平台下运行的离线模式
        /// </summary>
        public static bool isOfflineWindows;

        /// <summary>
        /// 内置资源包文件(包含已构建的原始文件)信息列表
        /// </summary>
        static readonly Dictionary<string, bool> BuildInBundleFileRecord = new();

        /// <summary>
        /// 自定义获取离线模式方法
        /// </summary>
        public static Func<bool> customGetOfflineModeFunc;

        #region 初始化、更新、下载、清除历史文件接口

        /// <summary>
        /// 初始化, 运行AssetManagement模块的第一步就是先初始化
        /// </summary>
        public static InitManifestOperation InitAsync()
        {
            // 读取基本设置设置参数
            AssetSettings settings = Resources.Load<AssetSettings>(nameof(AssetSettings));
            if (!settings) // 若Resources中没有(但正常来说使用此模块就应该在Assets目录下放一个AssetSettings)，则创建一个AssetSetting对象直接使用其默认值
                settings = ScriptableObject.CreateInstance<AssetSettings>();

            // 初始化参数
            // 离线模式
            offlineMode = customGetOfflineModeFunc?.Invoke() ?? settings.offlineMode;
            isOfflineWindows = offlineMode && (UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer || UnityEngine.Application.platform == RuntimePlatform.WindowsEditor);

            // 初始化资源基本目录(需放在读取是否Windows离线模式后面, 以免创建Bundle目录)
            AssetPath.InitAssetPath();

            // 首包资源列表
            if (settings.buildInBundleFileNameList != null)
                foreach (string name in settings.buildInBundleFileNameList)
                    BuildInBundleFileRecord[name] = true;

            InitManifestOperation initManifestOperation = InitManifestOperation.CreateInitManifestOperationFunc();
            initManifestOperation.Start();
            return initManifestOperation;
        }

        /// <summary>
        /// 更新指定的清单
        /// </summary>
        public static UpdateManifestOperation UpdateManifestAsync(int version = 0)
        {
            UpdateManifestOperation updateManifestOperation = new UpdateManifestOperation(version);
            updateManifestOperation.Start();
            return updateManifestOperation;
        }

        /// <summary>
        /// 获取指定清单所有资源的下载大小
        /// </summary>
        /// <param name="manifest">清单对象</param>
        public static GetDownloadSizeOperation GetDownloadSizeAsync(Manifest manifest)
        {
            GetDownloadSizeOperation operation = offlineMode ? new GetDownloadSizeOperation() : new GetDownloadSizeOperation(manifest.manifestBundleInfoList);
            operation.Start();
            return operation;
        }

        /// <summary>
        /// 获取指定资源列表的下载大小
        /// </summary>
        /// <param name="assetPathList">资源真实路径列表</param>
        /// <param name="manifestList">查找所用的清单列表, 不填则使用全局的清单列表</param>
        public static GetDownloadSizeOperation GetDownloadSizeAsync(string[] assetPathList, List<Manifest> manifestList = null)
        {
            List<ManifestBundleInfo> manifestBundleInfoList = null;
            if (!offlineMode)
                manifestBundleInfoList = GetNeedDownloadBundleInfoListByAssetPath(manifestList ?? ManifestHandler.ManifestList, assetPathList);

            GetDownloadSizeOperation operation = new GetDownloadSizeOperation(manifestBundleInfoList);
            operation.Start();
            return operation;
        }

        /// <summary>
        /// 开始下载操作
        /// </summary>
        /// <param name="downloadInfoList">需下载的列表</param>
        public static DownloadOperation DownloadAsync(List<DownloadInfo> downloadInfoList)
        {
            DownloadOperation operation = new DownloadOperation(downloadInfoList);
            operation.Start();
            return operation;
        }

        /// <summary>
        /// 清除历史文件
        /// </summary>
        public static ClearHistoryOperation ClearHistoryAsync()
        {
            var operation = new ClearHistoryOperation();
            operation.Start();
            return operation;
        }

        #endregion

        #region 加载资源、实例化对象、加载场景、加载原文件接口

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="address">资源地址(名字或路径)</param>
        /// <param name="type">资源类型</param>
        public static Asset LoadAsset(string address, Type type)
        {
            return AssetHandler.Load(address, type);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="address">资源地址(名字或路径)</param>
        /// <param name="type">资源类型</param>
        /// <param name="completed">加载完成回调</param>
        public static Asset LoadAssetAsync(string address, Type type, Action<Asset> completed = null)
        {
            return AssetHandler.LoadAsync(address, type, completed);
        }

        /// <summary>
        /// 同步实例化对象
        /// </summary>
        /// <param name="address">资源地址(名字或路径)</param>
        public static InstantiateObject InstantiateObject(string address)
        {
            return InstantiateObjectHandler.Instantiate(address);
        }

        /// <summary>
        /// 异步实例化对象
        /// </summary>
        /// <param name="address">资源地址(名字或路径)</param>
        /// <param name="completed">实例化完成回调</param>
        public static InstantiateObject InstantiateObjectAsync(string address, Action<InstantiateObject> completed = null)
        {
            return InstantiateObjectHandler.InstantiateAsync(address, completed);
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="address">资源地址(名字或路径)</param>
        /// <param name="isAdditive">是否使用叠加方式加载</param>
        public static Scene LoadScene(string address, bool isAdditive = false)
        {
            return SceneHandler.Load(address, isAdditive);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="address">资源地址(名字或路径)</param>
        /// <param name="isAdditive">是否使用叠加方式加载</param>
        /// <param name="completed">加载完成回调</param>
        public static Scene LoadSceneAsync(string address, bool isAdditive = false, Action<Scene> completed = null)
        {
            return SceneHandler.LoadAsync(address, isAdditive, completed);
        }

        /// <summary>
        /// 同步加载原文件(直接读取persistentDataPath中的文件, 然后可根据文件保存路径(RawFile.savePath)读取文件, 使用同步加载前需已保证文件更新)
        /// <param name="address">文件原打包路径('Assets/_Resources/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')</param>
        /// </summary>
        public static RawFile LoadRawFile(string address)
        {
            return RawFileHandler.Load(address);
        }

        /// <summary>
        /// 异步加载原文件(将所需的文件下载到persistentDataPath中, 完成后可根据文件保存路径(RawFile.savePath)自行读取文件)
        /// /// <param name="address">文件原打包路径('Assets/_Resources/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')</param>
        /// </summary>
        public static RawFile LoadRawFileAsync(string address, Action<RawFile> completed = null)
        {
            return RawFileHandler.LoadAsync(address, completed);
        }

        #endregion

        #region 内部工具函数

        /// <summary>
        /// 判断指定的资源包文件是否已下载
        /// </summary>
        internal static bool IsBundleFileAlreadyInDownloadPath(ManifestBundleInfo bundleInfo)
        {
            // 检查下载目录是否存在此文件
            string path = AssetPath.TranslateToDownloadDataPath(bundleInfo.SaveFileName);
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Exists && fileInfo.Length == bundleInfo.Size && Utility.ComputeHash(path) == bundleInfo.Hash;
        }

        /// <summary>
        /// 根据资源真实路径获取需要下载的资源包信息
        /// </summary>
        /// <param name="manifestList">用于获取的清单列表</param>
        /// <param name="assetPathList">资源真实路径列表</param>
        /// <returns>资源包信息列表</returns>
        static List<ManifestBundleInfo> GetNeedDownloadBundleInfoListByAssetPath(List<Manifest> manifestList, string[] assetPathList)
        {
            var bundleInfoList = new List<ManifestBundleInfo>();
            if (assetPathList == null || assetPathList.Length == 0)
            {
                foreach (Manifest manifest in manifestList)
                    bundleInfoList.AddRange(manifest.manifestBundleInfoList);
            }
            else
            {
                foreach (Manifest manifest in manifestList)
                {
                    foreach (string assetPath in assetPathList)
                    {
                        ManifestBundleInfo bundleInfo = manifest.GetBundleInfo(assetPath);
                        if (bundleInfo == null)
                            break;

                        if (!bundleInfoList.Contains(bundleInfo))
                            bundleInfoList.Add(bundleInfo);

                        foreach (ManifestBundleInfo dependentBundleInfo in manifest.GetDependentBundleInfoList(bundleInfo))
                            if (!BuildInBundleFileRecord.ContainsKey(dependentBundleInfo.NameWithHash) && !bundleInfoList.Contains(dependentBundleInfo))
                                bundleInfoList.Add(dependentBundleInfo);
                    }
                }
            }

            return bundleInfoList;
        }

        /// <summary>
        /// 是否首包资源
        /// </summary>
        /// <param name="fileNameWithHash">带hash的文件名字</param>
        internal static bool IsBuildInFile(string fileNameWithHash)
        {
            return BuildInBundleFileRecord.ContainsKey(fileNameWithHash);
        }

        #endregion
    }
}