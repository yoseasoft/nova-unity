/// <summary>
/// 2024-05-29 Game Framework Code By Hurley
/// </summary>

using Cysharp.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// LOGO场景逻辑处理类
    /// </summary>
    public static class LogoSceneInitEnvSystem
    {
        [OnStart]
        static void Start(this LogoScene self)
        {
            InitUISettings();
            FairyGUI.GRoot.inst.onSizeChanged.Add(InitUISettings);

            CreateMainUI().Forget();
        }

        [OnUpdate]
        static void Update(this LogoScene self)
        {
        }

        [OnDestroy]
        static void Destroy(this LogoScene self)
        {
            Debugger.Log("当前正在销毁并退出Logo场景初始化环境业务！");
        }

        /// <summary>
        /// 初始化UI系统
        /// </summary>
        static void InitUISettings()
        {
            {
                // 普通屏幕
                FairyGUI.GRoot.inst.SetContentScaleFactor(1920, 1080);
            }

            // FairyGUI相机背景颜色
            FairyGUI.StageCamera.main.backgroundColor = UnityEngine.Color.clear;
            UnityEngine.GameObject.DontDestroyOnLoad(FairyGUI.StageCamera.main);
        }

        /// <summary>
        /// 创建主场景相关UI界面
        /// </summary>
        static async UniTaskVoid CreateMainUI()
        {
            Debugger.Log("预加载！");

            // 加载通用包
            await GameEngine.FormHelper.AddCommonPackage("CommonButton");

            //FairyGUI.UIObjectFactory.SetPackageItemExtension(DifficultyContent.URL, typeof(DifficultyContent));
            //FairyGUI.UIObjectFactory.SetPackageItemExtension(SymbolCard.URL, typeof(SymbolCard));

            //NE.SceneHandler.ReplaceScene<MainScene>();
        }
    }
}
