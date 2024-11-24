using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // kv-int协议
    // 消息id 1801- 1900
    /// <summary>
    /// kv信息
    /// </summary>
    [ProtoContract]
    public partial class KvintBag : Object
    {
        [ProtoMember(1)]
        public int Uid { get; set; }

        [ProtoMember(2)]
        public List<KvintInfo> Kvdata { get; set; } = new();
    }

    /// <summary>
    /// kv背包变更通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.KvintChangedNotify)]
    public partial class KvintChangedNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public int Uid { get; set; }

        [ProtoMember(2)]
        public List<KvintInfo> Kvdata { get; set; } = new();
    }

    /// <summary>
    /// kv强制同步通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.KvintSyncNotify)]
    public partial class KvintSyncNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public int Uid { get; set; }

        [ProtoMember(2)]
        public List<KvintInfo> Kvdata { get; set; } = new();
    }
}