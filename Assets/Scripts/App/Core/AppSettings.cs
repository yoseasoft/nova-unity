using System;
using UnityEngine;
using System.Collections.Generic;

namespace AppEngine
{
    /// <summary>
    /// App配置，用于应用正式启动的参数设置
    /// </summary>
    // [CreateAssetMenu(fileName = "AppSettings", menuName = "AppSettings")] // 创建后不再显示在右键菜单
    public class AppSettings : ScriptableObject
    {
        // ----------------------------------------------------------------------------------------------------
        [Header("应用程序运行模式")]
        [Tooltip("编辑模式")]
        public bool editorMode = true;

        [Tooltip("调试模式")]
        public bool debugMode = true;

        [Tooltip("调试级别")]
        public int debugLevel = 5;

        [Tooltip("加密模式")]
        public bool cryptMode;

        [Tooltip("更新模式")]
        public bool updateMode;

        [Tooltip("启用Dll模式?(需要断点调试可关闭EnableDll)")]
        public bool enableDll = true;

        // ----------------------------------------------------------------------------------------------------
        [Header("应用程序配置参数")]
        [Tooltip("刷新帧数")]
        public int frameRate = 30;

        [Tooltip("动画速率")]
        public int animationRate = 30;

        [Tooltip("分辨率宽度")]
        public int designResolutionWidth = 1920;

        [Tooltip("分辨率高度")]
        public int designResolutionHeight = 1080;

        [Tooltip("应用名称")]
        public string applicationName = "unknown";

        [Tooltip("应用编码")]
        public int applicationCode;

        // ----------------------------------------------------------------------------------------------------
        [Header("动态参数设置")]
        public List<ExtraParam> extraParams = new();

        /// <summary>
        /// AppSettings示例
        /// </summary>
        public static AppSettings Instance
        {
            get
            {
                AppSettings settings = Resources.Load<AppSettings>(nameof(AppSettings));
                if (settings == null)
                {
                    settings = CreateInstance<AppSettings>();
                    Debug.LogError("AppSettings不存在！请在Resources目录创建AppSettings");
                }

                return settings;
            }
        }
    }

    /// <summary>
    /// 额外参数
    /// </summary>
    [Serializable]
    public class ExtraParam
    {
        public string key;
        public string value;
    }
}