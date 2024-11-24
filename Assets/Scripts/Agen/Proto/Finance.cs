using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 金融相关协议
    // 消息id 1601- 1700
    /// <summary>
    /// 货币背包信息
    /// </summary>
    [ProtoContract]
    public partial class WalletInfo : Object
    {
        [ProtoMember(1)]
        public int Uid { get; set; }

        [ProtoMember(2)]
        public List<CurrencyInfo> Currency { get; set; } = new();
    }

    /// <summary>
    /// 货币背包变更通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.WalletChangedNotify)]
    public partial class WalletChangedNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public int Uid { get; set; }

        [ProtoMember(2)]
        public List<CurrencyInfo> Currency { get; set; } = new();
    }

    /// <summary>
    /// 货币背包同步通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.WalletSyncNotify)]
    public partial class WalletSyncNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public int Uid { get; set; }

        [ProtoMember(2)]
        public List<CurrencyInfo> Currency { get; set; } = new();
    }
}