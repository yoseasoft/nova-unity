using Luban;

namespace Game.Config
{
    public sealed partial class SF_RewardAndRemoveSymbolOnProbability : SymbolEffect
    {
        public SF_RewardAndRemoveSymbolOnProbability(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            probabilityValue = buf.ReadInt();
            fixedCoins = buf.ReadInt();
            fixedCoinsForSelfMultipler = buf.ReadInt();
            fixedMultipler = buf.ReadInt();

            PostInit();
        }

        public static SF_RewardAndRemoveSymbolOnProbability DeserializeSF_RewardAndRemoveSymbolOnProbability(ByteBuf buf)
        {
            return new SF_RewardAndRemoveSymbolOnProbability(buf);
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

        public const int Id = 533748030;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "probabilityValue:" + probabilityValue + ","
            + "fixedCoins:" + fixedCoins + ","
            + "fixedCoinsForSelfMultipler:" + fixedCoinsForSelfMultipler + ","
            + "fixedMultipler:" + fixedMultipler + ","
            + "}";
        }

        partial void PostInit();
    }
}