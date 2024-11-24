
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
    public partial class SoldierConfigTable : ConfigSingleton<SoldierConfigTable>
    {
        private readonly List<SoldierConfig> _dataList;
        private readonly Dictionary<int, SoldierConfig> _dataMap;

        public SoldierConfigTable(ByteBuf buf)
        {
            _dataList = new List<SoldierConfig>();
            _dataMap = new Dictionary<int, SoldierConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                SoldierConfig v;
                v = SoldierConfig.DeserializeSoldierConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<SoldierConfig> DataList => _dataList;

        public Dictionary<int, SoldierConfig> DataMap => _dataMap;

        public SoldierConfig Get(int key) => _dataMap[key];

        public SoldierConfig this[int key] => _dataMap[key];

        public SoldierConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}
