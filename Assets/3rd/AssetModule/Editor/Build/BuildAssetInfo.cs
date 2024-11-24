using System;

namespace AssetModule.Editor.Build
{
    /// <summary>
    /// 参与打包的资源信息对象
    /// </summary>
    [Serializable]
    public class BuildAssetInfo
    {
        /// <summary>
        /// 资源打包时的真实路径
        /// </summary>
        public string path;

        /// <summary>
        /// 被分配到的打包文件名字(由BuildUtils.GetAssetPackedBundleName()赋值)
        /// 1.若是ab包则是ab包文件名字(此处不带hash, 故还不是最终文件名字),
        /// 2.若是原始文件则是打包复制后的最终文件名字
        /// </summary>
        public string buildFileName;

        #region 原始文件扩展字段

        /// <summary>
        /// 是否外部目录文件(仅原始文件使用)
        /// </summary>
        public bool isExternalFile;

        /// <summary>
        /// 原始外部目录
        /// </summary>
        public string originalExternalPath;

        /// <summary>
        /// 资源存放的文件夹名(仅原始文件使用)
        /// </summary>
        public string placeFolderName;

        #endregion
    }
}