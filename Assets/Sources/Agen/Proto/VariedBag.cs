using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 背包相关协议
    // 消息id 1001- 1100 为账号流程
    /// <summary>
    /// 多变背包信息
    /// </summary>
    [ProtoContract]
    public partial class VariedBagInfo : Object
    {
        /// <summary>
        /// 多变背包标识
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 多变物品
        /// </summary>
        [ProtoMember(2)]
        public List<VariedItemInfo> Items { get; set; } = new();
    }

    /// <summary>
    /// 多变数据更改通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.VariedChangedNotify)]
    public partial class VariedChangedNotify : Object, IMessage
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 多变物品
        /// </summary>
        [ProtoMember(2)]
        public List<VariedItemInfo> Items { get; set; } = new();
    }

    /// <summary>
    /// 多变物品全量同步
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.VariedSyncNotify)]
    public partial class VariedSyncNotify : Object, IMessage
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 多变物品
        /// </summary>
        [ProtoMember(2)]
        public List<VariedItemInfo> Items { get; set; } = new();
    }

    /// <summary>
    /// 使用多变物品
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.UseItemVariedItemReq)]
    public partial class UseItemVariedItemReq : Object, IMessage
    {
        /// <summary>
        /// 见 SYS_IDENTIFICATION 系统标识
        /// </summary>
        [ProtoMember(1)]
        public int BagUid { get; set; }

        /// <summary>
        /// 使用物品请求
        /// </summary>
        [ProtoMember(2)]
        public NetSlotItemData Item { get; set; }

        /// <summary>
        /// 使用参数
        /// </summary>
        [ProtoMember(3)]
        public VariedUseParams UseParams { get; set; }
    }
}