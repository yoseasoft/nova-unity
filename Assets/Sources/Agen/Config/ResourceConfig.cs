using Luban;

namespace Game.Config
{
    /// <summary>
    /// 资源配置
    /// </summary>
    public sealed partial class ResourceConfig : BeanBase
    {
        public ResourceConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            assetUrl = buf.ReadString();

            PostInit();
        }

        public static ResourceConfig DeserializeResourceConfig(ByteBuf buf)
        {
            return new ResourceConfig(buf);
        }

        /// <summary>
        /// 资源标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 资源地址
        /// </summary>
        public readonly string assetUrl;

        public const int Id = -1505018608;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "assetUrl:" + assetUrl + ","
            + "}";
        }

        partial void PostInit();
    }
}