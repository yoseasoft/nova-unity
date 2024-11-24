
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
    public partial class MonsterRoundsConfigTable : ConfigSingleton<MonsterRoundsConfigTable>
    {
        private readonly List<MonsterRoundsConfig> _dataList;
        private readonly Dictionary<int, MonsterRoundsConfig> _dataMap;

        public MonsterRoundsConfigTable(ByteBuf buf)
        {
            _dataList = new List<MonsterRoundsConfig>();
            _dataMap = new Dictionary<int, MonsterRoundsConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                MonsterRoundsConfig v;
                v = MonsterRoundsConfig.DeserializeMonsterRoundsConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<MonsterRoundsConfig> DataList => _dataList;

        public Dictionary<int, MonsterRoundsConfig> DataMap => _dataMap;

        public MonsterRoundsConfig Get(int key) => _dataMap[key];

        public MonsterRoundsConfig this[int key] => _dataMap[key];

        public MonsterRoundsConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}