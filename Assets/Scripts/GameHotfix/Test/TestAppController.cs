/// <summary>
/// 2024-05-11 Game Framework Code By Hurley
/// </summary>

using System;
using System.Reflection;

namespace Game
{
    /// <summary>
    /// 测试程序控制器操作
    /// </summary>
    public class TestAppController : ITestCase
    {
        public void Startup()
        {
            UnityEngine.MonoBehaviour controller = NovaEngine.AppEntry.RootController;
            FieldInfo[] fieldInfos = controller.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            System.Type launcherType = typeof(GameEngine.EngineLauncher);
            Debugger.Info("---------------------------------------------------------------------");
            for (int n = 0; n < fieldInfos.Length; ++n)
            {
                FieldInfo fieldInfo = fieldInfos[n];
                if (NovaEngine.Utility.Reflection.IsTypeOfAction(fieldInfo.FieldType))
                {
                    Debugger.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
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

                    Delegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(methodInfo);
                    if (null == callback)
                    {
                        Debugger.Warn("Cannot generic action delegate with target method '{0}', initialized controller callback property '{1}' failed.", methodInfo.Name, fieldInfo.Name);
                        continue;
                    }

                    Debugger.Info("Initialized controler property '{0}' to target method '{1}'.", fieldInfo.Name, methodInfo.Name);
                    fieldInfo.SetValue(controller, callback);
                }
            }

            Debugger.Warn("==================================================> TestAppController end <==================================================");
        }

        public void Shutdown()
        {
        }

        public void Update()
        {
        }
    }
}
