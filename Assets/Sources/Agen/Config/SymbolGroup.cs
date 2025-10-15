using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class SymbolGroupConfigTable : ConfigSingleton<SymbolGroupConfigTable>
    {
        private readonly List<SymbolGroupConfig> _dataList;
        private readonly Dictionary<int, SymbolGroupConfig> _dataMap;

        public SymbolGroupConfigTable(ByteBuf buf)
        {
            _dataList = new List<SymbolGroupConfig>();
            _dataMap = new Dictionary<int, SymbolGroupConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                SymbolGroupConfig v;
                v = SymbolGroupConfig.DeserializeSymbolGroupConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<SymbolGroupConfig> DataList => Instance._dataList;

        public static Dictionary<int, SymbolGroupConfig> DataMap => Instance._dataMap;

        public static SymbolGroupConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}