using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class LevelConfigTable : ConfigSingleton<LevelConfigTable>
    {
        private readonly List<LevelConfig> _dataList;
        private readonly Dictionary<int, LevelConfig> _dataMap;

        public LevelConfigTable(ByteBuf buf)
        {
            _dataList = new List<LevelConfig>();
            _dataMap = new Dictionary<int, LevelConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                LevelConfig v;
                v = LevelConfig.DeserializeLevelConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<LevelConfig> DataList => Instance._dataList;

        public static Dictionary<int, LevelConfig> DataMap => Instance._dataMap;

        public static LevelConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}