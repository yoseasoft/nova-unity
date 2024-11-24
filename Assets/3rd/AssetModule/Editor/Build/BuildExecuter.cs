using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using AssetModule.Editor.GUI;
using System.Collections.Generic;

namespace AssetModule.Editor.Build
{
    /// <summary>
    /// 打包执行类
    /// </summary>
    public static class BuildExecuter
    {
        #region 清单资源打包

        /// <summary>
        /// 运行资源打包
        /// </summary>
        public static void StartBuild()
        {
            int manifestCount = BuildUtils.GetAllManifestConfigs().Count;
            if (manifestCount == 0)
            {
                Debug.Log("请先创建资源清单进行配置");
                return;
            }

            BuildSelectWindow.Open();
        }

        /// <summary>
        /// 对所有资源清单包含的资源打包
        /// </summary>
        public static bool BuildAllManifests()
        {
            // 因单个任务经过构建ab包的过程, 所以创建的ManifestConfig(ScriptableObject)会被销毁
            // 所以要先存放在任务列表里再开始任务, 不能一边遍历GetAllMainifestConfigs中的对象一边开始任务
            List<BuildBundleTask> bundleTasks = new List<BuildBundleTask>();
            foreach (ManifestConfig manifestConfig in BuildUtils.GetAllManifestConfigs())
                bundleTasks.Add(new BuildBundleTask(manifestConfig));

            List<ManifestVersion> newBuildManifestVersionList = new List<ManifestVersion>();
            foreach (BuildBundleTask task in bundleTasks)
            {
                bool isFinished = task.Start();
                if (!isFinished)
                {
                    if (!string.IsNullOrEmpty(task.Error))
                        Debug.LogError(task.Error);
                    return false;
                }

                newBuildManifestVersionList.Add(task.ManifestVersion);
            }

            AfterBuildManifest(newBuildManifestVersionList);
            return true;
        }

        /// <summary>
        /// 对指定的资源清单包含的资源打包
        /// </summary>
        public static bool BuildManifest(ManifestConfig manifestConfig)
        {
            BuildBundleTask task = new BuildBundleTask(manifestConfig);
            bool isFinished = task.Start();
            if (!isFinished)
            {
                if (!string.IsNullOrEmpty(task.Error))
                    Debug.LogError(task.Error);
                return false;
            }

            AfterBuildManifest(new List<ManifestVersion>() { task.ManifestVersion });
            return true;
        }

        /// <summary>
        /// 清单打包完成后处理
        /// </summary>
        static void AfterBuildManifest(List<ManifestVersion> newBuildManifestVersionList)
        {
            // 判断文件变化
            if (!HasFileChanged(newBuildManifestVersionList))
            {
                Debug.Log("构建完成, 本次构建没有任何文件发生变化");
                return;
            }

            // 构建版本文件
            BuildManifestVersionContainerFile(newBuildManifestVersionList);

            // 打印新文件个数和大小
            PrintNewFileInfo(newBuildManifestVersionList);

            // 将新文件复制到上传目录, 并写入新文件名到txt
            CopyNewFileToUploadPathAndWriteInStatisticsFile(newBuildManifestVersionList);

            // 打包后清除过期文件
            ClearHistoryFiles();

            // 记录打包详情
            RecordBuildInfo(newBuildManifestVersionList);
        }

        /// <summary>
        /// 是否有文件改变
        /// </summary>
        static bool HasFileChanged(List<ManifestVersion> newBuildManifestVersionList)
        {
            // 清单名记录
            Dictionary<string, bool> newManifestNameRecord = new Dictionary<string, bool>();

            // 分析新清单资源文件变动
            foreach (ManifestVersion newManifestVersion in newBuildManifestVersionList)
            {
                newManifestNameRecord.Add(newManifestVersion.Name, true);
                Manifest oldManifest = BuildUtils.GetManifest(newManifestVersion.Name);
                Manifest newManifest = ManifestHandler.LoadManifest(BuildUtils.TranslateToBuildPath(newManifestVersion.FileName));

                // 是否有新文件
                var oldBundleInfoMap = new Dictionary<string, ManifestBundleInfo>();
                if (oldManifest)
                    foreach (ManifestBundleInfo oldBundleInfo in oldManifest.manifestBundleInfoList)
                        oldBundleInfoMap[oldBundleInfo.Name] = oldBundleInfo;

                foreach (ManifestBundleInfo newInfo in newManifest.manifestBundleInfoList)
                    if (!oldBundleInfoMap.TryGetValue(newInfo.Name, out ManifestBundleInfo oldInfo) || newInfo.Hash != oldInfo.Hash)
                        return true;

                // 是否有过时文件
                var newBundleInfoMap = new Dictionary<string, ManifestBundleInfo>();
                foreach (ManifestBundleInfo newBundleInfo in newManifest.manifestBundleInfoList)
                    newBundleInfoMap[newBundleInfo.Name] = newBundleInfo;

                if (oldManifest)
                    foreach (ManifestBundleInfo oldInfo in oldManifest.manifestBundleInfoList)
                        if (!newBundleInfoMap.TryGetValue(oldInfo.Name, out ManifestBundleInfo newInfo) || newInfo.Hash != oldInfo.Hash)
                            return true;
            }

            // 分析是否有删除的清单
            string versionContainerFilePath = BuildUtils.TranslateToBuildPath(ManifestHandler.VersionFileName);
            if (File.Exists(versionContainerFilePath))
            {
                ManifestVersionContainer oldVersionContainer = ManifestHandler.LoadManifestVersionContainer(versionContainerFilePath);
                if (oldVersionContainer)
                    foreach (ManifestVersion oldManifestVersion in oldVersionContainer.AllManifestVersions)
                        if (!newManifestNameRecord.ContainsKey(oldManifestVersion.Name))
                            return true;
            }

            return false;
        }

