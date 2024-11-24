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
using SystemPath = System.IO.Path;

using Cysharp.Threading.Tasks;

namespace GameEngine
{
    /// <summary>
    /// 场景模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.SceneModule"/>类
    /// </summary>
    public sealed partial class SceneHandler : EcsmHandler
    {
        /// <summary>
        /// 句柄对象锁实例
        /// </summary>
        private readonly object m_lock = new object();

        /// <summary>
        /// 场景对象类型映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, SystemType> m_sceneClassTypes;
        /// <summary>
        /// 场景功能类型映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, int> m_sceneFunctionTypes;

        /// <summary>
        /// 当前运行的场景对象类型
        /// </summary>
        private SystemType m_currentSceneType;
        /// <summary>
        /// 当前待命的场景对象类型
        /// </summary>
        private SystemType m_waitingSceneType;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static SceneHandler Instance => HandlerManagement.SceneHandler;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        public SceneHandler()
        {
            // 初始化场景类注册容器
            m_sceneClassTypes = new Dictionary<string, SystemType>();
            m_sceneFunctionTypes = new Dictionary<string, int>();
        }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~SceneHandler()
        {
            // 清理场景类注册容器
            m_sceneClassTypes.Clear();
            m_sceneFunctionTypes.Clear();
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
            CScene scene = GetCurrentScene();
            if (null != scene)
            {
                CallEntityDestroyProcess(scene);
                Call(scene.Shutdown);
                RemoveEntity(scene);

                // 回收场景实例
                ReleaseInstance(scene);

                m_currentSceneType = null;
            }

            // 清理场景类型注册列表
            UnregisterAllSceneClasses();

            base.OnCleanup();
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
            if (null != m_waitingSceneType)
            {
                lock (m_lock)
                {
                    ChangeScene(m_waitingSceneType);
                }
            }

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

        /// <summary>
        /// 获取当前运行的场景实例
        /// </summary>
        /// <returns>返回当前运行的场景实例，若没有则返回null</returns>
        public CScene GetCurrentScene()
        {
            if (Entities.Count > 0)
            {
                if (Entities.Count > 1)
                {
                    Debugger.Error("There can only be one valid scene instance in the containers, don't multiple insert.");
                }

                return Entities[0] as CScene;
            }

            return null;
        }

        /// <summary>
        /// 替换管理器中当前的场景实例
        /// 注意，替换操作并非立即执行，而是在下一次更新前进行替换
        /// 若在此之前多次进行替换操作，将只保留最后一次作为最终的场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void ReplaceScene(string sceneName)
        {
            SystemType sceneType;
            if (m_sceneClassTypes.TryGetValue(sceneName, out sceneType))
            {
                ReplaceScene(sceneType);
            }
        }

        /// <summary>
        /// 替换管理器中当前的场景实例
        /// 注意，替换操作并非立即执行，而是在下一次更新前进行替换
        /// 若在此之前多次进行替换操作，将只保留最后一次作为最终的场景
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        public void ReplaceScene<T>() where T : CScene
        {
            SystemType sceneType = typeof(T);
            ReplaceScene(sceneType);
        }

        /// <summary>
        /// 替换管理器中当前的场景实例
        /// 注意，替换操作并非立即执行，而是在下一次更新前进行替换
        /// 若在此之前多次进行替换操作，将只保留最后一次作为最终的场景
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        public void ReplaceScene(SystemType sceneType)
        {
            Debugger.Assert(null != sceneType, "Invalid arguments.");
            if (sceneType == m_currentSceneType)
            {
                Debugger.Warn("The replace scene '{0}' must be not equals to current scene, replaced it failed.", sceneType.FullName);
                return;
            }

            if (false == m_sceneClassTypes.Values.Contains(sceneType))
            {
                Debugger.Error("Could not found any correct scene class with target type '{0}', replaced scene failed.", sceneType.FullName);
                return;
            }

            if (sceneType == m_waitingSceneType)
            {
                Debugger.Warn("The target scene '{0}' was already in a waiting state, repeat setted it failed.", sceneType.FullName);
                return;
            }

            m_waitingSceneType = sceneType;
        }

        /// <summary>
        /// 通过指定的场景类型动态创建一个对应的场景对象实例
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        private CScene CreateScene(SystemType sceneType)
        {
            if (false == m_sceneClassTypes.Values.Contains(sceneType))
            {
                Debugger.Error("Unknown scene type '{0}', create the scene instance failed.", sceneType.FullName);
                return null;
            }

            return CreateInstance(sceneType) as CScene;
        }

        /// <summary>
        /// 将当前场景切换到指定名称的场景实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>返回改变的目标场景实例，否则切换场景失败返回null</returns>
        public CScene ChangeScene(string sceneName)
        {
            SystemType sceneType;
            if (m_sceneClassTypes.TryGetValue(sceneName, out sceneType))
            {
                return ChangeScene(sceneType);
            }

            return null;
        }

        /// <summary>
        /// 将当前场景切换到指定类型的场景实例
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        /// <returns>返回改变的目标场景实例，否则切换场景失败返回null</returns>
        public T ChangeScene<T>() where T : CScene
        {
            SystemType sceneType = typeof(T);
            return ChangeScene(sceneType) as T;
        }

        /// <summary>
        /// 将当前场景切换到指定类型的场景实例
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <returns>返回改变的目标场景实例，否则切换场景失败返回null</returns>
        public CScene ChangeScene(SystemType sceneType)
        {
            CScene scene = GetCurrentScene();
            if (null != scene)
            {
                // 相同的场景无需切换
                if (scene.GetType() == sceneType)
                {
                    return null;
                }

                SceneStatModule.CallStatAction(SceneStatModule.ON_SCENE_EXIT_CALL, scene);

                CallEntityDestroyProcess(scene);
                Call(scene.Shutdown);
                RemoveEntity(scene);

                // 回收场景实例
                ReleaseInstance(scene);

                m_currentSceneType = null;
            }

            scene = CreateScene(sceneType);
            if (null == scene || false == AddEntity(scene))
            {
                Debugger.Error("Create or register the scene instance '{0}' failed.", sceneType.FullName);
                return null;
            }

            m_currentSceneType = sceneType;
            // 每次替换场景后，都将待命的场景重置掉
            m_waitingSceneType = null;

            // 设置当前场景后再启动场景
            Call(scene.Startup);

            // 唤醒场景对象实例
            CallEntityAwakeProcess(scene);

            SceneStatModule.CallStatAction(SceneStatModule.ON_SCENE_ENTER_CALL, scene);

            return scene;
        }

        /// <summary>
        /// 加载指定名称及路径的场景资源对象实例
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
        /// <param name="assetUrl">场景资源路径</param>
        public AssetModule.Scene LoadSceneAsset(string assetName, string assetUrl, System.Action<AssetModule.Scene> completed = null)
        {
            return SceneModule.LoadScene(assetName, assetUrl, completed);
        }

        /// <summary>
        /// 异步加载场景资源
        /// </summary>
        public async UniTask<AssetModule.Scene> LoadSceneAsync(string assetUrl)
        {
            string sceneName = SystemPath.GetFileNameWithoutExtension(assetUrl);
            return await SceneModule.LoadScene(sceneName, assetUrl).Task;
        }

        /// <summary>
        /// 卸载指定名称的场景资源对象实例
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
        public void UnloadSceneAsset(string assetName)
        {
            SceneModule.UnloadScene(assetName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 通过指定的场景类型获取对应场景的名称
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        /// <returns>返回对应场景的名称，若场景不存在则返回null</returns>
        internal string GetSceneNameForType<T>() where T : CScene
        {
            return GetSceneNameForType(typeof(T));
        }

        /// <summary>
        /// 通过指定的场景类型获取对应场景的名称
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <returns>返回对应场景的名称，若场景不存在则返回null</returns>
        internal string GetSceneNameForType(SystemType sceneType)
        {
            foreach (KeyValuePair<string, SystemType> pair in m_sceneClassTypes)
            {
                if (pair.Value == sceneType)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        #region 场景对象类注册接口函数

        /// <summary>
        /// 注册指定的场景名称及对应的对象类到当前的句柄管理容器中
        /// 注意，注册的对象类必须继承自<see cref="GameEngine.CScene"/>，否则无法正常注册
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="clsType">场景类型</param>
        /// <param name="funcType">功能类型</param>
        /// <returns>若场景类型注册成功则返回true，否则返回false</returns>
        private bool RegisterSceneClass(string sceneName, SystemType clsType, int funcType)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(sceneName) && null != clsType, "Invalid arguments");

            if (false == typeof(CScene).IsAssignableFrom(clsType))
            {
                Debugger.Warn("The register type '{0}' must be inherited from 'CScene'.", clsType.Name);
                return false;
            }

            if (m_sceneClassTypes.ContainsKey(sceneName))
            {
                Debugger.Warn("The scene name '{0}' was already registed, repeat add will be override old name.", sceneName);
                m_sceneClassTypes.Remove(sceneName);
            }

            m_sceneClassTypes.Add(sceneName, clsType);
            if (funcType > 0)
            {
                m_sceneFunctionTypes.Add(sceneName, funcType);
            }

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有场景类型
        /// </summary>
        private void UnregisterAllSceneClasses()
        {
            m_sceneClassTypes.Clear();
            m_sceneFunctionTypes.Clear();
        }

        #endregion
    }
}
