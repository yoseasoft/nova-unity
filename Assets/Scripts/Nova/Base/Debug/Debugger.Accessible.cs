/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
    public partial class Debugger
    {
        /// <summary>
        /// 日志输出规范定义代理句柄接口
        /// </summary>
        public delegate void OutputHandler_object(object message);
        public delegate void OutputHandler_string(string message);
        public delegate void OutputHandler_cond_string(bool cond, string message);
        public delegate void OutputHandler_format_args(string format, params object[] args);
        public delegate void OutputHandler_cond_format_args(bool cond, string format, params object[] args);

        public delegate void OutputHandler_level_object(LogLevelType level, object message);
        public delegate void OutputHandler_level_string(LogLevelType level, string message);
        public delegate void OutputHandler_level_format_args(LogLevelType level, string format, params object[] args);

        /// <summary>
        /// 断言处理规范定义代理句柄接口
        /// </summary>
        public delegate void AssertHandler_empty(bool condition);
        public delegate void AssertHandler_object(bool condition, object message);
        public delegate void AssertHandler_string(bool condition, string message);
        public delegate void AssertHandler_format_args(bool condition, string format, params object[] args);

        /// <summary>
        /// 调试模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_log_object;
        private OutputHandler_string m_log_string;
        private OutputHandler_format_args m_log_format_args;

        /// <summary>
        /// 信息模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_info_object;
        private OutputHandler_string m_info_string;
        private OutputHandler_format_args m_info_format_args;

        /// <summary>
        /// 警告模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_warn_object;
        private OutputHandler_string m_warn_string;
        private OutputHandler_format_args m_warn_format_args;

        /// <summary>
        /// 错误模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_error_object;
        private OutputHandler_string m_error_string;
        private OutputHandler_format_args m_error_format_args;

        /// <summary>
        /// 崩溃模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_fatal_object;
        private OutputHandler_string m_fatal_string;
        private OutputHandler_format_args m_fatal_format_args;

        /// <summary>
        /// 自定义模式下的输出回调接口
        /// </summary>
        private OutputHandler_level_object m_output_object;
        private OutputHandler_level_string m_output_string;
        private OutputHandler_level_format_args m_output_format_args;

        /// <summary>
        /// 调试模型下的断言回调接口
        /// </summary>
        private AssertHandler_empty m_assert_empty;
        private AssertHandler_object m_assert_object;
        private AssertHandler_string m_assert_string;
        private AssertHandler_format_args m_assert_format_args;

        /// <summary>
        /// 空置版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="message">日志内容</param>
        private static void Blank_Output(object message) { }

        /// <summary>
        /// 空置版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="message">日志内容</param>
        private static void Blank_Output(string message) { }

        /// <summary>
        /// 空置版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Blank_Output(string format, params object[] args) { }

        /// <summary>
        /// 控制版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="level">日志基本</param>
        /// <param name="message">日志内容</param>
        private static void Blank_Output(LogLevelType level, object message) { }

        /// <summary>
        /// 控制版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="level">日志基本</param>
        /// <param name="message">日志内容</param>
        private static void Blank_Output(LogLevelType level, string message) { }

        /// <summary>
        /// 控制版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="level">日志基本</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Blank_Output(LogLevelType level, string format, params object[] args) { }

        /// <summary>
        /// 空置版本的日志断言接口，用于忽略指定级别类型对应的断言回调
        /// </summary>
        /// <param name="condition">条件表达式</param>
        private static void Blank_Assert(bool condition) { }

        /// <summary>
        /// 空置版本的日志断言接口，用于忽略指定级别类型对应的断言回调
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        private static void Blank_Assert(bool condition, object message) { }

        /// <summary>
        /// 空置版本的日志断言接口，用于忽略指定级别类型对应的断言回调
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        private static void Blank_Assert(bool condition, string message) { }

        /// <summary>
        /// 空置版本的日志断言接口，用于忽略指定级别类型对应的断言回调
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Blank_Assert(bool condition, string format, params object[] args) { }

        /// <summary>
        /// 重新绑定全部日志输出回调接口为空置模式
        /// </summary>
        private void RebindingBlankOutputHandler()
        {
            m_log_object = Blank_Output;
            m_log_string = Blank_Output;
            m_log_format_args = Blank_Output;

            m_info_object = Blank_Output;
            m_info_string = Blank_Output;
            m_info_format_args = Blank_Output;

            m_warn_object = Blank_Output;
            m_warn_string = Blank_Output;
            m_warn_format_args = Blank_Output;

            m_error_object = Blank_Output;
            m_error_string = Blank_Output;
            m_error_format_args = Blank_Output;

            m_fatal_object = Blank_Output;
            m_fatal_string = Blank_Output;
            m_fatal_format_args = Blank_Output;

            m_output_object = Blank_Output;
            m_output_string = Blank_Output;
            m_output_format_args = Blank_Output;

            m_assert_empty = Blank_Assert;
            m_assert_object = Blank_Assert;
            m_assert_string = Blank_Assert;
            m_assert_format_args = Blank_Assert;
        }

        #region 调试器对象输出回调接口的Getter/Setter函数

        protected internal OutputHandler_object Log_object
        {
            set { m_log_object = value; }
            get { return m_log_object; }
        }

        protected internal OutputHandler_string Log_string
        {
            set { m_log_string = value; }
            get { return m_log_string; }
        }

        protected internal OutputHandler_format_args Log_format_args
        {
            set { m_log_format_args = value; }
            get { return m_log_format_args; }
        }

        protected internal OutputHandler_object Info_object
        {
            set { m_info_object = value; }
            get { return m_info_object; }
        }

        protected internal OutputHandler_string Info_string
        {
            set { m_info_string = value; }
            get { return m_info_string; }
        }

        protected internal OutputHandler_format_args Info_format_args
        {
            set { m_info_format_args = value; }
            get { return m_info_format_args; }
        }

        protected internal OutputHandler_object Warn_object
        {
            set { m_warn_object = value; }
            get { return m_warn_object; }
        }

        protected internal OutputHandler_string Warn_string
        {
            set { m_warn_string = value; }
            get { return m_warn_string; }
        }

        protected internal OutputHandler_format_args Warn_format_args
        {
            set { m_warn_format_args = value; }
            get { return m_warn_format_args; }
        }

        protected internal OutputHandler_object Error_object
        {
            set { m_error_object = value; }
            get { return m_error_object; }
        }

        protected internal OutputHandler_string Error_string
        {
            set { m_error_string = value; }
            get { return m_error_string; }
        }

        protected internal OutputHandler_format_args Error_format_args
        {
            set { m_error_format_args = value; }
            get { return m_error_format_args; }
        }

        protected internal OutputHandler_object Fatal_object
        {
            set { m_fatal_object = value; }
            get { return m_fatal_object; }
        }

        protected internal OutputHandler_string Fatal_string
        {
            set { m_fatal_string = value; }
            get { return m_fatal_string; }
        }

        protected internal OutputHandler_format_args Fatal_format_args
        {
            set { m_fatal_format_args = value; }
            get { return m_fatal_format_args; }
        }

        protected internal OutputHandler_level_object Output_object
        {
            set { m_output_object = value; }
            get { return m_output_object; }
        }

        protected internal OutputHandler_level_string Output_string
        {
            set { m_output_string = value; }
            get { return m_output_string; }
        }

        protected internal OutputHandler_level_format_args Output_format_args
        {
            set { m_output_format_args = value; }
            get { return m_output_format_args; }
        }

        protected internal AssertHandler_empty Assert_empty
        {
            set { m_assert_empty = value; }
            get { return m_assert_empty; }
        }

        protected internal AssertHandler_object Assert_object
        {
            set { m_assert_object = value; }
            get { return m_assert_object; }
        }

        protected internal AssertHandler_string Assert_string
        {
            set { m_assert_string = value; }
            get { return m_assert_string; }
        }

        protected internal AssertHandler_format_args Assert_format_args
        {
            set { m_assert_format_args = value; }
            get { return m_assert_format_args; }
        }

        #endregion
    }
}