        /// <summary>
        /// 获取当前清单版本容器版本号
        /// </summary>
        static int GetCurrentContainerVersion()
        {
            string versionContainerFilePath = BuildUtils.TranslateToBuildPath(ManifestHandler.VersionFileName);
            if (File.Exists(versionContainerFilePath))
            {
                ManifestVersionContainer versionContainer = ManifestHandler.LoadManifestVersionContainer(versionContainerFilePath);
                if (versionContainer != null)
                    return versionContainer.Version;
            }

            return 0;
        }

        /// <summary>
        /// 构建清单版本容器文件
        /// </summary>
        static void BuildManifestVersionContainerFile(List<ManifestVersion> newBuildManifestVersionList)
        {
            int newVersion = GetCurrentContainerVersion() + 1;
            string versionContainerFilePath = BuildUtils.TranslateToBuildPath(ManifestHandler.VersionFileName);

            ManifestVersionContainer newVersionContainer;
            if (File.Exists(versionContainerFilePath))
                newVersionContainer = ManifestHandler.LoadManifestVersionContainer(versionContainerFilePath);
            else
                newVersionContainer = ScriptableObject.CreateInstance<ManifestVersionContainer>();

            newVersionContainer.Version = newVersion;
            newVersionContainer.Timestamp = DateTime.Now.ToFileTime();
            if (BuildUtils.GetAllManifestConfigs().Count == newBuildManifestVersionList.Count)
            {
                newVersionContainer.AllManifestVersions.Clear();
                newVersionContainer.AllManifestVersions.AddRange(newBuildManifestVersionList);
            }
            else
            {
                foreach (ManifestVersion newManifestVersion in newBuildManifestVersionList)
                {
                    ManifestVersion manifestVersion = newVersionContainer.AllManifestVersions.Find(v => v.Name == newManifestVersion.Name);
                    if (manifestVersion != null)
                        manifestVersion.Overwrite(newManifestVersion);
                    else
                        newVersionContainer.AllManifestVersions.Add(newManifestVersion);
                }
            }

            File.WriteAllText(versionContainerFilePath, ManifestHandler.ManifestObjectToJson(newVersionContainer));

            // 带版本号的文件, 网络下载用
            string filePathWithVersion = BuildUtils.TranslateToBuildPath(ManifestHandler.GetVersionFileNameWithVersion(newVersion));
            File.Copy(versionContainerFilePath, filePathWithVersion, true);

            Debug.Log($"构建资源包完成, 最新版本:{newVersion}");
        }

