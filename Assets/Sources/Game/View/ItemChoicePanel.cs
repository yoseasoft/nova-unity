using UnityEngine.UI;
using GameEngine;

namespace Game
{
    /// <summary>
    /// UGUI测试界面
    /// </summary>
    [CViewClass("ItemChoicePanel", ViewFormType.UGUI)]
    [CViewGroup("UGuiLevel")]
    public class ItemChoicePanel : CView
    {
        public Button ok;
        public Button cancel;
    }
}
