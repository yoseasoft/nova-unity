using FairyGUI;
using GameEngine;

namespace Game
{
    [DeclareViewClass("LoadingPanel")]
    public class LoadingPanel : CView
    {
        public GTextField title;

        public GTextField text;

        public GButton start;

        /// <summary>
        /// 进度条
        /// </summary>
        public GProgressBar progressBar;
    }
}