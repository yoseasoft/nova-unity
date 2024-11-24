
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
    public partial class BuildCardConfigTable : ConfigSingleton<BuildCardConfigTable>
    {
        private readonly List<BuildCardConfig> _dataList;
        private readonly Dictionary<int, BuildCardConfig> _dataMap;

        public BuildCardConfigTable(ByteBuf buf)
        {
            _dataList = new List<BuildCardConfig>();
            _dataMap = new Dictionary<int, BuildCardConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                BuildCardConfig v;
                v = BuildCardConfig.DeserializeBuildCardConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<BuildCardConfig> DataList => _dataList;

        public Dictionary<int, BuildCardConfig> DataMap => _dataMap;

        public BuildCardConfig Get(int key) => _dataMap[key];

        public BuildCardConfig this[int key] => _dataMap[key];

        public BuildCardConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}
