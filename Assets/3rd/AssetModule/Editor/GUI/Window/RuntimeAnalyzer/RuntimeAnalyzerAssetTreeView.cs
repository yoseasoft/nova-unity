using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 运行分析窗口列表的Item
    /// </summary>
    public sealed class RuntimeAnalyzerTreeViewItem : TreeViewItem
    {
        /// <summary>
        /// 引用数量
        /// </summary>
        public readonly int refCount;

        public RuntimeAnalyzerTreeViewItem(string assetPath, int refCount) : base(assetPath.GetHashCode(), 0)
        {
            displayName = assetPath;
            this.refCount = refCount;
            icon = AssetDatabase.GetCachedIcon(assetPath) as Texture2D;
        }
    }

    /// <summary>
    /// 运行分析窗口的资源列表
    /// </summary>
    public class RuntimeAnalyzerAssetTreeView : TreeView
    {
        public RuntimeAnalyzerAssetTreeView() : base(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(GetColumns())))
        {
            showAlternatingRowBackgrounds = true;
            multiColumnHeader.ResizeToFit();
            multiColumnHeader.sortingChanged += OnSortingChanged;
            RefreshAssetList();
        }

        /// <summary>
        /// 获取TreeView顶部菜单目录
        /// </summary>
        static MultiColumnHeaderState.Column[] GetColumns()
        {
            return new[]
            {
                new MultiColumnHeaderState.Column
                {
                    width = 500,
                    minWidth = 500,
                    canSort = true,
                    autoResize = true,
                    headerContent = new GUIContent("资源路径"),
                    headerTextAlignment = TextAlignment.Center,
                },
                new MultiColumnHeaderState.Column
                {
                    width = 298,
                    minWidth = 298,
                    canSort = true,
                    autoResize = true,
                    headerContent = new GUIContent("引用数量"),
                    headerTextAlignment = TextAlignment.Center,
                }
            };
        }

        /// <summary>
        /// 列表根节点
        /// </summary>
        readonly TreeViewItem _root = new(-1, -1);

        protected override TreeViewItem BuildRoot()
        {
            return _root;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                Rect cellRect = args.GetCellRect(i);
                RuntimeAnalyzerTreeViewItem item = (RuntimeAnalyzerTreeViewItem)args.item;
                switch (args.GetColumn(i))
                {
                    case 0: // 第一列:资源路径
                        Rect iconRect = Rect.zero;
                        if (item.icon)
                        {
                            iconRect = new Rect(cellRect.x + 1, cellRect.y + 1, cellRect.height - 2, cellRect.height - 2);
                            UnityEngine.GUI.DrawTexture(iconRect, item.icon, ScaleMode.ScaleToFit);
                        }

                        Rect displayNameRect = new Rect(cellRect.x + iconRect.width + 1, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                        EditorGUI.LabelField(displayNameRect, item.displayName, DefaultStyles.label);
                        break;
                    case 1: // 第二列:引用数量
                        DefaultGUI.Label(cellRect, item.refCount.ToString(), args.selected, args.focused);
                        break;
                }
            }
        }

        /// <summary>
        /// 最后刷新时间记录
        /// </summary>
        float _lastRefreshTime;

        /// <summary>
        /// 是否需要自动刷新
        /// </summary>
        public bool needAutoRefresh;

        public void Update()
        {
            if (needAutoRefresh && Time.realtimeSinceStartup - _lastRefreshTime > 0.3f)
                RefreshAssetList();
        }

        /// <summary>
        /// 刷新资源显示列表
        /// </summary>
        public void RefreshAssetList()
        {
            if (!UnityEngine.Application.isPlaying)
            {
                Reload();
                return;
            }

            if (_root.hasChildren)
                _root.children.Clear();

            foreach (Asset asset in AssetHandler.Cache.Values)
                _root.AddChild(new RuntimeAnalyzerTreeViewItem(asset.address, asset.reference.Count));

            SortAssetList();
            Reload();
            _lastRefreshTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 排序变动
        /// </summary>
        void OnSortingChanged(MultiColumnHeader header)
        {
            SortAssetList();
        }

        /// <summary>
        /// 显示列表排序
        /// </summary>
        static List<TreeViewItem> OrderTreeViewList<T>(List<TreeViewItem> treeView, bool ascending, Func<TreeViewItem, T> sortFunc)
        {
            return ascending ? treeView.OrderBy(sortFunc).ToList() : treeView.OrderByDescending(sortFunc).ToList();
        }

        /// <summary>
        /// 排序列表
        /// </summary>
        void SortAssetList()
        {
            if (!_root.hasChildren)
                return;

            int curIndex = multiColumnHeader.sortedColumnIndex;
            if (curIndex == -1)
                return;

            bool ascending = multiColumnHeader.IsSortedAscending(curIndex);

            if (curIndex == 0) // 资源路径
                _root.children = OrderTreeViewList(_root.children, ascending, item => ((RuntimeAnalyzerTreeViewItem)item).displayName);
            else if (curIndex == 1) // 引用数量
                _root.children = OrderTreeViewList(_root.children, ascending, item => ((RuntimeAnalyzerTreeViewItem)item).refCount);

            Reload();
        }

        /// <summary>
        /// 双击跳转资源
        /// </summary>
        protected override void DoubleClickedItem(int id)
        {
            TreeViewItem item = FindItem(id, rootItem);
            if (item == null)
                return;

            Object obj = AssetDatabase.LoadAssetAtPath<Object>(item.displayName);
            if (!obj)
                return;

            // 打开并聚焦到Project窗口
            EditorApplication.ExecuteMenuItem("Window/General/Project");

            // 使用ExecuteMenuItem打开窗口后, 不能立即显示选中效果, 所以延迟执行
            EditorApplication.delayCall += () =>
            {
                // 文件夹选中效果
                if (obj)
                    EditorGUIUtility.PingObject(obj);
            };
        }

        /// <summary>
        /// 右键复制资源路径
        /// </summary>
        protected override void ContextClickedItem(int id)
        {
            TreeViewItem item = FindItem(id, rootItem);
            if (item == null)
                return;

            string itemName = item.displayName;
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("复制"), false, () => { GUIUtility.systemCopyBuffer = itemName; });
            menu.ShowAsContext();
        }
    }
}