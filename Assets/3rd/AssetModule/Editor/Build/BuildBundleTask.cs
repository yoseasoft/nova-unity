using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Build.Pipeline;
using UnityEditor.Build.Pipeline;

namespace AssetModule.Editor.Build
{
    /// <summary>
    /// 打包任务
    /// </summary>
    public class BuildBundleTask
    {
        /// <summary>
        /// 本次打包任务对应的清单名字
        /// </summary>
        readonly string _manifestName;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// 清单版本信息
        /// </summary>
        public ManifestVersion ManifestVersion { get; private set; }

        /// <summary>
        /// 需打包的资源组列表
        /// </summary>
        readonly List<Group> _groups;

        /// <summary>
        /// AB包打包选项
        /// </summary>
        readonly BuildAssetBundleOptions _buildAssetBundleOptions;

        /// <summary>
        /// 需要处理依赖的资源列表
        /// </summary>
        readonly List<BuildAssetInfo> _needHandleDependenciesAssetInfoList = new(5000);

        /// <summary>
        /// 原始资源列表
        /// </summary>
        readonly List<BuildAssetInfo> _rawAssetInfoList = new(1000);

        /// <summary>
        /// 需要打成ab包的资源列表
        /// </summary>
        readonly List<BuildAssetInfo> _bundledAssetInfoList = new(5000);

        /// <summary>
        /// 资源路径和资源对象对照表, 主要用作防止资源被重复打包到不同的ab包中, 存储的内容和bundledAssetList保持一致
        /// </summary>
        readonly Dictionary<string, BuildAssetInfo> _pathToAssetInfo = new();

        public BuildBundleTask(ManifestConfig manifestConfig)
        {
            _manifestName = manifestConfig.name;
            _groups = manifestConfig.groups;
            _buildAssetBundleOptions = manifestConfig.buildAssetBundleOptions;
        }

        /// <summary>
        /// 开始打包
        /// </summary>
        public bool Start()
        {
            // 1.采集打包资源, 加入到需打包的资源列表(rawAssetList和bundledAssetList)中
            CollectAllGroupsAssets();

            // 清单bundle信息列表, 构建原始资源和打ab包时添加记录
            List<ManifestBundleInfo> manifestBundleInfoList = new List<ManifestBundleInfo>(3000);

            // 2.构建原始资源
            BuildRawAssets(manifestBundleInfoList);

            #region 3.构建ab包

            // 3-1.检查重复资源, 防止一个资源同时打到两个ab包中
            CheckRepeatedAssets();

            // 3-2.分析依赖, 将依赖分组并加入到需打包的资源列表中
            bool isCancel = AutoGroupDependencies();
            if (isCancel)
                return false;

            // 3-3.打ab包
            bool isSuccessful = BuildAssetBundles(manifestBundleInfoList);

            // 3-4.删除打包默认构建文件
            File.Delete(BuildUtils.TranslateToBuildPath("buildlogtep.json"));
            File.Delete(BuildUtils.TranslateToBuildPath($"{BuildUtils.GetActiveBuildPlatformName()}.manifest"));

            if (!isSuccessful)
                return false;

            #endregion

            // 4.构建清单文件
            BuildManifestFile(manifestBundleInfoList);

            return true;
        }

