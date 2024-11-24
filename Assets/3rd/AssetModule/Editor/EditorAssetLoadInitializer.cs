using System.IO;
using UnityEngine;
using AssetModule.Editor.Build;
using AssetModule.Editor.Simulation;

namespace AssetModule.Editor
{
    /// <summary>
    /// 编辑器资源加载设置初始化
    /// </summary>
    public static class EditorAssetLoadInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            AssetManagement.customGetOfflineModeFunc = GetOfflineMode;
            AssetPath.PlatformName = BuildUtils.GetActiveBuildPlatformName();

            AssetSettings settings = BuildUtils.GetOrCreateAssetSettings();
            Logger.loggable = settings.isEnableLog;
            switch (settings.editorAssetLoadMode)
            {
                case EditorAssetLoadMode.使用资源目录原文件加载:
                    AssetHandler.CreateAssetFunc = EditorAsset.Create;
                    SceneHandler.CreateSceneFunc = EditorScene.Create;
                    RawFileHandler.CreateRawFileFunc = EditorRawFile.Create;
                    InitManifestOperation.CreateInitManifestOperationFunc = EditorInitManifestOperation.Create;
                    break;
                case EditorAssetLoadMode.使用打包目录的bundle加载:
                    // 将运行时读取的本地目录设置为打包目录
                    AssetPath.LocalDataPath = AssetPath.CombinePath(System.Environment.CurrentDirectory, BuildUtils.PlatformBuildPath);
                    RawFileHandler.CreateRawFileFunc = EditorPackedRawFile.Create;
                    break;
                case EditorAssetLoadMode.正式加载:
                    // 若StreamingAssets文件夹中不存在资源包, 则清空首包资源记录列表
                    if (!Directory.Exists(AssetPath.CombinePath(UnityEngine.Application.streamingAssetsPath, AssetPath.BuildPath)))
                        settings.buildInBundleFileNameList = null;
                    break;
            }
        }

        /// <summary>
        /// 获取离线模式
        /// </summary>
        static bool GetOfflineMode()
        {
            AssetSettings settings = BuildUtils.GetOrCreateAssetSettings();
            return settings.editorAssetLoadMode switch
            {
                EditorAssetLoadMode.使用资源目录原文件加载 => true,
                EditorAssetLoadMode.使用打包目录的bundle加载 => true,
                EditorAssetLoadMode.正式加载 => settings.offlineMode,
                _ => settings.offlineMode,
            };
        }
    }
}