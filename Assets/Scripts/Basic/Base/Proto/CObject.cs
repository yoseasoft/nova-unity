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

using UnityGameObject = UnityEngine.GameObject;

namespace GameEngine
{
    /// <summary>
    /// 节点对象抽象类，对场景中的节点对象上下文进行封装及调度管理
    /// </summary>
    public abstract class CObject : CEntity
    {
        /// <summary>
        /// 获取对象句柄对象
        /// </summary>
        public static ObjectHandler ObjectHandler => ObjectHandler.Instance;

        /// <summary>
        /// 获取节点对象的名称
        /// </summary>
        public override sealed string Name => ObjectHandler.GetObjectNameForType(GetType());

        /// <summary>
        /// 对象初始化通知接口函数
        /// </summary>
        public override sealed void Initialize()
        {
            base.Initialize();

            OnInitialize();
        }

        /// <summary>
        /// 对象内部初始化通知接口函数
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// 对象清理通知接口函数
        /// </summary>
        public override sealed void Cleanup()
        {
            OnCleanup();

            base.Cleanup();
        }

        /// <summary>
        /// 对象内部清理通知接口函数
        /// </summary>
        protected virtual void OnCleanup() { }

        /// <summary>
        /// 对象启动通知接口函数
        /// </summary>
        public override sealed void Startup()
        {
            base.Startup();

            OnStartup();
        }

        /// <summary>
        /// 对象内部启动通知接口函数
        /// </summary>
        protected virtual void OnStartup() { }

        /// <summary>
        /// 对象关闭通知接口函数
        /// </summary>
        public override sealed void Shutdown()
        {
            OnShutdown();

            base.Shutdown();
        }

        /// <summary>
        /// 对象内部关闭通知接口函数
        /// </summary>
        protected virtual void OnShutdown() { }

        /// <summary>
        /// 对象刷新通知接口函数
        /// </summary>
        public override sealed void Update()
        {
            base.Update();

            OnUpdate();
        }

        /// <summary>
        /// 对象内部刷新通知接口函数
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// 对象后置刷新通知接口函数
        /// </summary>
        public override sealed void LateUpdate()
        {
            base.LateUpdate();

            OnLateUpdate();
        }

        /// <summary>
        /// 对象内部后置刷新通知接口函数
        /// </summary>
        protected virtual void OnLateUpdate() { }

        /// <summary>
        /// 对象唤醒通知函数接口
        /// </summary>
        public override sealed void Awake()
        {
            base.Awake();

            OnAwake();
        }

        /// <summary>
        /// 对象内部唤醒通知函数接口
        /// </summary>
        protected virtual void OnAwake()
        { }

        /// <summary>
        /// 对象开始通知函数接口
        /// </summary>
        public override sealed void Start()
        {
            base.Start();

            OnStart();
        }

        /// <summary>
        /// 对象内部开始通知函数接口
        /// </summary>
        protected virtual void OnStart()
        { }

        /// <summary>
        /// 对象销毁通知函数接口
        /// </summary>
        public override sealed void Destroy()
        {
            OnDestroy();

            base.Destroy();
        }

        /// <summary>
        /// 对象内部销毁通知函数接口
        /// </summary>
        protected virtual void OnDestroy()
        { }

        /// <summary>
        /// 同步实例化对象
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        public UnityGameObject Instantiate(string url)
        {
            return ObjectHandler.Instantiate(url);
        }
    }
}
