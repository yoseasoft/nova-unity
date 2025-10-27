using FairyGUI;
using GameEngine;

namespace Game
{
    [CViewClass("LoginPanel")]
    public class LoginPanel : CView
    {
        /// <summary>
        /// 开始
        /// </summary>
        public GButton btnStart;
        /// <summary>
        /// 继续
        /// </summary>
        public GButton btnContinue;
        /// <summary>
        /// 选项
        /// </summary>
        public GButton btnSetting;
        /// <summary>
        /// 退出
        /// </summary>
        public GButton btnExit;
    }
}