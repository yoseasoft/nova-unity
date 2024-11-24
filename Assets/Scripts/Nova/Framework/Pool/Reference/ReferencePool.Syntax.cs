/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

namespace NovaEngine
{
    /// <summary>
    /// 引用对象缓冲池句柄定义
    /// </summary>
    public static partial class ReferencePool
    {
        /// <summary>
        /// 引用对象后处理函数句柄定义
        /// </summary>
        /// <param name="reference">引用对象实例</param>
        public delegate void ReferencePostProcessHandler(IReference reference);

        /// <summary>
        /// 引用对象后处理句柄管理容器
        /// </summary>
        private static IDictionary<SystemType, ReferencePostProcessInfo> s_referencePostProcessInfos = new Dictionary<SystemType, ReferencePostProcessInfo>();

        /// <summary>
        /// 添加指定对象类型的后处理句柄函数
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="createCallback">创建回调</param>
        /// <param name="releaseCallback">释放回调</param>
        public static void AddReferencePostProcess(SystemType type, ReferencePostProcessHandler createCallback, ReferencePostProcessHandler releaseCallback)
        {
            if (s_referencePostProcessInfos.ContainsKey(type))
            {
                Logger.Warn("The reference '{0}' type's post process was already exist, repeat added it will be override old value.", type.FullName);

                s_referencePostProcessInfos.Remove(type);
            }

            // 后处理管理容器中已有的类型和当前添加类型存在继承关系，不允许进行此次添加操作
            if (IsHavingInheritanceRelationshipWithinPostProcessList(type))
            {
                Logger.Error("The reference '{0}' type having inheritance relationship from post process list, added it failed.", type.FullName);

                return;
            }

            ReferencePostProcessInfo info = new ReferencePostProcessInfo(type, createCallback, releaseCallback);
            s_referencePostProcessInfos.Add(type, info);
        }

        /// <summary>
        /// 移除指定对象类型的后处理句柄函数
        /// </summary>
        /// <param name="type">对象类型</param>
        public static void RemoveReferencePostProcess(SystemType type)
        {
            if (s_referencePostProcessInfos.ContainsKey(type))
            {
                s_referencePostProcessInfos.Remove(type);
            }
        }

        /// <summary>
        /// 清除所有引用对象的后处理信息
        /// </summary>
        public static void ClearAllPostProcesses()
        {
            s_referencePostProcessInfos.Clear();
        }

        /// <summary>
        /// 通过指定的对象类型，获取其对应的后处理函数信息
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="info">后处理信息</param>
        /// <returns>若存在指定类型的后处理实例则返回true，否则返回false</returns>
        private static bool TryGetReferencePostProcessInfo(SystemType type, out ReferencePostProcessInfo info)
        {
            IEnumerator<SystemType> e = s_referencePostProcessInfos.Keys.GetEnumerator();
            info = null;
            while (e.MoveNext())
            {
                if (e.Current.IsAssignableFrom(type))
                {
                    info = s_referencePostProcessInfos[e.Current];
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查指定的对象类型与当前后处理列表中的对象是否存在继承关系
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>若存在继承关系则返回true，否则返回false</returns>
        private static bool IsHavingInheritanceRelationshipWithinPostProcessList(SystemType type)
        {
            IEnumerator<SystemType> e = s_referencePostProcessInfos.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                if (IsHavingInheritanceRelationshipForTypes(type, e.Current))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查指定的两个对象类型是否存在继承关系
        /// </summary>
        /// <param name="arg0">对象类型</param>
        /// <param name="arg1">对象类型</param>
        /// <returns>若存在继承关系则返回true，否则返回false</returns>
        private static bool IsHavingInheritanceRelationshipForTypes(SystemType arg0, SystemType arg1)
        {
            if (arg0.IsAssignableFrom(arg1) || arg1.IsAssignableFrom(arg0))
            {
                return true;
            }

            return false;
        }

        #region 引用对象后处理函数管理数据结构定义

        /// <summary>
        /// 引用对象后处理信息对象类
        /// </summary>
        private class ReferencePostProcessInfo
        {
            /// <summary>
            /// 后处理操作的目标对象类型
            /// </summary>
            private readonly SystemType m_targetType;
            /// <summary>
            /// 引用对象实例创建的后处理回调
            /// </summary>
            private readonly ReferencePostProcessHandler m_createCallback;
            /// <summary>
            /// 引用对象实例销毁的后处理回调
            /// </summary>
            private readonly ReferencePostProcessHandler m_releaseCallback;

            public SystemType TargetType => m_targetType;
            public ReferencePostProcessHandler CreateCallback => m_createCallback;
            public ReferencePostProcessHandler ReleaseCallback => m_releaseCallback;

            public ReferencePostProcessInfo(SystemType targetType, ReferencePostProcessHandler createCallback, ReferencePostProcessHandler releaseCallback)
            {
                m_targetType = targetType;
                m_createCallback = createCallback;
                m_releaseCallback = releaseCallback;
            }
        }

        #endregion
    }
}
