/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-10-31
/// 功能描述：
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public static class ItemChoicePanelSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        private static void Start(this ItemChoicePanel self)
        {
            //GComponent contentPane = self.Window as GComponent;

            //self.title = contentPane.GetChild("lab_title") as GTextField;
            //self.text = contentPane.GetChild("lab_text") as GTextField;
            //self.start = contentPane.GetChild("btn_start") as GButton;
            //self.progressBar = contentPane.GetChild("schedule") as GProgressBar;
            //contentPane.onClick.Add(OnClick);

            //self.start.visible = false;

            GameObject root = self.Window as GameObject;
            Transform trans = root.transform.Find("BtnOk");
            self.ok = trans.GetComponent<Button>();

            trans = root.transform.Find("BtnCancel");
            self.cancel = trans.GetComponent<Button>();

            self.ok.onClick.AddListener(OnOkClick);
            self.cancel.onClick.AddListener(OnCancelClick);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        private static void Destroy(this ItemChoicePanel self)
        {
            self.ok.onClick.RemoveAllListeners();
            self.cancel.onClick.RemoveAllListeners();
        }

        static void OnOkClick()
        {
            Debugger.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }

        static void OnCancelClick()
        {
            Debugger.Info("????????????????????????????????????????????");
        }
    }
}