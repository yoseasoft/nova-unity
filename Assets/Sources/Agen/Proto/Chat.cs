using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 消息id 1900 - 2000
    [ProtoContract]
    public partial class ChatRoleInfo : Object
    {
        [ProtoMember(1)]
        public int Pid { get; set; }

        [ProtoMember(3)]
        public string Name { get; set; }
    }

    /// <summary>
    /// 聊天消息
    /// </summary>
    [ProtoContract]
    public partial class ChatMsgInfo : Object
    {
        /// <summary>
        /// 频道类型
        /// </summary>
        [ProtoMember(1)]
        public int ChannelType { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [ProtoMember(2)]
        public int MsgType { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [ProtoMember(3)]
        public int FromUid { get; set; }

        /// <summary>
        /// 接受者
        /// </summary>
        [ProtoMember(4)]
        public int ToUid { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [ProtoMember(5)]
        public string Content { get; set; }
    }

    /// <summary>
    /// 聊天请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.ChatReq)]
    public partial class ChatReq : Object, IMessage
    {
        /// <summary>
        /// 频道类型 1-公告 2-世界 3-队伍 4-帮派 5-私聊
        /// </summary>
        [ProtoMember(1)]
        public int ChannelType { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [ProtoMember(2)]
        public int MsgType { get; set; }

        /// <summary>
        /// 目标
        /// </summary>
        [ProtoMember(3)]
        public int ToUid { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [ProtoMember(4)]
        public string Content { get; set; }
    }

    /// <summary>
    /// 聊天通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.ChatNotify)]
    public partial class ChatNotify : Object, IMessage
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        [ProtoMember(1)]
        public ChatMsgInfo Msg { get; set; }
    }
}