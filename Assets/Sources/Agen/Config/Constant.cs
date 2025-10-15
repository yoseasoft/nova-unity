using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    /// <summary>
    /// 常量表
    /// </summary>
    [Config]
    public partial class ConstantConfigTable : ConfigSingleton<ConstantConfigTable>
    {
        private readonly ConstantConfig _data;

        public ConstantConfigTable(ByteBuf buf)
        {
            int n = buf.ReadSize();
            if (n != 1) throw new SerializationException("table mode=one, but size != 1");
            _data = ConstantConfig.DeserializeConstantConfig(buf);
        }

        public static ConstantConfig Data => Instance._data;

        /// <summary>
        /// 版本号
        /// </summary>
        public static string Version => Data.version;

        /// <summary>
        /// 难度关卡列表
        /// </summary>
        public static List<RankLevelList> LevelList => Data.levelList;

        partial void PostInit();
    }
}