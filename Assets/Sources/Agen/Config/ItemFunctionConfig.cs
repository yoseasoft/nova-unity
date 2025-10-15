using Luban;

namespace Game.Config
{
    /// <summary>
    /// 符号功能配置
    /// </summary>
    public sealed partial class ItemFunctionConfig : BeanBase
    {
        public ItemFunctionConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            effect = ItemEffect.DeserializeItemEffect(buf);

            PostInit();
        }

        public static ItemFunctionConfig DeserializeItemFunctionConfig(ByteBuf buf)
        {
            return new ItemFunctionConfig(buf);
        }

        /// <summary>
        /// 功能标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 效果
        /// </summary>
        public readonly ItemEffect effect;

        public const int Id = 1113933645;

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