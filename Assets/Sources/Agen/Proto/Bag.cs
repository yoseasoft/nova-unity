using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 背包相关协议
    // 消息id 901- 1000 为账号流程
    /// <summary>
    /// 背包数据
    /// </summary>
    [ProtoContract]
    public partial class BagInfo : Object
    {
        /// <summary>
        /// 标识 1：人物背包 2.临时背包
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 背包大小
        /// </summary>
        [ProtoMember(2)]
        public int Num { get; set; }

        /// <summary>
        /// 背包格子
        /// </summary>
        [ProtoMember(3)]
        public List<BagSlot> Slots { get; set; } = new();
    }

    /// <summary>
    /// 背包数据更改通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.BagChangedNotify)]
    public partial class BagChangedNotify : Object, IMessage
    {
        /// <summary>
        /// 标识 1：人物背包 2.临时背包
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        [ProtoMember(2)]
        public List<BagSlot> Slots { get; set; } = new();
    }

    /// <summary>
    /// 背包数据下发
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.BagInfoNotify)]
    public partial class BagInfoNotify : Object, IMessage
    {
        /// <summary>
        /// 背包数据
        /// </summary>
        [ProtoMember(1)]
        public BagInfo Bag { get; set; }
    }

    /// <summary>
    /// 使用背包物品
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.UseItemReq)]
    public partial class UseItemReq : Object, IMessage
    {
        /// <summary>
        /// 背包类型，1：人物背包、  2：临时背包
        /// </summary>
        [ProtoMember(1)]
        public int BagUid { get; set; }

        /// <summary>
        /// 使用物品请求
        /// </summary>
        [ProtoMember(2)]
        public NetSlotItemData Item { get; set; }
    }
}