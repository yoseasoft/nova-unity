/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-16
/// 功能描述：
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;

using GameEngine;
using System.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// Loading场景固定UI逻辑处理类
    /// </summary>
    public static class LoadingSceneFixedUISystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        private static void Awake(this LoadingScene self)
        {
            Debugger.Log("欢迎进入Loading场景固定UI业务流程！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        private static void Start(this LoadingScene self)
        {
            CreateLoadingPanel().Forget();
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        private static void Destroy(this LoadingScene self)
        {
            GameObject winPanelObject = GameObject.Find("WinPanel");
            if (null != winPanelObject)
            {
                GameObject.Destroy(winPanelObject);
            }

            NE.GuiHandler.CloseAllUI();

            Debugger.Log("当前正在销毁并退出Loading场景固定UI业务流程！");
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        private static void Update(this LoadingScene self)
        {
        }

        /// <summary>
        /// 创建Loading界面
        /// </summary>
        static async UniTaskVoid CreateLoadingPanel()
        {
            LoadingPanel loadingPanel = (LoadingPanel) await GuiHandler.Instance.OpenUI("LoadingPanel");
            loadingPanel?.StartProgress(0);

            // ItemChoicePanel itemChoicePanel = (ItemChoicePanel) await GuiHandler.Instance.OpenUI("ItemChoicePanel");

            //GameObject canvasObject = GameObject.Find("DynamicCanvas");
            //if (null == canvasObject)
            //{
            //    Debugger.Info("当前正在创建新的‘DynamicCanvas’节点！");
            //    canvasObject = new GameObject("DynamicCanvas");
            //    Canvas canvas = canvasObject.AddComponent<Canvas>();
            //    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            //    CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            //    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            //    canvasScaler.referenceResolution.Set(1920, 1080);
            //    canvasObject.AddComponent<GraphicRaycaster>();
            //}
            //else
            //{
            //    Debugger.Info("当前正在复用框架默认的‘DynamicCanvas’节点！");
            //}

            //GameObject eventSystemObject = GameObject.Find("DynamicEventSystem");
            //if (null == eventSystemObject)
            //{
            //    Debugger.Info("当前正在创建新的‘DynamicEventSystem’节点！");
            //    eventSystemObject = new GameObject("DynamicEventSystem");
            //    EventSystem eventSystem = eventSystemObject.AddComponent<EventSystem>();
            //    StandaloneInputModule standaloneInputModule = eventSystemObject.AddComponent<StandaloneInputModule>();
            //}
            //else
            //{
            //    Debugger.Info("当前正在复用框架默认的‘DynamicEventSystem’节点！");
            //}

            //string url = "Assets/_Resources/UGui/ItemChoicePanel/Main.prefab";
            ////GameObject winPanel = (GameObject) ResourceHandler.Instance.LoadAsset(url, typeof(GameObject));
            //GameObject winPanel = await ResourceHandler.Instance.LoadAssetAsync<GameObject>(url);
            ////if (null == asset) Debugger.Warn("asset is null");
            ////else
            ////{
            ////    Debugger.Warn("{%t}", asset.result);
            ////}
            //GameObject winPanelObject = GameObject.Instantiate(winPanel, canvasObject.transform);
            //ResourceHandler.Instance.UnloadAsset(winPanel);
            //// ResourceHandler.Instance.UnloadAsset(asset);
            //winPanelObject.name = "WinPanel";
        }
    }
}
