/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 运行时内存概览信息展示窗口的对象类
        /// </summary>
        private sealed partial class RuntimeMemorySummaryWindow
        {
            /// <summary>
            /// 用于对资源对象进行封装的记录对象类，对单个资源对象在当前引擎中所有引用实例进行统计
            /// </summary>
            private sealed class Record
            {
                /// <summary>
                /// 资源对象的名称
                /// </summary>
                private readonly string m_name;
                /// <summary>
                /// 资源对象引用的数量
                /// </summary>
                private int m_count;
                /// <summary>
                /// 资源对象消耗的内存总量
                /// </summary>
                private long m_size;

                /// <summary>
                /// 获取资源对象名称
                /// </summary>
                public string Name
                {
                    get { return m_name; }
                }

                /// <summary>
                /// 获取或设置资源对象引用的数量
                /// </summary>
                public int Count
                {
                    get { return m_count; }
                    set { m_count = value; }
                }

                /// <summary>
                /// 获取或设置资源对象消耗的内存总量
                /// </summary>
                public long Size
                {
                    get { return m_size; }
                    set { m_size = value; }
                }

                public Record(string name)
                {
                    m_name = name;
                    m_count = 0;
                    m_size = 0L;
                }
            }
        }
    }
}
