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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 切面管理类的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Interface | SystemAttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AspectAttribute : SystemAttribute
    {
        public AspectAttribute() : base() { }
    }

    /// <summary>
    /// 切面管理拦截的目标对象类的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Interface | SystemAttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class AspectOfTargetAttribute : AspectAttribute
    {
        /// <summary>
        /// 匹配切点的目标对象类型
        /// </summary>
        private readonly SystemType m_classType;

        public SystemType ClassType => m_classType;

        public AspectOfTargetAttribute(SystemType classType) : base()
        {
            m_classType = classType;
        }
    }

    /// <summary>
    /// 切面管理类的自动注入功能支持的属性类型定义
    /// </summary>
    // public class AspectIocSupportedAttribute : SystemAttribute { public AspectIocSupportedAttribute() : base() { } }
}
