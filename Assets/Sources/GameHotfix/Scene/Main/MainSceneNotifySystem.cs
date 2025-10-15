
using System.Runtime.Serialization;


/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-17
/// 功能描述：
/// </summary>
namespace Game
{
    public struct MainSimpleNotifyItemData
    {
        public int id;
        public string name;
        public string desc;

        public int level;
        public int quality;
    }

    /// <summary>
    /// 游戏主场景通知逻辑类
    /// </summary>
    static class MainSceneNotifySystem
    {
        [OnAwake]
        static void Awake(this MainScene self)
        {
            Debugger.Log("欢迎进入Main场景通知业务流程！");

            self.AddComponent<MainTerrainComponent>();
            self.AddComponent<MainBuildComponent>();
        }

        [OnStart]
        static void Start(this MainScene self)
        {
        }

        [OnDestroy]
        static void Destroy(this MainScene self)
        {
            self.RemoveComponent<MainTerrainComponent>();
            self.RemoveComponent<MainBuildComponent>();

            Debugger.Log("当前正在销毁并退出Main场景通知业务流程！");
        }

        [GameEngine.OnEventDispatchCall(3101)]
        [GameEngine.OnEventDispatchCall(3102)]
        private static void OnCatchGlobalEventByCode(int eventID, params object[] args)
        {
            Debugger.Info("Main场景以编码绑定方式成功捕获【全局】事件[{%d}]，事件参数：{%s}！", eventID, NovaEngine.Utility.Text.ToString(args));
        }

        [GameEngine.OnEventDispatchCall(typeof(MainSimpleNotifyItemData))]
        private static void OnRecvEventByType(MainSimpleNotifyItemData eventData)
        {
            Debugger.Info("Main场景以类型绑定方式成功捕获【全局】事件[{%t}] - {{{%d},{%s},{%s},{%d},{%d}}}！", eventData,
                eventData.id, eventData.name, eventData.desc, eventData.level, eventData.quality);
        }

        [GameEngine.OnMessageDispatchCall(Proto.ProtoOpcode.MessageErrorNotify)]
        [GameEngine.OnMessageDispatchCall(Proto.ProtoOpcode.KickNotify)]
        private static void OnRecvMessageByCode(ProtoBuf.Extension.IMessage message)
        {
            int opcode = NE.NetworkHandler.GetOpcodeByMessageType(message.GetType());

            if (message is Proto.MessageErrorNotify messageErrorNotify)
            {
                Debugger.Info("Main场景以编码绑定方式成功捕获【全局】消息[{%d}]，解析消息内容：{%s},{%d}！", opcode, messageErrorNotify.Cmd, messageErrorNotify.ErrorCode);
            }
            else if (message is Proto.KickNotify kickNotify)
            {
                Debugger.Info("Main场景以编码绑定方式成功捕获【全局】消息[{%d}]，解析消息内容：{%s}！", opcode, kickNotify.Reason);
            }
        }

        [GameEngine.OnMessageDispatchCall(typeof(Proto.MessageErrorNotify))]
        private static void OnRecvMessageByType(Proto.MessageErrorNotify message)
        {
            Debugger.Info("Main场景以类型绑定方式成功捕获【全局】消息[{%t}] - {{{%s},{%d}}}！", message, message.Cmd, message.ErrorCode);
        }
    }
}
