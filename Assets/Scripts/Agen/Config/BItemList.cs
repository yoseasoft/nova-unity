
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
    public sealed partial class BItemList : Bonus
    {
        public BItemList(ByteBuf buf) : base(buf) 
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);itemList = new System.Collections.Generic.List<DItem>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { DItem _e0;  _e0 = DItem.DeserializeDItem(buf); itemList.Add(_e0);}}

            PostInit();
        }

        public static BItemList DeserializeBItemList(ByteBuf buf)
        {
            return new BItemList(buf);
        }

        /// <summary>
        /// 物品列表
        /// </summary>
        public readonly System.Collections.Generic.List<DItem> itemList;

        public const int Id = -2085953357;

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