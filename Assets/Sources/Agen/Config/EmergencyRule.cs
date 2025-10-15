using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class EmergencyRuleConfigTable : ConfigSingleton<EmergencyRuleConfigTable>
    {
        private readonly List<EmergencyRuleConfig> _dataList;
        private readonly Dictionary<int, EmergencyRuleConfig> _dataMap;

        public EmergencyRuleConfigTable(ByteBuf buf)
        {
            _dataList = new List<EmergencyRuleConfig>();
            _dataMap = new Dictionary<int, EmergencyRuleConfig>();

            for (int n = buf.ReadSize(); n > 0; --n)
            {
                EmergencyRuleConfig v;
                v = EmergencyRuleConfig.DeserializeEmergencyRuleConfig(buf);
                _dataList.Add(v);
                _dataMap.Add(v.id, v);
            }

            PostInit();
        }

        public static List<EmergencyRuleConfig> DataList => Instance._dataList;

        public static Dictionary<int, EmergencyRuleConfig> DataMap => Instance._dataMap;

        public static EmergencyRuleConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}