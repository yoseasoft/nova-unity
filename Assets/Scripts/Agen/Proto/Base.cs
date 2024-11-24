using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    /// <summary>
    /// 消息错误
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MessageErrorNotify)]
    public partial class MessageErrorNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public string Cmd { get; set; }

        [ProtoMember(2)]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// ping消息
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.PingReq)]
    [MessageResponseType(ProtoOpcode.PingResp)]
    public partial class PingReq : Object, IMessage
    {
        /// <summary>
        /// 数据
        /// </summary>
        [ProtoMember(1)]
        public string Str { get; set; }
    }

    /// <summary>
    /// ping回复 sec_time + milli_time = 最终时间
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.PingResp)]
    public partial class PingResp : Object, IMessage
    {
        /// <summary>
        /// 回传
        /// </summary>
        [ProtoMember(1)]
        public string Str { get; set; }

        /// <summary>
        /// 秒值
        /// </summary>
        [ProtoMember(2)]
        public int SecTime { get; set; }

        /// <summary>
        /// 毫秒值
        /// </summary>
        [ProtoMember(3)]
        public int MilliTime { get; set; }
    }

    /// <summary>
    /// 握手
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.HandshakeReq)]
    [MessageResponseType(ProtoOpcode.HandshakeResp)]
    public partial class HandshakeReq : Object, IMessage
    {
        [ProtoMember(1)]
        public string Token { get; set; }
    }

    [ProtoContract]
    [Message(ProtoOpcode.HandshakeResp)]
    public partial class HandshakeResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }
    }

    /// <summary>
    /// 服务端主动踢下线前发送此协议
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.KickNotify)]
    public partial class KickNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public string Reason { get; set; }
    }

    /// <summary>
    /// 检查版本
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.CheckVersionReq)]
    [MessageResponseType(ProtoOpcode.CheckVersionResp)]
    public partial class CheckVersionReq : Object, IMessage
    {
        /// <summary>
        /// 客户端版本
        /// </summary>
        [ProtoMember(1)]
        public string CliVer { get; set; }

        /// <summary>
        /// 逻辑版本
        /// </summary>
        [ProtoMember(2)]
        public string LogicVer { get; set; }

        /// <summary>
        /// 引擎版本
        /// </summary>
        [ProtoMember(3)]
        public string EngineVer { get; set; }
    }

    /// <summary>
    /// 检查版本回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.CheckVersionResp)]
    public partial class CheckVersionResp : Object, IMessage
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [ProtoMember(1)]
        public int Code { get; set; }

        /// <summary>
        /// 时区值
        /// </summary>
        [ProtoMember(2)]
        public int TimeZone { get; set; }
    }

    /// <summary>
    /// 异常通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.ExceptionNotify)]
    public partial class ExceptionNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public int ReasonCode { get; set; }
    }

    /// <summary>
    /// 大包通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.BigPacketNotify)]
    public partial class BigPacketNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public int Type { get; set; }

        [ProtoMember(2)]
        public int Total { get; set; }

        [ProtoMember(3)]
        public int Index { get; set; }

        [ProtoMember(4)]
        public byte[] Data { get; set; }
    }
}