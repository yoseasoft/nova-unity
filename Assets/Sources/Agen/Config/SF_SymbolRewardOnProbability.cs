using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class SF_SymbolRewardOnProbability : SymbolEffect
    {
        public SF_SymbolRewardOnProbability(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            probabilityValue = buf.ReadInt();
            probabilityAttrId = buf.ReadInt();
            symbolCount = buf.ReadInt();
            spawnInPlace = buf.ReadBool();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_SymbolRewardOnProbability DeserializeSF_SymbolRewardOnProbability(ByteBuf buf)
        {
            return new SF_SymbolRewardOnProbability(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 概率值
        /// </summary>
        public readonly int probabilityValue;

        /// <summary>
        /// 概率属性标识
        /// </summary>
        public readonly int probabilityAttrId;

        /// <summary>
        /// 符号个数
        /// </summary>
        public readonly int symbolCount;

        /// <summary>
        /// 原地孵化状态
        /// </summary>
        public readonly bool spawnInPlace;

        /// <summary>
        /// 移除目标类型
        /// </summary>
        public readonly SymbolRemoveOperationType opTargetType;

        public const int Id = 1339691555;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "probabilityValue:" + probabilityValue + ","
            + "probabilityAttrId:" + probabilityAttrId + ","
            + "symbolCount:" + symbolCount + ","
            + "spawnInPlace:" + spawnInPlace + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}