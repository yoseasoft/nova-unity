
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
    public partial class DropConfigTable : ConfigSingleton<DropConfigTable>
    {
        private readonly List<DropConfig> _dataList;
        private readonly Dictionary<int, DropConfig> _dataMap;

        public DropConfigTable(ByteBuf buf)
        {
            _dataList = new List<DropConfig>();
            _dataMap = new Dictionary<int, DropConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                DropConfig v;
                v = DropConfig.DeserializeDropConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<DropConfig> DataList => _dataList;

        public Dictionary<int, DropConfig> DataMap => _dataMap;

        public DropConfig Get(int key) => _dataMap[key];

        public DropConfig this[int key] => _dataMap[key];

        public DropConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}