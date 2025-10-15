using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class IF_Fixed_RewardSymbol : ItemEffect
    {
        public IF_Fixed_RewardSymbol(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            symbolCount = buf.ReadInt();

            PostInit();
        }

        public static IF_Fixed_RewardSymbol DeserializeIF_Fixed_RewardSymbol(ByteBuf buf)
        {
            return new IF_Fixed_RewardSymbol(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 符号个数
        /// </summary>
        public readonly int symbolCount;

        public const int Id = 1958905652;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "symbolCount:" + symbolCount + ","
            + "}";
        }

        partial void PostInit();
    }
}