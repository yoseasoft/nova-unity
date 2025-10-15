using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    /// <summary>
    /// 符号组合配置
    /// </summary>
    public sealed partial class SymbolGroupConfig : BeanBase
    {
        public SymbolGroupConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);symbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); symbols.Add(_e0);}}

            PostInit();
        }

        public static SymbolGroupConfig DeserializeSymbolGroupConfig(ByteBuf buf)
        {
            return new SymbolGroupConfig(buf);
        }

        /// <summary>
        /// 组合标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 符号列表
        /// </summary>
        public readonly List<int> symbols;

        public const int Id = -1657811511;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "symbols:" + StringUtil.CollectionToString(symbols) + ","
            + "}";
        }

        partial void PostInit();
    }
}