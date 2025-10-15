using Luban;

namespace Game.Config
{
    public sealed partial class SF_RewardOnTotalRounds : SymbolEffect
    {
        public SF_RewardOnTotalRounds(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            roundCount = buf.ReadInt();
            fixedCoins = buf.ReadInt();
            fixedCoinsForSelfMultipler = buf.ReadInt();
            fixedCoinsForRoundsMultipler = buf.ReadInt();
            fixedMultipler = buf.ReadInt();
            cleanup = buf.ReadBool();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_RewardOnTotalRounds DeserializeSF_RewardOnTotalRounds(ByteBuf buf)
        {
            return new SF_RewardOnTotalRounds(buf);
        }

        /// <summary>
        /// 符号列表
        /// </summary>
        public readonly int targetSymbols;

        /// <summary>
        /// 回合次数
        /// </summary>
        public readonly int roundCount;

        /// <summary>
        /// 固定金币
        /// </summary>
        public readonly int fixedCoins;

        /// <summary>
        /// 自身倍率金币
        /// </summary>
        public readonly int fixedCoinsForSelfMultipler;

        /// <summary>
        /// 回合计数倍率金币
        /// </summary>
        public readonly int fixedCoinsForRoundsMultipler;

        /// <summary>
        /// 固定倍率
        /// </summary>
        public readonly int fixedMultipler;

        /// <summary>
        /// 清除状态
        /// </summary>
        public readonly bool cleanup;

        /// <summary>
        /// 移除目标类型
        /// </summary>
        public readonly SymbolRemoveOperationType opTargetType;

        public const int Id = -967931217;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "roundCount:" + roundCount + ","
            + "fixedCoins:" + fixedCoins + ","
            + "fixedCoinsForSelfMultipler:" + fixedCoinsForSelfMultipler + ","
            + "fixedCoinsForRoundsMultipler:" + fixedCoinsForRoundsMultipler + ","
            + "fixedMultipler:" + fixedMultipler + ","
            + "cleanup:" + cleanup + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}