using Luban;

namespace Game.Config
{
    public sealed partial class IF_Fixed_RewardCoin : ItemEffect
    {
        public IF_Fixed_RewardCoin(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            fixedCoins = buf.ReadInt();

            PostInit();
        }

        public static IF_Fixed_RewardCoin DeserializeIF_Fixed_RewardCoin(ByteBuf buf)
        {
            return new IF_Fixed_RewardCoin(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly int targetSymbols;

        /// <summary>
        /// 固定金币
        /// </summary>
        public readonly int fixedCoins;

        public const int Id = -186157267;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "fixedCoins:" + fixedCoins + ","
            + "}";
        }

        partial void PostInit();
    }
}