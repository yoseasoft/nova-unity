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
        /// 日志输出规范定义代理句柄接口
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        public delegate void OutputHandler(LogLevelType level, object message);

        /// <summary>
        /// 日志输入代理回调接口
        /// </summary>
        private static OutputHandler s_logOutputHandler = null;

        // internal static void AddOutputChannel(OutputHandler output)
        // {
        // s_logOutputHandler += output;
        // }

        /// <summary>
        /// 添加指定的输出通道
        /// </summary>
        /// <param name="log">通道实例</param>
        internal static void AddOutputChannel(ILogOutput log)
        {
            s_logOutputHandler += log.Output;
        }

        // internal static void RemoveOutputChannel(OutputHandler output)
        // {
        // s_logOutputHandler -= output;
        // }

        /// <summary>
        /// 移除指定的输出通道
        /// </summary>
        /// <param name="log">通道实例</param>
        internal static void RemoveOutputChannel(ILogOutput log)
        {
            s_logOutputHandler -= log.Output;
        }
    }
}
