using UnityEngine;
using UnityEditor;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 资源运行分析窗口
    /// </summary>
    public class RuntimeAnalyzerWindow : EditorWindow
    {
        /// <summary>
        /// 打开此窗口
        /// </summary>
        public static void Open()
        {
            GetWindow<RuntimeAnalyzerWindow>("资源运行分析").minSize = new Vector2(800, 500);
        }

        /// <summary>
        /// 工具栏高度
        /// </summary>
        const int ToolBarHeight = 20;

        /// <summary>
        /// 资源列表
        /// </summary>
        RuntimeAnalyzerAssetTreeView _assetTreeView;

        /// <summary>
        /// 是否需要自动刷新
        /// </summary>
        bool _needAutoRefresh = true;

        void OnGUI()
        {
            _assetTreeView ??= new RuntimeAnalyzerAssetTreeView();

            Rect toolBarRect = new Rect(0, 0, position.width, ToolBarHeight);
            DrawToolBar(toolBarRect);
            _assetTreeView.OnGUI(new Rect(0, ToolBarHeight + 2, position.width, position.height - ToolBarHeight - 2));
        }

        void Update()
        {
            _assetTreeView?.Update();
        }

        /// <summary>
        /// 绘制最上方的工具栏
        /// </summary>
        void DrawToolBar(Rect toolBarRect)
        {
            GUILayout.BeginArea(toolBarRect, EditorStyles.toolbar);
            {
                GUILayout.BeginHorizontal();
                {
                    // 自动刷新开关位置
                    Rect autoRefreshBtnRect = new Rect(0, 0, 100, toolBarRect.height);
                    _needAutoRefresh = UnityEngine.GUI.Toggle(autoRefreshBtnRect, _needAutoRefresh, "自动刷新", EditorStyles.toolbarButton);
                    _assetTreeView.needAutoRefresh = _needAutoRefresh;
                    if (!_needAutoRefresh)
                    {
                        // 刷新按钮位置
                        Rect refreshBtnRect = new Rect(autoRefreshBtnRect.x + autoRefreshBtnRect.width, 0, 50, toolBarRect.height);
                        if (UnityEngine.GUI.Button(refreshBtnRect, "刷新", EditorStyles.toolbarButton))
                        {
                            if (UnityEngine.Application.isPlaying)
                                _assetTreeView.RefreshAssetList();
                            else
                                ShowNotification(new GUIContent("请在游戏运行中刷新资源列表"));
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
    }
}