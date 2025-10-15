using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 新手任务相关协议
    // 消息id 1200- 1300
    /// <summary>
    /// 新手任务数据
    /// </summary>
    [ProtoContract]
    public partial class NoviceTaskInfo : Object
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
    /// 新手任务全量发送
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.NoviceTaskNotify)]
    public partial class NoviceTaskNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public List<NoviceTaskInfo> NoviceTaskList { get; set; } = new();
    }

    /// <summary>
    /// 新手任务变化通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.NoviceTaskChangedNotify)]
    public partial class NoviceTaskChangedNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public NoviceTaskInfo Task { get; set; }
    }

    /// <summary>
    /// 新手任务领奖请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.NoviceTaskGetRewardReq)]
    public partial class NoviceTaskGetRewardReq : Object, IMessage
    {
        /// <summary>
        /// 任务id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }
    }
}