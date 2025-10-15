using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

/// <summary>
/// FBX导入处理
/// </summary>
public class ModelPostprocessor : AssetPostprocessor
{
    /// <summary>
    /// 美术资源基础目录
    /// </summary>
    static string resourceBasePath = "Assets/_Resources";

    /// <summary>
    /// 模型资源基础目录
    /// </summary>
    static string modelBasePath = $"{resourceBasePath}/model";

    /// <summary>
    /// 特效资源基础目录
    /// </summary>
    static string effectBasePath = $"{resourceBasePath}/effect";

    static string readwriteMark = "_rw";

    #region 模型导入设置

    /// <summary>
    /// 导入模型(.fbx)之前通知, 由Unity底层调用
    /// https://docs.unity.cn/cn/current/ScriptReference/AssetPostprocessor.OnPreprocessModel.html
    /// </summary>
    void OnPreprocessModel()
    {
        if (!assetPath.StartsWith(resourceBasePath))
            return;

        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (!modelImporter)
            return;

        //isReadable默认情况下模型不需要可读，只有粒子系统选择Mesh模式的模型需要开启read/wrtite
        modelImporter.isReadable = assetPath.Contains(readwriteMark);

        modelImporter.importCameras = false;
        modelImporter.importLights = false;
        modelImporter.importVisibility = false;

        // 不导入材质
        modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;

        // 关闭动画重新采样(部分动画不能关闭, 暂时找不到判断方法, 故直接写Legacy类型)
        if (modelImporter.animationType != ModelImporterAnimationType.Legacy)
            modelImporter.resampleCurves = false;

        // 动画按最佳压缩方式处理
        if (modelImporter.animationType == ModelImporterAnimationType.Generic || modelImporter.animationType == ModelImporterAnimationType.Human)
            modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal; // Optimal仅支持通用和人形动画
        else
            modelImporter.animationCompression = ModelImporterAnimationCompression.KeyframeReductionAndCompression;
    }