        /// <summary>
        /// 打印新文件个数和大小
        /// </summary>
        static void PrintNewFileInfo(List<ManifestVersion> newBuildManifestVersionList)
        {
            int totalNewFileCount = 0;
            long totalNewFilesSize = 0L;

            int curVersion = GetCurrentContainerVersion();
            string oldVersionContainerPath = BuildUtils.TranslateToBuildPath(ManifestHandler.GetVersionFileNameWithVersion(curVersion - 1));
            ManifestVersionContainer oldVersionContainer;
            if (File.Exists(oldVersionContainerPath))
                oldVersionContainer = ManifestHandler.LoadManifestVersionContainer(oldVersionContainerPath);
            else
                oldVersionContainer = ScriptableObject.CreateInstance<ManifestVersionContainer>();

            foreach (ManifestVersion newManifestVersion in newBuildManifestVersionList)
            {
                string manifestName = newManifestVersion.Name;
                Manifest oldManifest = GetOrCreateLastVersionManifest(manifestName);
                Manifest newManifest = ManifestHandler.LoadManifest(BuildUtils.TranslateToBuildPath(newManifestVersion.FileName));

                // 计算新文件
                int newFileCount = 0;
                long newFilesSize = 0L;

                var oldBundleInfoMap = new Dictionary<string, ManifestBundleInfo>();
                if (oldManifest)
                    foreach (ManifestBundleInfo oldBundleInfo in oldManifest.manifestBundleInfoList)
                        oldBundleInfoMap[oldBundleInfo.Name] = oldBundleInfo;

                foreach (ManifestBundleInfo newInfo in newManifest.manifestBundleInfoList)
                {
                    if (oldBundleInfoMap.TryGetValue(newInfo.Name, out ManifestBundleInfo oldInfo) && newInfo.Hash == oldInfo.Hash)
                        continue;

                    newFileCount++;
                    newFilesSize += newInfo.Size;
                }

                // 计算新清单文件
                ManifestVersion oldManifestVersion = oldVersionContainer.AllManifestVersions.Find(v => v.Name == newManifestVersion.Name);
                if (oldManifestVersion == null || oldManifestVersion.FileName != newManifestVersion.FileName)
                {
                    string newManifestFilePath = BuildUtils.TranslateToBuildPath(newManifestVersion.FileName);
                    FileInfo newManifestFileInfo = new FileInfo(newManifestFilePath);
                    if (newManifestFileInfo.Exists)
                    {
                        newFileCount++;
                        newFilesSize += newManifestFileInfo.Length;
                    }
                    else
                        Debug.LogWarning($"新清单文件({newManifestVersion.FileName})不存在？？？");
                }

                totalNewFileCount += newFileCount;
                totalNewFilesSize += newFilesSize;
            }

            // 计算版本文件(此处不计算不带版本号的版本文件，因每个版本都有此文件，而且仅打包时用，不会上传)
            string newFilePathWithVersion = BuildUtils.TranslateToBuildPath(ManifestHandler.GetVersionFileNameWithVersion(curVersion));
            if (File.Exists(newFilePathWithVersion))
            {
                totalNewFileCount++;
                totalNewFilesSize += new FileInfo(newFilePathWithVersion).Length;
            }

            if (totalNewFileCount > 0)
                Debug.Log($"本次构建共产生{totalNewFileCount}个新文件, 新文件总大小为:{Utility.FormatBytes(totalNewFilesSize)}");
        }

        /// <summary>
        /// 获取或创建上一个版本的清单对象
        /// </summary>
        static Manifest GetOrCreateLastVersionManifest(string manifestName)
        {
            // 获取当前最新版本
            int version = GetCurrentContainerVersion();
            string lastVersionFilePath = BuildUtils.TranslateToBuildPath(ManifestHandler.GetVersionFileNameWithVersion(version - 1));
            if (File.Exists(lastVersionFilePath))
            {
                ManifestVersionContainer lastManifestVersionContainer = ManifestHandler.LoadManifestVersionContainer(lastVersionFilePath);
                ManifestVersion lastManifestVersion = lastManifestVersionContainer.AllManifestVersions.Find(v => v.Name == manifestName);
                if (lastManifestVersion != null)
                {
                    string lastManifestPath = BuildUtils.TranslateToBuildPath(lastManifestVersion.FileName);
                    if (File.Exists(lastManifestPath))
                        return ManifestHandler.LoadManifest(lastManifestPath);
                }
            }

            return null;
        }

