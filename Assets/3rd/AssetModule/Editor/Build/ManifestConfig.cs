using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace AssetModule.Editor.Build
{
    /// <summary>
    /// 清单配置
    /// </summary>
    // [CreateAssetMenu(menuName = "资源管理/资源清单", fileName = "NewManifest")] // (因目前只创建两个就足够, 创建后不再在菜单显示, 故屏蔽此行代码)
    public class ManifestConfig : ScriptableObject
    {
        /// <summary>
        /// AB打包选项
        /// </summary>
        [LabelText("AB打包选项"), DrawWithUnity]
        public BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;

        /// <summary>
        /// 资源组列表
        /// </summary>
        [LabelText("打包资源组列表"), ListDrawerSettings(DefaultExpandedState = true, OnBeginListElementGUI = "ElementTitle")]
        public List<Group> groups = new();

        void ElementTitle(int index)
        {
            EditorGUILayout.LabelField(groups[index].note, EditorStyles.boldLabel);
        }
    }
}