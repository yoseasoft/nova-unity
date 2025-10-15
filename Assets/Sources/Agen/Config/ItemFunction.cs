using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class ItemFunctionConfigTable : ConfigSingleton<ItemFunctionConfigTable>
    {
        private readonly List<ItemFunctionConfig> _dataList;
        private readonly Dictionary<int, ItemFunctionConfig> _dataMap;

        public ItemFunctionConfigTable(ByteBuf buf)
        {
            _dataList = new List<ItemFunctionConfig>();
            _dataMap = new Dictionary<int, ItemFunctionConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                ItemFunctionConfig v;
                v = ItemFunctionConfig.DeserializeItemFunctionConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<ItemFunctionConfig> DataList => Instance._dataList;

        public static Dictionary<int, ItemFunctionConfig> DataMap => Instance._dataMap;

        public static ItemFunctionConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}