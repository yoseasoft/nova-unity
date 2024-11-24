/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemAssembly = System.Reflection.Assembly;
using SystemMemoryStream = System.IO.MemoryStream;
using SystemFileStream = System.IO.FileStream;
using SystemFileMode = System.IO.FileMode;
using SystemFileAccess = System.IO.FileAccess;

namespace GameEngine
{
    /// <summary>
    /// 游戏运行库的静态管理类，对业务层载入的所有运行对象类进行统一加载管理<br/>
    /// 该管理类主要通过反射实现运行对象类的初始化及清理流程中的一些模版配置管理
    /// </summary>
    public static partial class GameLibrary
    {
        /// <summary>
        /// 游戏运行库的启动函数
        /// </summary>
        public static void Startup()
        {
            Loader.CodeLoader.Startup();

            Loader.CodeLoader.AddLoadClassProgressCallback(LoadClassProgress);
            Loader.CodeLoader.AddLoadAssemblyCompletedCallback(LoadAssemblyCompleted);
        }

        /// <summary>
        /// 游戏运行库的关闭函数
        /// </summary>
        public static void Shutdown()
        {
            Loader.CodeLoader.RemoveLoadClassProgressCallback(LoadClassProgress);
            Loader.CodeLoader.RemoveLoadAssemblyCompletedCallback(LoadAssemblyCompleted);

            Loader.CodeLoader.Shutdown();
        }

        /// <summary>
        /// 游戏运行库的重启函数
        /// </summary>
        public static void Restart()
        {
            Loader.CodeLoader.Restart();
        }

        /// <summary>
        /// 游戏运行库加载指定的实体配置
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void LoadFromConfigure(string path)
        {
            using (SystemFileStream fs = new SystemFileStream(path, SystemFileMode.Open, SystemFileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();

                Loader.CodeLoader.LoadFromConfigure(bytes, 0, bytes.Length);

                bytes = null;
            }
        }

        /// <summary>
        /// 游戏运行库加载指定的实体配置
        /// </summary>
        /// <param name="mstream">数据流</param>
        public static void LoadFromConfigure(SystemMemoryStream mstream)
        {
            Loader.CodeLoader.LoadFromConfigure(mstream);
        }

        /// <summary>
        /// 游戏运行库加载指定的程序集
        /// </summary>
        /// <param name="assembly">程序集</param>
        public static void LoadFromAssembly(SystemAssembly assembly)
        {
            LoadFromAssembly(assembly, false);
        }

        /// <summary>
        /// 游戏运行库加载指定的程序集
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="reload">重载状态标识</param>
        public static void LoadFromAssembly(SystemAssembly assembly, bool reload)
        {
            try
            {
                Loader.CodeLoader.LoadFromAssembly(assembly, reload);
            }
            catch (System.Exception e) { Debugger.Warn(e.ToString()); }
        }

        /// <summary>
        /// 重载游戏运行环境的上下文信息，用于在进行重载操作后，补充之前创建的对象的信息
        /// </summary>
        public static void ReloadContext()
        {
            // 切面服务重载
            ReloadAspectService();
        }

        /// <summary>
        /// 程序集解析进度通知转发
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="classType">当前解析类</param>
        /// <param name="current">当前进度值</param>
        /// <param name="max">上限值</param>
        private static void LoadClassProgress(string assemblyName, SystemType classType, int current, int max)
        {
        }

        /// <summary>
        /// 程序集解析完成通知转发
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        private static void LoadAssemblyCompleted(string assemblyName)
        {
        }
    }
}
