
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
    public sealed partial class BWeightItem : Bonus
    {
        public BWeightItem(ByteBuf buf) : base(buf) 
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);itemList = new System.Collections.Generic.List<DWeightItem>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { DWeightItem _e0;  _e0 = DWeightItem.DeserializeDWeightItem(buf); itemList.Add(_e0);}}

            PostInit();
        }

        public static BWeightItem DeserializeBWeightItem(ByteBuf buf)
        {
            return new BWeightItem(buf);
        }

        /// <summary>
        /// 物品列表
        /// </summary>
        public readonly System.Collections.Generic.List<DWeightItem> itemList;

        public const int Id = -2098845811;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "itemList:" + Luban.StringUtil.CollectionToString(itemList) + ","
            + "}";
        }

        partial void PostInit();
    }
}
