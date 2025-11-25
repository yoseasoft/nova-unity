/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-11-12
/// 功能描述：
/// </summary>

namespace Game
{
    /// <summary>
    /// 世界场景输入逻辑处理类
    /// </summary>
    public static class WorldSceneInputSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        private static void Awake(this WorldScene self)
        {
            Debugger.Log("欢迎进入世界场景输入业务流程！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        private static void Start(this WorldScene self)
        {
            AccountManager.Start();
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        private static void Destroy(this WorldScene self)
        {
            AccountManager.Stop();
            Debugger.Log("当前正在销毁并退出世界场景输入业务流程！");
        }

        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.Alpha1, GameEngine.InputOperationType.Released)]
        private static void OnRemoteAccessBegan(int keycode, int operationType)
        {
            OnServerConnect();
        }

        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.Alpha2, GameEngine.InputOperationType.Released)]
        private static void OnRemoteAccessNotified(int keycode, int operationType)
        {
            AccountManager.Send(new LogoTimerComponent());
        }

        [GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.Alpha3, GameEngine.InputOperationType.Released)]
        private static void OnRemoteAccessEnded(int keycode, int operationType)
        {
            AccountManager.Disconnect();
        }

        static async void OnServerConnect()
        {
            await AccountManager.Connect(NovaEngine.NetworkServiceType.WebSocket, "MainServer", "ws://192.168.110.54:80");
            Debugger.Info($"网络连接状态={AccountManager.Channel.IsConnected}");
        }
    }
}
