namespace AssetModule
{
    /// <summary>
    /// 资源清单文件信息
    /// </summary>
    [System.Serializable]
    public class ManifestVersion
    {
        #region json存储和写入字段

        /// <summary>
        /// 清单名字
        /// </summary>
        public string n;

        /// <summary>
        /// 清单文件的文件名
        /// </summary>
        public string f;

        /// <summary>
        /// 清单文件的大小(单位:字节(B))
        /// </summary>
        public long s;

        /// <summary>
        /// 清单文件的Hash
        /// </summary>
        public string h;

        #endregion

        #region 代码使用属性, 方便维护

        /// <summary>
        /// 清单名字
        /// </summary>
        public string Name
        {
            get => n;
            set => n = value;
        }

        /// <summary>
        /// 清单文件的文件名(带Hash,即保持唯一)
        /// </summary>
        public string FileName
        {
            get => f;
            set => f = value;
        }

        /// <summary>
        /// 清单文件的大小(单位:字节(B))
        /// </summary>
        public long Size
        {
            get => s;
            set => s = value;
        }

        /// <summary>
        /// 清单文件的Hash
        /// </summary>
        public string Hash
        {
            get => h;
            set => h = value;
        }

        #endregion

        /// <summary>
        /// 覆盖
        /// </summary>
        public void Overwrite(ManifestVersion newManifestVersion)
        {
            Name = newManifestVersion.Name;
            FileName = newManifestVersion.FileName;
            Size = newManifestVersion.Size;
            Hash = newManifestVersion.Hash;
        }
    }
}