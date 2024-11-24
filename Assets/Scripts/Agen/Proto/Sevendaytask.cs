using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 七日任务相关协议
    // 消息id 1301- 1400
    /// <summary>
    /// 任务数据
    /// </summary>
    [ProtoContract]
    public partial class SevendaytaskInfo : Object
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
    /// 七日任务全量发送
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.SevendaytaskNotify)]
    public partial class SevendaytaskNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public List<SevendaytaskInfo> TaskList { get; set; } = new();
    }

    /// <summary>
    /// 七日任务变化通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.SevendaytaskChangedNotify)]
    public partial class SevendaytaskChangedNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public SevendaytaskInfo Task { get; set; }
    }

    /// <summary>
    /// 七日任务领奖请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.SevendaytaskGetRewardReq)]
    public partial class SevendaytaskGetRewardReq : Object, IMessage
    {
        /// <summary>
        /// 任务id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }
    }
}