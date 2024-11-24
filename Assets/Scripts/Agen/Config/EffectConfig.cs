
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
    public sealed partial class EffectConfig : BeanBase
    {
        public EffectConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            duration = buf.ReadInt();
            resourceId = buf.ReadInt();

            PostInit();
        }

        public static EffectConfig DeserializeEffectConfig(ByteBuf buf)
        {
            return new EffectConfig(buf);
        }

        /// <summary>
        /// 特效ID
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 持续时间
        /// </summary>
        public readonly int duration;

        /// <summary>
        /// 资源ID
        /// </summary>
        public readonly int resourceId;

        public const int Id = -682668973;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "duration:" + duration + ","
            + "resourceId:" + resourceId + ","
            + "}";
        }

        partial void PostInit();
    }
}