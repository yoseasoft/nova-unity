namespace Game.Config
{
    /// <summary>
    /// 货币类型
    /// </summary>
    public enum CurrencyType
    {
        /// <summary>
        /// 无效
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 金币
        /// </summary>
        Gold = 11,

        /// <summary>
        /// 重掷代币
        /// </summary>
        RethrowingToken = 21,

        /// <summary>
        /// 移除代币
        /// </summary>
        RemovingToken = 22,

        /// <summary>
        /// 符号抓取代币
        /// </summary>
        SymbolGrabingToken = 31,

        /// <summary>
        /// 精华抓取代币
        /// </summary>
        CreamGrabingToken = 32,

        /// <summary>
        /// 物品抓取代币
        /// </summary>
        GoodsGrabingToken = 33
    }
}