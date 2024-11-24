/// <summary>
/// 2023-09-06 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 消息处理管理器封装对象类，对网络消息进行解析及派发等处理
    /// </summary>
    [GameEngine.MessageSystem]
    public static partial class MsgProcessor
    {
        [GameEngine.OnMessageDispatchCall(1001)]
        public static void OnLoginProcess(ProtoBuf.Extension.IMessage message)
        {
        }

        public static void OnLogoutProcess(ProtoBuf.Extension.IMessage message)
        {
        }

        [GameEngine.OnMessageDispatchCall(Proto.ProtoOpcode.PingResp)]
        public static void OnPingRespForOpcodeProcess(ProtoBuf.Extension.IMessage message)
        {
            Proto.PingResp resp = message as Proto.PingResp;
            Debugger.Warn("以全局静态函数的方式接收消息'{0}',标签为{1},内容为:[{2},{3},{4}].", message.GetType().FullName, Proto.ProtoOpcode.PingResp, resp.Str, resp.SecTime, resp.MilliTime);
        }

        [GameEngine.OnMessageDispatchCall(typeof(Proto.PingResp))]
        public static void OnPingRespForTypeProcess(Proto.PingResp message)
        {
            Debugger.Warn("以全局静态函数的方式接收消息'{0}',内容为:[{1},{2},{3}].", message.GetType().FullName, message.Str, message.SecTime, message.MilliTime);
        }

        [GameEngine.OnMessageDispatchCall(typeof(Soldier), typeof(Proto.PingResp))]
        public static void OnPingRespForProtoProcess(Soldier soldier, Proto.PingResp message)
        {
            Debugger.Warn("以全局静态函数的方式接收消息'{0}',内容为:[{1},{2},{3}],目标对象为{4}.", message.GetType().FullName, message.Str, message.SecTime, message.MilliTime, soldier.GetType().FullName);
        }

        [GameEngine.OnMessageDispatchCall(Proto.ProtoOpcode.HandshakeResp)]
        public static void OnHandshakeRespForOpcodeProcess(ProtoBuf.Extension.IMessage message)
        {
            Proto.HandshakeResp resp = message as Proto.HandshakeResp;
            Debugger.Warn("以全局静态函数的方式接收消息'{0}',标签为{1},内容为:[Code = {2}].", message.GetType().FullName, Proto.ProtoOpcode.HandshakeResp, resp.Code);
        }

        [GameEngine.OnMessageDispatchCall(typeof(Proto.HandshakeResp))]
        public static void OnHandshakeRespForTypeProcess(Proto.HandshakeResp message)
        {
            Debugger.Warn("以全局静态函数的方式接收消息'{0}',内容为:[Code = {1}].", message.GetType().FullName, message.Code);
        }

        [GameEngine.OnMessageDispatchCall(typeof(Soldier), typeof(Proto.HandshakeResp))]
        public static void OnPingRespForProtoProcess(Soldier soldier, Proto.HandshakeResp message)
        {
            Debugger.Warn("以全局静态函数的方式接收消息'{0}',内容为:[Code = {1}],目标对象为{2}.", message.GetType().FullName, message.Code, soldier.GetType().FullName);
        }
    }
}