        /// <summary>
        /// 采集所有组需要打包的资源
        /// </summary>
        void CollectAllGroupsAssets()
        {
            for (int i = 0; i < _groups.Count; i++)
            {
                Group group = _groups[i];
                if (!group.IsNeedBuild)
                    continue;

                DisplayProgressBar("正在采集需要打包的资源", $"当前组:{group.note}", i, _groups.Count);

                BuildAssetInfo[] assetInfoList = group.CollectAssets();
                if (group.bundleMode != BundleMode.原始文件)
                {
                    if (group.isNeedHandleDependencies)
                        _needHandleDependenciesAssetInfoList.AddRange(assetInfoList);

                    _bundledAssetInfoList.AddRange(assetInfoList);
                }
                else
                    _rawAssetInfoList.AddRange(assetInfoList);
            }

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 构建原始资源
        /// </summary>
        void BuildRawAssets(List<ManifestBundleInfo> manifestBundleInfoList)
        {
            for (int i = 0; i < _rawAssetInfoList.Count; i++)
            {
                BuildAssetInfo assetInfo = _rawAssetInfoList[i];
                string buildFileName = assetInfo.buildFileName;

                // 外部目录的原始文件打包时允许指定一个存放的文件夹
                if (assetInfo.isExternalFile)
                    buildFileName = AssetPath.CombinePath(assetInfo.placeFolderName, buildFileName);

                string buildPath = BuildUtils.TranslateToBuildPath(buildFileName);

                string buildPathDirectory = Path.GetDirectoryName(buildPath);
                if (!Directory.Exists(buildPathDirectory))
                    Directory.CreateDirectory(buildPathDirectory);

                File.Copy(assetInfo.path, buildPath, true);
                DisplayProgressBar("正在构建原始资源", assetInfo.path, i, _rawAssetInfoList.Count);

                string rawFileLoadPath = assetInfo.isExternalFile ? BuildUtils.GetExternalRawFileLoadPath(assetInfo.path, assetInfo.originalExternalPath, assetInfo.placeFolderName) : assetInfo.path;

                // 原始文件的包信息名字在正式运行中会用作保存目录用
                string bundleInfoName;

                // 外部文件需保留加载目录位置
                if (assetInfo.isExternalFile)
                {
                    // 若原始文件后缀为ab包的后缀则, 文件命名改为buildFileName(带Hash), 这样在仅保留Hash的情况下可以和其他ab包混在一起, 看不出来
                    if (Path.GetExtension(rawFileLoadPath).Equals(BuildUtils.AssetBundleFileExtension))
                        bundleInfoName = AssetPath.CombinePath(Path.GetDirectoryName(rawFileLoadPath), assetInfo.buildFileName); // 此处只能用assetInfo.buildFileName(不会带目录), 别用buildFileName(有可能带目录)
                    else
                        bundleInfoName = rawFileLoadPath;
                }
                // 内部文件不需要额外目录, 保存在最外层
                else
                    bundleInfoName = assetInfo.buildFileName; // 此处只能用assetInfo.buildFileName(不会带目录), 别用buildFileName(有可能带目录)

                manifestBundleInfoList.Add(new ManifestBundleInfo
                {
                    ID = manifestBundleInfoList.Count,
                    Name = bundleInfoName,
                    IsRawFile = true,
                    AssetPathList = new List<string> { rawFileLoadPath },
                    Size = new FileInfo(buildPath).Length,
                    Hash = BuildUtils.GetRawFileHashByBuildRawFileName(assetInfo.buildFileName), // 此处只能用assetInfo.buildFileName(不会带目录), 别用buildFileName(有可能带目录)
                    NameWithHash = buildFileName
                });
            }

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 检查重复的资源
        /// </summary>
        void CheckRepeatedAssets()
        {
            for (int i = 0; i < _bundledAssetInfoList.Count; i++)
            {
                BuildAssetInfo assetInfo = _bundledAssetInfoList[i];
                if (!_pathToAssetInfo.ContainsKey(assetInfo.path))
                    _pathToAssetInfo.Add(assetInfo.path, assetInfo);
                else
                    _bundledAssetInfoList.RemoveAt(i--);
            }
        }

        /// <summary>
        /// 自动将依赖分组
        /// 依赖分组规则:被多个资源依赖时才分出来,然后依赖资源按文件夹打包
        /// <returns>返回是否取消</returns>
        /// </summary>
        bool AutoGroupDependencies()
        {
            // key:被依赖资源的路径; value:依赖此资源的资源列表
            var dependentAssetPathToAssetInfoList = new Dictionary<string, List<BuildAssetInfo>>();

            for (int i = 0; i < _needHandleDependenciesAssetInfoList.Count; i++)
            {
                BuildAssetInfo assetInfo = _needHandleDependenciesAssetInfoList[i];
                bool isCancel = DisplayCancelableProgressBar("分析依赖中", assetInfo.path, i, _needHandleDependenciesAssetInfoList.Count);

                if (isCancel)
                {
                    Debug.Log("已取消构建");
                    EditorUtility.ClearProgressBar();
                    return true;
                }

                string[] dependentAssetPathList = BuildUtils.GetDependencies(assetInfo.path);
                foreach (string dependentAssetPath in dependentAssetPathList)
                {
                    // 针对Timeline资源的处理
                    // Timeline资源不能和Prefab分开打包, 会出现信息读取不到的问题, 若Unity新版本已解决, 可删除这个if代码段
                    // 所以就算被多个Prefab直接引用也不独立打包, 分开打到各个包里, 包体会大一点点, 但不会有bug
                    // https://forum.unity.com/threads/track-cannot-be-loaded.738869/
                    if (dependentAssetPath.EndsWith(".playable"))
                        continue;

                    // 引用到Editor目录下的资源打包后不会被引用, 直接忽略
                    if (dependentAssetPath.ToLower().Contains("/editor/"))
                        continue;

                    if (!dependentAssetPathToAssetInfoList.TryGetValue(dependentAssetPath, out List<BuildAssetInfo> assetInfoList))
                    {
                        assetInfoList = new List<BuildAssetInfo>(50);
                        dependentAssetPathToAssetInfoList[dependentAssetPath] = assetInfoList;
                    }

                    assetInfoList.Add(assetInfo);
                }
            }

            if (dependentAssetPathToAssetInfoList.Count > 0)
            {
                List<string> dependentAssetPathList = new List<string>(dependentAssetPathToAssetInfoList.Keys);
                dependentAssetPathList.Sort();
                for (int i = 0; i < dependentAssetPathList.Count; i++)
                {
                    string dependentAssetPath = dependentAssetPathList[i];
                    DisplayProgressBar("依赖分组中", dependentAssetPath, i, dependentAssetPathList.Count);
                    List<BuildAssetInfo> assetInfoList = dependentAssetPathToAssetInfoList[dependentAssetPath];

                    // 仅被单个资源依赖的资源不分开打包
                    if (assetInfoList.Count <= 1)
                        continue;

                    // 被依赖资源已经是需要打包的资源则跳过
                    if (_pathToAssetInfo.ContainsKey(dependentAssetPath))
                        continue;

                    BuildAssetInfo dependentAssetInfo = new BuildAssetInfo
                    {
                        path = dependentAssetPath,
                        buildFileName = BuildUtils.GetAssetPackedBundleName(dependentAssetPath, BundleMode.按文件夹打包)
                    };
                    _bundledAssetInfoList.Add(dependentAssetInfo);
                    _pathToAssetInfo.Add(dependentAssetPath, dependentAssetInfo);
                }
            }

            EditorUtility.ClearProgressBar();
            return false;
        }

        /// <summary>
        /// 根据需要打包的资源列表进行打包
        /// </summary>
        /// <param name="manifestBundleInfoList">清单包信息列表</param>
        /// <returns>返回UnityAssetBundle的清单信息</returns>
        bool BuildAssetBundles(List<ManifestBundleInfo> manifestBundleInfoList)
        {
            var bundleNameToAssetPathList = new Dictionary<string, List<string>>();

            // 根据需要打包的资源创建清单包数据
            foreach (BuildAssetInfo assetInfo in _bundledAssetInfoList)
            {
                if (!bundleNameToAssetPathList.TryGetValue(assetInfo.buildFileName, out List<string> assetPathList))
                {
                    assetPathList = new List<string>(50);
                    bundleNameToAssetPathList.Add(assetInfo.buildFileName, assetPathList);
                    manifestBundleInfoList.Add(new ManifestBundleInfo
                    {
                        ID = manifestBundleInfoList.Count,
                        Name = assetInfo.buildFileName,
                        AssetPathList = assetPathList
                    });
                }

                assetPathList.Add(assetInfo.path);
            }

            if (bundleNameToAssetPathList.Count == 0)
                return true;

            // 打ab包
            List<AssetBundleBuild> assetBundleBuildList = new List<AssetBundleBuild>(manifestBundleInfoList.Count);
            foreach (ManifestBundleInfo bundleInfo in manifestBundleInfoList)
            {
                // manifestBundleInfoList里可能有原始资源, 所以需要判断为打包资源才加入到ab构建队列中
                if (bundleNameToAssetPathList.ContainsKey(bundleInfo.Name))
                {
                    assetBundleBuildList.Add(new AssetBundleBuild
                    {
                        assetNames = bundleInfo.AssetPathList.ToArray(),
                        assetBundleName = bundleInfo.Name
                    });
                }
            }

            CompatibilityAssetBundleManifest assetBundleManifest = CompatibilityBuildPipeline.BuildAssetBundles(
                BuildUtils.PlatformBuildPath,
                assetBundleBuildList.ToArray(),
                _buildAssetBundleOptions | BuildAssetBundleOptions.AppendHashToAssetBundleName, // 打出名字后缀带有hash值的ab包
                EditorUserBuildSettings.activeBuildTarget);

            if (!assetBundleManifest)
            {
                Error = $"清单{_manifestName}打包失败";
                return false;
            }

            // 记录旧的清单文件, 为了下面进行比对将新的ab包文件进行加密
            Manifest oldManifest = BuildUtils.GetManifest(_manifestName);

            var originalBundleInfoMap = new Dictionary<string, ManifestBundleInfo>();
            if (oldManifest)
                foreach (ManifestBundleInfo originalBundleInfo in oldManifest.manifestBundleInfoList)
                    originalBundleInfoMap[originalBundleInfo.Name] = originalBundleInfo;

            // 完善清单包数据(nameWithHash、dependentManifestBundleIndexList)
            var nameToBundleInfo = new Dictionary<string, ManifestBundleInfo>();
            foreach (ManifestBundleInfo manifestBundleInfo in manifestBundleInfoList)
                nameToBundleInfo[manifestBundleInfo.Name] = manifestBundleInfo;

            string[] assetBundleNameList = assetBundleManifest.GetAllAssetBundles();

            // 获取ab包最终文件名
            string GetAssetBundleFileName(string assetBundleName)
            {
                if (BuildUtils.isBuildHashOnlyFile)
                    return assetBundleManifest.GetAssetBundleHash(assetBundleName) + BuildUtils.AssetBundleFileExtension;

                return $"{assetBundleName}_{assetBundleManifest.GetAssetBundleHash(assetBundleName)}{BuildUtils.AssetBundleFileExtension}";
            }

            // 为ab包文件加上后缀
            foreach (string assetBundleName in assetBundleNameList)
            {
                string originalFilePath = BuildUtils.TranslateToBuildPath($"{assetBundleName}_{assetBundleManifest.GetAssetBundleHash(assetBundleName)}");
                string targetFilePath = BuildUtils.TranslateToBuildPath(GetAssetBundleFileName(assetBundleName));
                if (File.Exists(targetFilePath))
                    File.Delete(originalFilePath); // 带hash的ab文件已存在时证明本次打包无变化, 直接删除原始ab文件
                else if (File.Exists(originalFilePath))
                    File.Move(originalFilePath, targetFilePath);
            }

            foreach (string assetBundleName in assetBundleNameList)
            {
                if (nameToBundleInfo.TryGetValue(assetBundleName, out ManifestBundleInfo manifestBundleInfo))
                {
                    string fileName = GetAssetBundleFileName(assetBundleName);
                    manifestBundleInfo.NameWithHash = fileName;
                    List<int> dependentBundleIDList = new();
                    foreach (string depAssetBundleName in assetBundleManifest.GetAllDependencies(assetBundleName))
                    {
                        if (!depAssetBundleName.Equals(assetBundleName)) // 获取ab依赖接口有可能包含自身, 此处需去除
                            dependentBundleIDList.Add(nameToBundleInfo[depAssetBundleName].ID);
                    }

                    manifestBundleInfo.DependentBundleIDList = dependentBundleIDList.ToArray();
                    string filePath = BuildUtils.TranslateToBuildPath(fileName);
                    if (File.Exists(filePath))
                    {
                        // 新ab文件加密
                        if (!originalBundleInfoMap.TryGetValue(assetBundleName, out ManifestBundleInfo oldInfo) || oldInfo.NameWithHash != fileName)
                            EncryptAssetBundle(filePath, manifestBundleInfo);

                        using FileStream stream = File.OpenRead(filePath);
                        manifestBundleInfo.Size = stream.Length;
                        manifestBundleInfo.Hash = Utility.ComputeHash(stream);
                    }
                    else
                    {
                        Debug.LogError($"ab包文件不存在:{fileName}");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError($"Bundle信息不存在:{assetBundleName}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 构建清单文件
        /// </summary>
        void BuildManifestFile(List<ManifestBundleInfo> manifestBundleInfoList)
        {
            // 创建清单配置
            Manifest manifest = ScriptableObject.CreateInstance<Manifest>();
            manifest.name = _manifestName;
            manifest.manifestBundleInfoList = manifestBundleInfoList;

            // 创建清单文件
            string filePath = BuildUtils.TranslateToBuildPath($"AssetModuleTempFile_{_manifestName}.json");
            File.WriteAllText(filePath, ManifestHandler.ManifestObjectToJson(manifest));

            // 重命名文件(使用hash用作文件后缀)
            string hash = Utility.ComputeHash(filePath);
            string newFileName = ManifestHandler.IsEncryptManifestFile ? (hash + BuildUtils.AssetBundleFileExtension) : $"{_manifestName}_{hash}.json";
            string newFilePath = BuildUtils.TranslateToBuildPath(newFileName);
            if (File.Exists(newFilePath))
                File.Delete(filePath); // 包含hash的文件已存在时证明本次打包无变化, 直接删除临时文件
            else
                File.Move(filePath, newFilePath);
            ManifestVersion = new ManifestVersion { Name = _manifestName, FileName = newFileName, Hash = hash, Size = new FileInfo(newFilePath).Length };
        }

        /// <summary>
        /// 显示Unity的进度条
        /// </summary>
        /// <param name="title">进度条窗口标题</param>
        /// <param name="content">进度条文本内容</param>
        /// <param name="index">当前进度值</param>
        /// <param name="max">当前进度最大值</param>
        static void DisplayProgressBar(string title, string content, int index, int max)
        {
            EditorUtility.DisplayProgressBar($"{title}({index}/{max}) ", content, (float)index / max);
        }

        /// <summary>
        /// 显示Unity的有取消按钮的进度条
        /// </summary>
        /// <param name="title">进度条窗口标题</param>
        /// <param name="content">进度条文本内容</param>
        /// <param name="index">当前进度值</param>
        /// <param name="max">当前进度最大值</param>
        /// <returns>是否取消</returns>
        static bool DisplayCancelableProgressBar(string title, string content, int index, int max)
        {
            return EditorUtility.DisplayCancelableProgressBar($"{title}({index}/{max}) ", content, (float)index / max);
        }

        /// <summary>
        /// 对ab包进行加密处理
        /// </summary>
        /// <param name="filePath">ab文件目录</param>
        /// <param name="info">BundleInfo</param>
        static void EncryptAssetBundle(string filePath, ManifestBundleInfo info)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"文件不存在:{filePath}");
            }

            if (BundleHandler.IsBundleEncrypt)
            {
                // 使用加密算法加密
                byte[] bytes = File.ReadAllBytes(filePath);
                string encryptFilePath = $"{filePath}_encrypt";
                CryptoAssetBundleStream cryptoStream = BundleHandler.NewCryptoStream(encryptFilePath, FileMode.Create, FileAccess.Write, info);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.Dispose();
                File.Delete(filePath);
                File.Move(encryptFilePath, filePath);
            }
            else if (BundleHandler.BundleOffset > 0)
            {
                // 仅进行偏移
                byte[] oldData = File.ReadAllBytes(filePath);
                int newDataLen = BundleHandler.BundleOffset + oldData.Length; // 空byte偏移
                byte[] newData = new byte[newDataLen];
                for (int i = 0; i < oldData.Length; i++)
                    newData[BundleHandler.BundleOffset + i] = oldData[i];
                FileStream fs = File.OpenWrite(filePath);
                fs.Write(newData, 0, newDataLen);
                fs.Close();
            }
        }
    }
}