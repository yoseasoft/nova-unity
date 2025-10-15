
using GameEngine;


/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-20
/// 功能描述：
/// </summary>
namespace Game
{
    /// <summary>
    /// LOGO场景逻辑处理类
    /// </summary>
    public static class LogoSceneExtendCallSystem
    {
        /// <summary>
        /// 测试
        /// </summary>
        static void LogoTest(this LogoScene self)
        {
            Debugger.Info("调用了 LogoTest 接口！");
        }

        /// <summary>
        /// 创建主场景相关UI界面
        /// </summary>
        [OnAspectExtendCall("LogoTest")]
        static void BeforeTest(this LogoScene self)
        {
            Debugger.Info("调用了 BeforeTest 接口！");
        }

        static void AfterTest(this LogoScene self)
        {
            Debugger.Info("调用了 AfterTest 接口！");
        }
    }
}

