using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 建筑相关协议
    // 消息id 2101- 2150
    /// <summary>
    /// 建筑列表请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.HouseInfoReq)]
    [MessageResponseType(ProtoOpcode.HouseInfoResp)]
    public partial class HouseInfoReq : Object, IMessage
    {
    }

    /// <summary>
    /// 建筑列表返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.HouseInfoResp)]
    public partial class HouseInfoResp : Object, IMessage
    {
        [ProtoMember(1)]
        public List<HouseInfo> HouseList { get; set; } = new();
    }

    /// <summary>
    /// 建筑升级请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.HouseUpgradeReq)]
    [MessageResponseType(ProtoOpcode.HouseUpgradeResp)]
    public partial class HouseUpgradeReq : Object, IMessage
    {
        [ProtoMember(1)]
        public int Id { get; set; }
    }

    /// <summary>
    /// 建筑升级返回
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.HouseUpgradeResp)]
    public partial class HouseUpgradeResp : Object, IMessage
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public int Level { get; set; }
    }

    /// <summary>
    /// 建筑解锁通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.HouseUnlockNotify)]
    public partial class HouseUnlockNotify : Object, IMessage
    {
        [ProtoMember(1)]
        public List<HouseInfo> HouseList { get; set; } = new();
    }
}