        /// <summary>
        /// 将新的文件复制到上传目录并写入统计文件(上传后可手动删除里面的文件, 以免下次重复上传, 导致上传时间长)
        /// </summary>
        static void CopyNewFileToUploadPathAndWriteInStatisticsFile(List<ManifestVersion> newBuildManifestVersionList)
        {
            #region 统计新文件

            // 新文件名列表(若是原始文件, 此处的名字会带有其所放在的目录)
            List<string> newFilesName = new List<string>(5000);

            // 清单文件包含的资源
            foreach (ManifestVersion manifestVersion in newBuildManifestVersionList)
            {
                string manifestName = manifestVersion.Name;
                Manifest oldManifest = GetOrCreateLastVersionManifest(manifestName);
                Manifest newManifest = ManifestHandler.LoadManifest(BuildUtils.TranslateToBuildPath(manifestVersion.FileName));

                var oldBundleInfoMap = new Dictionary<string, ManifestBundleInfo>();
                if (oldManifest)
                    foreach (ManifestBundleInfo oldBundleInfo in oldManifest.manifestBundleInfoList)
                        oldBundleInfoMap[oldBundleInfo.Name] = oldBundleInfo;

                foreach (ManifestBundleInfo newInfo in newManifest.manifestBundleInfoList)
                    if (!oldBundleInfoMap.TryGetValue(newInfo.Name, out ManifestBundleInfo oldInfo) || newInfo.Hash != oldInfo.Hash)
                        newFilesName.Add(newInfo.NameWithHash);
            }

            // 版本文件
            int curVersion = GetCurrentContainerVersion();
            string newVersionFileName = ManifestHandler.GetVersionFileNameWithVersion(curVersion);
            string newVersionFilePath = BuildUtils.TranslateToBuildPath(newVersionFileName);
            if (File.Exists(newVersionFilePath))
                newFilesName.Add(newVersionFileName);

            // 清单文件
            string oldVersionContainerPath = BuildUtils.TranslateToBuildPath(ManifestHandler.GetVersionFileNameWithVersion(curVersion - 1));
            ManifestVersionContainer oldVersionContainer;
            if (File.Exists(oldVersionContainerPath))
                oldVersionContainer = ManifestHandler.LoadManifestVersionContainer(oldVersionContainerPath);
            else
                oldVersionContainer = ScriptableObject.CreateInstance<ManifestVersionContainer>();
            foreach (ManifestVersion manifestVersion in newBuildManifestVersionList)
            {
                ManifestVersion oldManifestVersion = oldVersionContainer.AllManifestVersions.Find(v => v.Name == manifestVersion.Name);
                if (oldManifestVersion == null || oldManifestVersion.FileName != manifestVersion.FileName)
                    newFilesName.Add(manifestVersion.FileName);
            }

            #endregion

            #region 复制到上传目录

            for (int i = 0; i < newFilesName.Count; i++)
            {
                string fileName = newFilesName[i];
                string sourceFilePath = BuildUtils.TranslateToBuildPath(fileName);
                EditorUtility.DisplayProgressBar($"正在复制新文件到上传目录({i + 1}/{newFilesName.Count}) ", fileName, (float)(i + 1) / newFilesName.Count);
                if (File.Exists(sourceFilePath))
                {
                    string destFilePath = AssetPath.CombinePath(BuildUtils.PlatformUploadPath, fileName);
                    string destFolder = Path.GetDirectoryName(destFilePath);
                    if (!Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);
                    File.Copy(sourceFilePath, destFilePath, true);
                }
                else
                    Debug.LogWarning($"从打包目录复制文件({fileName})到上传目录失败, 文件({sourceFilePath})不存在！！");
            }

            EditorUtility.ClearProgressBar();

            #endregion

            #region 统计新上传文件到一个txt中

            // 成功复制的文件名列表
            List<string> copySuccessUploadFilesName = new List<string>(1000);
            foreach (string fileName in newFilesName)
                if (File.Exists(AssetPath.CombinePath(BuildUtils.PlatformUploadPath, fileName)))
                    copySuccessUploadFilesName.Add(fileName);

            if (copySuccessUploadFilesName.Count == 0)
                return;

            // 创建新上传文件统计文件, 方便cdn根据此文件做新文件的预热
            string uploadStatisticsFilePath = BuildUtils.GetUploadStatisticsFilePath(curVersion);
            if (File.Exists(uploadStatisticsFilePath))
                File.Delete(uploadStatisticsFilePath);

            FileStream uploadStatisticsFileStream = new FileStream(uploadStatisticsFilePath, FileMode.CreateNew);
            for (int i = 0; i < copySuccessUploadFilesName.Count; i++)
            {
                string fileName = copySuccessUploadFilesName[i];
                string writeStr = i < copySuccessUploadFilesName.Count - 1 ? $"{fileName}\n" : fileName;
                byte[] writeBytes = Encoding.UTF8.GetBytes(writeStr);
                uploadStatisticsFileStream.Write(writeBytes, 0, writeBytes.Length);
            }

            uploadStatisticsFileStream.Close();

            #endregion
        }

        /// <summary>
        /// 清除历史版本打包文件
        /// </summary>
        public static void ClearHistoryFiles()
        {
            // 当前使用到的文件列表
            List<string> usefulFileNameList = new List<string>(5000);

            ManifestVersionContainer versionContainer = null;
            string versionContainerFilePath = BuildUtils.TranslateToBuildPath(ManifestHandler.VersionFileName);
            if (File.Exists(versionContainerFilePath))
                versionContainer = ManifestHandler.LoadManifestVersionContainer(versionContainerFilePath);

            if (versionContainer != null)
            {
                // 加入版本文件
                usefulFileNameList.Add(ManifestHandler.VersionFileName);
                usefulFileNameList.Add(ManifestHandler.GetVersionFileNameWithVersion(versionContainer.Version));

                foreach (ManifestVersion manifestVersion in versionContainer.AllManifestVersions)
                {
                    Manifest manifest = ManifestHandler.LoadManifest(BuildUtils.TranslateToBuildPath(manifestVersion.FileName));
                    if (!manifest)
                        continue;

                    // 清单文件
                    usefulFileNameList.Add(manifestVersion.FileName);

                    // 所有构建的打包文件, 包含原始文件
                    foreach (ManifestBundleInfo bundleInfo in manifest.manifestBundleInfoList)
                    {
                        if (!bundleInfo.IsRawFile) // ab包文件
                            usefulFileNameList.Add(bundleInfo.NameWithHash);
                        else // 原始文件hash名可能带有目录, 所以需要去掉目录
                            usefulFileNameList.Add(Path.GetFileName(bundleInfo.NameWithHash));
                    }
                }
            }

            // 删除无用文件
            int deleteFileCount = 0;
            long deleteFileSize = 0L;
            List<string> filePathList = new List<string>(2000);
            BuildUtils.GetDirectoryAllFiles(BuildUtils.PlatformBuildPath, string.Empty, ref filePathList);
            for (int i = 0; i < filePathList.Count; i++)
            {
                string filePath = filePathList[i];
                string fileName = Path.GetFileName(filePath);

                // 跳过有用文件
                if (usefulFileNameList.Contains(fileName))
                    continue;

                // 跳过历史版本记录文件
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Directory is { Name: BuildUtils.BuildRecordFolderName })
                    continue;

                deleteFileCount++;
                deleteFileSize += fileInfo.Length;
                File.Delete(filePath);
                EditorUtility.DisplayProgressBar("正在清理过时文件", fileName, (float)i / filePathList.Count);
            }

