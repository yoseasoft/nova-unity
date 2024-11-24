using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 资源目录相关
    /// </summary>
    public static class AssetPath
    {
        /// <summary>
        /// 打包目录
        /// </summary>
        public const string BuildPath = "Bundles";

        /// <summary>
        /// 本地路径的网址前缀(例:file://)
        /// </summary>
        static string s_localProtocol;

        /// <summary>
        /// 资源下载地址
        /// </summary>
        static string s_downloadUrl;

        /// <summary>
        /// 资源下载地址, 需外部设置
        /// </summary>
        public static string DownloadUrl
        {
            get => s_downloadUrl;
            set
            {
                if (value.EndsWith("/") || value.EndsWith("\\"))
                    s_downloadUrl = value[..^1];
                else
                    s_downloadUrl = value;
            }
        }

        /// <summary>
        /// 自定义获取下载地址方法
        /// </summary>
        public static Func<string, string> customDownloadUrlFunc;

        /// <summary>
        /// 平台名字
        /// </summary>
        public static string PlatformName { get; set; }

        /// <summary>
        /// 本地资源目录
        /// </summary>
        public static string LocalDataPath { get; set; }

        /// <summary>
        /// 下载资源目录
        /// </summary>
        public static string DownloadDataPath { get; set; }

        /// <summary>
        /// 初始化资源基本目录
        /// </summary>
        public static void InitAssetPath()
        {
            var pf = UnityEngine.Application.platform;
            if (pf != RuntimePlatform.OSXEditor && pf != RuntimePlatform.OSXPlayer && pf != RuntimePlatform.IPhonePlayer)
                s_localProtocol = pf is RuntimePlatform.WindowsEditor or RuntimePlatform.WindowsPlayer ? "file:///" : string.Empty;
            else
                s_localProtocol = "file://";

            // 以下目录允许编辑器下直接修改, 所以需要先判空再初始化
            if (string.IsNullOrEmpty(PlatformName))
                PlatformName = Utility.CurrentPlatformName;

            if (string.IsNullOrEmpty(LocalDataPath))
                LocalDataPath = CombinePath(Application.streamingAssetsPath, BuildPath);

            if (string.IsNullOrEmpty(DownloadDataPath))
                DownloadDataPath = CombinePath(Application.persistentDataPath, BuildPath);

            if (!AssetManagement.isOfflineWindows && !Directory.Exists(DownloadDataPath))
                Directory.CreateDirectory(DownloadDataPath);
        }

        /// <summary>
        /// 将文件名转换成本地资源目录的具体路径
        /// </summary>
        public static string TranslateToLocalDataPath(string fileName)
        {
            return CombinePath(LocalDataPath, fileName);
        }

        /// <summary>
        /// 将文件名转换成下载保存目录的具体路径
        /// </summary>
        public static string TranslateToDownloadDataPath(string fileName)
        {
            return CombinePath(DownloadDataPath, fileName);
        }

        /// <summary>
        /// 将文件名转换成本地资源路径下的具体url
        /// </summary>
        public static string TranslateToLocalDataPathUrl(string fileName)
        {
            if (Application.platform != RuntimePlatform.WindowsPlayer)
                return $"{s_localProtocol}{CombinePath(LocalDataPath, fileName)}";

            // #号在网页链接里代表分段, UnityWebRequest不支持分段, 但Windows下用户安装文件夹各种各样, 故本地文件路径需要使用EscapeURL保证文件路径正确(另外一些符号也可能导致问题, 例如:+)
            return $"{s_localProtocol}{UnityWebRequest.EscapeURL(CombinePath(LocalDataPath, fileName))}";
        }

        /// <summary>
        /// 将文件名转换成下载地址内的具体url
        /// </summary>
        public static string TranslateToDownloadUrl(string fileName)
        {
            string customUrl = customDownloadUrlFunc?.Invoke(fileName);
            return !string.IsNullOrEmpty(customUrl) ? customUrl : $"{DownloadUrl}/{CombinePath(PlatformName, fileName)}";
        }

        /// <summary>
        /// 将文件名转换成临时目录下的具体路径
        /// </summary>
        public static string TransferToTempPath(string fileName)
        {
            return CombinePath(Application.temporaryCachePath, fileName);
        }

        /// <summary>
        /// 连接目录
        /// </summary>
        public static string CombinePath(string path1, string path2)
        {
            return Path.Combine(path1, path2).Replace('\\', '/');
        }

        #region 自定义资源加载地址

        /// <summary>
        /// 自定义资源加载地址(简化的名字或路径)和真实记录路径对照表
        /// </summary>
        static readonly Dictionary<string, string> CustomAssetAddressToPath = new();

        /// <summary>
        /// 自定义加载地址方法
        /// </summary>
        public static Func<string, string> CustomLoadPathFunc { get; set; }

        /// <summary>
        /// 根据真实资源路径和自定义加载地址方法获得自定义加载地址并进行记录
        /// </summary>
        /// <param name="assetPath">真实资源路径</param>
        internal static void RecordCustomLoadPath(string assetPath)
        {
            if (CustomLoadPathFunc == null)
                return;

            string customAddress = CustomLoadPathFunc(assetPath);
            if (string.IsNullOrEmpty(customAddress))
                return;

            CustomAssetAddressToPath[customAddress] = assetPath;
        }

        /// <summary>
        /// 根据加载地址获取真实加载路径
        /// </summary>
        internal static string GetActualPath(string address)
        {
            return CustomAssetAddressToPath.GetValueOrDefault(address, address);
        }

        #endregion
    }
}