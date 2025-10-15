using FairyGUI;
using FairyGUI.Utils;
using Game.Config;
using Game;

namespace Game
{
    static class SymbolCardSystem
    {
        public static void Init(this SymbolCard self, int id)
        {
            SymbolConfig symbolConfig = SymbolConfigTable.Get(id);
            self.title = symbolConfig.name;
            self.iconImg.url = symbolConfig.iconPath;
            self.txtGold.text = "给与" + symbolConfig.initCoin + "个";
            self.txtDesc.text = symbolConfig.positiveText;
        }
    }
}