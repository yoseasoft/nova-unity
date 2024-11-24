
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
    public sealed partial class SoldierAttrConfig : BeanBase
    {
        public SoldierAttrConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            level = buf.ReadInt();
            upgradeExp = buf.ReadInt();
            health = buf.ReadInt();
            attack = buf.ReadInt();
            defense = buf.ReadInt();

            PostInit();
        }

        public static SoldierAttrConfig DeserializeSoldierAttrConfig(ByteBuf buf)
        {
            return new SoldierAttrConfig(buf);
        }

        /// <summary>
        /// 士兵属性ID
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 等级
        /// </summary>
        public readonly int level;

        /// <summary>
        /// 升级所需经验
        /// </summary>
        public readonly int upgradeExp;

        /// <summary>
        /// 生命
        /// </summary>
        public readonly int health;

        /// <summary>
        /// 攻击
        /// </summary>
        public readonly int attack;

        /// <summary>
        /// 防御
        /// </summary>
        public readonly int defense;

        public const int Id = -1844677739;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "level:" + level + ","
            + "upgradeExp:" + upgradeExp + ","
            + "health:" + health + ","
            + "attack:" + attack + ","
            + "defense:" + defense + ","
            + "}";
        }

        partial void PostInit();
    }
}
