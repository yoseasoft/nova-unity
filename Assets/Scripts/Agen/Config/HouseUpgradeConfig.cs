
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;

namespace Game.Config
{
    public sealed partial class HouseUpgradeConfig : BeanBase
    {
        public HouseUpgradeConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            level = buf.ReadInt();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);costItems = new System.Collections.Generic.List<DItem>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { DItem _e0;  _e0 = DItem.DeserializeDItem(buf); costItems.Add(_e0);}}
            attrId = buf.ReadInt();
            logicId = buf.ReadInt();

            PostInit();
        }

        public static HouseUpgradeConfig DeserializeHouseUpgradeConfig(ByteBuf buf)
        {
            return new HouseUpgradeConfig(buf);
        }

        /// <summary>
        /// 房屋等级序号
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 等级
        /// </summary>
        public readonly int level;

        /// <summary>
        /// 升级消耗
        /// </summary>
        public readonly System.Collections.Generic.List<DItem> costItems;

        /// <summary>
        /// 属性id
        /// </summary>
        public readonly int attrId;

        /// <summary>
        /// 被动id
        /// </summary>
        public readonly int logicId;

        public const int Id = 767147358;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "level:" + level + ","
            + "costItems:" + Luban.StringUtil.CollectionToString(costItems) + ","
            + "attrId:" + attrId + ","
            + "logicId:" + logicId + ","
            + "}";
        }

        partial void PostInit();
    }
}
