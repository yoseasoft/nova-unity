/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 会话管理工具类，对项目内部所有会话进行统一管理，该接口在系统中是线程安全的
    /// </summary>
    public sealed partial class Session
    {
        /*
         * 会话模块的全局会话值生成函数
         * 分别提供了两套接口对线程安全及非线程的情形进行支持，您可根据自身的实际需求调用对应的方法
         */

        private static readonly object s_locked = new object();

        /// <summary>
        /// 字符串名称标识类型的会话增量管理容器，用于系统内部运行时批量数据ID自生成
        /// </summary>
        private static System.Collections.Hashtable m_sessionNameMappings = new System.Collections.Hashtable();

        /// <summary>
        /// 数值名称标识类型的会话增量管理容器，用于系统内部运行时批量数据ID自生成
        /// </summary>
        private static System.Collections.Hashtable m_sessionTypeMappings = new System.Collections.Hashtable();

        /// <summary>
        /// 会话增量获取接口，在线程安全的情况下提取会话值
        /// </summary>
        /// <param name="session">会话值</param>
        /// <param name="init">会话初始值</param>
        /// <param name="max">会话最大值</param>
        /// <returns>返回自增后的当前会话值标识</returns>
        private static int __CalcNextSessionID(int session, int init, int max)
        {
            lock (s_locked)
            {
                return __CalcNextUnsafeSessionID(session, init, max);
            }
        }

        /// <summary>
        /// 会话增量获取接口，在非线程安全的情况下提取会话值
        /// </summary>
        /// <param name="session">会话值</param>
        /// <param name="init">会话初始值</param>
        /// <param name="max">会话最大值</param>
        /// <returns>返回自增后的当前会话值标识</returns>
        private static int __CalcNextUnsafeSessionID(int session, int init, int max)
        {
            session++;
            if (session == max || session <= 0)
            {
                session = init + 1;
            }

            return session;
        }

        /// <summary>
        /// 会话增量获取接口，获取特定名称下对应的会话值，在线程安全的情况下提取该值
        /// </summary>
        /// <param name="name">名称标识</param>
        /// <param name="init">会话初始值</param>
        /// <param name="max">会话最大值</param>
        /// <returns>返回自增后的当前会话值标识</returns>
        public static int NextSessionID(string name, int init = Session.SESSION_INIT_VALUE, int max = System.Int32.MaxValue)
        {
            lock (s_locked)
            {
                return NextUnsafeSessionID(name, init, max);
            }
        }

        /// <summary>
        /// 会话增量获取接口，获取特定名称下对应的会话值，在非线程安全的情况下提取该值
        /// </summary>
        /// <param name="name">名称标识</param>
        /// <param name="init">会话初始值</param>
        /// <param name="max">会话最大值</param>
        /// <returns>返回自增后的当前会话值标识</returns>
        public static int NextUnsafeSessionID(string name, int init = Session.SESSION_INIT_VALUE, int max = System.Int32.MaxValue)
        {
            int session = 0;
            if (m_sessionNameMappings.ContainsKey(name))
            {
                session = (int) m_sessionNameMappings[name];
            }
            else
            {
                // 初次获取该名称对应的会话值，使用默认初始变量
                session = init;
            }

            session = __CalcNextUnsafeSessionID(session, init, max);
            m_sessionNameMappings[name] = session;

            return session;
        }

        /// <summary>
        /// 会话增量获取接口，获取特定类型下对应的会话值，在线程安全的情况下提取该值
        /// </summary>
        /// <param name="type">类型标识</param>
        /// <param name="init">会话初始值</param>
        /// <param name="max">会话最大值</param>
        /// <returns>返回自增后的当前会话值标识</returns>
        public static int NextSessionID(int type, int init = Session.SESSION_INIT_VALUE, int max = System.Int32.MaxValue)
        {
            lock (s_locked)
            {
                return NextUnsafeSessionID(type, init, max);
            }
        }

        /// <summary>
        /// 会话增量获取接口，获取特定类型下对应的会话值，在非线程安全的情况下提取该值
        /// </summary>
        /// <param name="type">类型标识</param>
        /// <param name="init">会话初始值</param>
        /// <param name="max">会话最大值</param>
        /// <returns>返回自增后的当前会话值标识</returns>
        public static int NextUnsafeSessionID(int type, int init = Session.SESSION_INIT_VALUE, int max = System.Int32.MaxValue)
        {
            int session = 0;
            if (m_sessionTypeMappings.ContainsKey(type))
            {
                session = (int) m_sessionTypeMappings[type];
            }
            else
            {
                // 初次获取该名称对应的会话值，使用默认初始变量
                session = init;
            }

            session = __CalcNextUnsafeSessionID(session, init, max);
            m_sessionTypeMappings[type] = session;

            return session;
        }

        /// <summary>
        /// 以线程安全的方式重置指定名称对应的会话记录信息
        /// </summary>
        /// <param name="name">名称标识</param>
        public static void ResetSession(string name)
        {
            lock (s_locked)
            {
                ResetUnsafeSession(name);
            }
        }

        /// <summary>
        /// 以非线程安全的方式重置指定名称对应的会话记录信息
        /// </summary>
        /// <param name="name">名称标识</param>
        public static void ResetUnsafeSession(string name)
        {
            if (m_sessionNameMappings.ContainsKey(name))
            {
                m_sessionNameMappings.Remove(name);
            }
        }

        /// <summary>
        /// 以线程安全的方式重置指定类型对应的会话记录信息
        /// </summary>
        /// <param name="type">类型标识</param>
        public static void ResetSession(int type)
        {
            lock (s_locked)
            {
                ResetUnsafeSession(type);
            }
        }

        /// <summary>
        /// 以非线程安全的方式重置指定类型对应的会话记录信息
        /// </summary>
        /// <param name="type">类型标识</param>
        public static void ResetUnsafeSession(int type)
        {
            if (m_sessionTypeMappings.ContainsKey(type))
            {
                m_sessionTypeMappings.Remove(type);
            }
        }

        /// <summary>
        /// 以线程安全的方式清理全部会话信息
        /// </summary>
        public static void CleanupAllSessions()
        {
            lock (s_locked)
            {
                CleanupAllUnsafeSessions();
            }
        }

        /// <summary>
        /// 以非线程安全的方式清理全部会话信息
        /// </summary>
        public static void CleanupAllUnsafeSessions()
        {
            m_sessionNameMappings.Clear();
            m_sessionTypeMappings.Clear();
        }
    }
}
