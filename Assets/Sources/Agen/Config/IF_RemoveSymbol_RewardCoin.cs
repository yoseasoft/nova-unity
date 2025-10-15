using Luban;

namespace Game.Config
{
    public sealed partial class IF_RemoveSymbol_RewardCoin : ItemEffect
    {
        public IF_RemoveSymbol_RewardCoin(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            fixedCoins = buf.ReadInt();
            fixedCoinsForRemoveSymbolCount = buf.ReadInt();

            PostInit();
        }

        public static IF_RemoveSymbol_RewardCoin DeserializeIF_RemoveSymbol_RewardCoin(ByteBuf buf)
        {
            return new IF_RemoveSymbol_RewardCoin(buf);
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
        /// 移除符号计数倍率金币
        /// </summary>
        public readonly int fixedCoinsForRemoveSymbolCount;

        public const int Id = 1918602785;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "fixedCoins:" + fixedCoins + ","
            + "fixedCoinsForRemoveSymbolCount:" + fixedCoinsForRemoveSymbolCount + ","
            + "}";
        }

        partial void PostInit();
    }
}