            if (deleteFileCount > 0)
            {
                Debug.Log($"共清理{deleteFileCount}个过时文件, 节省空间{Utility.FormatBytes(deleteFileSize)}");
                EditorUtility.ClearProgressBar();
            }
            else
                Debug.Log("清理检测完成, 暂无过时文件~");
        }

        /// <summary>
        /// 记录本次打包的详细信息
        /// </summary>
        static bool RecordBuildInfo(List<ManifestVersion> newBuildManifestVersionList)
        {
            string versionContainerFilePath = BuildUtils.TranslateToBuildPath(ManifestHandler.VersionFileName);
            if (!File.Exists(versionContainerFilePath))
                return false;

            ManifestVersionContainer versionContainer = ManifestHandler.LoadManifestVersionContainer(versionContainerFilePath);
            if (!versionContainer)
                return false;

            string saveFileDirectoryPath = BuildUtils.TranslateToBuildPath(BuildUtils.BuildRecordFolderName);
            if (!Directory.Exists(saveFileDirectoryPath))
                Directory.CreateDirectory(saveFileDirectoryPath);

            // 先加载上一个版本的记录数据
            DirectoryInfo directoryInfo = new DirectoryInfo(saveFileDirectoryPath);
            FileInfo[] fileInfoList = directoryInfo.GetFiles("*.json");
            long latestBuildTime = 0;
            string finalLoadFilePath = null;
            BuildRecord oldBuildRecord = null;
            foreach (FileInfo fileInfo in fileInfoList)
            {
                string fileName = fileInfo.Name;
                if (!fileName.StartsWith(BuildUtils.BuildRecordFilePrefix) || !fileName.EndsWith(".json"))
                    continue;

                if (VersionContrastUtils.GetBuildVersion(fileName) != versionContainer.Version - 1)
                    continue;

                // 以免存在手动修改版本后相同版本的文件, 加上判断最新的时间戳
                long buildTime = VersionContrastUtils.GetVersionBuildTime(fileName);
                if (buildTime > latestBuildTime)
                {
                    latestBuildTime = buildTime;
                    finalLoadFilePath = fileInfo.FullName;
                }
            }

            if (!string.IsNullOrEmpty(finalLoadFilePath))
                oldBuildRecord = JsonUtility.FromJson<BuildRecord>(File.ReadAllText(finalLoadFilePath));

            // 记录版本和清单文件信息
            List<VersionFileInfo> versionFileInfoList = new List<VersionFileInfo>
            {
                // 记录版本文件信息
                new()
                {
                    name = ManifestHandler.DefaultVersionFileName,
                    hash = Utility.ComputeHash(versionContainerFilePath),
                    size = new FileInfo(versionContainerFilePath).Length
                }
            };

            // 记录本次构建是否含有某个清单
            Dictionary<string, bool> hasManifestBuild = new Dictionary<string, bool>();
            foreach (ManifestVersion manifestVersion in newBuildManifestVersionList)
                hasManifestBuild.Add(manifestVersion.Name, true);

            // 上个版本的包数据记录
            Dictionary<string, RecordBundleInfo> bundleNameToOldBundleInfo = new Dictionary<string, RecordBundleInfo>();
            if (oldBuildRecord != null)
                foreach (RecordManifest recordManifest in oldBuildRecord.recordManifestList)
                    if (recordManifest.recordBundleInfoList != null)
                        foreach (RecordBundleInfo bundleInfo in recordManifest.recordBundleInfoList)
                            bundleNameToOldBundleInfo.Add(bundleInfo.Name, bundleInfo);

            // 记录清单列表信息
            List<RecordManifest> recordManifestList = new List<RecordManifest>();
            foreach (ManifestVersion manifestVersion in versionContainer.AllManifestVersions)
            {
                string manifestFilePath = BuildUtils.TranslateToBuildPath(manifestVersion.FileName);
                if (!File.Exists(manifestFilePath))
                    continue;

                Manifest manifest = ManifestHandler.LoadManifest(manifestFilePath);
                ManifestConfig manifestConfig = BuildUtils.GetManifestConfig(manifestVersion.Name);
                if (!manifestConfig)
                    continue;

                // 记录清单文件信息
                versionFileInfoList.Add(new VersionFileInfo()
                {
                    name = manifestVersion.Name,
                    hash = manifestVersion.Hash,
                    size = manifestVersion.Size
                });

                // 计算资源和组的对照表, 方便下面获取Bundle所在的组(在本次构建的清单中或者没有加载到上个版本的记录才进行收集)
                Dictionary<string, string> assetPathToGroupName = new Dictionary<string, string>();
                if (hasManifestBuild.ContainsKey(manifestVersion.Name) || oldBuildRecord == null)
                {
                    List<Group> groupList = manifestConfig.groups;
                    int groupListCount = groupList.Count;
                    for (int i = 0; i < groupListCount; i++)
                    {
                        Group group = groupList[i];
                        if (!group.IsNeedBuild)
                            continue;

                        string groupName = group.note;
                        EditorUtility.DisplayProgressBar($"正在收集清单({manifestVersion.Name})资源({i + 1}/{groupListCount})", groupName, (float)(i + 1) / groupListCount);
                        string[] assetPathList = group.GetAssetPathList();
                        foreach (string path in assetPathList)
                        {
                            string assetPath = path;
                            if (group.isExternalPath)
                                assetPath = BuildUtils.GetExternalRawFileLoadPath(assetPath, group.externalPath, group.placeFolderName);
                            assetPathToGroupName.TryAdd(assetPath, groupName);
                        }
                    }

                    EditorUtility.ClearProgressBar();
                }

                // Bundle信息记录赋值
                List<RecordBundleInfo> recordBundleInfoList = new List<RecordBundleInfo>();
                List<ManifestBundleInfo> bundleInfoList = manifest.manifestBundleInfoList;
                int bundleInfoListCount = bundleInfoList.Count;
                for (int i = 0; i < bundleInfoListCount; i++)
                {
                    ManifestBundleInfo bundleInfo = bundleInfoList[i];
                    EditorUtility.DisplayProgressBar($"正在记录清单({manifestVersion.Name})数据({i + 1}/{bundleInfoListCount})", bundleInfo.Name, (float)(i + 1) / bundleInfoListCount);

                    RecordBundleInfo recordBundleInfo;
                    string bundleName = !bundleInfo.IsRawFile ? bundleInfo.Name : bundleInfo.AssetPathList[0];
                    if (bundleNameToOldBundleInfo.TryGetValue(bundleName, out RecordBundleInfo oldRecordBundleInfo) && oldRecordBundleInfo.Hash == bundleInfo.Hash)
                        recordBundleInfo = oldRecordBundleInfo;
                    else
                    {
                        recordBundleInfo = new RecordBundleInfo()
                        {
                            Name = bundleName,
                            Size = bundleInfo.Size,
                            Hash = bundleInfo.Hash,
                            IsRawFile = bundleInfo.IsRawFile,
                            Group = assetPathToGroupName.TryGetValue(bundleInfo.AssetPathList[0], out string groupName) ? groupName : "自动分组"
                        };
                        if (!bundleInfo.IsRawFile)
                        {
                            List<RecordAssetInfo> assetInfoList = new List<RecordAssetInfo>();
                            foreach (string assetPath in bundleInfo.AssetPathList)
                            {
                                string fullPath = AssetPath.CombinePath(System.Environment.CurrentDirectory, assetPath);
                                if (File.Exists(fullPath))
                                {
                                    assetInfoList.Add(new RecordAssetInfo
                                    {
                                        AssetPath = assetPath,
                                        Size = new FileInfo(fullPath).Length,
                                        Hash = Utility.ComputeHash(fullPath)
                                    });
                                }
                                else
                                    assetInfoList.Add(new RecordAssetInfo { AssetPath = assetPath });
                            }

                            recordBundleInfo.RecordAssetInfoList = assetInfoList.ToArray();
                        }
                    }

                    recordBundleInfoList.Add(recordBundleInfo);
                }

                EditorUtility.ClearProgressBar();

                recordManifestList.Add(new RecordManifest()
                {
                    name = manifestVersion.Name,
                    recordBundleInfoList = recordBundleInfoList.ToArray()
                });
            }

            BuildRecord buildRecord = new BuildRecord()
            {
                version = versionContainer.Version,
                timestamp = versionContainer.Timestamp,
                versionFileInfoList = versionFileInfoList.ToArray(),
                recordManifestList = recordManifestList.ToArray()
            };

            string saveFilePath = AssetPath.CombinePath(saveFileDirectoryPath, $"{BuildUtils.BuildRecordFilePrefix}{buildRecord.version}_{buildRecord.timestamp}.json");
            File.WriteAllText(saveFilePath, JsonUtility.ToJson(buildRecord));
            return true;
        }

