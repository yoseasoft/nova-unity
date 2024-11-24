using System;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 清单列表里的单个资源包数据
    /// 因此类的数据需要转成json文件储存, 为节省json文件大小, 此类的字段名尽量简化, 通过属性来设置和获取提高代码可读性
    /// </summary>
    [Serializable]
    public class ManifestBundleInfo
    {
        #region json存储和写入字段

        /// <summary>
        /// 包id
        /// </summary>
        public int i;

        /// <summary>
        /// 包名
        /// (1.ab包的名字就是ab包原名(不带hash值))
        /// (2.原始文件的名字就是{组配置的打包文件夹名/原文件(并带有文件后缀)})
        /// </summary>
        public string n;

        /// <summary>
        /// 是否为原始文件(0:false, 1:true)
        /// 使用数字存储可令json文件小点, false和true的字符太长
        /// </summary>
        public int r;

        /// <summary>
        /// 所包含资源的真实路径列表
        /// </summary>
        public List<string> a;

        /// <summary>
        /// 包大小, 单位:字节(B)
        /// </summary>
        public long s;

        /// <summary>
        /// 包文件计算出来的Hash
        /// </summary>
        public string h;

        /// <summary>
        /// 带有Hash值的包名, 可在不同版本中保证唯一
        /// </summary>
        public string w;

        /// <summary>
        /// 依赖的包id列表
        /// </summary>
        public int[] d;

        #endregion

        #region 代码使用属性, 方便维护

        /// <summary>
        /// 包id
        /// </summary>
        public int ID
        {
            get => i;
            set => i = value;
        }

        /// <summary>
        /// 包名
        /// (1.ab包文件的名字就是不带hash值的ab包文件名字)
        /// (2.原始文件的名字就是{组配置的打包文件夹名/原文件(并带有文件后缀)})
        /// </summary>
        public string Name
        {
            get => n;
            set => n = value;
        }

        /// <summary>
        /// 是否原始文件
        /// </summary>
        public bool IsRawFile
        {
            get => r == 1;
            set => r = value ? 1 : 0;
        }

        /// <summary>
        /// 所包含资源的真实路径列表
        /// </summary>
        public List<string> AssetPathList
        {
            get => a;
            set => a = value;
        }

        /// <summary>
        /// 包大小, 单位:字节(B)
        /// </summary>
        public long Size
        {
            get => s;
            set => s = value;
        }

        /// <summary>
        /// 包文件计算出来的Hash
        /// </summary>
        public string Hash
        {
            get => h;
            set => h = value;
        }

        /// <summary>
        /// 带有Hash值的包名, 可在不同版本中保证唯一
        /// </summary>
        public string NameWithHash
        {
            get => w;
            set => w = value;
        }

        /// <summary>
        /// 依赖的包id列表
        /// </summary>
        public int[] DependentBundleIDList
        {
            get => d;
            set => d = value;
        }

        /// <summary>
        /// 保存文件名字(原始文件和ab包文件保存的时候名字使用的字段不一样, 所以获取保存目录下的文件名时需要使用此字段)
        /// 注:原始文件保存文件名使用组配置的打包文件夹名加上原文件带有后缀的名字, ab包文件保存文件名使用带hash的ab包文件名
        /// </summary>
        public string SaveFileName => IsRawFile ? Name : NameWithHash;

        #endregion
    }
}