using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 特效导入处理
/// </summary>
public class EffectPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // 导入资源等于一个时才显示提示, 代表是一个个修改, 多个导入代表SVN更新, 不显示提示
        if (importedAssets.Length != 1 || !EditorPrefs.GetBool("IsOpenEffectSaveTips")
        || !importedAssets[0].EndsWith(".prefab") || !importedAssets[0].StartsWith("Assets/_Resources/effect/"))
            return;

        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(importedAssets[0]);
        if (obj)
            AnalyzePrefabAndShowPlayState(obj);
    }

    /// <summary>
    /// 分析Prefab并显示播放状态
    /// </summary>
    static void AnalyzePrefabAndShowPlayState(GameObject obj)
    {
        var windowType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
        var projectWindow = EditorWindow.GetWindow(windowType);
        if (!projectWindow)
            return;

        var mode = DirectorWrapMode.Loop;

        // 和特效约定:特效的第一层或第二层需要放PlayableDirector, 没有则视为循环特效
        var director = obj.GetComponent<PlayableDirector>();
        if (director)
            mode = director.extrapolationMode;
        else if (obj.transform.childCount > 0)
        {
            director = obj.transform.GetChild(0).GetComponent<PlayableDirector>();
            if (director)
                mode = director.extrapolationMode;
        }

        projectWindow.ShowNotification(new GUIContent($"特效({Path.GetFileNameWithoutExtension(obj.name)})修改成功~ 播放状态为:{mode}"), 2);
    }
}