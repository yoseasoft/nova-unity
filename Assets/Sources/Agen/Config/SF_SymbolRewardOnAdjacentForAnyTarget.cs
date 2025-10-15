using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class SF_SymbolRewardOnAdjacentForAnyTarget : SymbolEffect
    {
        public SF_SymbolRewardOnAdjacentForAnyTarget(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            rewardCount = buf.ReadInt();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_SymbolRewardOnAdjacentForAnyTarget DeserializeSF_SymbolRewardOnAdjacentForAnyTarget(ByteBuf buf)
        {
            return new SF_SymbolRewardOnAdjacentForAnyTarget(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 奖励符号个数
        /// </summary>
        public readonly int rewardCount;

        /// <summary>
        /// 移除目标类型
        /// </summary>
        public readonly SymbolRemoveOperationType opTargetType;

        public const int Id = 1682651144;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "rewardCount:" + rewardCount + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}