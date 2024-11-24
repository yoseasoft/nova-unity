/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System.Collections.Generic;

using SystemStringComparer = System.StringComparer;
using SystemFile = System.IO.File;

namespace NovaEngine.IO.FileSystem
{
    /// <summary>
    /// 文件系统控制器基类
    /// </summary>
    internal sealed class FileSystemController : IFileSystemController
    {
        private readonly Dictionary<string, FileSystem> m_fileSystems;

        private IFileSystemHelper m_fileSystemHelper;

        /// <summary>
        /// 初始化文件系统管理器的新实例
        /// </summary>
        public FileSystemController()
        {
            m_fileSystems = new Dictionary<string, FileSystem>(SystemStringComparer.Ordinal);
            m_fileSystemHelper = null;
        }

        /// <summary>
        /// 获取文件系统数量
        /// </summary>
        public int Count
        {
            get { return m_fileSystems.Count; }
        }

        /// <summary>
        /// 关闭并清理文件系统管理器
        /// </summary>
        internal void Shutdown()
        {
            while (m_fileSystems.Count > 0)
            {
                foreach (KeyValuePair<string, FileSystem> fileSystem in m_fileSystems)
                {
                    DestroyFileSystem(fileSystem.Value, false);
                    break;
                }
            }
        }

        /// <summary>
        /// 设置文件系统辅助器
        /// </summary>
        /// <param name="fileSystemHelper">文件系统辅助器</param>
        public void SetFileSystemHelper(IFileSystemHelper fileSystemHelper)
        {
            if (null == fileSystemHelper)
            {
                throw new CException("File system helper is invalid.");
            }

            this.m_fileSystemHelper = fileSystemHelper;
        }

        /// <summary>
        /// 检查是否存在文件系统
        /// </summary>
        /// <param name="fullPath">要检查的文件系统的完整路径</param>
        /// <returns>返回是否存在文件系统</returns>
        public bool HasFileSystem(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                throw new CException("Full path is invalid.");
            }

            return m_fileSystems.ContainsKey(Utility.Path.GetRegularPath(fullPath));
        }

        /// <summary>
        /// 获取文件系统
        /// </summary>
        /// <param name="fullPath">要获取的文件系统的完整路径</param>
        /// <returns>返回获取的文件系统</returns>
        public IFileSystem GetFileSystem(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                throw new CException("Full path is invalid.");
            }

            FileSystem fileSystem = null;
            if (m_fileSystems.TryGetValue(Utility.Path.GetRegularPath(fullPath), out fileSystem))
            {
                return fileSystem;
            }

            return null;
        }

        /// <summary>
        /// 创建文件系统
        /// </summary>
        /// <param name="fullPath">要创建的文件系统的完整路径</param>
        /// <param name="accessType">要创建的文件系统的访问方式</param>
        /// <param name="maxFileCount">要创建的文件系统的最大文件数量</param>
        /// <param name="maxBlockCount">要创建的文件系统的最大块数据数量</param>
        /// <returns>返回创建的文件系统</returns>
        public IFileSystem CreateFileSystem(string fullPath, FileSystemAccessType accessType, int maxFileCount, int maxBlockCount)
        {
            if (null == m_fileSystemHelper)
            {
                throw new CException("File system helper is invalid.");
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                throw new CException("Full path is invalid.");
            }

            if (FileSystemAccessType.Unspecified == accessType)
            {
                throw new CException("Access is invalid.");
            }

            if (FileSystemAccessType.ReadOnly == accessType)
            {
                throw new CException("Access read is invalid.");
            }

            fullPath = Utility.Path.GetRegularPath(fullPath);
            if (m_fileSystems.ContainsKey(fullPath))
            {
                throw new CException("File system '{0}' is already exist.", fullPath);
            }

            FileSystemStream fileSystemStream = m_fileSystemHelper.CreateFileSystemStream(fullPath, accessType, true);
            if (null == fileSystemStream)
            {
                throw new CException("Create file system stream for '{0}' failure.", fullPath);
            }

            FileSystem fileSystem = FileSystem.Create(fullPath, accessType, fileSystemStream, maxFileCount, maxBlockCount);
            if (null == fileSystem)
            {
                throw new CException("Create file system '{0}' failure.", fullPath);
            }

            m_fileSystems.Add(fullPath, fileSystem);
            return fileSystem;
        }

        /// <summary>
        /// 加载文件系统
        /// </summary>
        /// <param name="fullPath">要加载的文件系统的完整路径</param>
        /// <param name="accessType">要加载的文件系统的访问方式</param>
        /// <returns>返回加载的文件系统</returns>
        public IFileSystem LoadFileSystem(string fullPath, FileSystemAccessType accessType)
        {
            if (null == m_fileSystemHelper)
            {
                throw new CException("File system helper is invalid.");
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                throw new CException("Full path is invalid.");
            }

            if (FileSystemAccessType.Unspecified == accessType)
            {
                throw new CException("Access is invalid.");
            }

            fullPath = Utility.Path.GetRegularPath(fullPath);
            if (m_fileSystems.ContainsKey(fullPath))
            {
                throw new CException("File system '{0}' is already exist.", fullPath);
            }

            FileSystemStream fileSystemStream = m_fileSystemHelper.CreateFileSystemStream(fullPath, accessType, false);
            if (null == fileSystemStream)
            {
                throw new CException("Create file system stream for '{0}' failure.", fullPath);
            }

            FileSystem fileSystem = FileSystem.Load(fullPath, accessType, fileSystemStream);
            if (null == fileSystem)
            {
                throw new CException("Load file system '{0}' failure.", fullPath);
            }

            m_fileSystems.Add(fullPath, fileSystem);
            return fileSystem;
        }

        /// <summary>
        /// 销毁文件系统
        /// </summary>
        /// <param name="fileSystem">要销毁的文件系统</param>
        /// <param name="deletePhysicalFile">是否删除文件系统对应的物理文件</param>
        public void DestroyFileSystem(IFileSystem fileSystem, bool deletePhysicalFile)
        {
            if (null == fileSystem)
            {
                throw new CException("File system is invalid.");
            }

            string fullPath = fileSystem.FullPath;
            ((FileSystem) fileSystem).Shutdown();
            m_fileSystems.Remove(fullPath);

            if (deletePhysicalFile && SystemFile.Exists(fullPath))
            {
                SystemFile.Delete(fullPath);
            }
        }

        /// <summary>
        /// 获取所有文件系统集合
        /// </summary>
        /// <returns>获取的所有文件系统集合</returns>
        public IFileSystem[] GetAllFileSystems()
        {
            int index = 0;
            IFileSystem[] results = new IFileSystem[m_fileSystems.Count];
            foreach (KeyValuePair<string, FileSystem> fileSystem in m_fileSystems)
            {
                results[index++] = fileSystem.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取所有文件系统集合
        /// </summary>
        /// <param name="results">获取的所有文件系统集合</param>
        public void GetAllFileSystems(List<IFileSystem> results)
        {
            if (null == results)
            {
                throw new CException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<string, FileSystem> fileSystem in m_fileSystems)
            {
                results.Add(fileSystem.Value);
            }
        }
    }
}
