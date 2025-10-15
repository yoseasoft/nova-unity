using Luban;

namespace Game.Config
{
    public sealed partial class IF_Used_RewardCurrency : ItemEffect
    {
        public IF_Used_RewardCurrency(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            currencyType = (CurrencyType)buf.ReadInt();
            currencyCount = buf.ReadInt();

            PostInit();
        }

        public static IF_Used_RewardCurrency DeserializeIF_Used_RewardCurrency(ByteBuf buf)
        {
            return new IF_Used_RewardCurrency(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly int targetSymbols;

        /// <summary>
        /// 货币类型
        /// </summary>
        public readonly CurrencyType currencyType;

        /// <summary>
        /// 货币个数
        /// </summary>
        public readonly int currencyCount;

        public const int Id = -317165824;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "currencyType:" + currencyType + ","
            + "currencyCount:" + currencyCount + ","
            + "}";
        }

        partial void PostInit();
    }
}