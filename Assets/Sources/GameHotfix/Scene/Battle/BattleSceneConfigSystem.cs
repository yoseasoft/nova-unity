
using Game.Proto;


/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-17
/// 功能描述：
/// </summary>
namespace Game
{
    /// <summary>
    /// Battle场景配置逻辑处理类
    /// </summary>
    public static class BattleSceneConfigSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        private static void Awake(this BattleScene self)
        {
            Debugger.Log("欢迎进入Battle场景配置业务流程！");

            self.AddComponent<BattleMapComponent>();
            self.AddComponent<BattleLoadComponent>();
            self.AddComponent<BattleCmdComponent>();
            self.AddComponent<BattleListenComponent>();
            self.AddComponent<BattleAnimationComponent>();
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        private static void Start(this BattleScene self)
        {
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        private static void Destroy(this BattleScene self)
        {
            self.RemoveComponent<BattleMapComponent>();
            self.RemoveComponent<BattleLoadComponent>();
            self.RemoveComponent<BattleCmdComponent>();
            self.RemoveComponent<BattleListenComponent>();
            self.RemoveComponent<BattleAnimationComponent>();

            Debugger.Log("当前正在销毁并退出Battle场景配置业务流程！");
        }

        //[GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        //private static void Update(this BattleScene self)
        //{
        //    if (NE.InputHandler.IsAnyKeycodeInputed())
        //    {
        //        if (NE.InputHandler.IsKeycodePressed(UnityEngine.KeyCode.Q))
        //        {
        //            GameTest.OnTest();
        //        }
        //        else if (NE.InputHandler.IsKeycodePressed(UnityEngine.KeyCode.R))
        //        {
        //            GameTest.Redo();
        //        }
        //    }
        //}
    }
}
