using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class SF_SymbolRewardOnRemoveSymbol : SymbolEffect
    {
        public SF_SymbolRewardOnRemoveSymbol(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            symbolCount = buf.ReadInt();
            spawnInPlace = buf.ReadBool();

            PostInit();
        }

        public static SF_SymbolRewardOnRemoveSymbol DeserializeSF_SymbolRewardOnRemoveSymbol(ByteBuf buf)
        {
            return new SF_SymbolRewardOnRemoveSymbol(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 符号个数
        /// </summary>
        public readonly int symbolCount;

        /// <summary>
        /// 原地孵化状态
        /// </summary>
        public readonly bool spawnInPlace;

        public const int Id = -915494130;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "symbolCount:" + symbolCount + ","
            + "spawnInPlace:" + spawnInPlace + ","
            + "}";
        }

        partial void PostInit();
    }
}