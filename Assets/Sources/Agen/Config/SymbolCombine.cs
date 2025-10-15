using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class SymbolCombineConfigTable : ConfigSingleton<SymbolCombineConfigTable>
    {
        private readonly List<SymbolCombineConfig> _dataList;
        private readonly Dictionary<int, SymbolCombineConfig> _dataMap;

        public SymbolCombineConfigTable(ByteBuf buf)
        {
            _dataList = new List<SymbolCombineConfig>();
            _dataMap = new Dictionary<int, SymbolCombineConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                SymbolCombineConfig v;
                v = SymbolCombineConfig.DeserializeSymbolCombineConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<SymbolCombineConfig> DataList => Instance._dataList;

        public static Dictionary<int, SymbolCombineConfig> DataMap => Instance._dataMap;

        public static SymbolCombineConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}