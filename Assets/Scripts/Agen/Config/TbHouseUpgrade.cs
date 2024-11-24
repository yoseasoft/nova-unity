
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class HouseUpgradeConfigTable : ConfigSingleton<HouseUpgradeConfigTable>
    {
        private readonly List<HouseUpgradeConfig> _dataList;
        private readonly Dictionary<int, HouseUpgradeConfig> _dataMap;

        public HouseUpgradeConfigTable(ByteBuf buf)
        {
            _dataList = new List<HouseUpgradeConfig>();
            _dataMap = new Dictionary<int, HouseUpgradeConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                HouseUpgradeConfig v;
                v = HouseUpgradeConfig.DeserializeHouseUpgradeConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<HouseUpgradeConfig> DataList => _dataList;

        public Dictionary<int, HouseUpgradeConfig> DataMap => _dataMap;

        public HouseUpgradeConfig Get(int key) => _dataMap[key];

        public HouseUpgradeConfig this[int key] => _dataMap[key];

        public HouseUpgradeConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}
