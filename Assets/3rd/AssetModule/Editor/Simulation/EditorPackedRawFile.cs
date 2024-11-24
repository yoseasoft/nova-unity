using System.IO;

namespace AssetModule.Editor.Simulation
{
    /// <summary>
    /// 编辑器下打包目录的原始文件加载
    /// </summary>
    public class EditorPackedRawFile : RawFile
    {
        /// <summary>
        /// 创建EditorPackedRawFile
        /// </summary>
        internal static EditorPackedRawFile Create(string filePath)
        {
            return new EditorPackedRawFile { filePath = filePath };
        }

        /// <summary>
        /// 设置文件保存目录并完成加载
        /// </summary>
        void SetRawFileSavePathAndFinish()
        {
            ManifestHandler.GetMainBundleInfoAndDependencies(filePath, out ManifestBundleInfo bundleInfo, out _);
            savePath = AssetPath.TranslateToLocalDataPath(bundleInfo.NameWithHash);
            address = savePath;
            Finish(File.Exists(savePath) ? null : "加载失败, 文件不存在");
        }

        protected override void OnLoad()
        {
            // 因编辑器下原始文件无需下载，直接读取即可, 故此处不加载, 放到OnUpdate中加载，模拟异步
        }

        protected override void OnLoadImmediately()
        {
            SetRawFileSavePathAndFinish();
        }

        protected override void OnUpdate()
        {
            if (Status == LoadableStatus.Loading)
                SetRawFileSavePathAndFinish();
        }
    }
}