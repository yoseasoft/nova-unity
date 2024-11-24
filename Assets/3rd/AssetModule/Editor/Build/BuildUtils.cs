using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AssetModule.Editor.Build
{
    /// <summary>
    /// 打包工具类
    /// </summary>
    public static class BuildUtils
    {
        /// <summary>
        /// 构建出的文件名是否只带Hash值, 建议正式上线时开启, 开发时可关闭, 
        /// 开启后可避免以下问题
        /// (1.避免文件名太长
        /// 2.避免文件名有中文
        /// 3.避免上线后用户可通过名字知道文件大概包含的内容)
        /// </summary>
        internal static bool isBuildHashOnlyFile = true;

        /// <summary>
        /// 上传文件目录名
        /// </summary>
        const string UploadFilesFolderName = "UploadFiles";

        /// <summary>
        /// 打包历史信息记录文件夹
        /// </summary>
        public const string BuildRecordFolderName = "~BuildRecord";

        /// <summary>
        /// 打包记录信息文件名前缀
        /// </summary>
        public const string BuildRecordFilePrefix = "buildRecord_v";

        /// <summary>
        /// ab包需要后缀为.unity3d, 不然超过一定数量非.unity3d后缀的文件会导致Android平台打包失败
        /// https://blog.csdn.net/qq_27437843/article/details/100585595
        /// </summary>
        internal const string AssetBundleFileExtension = ".unity3d";

        /// <summary>
        /// 依赖计算需要忽略的资源文件后缀
        /// </summary>
        static readonly List<string> ExcludeFiles = new()
        {
            ".cs",
            ".dll",
            ".spriteatlas",
            ".giparams",
            "LightingData.asset"
        };

        /// <summary>
        /// 根据版本获取上传统计文件的文件名
        /// </summary>
        public static string GetUploadStatisticsFilePath(int version)
        {
            return AssetPath.CombinePath(PlatformUploadPath, $"new_files_v{version}.txt");
        }

        /// <summary>
        /// 获取指定资源的所有依赖资源路径列表并去除自身路径和一些非必要依赖
        /// <param name="assetPath">资源路径</param>
        /// </summary>
        public static string[] GetDependencies(string assetPath)
        {
            HashSet<string> set = new HashSet<string>(AssetDatabase.GetDependencies(assetPath));
            set.Remove(assetPath);
            set.RemoveWhere(path => ExcludeFiles.Exists(path.EndsWith));
            return set.ToArray();
        }

        /// <summary>
        /// 获取当前版本的指定清单对象
        /// </summary>
        public static Manifest GetManifest(string manifestName)
        {
            string versionContainerFilePath = TranslateToBuildPath(ManifestHandler.VersionFileName);
            if (File.Exists(versionContainerFilePath))
            {
                ManifestVersionContainer versionContainer = ManifestHandler.LoadManifestVersionContainer(versionContainerFilePath);
                if (versionContainer is null)
                {
                    return null;
                }

                ManifestVersion manifestVersion = versionContainer.AllManifestVersions.Find(v => v.Name == manifestName);
                if (manifestVersion != null)
                {
                    string manifestFilePath = TranslateToBuildPath(manifestVersion.FileName);
                    if (File.Exists(manifestFilePath))
                        return ManifestHandler.LoadManifest(manifestFilePath);
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有清单配置
        /// </summary>
        public static List<ManifestConfig> GetAllManifestConfigs()
        {
            List<ManifestConfig> manifestConfigList = new List<ManifestConfig>();
            string[] guidList = AssetDatabase.FindAssets($"t:{typeof(ManifestConfig).FullName}");
            foreach (string guid in guidList)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(assetPath))
                    manifestConfigList.Add(AssetDatabase.LoadAssetAtPath<ManifestConfig>(assetPath));
            }

            return manifestConfigList;
        }

        /// <summary>
        /// 获取指定的清单配置
        /// </summary>
        public static ManifestConfig GetManifestConfig(string manifestName)
        {
            List<ManifestConfig> manifestConfigList = GetAllManifestConfigs();
            foreach (ManifestConfig manifestConfig in manifestConfigList)
                if (manifestConfig.name == manifestName)
                    return manifestConfig;

            return null;
        }

        /// <summary>
        /// 编辑器下获取或创建AssetSettings
        /// </summary>
        public static AssetSettings GetOrCreateAssetSettings()
        {
            // 直接放在Resources目录下
            string assetSettingFilePath = $"Assets/Resources/{nameof(AssetSettings)}.asset";

            AssetSettings settings = AssetDatabase.LoadAssetAtPath<AssetSettings>(assetSettingFilePath);
            if (!settings)
            {
                settings = ScriptableObject.CreateInstance<AssetSettings>();
                if (!Directory.Exists("Assets/Resources"))
                    Directory.CreateDirectory("Assets/Resources");
                AssetDatabase.CreateAsset(settings, assetSettingFilePath);
            }

            return settings;
        }

        /// <summary>
        /// 获取资源将要被打包的ab包名字, 若是原始文件则是打包的原始文件名
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <param name="bundleMode">打包方式</param>
        /// <param name="assetBundleFileName">打在同一个包时ab文件的命名</param>
        public static string GetAssetPackedBundleName(string assetPath, BundleMode bundleMode, string assetBundleFileName = "")
        {
            // 场景只支持独立打包
            if (assetPath.EndsWith(".unity"))
                bundleMode = BundleMode.单独打包;

            string bundleName;
            switch (bundleMode)
            {
                case BundleMode.整组打包:
                    bundleName = assetBundleFileName;
                    break;
                case BundleMode.单独打包:
                    bundleName = assetPath;
                    break;
                case BundleMode.按文件夹打包:
                    bundleName = Path.GetDirectoryName(assetPath);
                    break;
                case BundleMode.原始文件:
                    if (isBuildHashOnlyFile)
                        bundleName = Utility.ComputeHash(assetPath);
                    else
                    {
                        // 此处若有后缀的话则去除后缀, 下面再加上
                        if (Path.HasExtension(assetPath))
                            bundleName = $"{assetPath.Remove(assetPath.Length - Path.GetExtension(assetPath).Length)}_{Utility.ComputeHash(assetPath)}";
                        else
                            bundleName = $"{assetPath}_{Utility.ComputeHash(assetPath)}";
                    }

                    break;
                case BundleMode.匹配文件夹打包:
                    bundleName = assetBundleFileName;
                    break;
                default:
                    bundleName = string.Empty;
                    Debug.LogError($"未知打包类型:{bundleMode}！！！");
                    break;
            }

            // 去除空格,替换特殊符号,变为小写
            bundleName = bundleName.Replace(" ", string.Empty).Replace("\\", "/").Replace("/", "_").Replace("-", "_").Replace(".", "_").ToLower();

            // 原始文件需保留文件原有后缀(ab包的自定义后缀放到构建后再加上, 因SBP不支持构建前直接加后缀)
            if (bundleMode == BundleMode.原始文件)
                bundleName += Path.GetExtension(assetPath);

            return bundleName;
        }

        /// <summary>
        /// 首包资源放置目录
        /// </summary>
        public static string BuildLocalDataPath => $"{UnityEngine.Application.streamingAssetsPath}/{AssetPath.BuildPath}";

        /// <summary>
        /// 获取当前平台打包目录, 若没有该文件夹则顺便创建了
        /// </summary>
        public static string PlatformBuildPath
        {
            get
            {
                string path = AssetPath.CombinePath(AssetPath.BuildPath, GetActiveBuildPlatformName());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        /// <summary>
        /// 获取当前平台上传目录, 若没有该文件夹则顺便创建了
        /// </summary>
        public static string PlatformUploadPath
        {
            get
            {
                string path = AssetPath.CombinePath(UploadFilesFolderName, GetActiveBuildPlatformName());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        /// <summary>
        /// 获取当前激活的打包平台名字
        /// </summary>
        public static string GetActiveBuildPlatformName()
        {
            return Utility.CurrentPlatformName;
        }

        /// <summary>
        /// 转换成打包详细路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        public static string TranslateToBuildPath(string fileName)
        {
            return AssetPath.CombinePath(PlatformBuildPath, fileName);
        }

        /// <summary>
        /// 根据原始文件的构建名字获取hash(这样不用重新计算hash, 仅作字符串操作)
        /// </summary>
        public static string GetRawFileHashByBuildRawFileName(string buildRawFileName)
        {
            string fileName = Path.GetFileNameWithoutExtension(buildRawFileName);
            int pos = fileName.LastIndexOf("_", StringComparison.Ordinal);
            return fileName[(pos + 1)..];
        }

        /// <summary>
        /// 获取文件夹下所有文件
        /// </summary>
        /// <param name="directory">文件夹路径</param>
        /// <param name="pattern">文件匹配搜索字符</param>
        /// <param name="list">文件路径列表</param>
        public static void GetDirectoryAllFiles(string directory, string pattern, ref List<string> list)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            if (string.IsNullOrEmpty(pattern))
                foreach (FileInfo info in directoryInfo.GetFiles())
                    list.Add(info.FullName);
            else
                foreach (FileInfo info in directoryInfo.GetFiles(pattern))
                    list.Add(info.FullName);

            foreach (DirectoryInfo info in directoryInfo.GetDirectories())
                GetDirectoryAllFiles(info.FullName, pattern, ref list);
        }

        /// <summary>
        /// 获取外部原始文件的加载路径
        /// </summary>
        /// <param name="originalPath">原始文件目录</param>
        /// <param name="originalExternalFolderPath">原文件所在文件夹目录</param>
        /// <param name="placeFolderName">最后放置的文件夹目录</param>
        public static string GetExternalRawFileLoadPath(string originalPath, string originalExternalFolderPath, string placeFolderName)
        {
            // 去除原来的目录
            string tempPath = originalPath.Replace("\\", "/").Replace(originalExternalFolderPath.Replace("\\", "/"), string.Empty);
            if (tempPath.StartsWith("/"))                            // 清除外部目录后, 首字符可能会为'/'
                tempPath = tempPath[1..];                            // 去除'/'首字符
            return AssetPath.CombinePath(placeFolderName, tempPath); // 然后替换成放置目录
        }

        /// <summary>
        /// 选中清单配置
        /// </summary>
        public static void SelectManifestConfig()
        {
            string[] configs = AssetDatabase.FindAssets("t:ManifestConfig");
            if (configs.Length == 0)
            {
                Debug.LogWarning("没有找到资源清单配置, 请先创建(前往ManifestConfig.cs查看)");
                return;
            }

            var obj = AssetDatabase.LoadAssetAtPath<ManifestConfig>(AssetDatabase.GUIDToAssetPath(configs[0]));

            // 打开并聚焦到Project窗口
            EditorApplication.ExecuteMenuItem("Window/General/Project");

            // 使用ExecuteMenuItem打开窗口后, 不能立即显示选中效果, 所以延迟执行
            EditorApplication.delayCall += () =>
            {
                if (!obj)
                    return;

                // 文件夹选中效果
                EditorGUIUtility.PingObject(obj);

                // 真正选中文件夹,并在inspector展现
                Selection.activeObject = obj;
            };
        }
    }
}