/// <summary>
/// 2024-04-26 Game Framework Code By Hurley
/// </summary>

using UnityEngine;

namespace Game
{
    /// <summary>
    /// 特效组件逻辑
    /// </summary>
    [GameEngine.Aspect, GameEngine.ExtendSupported]
    public static class EffectComponentSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(EffectComponent), GameEngine.AspectBehaviourType.Initialize)]
        public static void Initialize(this EffectComponent self)
        {
            Debugger.Log("System '{0}' Initialize !!!", self.GetType().FullName);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(EffectComponent), GameEngine.AspectBehaviourType.Startup)]
        public static void Startup(this EffectComponent self)
        {
            Debugger.Log("System '{0}' Startup !!!", self.GetType().FullName);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(EffectComponent), GameEngine.AspectBehaviourType.Awake)]
        public static void Awake(this EffectComponent self)
        {
            Debugger.Log("System '{0}' Awake !!!", self.GetType().FullName);
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(EffectComponent), GameEngine.AspectBehaviourType.Start)]
        public static void BeforeStart(this EffectComponent self)
        {
            Debugger.Log("测试'{0}'对象在'Start'前的回调处理流程!", self.GetType().FullName);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(EffectComponent), GameEngine.AspectBehaviourType.Start)]
        public static void Start(this EffectComponent self)
        {
            Debugger.Log("System '{0}' Start !!!", self.GetType().FullName);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(EffectComponent), GameEngine.AspectBehaviourType.Destroy)]
        public static void Destroy(this EffectComponent self)
        {
            Debugger.Log("System '{0}' Destroy !!!", self.GetType().FullName);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(EffectComponent), GameEngine.AspectBehaviourType.Shutdown)]
        public static void Shutdown(this EffectComponent self)
        {
            Debugger.Log("System '{0}' Shutdown !!!", self.GetType().FullName);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(EffectComponent), GameEngine.AspectBehaviourType.Cleanup)]
        public static void Cleanup(this EffectComponent self)
        {
            Debugger.Log("System '{0}' Cleanup !!!", self.GetType().FullName);
        }
    }
}
