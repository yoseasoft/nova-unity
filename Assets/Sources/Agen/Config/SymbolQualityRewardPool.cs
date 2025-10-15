using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class SymbolQualityRewardPoolConfigTable : ConfigSingleton<SymbolQualityRewardPoolConfigTable>
    {
        private readonly List<SymbolQualityRewardPoolConfig> _dataList;
        private readonly Dictionary<int, SymbolQualityRewardPoolConfig> _dataMap;

        public SymbolQualityRewardPoolConfigTable(ByteBuf buf)
        {
            _dataList = new List<SymbolQualityRewardPoolConfig>();
            _dataMap = new Dictionary<int, SymbolQualityRewardPoolConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                SymbolQualityRewardPoolConfig v;
                v = SymbolQualityRewardPoolConfig.DeserializeSymbolQualityRewardPoolConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<SymbolQualityRewardPoolConfig> DataList => Instance._dataList;

        public static Dictionary<int, SymbolQualityRewardPoolConfig> DataMap => Instance._dataMap;

        public static SymbolQualityRewardPoolConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}