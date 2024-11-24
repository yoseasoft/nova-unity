using UnityEngine;
using UnityEditor;
using AssetModule.Editor.GUI;
using AssetModule.Editor.Build;

namespace AssetModule.Editor
{
    /// <summary>
    /// Unity菜单选项
    /// </summary>
    public static class AssetManagementMenuItems
    {
        /*
        [MenuItem("资源管理/构建资源包", false, 0)]
        static void BuildAllManifests()
        {
            BuildExecuter.StartBuild();
        }

        [MenuItem("资源管理/首包资源环境配置", false, 1)]
        static void ArrangeBuildInFilesEnvironment()
        {
            BuildExecuter.ArrangeBuildInFilesEnvironment();
        }

        [MenuItem("资源管理/手动修改打包版本", false, 2)]
        static void ChangeBuildVersion()
        {
            BuildExecuter.ChangeManifestVersionContainerVersion();
        }

        [MenuItem("资源管理/清理过期文件", false, 3)]
        static void ClearHistoryFiles()
        {
            BuildExecuter.ClearHistoryFiles();
        }

        [MenuItem("资源管理/资源打包分析", false, 101)]
        static void OpenBuildAnalyzerWindow()
        {
            BuildAnalyzerWindow.Open();
        }

        [MenuItem("资源管理/资源运行分析", false, 102)]
        static void OpenRuntimeAnalyzerWindow()
        {
            RuntimeAnalyzerWindow.Open();
        }

        [MenuItem("资源管理/查看版本变更", false, 103)]
        static void OpenVersionContrastWindow()
        {
            VersionContrastWindow.Open();
        }

        [MenuItem("资源管理/多清单资源引用检查", false, 104)]
        static void StartAssetsInMultiManifestCheck()
        {
            BuildExecuter.StartAssetsInMultiManifestCheck();
        }

        [MenuItem("资源管理/多清单资源引用检查", true, 104)]
        static bool IsNeedAssetsInMultiManifestCheck()
        {
            return BuildUtils.GetAllManifestConfigs().Count > 1;
        }

        [MenuItem("资源管理/选中资源清单配置", false, 201)]
        static void SelectManifestConfig()
        {
            BuildUtils.SelectManifestConfig();
        }

        [MenuItem("资源管理/打开打包目录", false, 301)]
        static void OpenBuildFolder()
        {
            EditorUtility.OpenWithDefaultApp(BuildUtils.PlatformBuildPath);
        }

        [MenuItem("资源管理/打开上传目录", false, 302)]
        static void OpenUploadFolder()
        {
            EditorUtility.OpenWithDefaultApp(BuildUtils.PlatformUploadPath);
        }

        [MenuItem("资源管理/打开下载目录", false, 303)]
        static void OpenDownloadFolder()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }

        [MenuItem("资源管理/打开临时目录", false, 304)]
        static void OpenTempFolder()
        {
            EditorUtility.OpenWithDefaultApp(Application.temporaryCachePath);
        }
        */
    }
}