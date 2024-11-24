namespace AssetModule
{
    /// <summary>
    /// 加载状态
    /// </summary>
    public enum LoadableStatus
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        Init,

        /// <summary>
        /// 检查版本中
        /// </summary>
        CheckingVersion,

        /// <summary>
        /// 依赖加载中
        /// </summary>
        DependentLoading,

        /// <summary>
        /// 解压中
        /// </summary>
        Unpacking,

        /// <summary>
        /// 加载中
        /// </summary>
        Loading,

        /// <summary>
        /// 加载成功
        /// </summary>
        LoadSuccessful,

        /// <summary>
        /// 加载失败
        /// </summary>
        LoadFailed,

        /// <summary>
        /// 已卸载
        /// </summary>
        Unloaded
    }
}