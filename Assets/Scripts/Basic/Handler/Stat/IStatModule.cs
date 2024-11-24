/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 业务框架统计模块对象的接口定义类
    /// 我们通过该对象对各个模块进行数据统计，方便我们进行程序优化
    /// </summary>
    public interface IStatModule
    {
        /// <summary>
        /// 统计模块注册绑定回调函数的声明属性类型定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        protected internal class OnStatModuleRegisterCallbackAttribute : SystemAttribute
        {
            /// <summary>
            /// 统计模块的功能标识
            /// </summary>
            private readonly int m_funcType;

            public int FuncType => m_funcType;

            public OnStatModuleRegisterCallbackAttribute(int funcType) : base()
            {
                m_funcType = funcType;
            }
        }

        /// <summary>
        /// 获取统计模块的模块类型标识
        /// </summary>
        int ModuleType { get; }

        /// <summary>
        /// 引擎统计模块实例初始化接口
        /// </summary>
        // void Initialize();

        /// <summary>
        /// 引擎统计模块实例清理接口
        /// </summary>
        // void Cleanup();

        /// <summary>
        /// 引擎统计模块实例垃圾卸载接口
        /// </summary>
        void Dump();

        /// <summary>
        /// 获取当前统计模块实例记录的所有统计项信息
        /// </summary>
        /// <returns>返回所有记录的统计项信息</returns>
        IList<IStatInfo> GetAllStatInfos();
    }
}
