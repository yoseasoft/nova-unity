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
    /// Logo场景定时器组件逻辑处理类
    /// </summary>
    public static class LogoTimerComponentSystem
    {
        [OnAwake]
        private static void Awake(this LogoTimerComponent self)
        {
            self.sessions = new List<int>();

            self.StartTimer();
        }

        private static void StartTimer(this LogoTimerComponent self)
        {
            int session = 0;
            session = self.Schedule(2000, -1, delegate (int s)
            {
                Debugger.Log("logo schedule first on {0}.", s);
            });
            self.sessions.Add(session);

            session = self.Schedule(1000, -1, delegate (int s)
            {
                Debugger.Log("logo schedule second on {0}.", s);
            });
            self.sessions.Add(session);

            session = self.Schedule(3000, -1, delegate (int s)
            {
                Debugger.Log("logo schedule third on {0}.", s);
            });
            self.sessions.Add(session);
        }

        public static void StopTimer(this LogoTimerComponent self)
        {
            for (int n = 0; n < self.sessions.Count; ++n)
            {
                self.Unschedule(self.sessions[n]);
            }

            self.sessions.Clear();
        }
    }
}

