using System.IO;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// 清除历史文件操作
    /// </summary>
    public sealed class ClearHistoryOperation : Operation
    {
        /// <summary>
        /// 需要删除的所有文件路径
        /// </summary>
        readonly List<string> _needDeleteFilePaths = new();

        /// <summary>
        /// 需要删除文件总数, 用于计算进度
        /// </summary>
        int _totalCount;

        protected override void OnStart()
        {
            _needDeleteFilePaths.AddRange(Directory.GetFiles(AssetPath.DownloadDataPath));
            List<string> usedFileNameList = new(); // 当前使用的文件

            // 版本文件
            usedFileNameList.Add(ManifestHandler.VersionFileName);

            foreach (Manifest manifest in ManifestHandler.ManifestList)
            {
                // 清单文件
                usedFileNameList.Add(manifest.fileName);

                // ab包和原始文件
                foreach (ManifestBundleInfo bundleInfo in manifest.manifestBundleInfoList)
                    if (!string.IsNullOrEmpty(bundleInfo.SaveFileName))
                        usedFileNameList.Add(bundleInfo.SaveFileName);
            }

            _needDeleteFilePaths.RemoveAll(filePath => usedFileNameList.Contains(Path.GetFileName(filePath)));

            _totalCount = _needDeleteFilePaths.Count;
        }

        protected override void OnUpdate()
        {
            if (Status != OperationStatus.Processing)
                return;

            while (_needDeleteFilePaths.Count > 0)
            {
                Progress = (float)(_totalCount - _needDeleteFilePaths.Count + 1) / _totalCount;
                string filePath = _needDeleteFilePaths[0];
                if (File.Exists(filePath))
                    File.Delete(filePath);
                _needDeleteFilePaths.RemoveAt(0);

                if (AssetDispatcher.Instance.IsBusy)
                    return;
            }

            Finish();
        }
    }
}