using UnityEngine;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 包含所有资源清单的版本信息的容器
    /// </summary>
    public class ManifestVersionContainer : ScriptableObject
    {
        #region json存储和写入字段

        /// <summary>
        /// 版本号, 仅文件名使用
        /// </summary>
        public int v;

        /// <summary>
        /// 构建时的时间戳
        /// </summary>
        public long t;

        /// <summary>
        /// 所有资源清单的版本信息
        /// </summary>
        public List<ManifestVersion> a = new();

        #endregion

        #region 代码使用属性, 方便维护

        /// <summary>
        /// 版本号, 仅文件名使用
        /// </summary>
        public int Version
        {
            get => v;
            set => v = value;
        }

        /// <summary>
        /// 构建时的时间戳
        /// </summary>
        public long Timestamp
        {
            get => t;
            set => t = value;
        }

        /// <summary>
        /// 所有资源清单的版本信息
        /// </summary>
        public List<ManifestVersion> AllManifestVersions => a;

        #endregion
    }
}