
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;

namespace Game.Config
{
    public sealed partial class AvatarLevelConfig : BeanBase
    {
        public AvatarLevelConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            nextLevelExp = buf.ReadInt();
            maxEnergy = buf.ReadInt();

            PostInit();
        }

        public static AvatarLevelConfig DeserializeAvatarLevelConfig(ByteBuf buf)
        {
            return new AvatarLevelConfig(buf);
        }

        /// <summary>
        /// 等级
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 升级经验
        /// </summary>
        public readonly int nextLevelExp;

        /// <summary>
        /// 最大体力
        /// </summary>
        public readonly int maxEnergy;

        public const int Id = 1472683181;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "nextLevelExp:" + nextLevelExp + ","
            + "maxEnergy:" + maxEnergy + ","
            + "}";
        }

        partial void PostInit();
    }
}
