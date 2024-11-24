/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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
    /// 可初始化类对象的抽象接口类，用于定义一个标准模式的初始化及清理内置回调接口函数<br/>
    /// 当您需要创建一个具备标准初始化内置回调接口的对象类时，可以考虑继承该接口，这样可以通过以可初始化模式的接口类进行参数传递及调用
    /// </summary>
    public interface IOnInitializing
    {
        /// <summary>
        /// 对象初始化内置回调函数接口
        /// </summary>
        void OnInitialize();

        /// <summary>
        /// 对象清理内置回调函数接口
        /// </summary>
        void OnCleanup();
    }
}
