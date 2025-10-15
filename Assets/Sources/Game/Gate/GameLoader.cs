/// <summary>
/// 2023-09-07 Game Framework Code By Hurley
/// </summary>

using Cysharp.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// 程序数据加载器封装对象类，对业务层数据提供加载、卸载及重载等操作接口
    /// </summary>
    public static class GameLoader
    {
        /// <summary>
        /// 数据加载处理函数
        /// </summary>
        public static async UniTask Load()
        {
            // 加载数据表
            await ConfigLoader.LoadAsync();
        }

        /// <summary>
        /// 数据卸载处理函数
        /// </summary>
        public static void Unload()
        {
        }

        /// <summary>
        /// 数据重载处理函数
        /// </summary>
        public static void Reload()
        {
        }
    }
}