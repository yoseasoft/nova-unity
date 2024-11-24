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

using System.Collections.Generic;

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 原型对象管理类，用于对场景上下文中的所有原型对象提供通用的访问操作接口
    /// </summary>
    public sealed partial class ProtoController
    {
        /// <summary>
        /// 组件对象类型映射注册管理容器
        /// </summary>
        private NovaEngine.DoubleMap<string, SystemType> m_componentClassTypes = null;

        /// <summary>
        /// 原型管理对象的组件模块初始化通知接口函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void OnComponentInitialize()
        {
            // 初始化组件类注册容器
            m_componentClassTypes = new NovaEngine.DoubleMap<string, SystemType>();
        }

        /// <summary>
        /// 原型管理对象的组件模块清理通知接口函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void OnComponentCleanup()
        {
            // 清理组件类注册容器
            UnregisterAllComponentClasses();
            m_componentClassTypes = null;
        }

        #region 组件类注册访问接口函数

        /// <summary>
        /// 通过指定的组件名称获取其对应的组件对象类型
        /// </summary>
        /// <param name="componentName">组件名称</param>
        /// <returns>返回名称对应的组件对象类型，若不存在则返回null</returns>
        public SystemType FindComponentTypeByName(string componentName)
        {
            if (m_componentClassTypes.TryGetValueByKey(componentName, out SystemType componentType))
            {
                return componentType;
            }

            return null;
        }

        /// <summary>
        /// 通过指定的组件对象类型获取其对应的组件名称
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>返回给定组件类型对应的名称</returns>
        public string GetComponentNameByType(SystemType componentType)
        {
            if (m_componentClassTypes.TryGetKeyByValue(componentType, out string componentName))
            {
                return componentName;
            }

            return null;
        }

        /// <summary>
        /// 注册指定的组件名称及对应的对象类到当前的句柄管理容器中
        /// 注意，注册的对象类必须继承自<see cref="GameEngine.CComponent"/>，否则无法正常注册
        /// </summary>
        /// <param name="componentName">组件名称</param>
        /// <param name="clsType">组件类型</param>
        /// <returns>若组件类型注册成功则返回true，否则返回false</returns>
        internal bool RegisterComponentClass(string componentName, SystemType clsType)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(componentName) && null != clsType, "Invalid arguments");

            if (false == typeof(CComponent).IsAssignableFrom(clsType))
            {
                Debugger.Warn("The register type {0} must be inherited from 'CComponent'.", clsType.Name);
                return false;
            }

            if (m_componentClassTypes.ContainsKey(componentName))
            {
                Debugger.Warn("The component name '{0}' was already registed, repeat add will be override old name.", componentName);
                m_componentClassTypes.RemoveByKey(componentName);
            }

            m_componentClassTypes.Add(componentName, clsType);

            return true;
        }

        /// <summary>
        /// 从当前的句柄管理容器中注销指定的组件名称及对应的实例
        /// </summary>
        /// <param name="componentName">组件名称</param>
        internal void UnregisterComponentClass(string componentName)
        {
            if (false == m_componentClassTypes.ContainsKey(componentName))
            {
                Debugger.Warn("Could not found any component with target name '{0}', removed it failed.", componentName);
                return;
            }

            m_componentClassTypes.RemoveByKey(componentName);
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有组件类型
        /// </summary>
        internal void UnregisterAllComponentClasses()
        {
            m_componentClassTypes.Clear();
        }

        #endregion
    }
}
