using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 宝石相关协议
    // 消息id 2401- 2500
    /// <summary>
    /// 背包宝石数据
    /// </summary>
    [ProtoContract]
    public partial class GemInfo : Object
    {
        /// <summary>
        /// 宝石id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [ProtoMember(2)]
        public int Num { get; set; }

        /// <summary>
        /// 新标记
        /// </summary>
        [ProtoMember(3)]
        public bool Newly { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        [ProtoMember(4)]
        public bool Locked { get; set; }
    }

    /// <summary>
    /// 镶嵌在孔位的宝石信息
    /// </summary>
    [ProtoContract]
    public partial class InlayGemInfo : Object
    {
        /// <summary>
        /// 镶嵌的孔位
        /// </summary>
        [ProtoMember(1)]
        public int InlaySlot { get; set; }

        /// <summary>
        /// 宝石id
        /// </summary>
        [ProtoMember(2)]
        public int Uid { get; set; }
    }

    /// <summary>
    /// 部位镶嵌宝石数据
    /// </summary>
    [ProtoContract]
    public partial class PartInlayGemInfo : Object
    {
        /// <summary>
        /// 部位镶嵌宝石列表
        /// </summary>
        [ProtoMember(1)]
        public List<InlayGemInfo> InlayGemList { get; set; } = new();
    }

    /// <summary>
    /// 方案镶嵌宝石数据
    /// </summary>
    [ProtoContract]
    public partial class PlanPartInlayGemInfo : Object
    {
        [ProtoMember(1)]
        public List<PartInlayGemInfo> PartGemList { get; set; } = new();
    }

    /// <summary>
    /// 合成宝石数据
    /// </summary>
    [ProtoContract]
    public partial class CraftingGemInfo : Object
    {
        /// <summary>
        /// 宝石id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [ProtoMember(2)]
        public int Num { get; set; }
    }

    /// <summary>
    /// 获取宝石背包请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GetGemBagListReq)]
    [MessageResponseType(ProtoOpcode.GetGemBagListResp)]
    public partial class GetGemBagListReq : Object, IMessage
    {
    }

    /// <summary>
    /// 获取宝石背包返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GetGemBagListResp)]
    public partial class GetGemBagListResp : Object, IMessage
    {
        /// <summary>
        /// 宝石背包列表
        /// </summary>
        [ProtoMember(1)]
        public List<GemInfo> GemBagList { get; set; } = new();
    }

    /// <summary>
    /// 获取镶嵌宝石数据请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GetInlayGemListReq)]
    [MessageResponseType(ProtoOpcode.GetInlayGemListResp)]
    public partial class GetInlayGemListReq : Object, IMessage
    {
    }

    /// <summary>
    /// 获取镶嵌宝石数据返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GetInlayGemListResp)]
    public partial class GetInlayGemListResp : Object, IMessage
    {
        /// <summary>
        /// 当前选择的方案
        /// </summary>
        [ProtoMember(1)]
        public int SelectedPlanId { get; set; }

        [ProtoMember(2)]
        public List<PlanPartInlayGemInfo> PlanGemList { get; set; } = new();
    }

    /// <summary>
    /// 镶嵌宝石请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.InlayGemReq)]
    [MessageResponseType(ProtoOpcode.InlayGemResp)]
    public partial class InlayGemReq : Object, IMessage
    {
        /// <summary>
        /// 背包宝石id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 方案id
        /// </summary>
        [ProtoMember(2)]
        public int PlanId { get; set; }

        /// <summary>
        /// 部位
        /// </summary>
        [ProtoMember(3)]
        public int Part { get; set; }

        /// <summary>
        /// 镶嵌的孔位
        /// </summary>
        [ProtoMember(4)]
        public int InlaySlot { get; set; }
    }

    /// <summary>
    /// 镶嵌宝石返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.InlayGemResp)]
    public partial class InlayGemResp : Object, IMessage
    {
        /// <summary>
        /// 方案id
        /// </summary>
        [ProtoMember(1)]
        public int PlanId { get; set; }

        /// <summary>
        /// 镶嵌部位
        /// </summary>
        [ProtoMember(2)]
        public int Part { get; set; }

        /// <summary>
        /// 镶嵌孔位数据
        /// </summary>
        [ProtoMember(3)]
        public InlayGemInfo SlotInfo { get; set; }
    }

    /// <summary>
    /// 合成宝石请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.CraftingGemReq)]
    [MessageResponseType(ProtoOpcode.CraftingGemResp)]
    public partial class CraftingGemReq : Object, IMessage
    {
        /// <summary>
        /// 合成宝石列表
        /// </summary>
        [ProtoMember(1)]
        public List<CraftingGemInfo> GemBagList { get; set; } = new();
    }

    /// <summary>
    /// 合成宝石返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.CraftingGemResp)]
    public partial class CraftingGemResp : Object, IMessage
    {
        /// <summary>
        /// 宝石列表
        /// </summary>
        [ProtoMember(1)]
        public List<GemInfo> GemList { get; set; } = new();
    }

    /// <summary>
    /// 洗炼宝石请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.RefineGemReq)]
    [MessageResponseType(ProtoOpcode.RefineGemResp)]
    public partial class RefineGemReq : Object, IMessage
    {
        /// <summary>
        /// 背包宝石id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }
    }

    /// <summary>
    /// 洗炼宝石返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.RefineGemResp)]
    public partial class RefineGemResp : Object, IMessage
    {
        /// <summary>
        /// 洗炼后的宝石id（表现用）
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }
    }

    /// <summary>
    /// 宝石背包变化通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GemBagChangeNotify)]
    public partial class GemBagChangeNotify : Object, IMessage
    {
        /// <summary>
        /// 宝石背包变化列表
        /// </summary>
        [ProtoMember(1)]
        public List<GemInfo> GemBagList { get; set; } = new();
    }

    /// <summary>
    /// 卸下宝石请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TakeoutInlaidGemReq)]
    [MessageResponseType(ProtoOpcode.TakeoutInlaidGemResp)]
    public partial class TakeoutInlaidGemReq : Object, IMessage
    {
        /// <summary>
        /// 方案id
        /// </summary>
        [ProtoMember(2)]
        public int PlanId { get; set; }

        /// <summary>
        /// 镶嵌部位
        /// </summary>
        [ProtoMember(1)]
        public int Part { get; set; }

        /// <summary>
        /// 镶嵌孔位
        /// </summary>
        [ProtoMember(3)]
        public int InlaySlot { get; set; }
    }

    /// <summary>
    /// 卸下宝石返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TakeoutInlaidGemResp)]
    public partial class TakeoutInlaidGemResp : Object, IMessage
    {
        /// <summary>
        /// 方案id
        /// </summary>
        [ProtoMember(1)]
        public int PlanId { get; set; }

        /// <summary>
        /// 镶嵌部位
        /// </summary>
        [ProtoMember(2)]
        public int Part { get; set; }

        /// <summary>
        /// 镶嵌孔位数据
        /// </summary>
        [ProtoMember(3)]
        public InlayGemInfo SlotInfo { get; set; }
    }

    /// <summary>
    /// 宝石背包格子中宝石的加/解锁请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.LockGemBagSlotReq)]
    public partial class LockGemBagSlotReq : Object, IMessage
    {
        /// <summary>
        /// 背包宝石id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// true:锁定;false:解锁
        /// </summary>
        [ProtoMember(2)]
        public bool Locked { get; set; }
    }

    /// <summary>
    /// 查看宝石背包新宝石请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.LookGemBagGemReq)]
    public partial class LookGemBagGemReq : Object, IMessage
    {
        /// <summary>
        /// 背包宝石id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }
    }

    /// <summary>
    /// 选择方案请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.SelectGemPlanReq)]
    [MessageResponseType(ProtoOpcode.SelectGemPlanResp)]
    public partial class SelectGemPlanReq : Object, IMessage
    {
        /// <summary>
        /// 选择的方案
        /// </summary>
        [ProtoMember(1)]
        public int SelectedPlanId { get; set; }
    }

    /// <summary>
    /// 选择方案返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.SelectGemPlanResp)]
    public partial class SelectGemPlanResp : Object, IMessage
    {
        /// <summary>
        /// 选择的方案
        /// </summary>
        [ProtoMember(1)]
        public int SelectedPlanId { get; set; }
    }
}