        #endregion

        #region 首包资源环境配置

        /// <summary>
        /// 首包资源环境配置
        /// </summary>
        /// <returns>是否完成</returns>
        public static bool ArrangeBuildInFilesEnvironment()
        {
            // 1.清空StreamingAssets的首包资源文件夹
            FileUtil.DeleteFileOrDirectory(BuildUtils.BuildLocalDataPath);

            // 2.复制所有首包资源文件到StreamingAssets目录
            bool isOk = CopyBundlesToStreamingAssets();
            if (!isOk)
            {
                FileUtil.DeleteFileOrDirectory(BuildUtils.BuildLocalDataPath);
                return false;
            }

            // 3.设置AssetSettings里的首包资源列表和清单列表字段
            SetBuildInFileListOfAssetSettings();

            Debug.Log("首包资源环境配置完成");
            return true;
        }

        /// <summary>
        /// 将指定文件从打包目录复制到StreamingAssets目录
        /// </summary>
        static void CopyBuildPathFileToStreamingAssetsPath(string fromFileName, string destFileName = null)
        {
            // 不传入目标文件名时使用源文件名
            destFileName ??= fromFileName;

            string srcFilePath = BuildUtils.TranslateToBuildPath(fromFileName);
            string destFilePath = AssetPath.CombinePath(BuildUtils.BuildLocalDataPath, destFileName);
            if (!File.Exists(srcFilePath))
            {
                Debug.LogWarning($"所需首包文件{srcFilePath}不存在, 请检查原因(例:是否打成游戏安装包前没有构建资源包？)");
                return;
            }

            string destFolderPath = Path.GetDirectoryName(destFilePath);
            if (!Directory.Exists(destFolderPath))
                Directory.CreateDirectory(destFolderPath);

            File.Copy(srcFilePath, destFilePath, true);
        }

