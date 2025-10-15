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
    /// Battle场景通知逻辑处理类
    /// </summary>
    public static class BattleSceneNotifySystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        private static void Awake(this BattleScene self)
        {
            Debugger.Log("欢迎进入Battle场景通知业务流程！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        private static void Start(this BattleScene self)
        {
            NE.NetworkHandler.OnSimulationReceiveMessageOfProtoBuf(new Proto.EnterWorldResp() { Code = 1 });
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        private static void Destroy(this BattleScene self)
        {
            Debugger.Log("当前正在销毁并退出Battle场景通知业务流程！");
        }

        [GameEngine.OnMessageDispatchCall(typeof(BattleScene), typeof(Proto.EnterWorldResp))]
        private static void OnRecvEnterWorldMessage(BattleScene self, Proto.EnterWorldResp message)
        {
            if (message.Code == 1)
            {
                self.GetComponent<BattleLoadComponent>().OnSoldierJoin();
            }
            else if (message.Code > 1 && message.Code < 10)
            {
                self.GetComponent<BattleLoadComponent>().OnMonsterJoin(message.Code);
            }
        }
    }
}
