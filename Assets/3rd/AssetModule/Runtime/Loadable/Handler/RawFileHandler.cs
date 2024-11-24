using System;

namespace AssetModule
{
    /// <summary>
    /// 原始资源文件对象管理
    /// </summary>
    public static class RawFileHandler
    {
        /// <summary>
        /// 创建场景对象方法, 可进行自定义覆盖, 默认为DefaultCreateRawFileFunc
        /// </summary>
        public static Func<string, RawFile> CreateRawFileFunc { get; set; } = DefaultCreateRawFileFunc;

        /// <summary>
        /// 默认创建原始文件对象的方法
        /// </summary>
        static RawFile DefaultCreateRawFileFunc(string filePath)
        {
            return new RawFile { filePath = filePath };
        }

        /// <summary>
        /// 同步加载原始资源
        /// </summary>
        /// <param name="filePath">文件原打包路径('Assets/_Resources/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')</param>
        internal static RawFile Load(string filePath)
        {
            RawFile rawFile = LoadAsync(filePath);
            rawFile?.LoadImmediately();
            return rawFile;
        }

        /// <summary>
        /// 异步加载原始资源文件
        /// </summary>
        /// /// <param name="filePath">文件原打包路径('Assets/_Resources/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')</param>
        /// <param name="completed">完成回调</param>
        internal static RawFile LoadAsync(string filePath, Action<RawFile> completed = null)
        {
            if (!ManifestHandler.IsContainAsset(filePath))
            {
                Logger.Error($"原始资源文件加载失败, 因所有资源清单中都没有此文件的记录:{filePath}");
                return null;
            }

            RawFile rawFile = CreateRawFileFunc(filePath);

            if (completed != null)
                rawFile.completed += completed;

            rawFile.Load();
            return rawFile;
        }
    }
}