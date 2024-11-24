/// <summary>
/// 2024-05-29 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// Battle场景逻辑处理类
    /// </summary>
    [GameEngine.Aspect]
    public static class BattleSceneSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(BattleScene), GameEngine.AspectBehaviourType.Awake)]
        public static void Awake(this BattleScene self)
        {
            Debugger.Log("欢迎进入战斗场景！");
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(BattleScene), GameEngine.AspectBehaviourType.Start)]
        public static void Start(this BattleScene self)
        {
            // WorldSimulator.LoadWorld(m_engine);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(BattleScene), GameEngine.AspectBehaviourType.Destroy)]
        public static void Destroy(this BattleScene self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(BattleScene), GameEngine.AspectBehaviourType.Update)]
        public static void Update(this BattleScene self)
        {
            if (NE.InputHandler.IsAnyKeycodeInputed())
            {
                if (NE.InputHandler.IsKeycodePressed(UnityEngine.KeyCode.Q))
                {
                    GameTest.OnTest();
                }
                else if (NE.InputHandler.IsKeycodePressed(UnityEngine.KeyCode.R))
                {
                    GameTest.Redo();
                }
            }
        }
    }
}
