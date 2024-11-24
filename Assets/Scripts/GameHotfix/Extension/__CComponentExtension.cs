/// <summary>
/// 2024-04-03 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 组件类的扩展接口声明
    /// </summary>
    public static class __CComponentExtension
    {
        public static void Fire<T>(this GameEngine.CComponent self, T eventData) where T : struct
        {
            self.Entity?.Fire(eventData);
        }

        public static void Subscribe<T>(this GameEngine.CComponent self, System.Action<T> listener) where T : struct
        {
            self.Entity?.Subscribe(listener);
        }
    }
}
