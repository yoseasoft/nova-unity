using Luban;

namespace Game.Config
{
    public sealed partial class IF_GlobalBonus_RewardAttributeValue : ItemEffect
    {
        public IF_GlobalBonus_RewardAttributeValue(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            attrId = buf.ReadInt();
            attrValue = buf.ReadInt();

            PostInit();
        }

        public static IF_GlobalBonus_RewardAttributeValue DeserializeIF_GlobalBonus_RewardAttributeValue(ByteBuf buf)
        {
            return new IF_GlobalBonus_RewardAttributeValue(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly int targetSymbols;

        /// <summary>
        /// 目标属性标识
        /// </summary>
        public readonly int attrId;

        /// <summary>
        /// 属性值
        /// </summary>
        public readonly int attrValue;

        public const int Id = 383843977;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "attrId:" + attrId + ","
            + "attrValue:" + attrValue + ","
            + "}";
        }

        partial void PostInit();
    }
}