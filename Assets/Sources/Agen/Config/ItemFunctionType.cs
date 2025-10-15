namespace Game.Config
{
    /// <summary>
    /// 物品功能类型
    /// </summary>
    public enum ItemFunctionType
    {
        /// <summary>
        /// 无效
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 全局目标奖励
        /// </summary>
        GlobalBonus_RewardCoinForTarget = 11,

        /// <summary>
        /// 全局属性奖励
        /// </summary>
        GlobalBonus_RewardAttributeValue = 12,

        /// <summary>
        /// 固定奖励金币
        /// </summary>
        Fixed_RewardCoin = 111,

        /// <summary>
        /// 固定奖励符号
        /// </summary>
        Fixed_RewardSymbol = 112,

        /// <summary>
        /// 使用奖励符号
        /// </summary>
        Used_RewardSymbol = 122,

        /// <summary>
        /// 使用奖励货币
        /// </summary>
        Used_RewardCurrency = 123,

        /// <summary>
        /// 目标激活奖励金币
        /// </summary>
        TargetActivation_RewardCoin = 131,

        /// <summary>
        /// 目标激活奖励符号
        /// </summary>
        TargetActivation_RewardSymbol = 132,

        /// <summary>
        /// 目标相邻奖励金币
        /// </summary>
        TargetAdjacent_RewardCoinToTarget = 133,

        /// <summary>
        /// 添加符号奖励金币
        /// </summary>
        AddSymbol_RewardCoin = 141,

        /// <summary>
        /// 移除符号奖励金币
        /// </summary>
        RemoveSymbol_RewardCoin = 142
    }
}