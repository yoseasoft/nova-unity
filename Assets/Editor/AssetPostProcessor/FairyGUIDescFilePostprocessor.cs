using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 描述文件导入处理
/// </summary>
public class FairyGUIDescFilePostprocessor : AssetPostprocessor
{
    /// <summary>
    /// 记录运行时是否有描述文件改变
    /// 有改变时等待Unity重新进入编辑状态时才刷新
    /// </summary>
    static bool s_isChangedInPlayMode;

    /// <summary>
    /// 资源变化通知接口(由Unity底层调用)
    /// </summary>
    /// <param name="importedAssets">新增的资源</param>
    /// <param name="deletedAssets">删除的资源</param>
    /// <param name="movedAssets">移动的资源</param>
    /// <param name="movedFromAssetPaths">移动的资源原始目录</param>
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (HasDescriptionFileChanged(importedAssets) || HasDescriptionFileChanged(deletedAssets) || HasDescriptionFileChanged(movedAssets) || HasDescriptionFileChanged(movedFromAssetPaths))
        {
            if (Application.isPlaying)
                s_isChangedInPlayMode = true;
            else
                FairyGUIEditor.EditorToolSet.ReloadPackages();
        }
    }

    /// <summary>
    /// 是否有FairyGUI的描述文件变化
    /// </summary>
    static bool HasDescriptionFileChanged(IEnumerable<string> paths)
    {
        return paths.Any(path => path.EndsWith("_fui.bytes"));
    }

    /// <summary>
    /// Unity初始化完成后添加播放模式改变监听
    /// </summary>
    [InitializeOnLoadMethod]
    static void AddPlayModeStateChangedListener()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    /// <summary>
    /// Unity播放模式变化处理
    /// </summary>
    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // 运行时描述文件有改变, 并且Unity进入编辑模式后, 刷新FairyGUI所有包
        if (s_isChangedInPlayMode && state == PlayModeStateChange.EnteredEditMode)
        {
            s_isChangedInPlayMode = false;
            FairyGUIEditor.EditorToolSet.ReloadPackages();
        }
    }
}