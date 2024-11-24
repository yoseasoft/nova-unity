using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections;
using AssetModule.Editor.Build;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using Object = UnityEngine.Object;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 版本对比窗口的列表Item
    /// </summary>
    public sealed class VersionContrastTreeViewItem : TreeViewItem
    {
        /// <summary>
        /// Item类型
        /// </summary>
        public enum ItemType
        {
            /// <summary>
            /// 组
            /// </summary>
            Group,

            /// <summary>
            /// 版本文件
            /// </summary>
            VersionFile,

            /// <summary>
            /// Bundle
            /// </summary>
            Bundle,

            /// <summary>
            /// 资源
            /// </summary>
            Asset
        }

        /// <summary>
        /// 改变类型
        /// </summary>
        public enum ChangeType
        {
            /// <summary>
            /// 新增
            /// </summary>
            Add,

            /// <summary>
            /// 删除
            /// </summary>
            Remove,

            /// <summary>
            /// 修改
            /// </summary>
            Modify,

            /// <summary>
            /// 一样
            /// </summary>
            Same
        }

        /// <summary>
        /// 信息(组名/包名/资源目录)
        /// </summary>
        public string info;

        /// <summary>
        /// 更新大小
        /// </summary>
        public long size;

        /// <summary>
        /// Item类型
        /// </summary>
        public ItemType itemType = ItemType.Bundle;

        /// <summary>
        /// 资源改变类型
        /// </summary>
        public ChangeType changeType = ChangeType.Modify;

        /// <summary>
        /// 是否有直接文件修改(仅BundleItem使用字段)
        /// </summary>
        public bool hasDirectFileChanged;

        /// <summary>
        /// 变化大小
        /// </summary>
        public long changedSize;

        public VersionContrastTreeViewItem(string info, int depth) : base(info.GetHashCode(), depth)
        {
            this.info = info;
            displayName = info; // 搜索用
            icon = AssetDatabase.GetCachedIcon(info) as Texture2D;
            if (!icon)
                icon = GetIconWhenAssetDeleted(info);
        }

        /// <summary>
        /// 文件后缀转缩略图
        /// </summary>
        static Dictionary<string, Texture2D> s_fileExtensionToIcon;

        /// <summary>
        /// 获取编辑器内置图标
        /// </summary>
        static Texture2D GetEditorTexture(string editorTextureName)
        {
            GUIContent content = EditorGUIUtility.IconContent(editorTextureName);
            if (content != null && content.image)
                return content.image as Texture2D;

            // 图标不存在时, 使用问号图标
            content = EditorGUIUtility.IconContent("d__Help");
            if (content != null && content.image)
                return content.image as Texture2D;

            return null;
        }

        /// <summary>
        /// 资源被删除时, 根据资源路径来返回缩略图
        /// </summary>
        static Texture2D GetIconWhenAssetDeleted(string assetPath)
        {
            if (!assetPath.StartsWith("Assets/"))
                return null;

            s_fileExtensionToIcon ??= new Dictionary<string, Texture2D>
            {
                ["unknown"] = GetEditorTexture("d__Help"),
                [".mat"] = GetEditorTexture("d_Material Icon"),
                [".prefab"] = GetEditorTexture("d_Prefab Icon"),
                [".bytes"] = GetEditorTexture("d_TextAsset Icon"),
                [".fbx"] = GetEditorTexture("d_PrefabModel Icon"),
                [".jpg"] = GetEditorTexture("d_TreeEditor.Trash"),
                [".png"] = GetEditorTexture("d_TreeEditor.Trash"),
                [".tga"] = GetEditorTexture("d_TreeEditor.Trash"),
                [".psd"] = GetEditorTexture("d_TreeEditor.Trash"),
                [".hdr"] = GetEditorTexture("d_TreeEditor.Trash"),
                [".exr"] = GetEditorTexture("d_TreeEditor.Trash"),
                [".anim"] = GetEditorTexture("d_AnimationClip Icon"),
                [".unity3d"] = GetEditorTexture("d_SceneAsset Icon"),
                [".playable"] = GetEditorTexture("d_TimelineAsset Icon"),
                [".asset"] = GetEditorTexture("d_ScriptableObject Icon"),
                [".controller"] = GetEditorTexture("d_AnimatorController Icon"),
            };

            string extension = Path.GetExtension(assetPath).ToLower();
            return s_fileExtensionToIcon.TryGetValue(extension, out Texture2D icon) ? icon : s_fileExtensionToIcon["unknown"];
        }
    }

    /// <summary>
    /// 版本对比窗口的组列表
    /// </summary>
    public class VersionContrastGroupTreeView : TreeView
    {
        public VersionContrastGroupTreeView(VersionContrastWindow parentWindow) : base(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(GetColumns())))
        {
            showBorder = true;                    // 显示边框
            showAlternatingRowBackgrounds = true; // 每行背景颜色不一样, 更清晰

            this._parentWindow = parentWindow;
            multiColumnHeader.sortingChanged += OnSortingChanged;

            // 初始化图标
            _changeTypeToGUITexture = new Dictionary<VersionContrastTreeViewItem.ChangeType, Texture2D>
            {
                [VersionContrastTreeViewItem.ChangeType.Add] = Resources.Load<Texture2D>("AssetModuleVersionContrast/Add"),
                [VersionContrastTreeViewItem.ChangeType.Remove] = Resources.Load<Texture2D>("AssetModuleVersionContrast/Remove"),
                [VersionContrastTreeViewItem.ChangeType.Modify] = Resources.Load<Texture2D>("AssetModuleVersionContrast/Modify")
            };
        }

        /// <summary>
        /// 所在窗口
        /// </summary>
        readonly VersionContrastWindow _parentWindow;

        /// <summary>
        /// 列表根节点
        /// </summary>
        TreeViewItem _root;

        /// <summary>
        /// 旧版本内容
        /// </summary>
        BuildRecord _oldBuildRecord;

        /// <summary>
        /// 新版本内容
        /// </summary>
        BuildRecord _newBuildRecord;

        /// <summary>
        /// 改变类型图标
        /// </summary>
        readonly Dictionary<VersionContrastTreeViewItem.ChangeType, Texture2D> _changeTypeToGUITexture;

        /// <summary>
        /// 大小变化文本样式
        /// </summary>
        GUIStyle _changedSizeTextStyle;

        /// <summary>
        /// 更新大小文本样式
        /// </summary>
        GUIStyle _updateSizeTextStyle;

        /// <summary>
        /// 版本对比分析进度
        /// </summary>
        public float InitProgress { get; private set; }

        /// <summary>
        /// 版本对比分析是否完成
        /// </summary>
        public bool IsInitFinished { get; private set; }

        /// <summary>
        /// 当前初始化协程
        /// </summary>
        EditorCoroutine _initCoroutine;

        /// <summary>
        /// 协程允许运行的最大时间, 超过则等到下一帧再运行
        /// </summary>
        const float MaxFrameTime = 0.02f;

        /// <summary>
        /// 上次进度条刷新时间
        /// </summary>
        double _lastUpdateTime;

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
                    width = 380,
                    minWidth = 380,
                    canSort = true,
                    autoResize = false,
                    headerContent = new GUIContent("旧版本"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    width = 380,
                    minWidth = 380,
                    canSort = false,
                    autoResize = false,
                    headerContent = new GUIContent("新版本"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    width = 35,
                    minWidth = 35,
                    canSort = false,
                    autoResize = false,
                    headerContent = new GUIContent("类型"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    width = 85,
                    minWidth = 85,
                    canSort = false,
                    autoResize = false,
                    headerContent = new GUIContent("大小变化"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    width = 85,
                    minWidth = 85,
                    canSort = true,
                    autoResize = false,
                    headerContent = new GUIContent("更新大小"),
                    headerTextAlignment = TextAlignment.Center
                }
            };
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _oldBuildRecord = null;
            _newBuildRecord = null;
        }

        /// <summary>
        /// 设置记录数据
        /// </summary>
        public void SetRecord(BuildRecord oldRecord, BuildRecord newRecord)
        {
            if (_oldBuildRecord == oldRecord && _newBuildRecord == newRecord)
                return;

            _oldBuildRecord = oldRecord;
            _newBuildRecord = newRecord;

            InitProgress = 0;
            IsInitFinished = false;
            if (_initCoroutine != null)
            {
                _parentWindow.StopCoroutine(_initCoroutine);
                _initCoroutine = null;
            }

            _initCoroutine = _parentWindow.StartCoroutine(RefreshTreeView());
        }

        /// <summary>
        /// 刷新列表显示
        /// </summary>
        IEnumerator RefreshTreeView()
        {
            _root = new TreeViewItem(-1, -1);

            float recordOldFileProportion = 0.1f;   // 记录旧文件和资源占比
            float showChangedFileProportion = 0.7f; // 显示改动和新增的文件占比
            float showDeletedFileProportion = 0.2f; // 显示删除的文件占比
            _lastUpdateTime = EditorApplication.timeSinceStartup;

            #region 版本或清单文件变动计算显示逻辑

            // 记录旧的版本或清掉文件Hash值, 方便后面分析改动和新增
            Dictionary<string, string> oldVersionFileNameToHash = new Dictionary<string, string>();
            Dictionary<string, long> oldVersionFileNameToSize = new Dictionary<string, long>();
            foreach (VersionFileInfo oldVersionFileInfo in _oldBuildRecord.versionFileInfoList)
            {
                oldVersionFileNameToHash.Add(oldVersionFileInfo.name, oldVersionFileInfo.hash);
                oldVersionFileNameToSize.Add(oldVersionFileInfo.name, oldVersionFileInfo.size);
            }

            // 版本文件组Item
            VersionContrastTreeViewItem versionGroupItem = null;

            // 显示所有版本文件, 并记录新版本文件, 方便后面显示已删除的清单
            Dictionary<string, bool> newVersionFileExist = new Dictionary<string, bool>();
            foreach (VersionFileInfo newVersionFileInfo in _newBuildRecord.versionFileInfoList)
            {
                newVersionFileExist.Add(newVersionFileInfo.name, true);

                if (versionGroupItem == null)
                {
                    versionGroupItem = new VersionContrastTreeViewItem("版本文件", 0) { itemType = VersionContrastTreeViewItem.ItemType.Group };
                    _root.AddChild(versionGroupItem);
                }

                VersionContrastTreeViewItem versionFileItem;
                if (!oldVersionFileNameToHash.TryGetValue(newVersionFileInfo.name, out string oldHash) || oldHash != newVersionFileInfo.hash)
                {
                    versionFileItem = new VersionContrastTreeViewItem(newVersionFileInfo.name, 1)
                    {
                        itemType = VersionContrastTreeViewItem.ItemType.VersionFile,
                        changeType = string.IsNullOrEmpty(oldHash) ? VersionContrastTreeViewItem.ChangeType.Add : VersionContrastTreeViewItem.ChangeType.Modify,
                        size = newVersionFileInfo.size,
                        changedSize = oldVersionFileNameToSize.TryGetValue(newVersionFileInfo.name, out long oldSize) ? newVersionFileInfo.size - oldSize : newVersionFileInfo.size
                    };
                    versionGroupItem.size += versionFileItem.size;
                    versionGroupItem.changedSize += versionFileItem.changedSize;
                }
                else
                {
                    versionFileItem = new VersionContrastTreeViewItem(newVersionFileInfo.name, 1)
                    {
                        itemType = VersionContrastTreeViewItem.ItemType.VersionFile,
                        changeType = VersionContrastTreeViewItem.ChangeType.Same,
                    };
                }

                versionGroupItem.AddChild(versionFileItem);
            }

            // 显示已删除的清单文件
            foreach (VersionFileInfo oldVersionFileInfo in _oldBuildRecord.versionFileInfoList)
            {
                if (newVersionFileExist.ContainsKey(oldVersionFileInfo.name))
                    continue;

                if (versionGroupItem == null)
                {
                    versionGroupItem = new VersionContrastTreeViewItem("版本文件", 0) { itemType = VersionContrastTreeViewItem.ItemType.Group };
                    _root.AddChild(versionGroupItem);
                }

                VersionContrastTreeViewItem versionFileItem = new VersionContrastTreeViewItem(oldVersionFileInfo.name, 1)
                {
                    itemType = VersionContrastTreeViewItem.ItemType.VersionFile,
                    changeType = VersionContrastTreeViewItem.ChangeType.Remove,
                    changedSize = -oldVersionFileInfo.size
                };
                versionGroupItem.AddChild(versionFileItem);
            }

            #endregion

            #region 资源变动计算显示逻辑

            // 记录旧的打包文件和资源文件Hash值, 方便后面分析改动和新增(FileName:最终打包文件(Bundle或RawFile)名, AssetPath:Unity里的资源路径)
            Dictionary<string, string> oldFileNameToHash = new Dictionary<string, string>();
            Dictionary<string, long> oldFileNameToSize = new Dictionary<string, long>();
            Dictionary<string, Dictionary<string, string>> oldFileNameToAssetPath2Hash = new Dictionary<string, Dictionary<string, string>>();

            int curOldInfoNum = 0;
            int totalOldInfoCount = 0;
            foreach (RecordManifest oldRecordManifest in _oldBuildRecord.recordManifestList)
                if (oldRecordManifest.recordBundleInfoList != null)
                    totalOldInfoCount += oldRecordManifest.recordBundleInfoList.Length;

            foreach (RecordManifest oldRecordManifest in _oldBuildRecord.recordManifestList)
            {
                if (oldRecordManifest.recordBundleInfoList == null)
                    continue;

                foreach (RecordBundleInfo oldBundleInfo in oldRecordManifest.recordBundleInfoList)
                {
                    curOldInfoNum++;
                    if (oldFileNameToHash.ContainsKey(oldBundleInfo.Name))
                        continue;

                    if (EditorApplication.timeSinceStartup - _lastUpdateTime > MaxFrameTime)
                    {
                        _lastUpdateTime = EditorApplication.timeSinceStartup;
                        InitProgress = recordOldFileProportion * ((float)curOldInfoNum / totalOldInfoCount);
                        _parentWindow.Repaint();
                        yield return null;
                    }

                    // 打包文件Hash值
                    oldFileNameToHash.Add(oldBundleInfo.Name, oldBundleInfo.Hash);

                    // 打包文件大小
                    oldFileNameToSize.Add(oldBundleInfo.Name, oldBundleInfo.Size);

                    // 包里所包含的资源路径Hash值
                    Dictionary<string, string> assetPathToHash = new Dictionary<string, string>();
                    foreach (RecordAssetInfo oldAssetInfo in oldBundleInfo.RecordAssetInfoList)
                        assetPathToHash.Add(oldAssetInfo.AssetPath, oldAssetInfo.Hash);
                    oldFileNameToAssetPath2Hash.Add(oldBundleInfo.Name, assetPathToHash);
                }
            }

            // 组名对应的Item记录
            Dictionary<string, VersionContrastTreeViewItem> groupNameToGroupItem = new Dictionary<string, VersionContrastTreeViewItem>();

            // 文件名(Bundle名)对应的Item记录
            Dictionary<string, VersionContrastTreeViewItem> fileNameToBundleItem = new Dictionary<string, VersionContrastTreeViewItem>();

            // 记录新的打包文件和资源文件是否存在, 方便后面显示删除的资源
            Dictionary<string, bool> newFileNameExist = new Dictionary<string, bool>();
            Dictionary<string, Dictionary<string, bool>> newFileNameToAssetPathExist = new Dictionary<string, Dictionary<string, bool>>();

            int curNewInfoNum = 0;
            int totalNewInfoCount = 0;
            foreach (RecordManifest newRecordManifest in _newBuildRecord.recordManifestList)
                if (newRecordManifest.recordBundleInfoList != null)
                    totalNewInfoCount += newRecordManifest.recordBundleInfoList.Length;

            // 分析改动和新增, 并记录新打包的文件和所包含的资源
            foreach (RecordManifest newRecordManifest in _newBuildRecord.recordManifestList)
            {
                if (newRecordManifest.recordBundleInfoList == null)
                    continue;

                foreach (RecordBundleInfo newBundleInfo in newRecordManifest.recordBundleInfoList)
                {
                    #region 记录新打包的文件和所包含的资源

                    curNewInfoNum++;
                    if (newFileNameExist.ContainsKey(newBundleInfo.Name))
                        continue;

                    if (EditorApplication.timeSinceStartup - _lastUpdateTime > MaxFrameTime)
                    {
                        _lastUpdateTime = EditorApplication.timeSinceStartup;
                        InitProgress = recordOldFileProportion + showChangedFileProportion * ((float)curNewInfoNum / totalNewInfoCount);
                        _parentWindow.Repaint();
                        yield return null;
                    }

                    // 打包文件名记录
                    newFileNameExist.Add(newBundleInfo.Name, true);

                    // 包里所包含的资源记录
                    Dictionary<string, bool> assetPathExist = new Dictionary<string, bool>();
                    foreach (RecordAssetInfo newAssetInfo in newBundleInfo.RecordAssetInfoList)
                        assetPathExist.Add(newAssetInfo.AssetPath, true);
                    newFileNameToAssetPathExist.Add(newBundleInfo.Name, assetPathExist);

                    #endregion

                    #region 显示新增和变动文件

                    if (oldFileNameToHash.TryGetValue(newBundleInfo.Name, out string hash) && hash.Equals(newBundleInfo.Hash))
                        continue;

                    if (!groupNameToGroupItem.TryGetValue(newBundleInfo.Group, out VersionContrastTreeViewItem groupItem))
                    {
                        groupItem = new VersionContrastTreeViewItem(newBundleInfo.Group, 0) { itemType = VersionContrastTreeViewItem.ItemType.Group };
                        groupNameToGroupItem.Add(newBundleInfo.Group, groupItem);
                        _root.AddChild(groupItem);
                    }

                    VersionContrastTreeViewItem bundleItem = new VersionContrastTreeViewItem(newBundleInfo.Name, 1)
                    {
                        itemType = !newBundleInfo.IsRawFile ? VersionContrastTreeViewItem.ItemType.Bundle : VersionContrastTreeViewItem.ItemType.Asset,
                        changeType = string.IsNullOrEmpty(hash) ? VersionContrastTreeViewItem.ChangeType.Add : VersionContrastTreeViewItem.ChangeType.Modify,
                        size = newBundleInfo.Size,
                        changedSize = oldFileNameToSize.TryGetValue(newBundleInfo.Name, out long oldSize) ? newBundleInfo.Size - oldSize : newBundleInfo.Size
                    };
                    groupItem.AddChild(bundleItem);
                    groupItem.size += newBundleInfo.Size;
                    groupItem.changedSize += bundleItem.changedSize;
                    fileNameToBundleItem.Add(newBundleInfo.Name, bundleItem);

                    foreach (RecordAssetInfo newAssetInfo in newBundleInfo.RecordAssetInfoList)
                    {
                        string oldHash = null;
                        if (!oldFileNameToAssetPath2Hash.TryGetValue(newBundleInfo.Name, out Dictionary<string, string> oldAssetPathToHash)
                            || !oldAssetPathToHash.TryGetValue(newAssetInfo.AssetPath, out oldHash) || oldHash != newAssetInfo.Hash)
                        {
                            VersionContrastTreeViewItem assetItem = new VersionContrastTreeViewItem(newAssetInfo.AssetPath, 2)
                            {
                                itemType = VersionContrastTreeViewItem.ItemType.Asset,
                                changeType = string.IsNullOrEmpty(oldHash) ? VersionContrastTreeViewItem.ChangeType.Add : VersionContrastTreeViewItem.ChangeType.Modify
                            };
                            bundleItem.AddChild(assetItem);
                            bundleItem.hasDirectFileChanged = true;
                        }
                        else
                        {
                            // 一样的也进行显示
                            VersionContrastTreeViewItem assetItem = new VersionContrastTreeViewItem(newAssetInfo.AssetPath, 2)
                            {
                                itemType = VersionContrastTreeViewItem.ItemType.Asset,
                                changeType = VersionContrastTreeViewItem.ChangeType.Same
                            };
                            bundleItem.AddChild(assetItem);
                        }
                    }

                    #endregion
                }
            }

            // 显示删除的文件和资源
            curOldInfoNum = 0;
            foreach (RecordManifest oldRecordManifest in _oldBuildRecord.recordManifestList)
            {
                if (oldRecordManifest.recordBundleInfoList == null)
                    continue;

                VersionContrastTreeViewItem bundleItem;
                foreach (RecordBundleInfo oldBundleInfo in oldRecordManifest.recordBundleInfoList)
                {
                    curOldInfoNum++;
                    if (EditorApplication.timeSinceStartup - _lastUpdateTime > MaxFrameTime)
                    {
                        _lastUpdateTime = EditorApplication.timeSinceStartup;
                        InitProgress = recordOldFileProportion + showChangedFileProportion + showDeletedFileProportion * ((float)curOldInfoNum / totalOldInfoCount);
                        _parentWindow.Repaint();
                        yield return null;
                    }

                    if (!newFileNameExist.ContainsKey(oldBundleInfo.Name))
                    {
                        if (!groupNameToGroupItem.TryGetValue(oldBundleInfo.Group, out VersionContrastTreeViewItem groupItem))
                        {
                            groupItem = new VersionContrastTreeViewItem(oldBundleInfo.Group, 0) { itemType = VersionContrastTreeViewItem.ItemType.Group };
                            groupNameToGroupItem.Add(oldBundleInfo.Group, groupItem);
                            _root.AddChild(groupItem);
                        }

                        // 添加删除的Bundle显示
                        bundleItem = new VersionContrastTreeViewItem(oldBundleInfo.Name, 1)
                        {
                            itemType = !oldBundleInfo.IsRawFile ? VersionContrastTreeViewItem.ItemType.Bundle : VersionContrastTreeViewItem.ItemType.Asset,
                            changeType = VersionContrastTreeViewItem.ChangeType.Remove,
                            changedSize = -oldBundleInfo.Size
                        };
                        groupItem.AddChild(bundleItem);
                        groupItem.changedSize += bundleItem.changedSize;
                        fileNameToBundleItem.Add(oldBundleInfo.Name, bundleItem);
                    }

                    // 添加Bundle里删除的资源显示
                    if (fileNameToBundleItem.TryGetValue(oldBundleInfo.Name, out bundleItem))
                    {
                        bool hasAddChild = false;
                        foreach (RecordAssetInfo oldAssetInfo in oldBundleInfo.RecordAssetInfoList)
                        {
                            if (newFileNameToAssetPathExist.TryGetValue(oldBundleInfo.Name, out Dictionary<string, bool> assetExist) && assetExist.ContainsKey(oldAssetInfo.AssetPath))
                                continue;

                            VersionContrastTreeViewItem assetItem = new VersionContrastTreeViewItem(oldAssetInfo.AssetPath, 2)
                            {
                                itemType = VersionContrastTreeViewItem.ItemType.Asset,
                                changeType = VersionContrastTreeViewItem.ChangeType.Remove
                            };
                            hasAddChild = true;
                            bundleItem.AddChild(assetItem);
                            bundleItem.hasDirectFileChanged = true;
                        }

                        // 排序资源列表, 避免删除的资源全部显示在列表最后
                        if (hasAddChild)
                        {
                            List<string> assetPathList = new List<string>();
                            Dictionary<string, VersionContrastTreeViewItem> assetPathToItem = new Dictionary<string, VersionContrastTreeViewItem>();
                            foreach (VersionContrastTreeViewItem item in bundleItem.children.Cast<VersionContrastTreeViewItem>())
                            {
                                assetPathList.Add(item.info);
                                assetPathToItem.Add(item.info, item);
                            }

                            assetPathList.Sort();
                            bundleItem.children.Clear();
                            foreach (string assetPath in assetPathList)
                                bundleItem.AddChild(assetPathToItem[assetPath]);
                        }
                    }
                }
            }

            #endregion

            InitProgress = 1;
            IsInitFinished = true;
            _initCoroutine = null;
            Reload();
            _parentWindow.Repaint();
        }

        /// <summary>
        /// 列表行显示逻辑
        /// </summary>
        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                VersionContrastTreeViewItem item = (VersionContrastTreeViewItem)args.item;

                Rect cellRect = args.GetCellRect(i);
                switch (args.GetColumn(i))
                {
                    case 0: // 第一列:旧记录数据
                        // 不是新增时才会显示在旧数据列
                        if (item.changeType != VersionContrastTreeViewItem.ChangeType.Add)
                        {
                            cellRect.xMin += GetContentIndent(item);
                            Rect iconRect = Rect.zero;
                            if (item.icon)
                            {
                                iconRect = new Rect(cellRect.x + 1, cellRect.y + 1, cellRect.height - 2, cellRect.height - 2);
                                UnityEngine.GUI.DrawTexture(iconRect, item.icon, ScaleMode.ScaleToFit);
                            }

                            Rect infoRect = new Rect(cellRect.x + iconRect.width + 1, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                            EditorGUI.LabelField(infoRect, item.info);
                        }

                        break;
                    case 1: // 第二列:新记录数据
                        if (item.itemType != VersionContrastTreeViewItem.ItemType.Group && item.changeType != VersionContrastTreeViewItem.ChangeType.Remove)
                        {
                            if (item.itemType == VersionContrastTreeViewItem.ItemType.Asset)
                                cellRect.xMin += 16;

                            Rect iconRect = Rect.zero;
                            if (item.icon)
                            {
                                iconRect = new Rect(cellRect.x + 1, cellRect.y + 1, cellRect.height - 2, cellRect.height - 2);
                                UnityEngine.GUI.DrawTexture(iconRect, item.icon, ScaleMode.ScaleToFit);
                            }

                            Rect infoRect = new Rect(cellRect.x + iconRect.width + 1, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                            EditorGUI.LabelField(infoRect, item.info);
                        }

                        break;
                    case 2: // 第三列:修改类型
                        if (item.changeType == VersionContrastTreeViewItem.ChangeType.Same || item.itemType == VersionContrastTreeViewItem.ItemType.Group)
                            break;

                        if (item.changeType == VersionContrastTreeViewItem.ChangeType.Modify)
                        {
                            if (item.itemType == VersionContrastTreeViewItem.ItemType.Bundle)
                                EditorGUI.LabelField(cellRect, new GUIContent("", item.hasDirectFileChanged ? "显性文件修改" : "关联文件修改"));
                            else
                                EditorGUI.LabelField(cellRect, new GUIContent("", "文件变动"));
                        }
                        else if (item.changeType == VersionContrastTreeViewItem.ChangeType.Add)
                            EditorGUI.LabelField(cellRect, new GUIContent("", "新增文件"));
                        else if (item.changeType == VersionContrastTreeViewItem.ChangeType.Remove)
                            EditorGUI.LabelField(cellRect, new GUIContent("", "已删除文件"));

                        cellRect.height -= 4;
                        UnityEngine.GUI.DrawTexture(cellRect, _changeTypeToGUITexture[item.changeType], ScaleMode.ScaleToFit);
                        break;
                    case 3: // 第四列:大小变化
                        if (item.itemType == VersionContrastTreeViewItem.ItemType.Asset)
                            break;

                        _changedSizeTextStyle ??= new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, richText = true };
                        if (item.changedSize > 0)
                            EditorGUI.LabelField(cellRect, $"<color=red>↑{Utility.FormatBytes(item.changedSize)}</color>", _changedSizeTextStyle);
                        else if (item.changedSize < 0)
                            EditorGUI.LabelField(cellRect, $"<color=lime>↓{Utility.FormatBytes(Math.Abs(item.changedSize))}</color>", _changedSizeTextStyle);
                        else
                            EditorGUI.LabelField(cellRect, "-", _changedSizeTextStyle);
                        break;
                    case 4: // 第五列:更新大小
                        if (item.size == 0)
                        {
                            if (item.itemType == VersionContrastTreeViewItem.ItemType.VersionFile)
                                EditorGUI.LabelField(cellRect, "-", _updateSizeTextStyle);
                            break;
                        }

                        _updateSizeTextStyle ??= new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
                        EditorGUI.LabelField(cellRect, Utility.FormatBytes(item.size), _updateSizeTextStyle);
                        break;
                }
            }
        }

        /// <summary>
        /// 关闭排序
        /// </summary>
        public void CloseSorting()
        {
            if (multiColumnHeader is { state: not null })
                multiColumnHeader.state.sortedColumns = null;
        }

        /// <summary>
        /// 显示列表排序
        /// </summary>
        static List<TreeViewItem> OrderTreeViewList<T>(List<TreeViewItem> treeView, bool ascending, Func<TreeViewItem, T> sortFunc)
        {
            return ascending ? treeView.OrderBy(sortFunc).ToList() : treeView.OrderByDescending(sortFunc).ToList();
        }

        /// <summary>
        /// 排序变动处理(仅排序前两层(组和Bundle), 资源层不进行排序)
        /// </summary>
        void OnSortingChanged(MultiColumnHeader header)
        {
            if (!_root.hasChildren)
                return;

            int curIndex = multiColumnHeader.sortedColumnIndex;
            if (curIndex == -1)
                return;

            bool ascending = multiColumnHeader.IsSortedAscending(curIndex);

            // 旧版本列, 使用名字排序
            if (curIndex == 0)
            {
                _root.children = OrderTreeViewList(_root.children, ascending, item => ((VersionContrastTreeViewItem)item).info);
                foreach (VersionContrastTreeViewItem groupItem in _root.children.Cast<VersionContrastTreeViewItem>())
                    if (groupItem.hasChildren)
                        groupItem.children = OrderTreeViewList(groupItem.children, ascending, item => ((VersionContrastTreeViewItem)item).info);
            }
            // 更新大小
            else if (curIndex == 4)
            {
                _root.children = OrderTreeViewList(_root.children, ascending, item => ((VersionContrastTreeViewItem)item).size);
                foreach (VersionContrastTreeViewItem groupItem in _root.children.Cast<VersionContrastTreeViewItem>())
                    if (groupItem.hasChildren)
                        groupItem.children = OrderTreeViewList(groupItem.children, ascending, item => ((VersionContrastTreeViewItem)item).size);
            }

            Reload();
        }

        /// <summary>
        /// 右键点击处理
        /// </summary>
        protected override void ContextClickedItem(int id)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("复制"), false, () =>
            {
                VersionContrastTreeViewItem item = FindItem(id, _root) as VersionContrastTreeViewItem;
                GUIUtility.systemCopyBuffer = item.info;
            });
            menu.ShowAsContext();
        }

        /// <summary>
        /// 双击Item处理
        /// </summary>
        protected override void DoubleClickedItem(int id)
        {
            VersionContrastTreeViewItem item = FindItem(id, _root) as VersionContrastTreeViewItem;
            if (item.itemType != VersionContrastTreeViewItem.ItemType.Asset)
                return;

            Object obj = AssetDatabase.LoadAssetAtPath<Object>(item.info);
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
        /// item选中改变处理
        /// </summary>
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            VersionContrastTreeViewItem[] clickedItems = Array.ConvertAll(selectedIds.ToArray(), id => FindItem(id, _root) as VersionContrastTreeViewItem);

            // 展开所有选中的item, 方便搜索时点击后查找
            foreach (VersionContrastTreeViewItem item in clickedItems)
            {
                TreeViewItem parent = item.parent;
                while (parent != null && parent.depth != -1)
                {
                    SetExpanded(parent.id, true);
                    parent = parent.parent;
                }
            }
        }
    }
}