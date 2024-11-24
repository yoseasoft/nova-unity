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

using UnityGameObject = UnityEngine.GameObject;

namespace GameEngine
{
    /// <summary>
    /// 对象模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.ObjectModule"/>类
    /// </summary>
    public sealed partial class ObjectHandler : EcsmHandler
    {
        /// <summary>
        /// 节点对象类型映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, SystemType> m_objectClassTypes;
        /// <summary>
        /// 节点功能类型映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, int> m_objectFunctionTypes;

        /// <summary>
        /// 通过当前模块实例化的对象实例管理容器
        /// </summary>
        private readonly IList<CObject> m_objects;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static ObjectHandler Instance => HandlerManagement.ObjectHandler;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        public ObjectHandler()
        {
            // 初始化对象类注册容器
            m_objectClassTypes = new Dictionary<string, SystemType>();
            m_objectFunctionTypes = new Dictionary<string, int>();
            m_objects = new List<CObject>();
        }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~ObjectHandler()
        {
            // 清理对象类注册容器
            m_objectClassTypes.Clear();
            m_objectFunctionTypes.Clear();
            m_objects.Clear();
        }

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            if (false == base.OnInitialize()) return false;

            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 移除全部对象实例
            RemoveAllObjects();

            // 清理对象类型注册列表
            UnregisterAllObjectClasses();

            base.OnCleanup();
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        /// <summary>
        /// 句柄对象内置延迟刷新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        public override void OnEventDispatch(NovaEngine.ModuleEventArgs e)
        {
        }

        #region 对象类注册/注销接口函数

        /// <summary>
        /// 注册指定的对象名称及对应的对象类到当前的句柄管理容器中
        /// 注意，注册的对象类必须继承自<see cref="GameEngine.CObject"/>，否则无法正常注册
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <param name="clsType">对象类型</param>
        /// <param name="funcType">功能类型</param>
        /// <returns>若对象类型注册成功则返回true，否则返回false</returns>
        private bool RegisterObjectClass(string objectName, SystemType clsType, int funcType)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(objectName) && null != clsType, "Invalid arguments");

            if (false == typeof(CObject).IsAssignableFrom(clsType))
            {
                Debugger.Warn("The register type {0} must be inherited from 'CObject'.", clsType.Name);
                return false;
            }

            if (m_objectClassTypes.ContainsKey(objectName))
            {
                Debugger.Warn("The object name {0} was already registed, repeat add will be override old name.", objectName);
                m_objectClassTypes.Remove(objectName);
            }

            m_objectClassTypes.Add(objectName, clsType);
            if (funcType > 0)
            {
                m_objectFunctionTypes.Add(objectName, funcType);
            }

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有对象类型
        /// </summary>
        private void UnregisterAllObjectClasses()
        {
            m_objectClassTypes.Clear();
            m_objectFunctionTypes.Clear();
        }

        #endregion

        #region 节点对象实例访问操作函数合集

        /// <summary>
        /// 通过指定的对象名称从实例容器中获取对应的对象实例列表
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <returns>返回对象实例列表，若检索失败则返回null</returns>
        public IList<CObject> GetObject(string objectName)
        {
            SystemType objectType = null;
            if (m_objectClassTypes.TryGetValue(objectName, out objectType))
            {
                return GetObject(objectType);
            }

            return null;
        }

        /// <summary>
        /// 通过指定的对象类型从实例容器中获取对应的对象实例列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回对象实例列表，若检索失败则返回null</returns>
        public IList<T> GetObject<T>() where T : CObject
        {
            SystemType objectType = typeof(T);

            return NovaEngine.Utility.Collection.CastAndToList<CObject, T>(GetObject(objectType));
        }

        /// <summary>
        /// 通过指定的对象类型从实例容器中获取对应的对象实例列表
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回对象实例列表，若检索失败则返回null</returns>
        public IList<CObject> GetObject(SystemType objectType)
        {
            List<CObject> objects = new List<CObject>();
            for (int n = 0; n < m_objects.Count; ++n)
            {
                CObject obj = m_objects[n];
                if (obj.GetType() == objectType)
                {
                    objects.Add(obj);
                }
            }

            if (objects.Count <= 0)
            {
                objects = null;
            }

            return objects;
        }

        /// <summary>
        /// 获取当前已创建的全部对象实例
        /// </summary>
        /// <returns>返回已创建的全部对象实例</returns>
        public IList<CObject> GetAllObjects()
        {
            return m_objects;
        }

        /// <summary>
        /// 通过指定的对象名称动态创建一个对应的对象实例
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public CObject CreateObject(string objectName)
        {
            SystemType objectType = null;
            if (false == m_objectClassTypes.TryGetValue(objectName, out objectType))
            {
                Debugger.Warn("Could not found any correct object class with target name '{0}', created object failed.", objectName);
                return null;
            }

            return CreateObject(objectType);
        }

        /// <summary>
        /// 通过指定的对象类型动态创建一个对应的对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public T CreateObject<T>() where T : CObject
        {
            SystemType objectType = typeof(T);

            return CreateObject(objectType) as T;
        }

        /// <summary>
        /// 通过指定的对象类型动态创建一个对应的对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public CObject CreateObject(SystemType objectType)
        {
            Debugger.Assert(null != objectType, "Invalid arguments.");
            if (false == m_objectClassTypes.Values.Contains(objectType))
            {
                Debugger.Error("Could not found any correct object class with target type '{0}', created object failed.", objectType.FullName);
                return null;
            }

            // 对象实例化
            CObject obj = CreateInstance(objectType) as CObject;
            if (false == AddEntity(obj))
            {
                Debugger.Warn("The object instance '{0}' initialization for error, added it failed.", objectType.FullName);
                return null;
            }

            // 添加实例到管理容器中
            m_objects.Add(obj);

            // 启动对象实例
            Call(obj.Startup);

            // 唤醒对象实例
            CallEntityAwakeProcess(obj);

            ObjectStatModule.CallStatAction(ObjectStatModule.ON_OBJECT_CREATE_CALL, obj);

            return obj;
        }

