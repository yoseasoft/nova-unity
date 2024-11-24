using System;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 初始化资源清单
    /// </summary>
    public class InitManifestOperation : Operation
    {
        /// <summary>
        /// 创建初始化资源清单
        /// </summary>
        public static Func<InitManifestOperation> CreateInitManifestOperationFunc { get; set; } = DefaultCreateInitManifestOperationFunc;

        /// <summary>
        /// 默认创建初始化清单操作方法
        /// </summary>
        static InitManifestOperation DefaultCreateInitManifestOperationFunc()
        {
            return new InitManifestOperation();
        }

        /// <summary>
        /// 步骤
        /// </summary>
        enum Step
        {
            /// <summary>
            /// 加载内置的清单版本列表信息容器
            /// </summary>
            LoadingBuildInManifestVersionContainer,

            /// <summary>
            /// 加载服务器的清单版本信息容器
            /// </summary>
            LoadingDownloadedManifestVersionContainer,

            /// <summary>
            /// 根据加载的清单版本列表加载所有清单
            /// </summary>
            LoadingAllManifests,
        }

        /// <summary>
        /// 当前运行到的步骤
        /// </summary>
        Step _step;

        /// <summary>
        /// 内置的版本文件保存目录
        /// </summary>
        string _buildInVersionFileSavePath;

        /// <summary>
        /// 内置的清单版本文件信息容器
        /// </summary>
        ManifestVersionContainer _buildInManifestVersionContainer;

        /// <summary>
        /// 内置的版本文件信息复制请求
        /// </summary>
        UnityWebRequest _buildInVersionFileCopyRequest;

        /// <summary>
        /// 已下载的清单版本文件信息容器
        /// </summary>
        ManifestVersionContainer _downloadedManifestVersionContainer;

        /// <summary>
        /// 内置清单文件复制请求列表
        /// </summary>
        List<UnityWebRequest> _buildInManifestFileCopyRequestList;

        /// <summary>
        /// 需加载的清单列表
        /// </summary>
        readonly List<ManifestVersion> _loadingManifestVersionList = new();

        /// <summary>
        /// 内置清单容器加载占比
        /// </summary>
        const float BuildInContainerProportion = 0.5f;

        /// <summary>
        /// 内置清单文件复制占比
        /// </summary>
        const float BuildInManifestCopyProportion = 0.4f;

        /// <summary>
        /// 当前生效的版本
        /// </summary>
        public int EffectiveVersion { get; private set; }

        protected override void OnStart()
        {
            LoadBuildInManifestVersionListAsync();
        }

        protected override void OnUpdate()
        {
            if (Status != OperationStatus.Processing)
                return;

            switch (_step)
            {
                case Step.LoadingBuildInManifestVersionContainer:
                    OnLoadingBuildInManifestVersionList();
                    break;
                case Step.LoadingDownloadedManifestVersionContainer:
                    OnLoadingDownloadedManifestVersionList();
                    break;
                case Step.LoadingAllManifests:
                    OnLoadingAllManifests();
                    break;
            }
        }

        /// <summary>
        /// 从StreamingAssets复制文件到指定目录
        /// </summary>
        static UnityWebRequest CopyFileFromStreamingAssetsAsync(string fromUrl, string toPath)
        {
            if (File.Exists(toPath))
                File.Delete(toPath);

            UnityWebRequest request = UnityWebRequest.Get(fromUrl);
            request.downloadHandler = new DownloadHandlerFile(toPath);
            request.SendWebRequest();
            return request;
        }

        /// <summary>
        /// 异步加载内置版本文件
        /// </summary>
        void LoadBuildInManifestVersionListAsync()
        {
            _buildInVersionFileSavePath = AssetPath.TransferToTempPath(ManifestHandler.VersionFileName);
            string buildInVersionFileUrl = AssetPath.TranslateToLocalDataPathUrl(ManifestHandler.VersionFileName);
            _buildInVersionFileCopyRequest = CopyFileFromStreamingAssetsAsync(buildInVersionFileUrl, _buildInVersionFileSavePath);
            _step = Step.LoadingBuildInManifestVersionContainer;
        }

        /// <summary>
        /// 内置清单版本信息加载中处理
        /// </summary>
        void OnLoadingBuildInManifestVersionList()
        {
            if (!_buildInVersionFileCopyRequest.isDone)
            {
                Progress = _buildInVersionFileCopyRequest.downloadProgress * BuildInContainerProportion;
                return;
            }

            UnityWebRequest request = _buildInVersionFileCopyRequest;
            _buildInVersionFileCopyRequest = null;
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Finish("内置版本文件复制到临时目录失败, " + request.error);
                return;
            }

            _buildInManifestVersionContainer = ManifestHandler.LoadManifestVersionContainer(_buildInVersionFileSavePath);
            if (_buildInManifestVersionContainer is null)
            {
                Finish("内置版本文件加载失败, path:" + _buildInVersionFileSavePath);
                return;
            }

            Progress = BuildInContainerProportion;
            _step = Step.LoadingDownloadedManifestVersionContainer;
        }

        /// <summary>
        /// 下载目录的清单版本信息加载中处理
        /// </summary>
        void OnLoadingDownloadedManifestVersionList()
        {
            string downloadedVersionFileSavePath = AssetPath.TranslateToDownloadDataPath(ManifestHandler.VersionFileName);
            if (File.Exists(downloadedVersionFileSavePath))
                _downloadedManifestVersionContainer = ManifestHandler.LoadManifestVersionContainer(downloadedVersionFileSavePath);

            LoadAllManifests();
        }

        /// <summary>
        /// 根据当前已加载的本地和服务器版本信息加载所有清单
        /// </summary>
        void LoadAllManifests()
        {
            bool isLoadDownloadedVersion = false; // 是否加载下载目录的版本文件

            // 下载目录版本文件的时间戳大于本地版本文件的时间戳时才加载下载目录的
            if (_downloadedManifestVersionContainer is not null && _downloadedManifestVersionContainer.Timestamp >= _buildInManifestVersionContainer.Timestamp)
            {
                isLoadDownloadedVersion = true;
                foreach (ManifestVersion manifestVersion in _downloadedManifestVersionContainer.AllManifestVersions)
                {
                    if (!File.Exists(AssetPath.TranslateToDownloadDataPath(manifestVersion.FileName)))
                    {
                        isLoadDownloadedVersion = false;
                        break;
                    }
                }
            }

            // 保证下载目录有内置的清单文件, 避免清单版本容器有更新, 但部分清单已存在时再次下载(Windows离线模式下忽略)
            // 也可以保证在检查版本时, 若内置清单版本较新, 可直接使用下载目录的内置清单, 无需再从StreamingAssets中复制出来, 此操作在更新清单操作里(UpdateManifestOperation.cs)
            if (!AssetManagement.isOfflineWindows)
            {
                foreach (ManifestVersion manifestVersion in _buildInManifestVersionContainer.AllManifestVersions)
                {
                    if (ManifestHandler.IsManifestFileExist(manifestVersion))
                        continue;

                    string buildInFileUrl = AssetPath.TranslateToLocalDataPathUrl(manifestVersion.FileName);
                    string savePath = AssetPath.TranslateToDownloadDataPath(manifestVersion.FileName);
                    _buildInManifestFileCopyRequestList ??= new List<UnityWebRequest>();
                    _buildInManifestFileCopyRequestList.Add(CopyFileFromStreamingAssetsAsync(buildInFileUrl, savePath));
                }
            }

            List<ManifestVersion> needLoadManifestVersionList;

            if (isLoadDownloadedVersion)
            {
                EffectiveVersion = _downloadedManifestVersionContainer.Version;
                needLoadManifestVersionList = _downloadedManifestVersionContainer.AllManifestVersions;
            }
            else
            {
                EffectiveVersion = _buildInManifestVersionContainer.Version;
                needLoadManifestVersionList = _buildInManifestVersionContainer.AllManifestVersions;
            }

            foreach (ManifestVersion manifestVersion in needLoadManifestVersionList)
                _loadingManifestVersionList.Add(manifestVersion);

            _step = Step.LoadingAllManifests;
        }

        /// <summary>
        /// 所有清单加载中处理
        /// </summary>
        void OnLoadingAllManifests()
        {
            // 加载清单的进度占比
            float loadManifestProportion = 1 - BuildInContainerProportion;

            // 内置清单文件复制到下载目录
            if (_buildInManifestFileCopyRequestList != null)
            {
                if (_buildInManifestFileCopyRequestList.Count > 0)
                {
                    float progress = 0;
                    bool isAllDone = true;
                    foreach (UnityWebRequest request in _buildInManifestFileCopyRequestList)
                    {
                        progress += request.downloadProgress;
                        if (!request.isDone)
                            isAllDone = false;
                    }

                    progress /= _buildInManifestFileCopyRequestList.Count;
                    Progress = BuildInContainerProportion + progress * BuildInManifestCopyProportion;

                    if (!isAllDone)
                        return;

                    foreach (UnityWebRequest request in _buildInManifestFileCopyRequestList)
                    {
                        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                        {
                            Finish($"复制内置的清单文件失败:{request.error}");
                            return;
                        }
                    }

                    _buildInManifestFileCopyRequestList.Clear();
                }

                Progress = BuildInContainerProportion + BuildInManifestCopyProportion;
                loadManifestProportion -= BuildInManifestCopyProportion;
            }

            // 加载清单并进行记录
            for (int i = 0; i < _loadingManifestVersionList.Count; i++)
            {
                ManifestVersion manifestVersion = _loadingManifestVersionList[i];
                string manifestFilePath;
                if (AssetManagement.isOfflineWindows)
                    manifestFilePath = AssetPath.TranslateToLocalDataPath(manifestVersion.FileName);
                else
                    manifestFilePath = AssetPath.TranslateToDownloadDataPath(manifestVersion.FileName);
                Manifest manifest = ManifestHandler.LoadManifest(manifestFilePath);
                if (manifest is null)
                {
                    Finish($"清单文件({manifestVersion.Name})加载失败");
                    return;
                }

                manifest.name = manifestVersion.Name;
                manifest.fileName = manifestVersion.FileName;

                ManifestHandler.RefreshGlobalManifest(manifest);
                _loadingManifestVersionList.RemoveAt(i);
                i--;
                Progress = BuildInContainerProportion + (1 - loadManifestProportion) + (float)(i + 1) / _loadingManifestVersionList.Count * loadManifestProportion;

                if (AssetDispatcher.Instance.IsBusy)
                    return;
            }

            Finish();
        }
    }
}