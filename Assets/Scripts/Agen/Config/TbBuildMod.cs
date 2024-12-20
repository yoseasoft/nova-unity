
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
    public partial class BuildModConfigTable : ConfigSingleton<BuildModConfigTable>
    {
        private readonly List<BuildModConfig> _dataList;
        private readonly Dictionary<int, BuildModConfig> _dataMap;

        public BuildModConfigTable(ByteBuf buf)
        {
            _dataList = new List<BuildModConfig>();
            _dataMap = new Dictionary<int, BuildModConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                BuildModConfig v;
                v = BuildModConfig.DeserializeBuildModConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<BuildModConfig> DataList => _dataList;

        public Dictionary<int, BuildModConfig> DataMap => _dataMap;

        public BuildModConfig Get(int key) => _dataMap[key];

        public BuildModConfig this[int key] => _dataMap[key];

        public BuildModConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}
