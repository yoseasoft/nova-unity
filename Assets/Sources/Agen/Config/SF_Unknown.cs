using Luban;

namespace Game.Config
{
    public sealed partial class SF_Unknown : SymbolEffect
    {
        public SF_Unknown(ByteBuf buf) : base(buf)
        {

            PostInit();
        }

        public static SF_Unknown DeserializeSF_Unknown(ByteBuf buf)
        {
            return new SF_Unknown(buf);
        }

        public const int Id = -1655442882;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "}";
        }

        partial void PostInit();
    }
}