/// <summary>
/// 2024-03-20 Game Framework Code By Hurley
/// </summary>

using FairyGUI;
using GameEngine;

namespace Game
{
    [Aspect]
    public static class LoadingPanelSystem
    {
        [OnAspectAfterCallOfTarget(typeof(LoadingPanel), GameEngine.AspectBehaviourType.Startup)]
        public static void StartupLoadingPanel(LoadingPanel self)
        {
            self.progressBar = self.ContentPane.GetChild("schedule") as GProgressBar;
            self.ContentPane.onClick.Add(OnClick);
        }

        static void OnClick()
        {
            Debugger.Info("123");
        }
    }
}