namespace Game.Config
{
    /// <summary>
    /// 符号触发类型
    /// </summary>
    public enum SymbolCallType
    {
        /// <summary>
        /// 无效
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 回合奖励
        /// </summary>
        RewardOnRoundBegan = 1111,

        /// <summary>
        /// 回合符号奖励
        /// </summary>
        SymbolRewardOnRoundBegan = 1112,

        /// <summary>
        /// 回合货币奖励
        /// </summary>
        CurrencyRewardOnRoundBegan = 1113,

        /// <summary>
        /// 几率奖励
        /// </summary>
        RewardOnProbability = 1211,

        /// <summary>
        /// 几率符号奖励
        /// </summary>
        SymbolRewardOnProbability = 1212,

        /// <summary>
        /// 消除奖励
        /// </summary>
        RewardOnRemoveSymbol = 1311,

        /// <summary>
        /// 消除符号奖励
        /// </summary>
        SymbolRewardOnRemoveSymbol = 1312,

        /// <summary>
        /// 消除货币奖励
        /// </summary>
        CurrencyRewardOnRemoveSymbol = 1313,

        /// <summary>
        /// 目标相邻奖励
        /// </summary>
        RewardOnAdjacentForTarget = 2111,

        /// <summary>
        /// 目标相邻符号奖励
        /// </summary>
        SymbolRewardOnAdjacentForTarget = 2112,

        /// <summary>
        /// 目标不相邻奖励
        /// </summary>
        RewardOnNotAdjacentForTarget = 2151,

        /// <summary>
        /// 目标不相邻符号奖励
        /// </summary>
        SymbolRewardOnNotAdjacentForTarget = 2152,

        /// <summary>
        /// 任意目标相邻奖励
        /// </summary>
        RewardOnAdjacentForAnyTarget = 2211,

        /// <summary>
        /// 任意目标相邻符号奖励
        /// </summary>
        SymbolRewardOnAdjacentForAnyTarget = 2212,

        /// <summary>
        /// 任意目标不相邻奖励
        /// </summary>
        RewardOnNotAdjacentForAnyTarget = 2251,

        /// <summary>
        /// 任意目标不相邻符号奖励
        /// </summary>
        SymbolRewardOnNotAdjacentForAnyTarget = 2252,

        /// <summary>
        /// 目标范围相邻奖励
        /// </summary>
        RewardOnAdjacentAreaForTarget = 2311,

        /// <summary>
        /// 目标范围相邻符号奖励
        /// </summary>
        SymbolRewardOnAdjacentAreaForTarget = 2312,

        /// <summary>
        /// 目标范围检索奖励
        /// </summary>
        RewardOnRangeQueryForTarget = 2411,

        /// <summary>
        /// 目标范围检索符号奖励
        /// </summary>
        SymbolRewardOnRangeQueryForTarget = 2412,

        /// <summary>
        /// 角落奖励
        /// </summary>
        RewardOnLocatedInCorner = 3111,

        /// <summary>
        /// 角落符号奖励
        /// </summary>
        SymbolRewardOnLocatedInCorner = 3113,

        /// <summary>
        /// 方向奖励
        /// </summary>
        RewardOnPointToDirection = 3211,

        /// <summary>
        /// 方向偿还
        /// </summary>
        RepayOnPointToDirection = 3212,

        /// <summary>
        /// 方向奖励并移除
        /// </summary>
        RewardAndRemoveSymbolOnPointToDirection = 3221,

        /// <summary>
        /// 任意目标相邻污染
        /// </summary>
        SymbolPolluteOnAdjacentForAnyTarget = 3311,

        /// <summary>
        /// 总回合计数奖励
        /// </summary>
        RewardOnTotalRounds = 5111,

        /// <summary>
        /// 循环回合计数奖励
        /// </summary>
        RewardOnLoopRounds = 5112,

        /// <summary>
        /// 总回合计数符号奖励
        /// </summary>
        SymbolRewardOnTotalRounds = 5113,

        /// <summary>
        /// 循环回合计数符号奖励
        /// </summary>
        SymbolRewardOnLoopRounds = 5114,

        /// <summary>
        /// 总消耗计数奖励
        /// </summary>
        RewardOnTotalUsedCount = 5211,

        /// <summary>
        /// 回合消耗计数奖励
        /// </summary>
        RewardOnRoundUsedCount = 5221,

        /// <summary>
        /// 总掷骰计数奖励
        /// </summary>
        RewardOnTotalRollDiceCount = 6111,

        /// <summary>
        /// 循环掷骰计数奖励
        /// </summary>
        RewardOnLoopRollDiceCount = 6112,

        /// <summary>
        /// 总掷骰计数奖励并移除
        /// </summary>
        RewardAndRemoveSymbolOnTotalRollDiceCount = 6121,

        /// <summary>
        /// 骰子点数计数奖励
        /// </summary>
        RewardOnDicePointsCount = 6131
    }
}