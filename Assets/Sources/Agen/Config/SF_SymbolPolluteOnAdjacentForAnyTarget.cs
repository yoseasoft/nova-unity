using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class SF_SymbolPolluteOnAdjacentForAnyTarget : SymbolEffect
    {
        public SF_SymbolPolluteOnAdjacentForAnyTarget(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            qualityFilter = buf.ReadBool();
            qualityType = (QualityType)buf.ReadInt();

            PostInit();
        }

        public static SF_SymbolPolluteOnAdjacentForAnyTarget DeserializeSF_SymbolPolluteOnAdjacentForAnyTarget(ByteBuf buf)
        {
            return new SF_SymbolPolluteOnAdjacentForAnyTarget(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 品质过滤状态
        /// </summary>
        public readonly bool qualityFilter;

        /// <summary>
        /// 品质类型
        /// </summary>
        public readonly QualityType qualityType;

        public const int Id = -1330515368;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "qualityFilter:" + qualityFilter + ","
            + "qualityType:" + qualityType + ","
            + "}";
        }

        partial void PostInit();
    }
}