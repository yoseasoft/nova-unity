using System;
using System.Collections.Generic;

namespace AppEngine
{
    /// <summary>
    /// 渠道设置
    /// </summary>
    [Serializable]
    public class AppChannelSettings
    {
        /// <summary>
        /// 渠道设置单例
        /// </summary>
        public static AppChannelSettings Instance { get; internal set; }

        /// <summary>
        /// 平台Id
        /// </summary>
        public int platformId;

        /// <summary>
        /// 渠道ID
        /// </summary>
        public int channelId;

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string channelName;

        /// <summary>
        /// 渠道动态参数
        /// </summary>
        public List<ExtraParam> extraParams = new();
    }
}