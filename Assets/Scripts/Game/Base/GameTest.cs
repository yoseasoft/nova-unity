/// <summary>
/// 2023-09-07 Game Framework Code By Hurley
/// </summary>

using System.Reflection;

namespace Game
{
    /// <summary>
    /// 程序测试用例管理封装对象类，对业务流程的测试用例提供运行环境
    /// </summary>
    public static class GameTest
    {
        private static bool _isClosing = false;
        private static bool _isRunning = false;

        /// <summary>
        /// 数据加载处理函数
        /// </summary>
        public static void OnTest()
        {
            if (_isClosing)
            {
                Debugger.Warn("测试用例已经关闭，需要手动开启后才可以再次运行！");
                return;
            }

            if (_isRunning)
            {
                Debugger.Warn("测试用例正在运行中，请勿重复操作！");
                return;
            }

            _isClosing = true;
            _isRunning = true;
            System.Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();

            for (int n = 0; n < allTypes.Length; ++n)
            {
                System.Type type = allTypes[n];

                NovaEngine.TestingClassAttribute cls_attribute = type.GetCustomAttribute<NovaEngine.TestingClassAttribute>(false);
                if (null == cls_attribute)
                {
                    continue;
                }

                MethodInfo[] methods = type.GetMethods();

                for (int m = 0; m < methods.Length; ++m)
                {
                    MethodInfo method = methods[m];
                    NovaEngine.StaticTestingCaseAttribute method_attribute = method.GetCustomAttribute<NovaEngine.StaticTestingCaseAttribute>(false);
                    if (method_attribute != null)
                    {
                        Debugger.Log("调用测试类‘{0}({1})’的测试函数‘{2}’：", type.Name, cls_attribute.Level, method.Name);
                        method.Invoke(null, null);
                    }
                }
            }

            _isRunning = false;
        }

        public static void Redo()
        {
            Debugger.Warn("重新恢复测试用例的可运行状态，请使用测试入口顺序进行用例测试！");
            _isClosing = false;
        }
    }
}
