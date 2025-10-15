using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    /// <summary>
    /// 符号合成配置
    /// </summary>
    public sealed partial class SymbolCombineConfig : BeanBase
    {
        public SymbolCombineConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            priority = buf.ReadInt();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);sourceSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); sourceSymbols.Add(_e0);}}
            resultSymbol = buf.ReadInt();

            PostInit();
        }

        public static SymbolCombineConfig DeserializeSymbolCombineConfig(ByteBuf buf)
        {
            return new SymbolCombineConfig(buf);
        }

        /// <summary>
        /// 合成标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 合成优先级
        /// </summary>
        public readonly int priority;

        /// <summary>
        /// 来源符号列表
        /// </summary>
        public readonly List<int> sourceSymbols;

        /// <summary>
        /// 生成符号
        /// </summary>
        public readonly int resultSymbol;

        /// <summary>
        /// 生成符号
        /// </summary>
        public SymbolConfig ResultSymbolConfig => SymbolConfigTable.Get(resultSymbol);

        public const int Id = -13919607;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "priority:" + priority + ","
            + "sourceSymbols:" + StringUtil.CollectionToString(sourceSymbols) + ","
            + "resultSymbol:" + resultSymbol + ","
            + "}";
        }

        partial void PostInit();
    }
}