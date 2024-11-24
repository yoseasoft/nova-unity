/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

using SystemDateTime = System.DateTime;

namespace GameEngine
{
    /// <summary>
    /// 对象统计模块，对对象模块对象提供数据统计所需的接口函数
    /// </summary>
    public sealed class ObjectStatModule : HandlerStatSingleton<ObjectStatModule>, IStatModule
    {
        public const int ON_OBJECT_CREATE_CALL = 1;
        public const int ON_OBJECT_RELEASE_CALL = 2;

        /// <summary>
        /// 对象访问统计信息容器列表
        /// </summary>
        private IList<ObjectStatInfo> m_objectStatInfos = null;

        /// <summary>
        /// 初始化统计模块实例的回调接口
        /// </summary>
        protected override void OnInitialize()
        {
            m_objectStatInfos = new List<ObjectStatInfo>();
        }

        /// <summary>
        /// 清理统计模块实例的回调接口
        /// </summary>
        protected override void OnCleanup()
        {
            m_objectStatInfos.Clear();
            m_objectStatInfos = null;
        }

        /// <summary>
        /// 卸载统计模块实例中的垃圾数据
        /// </summary>
        public void Dump()
        {
            m_objectStatInfos.Clear();
        }

        /// <summary>
        /// 获取当前所有对象访问的统计信息
        /// </summary>
        /// <returns>返回所有的操作访问统计信息</returns>
        public IList<IStatInfo> GetAllStatInfos()
        {
            List<IStatInfo> results = new List<IStatInfo>();
            results.AddRange(m_objectStatInfos);

            return results;
        }

        [IStatModule.OnStatModuleRegisterCallback(ON_OBJECT_CREATE_CALL)]
        private void OnObjectCreate(CObject obj)
        {
            ObjectStatInfo info = null;

            int uid = m_objectStatInfos.Count + 1;

            info = new ObjectStatInfo(uid, obj.Name, obj.GetHashCode());
            info.CreateTime = SystemDateTime.UtcNow;
            m_objectStatInfos.Add(info);
        }

        [IStatModule.OnStatModuleRegisterCallback(ON_OBJECT_RELEASE_CALL)]
        private void OnObjectRelease(CObject obj)
        {
            ObjectStatInfo info = null;
            if (false == TryGetObjectStatInfoByHashCode(obj.GetHashCode(), out info))
            {
                Debugger.Warn("Could not found any object stat info with name '{0}', exited it failed.", obj.Name);
                return;
            }

            info.ReleaseTime = SystemDateTime.UtcNow;
        }

        private bool TryGetObjectStatInfoByHashCode(int hashCode, out ObjectStatInfo info)
        {
            for (int n = m_objectStatInfos.Count - 1; n >= 0; --n)
            {
                ObjectStatInfo found = m_objectStatInfos[n];
                if (found.HashCode == hashCode)
                {
                    info = found;
                    return true;
                }
            }

            info = null;
            return false;
        }
    }
}
