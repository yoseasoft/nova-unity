using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class SF_SymbolRewardOnAdjacentForTarget : SymbolEffect
    {
        public SF_SymbolRewardOnAdjacentForTarget(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            completeMatch = buf.ReadBool();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);rewardSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); rewardSymbols.Add(_e0);}}
            rewardCount = buf.ReadInt();
            spawnInPlace = buf.ReadBool();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_SymbolRewardOnAdjacentForTarget DeserializeSF_SymbolRewardOnAdjacentForTarget(ByteBuf buf)
        {
            return new SF_SymbolRewardOnAdjacentForTarget(buf);
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
        /// 奖励符号列表
        /// </summary>
        public readonly List<int> rewardSymbols;

        /// <summary>
        /// 奖励符号个数
        /// </summary>
        public readonly int rewardCount;

        /// <summary>
        /// 原地孵化状态
        /// </summary>
        public readonly bool spawnInPlace;

        /// <summary>
        /// 移除目标类型
        /// </summary>
        public readonly SymbolRemoveOperationType opTargetType;

        public const int Id = -1851891706;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "completeMatch:" + completeMatch + ","
            + "rewardSymbols:" + StringUtil.CollectionToString(rewardSymbols) + ","
            + "rewardCount:" + rewardCount + ","
            + "spawnInPlace:" + spawnInPlace + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}