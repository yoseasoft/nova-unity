/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace NovaEngine
{
    /// <summary>
    /// 调试器对象工具类，用于引擎内部调试控制及输出相关接口声明
    /// </summary>
    public partial class Debugger : Singleton<Debugger>
    {
        /// <summary>
        /// 调试器对象类初始化接口
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// 调试器对象类清理接口
        /// </summary>
        protected override void Cleanup()
        {
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Log(object message)
        {
            Instance.m_log_object?.Invoke(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Log(string message)
        {
            Instance.m_log_string?.Invoke(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Log(bool condition, string message)
        {
            if (false == condition) Log(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Log(string format, params object[] args)
        {
            Instance.m_log_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Log(bool condition, string format, params object[] args)
        {
            if (false == condition) Log(format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message)
        {
            Instance.m_info_object?.Invoke(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            Instance.m_info_string?.Invoke(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Info(bool condition, string message)
        {
            if (false == condition) Info(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(string format, params object[] args)
        {
            Instance.m_info_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(bool condition, string format, params object[] args)
        {
            if (false == condition) Info(format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(object message)
        {
            Instance.m_warn_object?.Invoke(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(string message)
        {
            Instance.m_warn_string?.Invoke(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Warn(bool condition, string message)
        {
            if (false == condition) Warn(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(string format, params object[] args)
        {
            Instance.m_warn_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(bool condition, string format, params object[] args)
        {
            if (false == condition) Warn(format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(object message)
        {
            Instance.m_error_object?.Invoke(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(string message)
        {
            Instance.m_error_string?.Invoke(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Error(bool condition, string message)
        {
            if (false == condition) Error(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(string format, params object[] args)
        {
            Instance.m_error_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(bool condition, string format, params object[] args)
        {
            if (false == condition) Error(format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(object message)
        {
            Instance.m_fatal_object?.Invoke(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(string message)
        {
            Instance.m_fatal_string?.Invoke(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Fatal(bool condition, string message)
        {
            if (false == condition) Fatal(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(string format, params object[] args)
        {
            Instance.m_fatal_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(bool condition, string format, params object[] args)
        {
            if (false == condition) Fatal(format, args);
        }

        /// <summary>
        /// 基于指定的日志级别和内容进行输出的接口函数
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        public static void Output(LogLevelType level, object message)
        {
            Instance.m_output_object?.Invoke(level, message);
        }

        /// <summary>
        /// 基于指定的日志级别和内容进行输出的接口函数
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        public static void Output(LogLevelType level, string message)
        {
            Instance.m_output_string?.Invoke(level, message);
        }

        /// <summary>
        /// 基于指定的日志级别和内容进行输出的接口函数
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Output(LogLevelType level, string format, params object[] args)
        {
            Instance.m_output_format_args?.Invoke(level, format, args);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        public static void Assert(bool condition)
        {
            Instance.m_assert_empty?.Invoke(condition);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, object message)
        {
            Instance.m_assert_object?.Invoke(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, string message)
        {
            Instance.m_assert_string?.Invoke(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Assert(bool condition, string format, params object[] args)
        {
            Instance.m_assert_format_args?.Invoke(condition, format, args);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        public static void Assert(object obj)
        {
            Assert(null != obj);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="message">消息内容</param>
        public static void Assert(object obj, object message)
        {
            Assert(null != obj, message);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="message">消息内容</param>
        public static void Assert(object obj, string message)
        {
            Assert(null != obj, message);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Assert(object obj, string format, params object[] args)
        {
            Assert(null != obj, format, args);
        }
    }
}
