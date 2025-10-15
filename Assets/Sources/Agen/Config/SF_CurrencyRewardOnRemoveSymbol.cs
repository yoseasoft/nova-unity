using Luban;

namespace Game.Config
{
    public sealed partial class SF_CurrencyRewardOnRemoveSymbol : SymbolEffect
    {
        public SF_CurrencyRewardOnRemoveSymbol(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            currencyType = (CurrencyType)buf.ReadInt();
            currencyCount = buf.ReadInt();

            PostInit();
        }

        public static SF_CurrencyRewardOnRemoveSymbol DeserializeSF_CurrencyRewardOnRemoveSymbol(ByteBuf buf)
        {
            return new SF_CurrencyRewardOnRemoveSymbol(buf);
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

        public const int Id = 1445395463;

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