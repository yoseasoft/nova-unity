using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 音乐导入处理
/// </summary>

public class AudioPostprocessor : AssetPostprocessor
{
    private static string bgmBasePath = "Assets/_Resources/Sound/Bgm/";
    
    void OnPreprocessAudio()
    {
        AudioImporter audioImporter = assetImporter as AudioImporter;
        if (!audioImporter)
            return;
        
        audioImporter.forceToMono = true;

        UpdateTargetSetting(true, "Default", audioImporter);
        
        // Windows
        UpdateTargetSetting(false, "Standalone", audioImporter);
        
        // Android
        UpdateTargetSetting(false,"Android", audioImporter);
        
        // iOS
        UpdateTargetSetting(false,"iPhone", audioImporter);
        
        // WebGL
        UpdateTargetSetting(false,"WebGL", audioImporter);
    }

    void UpdateTargetSetting(bool defaultSetting,string targetName, AudioImporter audioImporter)
    {
        AudioImporterSampleSettings settings = defaultSetting ? audioImporter.defaultSampleSettings :
                                                                audioImporter.GetOverrideSampleSettings(targetName);
        bool isBGM = assetPath.Contains(bgmBasePath);
        if (isBGM)
        {
            //settings.preloadAudioData = false;
            settings.loadType = AudioClipLoadType.Streaming;
        }
        else
        {
            //settings.preloadAudioData = true;
            settings.loadType = AudioClipLoadType.DecompressOnLoad;
        }

        if (defaultSetting)
        {
            audioImporter.defaultSampleSettings = settings;
        }
        else
        {
            audioImporter.SetOverrideSampleSettings(targetName, settings);
        }
    }
}
