namespace Game.Config
{
    /// <summary>
    /// 符号奖励操作类型
    /// </summary>
    public enum SymbolRewardOperationType
    {
        /// <summary>
        /// 奖励无效
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 奖励自身
        /// </summary>
        Self = 1,

        /// <summary>
        /// 奖励目标
        /// </summary>
        Target = 2,

        /// <summary>
        /// 奖励全部
        /// </summary>
        All = 3
    }
}