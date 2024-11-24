
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
    public partial class ResourceConfigTable : ConfigSingleton<ResourceConfigTable>
    {
        private readonly List<ResourceConfig> _dataList;
        private readonly Dictionary<int, ResourceConfig> _dataMap;

        public ResourceConfigTable(ByteBuf buf)
        {
            _dataList = new List<ResourceConfig>();
            _dataMap = new Dictionary<int, ResourceConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                ResourceConfig v;
                v = ResourceConfig.DeserializeResourceConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<ResourceConfig> DataList => _dataList;

        public Dictionary<int, ResourceConfig> DataMap => _dataMap;

        public ResourceConfig Get(int key) => _dataMap[key];

        public ResourceConfig this[int key] => _dataMap[key];

        public ResourceConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}