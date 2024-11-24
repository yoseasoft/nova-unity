using UnityEngine;
using UnityEditor;
using System.Collections;
using AssetModule.Editor.Build;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using Unity.EditorCoroutines.Editor;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 打包分析窗口
    /// </summary>
    public class BuildAnalyzerWindow : EditorWindow
    {
        /// <summary>
        /// 打开此窗口
        /// </summary>
        public static void Open()
        {
            GetWindow<BuildAnalyzerWindow>("资源打包分析").minSize = new Vector2(800, 500);
        }

        /// <summary>
        /// 是否重新加载
        /// </summary>
        bool _isReload;

        /// <summary>
        /// 是否重新根据当前选择清单显示
        /// </summary>
        bool _isReloadManifest;

        /// <summary>
        /// 是否已将新选择的清单显示在TreeView里
        /// </summary>
        bool _isReloadManifestInTreeView;

        /// <summary>
        /// 工具栏高度
        /// </summary>
        const int ToolBarHeight = 20;

        /// <summary>
        /// 打包的清单列表
        /// </summary>
        List<ManifestConfig> _manifestConfigList;

        /// <summary>
        /// 选中的清单配置索引
        /// </summary>
        int _selectedManifestConfigIndex;

        /// <summary>
        /// 选中的清单
        /// </summary>
        Manifest _selectedManifest;

        /// <summary>
        /// 清单名字GUIContent, 此处声明仅为了不一直在OnGUI中new
        /// </summary>
        readonly GUIContent _manifestNameGUIContent = new("Manifest");

        /// <summary>
        /// 引用数量警告数值
        /// </summary>
        public int BundleDependenceWarningNum { get; private set; } = 3;

        /// <summary>
        /// 是否显示无被引用Prefab
        /// </summary>
        public static bool IsShowPrefabUnused { get; private set; } = false;

        /// <summary>
        /// 搜索框
        /// </summary>
        SearchField _searchField;

        /// <summary>
        /// 垂直分割工具
        /// </summary>
        VerticalSplitter _verticalSplitter;

        /// <summary>
        /// 组资源列表
        /// </summary>
        public BuildAnalyzerGroupTreeView GroupTreeView { get; private set; }

        /// <summary>
        /// 资源依赖列表
        /// </summary>
        public BuildAnalyzerDependenceTreeView DependenceTreeView { get; private set; }

        /// <summary>
        /// 警告值输入框居中样式
        /// </summary>
        GUIStyle _middleTextStyle;

        /// <summary>
        /// Prefab勾选框文本样式
        /// </summary>
        GUIStyle _prefabToggleTextStyle;

        /// <summary>
        /// 继续/暂停文字Style
        /// </summary>
        GUIStyle _pauseBtnLabelStyle;

        void OnEnable()
        {
            _isReload = true;
        }

        /// <summary>
        /// 异步获取所有清单配置, 防止打开窗口时卡顿导致白屏
        /// </summary>
        IEnumerator GetAllManifestConfigs()
        {
            _manifestConfigList = BuildUtils.GetAllManifestConfigs();
            yield return null;
        }

        void OnGUI()
        {
            _pauseBtnLabelStyle ??= new GUIStyle("button") { fontSize = 11 };

            if (_isReload)
            {
                _isReload = false;
                _isReloadManifest = true;
                _selectedManifestConfigIndex = 0;
                GroupTreeView?.StopAllCoroutines();
                _manifestConfigList = null;
                this.StartCoroutine(GetAllManifestConfigs());
            }

            // 列表未初始化完成, 不显示任何内容
            if (_manifestConfigList == null)
            {
                GUILayout.Label(string.Empty);
                return;
            }

            if (_manifestConfigList.Count == 0)
            {
                GUILayout.Label("没有读取到任何资源清单, 请先配置好资源清单并进行打包后再打开此窗口");
                return;
            }

            Rect toolBarRect = new Rect(0, 0, position.width, ToolBarHeight);
            DrawToolBar(toolBarRect);

            if (_isReloadManifest)
            {
                _isReloadManifest = false;
                _isReloadManifestInTreeView = true;
                GroupTreeView?.StopAllCoroutines();
                _selectedManifest = BuildUtils.GetManifest(_manifestConfigList[_selectedManifestConfigIndex].name);
            }

            // 为何这里又要写多次获取selectedManifest?
            // 因为selectedManifest是ScriptableObject.CreateInstance创建出来的, 切换场景时候会被Destroy掉
            // 为避免打开此窗口时切换场景导致selectedManifest = null, 故发现空的时候再尝试获取一次, 获取不到才是真的空
            if (!_selectedManifest)
                _selectedManifest = BuildUtils.GetManifest(_manifestConfigList[_selectedManifestConfigIndex].name);

            if (!_selectedManifest)
            {
                GUILayout.Space(ToolBarHeight);
                GUILayout.Label("请先对此清单进行打包后再查看此清单的打包分析");
                return;
            }

            if (_selectedManifest.manifestBundleInfoList.Count == 0)
            {
                GUILayout.Space(ToolBarHeight);
                GUILayout.Label("此清单中没有任何资源, 若已废弃, 可直接删除此清单");
                return;
            }

            if (_isReloadManifestInTreeView)
            {
                GroupTreeView = new BuildAnalyzerGroupTreeView(this);
                DependenceTreeView = new BuildAnalyzerDependenceTreeView(this);
                _isReloadManifestInTreeView = false;
                GroupTreeView.SetManifestConfig(_manifestConfigList[_selectedManifestConfigIndex], _selectedManifest);
            }

            if (!GroupTreeView.IsInitFinished)
            {
                Rect rect = new Rect(position.width * 0.5f - position.width * 0.4f, position.height * 0.46f, position.width * 0.8f, 20);
                EditorGUI.ProgressBar(rect, GroupTreeView.InitProgress, "初始化中");
                return;
            }

            _verticalSplitter ??= new VerticalSplitter() { MinHeight = 86 };

            Rect contentRect = new Rect(0, ToolBarHeight + 5, position.width, position.height - (ToolBarHeight + 5));
            _verticalSplitter.OnGUI(contentRect);
            if (_verticalSplitter.IsResizing)
                Repaint();

            GroupTreeView.OnGUI(new Rect(0, contentRect.y, position.width, _verticalSplitter.rect.y - 25));
            DependenceTreeView.OnGUI(new Rect(0, _verticalSplitter.rect.y, position.width, position.height - _verticalSplitter.rect.y));
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
                    // 清单下拉框
                    _manifestNameGUIContent.text = _manifestConfigList[_selectedManifestConfigIndex].name;
                    Rect manifestDropDownRect = new Rect(0, 0, 150, ToolBarHeight);
                    if (EditorGUI.DropdownButton(manifestDropDownRect, _manifestNameGUIContent, FocusType.Keyboard, EditorStyles.toolbarDropDown))
                    {
                        GenericMenu menu = new GenericMenu();
                        for (int i = 0; i < _manifestConfigList.Count; i++)
                            menu.AddItem(new GUIContent(_manifestConfigList[i].name), _selectedManifestConfigIndex == i, data =>
                            {
                                _selectedManifestConfigIndex = (int)data;
                                _isReloadManifest = true;
                            }, i);
                        menu.DropDown(manifestDropDownRect);
                    }

                    // 警告数量输入框
                    if (GroupTreeView != null && GroupTreeView.IsInitFinished)
                    {
                        Rect warningTipsRect = new Rect(manifestDropDownRect.xMax + 5, manifestDropDownRect.y + 1, 90, manifestDropDownRect.height - 2);
                        EditorGUI.LabelField(warningTipsRect, "引用数量警告值");

                        _middleTextStyle ??= new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleCenter };
                        Rect warningNumRect = new Rect(warningTipsRect.xMax, warningTipsRect.y + 1, 30, warningTipsRect.height - 2);
                        BundleDependenceWarningNum = EditorGUI.IntField(warningNumRect, BundleDependenceWarningNum, _middleTextStyle);
                        BundleDependenceWarningNum = Mathf.Clamp(BundleDependenceWarningNum, 1, 100);
                    }

                    // 搜索框位置
                    Rect searchFieldRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.toolbarSearchField);
                    searchFieldRect = new Rect(position.width - 230, searchFieldRect.y, 230, searchFieldRect.height);

                    if (GroupTreeView != null && GroupTreeView.IsInitFinished)
                    {
                        if (GroupTreeView.IsAssetDepAnalyzeFinished)
                        {
                            // Prefab无用提示勾选框位置
                            Rect prefabToggleRect = new Rect(searchFieldRect.x - 128 - 5, searchFieldRect.y, 128, searchFieldRect.height);
                            _prefabToggleTextStyle ??= new GUIStyle(EditorStyles.toggle) { contentOffset = new Vector2(0, -1) };
                            IsShowPrefabUnused = UnityEngine.GUI.Toggle(prefabToggleRect, IsShowPrefabUnused, "显示无被引用Prefab", _prefabToggleTextStyle);
                        }
                        else
                        {
                            if (GroupTreeView.isStartedAnalyzedAssetDep)
                            {
                                // 继续/暂停按钮位置
                                Rect stateBtnRect = new Rect(searchFieldRect.x - 38 - 5, searchFieldRect.y, 38, searchFieldRect.height);
                                if (UnityEngine.GUI.Button(stateBtnRect, GroupTreeView.isAllowAnalyzedAssetDep ? "暂停" : "继续", _pauseBtnLabelStyle))
                                    GroupTreeView.isAllowAnalyzedAssetDep = !GroupTreeView.isAllowAnalyzedAssetDep;

                                // 依赖分析进度条位置
                                float barWidth = (stateBtnRect.x - manifestDropDownRect.xMax) * 0.3f;
                                Rect progressBarRect = new Rect(stateBtnRect.x - barWidth - 1, searchFieldRect.y + 1, barWidth, searchFieldRect.height - 2);

                                // 资源依赖分析标题位置
                                Rect progressTitleRect = new Rect(progressBarRect.x - 43, progressBarRect.y + 1, 90, progressBarRect.height - 2);
                                EditorGUI.LabelField(progressTitleRect, "分析中:");
                                string progressStr = (System.Math.Round(GroupTreeView.AssetDepAnalyzeProgress, 2) * 100).ToString() + "%";
                                EditorGUI.ProgressBar(progressBarRect, GroupTreeView.AssetDepAnalyzeProgress, GroupTreeView.isAllowAnalyzedAssetDep ? progressStr : progressStr + "(已暂停)");
                            }
                            else
                            {
                                // 资源依赖分析按钮位置
                                Rect analyzeBtnRect = new Rect(searchFieldRect.x - 90 - 5, searchFieldRect.y, 90, searchFieldRect.height);
                                if (UnityEngine.GUI.Button(analyzeBtnRect, "资源依赖分析"))
                                    GroupTreeView.StartAnalyzeAllAssetDepedenciesAsync();
                            }
                        }
                    }

                    // 搜索框
                    if (_searchField == null)
                    {
                        _searchField = new SearchField();
                        _searchField.downOrUpArrowKeyPressed += () =>
                        {
                            if (GroupTreeView != null && GroupTreeView.IsInitFinished)
                                GroupTreeView.SetFocusAndEnsureSelectedItem();
                        };
                    }

                    if (GroupTreeView != null)
                        GroupTreeView.searchString = _searchField.OnToolbarGUI(searchFieldRect, GroupTreeView.searchString);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
    }
}