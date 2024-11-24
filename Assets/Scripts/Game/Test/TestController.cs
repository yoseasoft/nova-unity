/// <summary>
/// 2024-05-17 Game Framework Code By Hurley
/// </summary>

using System.Collections.Generic;

using SystemType = System.Type;

namespace Game
{
    /// <summary>
    /// 测试模块控制器
    /// </summary>
    public static class TestController
    {
        private static IDictionary<string, ITestCase> s_testCases = new Dictionary<string, ITestCase>();

        private static IDictionary<string, bool> s_testCaseEnabledStatus = new Dictionary<string, bool>();

        private static IList<ITestCase> s_testCaseActivationList = new List<ITestCase>();

        public static void Startup()
        {
            IList<SystemType> list = GameEngine.Loader.CodeLoader.FindClassTypesByFilterCondition(delegate (SystemType v)
            {
                if (typeof(ITestCase).IsAssignableFrom(v) && NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(v))
                {
                    return true;
                }

                return false;
            });

            for (int n = 0; null != list && n < list.Count; ++n)
            {
                SystemType classType = list[n];
                ITestCase testCase = System.Activator.CreateInstance(classType) as ITestCase;

                s_testCases.Add(testCase.CaseName, testCase);

                if ((s_testCaseEnabledStatus.ContainsKey(testCase.CaseName) && s_testCaseEnabledStatus[testCase.CaseName]) ||
                    (s_testCaseEnabledStatus.ContainsKey(classType.Name) && s_testCaseEnabledStatus[classType.Name]))
                {
                    s_testCaseActivationList.Add(testCase);
                }
            }

            for (int n = 0; n < s_testCaseActivationList.Count; ++n)
            {
                ITestCase testCase = s_testCaseActivationList[n];
                testCase.Startup();
            }
        }

        public static void Shutdown()
        {
            for (int n = 0; n < s_testCaseActivationList.Count; ++n)
            {
                ITestCase testCase = s_testCaseActivationList[n];
                testCase.Shutdown();
            }

            s_testCaseActivationList.Clear();
            s_testCaseActivationList = null;

            s_testCases.Clear();
            s_testCases = null;
        }

        public static void Update()
        {
            for (int n = 0; n < s_testCaseActivationList.Count; ++n)
            {
                ITestCase testCase = s_testCaseActivationList[n];
                testCase.Update();
            }
        }

        public static void SetTargetCaseEnabled<T>()
        {
            SetTargetCaseEnabled(typeof(T));
        }

        public static void SetTargetCaseEnabled(SystemType targetType)
        {
            s_testCaseEnabledStatus.Add(targetType.Name, true);
        }

        public static void SetTargetCaseEnabled(string caseName)
        {
            s_testCaseEnabledStatus.Add(caseName, true);
        }
    }
}
