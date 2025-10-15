using Luban;

namespace Game.Config
{
    public sealed partial class SF_RewardOnProbability : SymbolEffect
    {
        public SF_RewardOnProbability(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            probabilityValue = buf.ReadInt();
            probabilityAttrId = buf.ReadInt();
            fixedCoins = buf.ReadInt();
            fixedCoinsForSelfMultipler = buf.ReadInt();
            fixedMultipler = buf.ReadInt();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_RewardOnProbability DeserializeSF_RewardOnProbability(ByteBuf buf)
        {
            return new SF_RewardOnProbability(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly int targetSymbols;

        /// <summary>
        /// 概率值
        /// </summary>
        public readonly int probabilityValue;

        /// <summary>
        /// 概率属性标识
        /// </summary>
        public readonly int probabilityAttrId;

        /// <summary>
        /// 固定金币
        /// </summary>
        public readonly int fixedCoins;

        /// <summary>
        /// 自身倍率金币
        /// </summary>
        public readonly int fixedCoinsForSelfMultipler;

        /// <summary>
        /// 固定倍率
        /// </summary>
        public readonly int fixedMultipler;

        /// <summary>
        /// 移除目标类型
        /// </summary>
        public readonly SymbolRemoveOperationType opTargetType;

        public const int Id = -1489805861;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "probabilityValue:" + probabilityValue + ","
            + "probabilityAttrId:" + probabilityAttrId + ","
            + "fixedCoins:" + fixedCoins + ","
            + "fixedCoinsForSelfMultipler:" + fixedCoinsForSelfMultipler + ","
            + "fixedMultipler:" + fixedMultipler + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}