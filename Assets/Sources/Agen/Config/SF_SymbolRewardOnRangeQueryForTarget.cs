using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class SF_SymbolRewardOnRangeQueryForTarget : SymbolEffect
    {
        public SF_SymbolRewardOnRangeQueryForTarget(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            queryType = (SymbolRangeQueryType)buf.ReadInt();
            completeMatch = buf.ReadBool();
            symbolCount = buf.ReadInt();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);rewardSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); rewardSymbols.Add(_e0);}}
            rewardCount = buf.ReadInt();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_SymbolRewardOnRangeQueryForTarget DeserializeSF_SymbolRewardOnRangeQueryForTarget(ByteBuf buf)
        {
            return new SF_SymbolRewardOnRangeQueryForTarget(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 检索类型
        /// </summary>
        public readonly SymbolRangeQueryType queryType;

        /// <summary>
        /// 匹配状态
        /// </summary>
        public readonly bool completeMatch;

        /// <summary>
        /// 符号个数
        /// </summary>
        public readonly int symbolCount;

        /// <summary>
        /// 奖励符号列表
        /// </summary>
        public readonly List<int> rewardSymbols;

        /// <summary>
        /// 奖励符号个数
        /// </summary>
        public readonly int rewardCount;

        /// <summary>
        /// 移除目标类型
        /// </summary>
        public readonly SymbolRemoveOperationType opTargetType;

        public const int Id = -1783162275;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "queryType:" + queryType + ","
            + "completeMatch:" + completeMatch + ","
            + "symbolCount:" + symbolCount + ","
            + "rewardSymbols:" + StringUtil.CollectionToString(rewardSymbols) + ","
            + "rewardCount:" + rewardCount + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}