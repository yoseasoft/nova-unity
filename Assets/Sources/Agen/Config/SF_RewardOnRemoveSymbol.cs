using Luban;

namespace Game.Config
{
    public sealed partial class SF_RewardOnRemoveSymbol : SymbolEffect
    {
        public SF_RewardOnRemoveSymbol(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            fixedCoins = buf.ReadInt();
            fixedCoinsForSelfMultipler = buf.ReadInt();
            fixedCoinsForTotalRoundsMultipler = buf.ReadInt();
            fixedMultipler = buf.ReadInt();

            PostInit();
        }

        public static SF_RewardOnRemoveSymbol DeserializeSF_RewardOnRemoveSymbol(ByteBuf buf)
        {
            return new SF_RewardOnRemoveSymbol(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly int targetSymbols;

        /// <summary>
        /// 固定金币
        /// </summary>
        public readonly int fixedCoins;

        /// <summary>
        /// 自身倍率金币
        /// </summary>
        public readonly int fixedCoinsForSelfMultipler;

        /// <summary>
        /// 总回合计数倍率金币
        /// </summary>
        public readonly int fixedCoinsForTotalRoundsMultipler;

        /// <summary>
        /// 固定倍率
        /// </summary>
        public readonly int fixedMultipler;

        public const int Id = 1564399190;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "fixedCoins:" + fixedCoins + ","
            + "fixedCoinsForSelfMultipler:" + fixedCoinsForSelfMultipler + ","
            + "fixedCoinsForTotalRoundsMultipler:" + fixedCoinsForTotalRoundsMultipler + ","
            + "fixedMultipler:" + fixedMultipler + ","
            + "}";
        }

        partial void PostInit();
    }
}