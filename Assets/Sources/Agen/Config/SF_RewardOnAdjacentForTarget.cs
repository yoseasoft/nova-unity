using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class SF_RewardOnAdjacentForTarget : SymbolEffect
    {
        public SF_RewardOnAdjacentForTarget(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            completeMatch = buf.ReadBool();
            fixedCoins = buf.ReadInt();
            fixedCoinsForSelfMultipler = buf.ReadInt();
            fixedCoinsForTargetMultipler = buf.ReadInt();
            fixedMultipler = buf.ReadInt();
            cleanup = buf.ReadBool();
            opRewardType = (SymbolRewardOperationType)buf.ReadInt();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_RewardOnAdjacentForTarget DeserializeSF_RewardOnAdjacentForTarget(ByteBuf buf)
        {
            return new SF_RewardOnAdjacentForTarget(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 匹配状态
        /// </summary>
        public readonly bool completeMatch;

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
        /// 奖励目标类型
        /// </summary>
        public readonly SymbolRewardOperationType opRewardType;

        /// <summary>
        /// 移除目标类型
        /// </summary>
        public readonly SymbolRemoveOperationType opTargetType;

        public const int Id = -1156709954;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "completeMatch:" + completeMatch + ","
            + "fixedCoins:" + fixedCoins + ","
            + "fixedCoinsForSelfMultipler:" + fixedCoinsForSelfMultipler + ","
            + "fixedCoinsForTargetMultipler:" + fixedCoinsForTargetMultipler + ","
            + "fixedMultipler:" + fixedMultipler + ","
            + "cleanup:" + cleanup + ","
            + "opRewardType:" + opRewardType + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}