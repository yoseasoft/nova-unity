/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 日志相关函数集合工具类
    /// </summary>
    public static partial class Logger
    {
        /// <summary>
        /// 日志输出控制台模式操作管理类
        /// </summary>
        public sealed class Console : Singleton<Console>, ILogOutput
        {
            /// <summary>
            /// 启动日志输出控制台模式
            /// </summary>
            public static void Startup()
            {
                Console c = Console.Instance;
                Logger.AddOutputChannel(c);
            }

            /// <summary>
            /// 关闭日志输出控制台模式
            /// </summary>
            public static void Shutdown()
            {
                Console c = Console.Instance;
                Logger.RemoveOutputChannel(c);
                Console.Destroy();
            }

            /// <summary>
            /// 日志控制台类初始化接口
            /// </summary>
            protected override void Initialize()
            {
            }

            /// <summary>
            /// 日志控制台类清理接口
            /// </summary>
            protected override void Cleanup()
            {
            }

            /// <summary>
            /// 日志输入记录接口
            /// </summary>
            /// <param name="level">日志等级</param>
            /// <param name="message">日志内容</param>
            public void Output(LogLevelType level, object message)
            {
                switch (level)
                {
                    case LogLevelType.Debug:
                    case LogLevelType.Info:
                        UnityEngine.Debug.Log(message.ToString());
                        break;
                    case LogLevelType.Warning:
                        UnityEngine.Debug.LogWarning(message.ToString());
                        break;
                    case LogLevelType.Error:
                    case LogLevelType.Fatal:
                        UnityEngine.Debug.LogError(message.ToString());
                        break;
                }
            }
        }
    }
}
