namespace Game.Config
{
    /// <summary>
    /// 符号范围检索类型
    /// </summary>
    public enum SymbolRangeQueryType
    {
        /// <summary>
        /// 无效
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 串联
        /// </summary>
        AdjacentLinked = 1,

        /// <summary>
        /// 同行
        /// </summary>
        LocatedInRow = 2,

        /// <summary>
        /// 同列
        /// </summary>
        LocatedInCol = 3,

        /// <summary>
        /// 同面
        /// </summary>
        SameSide = 4,

        /// <summary>
        /// 全场
        /// </summary>
        SameSpace = 5
    }
}