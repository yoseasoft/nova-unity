using Luban;

namespace Game.Config
{
    /// <summary>
    /// 符号效果数据结构
    /// </summary>
    public abstract partial class SymbolEffect : BeanBase
    {
        public SymbolEffect(ByteBuf buf)
        {

            PostInit();
        }

        public static SymbolEffect DeserializeSymbolEffect(ByteBuf buf)
        {
            switch (buf.ReadInt())
            {
                case SF_Unknown.Id: return new SF_Unknown(buf);
                case SF_RewardOnRoundBegan.Id: return new SF_RewardOnRoundBegan(buf);
                case SF_SymbolRewardOnRoundBegan.Id: return new SF_SymbolRewardOnRoundBegan(buf);
                case SF_CurrencyRewardOnRoundBegan.Id: return new SF_CurrencyRewardOnRoundBegan(buf);
                case SF_RewardOnProbability.Id: return new SF_RewardOnProbability(buf);
                case SF_SymbolRewardOnProbability.Id: return new SF_SymbolRewardOnProbability(buf);
                case SF_RewardOnRemoveSymbol.Id: return new SF_RewardOnRemoveSymbol(buf);
                case SF_SymbolRewardOnRemoveSymbol.Id: return new SF_SymbolRewardOnRemoveSymbol(buf);
                case SF_CurrencyRewardOnRemoveSymbol.Id: return new SF_CurrencyRewardOnRemoveSymbol(buf);
                case SF_RewardOnAdjacentForTarget.Id: return new SF_RewardOnAdjacentForTarget(buf);
                case SF_SymbolRewardOnAdjacentForTarget.Id: return new SF_SymbolRewardOnAdjacentForTarget(buf);
                case SF_RewardOnNotAdjacentForTarget.Id: return new SF_RewardOnNotAdjacentForTarget(buf);
                case SF_SymbolRewardOnNotAdjacentForTarget.Id: return new SF_SymbolRewardOnNotAdjacentForTarget(buf);
                case SF_RewardOnAdjacentForAnyTarget.Id: return new SF_RewardOnAdjacentForAnyTarget(buf);
                case SF_SymbolRewardOnAdjacentForAnyTarget.Id: return new SF_SymbolRewardOnAdjacentForAnyTarget(buf);
                case SF_RewardOnNotAdjacentForAnyTarget.Id: return new SF_RewardOnNotAdjacentForAnyTarget(buf);
                case SF_SymbolRewardOnNotAdjacentForAnyTarget.Id: return new SF_SymbolRewardOnNotAdjacentForAnyTarget(buf);
                case SF_RewardOnAdjacentAreaForTarget.Id: return new SF_RewardOnAdjacentAreaForTarget(buf);
                case SF_SymbolRewardOnAdjacentAreaForTarget.Id: return new SF_SymbolRewardOnAdjacentAreaForTarget(buf);
                case SF_RewardOnRangeQueryForTarget.Id: return new SF_RewardOnRangeQueryForTarget(buf);
                case SF_SymbolRewardOnRangeQueryForTarget.Id: return new SF_SymbolRewardOnRangeQueryForTarget(buf);
                case SF_SymbolPolluteOnAdjacentForAnyTarget.Id: return new SF_SymbolPolluteOnAdjacentForAnyTarget(buf);
                case SF_RewardOnTotalRounds.Id: return new SF_RewardOnTotalRounds(buf);
                case SF_RewardOnLoopRounds.Id: return new SF_RewardOnLoopRounds(buf);
                case SF_SymbolRewardOnTotalRounds.Id: return new SF_SymbolRewardOnTotalRounds(buf);
                case SF_SymbolRewardOnLoopRounds.Id: return new SF_SymbolRewardOnLoopRounds(buf);
                case SF_RewardAndRemoveSymbolOnProbability.Id: return new SF_RewardAndRemoveSymbolOnProbability(buf);
                case SF_RewardAndRemoveSymbolOnTotalAdjacentCountForTarget.Id: return new SF_RewardAndRemoveSymbolOnTotalAdjacentCountForTarget(buf);
                case SF_RewardAndRemoveSymbolOnRoundAdjacentCountForTarget.Id: return new SF_RewardAndRemoveSymbolOnRoundAdjacentCountForTarget(buf);
                case SF_SymbolRewardAndRemoveSymbolOnTotalAdjacentCountForTarget.Id: return new SF_SymbolRewardAndRemoveSymbolOnTotalAdjacentCountForTarget(buf);
                case SF_SymbolRewardAndRemoveSymbolOnRoundAdjacentCountForTarget.Id: return new SF_SymbolRewardAndRemoveSymbolOnRoundAdjacentCountForTarget(buf);
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