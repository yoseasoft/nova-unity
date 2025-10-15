using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class RankLevelList : BeanBase
    {
        public RankLevelList(ByteBuf buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);levelIds = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); levelIds.Add(_e0);}}

            PostInit();
        }

        public static RankLevelList DeserializeRankLevelList(ByteBuf buf)
        {
            return new RankLevelList(buf);
        }

        /// <summary>
        /// 关卡列表
        /// </summary>
        public readonly List<int> levelIds;

        public const int Id = 2046120790;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "levelIds:" + StringUtil.CollectionToString(levelIds) + ","
            + "}";
        }

        partial void PostInit();
    }
}