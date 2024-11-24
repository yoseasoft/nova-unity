using System;

namespace AssetModule.Editor.Build
{
    /// <summary>
    /// 打包信息记录
    /// </summary>
    [Serializable]
    public class BuildRecord
    {
        /// <summary>
        /// 版本
        /// </summary>
        public int version;

        /// <summary>
        /// 构建时间戳
        /// </summary>
        public long timestamp;

        /// <summary>
        /// 版本(或清单)文件数据列表
        /// </summary>
        public VersionFileInfo[] versionFileInfoList;

        /// <summary>
        /// 资源清单列表
        /// </summary>
        public RecordManifest[] recordManifestList;
    }

    /// <summary>
    /// 版本(或清单)文件数据
    /// </summary>
    [Serializable]
    public class VersionFileInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string name;

        /// <summary>
        /// 文件Hash值
        /// </summary>
        public string hash;

        /// <summary>
        /// 文件大小
        /// </summary>
        public long size;
    }

    /// <summary>
    /// 清单数据
    /// </summary>
    [Serializable]
    public class RecordManifest
    {
        /// <summary>
        /// 清单名字
        /// </summary>
        public string name;

        /// <summary>
        /// Bundle或原文件信息列表
        /// </summary>
        public RecordBundleInfo[] recordBundleInfoList;
    }

    /// <summary>
    /// Bundle或原文件信息
    /// </summary>
    [Serializable]
    public class RecordBundleInfo
    {
        #region json存储和写入字段

        /// <summary>
        /// 所在组名
        /// </summary>
        public string g;

        /// <summary>
        /// 名字
        /// </summary>
        public string n;

        /// <summary>
        /// 文件大小
        /// </summary>
        public long s;

        /// <summary>
        /// 文件MD5值
        /// </summary>
        public string h;

        /// <summary>
        /// 是否为原文件
        /// </summary>
        public int r;

        /// <summary>
        /// 资源文件信息列表
        /// </summary>
        public RecordAssetInfo[] l;

        #endregion

        #region 代码使用属性, 方便维护

        /// <summary>
        /// 所在组名
        /// </summary>
        public string Group
        {
            get => g;
            set => g = value;
        }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name
        {
            get => n;
            set => n = value;
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size
        {
            get => s;
            set => s = value;
        }

        /// <summary>
        /// 文件MD5值
        /// </summary>
        public string Hash
        {
            get => h;
            set => h = value;
        }

        /// <summary>
        /// 是否为原文件
        /// </summary>
        public bool IsRawFile
        {
            get => r == 1;
            set => r = value ? 1 : 0;
        }

        /// <summary>
        /// 资源文件信息列表
        /// </summary>
        public RecordAssetInfo[] RecordAssetInfoList
        {
            get => l;
            set => l = value;
        }

        #endregion
    }

    /// <summary>
    /// Bundle里的Assets目录资源文件信息
    /// </summary>
    [Serializable]
    public class RecordAssetInfo
    {
        #region json存储和写入字段

        /// <summary>
        /// 资源目录
        /// </summary>
        public string a;

        /// <summary>
        /// 文件大小
        /// </summary>
        public long s;

        /// <summary>
        /// 文件MD5值
        /// </summary>
        public string h;

        #endregion

        #region 代码使用属性, 方便维护

        /// <summary>
        /// 资源目录
        /// </summary>
        public string AssetPath
        {
            get => a;
            set => a = value;
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size
        {
            get => s;
            set => s = value;
        }

        /// <summary>
        /// 文件MD5值
        /// </summary>
        public string Hash
        {
            get => h;
            set => h = value;
        }

        #endregion
    }
}