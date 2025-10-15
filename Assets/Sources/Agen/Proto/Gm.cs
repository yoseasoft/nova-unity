using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    /// <summary>
    /// 握手
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GmReq)]
    public partial class GmReq : Object, IMessage
    {
        /// <summary>
        /// gm指令
        /// </summary>
        [ProtoMember(1)]
        public string GmCmd { get; set; }

        /// <summary>
        /// gm参数
        /// </summary>
        [ProtoMember(2)]
        public string GmParams { get; set; }
    }

    /// <summary>
    /// gm日志信息
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GmLogNotify)]
    public partial class GmLogNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public string GmStr { get; set; }
    }

    /// <summary>
    /// 测试消息
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TestReq)]
    [MessageResponseType(ProtoOpcode.TestResp)]
    public partial class TestReq : Object, IMessage
    {
    }

    /// <summary>
    /// 测试消息回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TestResp)]
    public partial class TestResp : Object, IMessage
    {
    }
}