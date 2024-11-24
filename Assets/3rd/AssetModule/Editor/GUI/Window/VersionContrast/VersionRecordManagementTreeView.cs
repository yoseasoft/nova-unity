using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using AssetModule.Editor.Build;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 版本记录管理窗口的列表Item
    /// </summary>
    public sealed class VersionRecordManagementTreeViewItem : TreeViewItem
    {
        /// <summary>
        /// 文件名字
        /// </summary>
        public readonly string fileName;

        /// <summary>
        /// 版本备注
        /// </summary>
        public string comment;

        public VersionRecordManagementTreeViewItem(string fileName, string comment = "") : base(fileName.GetHashCode(), 0)
        {
            this.fileName = fileName;
            this.comment = comment;
            displayName = VersionContrastUtils.ToShowName(fileName);
        }
    }

    /// <summary>
    /// 版本记录管理窗口的列表
    /// </summary>
    public class VersionRecordManagementTreeView : TreeView
    {
        public VersionRecordManagementTreeView() : base(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(GetColumns())))
        {
            showBorder = true;                    // 显示边框
            showAlternatingRowBackgrounds = true; // 每行背景颜色不一样, 更清晰

            // 刷新备注列表
            RefreshCommentList();
        }

        /// <summary>
        /// 列表根节点
        /// </summary>
        TreeViewItem _root;

        protected override TreeViewItem BuildRoot()
        {
            return _root;
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
                    width = 260,
                    minWidth = 260,
                    canSort = false,
                    autoResize = true,
                    headerContent = new GUIContent("版本"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    width = 200,
                    minWidth = 200,
                    canSort = false,
                    autoResize = true,
                    headerContent = new GUIContent("备注"),
                    headerTextAlignment = TextAlignment.Center
                }
            };
        }

        /// <summary>
        /// 刷新备注列表
        /// </summary>
        internal void RefreshCommentList()
        {
            _root = new TreeViewItem(-1, -1);

            List<string> versionFileNameList = new List<string>();
            Dictionary<string, string> recordFileNameToComment = new Dictionary<string, string>();
            VersionContrastUtils.GetVersionFileNameList(versionFileNameList);
            VersionContrastUtils.LoadCommentDataAndRefreshCommentDictionary(recordFileNameToComment);

            foreach (string fileName in versionFileNameList)
            {
                string comment = recordFileNameToComment.TryGetValue(fileName, out string content) ? content : string.Empty;
                _root.AddChild(new VersionRecordManagementTreeViewItem(fileName, comment));
            }

            Reload();
        }

        /// <summary>
        /// 保存最新注释
        /// </summary>
        public void SaveComment()
        {
            string recordCommentFilePath = BuildUtils.TranslateToBuildPath(AssetPath.CombinePath(BuildUtils.BuildRecordFolderName, RecordComment.RecordCommentFileName));
            File.Delete(recordCommentFilePath);

            List<string> fileNameList = new List<string>();
            List<string> commentList = new List<string>();
            foreach (VersionRecordManagementTreeViewItem item in _root.children.Cast<VersionRecordManagementTreeViewItem>())
            {
                if (!string.IsNullOrEmpty(item.comment))
                {
                    fileNameList.Add(item.fileName);
                    commentList.Add(item.comment);
                }
            }

            if (fileNameList.Count == 0)
                return;

            RecordComment recordComment = new RecordComment()
            {
                recordFileNameList = fileNameList.ToArray(),
                recordCommentList = commentList.ToArray()
            };
            File.WriteAllText(recordCommentFilePath, JsonUtility.ToJson(recordComment));
        }

        /// <summary>
        /// 列表行显示逻辑
        /// </summary>
        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                Rect cellRect = args.GetCellRect(i);
                VersionRecordManagementTreeViewItem item = args.item as VersionRecordManagementTreeViewItem;
                switch (args.GetColumn(i))
                {
                    case 0: // 第一列: 显示名字
                        if (string.IsNullOrEmpty(item.comment))
                            EditorGUI.LabelField(cellRect, item.displayName);
                        else
                            EditorGUI.LabelField(cellRect, $"{item.displayName}({item.comment})");
                        break;
                    case 1: // 第二列: 注释
                        item.comment = EditorGUI.TextField(cellRect, item.comment);
                        break;
                }
            }
        }

        /// <summary>
        /// 右键删除版本记录
        /// </summary>
        protected override void ContextClickedItem(int id)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("删除"), false, () =>
            {
                string buildRecordFolderPath = BuildUtils.TranslateToBuildPath(BuildUtils.BuildRecordFolderName);
                VersionRecordManagementTreeViewItem[] selectedItems = Array.ConvertAll(GetSelection().ToArray(), input => FindItem(input, _root) as VersionRecordManagementTreeViewItem);
                foreach (VersionRecordManagementTreeViewItem item in selectedItems)
                {
                    File.Delete(Path.Combine(buildRecordFolderPath, item.fileName));
                    item.parent.children.Remove(item);
                }

                if (EditorWindow.HasOpenInstances<VersionContrastWindow>())
                {
                    VersionContrastWindow versionContrastWindow = EditorWindow.GetWindow<VersionContrastWindow>();
                    versionContrastWindow.ResetValues();
                    versionContrastWindow.RefreshVersionFileNameList();
                }

                Reload();
            });
            menu.ShowAsContext();
        }
    }
}