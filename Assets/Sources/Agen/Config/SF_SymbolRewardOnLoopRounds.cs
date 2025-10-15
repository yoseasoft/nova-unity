using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class SF_SymbolRewardOnLoopRounds : SymbolEffect
    {
        public SF_SymbolRewardOnLoopRounds(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            roundCount = buf.ReadInt();
            symbolCount = buf.ReadInt();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_SymbolRewardOnLoopRounds DeserializeSF_SymbolRewardOnLoopRounds(ByteBuf buf)
        {
            return new SF_SymbolRewardOnLoopRounds(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 回合次数
        /// </summary>
        public readonly int roundCount;

        /// <summary>
        /// 符号个数
        /// </summary>
        public readonly int symbolCount;

        /// <summary>
        /// 移除目标类型
        /// </summary>
        public readonly SymbolRemoveOperationType opTargetType;

        public const int Id = -1829750597;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "roundCount:" + roundCount + ","
            + "symbolCount:" + symbolCount + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}