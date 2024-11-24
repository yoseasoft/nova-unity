/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
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
    /// 标准更新对象接口，预定义定时更新操作接口
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// 逻辑事务刷新操作接口
        /// </summary>
        void Execute();

        /// <summary>
        /// 逻辑后置刷新操作接口
        /// </summary>
        void LateExecute();

        /// <summary>
        /// 更新对象唯一识别编码
        /// </summary>
        /// <returns>返回一个整型数字用于区分不同更新对象</returns>
        int ExecutingCode();
    }
}
