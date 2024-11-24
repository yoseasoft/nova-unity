/// <summary>
/// 2024-03-14 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    public struct TestLogoEventDataInfo
    {
        public int logoID;
        public string logoName;
        public int logoType;
    }

    /// <summary>
    /// LOGO事件服务类，用于进行接口测试
    /// </summary>
    [GameEngine.EventSystem]
    public static class LogoEventService
    {
        [GameEngine.OnEventDispatchCall(1001)]
        private static void OnLogoEvent1(int eventID, params object[] args)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendFormat("测试针对事件标识'{0}'的全局带参静态函数'OnLogoEvent1'调用, 参数值:", eventID);
            for (int n = 0; null != args && n < args.Length; ++n)
            {
                sb.AppendFormat("[{0}] = {1}, ", n, args[n].ToString());
            }
            Debugger.Log(sb.ToString());
        }

        [GameEngine.OnEventDispatchCall(1001)]
        private static void OnLogoEvent1WithNullParameter()
        {
            Debugger.Log("测试针对事件标识'1001'的全局无参静态函数'OnLogoEvent1WithNullParameter'调用.");
        }

        [GameEngine.OnEventDispatchCall(typeof(TestLogoEventDataInfo))]
        private static void OnLogoEvent2(TestLogoEventDataInfo dataInfo)
        {
            Debugger.Log("测试针对事件类型'{0}'的全局带参静态函数'OnLogoEvent2'调用, 参数值:'{1}, {2}, {3}'.",
                    dataInfo.GetType().FullName, dataInfo.logoID, dataInfo.logoName, dataInfo.logoType);
        }

        [GameEngine.OnEventDispatchCall(typeof(TestLogoEventDataInfo))]
        private static void OnLogoEvent2WithNullParameter()
        {
            Debugger.Log("测试针对事件类型'TestLogoEventDataInfo'的全局无参静态函数'OnLogoEvent2WithNullParameter'调用.");
        }
    }
}
