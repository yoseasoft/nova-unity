using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    /// <summary>
    /// 资源配置表
    /// </summary>
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

        public static List<ResourceConfig> DataList => Instance._dataList;

        public static Dictionary<int, ResourceConfig> DataMap => Instance._dataMap;

        public static ResourceConfig Get(int key) => Instance._dataMap.GetValueOrDefault(key);

        partial void PostInit();
    }
}