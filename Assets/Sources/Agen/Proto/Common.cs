using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    /// <summary>
    /// 装备数据
    /// </summary>
    [ProtoContract]
    public partial class BagEquipmentData : Object
    {
        [ProtoMember(1)]
        public List<int> ExtAttrIds { get; set; } = new();
    }

    /// <summary>
    /// 背包物品
    /// </summary>
    [ProtoContract]
    public partial class BagItem : Object
    {
        /// <summary>
        /// 物品id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 物品数量
        /// </summary>
        [ProtoMember(2)]
        public int Num { get; set; }

        /// <summary>
        /// 绑定标识
        /// </summary>
        [ProtoMember(3)]
        public bool Bind { get; set; }

        /// <summary>
        /// 多变id
        /// </summary>
        [ProtoMember(4)]
        public int VariedId { get; set; }

        /// <summary>
        /// 来源标识
        /// </summary>
        [ProtoMember(5)]
        public int SourceId { get; set; }

        /// <summary>
        /// 装备数据
        /// </summary>
        [ProtoMember(6)]
        public BagEquipmentData Equip { get; set; }
    }

    /// <summary>
    /// 背包格子
    /// </summary>
    [ProtoContract]
    public partial class BagSlot : Object
    {
        /// <summary>
        /// 标识- 格子位置
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 装载物品
        /// </summary>
        [ProtoMember(2)]
        public BagItem Attach { get; set; }
    }

    /// <summary>
    /// 网络格子物品数据
    /// </summary>
    [ProtoContract]
    public partial class NetSlotItemData : Object
    {
        /// <summary>
        /// 物品id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 物品格子
        /// </summary>
        [ProtoMember(2)]
        public int Slot { get; set; }

        /// <summary>
        /// 物品数量
        /// </summary>
        [ProtoMember(3)]
        public int Num { get; set; }
    }

    /// <summary>
    /// 货币信息
    /// </summary>
    [ProtoContract]
    public partial class CurrencyInfo : Object
    {
        /// <summary>
        /// 货币id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 货币值
        /// </summary>
        [ProtoMember(2)]
        public int Val { get; set; }
    }

    /// <summary>
    /// 键数据
    /// </summary>
    [ProtoContract]
    public partial class KvintInfo : Object
    {
        /// <summary>
        /// key
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [ProtoMember(2)]
        public int Val { get; set; }
    }

    /// <summary>
    /// 多变附属数据
    /// </summary>
    [ProtoContract]
    public partial class VariedAttachInfo : Object
    {
        /// <summary>
        /// 参数列表，根据具体业务进行处理
        /// </summary>
        [ProtoMember(1)]
        public List<int> IntParams { get; set; } = new();

        /// <summary>
        /// 标识参数
        /// </summary>
        [ProtoMember(2)]
        public int FlagParam { get; set; }
    }

    /// <summary>
    /// 多变物品信息
    /// </summary>
    [ProtoContract]
    public partial class VariedItemInfo : Object
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 多变类型 0- 标识清除 *- 其他代表多变类型
        /// </summary>
        [ProtoMember(2)]
        public int VariedType { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        [ProtoMember(3)]
        public VariedAttachInfo Attach { get; set; }
    }

    /// <summary>
    /// 多变使用参数
    /// </summary>
    [ProtoContract]
    public partial class VariedUseParams : Object
    {
        /// <summary>
        /// 使用参数，具体使用看业务需求
        /// </summary>
        [ProtoMember(1)]
        public List<int> IntParams { get; set; } = new();

        /// <summary>
        /// 标识参数
        /// </summary>
        [ProtoMember(2)]
        public int FlagParam { get; set; }
    }

    /// <summary>
    /// 物品信息
    /// </summary>
    [ProtoContract]
    public partial class ItemInfo : Object
    {
        /// <summary>
        /// 物品id
        /// </summary>
        [ProtoMember(1)]
        public int ItemId { get; set; }

        /// <summary>
        /// 物品数量
        /// </summary>
        [ProtoMember(2)]
        public int ItemNum { get; set; }
    }

    /// <summary>
    /// 建筑信息
    /// </summary>
    [ProtoContract]
    public partial class HouseInfo : Object
    {
        /// <summary>
        /// 建筑id
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 建筑等级
        /// </summary>
        [ProtoMember(2)]
        public int Level { get; set; }
    }
}