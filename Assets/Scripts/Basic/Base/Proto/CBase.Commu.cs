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
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 基础对象抽象类，对需要进行对象定义的场景提供一个通用的基类
    /// </summary>
    public abstract partial class CBase
    {
        /// <summary>
        /// 基础对象内部订阅事件的监听回调容器列表
        /// </summary>
        private IDictionary<int, IDictionary<string, EventCallSyntaxInfo>> m_eventCallInfosForId;
        /// <summary>
        /// 基础对象内部订阅事件的监听回调容器列表
        /// </summary>
        private IDictionary<SystemType, IDictionary<string, EventCallSyntaxInfo>> m_eventCallInfosForType;

        /// <summary>
        /// 基础对象内部消息通知的监听回调的映射容器列表
        /// </summary>
        private IDictionary<int, IDictionary<string, MessageCallSyntaxInfo>> m_messageCallInfosForType;

        /// <summary>
        /// 基础对象的通信模块包初始化函数接口
        /// </summary>
        private void OnCommuPackageInitialize()
        {
            // 事件监听回调映射容器初始化
            m_eventCallInfosForId = new Dictionary<int, IDictionary<string, EventCallSyntaxInfo>>();
            m_eventCallInfosForType = new Dictionary<SystemType, IDictionary<string, EventCallSyntaxInfo>>();

            // 消息监听回调映射容器初始化
            m_messageCallInfosForType = new Dictionary<int, IDictionary<string, MessageCallSyntaxInfo>>();
        }

        /// <summary>
        /// 基础对象的通信模块包清理函数接口
        /// </summary>
        private void OnCommuPackageCleanup()
        {
            // 移除所有订阅事件
            UnsubscribeAllEvents();
            m_eventCallInfosForId = null;
            m_eventCallInfosForType = null;

            // 移除所有消息通知
            RemoveAllMessageListeners();
            m_messageCallInfosForType = null;
        }

        #region 基础对象事件转发相关操作函数合集

        /// <summary>
        /// 基础对象的订阅事件的监听回调函数<br/>
        /// 该函数针对事件转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析事件，可以通过重写<see cref="GameEngine.CBase.OnEvent(int, object[])"/>实现事件的自定义处理逻辑
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        public virtual void OnEventDispatchForId(int eventID, params object[] args)
        {
            if (m_eventCallInfosForId.TryGetValue(eventID, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                IEnumerator<EventCallSyntaxInfo> e = infos.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Invoke(eventID, args);
                }
            }

            OnEvent(eventID, args);
        }

        /// <summary>
        /// 基础对象的订阅事件的监听回调函数<br/>
        /// 该函数针对事件转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析事件，可以通过重写<see cref="GameEngine.CBase.OnEvent(object)"/>实现事件的自定义处理逻辑
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public virtual void OnEventDispatchForType(object eventData)
        {
            if (m_eventCallInfosForType.TryGetValue(eventData.GetType(), out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                IEnumerator<EventCallSyntaxInfo> e = infos.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Invoke(eventData);
                }
            }

            OnEvent(eventData);
        }

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        protected abstract void OnEvent(int eventID, params object[] args);

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventData">事件数据</param>
        protected abstract void OnEvent(object eventData);

        /// <summary>
        /// 针对指定事件标识新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnSubscribeActionPostProcess(int eventID);
        /// <summary>
        /// 针对指定事件类型新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnSubscribeActionPostProcess(SystemType eventType);
        /// <summary>
        /// 针对指定事件标识移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        protected abstract void OnUnsubscribeActionPostProcess(int eventID);
        /// <summary>
        /// 针对指定事件类型移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        protected abstract void OnUnsubscribeActionPostProcess(SystemType eventType);

        /// <summary>
        /// 检测当前基础对象是否订阅了目标事件标识
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>若订阅了给定事件标识则返回true，否则返回false</returns>
        protected internal virtual bool IsSubscribedOfTargetId(int eventID)
        {
            if (m_eventCallInfosForId.ContainsKey(eventID) && m_eventCallInfosForId[eventID].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前基础对象是否订阅了目标事件类型
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>若订阅了给定事件类型则返回true，否则返回false</returns>
        protected internal virtual bool IsSubscribedOfTargetType(SystemType eventType)
        {
            if (m_eventCallInfosForType.ContainsKey(eventType) && m_eventCallInfosForType[eventType].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public virtual bool Subscribe(int eventID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe(int eventID, SystemMethodInfo methodInfo)
        {
            return Subscribe(eventID, methodInfo, false);
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        protected internal bool Subscribe(int eventID, SystemMethodInfo methodInfo, bool automatically)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfProtoExtendEventCallFunction(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfEventCallFunction(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error("The event listener '{0}' was invalid format, subscribed it failed.", NovaEngine.Utility.Text.ToString(methodInfo));
                    return false;
                }
            }

            EventCallSyntaxInfo info = new EventCallSyntaxInfo(this, eventID, methodInfo, automatically);

            if (false == m_eventCallInfosForId.TryGetValue(eventID, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                // 创建监听列表
                infos = new Dictionary<string, EventCallSyntaxInfo>();
                infos.Add(info.Fullname, info);

                m_eventCallInfosForId.Add(eventID, infos);

                // 新增事件订阅的后处理程序
                return OnSubscribeActionPostProcess(eventID);
            }

            if (infos.ContainsKey(info.Fullname))
            {
                Debugger.Warn("The '{0}' instance's event '{1}' was already add same listener by name '{2}', repeat do it failed.",
                        NovaEngine.Utility.Text.ToString(GetType()), eventID, info.Fullname);
                return false;
            }

            infos.Add(info.Fullname, info);

            return true;
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe<T>() where T : struct
        {
            return Subscribe(typeof(T));
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public virtual bool Subscribe(SystemType eventType)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="func">监听回调函数</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe<T>(System.Action<T> func) where T : struct
        {
            return Subscribe(typeof(T), func.Method);
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe(SystemType eventType, SystemMethodInfo methodInfo)
        {
            return Subscribe(eventType, methodInfo, false);
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        protected internal bool Subscribe(SystemType eventType, SystemMethodInfo methodInfo, bool automatically)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfProtoExtendEventCallFunction(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfEventCallFunction(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error("The event listener '{0}' was invalid format, subscribed it failed.", NovaEngine.Utility.Text.ToString(methodInfo));
                    return false;
                }
            }

            EventCallSyntaxInfo info = new EventCallSyntaxInfo(this, eventType, methodInfo, automatically);

            if (false == m_eventCallInfosForType.TryGetValue(eventType, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                // 创建监听列表
                infos = new Dictionary<string, EventCallSyntaxInfo>();
                infos.Add(info.Fullname, info);

                m_eventCallInfosForType.Add(eventType, infos);

                // 新增事件订阅的后处理程序
                return OnSubscribeActionPostProcess(eventType);
            }

            if (infos.ContainsKey(info.Fullname))
            {
                Debugger.Warn("The '{0}' instance's event '{1}' was already add same listener by name '{2}', repeat do it failed.",
                        NovaEngine.Utility.Text.ToString(GetType()), eventType.FullName, info.Fullname);
                return false;
            }

            infos.Add(info.Fullname, info);

            return true;
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <param name="eventID">事件标识</param>
        public virtual void Unsubscribe(int eventID)
        {
            // 若针对特定事件绑定了监听回调，则移除相应的回调句柄
            if (m_eventCallInfosForId.ContainsKey(eventID))
            {
                m_eventCallInfosForId.Remove(eventID);
            }

            // 移除事件订阅的后处理程序
            OnUnsubscribeActionPostProcess(eventID);
        }

        /// <summary>
        /// 取消当前基础对象对指定扩展事件对应的监听回调函数
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void Unsubscribe(int eventID, SystemMethodInfo methodInfo)
        {
            string funcName = EventCallSyntaxInfo.GenCallName(methodInfo);

            Unsubscribe(eventID, funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定扩展事件对应的监听回调函数
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="funcName">函数名称</param>
        protected internal void Unsubscribe(int eventID, string funcName)
        {
            if (m_eventCallInfosForId.TryGetValue(eventID, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                if (infos.ContainsKey(funcName))
                {
                    infos.Remove(funcName);
                }
            }

            // 当前监听列表为空时，移除该事件的监听
            if (false == IsSubscribedOfTargetId(eventID))
            {
                Unsubscribe(eventID);
            }
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        public void Unsubscribe<T>()
        {
            Unsubscribe(typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public virtual void Unsubscribe(SystemType eventType)
        {
            // 若针对特定事件绑定了监听回调，则移除相应的回调句柄
            if (m_eventCallInfosForType.ContainsKey(eventType))
            {
                m_eventCallInfosForType.Remove(eventType);
            }

            // 移除事件订阅的后处理程序
            OnUnsubscribeActionPostProcess(eventType);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件对应的监听回调函数
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="methodInfo">监听回调函数</param>
        public void Unsubscribe<T>(SystemMethodInfo methodInfo)
        {
            Unsubscribe(typeof(T), methodInfo);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void Unsubscribe(SystemType eventType, SystemMethodInfo methodInfo)
        {
            string funcName = EventCallSyntaxInfo.GenCallName(methodInfo);

            Unsubscribe(eventType, funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件对应的监听回调函数
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="funcName">函数名称</param>
        protected internal void Unsubscribe<T>(string funcName)
        {
            Unsubscribe(typeof(T), funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件对应的监听回调函数
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="funcName">函数名称</param>
        protected internal void Unsubscribe(SystemType eventType, string funcName)
        {
            if (m_eventCallInfosForType.TryGetValue(eventType, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                if (infos.ContainsKey(funcName))
                {
                    infos.Remove(funcName);
                }
            }

            // 当前监听列表为空时，移除该事件的监听
            if (false == IsSubscribedOfTargetType(eventType))
            {
                Unsubscribe(eventType);
            }
        }

        /// <summary>
        /// 移除所有自动注册的事件订阅回调接口
        /// </summary>
        protected internal void UnsubscribeAllAutomaticallyEvents()
        {
            OnAutomaticallyCallSyntaxInfoProcessHandler<int, EventCallSyntaxInfo>(m_eventCallInfosForId, Unsubscribe);
            OnAutomaticallyCallSyntaxInfoProcessHandler<SystemType, EventCallSyntaxInfo>(m_eventCallInfosForType, Unsubscribe);
        }

        /// <summary>
        /// 取消当前基础对象的所有事件订阅
        /// </summary>
        public virtual void UnsubscribeAllEvents()
        {
            IList<int> id_keys = NovaEngine.Utility.Collection.ToListForKeys<int, IDictionary<string, EventCallSyntaxInfo>>(m_eventCallInfosForId);
            for (int n = 0; null != id_keys && n < id_keys.Count; ++n) { Unsubscribe(id_keys[n]); }

            IList<SystemType> type_keys = NovaEngine.Utility.Collection.ToListForKeys<SystemType, IDictionary<string, EventCallSyntaxInfo>>(m_eventCallInfosForType);
            for (int n = 0; null != type_keys && n < type_keys.Count; ++n) { Unsubscribe(type_keys[n]); }

            m_eventCallInfosForId.Clear();
            m_eventCallInfosForType.Clear();
        }

        #endregion

        #region 基础对象消息通知相关操作函数合集

        /// <summary>
        /// 基础对象的消息通知的监听回调函数<br/>
        /// 该函数针对消息转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行处理消息，可以通过重写<see cref="GameEngine.CBase.OnMessage(ProtoBuf.Extension.IMessage)"/>实现消息的自定义处理逻辑
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="message">消息对象实例</param>
        public virtual void OnMessageDispatch(int opcode, ProtoBuf.Extension.IMessage message)
        {
            if (m_messageCallInfosForType.TryGetValue(opcode, out IDictionary<string, MessageCallSyntaxInfo> infos))
            {
                IEnumerator<MessageCallSyntaxInfo> e = infos.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Invoke(message);
                }
            }

            OnMessage(opcode, message);
        }

        /// <summary>
        /// 用户自定义的消息处理函数，您可以通过重写该函数处理自定义消息通知
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="message">消息对象实例</param>
        protected abstract void OnMessage(int opcode, ProtoBuf.Extension.IMessage message);

        /// <summary>
        /// 针对指定消息标识新增消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnMessageListenerAddedActionPostProcess(int opcode);
        /// <summary>
        /// 针对指定消息标识移除消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        protected abstract void OnMessageListenerRemovedActionPostProcess(int opcode);

        /// <summary>
        /// 检测当前基础对象是否监听了目标消息类型
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若监听了给定消息类型则返回true，否则返回false</returns>
        protected internal virtual bool IsMessageListenedOfTargetType(int opcode)
        {
            if (m_messageCallInfosForType.ContainsKey(opcode) && m_messageCallInfosForType[opcode].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，对一个指定的消息进行转发监听
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public virtual bool AddMessageListener(int opcode)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的协议码绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener(int opcode, SystemMethodInfo methodInfo)
        {
            return AddMessageListener(opcode, methodInfo, false);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的协议码绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListener(int opcode, SystemMethodInfo methodInfo, bool automatically)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfProtoExtendMessageCallFunction(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfMessageCallFunction(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error("The message dispatch listener '{0}' was invalid format, added it failed.", NovaEngine.Utility.Text.ToString(methodInfo));
                    return false;
                }
            }

            MessageCallSyntaxInfo info = new MessageCallSyntaxInfo(this, opcode, methodInfo, automatically);

            if (false == m_messageCallInfosForType.TryGetValue(opcode, out IDictionary<string, MessageCallSyntaxInfo> infos))
            {
                // 创建监听列表
                infos = new Dictionary<string, MessageCallSyntaxInfo>();
                infos.Add(info.Fullname, info);

                m_messageCallInfosForType.Add(opcode, infos);

                // 新增消息监听的后处理程序
                return OnMessageListenerAddedActionPostProcess(opcode);
            }

            if (infos.ContainsKey(info.Fullname))
            {
                Debugger.Warn("The 'CComponent' instance's message type '{0}' was already add same listener by name '{1}', repeat added it failed.", opcode, info.Fullname);
                return false;
            }

            infos.Add(info.Fullname, info);

            return true;
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener<T>() where T : ProtoBuf.Extension.IMessage
        {
            return AddMessageListener(typeof(T));
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener(SystemType messageType)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            return AddMessageListener(opcode);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="func">监听回调函数</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener<T>(System.Action<T> func) where T : ProtoBuf.Extension.IMessage
        {
            return AddMessageListener(typeof(T), func.Method);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener(SystemType messageType, SystemMethodInfo methodInfo)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            return AddMessageListener(opcode, methodInfo);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListener(SystemType messageType, SystemMethodInfo methodInfo, bool automatically)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            return AddMessageListener(opcode, methodInfo, automatically);
        }

        /// <summary>
        /// 取消当前基础对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        public virtual void RemoveMessageListener(int opcode)
        {
            // 若针对特定消息绑定了监听回调，则移除相应的回调句柄
            if (m_messageCallInfosForType.ContainsKey(opcode))
            {
                m_messageCallInfosForType.Remove(opcode);
            }

            // 移除消息监听的后处理程序
            OnMessageListenerRemovedActionPostProcess(opcode);
        }

        /// <summary>
        /// 取消当前基础对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void RemoveMessageListener(int opcode, SystemMethodInfo methodInfo)
        {
            string funcName = MessageCallSyntaxInfo.GenCallName(methodInfo);

            RemoveMessageListener(opcode, funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveMessageListener(int opcode, string funcName)
        {
            if (m_messageCallInfosForType.TryGetValue(opcode, out IDictionary<string, MessageCallSyntaxInfo> infos))
            {
                if (infos.ContainsKey(funcName))
                {
                    infos.Remove(funcName);
                }
            }

            // 当前监听列表为空时，移除该消息的监听
            if (false == IsMessageListenedOfTargetType(opcode))
            {
                RemoveMessageListener(opcode);
            }
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        public void RemoveMessageListener<T>()
        {
            RemoveMessageListener(typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <param name="messageType">消息类型</param>
        public void RemoveMessageListener(SystemType messageType)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            RemoveMessageListener(opcode);
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="methodInfo">监听回调函数</param>
        public void RemoveMessageListener<T>(SystemMethodInfo methodInfo)
        {
            RemoveMessageListener(typeof(T), methodInfo);
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void RemoveMessageListener(SystemType messageType, SystemMethodInfo methodInfo)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            RemoveMessageListener(opcode, methodInfo);
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveMessageListener<T>(string funcName)
        {
            RemoveMessageListener(typeof(T), funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveMessageListener(SystemType messageType, string funcName)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            RemoveMessageListener(opcode, funcName);
        }

        /// <summary>
        /// 移除所有自动注册的消息监听回调接口
        /// </summary>
        protected internal void RemoveAllAutomaticallyMessageListeners()
        {
            OnAutomaticallyCallSyntaxInfoProcessHandler<int, MessageCallSyntaxInfo>(m_messageCallInfosForType, RemoveMessageListener);
        }

        /// <summary>
        /// 取消当前基础对象的所有注册的消息监听回调
        /// </summary>
        public virtual void RemoveAllMessageListeners()
        {
            IList<int> id_keys = NovaEngine.Utility.Collection.ToListForKeys<int, IDictionary<string, MessageCallSyntaxInfo>>(m_messageCallInfosForType);
            for (int n = 0; null != id_keys && n < id_keys.Count; ++n) { RemoveMessageListener(id_keys[n]); }

            m_messageCallInfosForType.Clear();
        }

        #endregion

        #region 基础回调接口包装类型定义

        protected class BaseCallSyntaxInfo
        {
            /// <summary>
            /// 回调函数的目标对象实例
            /// </summary>
            protected readonly CBase m_targetObject;
            /// <summary>
            /// 回调函数的完整名称
            /// </summary>
            protected readonly string m_fullname;
            /// <summary>
            /// 回调函数的函数信息实例
            /// </summary>
            protected readonly SystemMethodInfo m_methodInfo;
            /// <summary>
            /// 回调函数的动态构建回调句柄
            /// </summary>
            protected readonly SystemDelegate m_callback;
            /// <summary>
            /// 回调函数的自动注册状态标识
            /// </summary>
            protected readonly bool m_automatically;
            /// <summary>
            /// 回调函数的扩展定义状态标识
            /// </summary>
            protected readonly bool m_isExtensionType;
            /// <summary>
            /// 回调函数的无参状态标识
            /// </summary>
            protected readonly bool m_isNullParameterType;

            public string Fullname => m_fullname;
            public SystemMethodInfo MethodInfo => m_methodInfo;
            public SystemDelegate Callback => m_callback;
            public bool Automatically => m_automatically;
            public bool IsExtensionType => m_isExtensionType;
            public bool IsNullParameterType => m_isNullParameterType;

            protected BaseCallSyntaxInfo(CBase targetObject, SystemMethodInfo methodInfo, bool automatically)
            {
                m_targetObject = targetObject;
                m_methodInfo = methodInfo;
                m_automatically = automatically;
                m_isExtensionType = NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo);
                m_isNullParameterType = Loader.Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(methodInfo);

                object obj = targetObject;
                if (m_isExtensionType)
                {
                    // 扩展函数在构建委托时不需要传入运行时对象实例，而是在调用时传入
                    obj = null;
                }

                string fullname = GenCallName(methodInfo);

                SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(obj, methodInfo);
                Debugger.Assert(null != callback, "Invalid method type.");

                m_fullname = fullname;
                m_callback = callback;
            }

            /// <summary>
            /// 根据函数信息生成事件回调的名字标签
            /// </summary>
            /// <param name="methodInfo">函数对象信息</param>
            /// <returns>返回通过函数信息生成的名字标签</returns>
            protected internal static string GenCallName(SystemMethodInfo methodInfo)
            {
                return NovaEngine.Utility.Text.GetFullName(methodInfo);
            }
        }

        #endregion

        #region 事件回调接口包装结构及处理函数声明

        /// <summary>
        /// 事件回调接口的包装对象类
        /// </summary>
        protected class EventCallSyntaxInfo : BaseCallSyntaxInfo
        {
            /// <summary>
            /// 事件回调绑定的事件标识
            /// </summary>
            private readonly int m_eventID;
            /// <summary>
            /// 事件回调绑定的事件数据类型
            /// </summary>
            private readonly SystemType m_eventType;

            public int EventID => m_eventID;
            public SystemType EventType => m_eventType;

            public EventCallSyntaxInfo(CBase targetObject, int eventID, SystemMethodInfo methodInfo) : this(targetObject, eventID, methodInfo, false)
            { }

            public EventCallSyntaxInfo(CBase targetObject, int eventID, SystemMethodInfo methodInfo, bool automatically) : this(targetObject, eventID, null, methodInfo, automatically)
            { }

            public EventCallSyntaxInfo(CBase targetObject, SystemType eventType, SystemMethodInfo methodInfo) : this(targetObject, eventType, methodInfo, false)
            { }

            public EventCallSyntaxInfo(CBase targetObject, SystemType eventType, SystemMethodInfo methodInfo, bool automatically) : this(targetObject, 0, eventType, methodInfo, automatically)
            { }

            private EventCallSyntaxInfo(CBase targetObject, int eventID, SystemType eventType, SystemMethodInfo methodInfo, bool automatically) : base(targetObject, methodInfo, automatically)
            {
                m_eventID = eventID;
                m_eventType = eventType;
            }

            /// <summary>
            /// 事件回调的调度函数
            /// </summary>
            /// <param name="eventID">事件标识</param>
            /// <param name="args">事件数据参数</param>
            public void Invoke(int eventID, params object[] args)
            {
                if (m_isExtensionType)
                {
                    if (m_isNullParameterType)
                    {
                        m_callback.DynamicInvoke(m_targetObject);
                    }
                    else
                    {
                        m_callback.DynamicInvoke(m_targetObject, eventID, args);
                    }
                }
                else
                {
                    if (m_isNullParameterType)
                    {
                        m_callback.DynamicInvoke();
                    }
                    else
                    {
                        m_callback.DynamicInvoke(eventID, args);
                    }
                }
            }

            /// <summary>
            /// 事件回调的调度函数
            /// </summary>
            /// <param name="eventData">事件数据</param>
            public void Invoke(object eventData)
            {
                if (m_isExtensionType)
                {
                    if (m_isNullParameterType)
                    {
                        m_callback.DynamicInvoke(m_targetObject);
                    }
                    else
                    {
                        m_callback.DynamicInvoke(m_targetObject, eventData);
                    }
                }
                else
                {
                    if (m_isNullParameterType)
                    {
                        m_callback.DynamicInvoke();
                    }
                    else
                    {
                        m_callback.DynamicInvoke(eventData);
                    }
                }
            }
        }

        #endregion

        #region 消息回调接口包装结构及处理函数声明

        /// <summary>
        /// 消息回调接口的包装对象类
        /// </summary>
        protected class MessageCallSyntaxInfo : BaseCallSyntaxInfo
        {
            /// <summary>
            /// 消息回调绑定的协议标识
            /// </summary>
            private readonly int m_opcode;
            /// <summary>
            /// 消息回调绑定的协议对象类型
            /// </summary>
            private readonly SystemType m_messageType;

            public int Opcode => m_opcode;
            public SystemType MessageType => m_messageType;

            public MessageCallSyntaxInfo(CBase targetObject, int opcode, SystemMethodInfo methodInfo) : this(targetObject, opcode, methodInfo, false)
            { }

            public MessageCallSyntaxInfo(CBase targetObject, int opcode, SystemMethodInfo methodInfo, bool automatically) : this(targetObject, opcode, null, methodInfo, automatically)
            { }

            public MessageCallSyntaxInfo(CBase targetObject, SystemType messageType, SystemMethodInfo methodInfo) : this(targetObject, messageType, methodInfo, false)
            { }

            public MessageCallSyntaxInfo(CBase targetObject, SystemType messageType, SystemMethodInfo methodInfo, bool automatically) : this(targetObject, 0, messageType, methodInfo, automatically)
            { }

            private MessageCallSyntaxInfo(CBase targetObject, int opcode, SystemType messageType, SystemMethodInfo methodInfo, bool automatically) : base(targetObject, methodInfo, automatically)
            {
                m_opcode = opcode;
                m_messageType = messageType;
            }

            /// <summary>
            /// 消息回调的调度函数
            /// </summary>
            /// <param name="message">消息对象实例</param>
            public void Invoke(ProtoBuf.Extension.IMessage message)
            {
                if (m_isExtensionType)
                {
                    if (m_isNullParameterType)
                    {
                        m_callback.DynamicInvoke(m_targetObject);
                    }
                    else
                    {
                        m_callback.DynamicInvoke(m_targetObject, message);
                    }
                }
                else
                {
                    if (m_isNullParameterType)
                    {
                        m_callback.DynamicInvoke();
                    }
                    else
                    {
                        m_callback.DynamicInvoke(message);
                    }
                }
            }
        }

        #endregion

        #region 包装类型回调信息类自动绑定数据管理函数接口

        /// <summary>
        /// 处理所有包装类型回调信息数据
        /// </summary>
        /// <typeparam name="RegType">注册类型</typeparam>
        /// <typeparam name="CallInfoType">回调信息类型</typeparam>
        /// <param name="container">回调信息数据容器</param>
        /// <param name="func">操作回调接口</param>
        private void OnAutomaticallyCallSyntaxInfoProcessHandler<RegType, CallInfoType>(IDictionary<RegType, IDictionary<string, CallInfoType>> container,
                                                                                        System.Action<RegType, string> func)
                                                                                        where CallInfoType : BaseCallSyntaxInfo
        {
            IDictionary<RegType, IList<string>> list = new Dictionary<RegType, IList<string>>();

            foreach (KeyValuePair<RegType, IDictionary<string, CallInfoType>> kvp in container)
            {
                IDictionary<string, CallInfoType> callInfos = kvp.Value;
                foreach (KeyValuePair<string, CallInfoType> kvp_info in callInfos)
                {
                    if (kvp_info.Value.Automatically)
                    {
                        if (false == list.TryGetValue(kvp.Key, out IList<string> infos))
                        {
                            infos = new List<string>();
                            list.Add(kvp.Key, infos);
                        }

                        if (infos.Contains(kvp_info.Key))
                        {
                            Debugger.Warn("The call info was already exist with target type '{0}' and name '{1}', repeat added it will override old value.", kvp.Key, kvp_info.Key);
                            infos.Remove(kvp_info.Key);
                        }

                        infos.Add(kvp_info.Key);
                    }
                }
            }

            if (list.Count > 0)
            {
                foreach (KeyValuePair<RegType, IList<string>> kvp in list)
                {
                    IList<string> infos = kvp.Value;
                    for (int n = 0; n < infos.Count; ++n)
                    {
                        func(kvp.Key, infos[n]);
                    }
                }
            }
        }

        #endregion
    }
}
