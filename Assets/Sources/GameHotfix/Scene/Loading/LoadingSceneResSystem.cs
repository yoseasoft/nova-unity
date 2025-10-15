/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-16
/// 功能描述：
/// </summary>

namespace Game
{
    /// <summary>
    /// Loading场景资源逻辑处理类
    /// </summary>
    public static class LoadingSceneResSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        public static void Awake(this LoadingScene self)
        {
            Debugger.Log("欢迎进入Loading场景资源预载入业务流程！");

            self.AddComponent<LoadingResComponent>();
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        public static void Start(this LoadingScene self)
        {
            // 模拟接收到服务端的消息通知
            NE.NetworkHandler.OnSimulationReceiveMessageOfProtoBuf(new Proto.PingResp() { Str = "yukie", SecTime = 10101, MilliTime = 20202 });
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        public static void Destroy(this LoadingScene self)
        {
            self.RemoveComponent<LoadingResComponent>();

            Debugger.Log("当前正在销毁并退出Loading场景资源预载入业务流程！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        public static void Update(this LoadingScene self)
        {
        }
    }
}
