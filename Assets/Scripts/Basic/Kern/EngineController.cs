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
using System.Reflection;

namespace GameEngine
{
    /// <summary>
    /// 应用程序核心引擎的控制器脚本类，用于挂载到场景节点中来接收<see cref="UnityEngine.MonoBehaviour"/>的通知
    /// 2024-04-11 修正：
    /// 取消控制器直接继承<see cref="UnityEngine.MonoBehaviour"/>的方式，改为使用回调接口绑定的方式处理Unity脚本的生命周期相关接口
    /// 因此无需再声明 [UnityEngine.DisallowMultipleComponent] 标签
    /// </summary>
    public static class EngineController
    {
        /// <summary>
        /// 启动引擎的外部调用接口函数
        /// </summary>
        /// <param name="controller">控制器对象实例</param>
        /// <param name="variables">环境参数</param>
        public static void Start(object controller, IDictionary<string, string> variables)
        {
            // 初始化回调接口
            InitControllerCallback(controller);

            // 装载引擎
            EngineLauncher.OnCreate(controller, variables);
        }

        /// <summary>
        /// 重载引擎的外部调用接口函数
        /// </summary>
        /// <param name="controller">控制器对象实例</param>
        public static void Reload(object controller)
        {
            // 重载引擎
            EngineLauncher.OnReload();
        }

        /// <summary>
        /// 引擎的回调接口绑定处理函数
        /// </summary>
        /// <param name="controller">目标脚本对象</param>
        private static void InitControllerCallback(object controller)
        {
            FieldInfo[] fieldInfos = controller.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            System.Type launcherType = typeof(EngineLauncher);
            for (int n = 0; n < fieldInfos.Length; ++n)
            {
                FieldInfo fieldInfo = fieldInfos[n];
                if (NovaEngine.Utility.Reflection.IsTypeOfAction(fieldInfo.FieldType))
                {
                    string fieldName = fieldInfo.Name;
                    if (char.IsLower(fieldName[0]))
                    {
                        fieldName = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
                    }

                    MethodInfo methodInfo = launcherType.GetMethod(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (null == methodInfo)
                    {
                        Debugger.Warn("Could not found any launcher method with name '{0}', initialized controller callback property '{1}' failed.", fieldName, fieldInfo.Name);
                        continue;
                    }

                    System.Delegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(methodInfo);
                    if (null == callback)
                    {
                        Debugger.Warn("Cannot generic action delegate with target method '{0}', initialized controller callback property '{1}' failed.", methodInfo.Name, fieldInfo.Name);
                        continue;
                    }

                    Debugger.Info(LogGroupTag.Basic, "Initialized controler property '{0}' to target method '{1}'.", fieldInfo.Name, methodInfo.Name);
                    fieldInfo.SetValue(controller, callback);
                }
            }
        }
    }
}
