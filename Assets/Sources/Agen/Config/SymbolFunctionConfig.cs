using Luban;

namespace Game.Config
{
    /// <summary>
    /// 符号功能配置
    /// </summary>
    public sealed partial class SymbolFunctionConfig : BeanBase
    {
        public SymbolFunctionConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            effect = SymbolEffect.DeserializeSymbolEffect(buf);

            PostInit();
        }

        public static SymbolFunctionConfig DeserializeSymbolFunctionConfig(ByteBuf buf)
        {
            return new SymbolFunctionConfig(buf);
        }

        /// <summary>
        /// 功能标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 效果
        /// </summary>
        public readonly SymbolEffect effect;

        public const int Id = -3183342;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "effect:" + effect + ","
            + "}";
        }

        partial void PostInit();
    }
}