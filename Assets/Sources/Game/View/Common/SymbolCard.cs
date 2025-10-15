
using FairyGUI;
using FairyGUI.Utils;

namespace Game
{
    public partial class SymbolCard : GButton
    {
        public GLoader iconImg;
        public GTextField txtGold;
        public GTextField txtDesc;
        public const string URL = "ui://ChoosePanel/红色卡牌";

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            iconImg = (GLoader) GetChild("icon_img");
            txtGold = (GTextField) GetChild("txt_gold");
            txtDesc = (GTextField) GetChild("txt_desc");
        }
    }
}