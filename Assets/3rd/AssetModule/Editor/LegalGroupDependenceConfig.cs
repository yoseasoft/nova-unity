using System;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;

namespace AssetModule.Editor
{
    /// <summary>
    /// 组合理引用
    /// </summary>
    [Serializable]
    public class GroupLegalDependence
    {
        /// <summary>
        /// 组名
        /// </summary>
        [LabelText("组名")]
        public string groupName;

        /// <summary>
        /// 该组合理的包引用
        /// </summary>
        [LabelText("合理引用包名列表"), ListDrawerSettings(DefaultExpandedState = true)]
        public List<string> legalBundleList = new List<string>();

        /// <summary>
        /// 合理包名字字典
        /// </summary>
        Dictionary<string, bool> _legalBundleMap;

        /// <summary>
        /// 重置字典
        /// </summary>
        public void Reset()
        {
            _legalBundleMap = null;
        }

        /// <summary>
        /// 判断指定的包是否为合法引用
        /// </summary>
        /// <param name="selfBundleName">自身包名</param>
        /// <param name="depBundleName">引用包名</param>
        public bool IsLegalBundle(string selfBundleName, string depBundleName)
        {
            if (_legalBundleMap == null)
            {
                _legalBundleMap = new Dictionary<string, bool>();
                foreach (string name in legalBundleList)
                    _legalBundleMap[name] = true;
            }

            return _legalBundleMap.ContainsKey(depBundleName) || IsLegalBundleByCustomRules(selfBundleName, depBundleName);
        }

        /// <summary>
        /// 通过自定义规则判断指定的包是否为合法引用, 由子类重写
        /// </summary>
        /// <param name="selfBundleName">自身包名</param>
        /// <param name="depBundleName">引用包名</param>
        protected virtual bool IsLegalBundleByCustomRules(string selfBundleName, string depBundleName)
        {
            return false;
        }
    }

    /// <summary>
    /// 编辑器资源加载设置初始化
    /// </summary>
    // [CreateAssetMenu(menuName = "资源管理/合理组依赖配置", fileName = "LegalGroupDependenceConfig")] // (一个项目创建一个就足够, 创建后不再在菜单显示, 故屏蔽此行代码)
    public class LegalGroupDependenceConfig : ScriptableObject
    {
        /// <summary>
        /// 配置列表
        /// </summary>
        [LabelText("配置列表"), ListDrawerSettings(DefaultExpandedState = true)]
        public List<GroupLegalDependence> legalGroupDependenciesList = new();

        /// <summary>
        /// 使用字典记录组名和对应的配置
        /// </summary>
        Dictionary<string, GroupLegalDependence> _groupNameToGroupLegalDepMap;

        /// <summary>
        /// 自定义合法组配置类型(可以通过重写IsCustomLegalBundle方法自定义合法规则)
        /// </summary>
        static Dictionary<string, Type> s_customGroupConfigType;

        /// <summary>
        /// 单例
        /// </summary>
        static LegalGroupDependenceConfig s_instance;

        public static LegalGroupDependenceConfig Instance
        {
            get
            {
                if (!s_instance)
                {
                    string[] guidList = AssetDatabase.FindAssets($"t:{typeof(LegalGroupDependenceConfig).FullName}");
                    if (guidList.Length > 0)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guidList[0]);
                        if (!string.IsNullOrEmpty(assetPath))
                            s_instance = AssetDatabase.LoadAssetAtPath<LegalGroupDependenceConfig>(assetPath);
                    }
                }

                return s_instance;
            }
        }

        /// <summary>
        /// 添加自定义规则的类(可用InitializeOnLoadMethod标签, 在Unity启动时加入自定义类)
        /// </summary>
        public static void AddCustomGroupConfigType(string groupName, Type type)
        {
            s_customGroupConfigType ??= new Dictionary<string, Type>();
            s_customGroupConfigType[groupName] = type;
        }

        /// <summary>
        /// 清除字典记录
        /// </summary>
        public void Reset()
        {
            _groupNameToGroupLegalDepMap = null;
            foreach (GroupLegalDependence config in legalGroupDependenciesList)
                config.Reset();
        }

        /// <summary>
        /// 将列表转为字典方便获取
        /// </summary>
        void InitGroupNameToGroupLegalDepMap()
        {
            if (_groupNameToGroupLegalDepMap != null)
                return;

            _groupNameToGroupLegalDepMap = new Dictionary<string, GroupLegalDependence>();
            foreach (GroupLegalDependence config in legalGroupDependenciesList)
            {
                string groupName = config.groupName;
                if (_groupNameToGroupLegalDepMap.ContainsKey(groupName))
                {
                    Debug.LogError($"发现重复组名({groupName}), 请检查组配置");
                    continue;
                }

                if (s_customGroupConfigType == null || !s_customGroupConfigType.ContainsKey(groupName))
                    _groupNameToGroupLegalDepMap[groupName] = config;
                else
                {
                    GroupLegalDependence newConfig = (GroupLegalDependence)Activator.CreateInstance(s_customGroupConfigType[groupName]);
                    newConfig.groupName = groupName;
                    newConfig.legalBundleList = config.legalBundleList;
                    _groupNameToGroupLegalDepMap[groupName] = newConfig;
                }
            }

            if (s_customGroupConfigType != null)
            {
                foreach (string groupName in s_customGroupConfigType.Keys)
                {
                    if (!_groupNameToGroupLegalDepMap.ContainsKey(groupName))
                    {
                        GroupLegalDependence newConfig = (GroupLegalDependence)Activator.CreateInstance(s_customGroupConfigType[groupName]);
                        newConfig.groupName = groupName;
                        _groupNameToGroupLegalDepMap[groupName] = newConfig;
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定组的合法依赖配置
        /// </summary>
        public GroupLegalDependence GetGroupLegalDependence(string groupName)
        {
            InitGroupNameToGroupLegalDepMap();
            return _groupNameToGroupLegalDepMap.GetValueOrDefault(groupName);
        }

        /// <summary>
        /// 根据组名判断指定的包是否为合法引用
        /// </summary>
        /// <param name="selfGroupName">所在组名</param>
        /// <param name="selfBundleName">自身包名</param>
        /// <param name="depBundleName">引用包名</param>
        public bool IsLegalDependence(string selfGroupName, string selfBundleName, string depBundleName)
        {
            InitGroupNameToGroupLegalDepMap();

            if (_groupNameToGroupLegalDepMap.TryGetValue(selfGroupName, out GroupLegalDependence groupConfig))
                return groupConfig.IsLegalBundle(selfBundleName, depBundleName);

            return false;
        }
    }

    /// <summary>
    /// 编辑器类, 只是为了变化后清除数据
    /// </summary>
    [CustomEditor(typeof(LegalGroupDependenceConfig))]
    public class LegalGroupDependenceConfigEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            // 改变时清除单例, 获取时可使用最新数据
            if (EditorGUI.EndChangeCheck())
            {
                LegalGroupDependenceConfig instance = LegalGroupDependenceConfig.Instance;
                if (instance)
                    instance.Reset();
            }
        }
    }
}