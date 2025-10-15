
using FairyGUI;
using FairyGUI.Utils;

namespace Game
{
    public partial class DifficultyContent : GComponent
    {
        public GList m_level_list;
        public GTextField m_title;

        public const string URL = "ui://liq5u8kwi8121";

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_level_list = (GList)GetChildAt(1);
            m_title = (GTextField)GetChildAt(2);
        }
    }
}