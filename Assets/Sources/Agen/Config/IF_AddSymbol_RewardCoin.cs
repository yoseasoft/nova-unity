using Luban;

namespace Game.Config
{
    public sealed partial class IF_AddSymbol_RewardCoin : ItemEffect
    {
        public IF_AddSymbol_RewardCoin(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            fixedCoins = buf.ReadInt();
            fixedCoinsForAddSymbolCount = buf.ReadInt();

            PostInit();
        }

        public static IF_AddSymbol_RewardCoin DeserializeIF_AddSymbol_RewardCoin(ByteBuf buf)
        {
            return new IF_AddSymbol_RewardCoin(buf);
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
        /// 新增符号计数倍率金币
        /// </summary>
        public readonly int fixedCoinsForAddSymbolCount;

        public const int Id = -777316632;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "fixedCoins:" + fixedCoins + ","
            + "fixedCoinsForAddSymbolCount:" + fixedCoinsForAddSymbolCount + ","
            + "}";
        }

        partial void PostInit();
    }
}