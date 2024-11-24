using UnityEngine;
using Sirenix.OdinInspector;

namespace AssetModule
{
    /// <summary>
    /// 资源运行基础配置
    /// </summary>
    // [CreateAssetMenu(menuName = "资源管理/资源基础设置", fileName = "AssetSettings")]// (因只创建一个就足够, 创建后不再在菜单显示, 故屏蔽此行代码)
    public class AssetSettings : ScriptableObject
    {
#if UNITY_EDITOR
        [Title("配置(仅编辑器下使用)")]
        [LabelText("资源加载模式"), DrawWithUnity]
        public EditorAssetLoadMode editorAssetLoadMode;

        [LabelText("资源加载Log开关")]
        public bool isEnableLog;
#endif

        /// <summary>
        /// 离线模式
        /// </summary>
        [Title("配置")]
        [LabelText("离线模式"), InfoBox("离线模式字段仅在资源使用正式加载模式时生效, 其他情况默认为true")]
        public bool offlineMode;

        /// <summary>
        /// 内置资源包文件(包含已构建的原始文件)信息列表
        /// </summary>
        [LabelText("首包资源打包文件列表"), ReadOnly, ListDrawerSettings(DefaultExpandedState = true), Space(10)]
        public string[] buildInBundleFileNameList;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 编辑器下的资源加载模式
    /// </summary>
    public enum EditorAssetLoadMode
    {
        使用资源目录原文件加载,

        使用打包目录的bundle加载,

        正式加载,
    }
#endif
}