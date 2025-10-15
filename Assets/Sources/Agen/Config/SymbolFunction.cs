using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class SymbolFunctionConfigTable : ConfigSingleton<SymbolFunctionConfigTable>
    {
        private readonly List<SymbolFunctionConfig> _dataList;
        private readonly Dictionary<int, SymbolFunctionConfig> _dataMap;

        public SymbolFunctionConfigTable(ByteBuf buf)
        {
            _dataList = new List<SymbolFunctionConfig>();
            _dataMap = new Dictionary<int, SymbolFunctionConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                SymbolFunctionConfig v;
                v = SymbolFunctionConfig.DeserializeSymbolFunctionConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<SymbolFunctionConfig> DataList => Instance._dataList;

        public static Dictionary<int, SymbolFunctionConfig> DataMap => Instance._dataMap;

        public static SymbolFunctionConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}