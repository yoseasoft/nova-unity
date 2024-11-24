
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
    public sealed partial class BulletConfig : BeanBase
    {
        public BulletConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            name = buf.ReadString();
            type = buf.ReadInt();
            subtype = buf.ReadInt();
            damageRadius = buf.ReadFloat();
            onGroundAttack = buf.ReadBool();
            onAirStrike = buf.ReadBool();
            isRangeAttack = buf.ReadBool();
            punctureCount = buf.ReadInt();
            delayTime = buf.ReadFloat();
            flightSpeed = buf.ReadFloat();
            iconId = buf.ReadInt();
            soundId = buf.ReadInt();
            modelId = buf.ReadInt();
            hitSoundId = buf.ReadInt();
            hitEffectId = buf.ReadInt();

            PostInit();
        }

        public static BulletConfig DeserializeBulletConfig(ByteBuf buf)
        {
            return new BulletConfig(buf);
        }

        /// <summary>
        /// 子弹ID
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 子弹名称
        /// </summary>
        public readonly string name;

        /// <summary>
        /// 类型
        /// </summary>
        public readonly int type;

        /// <summary>
        /// 二级类型
        /// </summary>
        public readonly int subtype;

        /// <summary>
        /// 伤害半径
        /// </summary>
        public readonly float damageRadius;

        /// <summary>
        /// 地面攻击
        /// </summary>
        public readonly bool onGroundAttack;

        /// <summary>
        /// 空中打击
        /// </summary>
        public readonly bool onAirStrike;

        /// <summary>
        /// 是否为远程攻击
        /// </summary>
        public readonly bool isRangeAttack;

        /// <summary>
        /// 穿透攻击次数
        /// </summary>
        public readonly int punctureCount;

        /// <summary>
        /// 延迟时间
        /// </summary>
        public readonly float delayTime;

        /// <summary>
        /// 飞行速度
        /// </summary>
        public readonly float flightSpeed;

        /// <summary>
        /// 图标资源ID
        /// </summary>
        public readonly int iconId;

        /// <summary>
        /// 音效资源ID
        /// </summary>
        public readonly int soundId;

        /// <summary>
        /// 模型资源ID
        /// </summary>
        public readonly int modelId;

        /// <summary>
        /// 命中音效资源ID
        /// </summary>
        public readonly int hitSoundId;

        /// <summary>
        /// 命中特效资源ID
        /// </summary>
        public readonly int hitEffectId;

        public const int Id = 284876548;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "name:" + name + ","
            + "type:" + type + ","
            + "subtype:" + subtype + ","
            + "damageRadius:" + damageRadius + ","
            + "onGroundAttack:" + onGroundAttack + ","
            + "onAirStrike:" + onAirStrike + ","
            + "isRangeAttack:" + isRangeAttack + ","
            + "punctureCount:" + punctureCount + ","
            + "delayTime:" + delayTime + ","
            + "flightSpeed:" + flightSpeed + ","
            + "iconId:" + iconId + ","
            + "soundId:" + soundId + ","
            + "modelId:" + modelId + ","
            + "hitSoundId:" + hitSoundId + ","
            + "hitEffectId:" + hitEffectId + ","
            + "}";
        }

        partial void PostInit();
    }
}
