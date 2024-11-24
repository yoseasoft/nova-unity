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
    public partial class Debugger
    {
        /// <summary>
        /// 调试器对象配置加载接口函数
        /// </summary>
        public void LoadConfig()
        {
            int lv = Environment.debugLevel;

            // 未开启调试模式的情况下，调试级别直接归零
            if (!Environment.debugMode) lv = 0;

            // 重新绑定为空置模式
            RebindingBlankOutputHandler();

            Logger.Info("The platform current debug level was {%d}.", lv);

            // 自定义模式在任意级别下均会加载
            Output_object = Logger.__Output;
            Output_string = Logger.__Output;
            Output_format_args = Logger.__Output;

            Assert_empty = Logger.__System_Assert;
            Assert_object = Logger.__System_Assert;
            Assert_string = Logger.__System_Assert;
            Assert_format_args = Logger.__System_Assert;

            if (lv >= 0)
            {
                Error_object = Logger.Error;
                Error_string = Logger.Error;
                Error_format_args = Logger.Error;

                Fatal_object = Logger.Fatal;
                Fatal_string = Logger.Fatal;
                Fatal_format_args = Logger.Fatal;
            }

            if (lv >= 1)
            {
                Warn_object = Logger.Warn;
                Warn_string = Logger.Warn;
                Warn_format_args = Logger.Warn;

                Error_object = Logger.TraceError;
                Error_string = Logger.TraceError;
                Error_format_args = Logger.TraceError;

                Fatal_object = Logger.TraceFatal;
                Fatal_string = Logger.TraceFatal;
                Fatal_format_args = Logger.TraceFatal;
            }

            if (lv >= 2)
            {
                Info_object = Logger.Info;
                Info_string = Logger.Info;
                Info_format_args = Logger.Info;

                Warn_object = Logger.TraceWarn;
                Warn_string = Logger.TraceWarn;
                Warn_format_args = Logger.TraceWarn;
            }

            if (lv >= 3)
            {
                Log_object = Logger.Debug;
                Log_string = Logger.Debug;
                Log_format_args = Logger.Debug;
            }

            if (lv >= 4)
            {
                Log_object = Logger.TraceDebug;
                Log_string = Logger.TraceDebug;
                Log_format_args = Logger.TraceDebug;

                Info_object = Logger.TraceInfo;
                Info_string = Logger.TraceInfo;
                Info_format_args = Logger.TraceInfo;
            }
        }

        /// <summary>
        /// 调试器对象配置卸载接口函数
        /// </summary>
        public void UnloadConfig()
        {
            // 重新绑定为空置模式
            RebindingBlankOutputHandler();
        }
    }
}
