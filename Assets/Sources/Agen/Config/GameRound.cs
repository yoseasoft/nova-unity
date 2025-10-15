using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class GameRoundConfigTable : ConfigSingleton<GameRoundConfigTable>
    {
        private readonly List<GameRoundConfig> _dataList;
        private readonly Dictionary<int, GameRoundConfig> _dataMap;

        public GameRoundConfigTable(ByteBuf buf)
        {
            _dataList = new List<GameRoundConfig>();
            _dataMap = new Dictionary<int, GameRoundConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                GameRoundConfig v;
                v = GameRoundConfig.DeserializeGameRoundConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<GameRoundConfig> DataList => Instance._dataList;

        public static Dictionary<int, GameRoundConfig> DataMap => Instance._dataMap;

        public static GameRoundConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}