    void OnPostprocessModel(GameObject gameObject)
    {
        // 清空模型里的材质, 以免使用了Standard Shader
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
        if (renderers != null && renderers.Length > 0)
            foreach (Renderer renderer in renderers)
                renderer.sharedMaterials = new Material[renderer.sharedMaterials.Length];

        // 模型目录的动作在OnPostprocessAllAssets处理, 这里不再处理
        if (assetPath.StartsWith(modelBasePath) && Path.GetFileNameWithoutExtension(assetPath).Contains("@"))
            return;

        // 动画精度优化
        List<AnimationClip> animationClipList = new List<AnimationClip>(AnimationUtility.GetAnimationClips(gameObject));
        if (animationClipList.Count == 0)
        {
            AnimationClip[] objectList = UnityEngine.Object.FindObjectsOfType(typeof(AnimationClip)) as AnimationClip[];
            animationClipList.AddRange(objectList);
        }

        foreach (AnimationClip animationClip in animationClipList)
            if (animationClip)
                // AnimationClipOptimize.OptimizeAnimationClip(animationClip, true);

        // 非模型文件夹下的FBX, 若本身没有动画, 则关闭动画导入设置
        if (animationClipList.Count == 0 && !assetPath.StartsWith(modelBasePath))
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            if (!modelImporter)
                return;

            modelImporter.importAnimation = false;
        }
    }

    #endregion

    #region 动作文件检查(目录、命名)、AnimatorController自动赋值处理

    /// <summary>
    /// (动作文件检查, AnimatorController自动赋值)的功能开关
    /// </summary>
    //static bool IsOpenAnimationAutoCheck => ModelAnimationNameConfig.Instance;
    // static bool IsOpenAnimationAutoCheck = false;//=> ModelAnimationNameConfig.Instance;

    /// <summary>
    /// 动画规范放置文件夹名字
    /// </summary>
    const string animationFolderName = "animation";

    /// <summary>
    /// 是否需要刷新文件视图
    /// </summary>
    static bool isNeedRefresh = false;

    /// <summary>
    /// 错误列表
    /// </summary>
    static List<string> errorList = new List<string>();

    /// <summary>
    /// 等待排序的AnimatorController目录列表
    /// </summary>
    static List<string> waitingSortAnimatorControllerPathList = new List<string>();

    /// <summary>
    /// 已检查过的动作目录
    /// </summary>
    static Dictionary<string, bool> checkedAnimFolderMap = new Dictionary<string, bool>();

    /// <summary>
    /// 资源变化通知接口(由Unity底层调用)
    /// </summary>
    /// <param name="importedAssets">新增的资源</param>
    /// <param name="deletedAssets">删除的资源</param>
    /// <param name="movedAssets">移动的资源</param>
    /// <param name="movedFromAssetPaths">移动的资源原始目录</param>
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //if (!IsOpenAnimationAutoCheck)
        //    return;

        errorList.Clear();
        checkedAnimFolderMap.Clear();
        waitingSortAnimatorControllerPathList.Clear();

        foreach (string assetPath in importedAssets)
            ProcessNewOrMovedAsset(assetPath);
        foreach (string assetPath in movedAssets)
            ProcessNewOrMovedAsset(assetPath);

        foreach (string assetPath in deletedAssets)
            ProcessDeleteOrMovedFromAsset(assetPath);
        foreach (string assetPath in movedFromAssetPaths)
            ProcessDeleteOrMovedFromAsset(assetPath);

        foreach (string path in waitingSortAnimatorControllerPathList)
            SortAndSaveAnimatorStates(path);

        if (isNeedRefresh || waitingSortAnimatorControllerPathList.Count > 0)
        {
            isNeedRefresh = false;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            waitingSortAnimatorControllerPathList.Clear();
        }

        checkedAnimFolderMap.Clear();

        if (errorList.Count > 0)
        {
            foreach (string error in errorList)
                Debug.LogError(error);

            errorList.Clear();
            EditorApplication.ExecuteMenuItem("Window/General/Console");
        }
    }

    /// <summary>
    /// 添加错误(去重)
    /// </summary>
    static void AddError(string text)
    {
        if (!errorList.Contains(text))
            errorList.Add(text);
    }

    /// <summary>
    /// 处理新资源或移动的资源
    /// </summary>
    static void ProcessNewOrMovedAsset(string assetPath)
    {
        if (assetPath.StartsWith(modelBasePath) && Path.GetFileNameWithoutExtension(assetPath).Contains("@") && (assetPath.EndsWith(".FBX") || assetPath.EndsWith(".anim")))
            ProcessNewRoleAnimation(assetPath);
    }

    /// <summary>
    /// 处理删除的资源或移动资源的原目录
    /// </summary>
    static void ProcessDeleteOrMovedFromAsset(string assetPath)
    {
        if (assetPath.StartsWith(modelBasePath) && assetPath.EndsWith(".anim") && Path.GetFileNameWithoutExtension(assetPath).Contains("@"))
            ProcessDeletedRoleAnimation(assetPath);
    }

    /// <summary>
    /// 获取AnimatorController上的StateMachine(复制自internal函数:AnimatorController.FindEffectiveRootStateMachine(int layerIndex))
    /// </summary>
    static AnimatorStateMachine FindEffectiveRootStateMachine(AnimatorController animatorController)
    {
        AnimatorControllerLayer animatorControllerLayer = animatorController.layers[0];
        while (animatorControllerLayer.syncedLayerIndex != -1)
            animatorControllerLayer = animatorController.layers[animatorControllerLayer.syncedLayerIndex];
        return animatorControllerLayer.stateMachine;
    }

    /// <summary>
    /// 检查并获取指定目录的AnimatorController目录
    /// </summary>
    static string CheckAndGetAnimatorControllerPath(string assetPath, bool quiet = false)
    {
        string directoryPath = Path.GetDirectoryName(assetPath);
        string animatorControllerName = new FileInfo(assetPath).Directory.Parent.Name; // 控制器的名字就以动画文件夹的父文件夹名字作为规范

        if (!quiet && !checkedAnimFolderMap.ContainsKey(directoryPath))
        {
            checkedAnimFolderMap.Add(directoryPath, true);
            string[] allAnimatorControllerGUID = AssetDatabase.FindAssets($"t:{typeof(AnimatorController).Name}", new string[] { directoryPath });
            foreach (string guid in allAnimatorControllerGUID)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!Path.GetFileNameWithoutExtension(path).ToLower().Equals(animatorControllerName.ToLower()))
                    AddError($"检测到多余或命名不规范的AnimatorController位置:{path}");
            }
        }

        return $"{Path.Combine(directoryPath, animatorControllerName)}.controller";
    }

    /// <summary>
    /// 检查并获取指定目录的AnimatorController
    /// </summary>
    static AnimatorStateMachine CheckAndGetAnimatorStateMachine(string assetPath)
    {
        string animationControllerPath = CheckAndGetAnimatorControllerPath(assetPath);
        AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(animationControllerPath);
        if (!animatorController)
            animatorController = AnimatorController.CreateAnimatorControllerAtPath(animationControllerPath);

        if (animatorController.layers.Length == 0)
            animatorController.AddLayer("Base Layer");

        return FindEffectiveRootStateMachine(animatorController);
    }

    /// <summary>
    /// 处理新的角色动作FBX文件或动作文件
    /// </summary>
    static void ProcessNewRoleAnimation(string assetPath)
    {
        // 目录放置规范检查
        FileInfo fileInfo = new FileInfo(assetPath);
        string directoryName = fileInfo.Directory.Name;
        if (!directoryName.Equals(animationFolderName))
        {
            AddError($"动作文件({fileInfo.Name})请放在animation文件夹中, 现放在了{directoryName}文件夹中, 资源位置:{assetPath}");
            return;
        }

        // 动作名合规检查
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
        string animName = fileNameWithoutExtension.Split('@')[1];

        //var modelAnimationNameConfig = ModelAnimationNameConfig.Instance;
        //if (modelAnimationNameConfig && !modelAnimationNameConfig.legalModelAnimationNames.Exists(info => info.name == animName))
        //{
        //    AddError($"动作文件({fileInfo.Name})命名({animName})不合规, 也可能此动作名从未在合规动作名中配置, 资源位置:{assetPath}, 动作名配置目录:{AssetDatabase.GetAssetPath(modelAnimationNameConfig)}");
        //    return;
        //}

        AnimatorStateMachine animatorStateMachine = CheckAndGetAnimatorStateMachine(assetPath);
        if (!animatorStateMachine)
        {
            Debug.LogWarning($"No animatorStateMachine???, {assetPath}");
            return;
        }

        AnimationClip clipOfImportedAsset = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
        if (!clipOfImportedAsset)
        {
            AddError($"资源:{assetPath}中不存在动画文件");
            return;
        }

        bool isNeedSetLoop = false; // modelAnimationNameConfig && (modelAnimationNameConfig.loopAnimationNamesForEquals.Exists(info => animName == info.name) || modelAnimationNameConfig.loopAnimationNamesForContains.Exists(info => animName.Contains(info.name)));

        // 在AnimatorController中寻找此动画, 没有则自动添加
        ChildAnimatorState childAnimatorState = animatorStateMachine.states.ToList().Find(info => info.state && info.state.name == animName);
        if (!childAnimatorState.state || !childAnimatorState.state.motion)
        {
            isNeedRefresh = true;
            AnimatorState state = childAnimatorState.state != null ? childAnimatorState.state : animatorStateMachine.AddState(animName);
            string animClipAssetPath = assetPath;
            state.motion = assetPath.EndsWith(".FBX") ? TransferAndOptimizeAnimFbxToAnimation(assetPath, isNeedSetLoop, out animClipAssetPath) : clipOfImportedAsset;

            // 添加后进行排序
            string animatorControllerPath = CheckAndGetAnimatorControllerPath(animClipAssetPath, true);
            if (!waitingSortAnimatorControllerPathList.Contains(animatorControllerPath))
                waitingSortAnimatorControllerPathList.Add(animatorControllerPath);
        }
        // 若已经存在
        else
        {
            // 判断是否有重复
            if (Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(childAnimatorState.state.motion)) != Path.GetFileNameWithoutExtension(assetPath))
            {
                AddError($"文件夹中存在两个命名相同的动作文件({animName}), 已忽略{assetPath}");
                return;
            }

            // 已经存在的动画文件不用再作转换处理, 仅检测循环配置
            if (assetPath.EndsWith(".anim"))
            {
                if (isNeedSetLoop)
                {
                    AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clipOfImportedAsset);
                    if (!settings.loopTime)
                    {
                        isNeedRefresh = true;
                        settings.loopTime = true;
                        AnimationUtility.SetAnimationClipSettings(clipOfImportedAsset, settings);
                        EditorUtility.SetDirty(clipOfImportedAsset);
                    }
                }

                return;
            }

            // 若是FBX，则转换成动画文件
            isNeedRefresh = true;
            childAnimatorState.state.motion = TransferAndOptimizeAnimFbxToAnimation(assetPath, isNeedSetLoop, out string _);
        }
    }

    /// <summary>
    /// 处理删除的角色动作文件
    /// </summary>
    static void ProcessDeletedRoleAnimation(string assetPath)
    {
        // 非动作文件夹下删除的文件不作处理
        if (!new FileInfo(assetPath).Directory.Name.Equals(animationFolderName))
            return;

        // 删除时没有Controller时不处理
        if (!File.Exists(CheckAndGetAnimatorControllerPath(assetPath, true)))
            return;

        AnimatorStateMachine animatorStateMachine = CheckAndGetAnimatorStateMachine(assetPath);
        if (!animatorStateMachine)
        {
            Debug.LogWarning($"No animatorStateMachine???, {assetPath}");
            return;
        }

        string removeAnimName = Path.GetFileNameWithoutExtension(assetPath).Split('@')[1];
        ChildAnimatorState childAnimatorState = animatorStateMachine.states.ToList().Find(info => info.state && info.state.name == removeAnimName);
        if (childAnimatorState.state && !childAnimatorState.state.motion) // 确定删除的时原来的动画文件才进行移除, 避免@符号后名字一样, 导致错误删除(例:错误同时放入role01@idle和role02@idle, 然后删除其中一个)
            animatorStateMachine.RemoveState(childAnimatorState.state);

        // 移除后进行排序
        string animatorControllerPath = CheckAndGetAnimatorControllerPath(assetPath, true);
        if (!waitingSortAnimatorControllerPathList.Contains(animatorControllerPath))
            waitingSortAnimatorControllerPathList.Add(animatorControllerPath);
    }

    /// <summary>
    /// 根据名字排序, 并保存修改
    /// </summary>
    static void SortAndSaveAnimatorStates(string animatorControllerPath)
    {
        AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorControllerPath);
        if (!animatorController)
            return;

        AnimatorStateMachine animatorStateMachine = FindEffectiveRootStateMachine(animatorController);
        if (!animatorStateMachine)
            return;

        // 清理无用的State
        List<AnimatorState> illegalStates = new List<AnimatorState>();
        foreach (ChildAnimatorState item in animatorStateMachine.states)
            if (!item.state.motion)
                illegalStates.Add(item.state);
        foreach (AnimatorState state in illegalStates)
            animatorStateMachine.RemoveState(state);

        // 自动排列
        ChildAnimatorState[] childAnimatorStates = animatorStateMachine.states;
        List<string> stateNameList = Array.ConvertAll(childAnimatorStates, info => info.state.name).ToList();
        stateNameList.Sort();

        // 根据优先配置排序
        //var modelAnimationNameConfig = ModelAnimationNameConfig.Instance;
        //if (modelAnimationNameConfig != null)
        //{
        //    var priorList = modelAnimationNameConfig.priorModelAnimationNames;
        //    for (int i = priorList.Count - 1; i >= 0; i--)
        //    {
        //        string priorName = priorList[i].name;
        //        if (stateNameList.Contains(priorName))
        //        {
        //            int index = stateNameList.IndexOf(priorName);
        //            stateNameList.RemoveAt(index);
        //            stateNameList.Insert(0, priorName);
        //        }
        //    }
        //}

        // 根据排序自动设置Animator窗口中的状态位置
        List<ChildAnimatorState> childAnimatorStateList = new List<ChildAnimatorState>();
        foreach (string name in stateNameList)
            childAnimatorStateList.Add(childAnimatorStates.First(info => info.state.name == name));

        // 初始位置(30, 300, 0), 行距50, 列距270
        for (int i = 0; i < stateNameList.Count; i++)
        {
            ChildAnimatorState temp = childAnimatorStateList[i];
            temp.position = new Vector3(30 + i / 10 * 270, 300 + (i % 10) * 50, 0);
            childAnimatorStateList[i] = temp;
        }

        animatorStateMachine.states = childAnimatorStateList.ToArray();
        if (childAnimatorStateList.Count > 0)
            animatorStateMachine.defaultState = childAnimatorStateList[0].state;

        EditorUtility.SetDirty(animatorController);
    }

    #endregion

    #region 将FBX转换成动作文件并进行优化的相关逻辑

    /// <summary>
    /// 设置动作为循环
    /// </summary>
    static void SetAnimationClipLoop(AnimationClip clip)
    {
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
    }

    /// <summary>
    /// 将带动作的Fbx文件转换成动画文件并对其进行优化
    /// </summary>
    static AnimationClip TransferAndOptimizeAnimFbxToAnimation(string assetPath, bool isNeedSetLoop, out string newClipAssetPath)
    {
        AnimationClip newClip;
        string fbxFileName = Path.GetFileNameWithoutExtension(assetPath);
        newClipAssetPath = Path.Combine(Path.GetDirectoryName(assetPath), $"{fbxFileName}.anim");
        AnimationClip animClipFromFbx = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
        if (File.Exists(newClipAssetPath))
        {
            // 覆盖原来的动画文件并进行优化
            newClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(newClipAssetPath);
            bool isLoop = newClip.isLooping;
            EditorUtility.CopySerialized(animClipFromFbx, newClip);
            newClip.name = fbxFileName;
            if (isLoop)
                SetAnimationClipLoop(newClip);
            // AnimationClipOptimize.OptimizeAnimationClip(newClip);
            if (isNeedSetLoop)
                SetAnimationClipLoop(newClip);
        }
        else
        {
            // 新建一个动画文件并进行优化
            newClip = new AnimationClip();
            EditorUtility.CopySerialized(animClipFromFbx, newClip);
            newClip.name = fbxFileName;
            // AnimationClipOptimize.OptimizeAnimationClip(newClip);
            if (isNeedSetLoop)
                SetAnimationClipLoop(newClip);
            AssetDatabase.CreateAsset(newClip, newClipAssetPath);
        }

        // 删除FBX文件
        AssetDatabase.DeleteAsset(assetPath);
        return newClip;
    }

    #endregion
}