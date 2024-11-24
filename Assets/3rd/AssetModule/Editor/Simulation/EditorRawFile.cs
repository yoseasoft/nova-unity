using System.IO;

namespace AssetModule.Editor.Simulation
{
    /// <summary>
    /// 编辑器下原始文件加载
    /// </summary>
    public class EditorRawFile : RawFile
    {
        /// <summary>
        /// 创建EditorRawFile
        /// </summary>
        internal static EditorRawFile Create(string filePath)
        {
            return new EditorRawFile { filePath = filePath };
        }

        /// <summary>
        /// 设置文件保存目录并完成加载
        /// </summary>
        void SetRawFileSavePathAndFinish()
        {
            ManifestHandler.GetMainBundleInfoAndDependencies(filePath, out ManifestBundleInfo bundleInfo, out _);
            if (bundleInfo != null && !string.IsNullOrEmpty(bundleInfo.Name)) // 若获取到的bundle信息名字不为空，证明是外部目录资源, Name就是文件加载路径, 具体可查看EditorManifestAsset
                savePath = bundleInfo.Name;
            else
                savePath = Path.Combine(System.Environment.CurrentDirectory, filePath);
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