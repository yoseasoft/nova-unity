using Luban;

namespace Game.Config
{
    public sealed partial class SF_RewardOnAdjacentForAnyTarget : SymbolEffect
    {
        public SF_RewardOnAdjacentForAnyTarget(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            fixedCoins = buf.ReadInt();
            fixedCoinsForSelfMultipler = buf.ReadInt();
            fixedCoinsForTargetMultipler = buf.ReadInt();
            fixedMultipler = buf.ReadInt();
            cleanup = buf.ReadBool();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_RewardOnAdjacentForAnyTarget DeserializeSF_RewardOnAdjacentForAnyTarget(ByteBuf buf)
        {
            return new SF_RewardOnAdjacentForAnyTarget(buf);
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
        /// 目标倍率金币
        /// </summary>
        public readonly int fixedCoinsForTargetMultipler;

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

        public const int Id = 1509923664;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "fixedCoins:" + fixedCoins + ","
            + "fixedCoinsForSelfMultipler:" + fixedCoinsForSelfMultipler + ","
            + "fixedCoinsForTargetMultipler:" + fixedCoinsForTargetMultipler + ","
            + "fixedMultipler:" + fixedMultipler + ","
            + "cleanup:" + cleanup + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}