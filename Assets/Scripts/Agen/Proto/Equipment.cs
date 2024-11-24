using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 游戏相关协议
    // 消息id 2301- 2400
    /// <summary>
    /// 装备数据
    /// </summary>
    [ProtoContract]
    public partial class EquipmentInfo : Object
    {
        /// <summary>
        /// 装备部位
        /// </summary>
        [ProtoMember(1)]
        public int Slot { get; set; }

        /// <summary>
        /// 装备部位等级
        /// </summary>
        [ProtoMember(2)]
        public int Level { get; set; }

        /// <summary>
        /// 装备id
        /// </summary>
        [ProtoMember(3)]
        public int Uid { get; set; }

        /// <summary>
        /// 附加属性
        /// </summary>
        [ProtoMember(4)]
        public List<int> ExtAttrList { get; set; } = new();
    }

    /// <summary>
    /// 装备部位等级数据
    /// </summary>
    [ProtoContract]
    public partial class EquipSlotInfo : Object
    {
        /// <summary>
        /// 装备部位
        /// </summary>
        [ProtoMember(1)]
        public int Slot { get; set; }

        /// <summary>
        /// 装备部位等级
        /// </summary>
        [ProtoMember(2)]
        public int Level { get; set; }
    }

    /// <summary>
    /// 获取装备数据请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GetEquipmentListReq)]
    [MessageResponseType(ProtoOpcode.GetEquipmentListResp)]
    public partial class GetEquipmentListReq : Object, IMessage
    {
    }

    /// <summary>
    /// 获取装备数据返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GetEquipmentListResp)]
    public partial class GetEquipmentListResp : Object, IMessage
    {
        /// <summary>
        /// 装备列表
        /// </summary>
        [ProtoMember(1)]
        public List<EquipmentInfo> EquipmentList { get; set; } = new();
    }

    /// <summary>
    /// 穿上装备请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.WearEquipmentReq)]
    [MessageResponseType(ProtoOpcode.WearEquipmentResp)]
    public partial class WearEquipmentReq : Object, IMessage
    {
        /// <summary>
        /// 背包类型, 1:人物背包; 2:临时背包
        /// </summary>
        [ProtoMember(1)]
        public int BagUid { get; set; }

        /// <summary>
        /// 背包格子
        /// </summary>
        [ProtoMember(2)]
        public int Slot { get; set; }
    }

    /// <summary>
    /// 穿上装备返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.WearEquipmentResp)]
    public partial class WearEquipmentResp : Object, IMessage
    {
        /// <summary>
        /// 变化的装备
        /// </summary>
        [ProtoMember(1)]
        public List<EquipmentInfo> EquipmentList { get; set; } = new();
    }

    /// <summary>
    /// 升级装备请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.UpgradeEquipmentReq)]
    [MessageResponseType(ProtoOpcode.UpgradeEquipmentResp)]
    public partial class UpgradeEquipmentReq : Object, IMessage
    {
        /// <summary>
        /// 装备孔位
        /// </summary>
        [ProtoMember(1)]
        public int Slot { get; set; }
    }

    /// <summary>
    /// 升级装备返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.UpgradeEquipmentResp)]
    public partial class UpgradeEquipmentResp : Object, IMessage
    {
        /// <summary>
        /// 变化的装备
        /// </summary>
        [ProtoMember(1)]
        public List<EquipmentInfo> EquipmentList { get; set; } = new();
    }

    /// <summary>
    /// 洗练装备请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.RefineEquipmentReq)]
    [MessageResponseType(ProtoOpcode.RefineEquipmentResp)]
    public partial class RefineEquipmentReq : Object, IMessage
    {
        /// <summary>
        /// 装备部位
        /// </summary>
        [ProtoMember(1)]
        public int Slot { get; set; }
    }

    /// <summary>
    /// 洗练装备返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.RefineEquipmentResp)]
    public partial class RefineEquipmentResp : Object, IMessage
    {
        /// <summary>
        /// 装备部位
        /// </summary>
        [ProtoMember(1)]
        public int Slot { get; set; }

        /// <summary>
        /// 洗炼出的附加属性
        /// </summary>
        [ProtoMember(2)]
        public List<int> ExtAttrList { get; set; } = new();
    }

    /// <summary>
    /// 洗炼装备属性替换请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.RefineEquipmentReplaceReq)]
    [MessageResponseType(ProtoOpcode.RefineEquipmentReplaceResp)]
    public partial class RefineEquipmentReplaceReq : Object, IMessage
    {
        /// <summary>
        /// 装备部位
        /// </summary>
        [ProtoMember(1)]
        public int Slot { get; set; }

        /// <summary>
        /// 是否替换属性
        /// </summary>
        [ProtoMember(2)]
        public bool Replace { get; set; }
    }

    /// <summary>
    /// 洗炼装备属性替换返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.RefineEquipmentReplaceResp)]
    public partial class RefineEquipmentReplaceResp : Object, IMessage
    {
        /// <summary>
        /// 变化的装备
        /// </summary>
        [ProtoMember(1)]
        public List<EquipmentInfo> EquipmentList { get; set; } = new();
    }

    /// <summary>
    /// 分解背包装备请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.DismantleEquipmentReq)]
    [MessageResponseType(ProtoOpcode.DismantleEquipmentResp)]
    public partial class DismantleEquipmentReq : Object, IMessage
    {
        /// <summary>
        /// 背包类型, 1:人物背包; 2:临时背包
        /// </summary>
        [ProtoMember(1)]
        public int BagUid { get; set; }

        /// <summary>
        /// 背包格子列表
        /// </summary>
        [ProtoMember(2)]
        public List<int> Slots { get; set; } = new();
    }

    /// <summary>
    /// 分解背包装备返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.DismantleEquipmentResp)]
    public partial class DismantleEquipmentResp : Object, IMessage
    {
    }

    /// <summary>
    /// 一键升级请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.QuickUpgradeEquipmentReq)]
    [MessageResponseType(ProtoOpcode.QuickUpgradeEquipmentResp)]
    public partial class QuickUpgradeEquipmentReq : Object, IMessage
    {
        /// <summary>
        /// 装备部位升级列表
        /// </summary>
        [ProtoMember(1)]
        public List<EquipSlotInfo> EquipSlotList { get; set; } = new();
    }

    /// <summary>
    /// 一键升级返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.QuickUpgradeEquipmentResp)]
    public partial class QuickUpgradeEquipmentResp : Object, IMessage
    {
        /// <summary>
        /// 变化的装备
        /// </summary>
        [ProtoMember(1)]
        public List<EquipmentInfo> EquipmentList { get; set; } = new();
    }
}