        /// <summary>
        /// 获取当前已打包的清单版本列表
        /// </summary>
        static List<ManifestVersion> GetBuildManifestVersionList()
        {
            ManifestVersionContainer versionContainer = null;
            string versionContainerFilePath = BuildUtils.TranslateToBuildPath(ManifestHandler.VersionFileName);
            if (File.Exists(versionContainerFilePath))
                versionContainer = ManifestHandler.LoadManifestVersionContainer(versionContainerFilePath);
            return versionContainer != null ? versionContainer.AllManifestVersions : null;
        }

        /// <summary>
        /// 复制所有首包资源文件到StreamingAssets目录
        /// </summary>
        /// <returns>是否完成</returns>
        static bool CopyBundlesToStreamingAssets()
        {
            List<ManifestVersion> manifestVersions = GetBuildManifestVersionList();
            if (manifestVersions == null)
                return true;

            // 版本文件
            CopyBuildPathFileToStreamingAssetsPath(ManifestHandler.VersionFileName);

            foreach (ManifestVersion manifestVersion in manifestVersions)
            {
                Manifest manifest = ManifestHandler.LoadManifest(BuildUtils.TranslateToBuildPath(manifestVersion.FileName));
                if (manifest == null)
                    continue;

                // 清单文件
                CopyBuildPathFileToStreamingAssetsPath(manifestVersion.FileName);

                int count = manifest.manifestBundleInfoList.Count;
                for (int i = 0; i < count; i++)
                {
                    ManifestBundleInfo bundleInfo = manifest.manifestBundleInfoList[i];
                    bool isCancel = EditorUtility.DisplayCancelableProgressBar($"正在复制文件到StreamingAssets({i + 1}/{count})", bundleInfo.NameWithHash, (float)(i + 1) / count);
                    if (isCancel)
                    {
                        Debug.Log("已取消复制首包资源");
                        EditorUtility.ClearProgressBar();
                        return false;
                    }

                    if (bundleInfo.IsRawFile) // 原始文件使用不带hash的原文件名复制到StreamingAssets
                        CopyBuildPathFileToStreamingAssetsPath(bundleInfo.NameWithHash, bundleInfo.Name);
                    else
                        CopyBuildPathFileToStreamingAssetsPath(bundleInfo.NameWithHash);
                }
            }

            // 刷新Project视图文件显示
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            Debug.Log("第一步:复制所有首包资源文件到StreamingAssets目录——————完成");

            return true;
        }