        /// <summary>
        /// 从当前对象管理容器中移除指定的对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        internal void RemoveObject(CObject obj)
        {
            if (false == m_objects.Contains(obj))
            {
                Debugger.Warn("Could not found target object instance '{0}' from current container, removed it failed.", obj.GetType().FullName);
                return;
            }

            ObjectStatModule.CallStatAction(ObjectStatModule.ON_OBJECT_RELEASE_CALL, obj);

            // 销毁对象实例
            CallEntityDestroyProcess(obj);

            // 关闭对象实例
            Call(obj.Shutdown);

            // 从管理容器中移除实例
            m_objects.Remove(obj);

            // 移除实例
            RemoveEntity(obj);

            // 回收对象实例
            ReleaseInstance(obj);
        }

        /// <summary>
        /// 从当前对象管理容器中移除指定名称对应的所有对象实例
        /// </summary>
        /// <param name="objectName">对象名称</param>
        internal void RemoveObject(string objectName)
        {
            SystemType objectType = null;
            if (m_objectClassTypes.TryGetValue(objectName, out objectType))
            {
                RemoveObject(objectType);
            }
        }

        /// <summary>
        /// 从当前对象管理容器中移除指定类型对应的所有对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        internal void RemoveObject<T>() where T : CObject
        {
            SystemType objectType = typeof(T);

            RemoveObject(objectType);
        }

        /// <summary>
        /// 从当前对象管理容器中移除指定类型对应的所有对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        internal void RemoveObject(SystemType objectType)
        {
            IEnumerable<CObject> objects = NovaEngine.Utility.Collection.Reverse<CObject>(m_objects);
            foreach (CObject obj in objects)
            {
                if (obj.GetType() == objectType)
                {
                    RemoveObject(obj);
                }
            }
        }

        /// <summary>
        /// 从当前对象管理容器中移除所有注册对对象实例
        /// </summary>
        internal void RemoveAllObjects()
        {
            while (m_objects.Count > 0)
            {
                RemoveObject(m_objects[m_objects.Count - 1]);
            }
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定的对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        public void DestroyObject(CObject obj)
        {
            if (false == m_objects.Contains(obj))
            {
                Debugger.Warn("Could not found target object instance '{0}' from current container, removed it failed.", obj.GetType().FullName);
                return;
            }

            // 刷新状态时推到销毁队列中
            // if ( /* false == obj.IsOnSchedulingProcessForTargetLifecycle(CBase.LifecycleKeypointType.Start) || */ obj.IsOnUpdatingStatus())
            if (obj.CurrentLifecycleScheduleRunning)
            {
                obj.OnPrepareToDestroy();
                return;
            }

            // 在非逻辑刷新的状态下，直接移除对象即可
            RemoveObject(obj);
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定名称对应的所有对象实例
        /// </summary>
        /// <param name="objectName">对象名称</param>
        public void DestroyObject(string objectName)
        {
            SystemType objectType = null;
            if (m_objectClassTypes.TryGetValue(objectName, out objectType))
            {
                DestroyObject(objectType);
            }
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定类型对应的所有对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public void DestroyObject<T>() where T : CObject
        {
            SystemType objectType = typeof(T);

            DestroyObject(objectType);
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定类型对应的所有对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        public void DestroyObject(SystemType objectType)
        {
            IEnumerable<CObject> objects = NovaEngine.Utility.Collection.Reverse<CObject>(m_objects);
            foreach (CObject obj in objects)
            {
                if (obj.GetType() == objectType)
                {
                    DestroyObject(obj);
                }
            }
        }

        /// <summary>
        /// 从当前对象管理容器中销毁所有注册对对象实例
        /// </summary>
        public void DestroyAllObjects()
        {
            while (m_objects.Count > 0)
            {
                DestroyObject(m_objects[m_objects.Count - 1]);
            }
        }

        #endregion

        #region 节点对象扩展操作函数合集

        /// <summary>
        /// 同步实例化对象
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        public UnityGameObject Instantiate(string url)
        {
            return ResourceModule.InstantiateObject(url);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 通过指定的对象类型获取对应对象的名称
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回对应对象的名称，若对象不存在则返回null</returns>
        internal string GetObjectNameForType<T>() where T : CObject
        {
            return GetObjectNameForType(typeof(T));
        }

        /// <summary>
        /// 通过指定的对象类型获取对应对象的名称
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回对应对象的名称，若对象不存在则返回null</returns>
        internal string GetObjectNameForType(SystemType objectType)
        {
            foreach (KeyValuePair<string, SystemType> pair in m_objectClassTypes)
            {
                if (pair.Value == objectType)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// 通过指定的对象类型，搜索该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        internal IList<CObject> FindAllObjectsByType(SystemType objectType)
        {
            IList<CObject> result = new List<CObject>();
            IEnumerator<CObject> e = m_objects.GetEnumerator();
            while (e.MoveNext())
            {
                CObject obj = e.Current;
                if (objectType.IsAssignableFrom(obj.GetType()))
                {
                    result.Add(obj);
                }
            }

            // 如果搜索结果为空，则直接返回null
            if (result.Count <= 0)
            {
                result = null;
            }

            return result;
        }

        #endregion
    }
}
