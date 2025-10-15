namespace Game.Config
{
    /// <summary>
    /// 符号移除操作类型
    /// </summary>
    public enum SymbolRemoveOperationType
    {
        /// <summary>
        /// 移除无效
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 移除自身
        /// </summary>
        Self = 1,

        /// <summary>
        /// 移除目标
        /// </summary>
        Target = 2,

        /// <summary>
        /// 移除全部
        /// </summary>
        All = 3
    }
}