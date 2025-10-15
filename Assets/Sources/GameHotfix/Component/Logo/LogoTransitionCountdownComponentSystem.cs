/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-16
/// 功能描述：
/// </summary>

using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// Logo场景转场倒计时组件逻辑处理类
    /// </summary>
    public static class LogoTransitionCountdownComponentSystem
    {
        [OnAwake]
        private static void Awake(this LogoTransitionCountdownComponent self)
        {
            self.running = false;
            self.completed = false;
        }

        [OnUpdate]
        private static void Update(this LogoTransitionCountdownComponent self)
        {
            if (!self.running || self.completed) return;

            float now = NovaEngine.Timestamp.RealtimeSinceStartup;
            if (now - self.enterSceneTimestamp > 1.0f)
            {
                self.enterSceneTimestamp = now;
                --self.countdownTime;

                if (self.countdownTime > 0)
                {
                    Debugger.Log("即将进入主业务场景，还剩 {0} 秒！", self.countdownTime);
                }
                else
                {
                    self.completed = true;

                    self.Fire(1201);
                }
            }
        }

        public static void StartTransitionCountdown(this LogoTransitionCountdownComponent self)
        {
            self.enterSceneTimestamp = NovaEngine.Timestamp.RealtimeSinceStartup;
            self.countdownTime = 4;
            self.running = true;
        }
    }
}

