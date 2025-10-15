using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class ItemConfigTable : ConfigSingleton<ItemConfigTable>
    {
        private readonly List<ItemConfig> _dataList;
        private readonly Dictionary<int, ItemConfig> _dataMap;

        public ItemConfigTable(ByteBuf buf)
        {
            _dataList = new List<ItemConfig>();
            _dataMap = new Dictionary<int, ItemConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                ItemConfig v;
                v = ItemConfig.DeserializeItemConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<ItemConfig> DataList => Instance._dataList;

        public static Dictionary<int, ItemConfig> DataMap => Instance._dataMap;

        public static ItemConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}