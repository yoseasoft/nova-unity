/// <summary>
/// 2023-08-28 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 游戏层提供的调试对象类，它是基于对<see cref="GameEngine.Debugger"/>的便捷性接口封装
    /// </summary>
    public static class Debugger
    {
        /// <summary>
        /// 基于调试模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Log(object message)
        {
            GameEngine.Debugger.Log(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Log(string message)
        {
            GameEngine.Debugger.Log(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Log(string format, params object[] args)
        {
            GameEngine.Debugger.Log(format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message)
        {
            GameEngine.Debugger.Info(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            GameEngine.Debugger.Info(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(string format, params object[] args)
        {
            GameEngine.Debugger.Info(format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(object message)
        {
            GameEngine.Debugger.Warn(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(string message)
        {
            GameEngine.Debugger.Warn(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(string format, params object[] args)
        {
            GameEngine.Debugger.Warn(format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(object message)
        {
            GameEngine.Debugger.Error(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(string message)
        {
            GameEngine.Debugger.Error(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(string format, params object[] args)
        {
            GameEngine.Debugger.Error(format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(object message)
        {
            GameEngine.Debugger.Fatal(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(string message)
        {
            GameEngine.Debugger.Fatal(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(string format, params object[] args)
        {
            GameEngine.Debugger.Fatal(format, args);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        public static void Assert(bool condition)
        {
            GameEngine.Debugger.Assert(condition);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, object message)
        {
            GameEngine.Debugger.Assert(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, string message)
        {
            GameEngine.Debugger.Assert(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Assert(bool condition, string format, params object[] args)
        {
            GameEngine.Debugger.Assert(condition, format, args);
        }
    }
}
