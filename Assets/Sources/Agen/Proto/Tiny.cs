using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // tiny相关协议
    // 消息id 2001- 2100 为tiny流程
    /// <summary>
    /// 卡牌数据
    /// </summary>
    [ProtoContract]
    public partial class TinyCard : Object
    {
        /// <summary>
        /// 卡牌唯一id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 卡牌表id
        /// </summary>
        [ProtoMember(2)]
        public int ClassId { get; set; }

        /// <summary>
        /// 摆放位置
        /// </summary>
        [ProtoMember(3)]
        public int Pos { get; set; }
    }

    /// <summary>
    /// 舞台数据
    /// </summary>
    [ProtoContract]
    public partial class TinyStage : Object
    {
        /// <summary>
        /// 关卡id
        /// </summary>
        [ProtoMember(1)]
        public int LevelId { get; set; }

        /// <summary>
        /// 回合
        /// </summary>
        [ProtoMember(2)]
        public int RoundId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [ProtoMember(3)]
        public int Status { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        [ProtoMember(4)]
        public int Score { get; set; }

        /// <summary>
        /// 可抽卡数
        /// </summary>
        [ProtoMember(5)]
        public int DrawCardNum { get; set; }

        /// <summary>
        /// 轮盘抽卡数量
        /// </summary>
        [ProtoMember(6)]
        public int DrawWheelCardNum { get; set; }

        /// <summary>
        /// 等待选择卡牌
        /// </summary>
        [ProtoMember(7)]
        public List<int> WaitSelectedCards { get; set; } = new();

        /// <summary>
        /// 手牌
        /// </summary>
        [ProtoMember(8)]
        public List<TinyCard> HandCards { get; set; } = new();

        /// <summary>
        /// 已摆放卡牌
        /// </summary>
        [ProtoMember(9)]
        public List<TinyCard> Cards { get; set; } = new();

        /// <summary>
        /// build列表
        /// </summary>
        [ProtoMember(10)]
        public List<int> BuildIds { get; set; } = new();

        /// <summary>
        /// 业务build列表
        /// </summary>
        [ProtoMember(11)]
        public List<int> BusBuildIds { get; set; } = new();
    }

    /// <summary>
    /// 进入tiny通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.EnterTinyNotify)]
    public partial class EnterTinyNotify : Object, IMessage
    {
        /// <summary>
        /// 舞台状态
        /// </summary>
        [ProtoMember(1)]
        public TinyStage Stage { get; set; }
    }

    /// <summary>
    /// 获得build通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyGainBuildNotify)]
    public partial class TinyGainBuildNotify : Object, IMessage
    {
        /// <summary>
        /// 新获得build
        /// </summary>
        [ProtoMember(1)]
        public int BuildId { get; set; }
    }

    /// <summary>
    /// 获得手牌通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyGainHandCardNotify)]
    public partial class TinyGainHandCardNotify : Object, IMessage
    {
        /// <summary>
        /// 手牌
        /// </summary>
        [ProtoMember(1)]
        public TinyCard Card { get; set; }
    }

    /// <summary>
    /// 游戏状态变化通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyChangedNotify)]
    public partial class TinyChangedNotify : Object, IMessage
    {
        /// <summary>
        /// 回合
        /// </summary>
        [ProtoMember(1)]
        public int RoundId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [ProtoMember(2)]
        public int Status { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        [ProtoMember(3)]
        public int Score { get; set; }

        /// <summary>
        /// 可抽卡数
        /// </summary>
        [ProtoMember(4)]
        public int DrawCardNum { get; set; }

        /// <summary>
        /// 轮盘抽卡数量
        /// </summary>
        [ProtoMember(5)]
        public int DrawWheelCardNum { get; set; }
    }

    /// <summary>
    /// 进入黑夜请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyEnterNightReq)]
    [MessageResponseType(ProtoOpcode.TinyEnterNightResp)]
    public partial class TinyEnterNightReq : Object, IMessage
    {
    }

    /// <summary>
    /// 进入黑夜回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyEnterNightResp)]
    public partial class TinyEnterNightResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }
    }

    /// <summary>
    /// 选择卡牌请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinySelectCardReq)]
    [MessageResponseType(ProtoOpcode.TinySelectCardResp)]
    public partial class TinySelectCardReq : Object, IMessage
    {
        /// <summary>
        /// 索引
        /// </summary>
        [ProtoMember(1)]
        public int Idx { get; set; }
    }

    /// <summary>
    /// 选择卡牌回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinySelectCardResp)]
    public partial class TinySelectCardResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }
    }

    /// <summary>
    /// 摆放卡牌请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyPlaceCardReq)]
    [MessageResponseType(ProtoOpcode.TinyPlaceCardResp)]
    public partial class TinyPlaceCardReq : Object, IMessage
    {
        /// <summary>
        /// 卡牌唯一id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [ProtoMember(2)]
        public int Pos { get; set; }
    }

    /// <summary>
    /// 摆放卡牌回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyPlaceCardResp)]
    public partial class TinyPlaceCardResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        /// <summary>
        /// 变化的牌
        /// </summary>
        [ProtoMember(2)]
        public TinyCard Card { get; set; }
    }

    /// <summary>
    /// 移动卡牌请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyMoveCardReq)]
    [MessageResponseType(ProtoOpcode.TinyMoveCardResp)]
    public partial class TinyMoveCardReq : Object, IMessage
    {
        /// <summary>
        /// 卡牌唯一id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [ProtoMember(2)]
        public int Pos { get; set; }
    }

    /// <summary>
    /// 移动卡牌回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyMoveCardResp)]
    public partial class TinyMoveCardResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        /// <summary>
        /// 变化的牌
        /// </summary>
        [ProtoMember(2)]
        public TinyCard Card { get; set; }
    }

    /// <summary>
    /// 交换卡牌请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinySwapCardReq)]
    [MessageResponseType(ProtoOpcode.TinySwapCardResp)]
    public partial class TinySwapCardReq : Object, IMessage
    {
        /// <summary>
        /// 卡牌唯一id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 目标卡牌唯一id
        /// </summary>
        [ProtoMember(2)]
        public int TargetId { get; set; }
    }

    /// <summary>
    /// 交换卡牌回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinySwapCardResp)]
    public partial class TinySwapCardResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        /// <summary>
        /// 变化的牌
        /// </summary>
        [ProtoMember(2)]
        public List<TinyCard> Cards { get; set; } = new();
    }

    /// <summary>
    /// 抽卡请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyDrawCardReq)]
    [MessageResponseType(ProtoOpcode.TinyDrawCardResp)]
    public partial class TinyDrawCardReq : Object, IMessage
    {
    }

    /// <summary>
    /// 抽卡回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyDrawCardResp)]
    public partial class TinyDrawCardResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        /// <summary>
        /// 剩余抽卡数
        /// </summary>
        [ProtoMember(2)]
        public int DrawCardNum { get; set; }

        /// <summary>
        /// 待选择列表
        /// </summary>
        [ProtoMember(3)]
        public List<int> WaitSelectedCards { get; set; } = new();
    }

    /// <summary>
    /// 轮盘抽卡
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.DrawWheelCardReq)]
    [MessageResponseType(ProtoOpcode.DrawWheelCardResp)]
    public partial class DrawWheelCardReq : Object, IMessage
    {
    }

    /// <summary>
    /// 轮盘抽卡
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.DrawWheelCardResp)]
    public partial class DrawWheelCardResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        /// <summary>
        /// 轮盘抽卡数
        /// </summary>
        [ProtoMember(2)]
        public int DrawWheelCardNum { get; set; }

        /// <summary>
        /// 卡池
        /// </summary>
        [ProtoMember(3)]
        public List<int> CardIdList { get; set; } = new();

        /// <summary>
        /// 选中卡id
        /// </summary>
        [ProtoMember(4)]
        public List<int> SelectedCardIdList { get; set; } = new();

        /// <summary>
        /// 变化的牌
        /// </summary>
        [ProtoMember(5)]
        public List<TinyCard> Cards { get; set; } = new();
    }

    // ------------------------------------------------------------
    // 消息id 2051- 2070 为tiny关卡及战斗结算相关
    /// <summary>
    /// 关卡回合奖励领取状态数据
    /// </summary>
    [ProtoContract]
    public partial class LevelRoundRewardInfo : Object
    {
        /// <summary>
        /// 回合条件序号
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 奖励领取状态 LevelRoundRewardStatus
        /// </summary>
        [ProtoMember(2)]
        public int Status { get; set; }
    }

    [ProtoContract]
    public partial class LevelChallengeInfo : Object
    {
        /// <summary>
        /// 关卡id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        [ProtoMember(2)]
        public bool Passed { get; set; }

        /// <summary>
        /// 阶段奖励领取状态列表
        /// </summary>
        [ProtoMember(3)]
        public List<LevelRoundRewardInfo> StatusList { get; set; } = new();
    }

    /// <summary>
    /// 主线关卡挑战列表请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyGetLevelInfoReq)]
    [MessageResponseType(ProtoOpcode.TinyGetLevelInfoResp)]
    public partial class TinyGetLevelInfoReq : Object, IMessage
    {
    }

    /// <summary>
    /// 主线关卡挑战列表回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyGetLevelInfoResp)]
    public partial class TinyGetLevelInfoResp : Object, IMessage
    {
        /// <summary>
        /// 关卡挑战列表
        /// </summary>
        [ProtoMember(1)]
        public List<LevelChallengeInfo> LevelList { get; set; } = new();
    }

    /// <summary>
    /// 领取关卡回合奖励请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyLevelRoundRewardReq)]
    [MessageResponseType(ProtoOpcode.TinyLevelRoundRewardResp)]
    public partial class TinyLevelRoundRewardReq : Object, IMessage
    {
        /// <summary>
        /// 关卡id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 关卡进度奖励配置序号
        /// </summary>
        [ProtoMember(2)]
        public int ProgressIdx { get; set; }
    }

    /// <summary>
    /// 领取关卡回合奖励回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyLevelRoundRewardResp)]
    public partial class TinyLevelRoundRewardResp : Object, IMessage
    {
        /// <summary>
        /// 关卡id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        [ProtoMember(2)]
        public bool Passed { get; set; }

        /// <summary>
        /// 阶段奖励领取状态列表
        /// </summary>
        [ProtoMember(3)]
        public List<LevelRoundRewardInfo> StatusList { get; set; } = new();
    }

    /// <summary>
    /// 进入主线关卡请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyEnterLevelReq)]
    public partial class TinyEnterLevelReq : Object, IMessage
    {
        /// <summary>
        /// 关卡id
        /// </summary>
        [ProtoMember(1)]
        public int LevelId { get; set; }
    }

    /// <summary>
    /// 怪物死亡上报
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyLevelMonsterDeadReq)]
    public partial class TinyLevelMonsterDeadReq : Object, IMessage
    {
        /// <summary>
        /// 生成怪物的唯一id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }
    }

    /// <summary>
    /// 战斗回合结果上报
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyBattleRoundResultReq)]
    public partial class TinyBattleRoundResultReq : Object, IMessage
    {
        /// <summary>
        /// 战斗结果
        /// </summary>
        [ProtoMember(1)]
        public bool BattleResult { get; set; }
    }

    /// <summary>
    /// 主线关卡结算通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.TinyLevelSettleNotify)]
    public partial class TinyLevelSettleNotify : Object, IMessage
    {
        /// <summary>
        /// 战斗结果
        /// </summary>
        [ProtoMember(1)]
        public bool BattleResult { get; set; }

        /// <summary>
        /// 经验值
        /// </summary>
        [ProtoMember(2)]
        public int Exp { get; set; }

        /// <summary>
        /// 奖励道具
        /// </summary>
        [ProtoMember(3)]
        public List<ItemInfo> ItemList { get; set; } = new();
    }
}