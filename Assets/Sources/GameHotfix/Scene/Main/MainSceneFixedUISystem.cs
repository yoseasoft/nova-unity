/// <summary>
/// 2024-12-22 Game Framework Code By Hurley
/// </summary>

using Cysharp.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// 游戏主场景固定UI逻辑类
    /// </summary>
    static class MainSceneFixedUISystem
    {
        [OnAwake]
        static void Awake(this MainScene self)
        {
            Debugger.Log("欢迎进入Main场景固定UI业务流程！");
        }

        [OnStart]
        static void Start(this MainScene self)
        {
            CreateMainUI().Forget();
        }

        [OnDestroy]
        static void Destroy(this MainScene self)
        {
            NE.GuiHandler.CloseAllUI();

            Debugger.Log("当前正在销毁并退出Main场景固定UI业务流程！");
        }

        /// <summary>
        /// 创建主场景相关UI界面
        /// </summary>
        static async UniTaskVoid CreateMainUI()
        {
            await UniTask.Delay(1);
            await NE.GuiHandler.OpenUI("LoginPanel");
        }
    }
}
