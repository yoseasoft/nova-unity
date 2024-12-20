
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
    public sealed partial class DProbItem : BeanBase
    {
        public DProbItem(ByteBuf buf)
        {
            itemId = buf.ReadInt();
            itemNum = buf.ReadInt();
            rate = buf.ReadInt();

            PostInit();
        }

        public static DProbItem DeserializeDProbItem(ByteBuf buf)
        {
            return new DProbItem(buf);
        }

        /// <summary>
        /// 物品id
        /// </summary>
        public readonly int itemId;

        /// <summary>
        /// 物品数量
        /// </summary>
        public readonly int itemNum;

        /// <summary>
        /// 概率
        /// </summary>
        public readonly int rate;

        public const int Id = 704888940;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "itemId:" + itemId + ","
            + "itemNum:" + itemNum + ","
            + "rate:" + rate + ","
            + "}";
        }

        partial void PostInit();
    }
}
