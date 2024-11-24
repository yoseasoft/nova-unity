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

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集的分析处理类，对业务层载入的所有对象类进行统一加载及分析处理
    /// </summary>
    public static partial class CodeLoader
    {
        /// <summary>
        /// 程序集的编码分类类型的枚举定义
        /// </summary>
        [System.Flags]
        private enum CodeClassifyType : uint
        {
            /// <summary>
            /// 未知类型
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// 事件类型
            /// </summary>
            Event = 1,

            /// <summary>
            /// 网络类型
            /// </summary>
            Network = 2,

            /// <summary>
            /// 实体类型
            /// </summary>
            Proto = 4,

            /// <summary>
            /// 扩展类型
            /// </summary>
            Extend = 8,

            /// <summary>
            /// 切面类型
            /// </summary>
            Aspect = 16,

            /// <summary>
            /// 注入类型
            /// </summary>
            Inject = 32,

            /// <summary>
            /// 对象池类型
            /// </summary>
            Pool = 64,
        }
    }
}
