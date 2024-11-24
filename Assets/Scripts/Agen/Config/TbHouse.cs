
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
    public partial class HouseConfigTable : ConfigSingleton<HouseConfigTable>
    {
        private readonly List<HouseConfig> _dataList;
        private readonly Dictionary<int, HouseConfig> _dataMap;

        public HouseConfigTable(ByteBuf buf)
        {
            _dataList = new List<HouseConfig>();
            _dataMap = new Dictionary<int, HouseConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                HouseConfig v;
                v = HouseConfig.DeserializeHouseConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<HouseConfig> DataList => _dataList;

        public Dictionary<int, HouseConfig> DataMap => _dataMap;

        public HouseConfig Get(int key) => _dataMap[key];

        public HouseConfig this[int key] => _dataMap[key];

        public HouseConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}