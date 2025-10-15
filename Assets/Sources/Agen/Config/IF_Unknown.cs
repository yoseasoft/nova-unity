using Luban;

namespace Game.Config
{
    public sealed partial class IF_Unknown : ItemEffect
    {
        public IF_Unknown(ByteBuf buf) : base(buf)
        {

            PostInit();
        }

        public static IF_Unknown DeserializeIF_Unknown(ByteBuf buf)
        {
            return new IF_Unknown(buf);
        }

        public const int Id = 309692168;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "}";
        }

        partial void PostInit();
    }
}