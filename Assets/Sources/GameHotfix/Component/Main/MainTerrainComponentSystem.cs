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
    /// Main场景地形组件逻辑处理类
    /// </summary>
    public static class MainTerrainComponentSystem
    {
        [OnAwake]
        private static void Awake(this MainTerrainComponent self)
        {
            Debugger.Log("当前正在创建并激活Main场景地形组件！");
        }

        [OnDestroy]
        private static void Destroy(this MainTerrainComponent self)
        {
            Debugger.Log("当前正在销毁并退出Main场景地形组件！");
        }

        [GameEngine.EventSubscribeBindingOfTarget(3101)]
        [GameEngine.EventSubscribeBindingOfTarget(3102)]
        private static void OnRecvEventByCode(this MainTerrainComponent self, int eventID, params object[] args)
        {
            Debugger.Info("Main场景【地形】组件以编码绑定方式成功接收事件[{%d}]，事件参数：{%s}！", eventID, NovaEngine.Utility.Text.ToString(args));
        }

        [GameEngine.EventSubscribeBindingOfTarget(typeof(MainSimpleNotifyItemData))]
        private static void OnRecvEventByType(this MainTerrainComponent self, MainSimpleNotifyItemData eventData)
        {
            Debugger.Info("Main场景【地形】组件以类型绑定方式成功接收事件[{%t}] - {{{%d},{%s},{%s},{%d},{%d}}}！", eventData,
                eventData.id, eventData.name, eventData.desc, eventData.level, eventData.quality);
        }

        [GameEngine.MessageListenerBindingOfTarget(Proto.ProtoOpcode.MessageErrorNotify)]
        [GameEngine.MessageListenerBindingOfTarget(Proto.ProtoOpcode.KickNotify)]
        private static void OnRecvMessageByCode(this MainTerrainComponent self, ProtoBuf.Extension.IMessage message)
        {
            int opcode = NE.NetworkHandler.GetOpcodeByMessageType(message.GetType());

            if (message is Proto.MessageErrorNotify messageErrorNotify)
            {
                Debugger.Info("Main场景【地形】组件以编码绑定方式成功接收消息[{%d}]，解析消息内容：{%s},{%d}！", opcode, messageErrorNotify.Cmd, messageErrorNotify.ErrorCode);
            }
            else if (message is Proto.KickNotify kickNotify)
            {
                Debugger.Info("Main场景【地形】组件以编码绑定方式成功接收消息[{%d}]，解析消息内容：{%s}！", opcode, kickNotify.Reason);
            }
        }

        [GameEngine.MessageListenerBindingOfTarget(typeof(Proto.MessageErrorNotify))]
        private static void OnRecvMessageByType(this MainTerrainComponent self, Proto.MessageErrorNotify message)
        {
            Debugger.Info("Main场景【地形】组件以类型绑定方式成功接收消息[{%t}] - {{{%s},{%d}}}！", message, message.Cmd, message.ErrorCode);
        }
    }
}

