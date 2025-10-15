/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-16
/// 功能描述：
/// </summary>

using FairyGUI;

namespace Game
{
    public static class LoadingPanelSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        private static void StartupLoadingPanel(this LoadingPanel self)
        {
            GComponent contentPane = self.Window as GComponent;

            self.title = contentPane.GetChild("lab_title") as GTextField;
            self.text = contentPane.GetChild("lab_text") as GTextField;
            self.start = contentPane.GetChild("btn_start") as GButton;
            self.progressBar = contentPane.GetChild("schedule") as GProgressBar;
            contentPane.onClick.Add(OnClick);

            self.start.visible = false;
        }

        static void OnClick()
        {
            NE.SceneHandler.ReplaceScene<MainScene>();
        }
        
        /// <summary>
        /// 显示进度条
        /// </summary>
        public static void StartProgress(this LoadingPanel self, double value)
        {
            if (!self.IsReady) return;

            self.progressBar.value = value;
            //Debugger.Info("Loading Panel Default StartProgress Value = " + value);
        }

        [GameEngine.EventSubscribeBindingOfTarget(typeof(LoadingResourceProgressInfo))]
        private static void OnLoadingResourceProgressNotify(this LoadingPanel self, LoadingResourceProgressInfo progressInfo)
        {
            if (!self.IsReady) return;

            if (progressInfo.final)
            {
                self.text.text = "资源加载完成！";
                self.StartProgress(100);
                self.start.visible = true;
            }
            else
            {
                self.text.text = $"当前已经加载到第{progressInfo.index}个资源，资源名称为：{progressInfo.name}！";
                self.StartProgress(progressInfo.progress);
            }
        }
    }
}