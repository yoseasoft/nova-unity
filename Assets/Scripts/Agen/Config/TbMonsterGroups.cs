
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
    public partial class MonsterGroupsConfigTable : ConfigSingleton<MonsterGroupsConfigTable>
    {
        private readonly List<MonsterGroupsConfig> _dataList;
        private readonly Dictionary<int, MonsterGroupsConfig> _dataMap;

        public MonsterGroupsConfigTable(ByteBuf buf)
        {
            _dataList = new List<MonsterGroupsConfig>();
            _dataMap = new Dictionary<int, MonsterGroupsConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                MonsterGroupsConfig v;
                v = MonsterGroupsConfig.DeserializeMonsterGroupsConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<MonsterGroupsConfig> DataList => _dataList;

        public Dictionary<int, MonsterGroupsConfig> DataMap => _dataMap;

        public MonsterGroupsConfig Get(int key) => _dataMap[key];

        public MonsterGroupsConfig this[int key] => _dataMap[key];

        public MonsterGroupsConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}
