/// <summary>
/// 2023-08-25 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 世界管理器的接口对象类，通过该接口绑定所有从世界管理器中进行逻辑转发<br/>
    /// 所有通过该接口注册的回调通知，没有调度的先后顺序，在绑定时需要注意这一点
    /// </summary>
    public interface IWorldObserver
    {
        /// <summary>
        /// 开始回调通知接口函数
        /// </summary>
        void Startup();

        /// <summary>
        /// 关闭回调通知接口函数
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 固定刷新回调通知接口函数
        /// </summary>
        void FixedUpdate();

        /// <summary>
        /// 刷新回调通知接口函数
        /// </summary>
        void Update();

        /// <summary>
        /// 后置刷新回调通知接口函数
        /// </summary>
        void LateUpdate();

        /// <summary>
        /// 固定执行回调通知接口函数
        /// </summary>
        void FixedExecute();

        /// <summary>
        /// 执行回调通知接口函数
        /// </summary>
        void Execute();

        /// <summary>
        /// 后置执行回调通知接口函数
        /// </summary>
        void LateExecute();
    }
}
