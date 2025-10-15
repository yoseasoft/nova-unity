using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 每日任务相关协议
    // 消息id 1100- 1200
    /// <summary>
    /// 背包数据
    /// </summary>
    [ProtoContract]
    public partial class DaytaskInfo : Object
    {
        /// <summary>
        /// 任务id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 当前数
        /// </summary>
        [ProtoMember(2)]
        public int CurNum { get; set; }

        /// <summary>
        /// 是否已领奖
        /// </summary>
        [ProtoMember(3)]
        public int IsReward { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        [ProtoMember(4)]
        public bool IsFinish { get; set; }
    }

    /// <summary>
    /// 每日任务全量发送
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.DaytaskNotify)]
    public partial class DaytaskNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public List<DaytaskInfo> DaytaskList { get; set; } = new();
    }

    /// <summary>
    /// 每日任务变化通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.DaytaskChangedNotify)]
    public partial class DaytaskChangedNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public DaytaskInfo Task { get; set; }
    }

    /// <summary>
    /// 每日任务领奖请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.DaytaskGetRewardReq)]
    public partial class DaytaskGetRewardReq : Object, IMessage
    {
        /// <summary>
        /// 任务id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }
    }
}