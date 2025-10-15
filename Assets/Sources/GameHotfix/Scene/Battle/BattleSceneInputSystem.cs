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
    /// Battle场景输入逻辑处理类
    /// </summary>
    public static class BattleSceneInputSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        private static void Awake(this BattleScene self)
        {
            Debugger.Log("欢迎进入Battle场景输入业务流程！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        private static void Start(this BattleScene self)
        {
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        private static void Destroy(this BattleScene self)
        {
            Debugger.Log("当前正在销毁并退出Battle场景输入业务流程！");
        }

        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.A, GameEngine.InputOperationType.Released)]
        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.S, GameEngine.InputOperationType.Released)]
        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.D, GameEngine.InputOperationType.Released)]
        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.W, GameEngine.InputOperationType.Released)]
        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.K, GameEngine.InputOperationType.Released)]
        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.C, GameEngine.InputOperationType.Released)]
        private static void OnBattleSceneInputed(int keycode, int operationType)
        {
            BattleScene scene = NE.SceneHandler.GetCurrentScene() as BattleScene;
            if (null == scene) return;

            BattleLoadComponent loadComponent = scene.GetComponent<BattleLoadComponent>();

            // 增加怪物
            if ((int) UnityEngine.KeyCode.C == keycode)
            {
                int code = NovaEngine.Utility.Random.Next(2) + 2; // 获取 2~3 的随机值
                NE.NetworkHandler.OnSimulationReceiveMessageOfProtoBuf(new Proto.EnterWorldResp() { Code = code });
            }
            else if ((int) UnityEngine.KeyCode.A == keycode)
            {
                int uid = loadComponent.player.GetComponent<IdentityComponent>().objectId;
                GameEngine.GameApi.Send(new BattleActorCmdNotify() { uid = uid, cmd = CmdType.MoveLeft });
            }
            else if ((int) UnityEngine.KeyCode.S == keycode)
            {
                int uid = loadComponent.player.GetComponent<IdentityComponent>().objectId;
                GameEngine.GameApi.Send(new BattleActorCmdNotify() { uid = uid, cmd = CmdType.MoveDown });
            }
            else if ((int) UnityEngine.KeyCode.D == keycode)
            {
                int uid = loadComponent.player.GetComponent<IdentityComponent>().objectId;
                GameEngine.GameApi.Send(new BattleActorCmdNotify() { uid = uid, cmd = CmdType.MoveRight });
            }
            else if ((int) UnityEngine.KeyCode.W == keycode)
            {
                int uid = loadComponent.player.GetComponent<IdentityComponent>().objectId;
                GameEngine.GameApi.Send(new BattleActorCmdNotify() { uid = uid, cmd = CmdType.MoveUp });
            }
            else if ((int) UnityEngine.KeyCode.K == keycode)
            {
                int uid = loadComponent.player.GetComponent<IdentityComponent>().objectId;
                GameEngine.GameApi.Send(new BattleActorCmdNotify() { uid = uid, cmd = CmdType.Attack });
            }
        }

        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.Escape, GameEngine.InputOperationType.Released)]
        private static void OnBattleSceneExit(int keycode, int operationType)
        {
            BattleScene scene = NE.SceneHandler.GetCurrentScene() as BattleScene;
            if (null == scene) return;

            NE.SceneHandler.ReplaceScene<MainScene>();
        }
    }
}
