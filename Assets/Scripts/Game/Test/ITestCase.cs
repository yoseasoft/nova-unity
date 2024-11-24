/// <summary>
/// 2024-05-17 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 测试案例接口定义
    /// </summary>
    public interface ITestCase
    {
        /// <summary>
        /// 测试案例名称
        /// </summary>
        public string CaseName => GetType().FullName;

        void Startup();

        void Shutdown();

        void Update();
    }
}