        /// <summary>
        /// 设置AssetSettings里的首包资源打包文件列表字段
        /// </summary>
        static void SetBuildInFileListOfAssetSettings()
        {
            List<string> buildInFileNameList = new List<string>(5000);

            List<ManifestVersion> manifestVersions = GetBuildManifestVersionList();
            if (manifestVersions != null)
            {
                // 加入当前所有清单所带有的打包文件
                foreach (ManifestVersion manifestVersion in manifestVersions)
                {
                    Manifest manifest = ManifestHandler.LoadManifest(BuildUtils.TranslateToBuildPath(manifestVersion.FileName));
                    if (!manifest)
                        continue;

                    // 所有构建的打包文件, 包含原始文件(此处的原始文件使用带hash的名字作为记录, 因判断首包文件是否存在时是用nameWithHash判断)
                    foreach (ManifestBundleInfo bundleInfo in manifest.manifestBundleInfoList)
                        buildInFileNameList.Add(bundleInfo.NameWithHash);
                }
            }

            AssetSettings settings = BuildUtils.GetOrCreateAssetSettings();
            settings.buildInBundleFileNameList = buildInFileNameList.Count > 0 ? buildInFileNameList.ToArray() : null;

            // 保存修改
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

            Debug.Log("第二步:设置AssetSettings里的首包资源打包文件列表字段——————完成");
        }

        #endregion

        #region 跨清单资源检查

        /// <summary>
        /// 开始进行同一资源跨清单打包检测
        /// 防止两个清单同时包含了一个资源, 造成冗余, 通常在进行清单配置时应尽量将有关联的资源都放在一个清单里
        /// </summary>
        public static void StartAssetsInMultiManifestCheck()
        {
            List<ManifestConfig> manifestConfigs = BuildUtils.GetAllManifestConfigs();
            if (manifestConfigs.Count <= 1)
            {
                Debug.Log("没有2个或以上的清单, 无需进行检查");
                return;
            }

            Dictionary<string, List<string>> assetPathToManifestNameList = new Dictionary<string, List<string>>();

            // 添加资源所属的清单记录到字典中
            void AddAssetRecord(string assetPath, string manifestName)
            {
                if (!assetPathToManifestNameList.TryGetValue(assetPath, out var nameList))
                {
                    nameList = new List<string>();
                    assetPathToManifestNameList.Add(assetPath, nameList);
                }

                if (!nameList.Contains(manifestName))
                    nameList.Add(manifestName);
            }

            foreach (ManifestConfig config in manifestConfigs)
            {
                string configName = config.name;
                int count = config.groups.Count;
                for (int i = 0; i < count; i++)
                {
                    Group group = config.groups[i];
                    if (!group.IsNeedBuild)
                        continue;

                    bool isCancel = EditorUtility.DisplayCancelableProgressBar($"正在计算资源归属({i}/{count})", $"{configName},{group.note}", (float)i / count);
                    if (isCancel)
                    {
                        Debug.Log("已取消检查");
                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    BuildAssetInfo[] assetInfoList = group.CollectAssets();
                    if (group.bundleMode != BundleMode.原始文件)
                    {
                        int length = assetInfoList.Length;
                        for (int j = 0; j < length; j++)
                        {
                            BuildAssetInfo info = assetInfoList[j];
                            isCancel = EditorUtility.DisplayCancelableProgressBar($"正在计算资源归属({i + 1}/{count})", $"{configName},{group.note}", (float)i / count + (float)(j + 1) / length / count);
                            if (isCancel)
                            {
                                Debug.Log("已取消检查");
                                EditorUtility.ClearProgressBar();
                                return;
                            }

                            AddAssetRecord(info.path, configName);
                            foreach (string path in BuildUtils.GetDependencies(info.path))
                                AddAssetRecord(path, configName);
                        }
                    }
                    else
                        foreach (BuildAssetInfo info in assetInfoList)
                            AddAssetRecord(info.path, configName);
                }
            }

            EditorUtility.ClearProgressBar();

            bool hasMultiReference = false;

            foreach (var item in assetPathToManifestNameList)
            {
                int listCount = item.Value.Count;
                if (listCount > 1)
                {
                    hasMultiReference = true;
                    string manifestNames = string.Empty;
                    for (int i = 0; i < listCount; i++)
                    {
                        manifestNames += item.Value[i];
                        if (i < listCount - 1)
                            manifestNames += ",";
                    }

                    Debug.Log($"{item.Key}被多个清单引用({manifestNames})");
                }
            }

            if (!hasMultiReference)
                Debug.Log("检测完成, 清单配置正常, 没有被多个清单引用的资源");
            else
                Debug.Log("检测到存在被多个清单引用的资源, 请及时在清单配置中进行修改");
        }

        #endregion

        #region 修改打包版本

        /// <summary>
        /// 修改清单容器版本
        /// </summary>
        public static void ChangeManifestVersionContainerVersion()
        {
            ChangeBuildVersionWindow.Open();
        }

        #endregion
    }
}