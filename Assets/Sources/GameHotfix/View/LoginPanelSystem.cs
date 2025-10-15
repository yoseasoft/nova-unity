/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-17
/// 功能描述：
/// </summary>

using FairyGUI;

namespace Game
{
    [GameEngine.Aspect]
    public static class LoginPanelSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        static void Start(LoginPanel self)
        {
            GComponent contentPane = self.Window as GComponent;

            self.btnStart = contentPane.GetChild("btn_start").asButton;
            self.btnContinue = contentPane.GetChild("btn_continue").asButton;
            self.btnSetting = contentPane.GetChild("btn_setting").asButton;
            self.btnExit = contentPane.GetChild("btn_exit").asButton;

            self.SetListeners();
        }

        /// <summary>
        /// 设置按钮监听
        /// </summary>
        static void SetListeners(this LoginPanel self)
        {
            self.btnStart.onClick.Set(self.OnClickStart);
            self.btnContinue.onClick.Set(self.OnClickContinue);
            self.btnSetting.onClick.Set(self.OnClickSetting);
            self.btnExit.onClick.Set(self.OnClickExit);
        }

        static void OnClickStart(this LoginPanel self)
        {
            //GameEngine.SceneHandler.Instance.ReplaceScene<DifficultyLevelScene>();
            NE.SceneHandler.ReplaceScene<BattleScene>();
        }

        static void OnClickContinue(this LoginPanel self)
        {
            GameEngine.GameApi.Send(3101, "AM", 9, "hello", 1);
            GameEngine.GameApi.Send(new MainSimpleNotifyItemData() { id = 101, name = "yukie", desc = "广东省广州市", level = 10, quality = 2 });
            NE.NetworkHandler.OnSimulationReceiveMessageOfProtoBuf(new Proto.MessageErrorNotify() { Cmd = "来自Login的测试MessageError通知消息", ErrorCode = 999 });
        }

        static void OnClickSetting(this LoginPanel self)
        {
            GameEngine.GameApi.Send(3102, "PM", 21, "bye", 2);

            NE.SceneHandler.GetCurrentScene()?.Fire(
                new MainSimpleNotifyItemData() { id = 102, name = "joy", desc = "广西省南宁市", level = 5, quality = 3 });

            NE.NetworkHandler.OnSimulationReceiveMessageOfProtoBuf(new Proto.KickNotify() { Reason = "来自Login的测试Kick通知消息" });
        }

        static void OnClickExit(this LoginPanel self)
        {
            NE.SceneHandler.ReplaceScene<LoadingScene>();
        }
    }
}