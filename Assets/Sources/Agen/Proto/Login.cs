using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 消息id 1- 100 为预留
    // 消息id 101- 200 为账号流程
    // -------------------------------------------------------- 账号流程 -------------------------------------------------------
    /// <summary>
    /// 获取角色列表信息
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GetPlayerListReq)]
    [MessageResponseType(ProtoOpcode.GetPlayerListResp)]
    public partial class GetPlayerListReq : Object, IMessage
    {
    }

    /// <summary>
    /// 获取账号信息回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GetPlayerListResp)]
    public partial class GetPlayerListResp : Object, IMessage
    {
        [ProtoMember(1)]
        public ulong Uid { get; set; }
    }

    /// <summary>
    /// 创建玩家
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.CreatePlayerReq)]
    [MessageResponseType(ProtoOpcode.CreatePlayerResp)]
    public partial class CreatePlayerReq : Object, IMessage
    {
    }

    /// <summary>
    /// 创建玩家回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.CreatePlayerResp)]
    public partial class CreatePlayerResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        [ProtoMember(2)]
        public ulong Uid { get; set; }
    }

    /// <summary>
    /// 基础信息
    /// </summary>
    [ProtoContract]
    public partial class BaseInfo : Object
    {
        [ProtoMember(1)]
        public ulong Uid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ProtoMember(2)]
        public int CreateTime { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        [ProtoMember(3)]
        public int LoginTime { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [ProtoMember(4)]
        public string Name { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [ProtoMember(5)]
        public string Signature { get; set; }

        /// <summary>
        /// 已开启功能
        /// </summary>
        [ProtoMember(8)]
        public List<int> FuncOpenIdList { get; set; } = new();
    }

    /// <summary>
    /// 主状态信息
    /// </summary>
    [ProtoContract]
    public partial class MainStatInfo : Object
    {
        /// <summary>
        /// 等级
        /// </summary>
        [ProtoMember(1)]
        public int Level { get; set; }

        /// <summary>
        /// 经验
        /// </summary>
        [ProtoMember(2)]
        public int Exp { get; set; }

        /// <summary>
        /// 体力
        /// </summary>
        [ProtoMember(3)]
        public int Energy { get; set; }

        /// <summary>
        /// 上次体力恢复时间
        /// </summary>
        [ProtoMember(4)]
        public int EnergyTime { get; set; }
    }

    /// <summary>
    /// 挂机信息
    /// </summary>
    [ProtoContract]
    public partial class IdleStatInfo : Object
    {
        /// <summary>
        /// 挂机上次奖励计时开始时间
        /// </summary>
        [ProtoMember(1)]
        public int LastRewardTime { get; set; }

        /// <summary>
        /// 快速巡逻领取次数
        /// </summary>
        [ProtoMember(2)]
        public int QuickPassRewardCount { get; set; }

        /// <summary>
        /// 广告巡逻领取次数
        /// </summary>
        [ProtoMember(3)]
        public int AdPassRewardCount { get; set; }
    }

    /// <summary>
    /// 玩家信息
    /// </summary>
    [ProtoContract]
    public partial class PlayerInfo : Object
    {
        [ProtoMember(1)]
        public BaseInfo Basic { get; set; }

        [ProtoMember(2)]
        public BagInfo Bag { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        [ProtoMember(3)]
        public WalletInfo Wallet { get; set; }

        /// <summary>
        /// kv包
        /// </summary>
        [ProtoMember(4)]
        public KvintBag KvBag { get; set; }

        /// <summary>
        /// 主状态
        /// </summary>
        [ProtoMember(5)]
        public MainStatInfo MainStat { get; set; }

        /// <summary>
        /// 装备列表
        /// </summary>
        [ProtoMember(6)]
        public List<EquipmentInfo> EquipmentList { get; set; } = new();

        /// <summary>
        /// 挂机信息
        /// </summary>
        [ProtoMember(7)]
        public IdleStatInfo IdleStat { get; set; }
    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.EnterWorldReq)]
    [MessageResponseType(ProtoOpcode.EnterWorldResp)]
    public partial class EnterWorldReq : Object, IMessage
    {
        [ProtoMember(1)]
        public ulong Uid { get; set; }
    }

    /// <summary>
    /// 进入游戏回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.EnterWorldResp)]
    public partial class EnterWorldResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }
    }

    [ProtoContract]
    [Message(ProtoOpcode.ExitWorldReq)]
    [MessageResponseType(ProtoOpcode.ExitWorldResp)]
    public partial class ExitWorldReq : Object, IMessage
    {
    }

    [ProtoContract]
    [Message(ProtoOpcode.ExitWorldResp)]
    public partial class ExitWorldResp : Object, IMessage
    {
    }

    /// <summary>
    /// 弱连接请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.WeakReconnectReq)]
    [MessageResponseType(ProtoOpcode.WeakReconnectResp)]
    public partial class WeakReconnectReq : Object, IMessage
    {
    }

    /// <summary>
    /// 弱连接回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.WeakReconnectResp)]
    public partial class WeakReconnectResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Code { get; set; }
    }

    /// <summary>
    /// 玩家信息同步
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.PlayerInfoSyncNotify)]
    public partial class PlayerInfoSyncNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public PlayerInfo Player { get; set; }
    }

    /// <summary>
    /// 请求服务器时间
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.GetServerTimeReq)]
    [MessageResponseType(ProtoOpcode.GetServerTimeResp)]
    public partial class GetServerTimeReq : Object, IMessage
    {
    }

    [ProtoContract]
    [Message(ProtoOpcode.GetServerTimeResp)]
    public partial class GetServerTimeResp : Object, IMessage
    {
        [ProtoMember(1)]
        public ulong ServerTime { get; set; }
    }

    /// <summary>
    /// 玩家经验变动
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.PlayerExpChangedNotify)]
    public partial class PlayerExpChangedNotify : Object, IMessage
    {
        /// <summary>
        /// 等级
        /// </summary>
        [ProtoMember(1)]
        public int Level { get; set; }

        /// <summary>
        /// 经验
        /// </summary>
        [ProtoMember(2)]
        public int Exp { get; set; }
    }

    /// <summary>
    /// 功能开启通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.FuncOpenChangedNotify)]
    public partial class FuncOpenChangedNotify : Object, IMessage
    {
        /// <summary>
        /// 功能id
        /// </summary>
        [ProtoMember(1)]
        public int FuncId { get; set; }
    }
}