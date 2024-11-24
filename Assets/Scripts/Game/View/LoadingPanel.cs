using FairyGUI;
using GameEngine;
using NovaEngine;

namespace Game
{
    [DeclareViewClass("LoadingPanel")]
    public class LoadingPanel : CView
    {
        /// <summary>
        /// 进度条
        /// </summary>
        public GProgressBar progressBar;

        /// <summary>
        /// 显示进度条
        /// </summary>
        public void StartProgress(double value)
        {
            // self.progressBar.value = value;
            Debugger.Info("StartProgress:" + value);
        }
    }
}