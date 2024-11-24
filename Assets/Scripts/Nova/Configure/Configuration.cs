/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using System.Collections.Generic;

namespace NovaEngine
{
    /// <summary>
    /// 基础配置参数定义类，对当前引擎运行所需的初始化参数进行设置及管理
    /// </summary>
    public static partial class Configuration
    {
        /// <summary>
        /// 屏幕禁止休眠模式启用开关
        /// </summary>
        public readonly static bool OnScreenNeverSleepEnabled = false;

        /// <summary>
        /// 鼠标输入模式启用开关
        /// </summary>
        public readonly static bool OnMouseInputEnabled = true;

        /// <summary>
        /// 键盘输入模式启用开关
        /// </summary>
        public readonly static bool OnKeyboardInputEnabled = true;

        /// <summary>
        /// 设置系统环境参数
        /// </summary>
        /// <param name="name">参数键</param>
        /// <param name="value">参数值</param>
        public static void SetSystemEnvironment(string name, string value)
        {
            System.Environment.SetEnvironmentVariable(name, value);
        }

        /// <summary>
        /// 获取系统环境参数
        /// </summary>
        /// <param name="name">参数键</param>
        /// <returns>返回给定键对应的环境参数值</returns>
        public static string GetSystemEnvironment(string name)
        {
            return System.Environment.GetEnvironmentVariable(name);
        }
    }
}
