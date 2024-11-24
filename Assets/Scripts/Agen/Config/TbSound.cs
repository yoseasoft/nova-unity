
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
    public partial class SoundConfigTable : ConfigSingleton<SoundConfigTable>
    {
        private readonly List<SoundConfig> _dataList;
        private readonly Dictionary<int, SoundConfig> _dataMap;

        public SoundConfigTable(ByteBuf buf)
        {
            _dataList = new List<SoundConfig>();
            _dataMap = new Dictionary<int, SoundConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                SoundConfig v;
                v = SoundConfig.DeserializeSoundConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public List<SoundConfig> DataList => _dataList;

        public Dictionary<int, SoundConfig> DataMap => _dataMap;

        public SoundConfig Get(int key) => _dataMap[key];

        public SoundConfig this[int key] => _dataMap[key];

        public SoundConfig GetOrDefault(int key) => _dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}