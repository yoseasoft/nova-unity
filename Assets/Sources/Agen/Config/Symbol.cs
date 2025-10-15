using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class SymbolConfigTable : ConfigSingleton<SymbolConfigTable>
    {
        private readonly List<SymbolConfig> _dataList;
        private readonly Dictionary<int, SymbolConfig> _dataMap;

        public SymbolConfigTable(ByteBuf buf)
        {
            _dataList = new List<SymbolConfig>();
            _dataMap = new Dictionary<int, SymbolConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                SymbolConfig v;
                v = SymbolConfig.DeserializeSymbolConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<SymbolConfig> DataList => Instance._dataList;

        public static Dictionary<int, SymbolConfig> DataMap => Instance._dataMap;

        public static SymbolConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}