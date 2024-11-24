using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace AssetModule.Editor.Build
{
    /// <summary>
    /// 打包资源组, 在清单配置里显示
    /// </summary>
    [Serializable]
    public class Group
    {
        /// <summary>
        /// 备注
        /// </summary>
        [LabelText("备注")]
        public string note;

        /// <summary>
        /// 是否启用
        /// </summary>
        [LabelText("参与打包?"), SerializeField]
        bool isActive = true;

        /// <summary>
        /// 组打包方式
        /// </summary>
        [LabelText("打包方式"), DrawWithUnity, ShowIf("isActive")]
        public BundleMode bundleMode = BundleMode.单独打包;

        /// <summary>
        /// ab包文件名, 资源打在一个包时会使用此名字对ab包命名
        /// </summary>
        [LabelText("ab包文件名"), ShowIf("@bundleMode == BundleMode.整组打包")]
        [InfoBox("请填写ab包文件名", InfoMessageType.Error, "@string.IsNullOrEmpty(assetBundleFileName)")]
        public string assetBundleFileName;

        /// <summary>
        /// 是否Assets目录外的目录
        /// </summary>
        [LabelText("Assets外的目录?"), ShowIf("@isActive && bundleMode == BundleMode.原始文件")]
        public bool isExternalPath;

        /// <summary>
        /// 目标文件或文件夹的Unity对象
        /// </summary>
        [LabelText("目标文件或文件夹"), ShowIf("@isActive && (!isExternalPath || bundleMode != BundleMode.原始文件)")]
        public Object target;

        /// <summary>
        /// 资源过滤条件文本
        /// https://docs.unity.cn/cn/2018.4/ScriptReference/AssetDatabase.FindAssets.html
        /// </summary>
        [LabelText("资源过滤条件"), ShowIf("@isActive && (!isExternalPath || bundleMode != BundleMode.原始文件)")]
        [PropertyTooltip("网页搜索AssetDatabase.FindAssets查看文档, 代码里注释也有链接")]
        public string filter = "";

        /// <summary>
        /// 是否需要处理依赖
        /// </summary>
        [LabelText("需要分析依赖?"), ShowIf("@isActive && bundleMode != BundleMode.原始文件")]
        [PropertyTooltip("最原始的资源不需要分析依赖, 节省打包时间(例如png, png不会依赖任何资源, 只有其他资源会依赖它)")]
        public bool isNeedHandleDependencies = true;

        /// <summary>
        /// 外部文件目录
        /// </summary>
        [LabelText("外部文件目录"), ShowIf("@isActive && bundleMode == BundleMode.原始文件 && isExternalPath")]
        [InfoBox("@GetExternalPathErrorTips()", InfoMessageType.Error, "@GetExternalPathErrorTips() != null")]
        public string externalPath;

        /// <summary>
        /// 文件或文件夹匹配搜索字符
        /// 文件:https://docs.microsoft.com/zh-cn/dotnet/api/system.io.directoryinfo.getfiles?view=net-5.0
        /// 文件夹:https://docs.microsoft.com/zh-cn/dotnet/api/system.io.directoryinfo.getdirectories?view=net-5.0
        /// </summary>
        [LabelText("文件匹配搜索字符"), ShowIf("@isActive && ((bundleMode == BundleMode.原始文件 && isExternalPath && IsExternalPathDirectory()) || bundleMode == BundleMode.匹配文件夹打包)")]
        [PropertyTooltip("网页搜索DirectoryInfo.GetFiles或DirectoryInfo.GetDirectories查看文档, 代码里注释也有链接")]
        public string searchPattern;

        /// <summary>
        /// 原始文件打包时放入的文件夹名字
        /// </summary>
        [LabelText("文件打包时放入的文件夹名字(不填就直接放在打包目录)"), ShowIf("@isActive && bundleMode == BundleMode.原始文件 && isExternalPath")]
        [PropertyTooltip("文件打包时放入的文件夹名字(不填就直接放在打包目录)")]
        public string placeFolderName;

        /// <summary>
        /// 原文件打包平台
        /// </summary>
        [LabelText("文件需打包的平台列表(不填代表所有平台)"), ShowIf("@isActive && bundleMode == BundleMode.原始文件"), ListDrawerSettings(DefaultExpandedState = true)]
        public BuildTarget[] buildTargetList = new BuildTarget[0];

        /// <summary>
        /// 匹配到文件夹后, 最后向上获取文件夹的层数(例如:默认为0, 就是匹配的文件夹本身, 1就是匹配文件夹的所在文件夹)
        /// </summary>
        [LabelText("文件夹向上层数"), ShowIf("@isActive && bundleMode == BundleMode.匹配文件夹打包")]
        [Tooltip("匹配到文件夹后, 最后向上获取文件夹的层数(例如:默认为0, 就是匹配到的文件夹本身, 1就是匹配到的文件夹的所在文件夹, 以此类推)")]
        public int parentFolderLayer;

        /// <summary>
        /// 判断当前组是否需要进行打包
        /// </summary>
        public bool IsNeedBuild => isActive && IsCurrentPlatformNeedBuild() && GetExternalPathErrorTips() == null;

        /// <summary>
        /// 根据当前平台判断是否需要打包(仅针对原始文件)
        /// </summary>
        bool IsCurrentPlatformNeedBuild()
        {
            return bundleMode != BundleMode.原始文件 || buildTargetList.Length == 0 || Array.Exists(buildTargetList, buildTarget => buildTarget == EditorUserBuildSettings.activeBuildTarget);
        }

        /// <summary>
        /// 判断外部目录是否为文件夹
        /// </summary>
        bool IsExternalPathDirectory()
        {
            return !string.IsNullOrEmpty(externalPath) && Directory.Exists(AssetPath.CombinePath(System.Environment.CurrentDirectory, externalPath));
        }

        /// <summary>
        /// 判断外部目录填写是否有误(仅针对原始文件)
        /// </summary>
        string GetExternalPathErrorTips()
        {
            if (bundleMode == BundleMode.原始文件 && isExternalPath)
            {
                if (string.IsNullOrEmpty(externalPath))
                    return "外部文件目录不能为空";

                string path = AssetPath.CombinePath(System.Environment.CurrentDirectory, externalPath);
                if (!Directory.Exists(path) && !File.Exists(path))
                    return $"文件不存在, 请确定填写的路径是从和Assets文件夹的同级目录(即{System.Environment.CurrentDirectory}下)开始填写";

                if (placeFolderName.Equals(BuildUtils.BuildRecordFolderName))
                    return "文件打包时放入的文件夹名字不能与打包记录文件夹重名, 请换个文件夹命名";
            }

            return null;
        }

        /// <summary>
        /// 根据过滤文本获取此组所有资源的路径列表
        /// </summary>
        public string[] GetAssetPathList()
        {
            if (bundleMode == BundleMode.原始文件 && isExternalPath)
            {
                if (GetExternalPathErrorTips() != null)
                    return new string[0];

                string targetPath = AssetPath.CombinePath(System.Environment.CurrentDirectory, externalPath);

                // 单个文件
                if (File.Exists(targetPath))
                    return new string[] { targetPath };

                // 文件夹下的所有文件
                List<string> filePathList = new List<string>();
                BuildUtils.GetDirectoryAllFiles(targetPath, searchPattern, ref filePathList);
                return Array.ConvertAll(filePathList.ToArray(), fullPath => fullPath.Replace($"{System.Environment.CurrentDirectory}\\", string.Empty).Replace("\\", "/"));
            }
            else
            {
                string targetPath = target ? AssetDatabase.GetAssetPath(target) : string.Empty;
                if (string.IsNullOrEmpty(targetPath))
                    return Array.Empty<string>();

                if (bundleMode != BundleMode.匹配文件夹打包)
                {
                    if (!Directory.Exists(targetPath))
                        return new[] { targetPath };

                    HashSet<string> set = new HashSet<string>();
                    string[] guidList = AssetDatabase.FindAssets(filter, new[] { targetPath });
                    foreach (string guid in guidList)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        if (!string.IsNullOrEmpty(assetPath) && !Directory.Exists(assetPath)) // 空字符或文件夹跳过
                            set.Add(assetPath);
                    }

                    return set.ToArray();
                }

                return Array.ConvertAll(CollectAssets(), buildAssetInfo => buildAssetInfo.path);
            }
        }

        /// <summary>
        /// 收集需要打包的资源
        /// </summary>
        /// <returns>返回资源对象列表</returns>
        public BuildAssetInfo[] CollectAssets()
        {
            if (bundleMode != BundleMode.匹配文件夹打包)
            {
                return Array.ConvertAll(GetAssetPathList(), assetPath =>
                {
                    assetPath = assetPath.Replace("\\", "/");
                    string originalExternalPath = string.Empty;
                    if (bundleMode == BundleMode.原始文件 && isExternalPath)
                    {
                        originalExternalPath = externalPath.Replace("\\", "/");
                        if (!originalExternalPath.EndsWith("/"))
                            originalExternalPath += "/";
                    }

                    return new BuildAssetInfo
                    {
                        path = assetPath,
                        buildFileName = BuildUtils.GetAssetPackedBundleName(assetPath, bundleMode, assetBundleFileName),
                        isExternalFile = bundleMode == BundleMode.原始文件 && isExternalPath,
                        originalExternalPath = originalExternalPath,
                        placeFolderName = bundleMode == BundleMode.原始文件 ? placeFolderName : string.Empty,
                    };
                });
            }
            else
            {
                string targetPath = target ? AssetDatabase.GetAssetPath(target) : string.Empty;
                if (string.IsNullOrEmpty(targetPath))
                    return Array.Empty<BuildAssetInfo>();

                if (!Directory.Exists(targetPath))
                    return Array.Empty<BuildAssetInfo>();

                // 搜索文件夹
                DirectoryInfo startDirectoryInfo = new DirectoryInfo(targetPath);
                DirectoryInfo[] searchDirectoryInfos = startDirectoryInfo.GetDirectories(searchPattern, SearchOption.AllDirectories);
                DirectoryInfo[] finalDirectoryInfos;
                if (parentFolderLayer > 0)
                {
                    int startDirectoryPathLength = startDirectoryInfo.FullName.Length;
                    List<DirectoryInfo> finalDirectoryInfoList = new List<DirectoryInfo>();
                    foreach (var directoryInfo in searchDirectoryInfos)
                    {
                        DirectoryInfo finalDirectory = directoryInfo;
                        for (int i = 0; i < parentFolderLayer; i++)
                            finalDirectory = finalDirectory.Parent;
                        if (finalDirectory != null)
                        {
                            if (finalDirectory.FullName.Length <= startDirectoryPathLength)
                                finalDirectory = directoryInfo;
                            finalDirectoryInfoList.Add(finalDirectory);
                        }
                    }

                    finalDirectoryInfos = finalDirectoryInfoList.ToArray();
                }
                else
                    finalDirectoryInfos = searchDirectoryInfos;

                List<BuildAssetInfo> buildAssetInfoList = new List<BuildAssetInfo>();
                Dictionary<string, bool> buildAssetPathMap = new Dictionary<string, bool>();

                // 项目目录长度
                int projectPathLength = new DirectoryInfo(UnityEngine.Application.dataPath).Parent.FullName.Length;

                // 在搜出来的文件夹里搜索资源, 每个文件夹单独打包
                foreach (var dirInfo in finalDirectoryInfos)
                {
                    string simplePath = dirInfo.FullName[(projectPathLength + 1)..];
                    string[] guidList = AssetDatabase.FindAssets(filter, new string[] { simplePath });
                    foreach (string guid in guidList)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        // 空字符或文件夹跳过
                        if (string.IsNullOrEmpty(assetPath) || Directory.Exists(assetPath))
                            continue;

                        // 重复的也忽略, 没有重复则进行添加
                        if (!buildAssetPathMap.TryAdd(assetPath, false))
                            continue;

                        buildAssetInfoList.Add(new BuildAssetInfo
                        {
                            path = assetPath,
                            buildFileName = BuildUtils.GetAssetPackedBundleName(assetPath, bundleMode, simplePath),
                            isExternalFile = false,
                            originalExternalPath = string.Empty,
                            placeFolderName = string.Empty,
                        });
                    }
                }

                return buildAssetInfoList.ToArray();
            }
        }
    }
}