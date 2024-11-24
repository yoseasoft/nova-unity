using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 游戏相关协议
    // 消息id 2201- 2300
    /// <summary>
    /// 体力购买请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.EnergyBuyReq)]
    public partial class EnergyBuyReq : Object, IMessage
    {
        [ProtoMember(1)]
        public int Num { get; set; }
    }

    /// <summary>
    /// 存储体力购买请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.EnergyStorageBuyReq)]
    public partial class EnergyStorageBuyReq : Object, IMessage
    {
        [ProtoMember(1)]
        public int Num { get; set; }
    }

    /// <summary>
    /// 体力变更通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.EnergyChangedNotify)]
    public partial class EnergyChangedNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public int Num { get; set; }
    }

    /// <summary>
    /// 体力恢复时间变更通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.EnergyTimeChangedNotify)]
    public partial class EnergyTimeChangedNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public int Num { get; set; }
    }

    /// <summary>
    /// 充值请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.RechargeReq)]
    [MessageResponseType(ProtoOpcode.RechargeResp)]
    public partial class RechargeReq : Object, IMessage
    {
        /// <summary>
        /// 充值产品id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 扩展数据
        /// </summary>
        [ProtoMember(2)]
        public string Extension { get; set; }
    }

    /// <summary>
    /// 充值回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.RechargeResp)]
    public partial class RechargeResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        /// <summary>
        /// 产品id
        /// </summary>
        [ProtoMember(2)]
        public int Id { get; set; }

        /// <summary>
        /// 内部订单号
        /// </summary>
        [ProtoMember(3)]
        public string Cporder { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        [ProtoMember(4)]
        public string Extension { get; set; }
    }

    /// <summary>
    /// 月卡信息
    /// </summary>
    [ProtoContract]
    public partial class MonthCardInfo : Object
    {
        /// <summary>
        /// 月卡id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 月卡过期时间
        /// </summary>
        [ProtoMember(2)]
        public int ExpireTime { get; set; }
    }

    /// <summary>
    /// 月卡通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MonthCardNotify)]
    public partial class MonthCardNotify : Object, IMessage
    {
        /// <summary>
        /// 月卡列表
        /// </summary>
        [ProtoMember(1)]
        public List<MonthCardInfo> MonthCardList { get; set; } = new();
    }

    /// <summary>
    /// 宝石商店广告抽卡
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TreasureShopAdDrawReq)]
    [MessageResponseType(ProtoOpcode.TreasureShopAdDrawResp)]
    public partial class TreasureShopAdDrawReq : Object, IMessage
    {
        /// <summary>
        /// 宝箱id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }
    }

    /// <summary>
    /// 宝石商城广告抽卡回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TreasureShopAdDrawResp)]
    public partial class TreasureShopAdDrawResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        [ProtoMember(2)]
        public List<BagItem> ItemList { get; set; } = new();
    }

    /// <summary>
    /// 宝石商店普通抽卡
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TreasureShopDrawReq)]
    [MessageResponseType(ProtoOpcode.TreasureShopDrawResp)]
    public partial class TreasureShopDrawReq : Object, IMessage
    {
        /// <summary>
        /// 宝箱id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 抽卡数量
        /// </summary>
        [ProtoMember(2)]
        public int Num { get; set; }
    }

    /// <summary>
    /// 宝石商店普通抽卡回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TreasureShopDrawResp)]
    public partial class TreasureShopDrawResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        [ProtoMember(2)]
        public List<BagItem> ItemList { get; set; } = new();
    }

    /// <summary>
    /// 宝石商店物品信息
    /// </summary>
    [ProtoContract]
    public partial class TreasureShopItemInfo : Object
    {
        /// <summary>
        /// 产品id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 广告购买数量
        /// </summary>
        [ProtoMember(2)]
        public int AdBuyCount { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        [ProtoMember(3)]
        public int BuyCount { get; set; }
    }

    /// <summary>
    /// 宝石商店信息同步
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TreasureShopNotify)]
    public partial class TreasureShopNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public List<TreasureShopItemInfo> ItemList { get; set; } = new();
    }

    /// <summary>
    /// 宝石商店变更同步
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TreasureShopChangedNotify)]
    public partial class TreasureShopChangedNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public TreasureShopItemInfo Item { get; set; }
    }

    /// <summary>
    /// 商品购买记录
    /// </summary>
    [ProtoContract]
    public partial class ShopBuyInfo : Object
    {
        /// <summary>
        /// 商品id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        [ProtoMember(2)]
        public int BuyCount { get; set; }
    }

    /// <summary>
    /// 商店信息同步
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.ShopNotify)]
    public partial class ShopNotify : Object, IMessage
    {
        /// <summary>
        /// 购买记录列表
        /// </summary>
        [ProtoMember(1)]
        public List<ShopBuyInfo> ItemList { get; set; } = new();
    }

    /// <summary>
    /// 商店变更同步
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.ShopChangedNotify)]
    public partial class ShopChangedNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public List<ShopBuyInfo> Item { get; set; } = new();
    }

    /// <summary>
    /// 商品购买
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.ShopBuyReq)]
    [MessageResponseType(ProtoOpcode.ShopBuyResp)]
    public partial class ShopBuyReq : Object, IMessage
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public int Num { get; set; }
    }

    /// <summary>
    /// 商品购买回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.ShopBuyResp)]
    public partial class ShopBuyResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        [ProtoMember(2)]
        public List<BagItem> ItemList { get; set; } = new();
    }

    /// <summary>
    /// 月卡日奖励领取请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MonthCardDayRewardReq)]
    [MessageResponseType(ProtoOpcode.MonthCardDayRewardResp)]
    public partial class MonthCardDayRewardReq : Object, IMessage
    {
    }

    /// <summary>
    /// 月卡日奖励领取
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MonthCardDayRewardResp)]
    public partial class MonthCardDayRewardResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        [ProtoMember(2)]
        public List<BagItem> ItemList { get; set; } = new();
    }

    /// <summary>
    /// 挂机奖励领取请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.IdleRewardReq)]
    [MessageResponseType(ProtoOpcode.IdleRewardResp)]
    public partial class IdleRewardReq : Object, IMessage
    {
    }

    /// <summary>
    /// 挂机奖励领取回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.IdleRewardResp)]
    public partial class IdleRewardResp : Object, IMessage
    {
        /// <summary>
        /// 最后一次奖励时间
        /// </summary>
        [ProtoMember(1)]
        public int LastRewardTime { get; set; }

        /// <summary>
        /// 展示的奖励物品列表
        /// </summary>
        [ProtoMember(2)]
        public List<BagItem> RewardList { get; set; } = new();
    }

    /// <summary>
    /// 巡逻奖励请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.QuickPassRewardReq)]
    [MessageResponseType(ProtoOpcode.QuickPassRewardResp)]
    public partial class QuickPassRewardReq : Object, IMessage
    {
    }

    /// <summary>
    /// 巡逻奖励回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.QuickPassRewardResp)]
    public partial class QuickPassRewardResp : Object, IMessage
    {
        /// <summary>
        /// 巡逻奖励领取次数
        /// </summary>
        [ProtoMember(1)]
        public int QuickPassRewardCount { get; set; }

        /// <summary>
        /// 经验值
        /// </summary>
        [ProtoMember(2)]
        public int Exp { get; set; }

        /// <summary>
        /// 展示的奖励物品列表
        /// </summary>
        [ProtoMember(3)]
        public List<BagItem> RewardList { get; set; } = new();
    }

    /// <summary>
    /// 广告巡逻奖励请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.AdQuickPassRewardReq)]
    [MessageResponseType(ProtoOpcode.AdQuickPassRewardResp)]
    public partial class AdQuickPassRewardReq : Object, IMessage
    {
    }

    /// <summary>
    /// 广告巡逻奖励回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.AdQuickPassRewardResp)]
    public partial class AdQuickPassRewardResp : Object, IMessage
    {
        /// <summary>
        /// 广告巡逻奖励领取次数
        /// </summary>
        [ProtoMember(1)]
        public int AdPassRewardCount { get; set; }

        /// <summary>
        /// 经验值
        /// </summary>
        [ProtoMember(2)]
        public int Exp { get; set; }

        /// <summary>
        /// 展示的奖励物品列表
        /// </summary>
        [ProtoMember(3)]
        public List<BagItem> RewardList { get; set; } = new();
    }

    /// <summary>
    /// 挂机特殊奖励个数预览请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.IdleSpecialRewardPreviewReq)]
    [MessageResponseType(ProtoOpcode.IdleSpecialRewardPreviewResp)]
    public partial class IdleSpecialRewardPreviewReq : Object, IMessage
    {
    }

    /// <summary>
    /// 挂机特殊奖励个数预览返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.IdleSpecialRewardPreviewResp)]
    public partial class IdleSpecialRewardPreviewResp : Object, IMessage
    {
        /// <summary>
        /// 普通奖励可领取次数
        /// </summary>
        [ProtoMember(1)]
        public int CanReceiveCount { get; set; }

        /// <summary>
        /// 特殊奖励1的个数
        /// </summary>
        [ProtoMember(2)]
        public int PreSpecialRewardCount { get; set; }

        /// <summary>
        /// 特殊奖励2的个数
        /// </summary>
        [ProtoMember(3)]
        public int PreSpecialReward2Count { get; set; }
    }
}