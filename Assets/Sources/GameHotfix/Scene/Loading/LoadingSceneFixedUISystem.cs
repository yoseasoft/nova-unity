/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-16
/// 功能描述：
/// </summary>

using Cysharp.Threading.Tasks;

using GameEngine;

namespace Game
{
    /// <summary>
    /// Loading场景固定UI逻辑处理类
    /// </summary>
    public static class LoadingSceneFixedUISystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        private static void Awake(this LoadingScene self)
        {
            Debugger.Log("欢迎进入Loading场景固定UI业务流程！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        private static void Start(this LoadingScene self)
        {
            CreateLoadingPanel().Forget();
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        private static void Destroy(this LoadingScene self)
        {
            NE.GuiHandler.CloseAllUI();

            Debugger.Log("当前正在销毁并退出Loading场景固定UI业务流程！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        private static void Update(this LoadingScene self)
        {
        }

        /// <summary>
        /// 创建Loading界面
        /// </summary>
        static async UniTaskVoid CreateLoadingPanel()
        {
            LoadingPanel loadingPanel = (LoadingPanel) await GuiHandler.Instance.OpenUI("LoadingPanel");
            loadingPanel?.StartProgress(0);
        }
    }
}
