/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemIEnumerator = System.Collections.IEnumerator;

using UnityObject = UnityEngine.Object;
using UnityGameObject = UnityEngine.GameObject;
using UnityComponent = UnityEngine.Component;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityCoroutine = UnityEngine.Coroutine;

namespace NovaEngine
{
    /// <summary>
    /// 基础管理句柄，统一管理程序内部所有的管理器，指令等信息，同时对外提供全部管理器的统一访问接口
    /// </summary>
    public partial class Facade
    {
        /// <summary>
        /// 表现层对象静态实例
        /// </summary>
        private static Facade s_instance = null;

        /// <summary>
        /// 引擎对象实例
        /// </summary>
        private readonly Engine m_engine = null;

        /// <summary>
        /// 表现层所依赖的根节点组件实例
        /// </summary>
        // private readonly UnityGameObject m_rootGameObject = null;

        /// <summary>
        /// 表现层所依赖的MONO对象组件实例
        /// </summary>
        // private readonly UnityMonoBehaviour m_monoBehaviour = null;

        /// <summary>
        /// 表现层对象构造函数
        /// </summary>
        /// <param name="gameObject">根节点组件实例</param>
        protected Facade(Engine engine)
        {
            // 初始化根节点组件实例
            // 该实例一旦初始化后不可更改
            m_engine = engine;
            // m_rootGameObject = engine.MonoBehaviour.gameObject;
            // m_monoBehaviour = engine.MonoBehaviour;
        }

        /// <summary>
        /// 表现层对象析构函数
        /// </summary>
        ~Facade()
        {
            // m_engine = null;
            // m_rootGameObject = null;
            // m_monoBehaviour = null;
        }

        /// <summary>
        /// 对象初始化回调接口，在实例构建成功时调用，子类中可以不处理该接口
        /// </summary>
        /// <returns>默认返回true，若返回值为false，则实例初始化失败</returns>
        protected virtual bool Initialize()
        {
            // 该初始化接口仅可调用一次，若需再次初始化该接口，需将之前的实例销毁掉
            Logger.Assert(null == s_instance);

            // if (null == m_rootGameObject) { Logger.Error("管理器对象的根节点组件实例为空，不可添加组件实例及进行MONO协程调度！"); return false; }

            // 将当前对象赋予表现层静态实例
            s_instance = this;

            // 初始化模块控制器的配置信息
            ModuleController.Config.InitModuleConfigure();

            return true;
        }

        /// <summary>
        /// 对象清理回调接口，在实例销毁之前调用，子类中可以不处理该接口
        /// </summary>
        protected virtual void Cleanup()
        {
        }

        /// <summary>
        /// 该接口并不产生单例对象，仅返回当前的静态实例属性值
        /// </summary>
        /// <returns>返回表现层的静态实例对象</returns>
        public static Facade Instance
        {
            get { return s_instance; }
        }

        /// <summary>
        /// 单例类的实例创建接口
        /// </summary>
        public static Facade Create(Engine engine)
        {
            if (s_instance == null)
            {
                Facade facade = new Facade(engine);
                if (facade.Initialize())
                {
                    s_instance = facade;
                }
            }

            return s_instance;
        }

        /// <summary>
        /// 单例类的实例销毁接口
        /// </summary>
        public static void Destroy()
        {
            if (s_instance != null)
            {
                s_instance.Cleanup();
                s_instance = null;
            }
        }

        /// <summary>
        /// 表现层统一初始启动接口
        /// </summary>
        public virtual void Startup()
        {
            ModuleController.Startup();
        }

        /// <summary>
        /// 表现层统一结束关闭接口
        /// </summary>
        public virtual void Shutdown()
        {
            ModuleController.Shutdown();
        }

        /// <summary>
        /// 表现层统一逻辑更新接口
        /// </summary>
        public virtual void Update()
        {
            // 时间戳刷新
            Timestamp.RefreshTimeOnUpdate();

            // 模块刷新
            ModuleController.Update();
        }

        /// <summary>
        /// 表现层统一后置更新接口
        /// </summary>
        public virtual void LateUpdate()
        {
            // 模块后置刷新
            ModuleController.LateUpdate();
        }

        /// <summary>
        /// 表现层统一临时资源清理接口
        /// </summary>
        public virtual void Dump()
        {
        }

        /// <summary>
        /// 表现层统一回收清理接口
        /// </summary>
        public void Collect()
        {
            System.GC.Collect();
        }

        #region 消息指令与模块对象注册/转发接口

        /// <summary>
        /// 发送模块指令，由当前系统控制器立即执行处理
        /// </summary>
        /// <param name="id">事件标识</param>
        /// <param name="type">事件类型</param>
        public void SendModuleCommand(int id, int type)
        {
            ModuleController.CallCommand(id, type);
        }

        /// <summary>
        /// 发送模块指令，由当前系统控制器立即执行处理
        /// </summary>
        /// <param name="args">指令参数实例</param>
        public void SendModuleCommand(ModuleCommandArgs args)
        {
            ModuleController.CallCommand(args);
        }

        /// <summary>
        /// 通过当前表现层管理容器中指定类型名获取对应的模块实例对象，该实例对象是继承于<see cref="NovaEngine.ModuleObject"/>实现类
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>返回该名称对应的模块对象实例</returns>
        public T GetModule<T>() where T : ModuleObject
        {
            return ModuleController.GetModule<T>();
        }

        #endregion

        #region 线程/协程组件调用操作接口

        /// <summary>
        /// 执行协程调度接口
        /// </summary>
        /// <param name="coroutine">协程对象实例</param>
        public void DoWork(ICoroutinable coroutine)
        {
            Utility.Thread.DoWork(coroutine);
        }

        /// <summary>
        /// 执行协程调度接口
        /// </summary>
        /// <param name="routine">协程对象实例</param>
        /// <returns>协程调度返回信息</returns>
        public UnityCoroutine StartCoroutine(SystemIEnumerator routine)
        {
            UnityMonoBehaviour controller = GetRootController();
            if (null != controller)
            {
                return controller.StartCoroutine(routine);
            }

            return null;
        }

        /// <summary>
        /// 执行线程调度接口
        /// </summary>
        /// <param name="runnable">线程对象实例</param>
        public void DoRun(IRunnable runnable)
        {
            Utility.Thread.DoRun(runnable);
        }

        #endregion
    }
}
