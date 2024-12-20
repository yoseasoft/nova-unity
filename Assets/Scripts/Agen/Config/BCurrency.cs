
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
    public sealed partial class BCurrency : Bonus
    {
        public BCurrency(ByteBuf buf) : base(buf) 
        {
            currencyType = buf.ReadInt();
            num = buf.ReadInt();

            PostInit();
        }

        public static BCurrency DeserializeBCurrency(ByteBuf buf)
        {
            return new BCurrency(buf);
        }

        /// <summary>
        /// 货币类型
        /// </summary>
        public readonly int currencyType;

        /// <summary>
        /// 货币数量
        /// </summary>
        public readonly int num;

        public const int Id = 1607135859;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "currencyType:" + currencyType + ","
            + "num:" + num + ","
            + "}";
        }

        partial void PostInit();
    }
}
