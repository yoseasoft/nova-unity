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
        /// 加载处理函数
        /// </summary>
        public static void Load(GameWorldCommandType commandType)
        {
            switch (commandType)
            {
                case GameWorldCommandType.Assembly:
                    // 加载程序集
                    AssemblyLoader.Load();
                    break;
                case GameWorldCommandType.Context:
                    // 加载上下文
                    ContextLoader.Load();
                    break;
                case GameWorldCommandType.Configure:
                    // 加载配置表
                    ConfigureLoader.Load().Forget();
                    break;
                default:
                    Debugger.Throw<System.InvalidOperationException>($"Invalid load type {commandType}.");
                    break;
            }
        }

        /// <summary>
        /// 卸载处理函数
        /// </summary>
        public static void Unload(GameWorldCommandType commandType)
        {
            switch (commandType)
            {
                case GameWorldCommandType.Assembly:
                    // 卸载程序集
                    AssemblyLoader.Unload();
                    break;
                case GameWorldCommandType.Context:
                    // 卸载上下文
                    ContextLoader.Unload();
                    break;
                case GameWorldCommandType.Configure:
                    // 卸载配置表
                    ConfigureLoader.Unload();
                    break;
                default:
                    Debugger.Throw<System.InvalidOperationException>($"Invalid unload type {commandType}.");
                    break;
            }
        }

        /// <summary>
        /// 重载处理函数
        /// </summary>
        public static void Reload(GameWorldCommandType commandType)
        {
            switch (commandType)
            {
                case GameWorldCommandType.Assembly:
                    // 重载程序集
                    AssemblyLoader.Reload();
                    break;
                case GameWorldCommandType.Context:
                    // 重载上下文
                    ContextLoader.Reload();
                    break;
                case GameWorldCommandType.Configure:
                    // 重载配置表
                    ConfigureLoader.Reload();
                    break;
                default:
                    Debugger.Throw<System.InvalidOperationException>($"Invalid reload type {commandType}.");
                    break;
            }
        }
    }
}
