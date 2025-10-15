using Luban;

namespace Game.Config
{
    public sealed partial class SF_CurrencyRewardOnRoundBegan : SymbolEffect
    {
        public SF_CurrencyRewardOnRoundBegan(ByteBuf buf) : base(buf)
        {
            targetSymbols = buf.ReadInt();
            currencyType = (CurrencyType)buf.ReadInt();
            currencyCount = buf.ReadInt();
            opTargetType = (SymbolRemoveOperationType)buf.ReadInt();

            PostInit();
        }

        public static SF_CurrencyRewardOnRoundBegan DeserializeSF_CurrencyRewardOnRoundBegan(ByteBuf buf)
        {
            return new SF_CurrencyRewardOnRoundBegan(buf);
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

        /// <summary>
        /// 移除目标类型
        /// </summary>
        public readonly SymbolRemoveOperationType opTargetType;

        public const int Id = 682846798;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + targetSymbols + ","
            + "currencyType:" + currencyType + ","
            + "currencyCount:" + currencyCount + ","
            + "opTargetType:" + opTargetType + ","
            + "}";
        }

        partial void PostInit();
    }
}