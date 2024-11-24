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
    /// 在发布模式下启用的状态标识的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Interface | SystemAttributeTargets.Class | SystemAttributeTargets.Field | SystemAttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EnableOnReleaseModeAttribute : SystemAttribute
    {
        public EnableOnReleaseModeAttribute() : base() { }
    }

    /// <summary>
    /// 在发布模式下禁用的状态标识的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Interface | SystemAttributeTargets.Class | SystemAttributeTargets.Field | SystemAttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DisableOnReleaseModeAttribute : SystemAttribute
    {
        public DisableOnReleaseModeAttribute() : base() { }
    }

    /// <summary>
    /// 在发布模式下进行指定参数设置的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Interface | SystemAttributeTargets.Class | SystemAttributeTargets.Field | SystemAttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class AssignableOnReleaseModeAttribute : SystemAttribute
    {
        /// <summary>
        /// 参数值
        /// </summary>
        private object m_value;

        public object Value => m_value;

        public AssignableOnReleaseModeAttribute(object value) : base()
        {
            m_value = value;
        }
    }
}
