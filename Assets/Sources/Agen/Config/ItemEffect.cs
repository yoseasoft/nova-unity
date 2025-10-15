using Luban;

namespace Game.Config
{
    /// <summary>
    /// 物品功能数据结构
    /// </summary>
    public abstract partial class ItemEffect : BeanBase
    {
        public ItemEffect(ByteBuf buf)
        {

            PostInit();
        }

        public static ItemEffect DeserializeItemEffect(ByteBuf buf)
        {
            switch (buf.ReadInt())
            {
                case IF_Unknown.Id: return new IF_Unknown(buf);
                case IF_GlobalBonus_RewardCoinForTarget.Id: return new IF_GlobalBonus_RewardCoinForTarget(buf);
                case IF_GlobalBonus_RewardAttributeValue.Id: return new IF_GlobalBonus_RewardAttributeValue(buf);
                case IF_Fixed_RewardCoin.Id: return new IF_Fixed_RewardCoin(buf);
                case IF_Fixed_RewardSymbol.Id: return new IF_Fixed_RewardSymbol(buf);
                case IF_Used_RewardSymbol.Id: return new IF_Used_RewardSymbol(buf);
                case IF_Used_RewardCurrency.Id: return new IF_Used_RewardCurrency(buf);
                case IF_AddSymbol_RewardCoin.Id: return new IF_AddSymbol_RewardCoin(buf);
                case IF_RemoveSymbol_RewardCoin.Id: return new IF_RemoveSymbol_RewardCoin(buf);
                default: throw new SerializationException();
            }
        }

        public override string ToString()
        {
            return "{ "
            + "}";
        }

        partial void PostInit();
    }
}