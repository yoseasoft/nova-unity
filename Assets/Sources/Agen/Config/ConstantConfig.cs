using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    /// <summary>
    /// 常量配置
    /// </summary>
    public sealed partial class ConstantConfig : BeanBase
    {
        public ConstantConfig(ByteBuf buf)
        {
            version = buf.ReadString();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);levelList = new List<RankLevelList>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { RankLevelList _e0;  _e0 = RankLevelList.DeserializeRankLevelList(buf); levelList.Add(_e0);}}

            PostInit();
        }

        public static ConstantConfig DeserializeConstantConfig(ByteBuf buf)
        {
            return new ConstantConfig(buf);
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public readonly string version;

        /// <summary>
        /// 难度关卡列表
        /// </summary>
        public readonly List<RankLevelList> levelList;

        public const int Id = -1411722202;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "version:" + version + ","
            + "levelList:" + StringUtil.CollectionToString(levelList) + ","
            + "}";
        }

        partial void PostInit();
    }
}