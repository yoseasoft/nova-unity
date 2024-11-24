
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
    public sealed partial class DropConfig : BeanBase
    {
        public DropConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            name = buf.ReadString();
            clientShowItems = Bonus.DeserializeBonus(buf);
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);commonDrop = new System.Collections.Generic.List<DItem>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { DItem _e0;  _e0 = DItem.DeserializeDItem(buf); commonDrop.Add(_e0);}}
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);randomDrop = new System.Collections.Generic.List<DProbItem>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { DProbItem _e0;  _e0 = DProbItem.DeserializeDProbItem(buf); randomDrop.Add(_e0);}}
            dropItems = Bonus.DeserializeBonus(buf);

            PostInit();
        }

        public static DropConfig DeserializeDropConfig(ByteBuf buf)
        {
            return new DropConfig(buf);
        }

        /// <summary>
        /// 掉落id
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 名称
        /// </summary>
        public readonly string name;

        /// <summary>
        /// 客户端显示物品
        /// </summary>
        public readonly Bonus clientShowItems;

        /// <summary>
        /// 必定掉落
        /// </summary>
        public readonly System.Collections.Generic.List<DItem> commonDrop;

        /// <summary>
        /// 必定掉落
        /// </summary>
        public readonly System.Collections.Generic.List<DProbItem> randomDrop;

        /// <summary>
        /// 掉落
        /// </summary>
        public readonly Bonus dropItems;

        public const int Id = -1845736655;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "name:" + name + ","
            + "clientShowItems:" + clientShowItems + ","
            + "commonDrop:" + Luban.StringUtil.CollectionToString(commonDrop) + ","
            + "randomDrop:" + Luban.StringUtil.CollectionToString(randomDrop) + ","
            + "dropItems:" + dropItems + ","
            + "}";
        }

        partial void PostInit();
    }
}