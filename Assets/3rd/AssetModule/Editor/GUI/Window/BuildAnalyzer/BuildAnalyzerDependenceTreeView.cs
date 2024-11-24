using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 依赖显示信息
    /// </summary>
    public class DependenceTreeInfo
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title;

        /// <summary>
        /// 内容列表
        /// </summary>
        public List<string> dataList;
    }

    /// <summary>
    /// 分析窗口的依赖列表
    /// </summary>
    public class BuildAnalyzerDependenceTreeView : TreeView
    {
        /// <summary>
        /// 所在的窗口
        /// </summary>
        readonly BuildAnalyzerWindow _parentWindow;

        /// <summary>
        /// 显示的信息列表
        /// </summary>
        List<DependenceTreeInfo> _dependenceTreeInfoList = new();

        /// <summary>
        /// 记录跳转前GroupTreeView当前所选中的ID列表
        /// </summary>
        readonly List<int> _groupSelectedItemIDListBeforeJump = new();

        public BuildAnalyzerDependenceTreeView(BuildAnalyzerWindow window) : base(new TreeViewState())
        {
            _parentWindow = window;
            Reload();
        }

        /// <summary>
        /// 设置显示的依赖或引用显示
        /// </summary>
        public void SetDependenceInfo(List<DependenceTreeInfo> depInfoList)
        {
            List<int> originalExpandIndexList = null;
            if (rootItem.hasChildren)
            {
                // 记录刷新前展开的项
                originalExpandIndexList = new List<int>();
                for (int i = 0; i < rootItem.children.Count; i++)
                    if (IsExpanded(rootItem.children[i].id))
                        originalExpandIndexList.Add(i);
            }

            _dependenceTreeInfoList = depInfoList;
            Reload();

            if (rootItem.hasChildren && originalExpandIndexList != null)
            {
                // 收缩全部后展开原来有展开的项
                CollapseAll();
                for (int i = 0; i < rootItem.children.Count; i++)
                    if (originalExpandIndexList.Contains(i))
                        SetExpanded(rootItem.children[i].id, true);
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(-1, -1);
            foreach (DependenceTreeInfo info in _dependenceTreeInfoList)
            {
                TreeViewItem titleItem = new TreeViewItem(info.title.GetHashCode(), 0, info.title);
                root.AddChild(titleItem);
                if (info.dataList == null || info.dataList.Count == 0)
                {
                    string none = "无";
                    titleItem.AddChild(new TreeViewItem(none.GetHashCode(), 1, none));
                }
                else
                    foreach (string data in info.dataList)
                        titleItem.AddChild(new TreeViewItem(data.GetHashCode(), 1, data));
            }

            return root;
        }

        /// <summary>
        /// 重写后可支持空列表
        /// </summary>
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return root.hasChildren ? base.BuildRows(root) : new List<TreeViewItem>();
        }

        public override void OnGUI(Rect rect)
        {
            if (!rootItem.hasChildren)
            {
                base.OnGUI(rect);
                return;
            }

            // 缩小留空间给右侧操作按钮
            rect.width -= 75;

            base.OnGUI(rect);

            int selectedID = 0;
            TreeViewItem selectedItem = null;
            var selection = GetSelection();
            if (selection.Count > 0)
            {
                selectedID = selection[0];
                selectedItem = FindItem(selectedID, rootItem);
            }

            UnityEngine.GUI.enabled = selectedItem != null && selectedItem.depth == 1 && !selectedItem.displayName.Equals("无");

            // 上方跳转按钮
            Rect searchJumpBtnRect = new Rect(rect.width + 5, rect.y + 5, 63, 22);
            if (UnityEngine.GUI.Button(searchJumpBtnRect, new GUIContent("上方跳转")))
            {
                int findItemID = !selectedItem.displayName.StartsWith("*") ? selectedID : selectedItem.displayName.Substring(1).GetHashCode();
                RecordAndPingGroupTreeViewItem(findItemID);
            }

            UnityEngine.GUI.enabled = _groupSelectedItemIDListBeforeJump.Count > 0;

            // 跳转返回按钮
            Rect jumpBackBtnRect = new Rect(rect.width + 5, searchJumpBtnRect.yMax + 5, 63, 22);
            if (UnityEngine.GUI.Button(jumpBackBtnRect, "跳转返回"))
            {
                int lastIndex = _groupSelectedItemIDListBeforeJump.Count - 1;
                int backItemID = _groupSelectedItemIDListBeforeJump[lastIndex];
                _groupSelectedItemIDListBeforeJump.RemoveAt(lastIndex);
                _parentWindow.GroupTreeView.PingItem(backItemID);
            }

            // 清除跳转记录按钮
            Rect clearRecordBtnRect = new Rect(rect.width + 5, jumpBackBtnRect.yMax + 5, 63, 22);
            if (UnityEngine.GUI.Button(clearRecordBtnRect, "清除记录"))
                _groupSelectedItemIDListBeforeJump.Clear();

            UnityEngine.GUI.enabled = true;
        }

        /// <summary>
        /// 让GroupTreeView选中并跳转到指定的Item位置, 并记录跳转前GroupTreeView选中的ItemID
        /// </summary>
        void RecordAndPingGroupTreeViewItem(int itemID)
        {
            var groupSelection = _parentWindow.GroupTreeView.GetSelection();
            int curGroupTreeViewSelectedID = groupSelection[^1]; // 在跳转前获取当前选中ItemID
            if (!_parentWindow.GroupTreeView.PingItem(itemID))
                return;

            if (curGroupTreeViewSelectedID == itemID)
                return;

            int count = _groupSelectedItemIDListBeforeJump.Count;
            if (count > 0 && _groupSelectedItemIDListBeforeJump[count - 1] == curGroupTreeViewSelectedID)
                return;

            _groupSelectedItemIDListBeforeJump.Add(curGroupTreeViewSelectedID);
        }

        /// <summary>
        /// 双击处理
        /// </summary>
        protected override void DoubleClickedItem(int id)
        {
            // 先寻找资源, 若有则选择资源
            string assetPath = FindItem(id, rootItem).displayName;
            if (assetPath.StartsWith("*")) // 部分资源名会有*号提示
                assetPath = assetPath[1..];
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (obj)
            {
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
            else // 若没有则选择组资源列表中的item
                RecordAndPingGroupTreeViewItem(id);
        }

        /// <summary>
        /// 选择变化处理(此处让TreeView变更为只支持单选)
        /// </summary>
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count > 1)
                SetSelection(new List<int>() { selectedIds[selectedIds.Count - 1] });
        }

        /// <summary>
        /// 右键处理, 弹出菜单复制内容
        /// </summary>
        protected override void ContextClickedItem(int id)
        {
            TreeViewItem item = FindItem(id, rootItem);
            if (item is not { depth: > 0 })
                return;

            string assetPath = item.displayName;
            if (assetPath == "无")
                return;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("复制"), false, () =>
            {
                if (assetPath.StartsWith("*")) // 部分资源名会有*号提示
                    GUIUtility.systemCopyBuffer = assetPath[1..];
                else
                    GUIUtility.systemCopyBuffer = assetPath;
            });
            menu.ShowAsContext();
        }
    }
}