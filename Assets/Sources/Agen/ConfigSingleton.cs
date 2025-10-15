namespace Game
{
    /// <summary>
    /// 配置表单例基类
    /// </summary>
    public abstract class ConfigSingleton<T> : IConfigRegister where T : ConfigSingleton<T>
    {
        public static T Instance { get; private set; }

        public void Register()
        {
            Instance = (T)this;
        }
    }

    /// <summary>
    /// 注册接口
    /// </summary>
    public interface IConfigRegister
    {
        /// <summary>
        /// 注册单例
        /// </summary>
        public abstract void Register();
    }
}