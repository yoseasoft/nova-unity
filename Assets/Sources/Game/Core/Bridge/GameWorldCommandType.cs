/// <summary>
/// 2025-11-25 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 游戏调度指令类型参数的枚举类型定义
    /// </summary>
    public enum GameWorldCommandType : byte
    {
        /// <summary>
        /// 无效指令
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 程序集
        /// </summary>
        Assembly,
        /// <summary>
        /// 上下文
        /// </summary>
        Context,
        /// <summary>
        /// 配置表
        /// </summary>
        Configure,
    }
}