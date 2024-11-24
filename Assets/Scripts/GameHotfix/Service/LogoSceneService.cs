/// <summary>
/// 2024-03-04 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// LOGO场景服务类，用于进行接口测试
    /// </summary>
    [GameEngine.Aspect]
    [GameEngine.EventSystem]
    public static class LogoSceneService
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(LogoScene), "Enter")]
        private static void LogoEnter(LogoScene scene)
        {
            Debugger.Log("Exec LogoEnter from game service ...");

            // GameEngine.GeneralCodeInfo gci = GameEngine.CodeLoader.LookupGeneralCodeInfo(typeof(Game.Proto.C2G_TestReq));
            // GameEngine.NetworkMessageCodeInfo nmci = gci == null ? null : gci as GameEngine.NetworkMessageCodeInfo;
            // Debugger.Info("print network message code info '{0}' from 'Game.Proto.C2G_TestReq' class.", nmci == null ? "unknown" : nmci.ToString());
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(LogoScene), "Exit")]
        private static void LogoExit(LogoScene scene)
        {
            Debugger.Log("Exec LogoExit from game service ...");

            GameEngine.AspectBehaviourType bt = GameEngine.AspectBehaviourType.Initialize;
            GameEngine.AspectBehaviourType bt2 = GameEngine.AspectBehaviourType.Initialize | GameEngine.AspectBehaviourType.Update | GameEngine.AspectBehaviourType.LateUpdate;
            Debugger.Log(" --------------------------- {0}, {1}, {2} ---------------------------", bt.GetType().FullName, bt.ToString(), NovaEngine.Utility.Convertion.IsCorrectedEnumValue<GameEngine.AspectBehaviourType>((int) bt));
            Debugger.Log(" --------------------------- {0}, {1}, {2} ---------------------------", bt.GetType().FullName, bt.ToString(), NovaEngine.Utility.Convertion.IsCorrectedEnumValue<GameEngine.AspectBehaviourType>(101));
            Debugger.Log(" --------------------------- {0}, {1} ---------------------------", bt2.GetType().FullName, bt2.ToString());
        }

        [GameEngine.OnAspectExtendCallOfTarget(typeof(LogoScene), "Test")]
        private static void LogoTest(LogoScene scene)
        {
            Debugger.Log("Exec LogoTest from game service ...");

            //LogoTestMultiParams(101, "hello", scene, 502, "yukie");
            //Debugger.Log("-------------------------------------------------------------");
            //LogoTestMultiParams(202, "who", NE.NetworkHandler, NE.SceneHandler, "111", scene);

            /*
            System.Reflection.MethodInfo methodInfo = typeof(LogoSceneService).GetMethod("LogoTestMultiParams", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Reflection.ParameterInfo[] paramInfos = methodInfo.GetParameters();
            for (int n = 0; n < paramInfos.Length; ++n)
            {
                Debugger.Log("method '{0}' param[{1}] type = '{2}'", methodInfo.Name, n, paramInfos[n].ParameterType);
            }
            */
        }

        [GameEngine.OnEventDispatchCall(typeof(LogoScene), 1001)]
        private static void OnLogoTestEvent(LogoScene scene, int eventID, params object[] args)
        {
            Debugger.Log("测试场景对象'{0}'监听事件标识'{1}'的带参全局静态函数'OnLogoTestEvent'调用.", scene.GetType().FullName, eventID);

            //GameEngine.GeneralCodeInfo gci = GameEngine.CodeLoader.LookupGeneralCodeInfo(typeof(Game.Proto.CheckVersionReq));
            //GameEngine.NetworkMessageCodeInfo nmci = gci == null ? null : gci as GameEngine.NetworkMessageCodeInfo;
            //Debugger.Info("print network message code info '{0}' from 'Game.Proto.CheckVersionReq' class.", nmci == null ? "unknown" : nmci.ToString());
        }

        [GameEngine.OnEventDispatchCall(typeof(LogoScene), 1001)]
        private static void OnLogoTestEventWithNullParameter(LogoScene scene)
        {
            Debugger.Log("测试场景对象'{0}'监听事件标识'1001'的无参全局静态函数'OnLogoTestEventWithNullParameter'调用.", scene.GetType().FullName);

            //GameEngine.GeneralCodeInfo gci = GameEngine.CodeLoader.LookupGeneralCodeInfo(typeof(Game.Proto.CheckVersionReq));
            //GameEngine.NetworkMessageCodeInfo nmci = gci == null ? null : gci as GameEngine.NetworkMessageCodeInfo;
            //Debugger.Info("print network message code info '{0}' from 'Game.Proto.CheckVersionReq' class.", nmci == null ? "unknown" : nmci.ToString());
        }

        /*
        private class TestMultiParamsInfo
        {
            public int _id;
            public object[] _params;
        }

        private static void LogoTestMultiParams(int id, params object[] args)
        {
            TestMultiParamsInfo info = new TestMultiParamsInfo();
            info._id = id;
            info._params = args;

            Debugger.Log(" ??????????????????????????????????????????????????? ");
            PrintTestMultiParams(info._id, info._params);
        }

        private static void PrintTestMultiParams(int id, params object[] args)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("id = {0}, ", id);
            sb.Append("args = { ");
            for (int n = 0; n < args.Length; ++n)
            {
                sb.AppendFormat("[{0}] = {1}, ", n, args[n].ToString());
            }
            sb.Append(" }");
            Debugger.Log("PrintTestMultiParams = '{0}'", sb.ToString());
        }
        */
    }
}
