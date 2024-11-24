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
    /// 分析窗口列表的Item
    /// </summary>
    public sealed class BuildAnalyzerTreeViewItem : TreeViewItem
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
            /// Bundle
            /// </summary>
            Bundle,

            /// <summary>
            /// 资源
            /// </summary>
            Asset,
        }

        /// <summary>
        /// 信息(组名/包名/资源目录)
        /// </summary>
        public readonly string info;

        /// <summary>
        /// 大小
        /// </summary>
        public long size;

        /// <summary>
        /// Item类型
        /// </summary>
        public readonly ItemType itemType;

        /// <summary>
        /// 不合理引用包数量
        /// </summary>
        public int illegalDepBundleCount;

        /// <summary>
        /// 资源没有被任何资源引用提示
        /// </summary>
        public bool isUnused;

        /// <summary>
        /// Prefab没有被任何资源引用提示
        /// </summary>
        public bool isPrefabUnused;

        /// <summary>
        /// 资源引用其他包提示
        /// </summary>
        public bool hasDependOtherBundle;

        /// <summary>
        /// 资源被其他包引用提示
        /// </summary>
        public bool hasOtherBundleReference;

        public BuildAnalyzerTreeViewItem(string info, int depth, ItemType type) : base(info.GetHashCode(), depth)
        {
            this.info = info;
            displayName = info; // 搜索用
            itemType = type;
            icon = AssetDatabase.GetCachedIcon(info) as Texture2D;
        }

        /// <summary>
        /// 额外状态提示
        /// </summary>
        public string ExtraStateTips
        {
            get
            {
                bool isShowUnused = isUnused || (isPrefabUnused && BuildAnalyzerWindow.IsShowPrefabUnused);
                if (!hasDependOtherBundle)
                {
                    if (isShowUnused && hasOtherBundleReference)
                        return "被,无";

                    if (isShowUnused)
                        return "无";

                    return hasOtherBundleReference ? "被" : string.Empty;
                }

                if (isShowUnused && hasOtherBundleReference)
                    return "主,被,无";

                if (isShowUnused)
                    return "主,无";

                return hasOtherBundleReference ? "主,被" : "主";
            }
        }
    }

    /// <summary>
    /// 分析窗口的组列表
    /// </summary>
    public class BuildAnalyzerGroupTreeView : TreeView
    {
        /// <summary>
        /// 所在的窗口
        /// </summary>
        readonly BuildAnalyzerWindow _parentWindow;

        /// <summary>
        /// 当前显示的清单配置
        /// </summary>
        ManifestConfig _manifestConfig;

        /// <summary>
        /// 当前显示的清单配置对应的清单
        /// </summary>
        Manifest _manifest;

        /// <summary>
        /// 根节点
        /// </summary>
        TreeViewItem _root;

        /// <summary>
        /// 显示组的总进度占比
        /// </summary>
        const float GroupShowProgress = 0.8f;

        /// <summary>
        /// 初始化进度
        /// </summary>
        public float InitProgress { get; private set; }

        /// <summary>
        /// 是否初始化完成
        /// </summary>
        public bool IsInitFinished { get; private set; }

        /// <summary>
        /// 当前运行的初始化协程
        /// </summary>
        EditorCoroutine _curInitCoroutine;

        /// <summary>
        /// 是否已开始分析资源依赖
        /// </summary>
        public bool isStartedAnalyzedAssetDep;

        /// <summary>
        /// 是否允许分析资源依赖
        /// </summary>
        public bool isAllowAnalyzedAssetDep = true;

        /// <summary>
        /// 资源分析依赖进度
        /// </summary>
        public float AssetDepAnalyzeProgress { get; private set; }

        /// <summary>
        /// 资源分析依赖是否完成
        /// </summary>
        public bool IsAssetDepAnalyzeFinished { get; private set; }

        /// <summary>
        /// 当前运行的资源分析依赖协程
        /// </summary>
        EditorCoroutine _curAssetDepAnalyzeCoroutine;

        /// <summary>
        /// 上次资源分析刷新时间
        /// </summary>
        double _lastUpdateTime;

        /// <summary>
        /// 需要删除无用资源的BundleItem
        /// </summary>
        BuildAnalyzerTreeViewItem _needDeleteUnusedAssetsBundleItem;

        /// <summary>
        /// 协程允许运行的最大时间, 超过则等到下一帧再运行
        /// </summary>
        const float MaxFrameTime = 0.02f;

        /// <summary>
        /// key:bundle名字, value:此bundle依赖的bundle列表
        /// </summary>
        readonly Dictionary<string, List<string>> _bundleNameToDependenceList = new();

        /// <summary>
        /// key:bundle名字, value:依赖此bundle的bundle列表
        /// </summary>
        readonly Dictionary<string, List<string>> _bundleNameToReferenceList = new();

        /// <summary>
        /// key:资源路径, value:此资源依赖的资源列表
        /// </summary>
        readonly Dictionary<string, List<string>> _assetPathToDependenceList = new();

        /// <summary>
        /// key:资源路径, value:依赖此资源的资源列表
        /// </summary>
        readonly Dictionary<string, List<string>> _assetPathToReferenceList = new();

        /// <summary>
        /// 警告文字Style
        /// </summary>
        GUIStyle warningLabelStyle = new(DefaultStyles.label);

        public BuildAnalyzerGroupTreeView(BuildAnalyzerWindow window) : base(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(GetColumns())))
        {
            showBorder = true;                    // 显示边框
            showAlternatingRowBackgrounds = true; // 每行背景颜色不一样, 更清晰
            _parentWindow = window;

            multiColumnHeader.sortingChanged += OnSortingChanged;
            warningLabelStyle.normal.textColor = Color.yellow;
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
                    headerContent = new GUIContent("名字"),
                    headerTextAlignment = TextAlignment.Center,
                },
                new MultiColumnHeaderState.Column
                {
                    width = 298,
                    minWidth = 298,
                    canSort = true,
                    autoResize = true,
                    headerContent = new GUIContent("大小"),
                    headerTextAlignment = TextAlignment.Center,
                }
            };
        }

        /// <summary>
        /// 设置当前显示的清单
        /// </summary>
        public void SetManifestConfig(ManifestConfig config, Manifest mf)
        {
            _manifestConfig = config;
            _manifest = mf;

            StopAllCoroutines();

            InitProgress = 0;
            IsInitFinished = false;
            _curInitCoroutine = _parentWindow.StartCoroutine(InitGroupAssetListAsync());
        }

        /// <summary>
        /// 停止所有协程
        /// </summary>
        public void StopAllCoroutines()
        {
            if (_curAssetDepAnalyzeCoroutine != null)
            {
                _parentWindow.StopCoroutine(_curAssetDepAnalyzeCoroutine);
                _curAssetDepAnalyzeCoroutine = null;
            }

            if (_curInitCoroutine != null)
            {
                _parentWindow.StopCoroutine(_curInitCoroutine);
                _curInitCoroutine = null;
            }
        }

        /// <summary>
        /// 异步初始化组资源列表
        /// </summary>
        IEnumerator InitGroupAssetListAsync()
        {
            _root = new TreeViewItem(-1, -1);

            // 已添加的Bundle
            Dictionary<int, bool> showedBundleIDMap = new Dictionary<int, bool>();

            // key:bundleID, value:引用此Bundle的组名
            Dictionary<int, List<string>> bundleIDToReferenceGroups = new Dictionary<int, List<string>>();

            // 组的Item
            Dictionary<string, BuildAnalyzerTreeViewItem> groupNameToGroupItem = new Dictionary<string, BuildAnalyzerTreeViewItem>();

            _assetPathToDependenceList.Clear();
            _assetPathToReferenceList.Clear();

            // 这里加起来0.8, 留0.2给Bundle依赖分析
            float collectAssetsProportion = 0.55f;        // 收集组资源目录占比
            float bundleFileCalulateProportion = 0.1f;    // Bundle文件分析占比
            float autoGroupFileCalulateProportion = 0.1f; // 自动分组分析占比
            float rawFileCalulateProportion = 0.05f;      // 原始文件分析占比

            List<Group> bundleGroupList = new List<Group>();
            List<Group> rawFileGroupList = new List<Group>();
            foreach (Group group in _manifestConfig.groups)
            {
                if (!group.IsNeedBuild)
                    continue;

                if (group.bundleMode != BundleMode.原始文件)
                    bundleGroupList.Add(group);
                else
                    rawFileGroupList.Add(group);
            }

            // 根据组数量改变占比
            if (bundleGroupList.Count > 0 && rawFileGroupList.Count == 0)
            {
                bundleFileCalulateProportion = 0.15f;
                rawFileCalulateProportion = 0;
            }
            else if (bundleGroupList.Count == 0 && rawFileGroupList.Count > 0)
            {
                bundleFileCalulateProportion = 0;
                autoGroupFileCalulateProportion = 0;
                rawFileCalulateProportion = 0.25f;
            }

            _lastUpdateTime = EditorApplication.timeSinceStartup;

            #region Bundle文件显示

            int bundleGroupCount = bundleGroupList.Count;

            // 获取去重的资源列表
            Dictionary<string, bool> assetPathRecord = new Dictionary<string, bool>();
            List<string[]> groupAssetPathList = new List<string[]>();
            for (int i = 0; i < bundleGroupCount; i++)
            {
                if (EditorApplication.timeSinceStartup - _lastUpdateTime > MaxFrameTime)
                {
                    _lastUpdateTime = EditorApplication.timeSinceStartup;
                    InitProgress = ((float)(i + 1) / bundleGroupCount) * collectAssetsProportion;
                    _parentWindow.Repaint();
                    yield return null;
                }

                Group group = bundleGroupList[i];
                string[] assetPathList = group.GetAssetPathList();
                List<string> effectiveAssetPathList = new List<string>();
                foreach (string assetPath in assetPathList)
                {
                    if (assetPathRecord.ContainsKey(assetPath))
                        continue;

                    effectiveAssetPathList.Add(assetPath);
                    assetPathRecord.Add(assetPath, false);
                }

                effectiveAssetPathList.Sort();
                groupAssetPathList.Add(effectiveAssetPathList.ToArray());
            }

            float startProgress = InitProgress = collectAssetsProportion;

            for (int i = 0; i < bundleGroupCount; i++)
            {
                Group group = bundleGroupList[i];
                string groupName = group.note;
                BuildAnalyzerTreeViewItem groupItem = new BuildAnalyzerTreeViewItem(groupName, 0, BuildAnalyzerTreeViewItem.ItemType.Group);
                _root.AddChild(groupItem);
                groupNameToGroupItem.Add(groupName, groupItem);

                string[] assetPathList = groupAssetPathList[i];
                int assetPathLength = assetPathList.Length;
                for (int j = 0; j < assetPathLength; j++)
                {
                    if (EditorApplication.timeSinceStartup - _lastUpdateTime > MaxFrameTime)
                    {
                        _lastUpdateTime = EditorApplication.timeSinceStartup;
                        InitProgress = startProgress + ((float)i / bundleGroupCount + 1f / bundleGroupCount * (j + 1) / assetPathLength) * bundleFileCalulateProportion;
                        _parentWindow.Repaint();
                        yield return null;
                    }

                    string assetPath = assetPathList[j];
                    ManifestBundleInfo manifestBundleInfo = _manifest.GetBundleInfo(assetPath);
                    if (manifestBundleInfo != null)
                    {
                        showedBundleIDMap[manifestBundleInfo.ID] = true;

                        if (manifestBundleInfo.DependentBundleIDList != null)
                        {
                            foreach (int depedentBundleID in manifestBundleInfo.DependentBundleIDList)
                            {
                                if (bundleIDToReferenceGroups.ContainsKey(depedentBundleID))
                                {
                                    List<string> groups = bundleIDToReferenceGroups[depedentBundleID];
                                    if (!groups.Contains(groupName))
                                        groups.Add(groupName);
                                }
                                else
                                    bundleIDToReferenceGroups.Add(depedentBundleID, new List<string>() { groupName });
                            }
                        }

                        BuildAnalyzerTreeViewItem bundleItem = null;
                        string bundleName = manifestBundleInfo.Name;
                        if (groupItem.hasChildren)
                        {
                            foreach (BuildAnalyzerTreeViewItem item in groupItem.children.Cast<BuildAnalyzerTreeViewItem>())
                            {
                                if (item.info == bundleName)
                                {
                                    bundleItem = item;
                                    break;
                                }
                            }
                        }

                        if (bundleItem == null)
                        {
                            bundleItem = new BuildAnalyzerTreeViewItem(bundleName, 1, BuildAnalyzerTreeViewItem.ItemType.Bundle) { size = manifestBundleInfo.Size };
                            bundleItem.displayName += $"({manifestBundleInfo.NameWithHash})"; // 方便通过文件名搜索
                            groupItem.AddChild(bundleItem);
                            groupItem.size += manifestBundleInfo.Size;
                        }

                        FileInfo file = new FileInfo(assetPath);
                        bundleItem.AddChild(new BuildAnalyzerTreeViewItem(assetPath, 2, BuildAnalyzerTreeViewItem.ItemType.Asset) { size = file.Exists ? file.Length : 0 });
                    }
                }
            }

            #endregion

            #region 自动分组Bundle显示

            startProgress = InitProgress = startProgress + bundleFileCalulateProportion;

            // 被多个分组引用的Bundle组
            BuildAnalyzerTreeViewItem autoGroupByMultiGroupReferenceItem = null;

            // 组内引用的Bundle组Item记录
            Dictionary<string, BuildAnalyzerTreeViewItem> groupNameToAutoGroupItem = new Dictionary<string, BuildAnalyzerTreeViewItem>();

            int bundleInfoListCount = _manifest.manifestBundleInfoList.Count;
            for (int i = 0; i < bundleInfoListCount; i++)
            {
                ManifestBundleInfo bundleInfo = _manifest.manifestBundleInfoList[i];
                if (bundleInfo.IsRawFile || showedBundleIDMap.ContainsKey(bundleInfo.ID))
                    continue;

                if (bundleIDToReferenceGroups.TryGetValue(bundleInfo.ID, out List<string> groups) && groups.Count == 1)
                {
                    string groupName = groups[0];
                    BuildAnalyzerTreeViewItem groupItem = groupNameToGroupItem[groupName];
                    if (!groupNameToAutoGroupItem.TryGetValue(groupName, out BuildAnalyzerTreeViewItem autoGroupItem))
                    {
                        autoGroupItem = new BuildAnalyzerTreeViewItem($"自动分组(仅{groupName}组引用)", 1, BuildAnalyzerTreeViewItem.ItemType.Group);
                        groupNameToAutoGroupItem.Add(groupName, autoGroupItem);
                        groupItem.AddChild(autoGroupItem);
                    }

                    BuildAnalyzerTreeViewItem bundleItem = new BuildAnalyzerTreeViewItem(bundleInfo.Name, 2, BuildAnalyzerTreeViewItem.ItemType.Bundle) { size = bundleInfo.Size };
                    bundleItem.displayName += $"({bundleInfo.NameWithHash})"; // 方便通过文件名搜索
                    autoGroupItem.AddChild(bundleItem);
                    autoGroupItem.size += bundleInfo.Size;
                    groupItem.size += bundleInfo.Size;

                    foreach (string assetPath in bundleInfo.AssetPathList)
                    {
                        FileInfo file = new FileInfo(assetPath);
                        long fileSize = file.Exists ? file.Length : 0;
                        bundleItem.AddChild(new BuildAnalyzerTreeViewItem(assetPath, 3, BuildAnalyzerTreeViewItem.ItemType.Asset) { size = fileSize });
                    }
                }
                else
                {
                    if (autoGroupByMultiGroupReferenceItem == null)
                    {
                        autoGroupByMultiGroupReferenceItem = new BuildAnalyzerTreeViewItem("自动分组(多组引用)", 0, BuildAnalyzerTreeViewItem.ItemType.Group);
                        _root.AddChild(autoGroupByMultiGroupReferenceItem);
                    }

                    BuildAnalyzerTreeViewItem bundleItem = new BuildAnalyzerTreeViewItem(bundleInfo.Name, 1, BuildAnalyzerTreeViewItem.ItemType.Bundle) { size = bundleInfo.Size };
                    bundleItem.displayName += $"({bundleInfo.NameWithHash})"; // 方便通过文件名搜索
                    autoGroupByMultiGroupReferenceItem.AddChild(bundleItem);
                    autoGroupByMultiGroupReferenceItem.size += bundleInfo.Size;

                    foreach (string assetPath in bundleInfo.AssetPathList)
                    {
                        FileInfo file = new FileInfo(assetPath);
                        long fileSize = file.Exists ? file.Length : 0;
                        bundleItem.AddChild(new BuildAnalyzerTreeViewItem(assetPath, 2, BuildAnalyzerTreeViewItem.ItemType.Asset) { size = fileSize });
                    }
                }

                if (EditorApplication.timeSinceStartup - _lastUpdateTime > MaxFrameTime)
                {
                    _lastUpdateTime = EditorApplication.timeSinceStartup;
                    InitProgress = startProgress + (float)(i + 1) / bundleInfoListCount * autoGroupFileCalulateProportion;
                    _parentWindow.Repaint();
                    yield return null;
                }
            }

            #endregion

            #region 原始文件显示

            startProgress = InitProgress = startProgress + autoGroupFileCalulateProportion;

            int rawFileGroupCount = rawFileGroupList.Count;
            for (int i = 0; i < rawFileGroupCount; i++)
            {
                Group group = rawFileGroupList[i];
                BuildAnalyzerTreeViewItem groupItem = new BuildAnalyzerTreeViewItem(group.note, 0, BuildAnalyzerTreeViewItem.ItemType.Group);
                _root.AddChild(groupItem);

                string assetPath;
                foreach (string path in group.GetAssetPathList())
                {
                    assetPath = path.Replace('\\', '/');
                    string manifestRecordPath = group.isExternalPath ? BuildUtils.GetExternalRawFileLoadPath(assetPath, group.externalPath, group.placeFolderName) : assetPath;
                    if (manifestRecordPath.StartsWith("/")) // placeFolderName为空字符时, 首字符会为'/'
                        manifestRecordPath = manifestRecordPath.Substring(1);

                    ManifestBundleInfo manifestBundleInfo = _manifest.GetBundleInfo(manifestRecordPath);
                    if (manifestBundleInfo != null)
                    {
                        FileInfo file = new FileInfo(assetPath);
                        long fileSize = file.Exists ? file.Length : 0;
                        BuildAnalyzerTreeViewItem assetItem = new BuildAnalyzerTreeViewItem(assetPath, 1, BuildAnalyzerTreeViewItem.ItemType.Asset) { size = fileSize };
                        assetItem.displayName += $"({manifestBundleInfo.NameWithHash})"; // 方便通过文件名搜索
                        groupItem.AddChild(assetItem);
                        groupItem.size += fileSize;
                    }
                }

                if (EditorApplication.timeSinceStartup - _lastUpdateTime > MaxFrameTime)
                {
                    _lastUpdateTime = EditorApplication.timeSinceStartup;
                    InitProgress = startProgress + (float)(i + 1) / rawFileGroupCount * rawFileCalulateProportion;
                    _parentWindow.Repaint();
                    yield return null;
                }
            }

            #endregion

            InitProgress = GroupShowProgress;
            _parentWindow.Repaint();
            yield return null;

            // 分析Bundle依赖
            yield return AnalyzeAllBundleDepedencies();

            InitProgress = 1;
            _parentWindow.Repaint();
            yield return null;

            Reload();
            IsInitFinished = true;
            _parentWindow.Repaint();
            _curInitCoroutine = null;
        }

        /// <summary>
        /// 分析所有Bundle依赖
        /// </summary>
        IEnumerator AnalyzeAllBundleDepedencies()
        {
            foreach (ManifestBundleInfo bundleInfo in _manifest.manifestBundleInfoList)
            {
                if (!_bundleNameToDependenceList.TryGetValue(bundleInfo.Name, out List<string> depBundleList))
                {
                    depBundleList = new List<string>();
                    _bundleNameToDependenceList.Add(bundleInfo.Name, depBundleList);
                }

                foreach (ManifestBundleInfo depBundleInfo in _manifest.GetDependentBundleInfoList(bundleInfo))
                {
                    string depBundleName = depBundleInfo.Name;
                    depBundleList.Add(depBundleName);
                    if (!_bundleNameToReferenceList.TryGetValue(depBundleName, out List<string> refBundleList))
                    {
                        refBundleList = new List<string>();
                        _bundleNameToReferenceList.Add(depBundleName, refBundleList);
                    }

                    refBundleList.Add(bundleInfo.Name);
                }
            }

            yield return RefreshItemIllegalDepBundleNum();
        }

        /// <summary>
        /// 开始分析全部资源依赖
        /// </summary>
        public void StartAnalyzeAllAssetDepedenciesAsync()
        {
            isStartedAnalyzedAssetDep = true;
            _curAssetDepAnalyzeCoroutine = _parentWindow.StartCoroutine(AnalyzeAllAssetDepedenciesAsync());
        }

        /// <summary>
        /// 开始异步分析全部资源依赖
        /// </summary>
        IEnumerator AnalyzeAllAssetDepedenciesAsync()
        {
            RefreshSelectionInfo(); // 为了刷新文字:(未分析→分析中)
            _lastUpdateTime = EditorApplication.timeSinceStartup;

            int bundleInfoListCount = _manifest.manifestBundleInfoList.Count;
            for (int i = 0; i < bundleInfoListCount; i++)
            {
                AssetDepAnalyzeProgress = (float)(i + 1) / bundleInfoListCount;
                _parentWindow.Repaint();
                yield return null;

                ManifestBundleInfo bundleInfo = _manifest.manifestBundleInfoList[i];
                foreach (string assetPath in bundleInfo.AssetPathList)
                {
                    if (!_assetPathToDependenceList.TryGetValue(assetPath, out List<string> depAssetPathList))
                    {
                        depAssetPathList = new List<string>();
                        _assetPathToDependenceList.Add(assetPath, depAssetPathList);
                    }

                    foreach (string depPath in BuildUtils.GetDependencies(assetPath))
                    {
                        // 等待到允许继续分析为止
                        while (!isAllowAnalyzedAssetDep)
                        {
                            yield return null;
                        }

                        depAssetPathList.Add(depPath);
                        if (!_assetPathToReferenceList.TryGetValue(depPath, out List<string> refAssetPathList))
                        {
                            refAssetPathList = new List<string>();
                            _assetPathToReferenceList.Add(depPath, refAssetPathList);
                        }

                        if (!refAssetPathList.Contains(assetPath))
                            refAssetPathList.Add(assetPath);

                        // 耗时太长时, 下帧继续处理
                        if (EditorApplication.timeSinceStartup - _lastUpdateTime > MaxFrameTime)
                        {
                            _lastUpdateTime = EditorApplication.timeSinceStartup;
                            yield return null;
                        }
                    }
                }
            }

            AssetDepAnalyzeProgress = 1;
            _parentWindow.Repaint();
            yield return null;

            IsAssetDepAnalyzeFinished = true;
            RefreshExtraState();
            RefreshSelectionInfo();
            yield return null;

            _parentWindow.Repaint();
            _curAssetDepAnalyzeCoroutine = null;
        }

        protected override TreeViewItem BuildRoot()
        {
            return _root;
        }

        /// <summary>
        /// 判断指定的Item是否合理引用指定的包名
        /// </summary>
        bool IsLegalDependent(BuildAnalyzerTreeViewItem bundleItem, string depBundleName)
        {
            LegalGroupDependenceConfig config = LegalGroupDependenceConfig.Instance;
            if (!config)
                return false;

            string bundleName = bundleItem.info;
            if (!_bundleNameToDependenceList.TryGetValue(bundleName, out List<string> dependentBundleList))
                return false;

            string groupName = GetGroupName(bundleItem);
            if (config.IsLegalDependence(groupName, bundleName, depBundleName))
                return true;

            // 若不是本包的合理引用, 则在所有引用包里获取是否其他包的合理引用, 若是引用包的合理引用, 则不计入计数中
            foreach (string depBundleName2 in dependentBundleList)
            {
                if (depBundleName2.Equals(depBundleName))
                    continue;

                TreeViewItem depBundleItem = FindItem(depBundleName2.GetHashCode(), _root);
                if (depBundleItem == null)
                    continue;

                string depGroupName = GetGroupName(depBundleItem as BuildAnalyzerTreeViewItem);
                if (config.IsLegalDependence(depGroupName, depBundleName2, depBundleName))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获取Bundle不合理引用的bundle数量
        /// </summary>
        int GetIllegalDepBundleCount(BuildAnalyzerTreeViewItem bundleItem)
        {
            LegalGroupDependenceConfig config = LegalGroupDependenceConfig.Instance;
            if (!config)
                return 0;

            if (!_bundleNameToDependenceList.TryGetValue(bundleItem.info, out List<string> dependentBundleList))
                return 0;

            // 记录直接配置合法的bundle名字, 提高初始化性能
            Dictionary<string, bool> legalBundleNameMap = new Dictionary<string, bool>();

            // 记录本组的合法依赖
            string groupName = GetGroupName(bundleItem);
            GroupLegalDependence legal = config.GetGroupLegalDependence(groupName);
            if (legal != null)
                foreach (string legalBundleName in legal.legalBundleList)
                    legalBundleNameMap[legalBundleName] = true;

            // 记录依赖包的合法依赖
            foreach (string depBundleName in dependentBundleList)
            {
                TreeViewItem depBundleItem = FindItem(depBundleName.GetHashCode(), _root);
                if (depBundleItem == null)
                    continue;

                groupName = GetGroupName(depBundleItem as BuildAnalyzerTreeViewItem);
                legal = config.GetGroupLegalDependence(groupName);
                if (legal != null)
                    foreach (string legalBundleName in legal.legalBundleList)
                        legalBundleNameMap[legalBundleName] = true;
            }

            int count = 0;
            foreach (string depBundleName in dependentBundleList)
                if (!legalBundleNameMap.ContainsKey(depBundleName) && !IsLegalDependent(bundleItem, depBundleName))
                    count++;
            return count;
        }

        /// <summary>
        /// 刷新全部Item不合理引用Bundle数量
        /// </summary>
        IEnumerator RefreshItemIllegalDepBundleNum()
        {
            if (!_root.hasChildren)
                yield break;

            List<TreeViewItem> children = _root.children;
            int childCount = children.Count;
            BuildAnalyzerTreeViewItem groupItem;
            List<TreeViewItem> groupChildren;
            int groupChildCount;
            BuildAnalyzerTreeViewItem secondItem;
            for (int i = 0; i < childCount; i++)
            {
                groupItem = children[i] as BuildAnalyzerTreeViewItem;
                groupItem.illegalDepBundleCount = 0;
                if (!groupItem.hasChildren)
                    continue;

                groupChildren = groupItem.children;
                groupChildCount = groupChildren.Count;
                float groupProgress = (float)i / childCount;
                for (int j = 0; j < groupChildCount; j++)
                {
                    InitProgress = GroupShowProgress + (groupProgress + (float)j / groupChildCount / childCount) * 0.2f;
                    _parentWindow.Repaint();

                    // 等待到允许继续分析为止
                    while (!isAllowAnalyzedAssetDep)
                    {
                        yield return null;
                    }

                    // 耗时太长时, 下帧继续处理
                    if (EditorApplication.timeSinceStartup - _lastUpdateTime > MaxFrameTime)
                    {
                        _lastUpdateTime = EditorApplication.timeSinceStartup;
                        yield return null;
                    }

                    secondItem = groupChildren[j] as BuildAnalyzerTreeViewItem;
                    if (secondItem.itemType == BuildAnalyzerTreeViewItem.ItemType.Bundle)
                    {
                        secondItem.illegalDepBundleCount = GetIllegalDepBundleCount(secondItem);

                        // 组item使用最大的数量
                        if (secondItem.illegalDepBundleCount > groupItem.illegalDepBundleCount)
                            groupItem.illegalDepBundleCount = secondItem.illegalDepBundleCount;
                    }
                    else if (secondItem.itemType == BuildAnalyzerTreeViewItem.ItemType.Group) // 组里也会有组(自动分组)
                    {
                        secondItem.illegalDepBundleCount = 0;
                        if (!secondItem.hasChildren)
                            continue;

                        foreach (BuildAnalyzerTreeViewItem thirdItem in secondItem.children.Cast<BuildAnalyzerTreeViewItem>())
                        {
                            if (thirdItem.itemType == BuildAnalyzerTreeViewItem.ItemType.Bundle)
                            {
                                int count = GetIllegalDepBundleCount(thirdItem);
                                thirdItem.illegalDepBundleCount = count;
                                if (count > secondItem.illegalDepBundleCount)
                                    secondItem.illegalDepBundleCount = count;
                                if (count > groupItem.illegalDepBundleCount)
                                    groupItem.illegalDepBundleCount = count;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刷新额外状态信息
        /// </summary>
        void RefreshExtraState()
        {
            if (!_root.hasChildren)
                return;

            foreach (BuildAnalyzerTreeViewItem groupItem in _root.children.Cast<BuildAnalyzerTreeViewItem>())
            {
                if (!groupItem.hasChildren)
                    continue;

                foreach (BuildAnalyzerTreeViewItem secondItem in groupItem.children.Cast<BuildAnalyzerTreeViewItem>())
                {
                    if (secondItem.itemType == BuildAnalyzerTreeViewItem.ItemType.Bundle)
                        RefreshBundleItemExtraState(secondItem);
                    else if (secondItem.itemType == BuildAnalyzerTreeViewItem.ItemType.Group) // 组里也会有组(自动分组)
                        if (secondItem.hasChildren)
                            foreach (BuildAnalyzerTreeViewItem thirdItem in secondItem.children.Cast<BuildAnalyzerTreeViewItem>())
                                if (thirdItem.itemType == BuildAnalyzerTreeViewItem.ItemType.Bundle)
                                    RefreshBundleItemExtraState(thirdItem);
                }
            }
        }

        /// <summary>
        /// 刷新指定BundleItem的额外状态信息
        /// </summary>
        void RefreshBundleItemExtraState(BuildAnalyzerTreeViewItem bundleItem)
        {
            bundleItem.isUnused = false;
            bundleItem.isPrefabUnused = false;
            bundleItem.hasDependOtherBundle = false;
            bundleItem.hasOtherBundleReference = false;
            if (!bundleItem.hasChildren)
                return;

            ManifestBundleInfo bundleInfoOfItem;

            foreach (BuildAnalyzerTreeViewItem assetItem in bundleItem.children.Cast<BuildAnalyzerTreeViewItem>())
            {
                bundleInfoOfItem = _manifest.GetBundleInfo(assetItem.info);

                // 依赖其他包提示
                if (_assetPathToDependenceList.TryGetValue(assetItem.info, out List<string> dependenceAssetList))
                {
                    foreach (string depAssetPath in dependenceAssetList)
                    {
                        ManifestBundleInfo depBundleInfo = _manifest.GetBundleInfo(depAssetPath);
                        if (depBundleInfo != null && depBundleInfo.ID != bundleInfoOfItem.ID && !IsLegalDependent(bundleItem, depBundleInfo.Name))
                        {
                            assetItem.hasDependOtherBundle = true;
                            bundleItem.hasDependOtherBundle = true;
                            break;
                        }
                    }
                }

                // 无任何资源引用时进行标记
                if (!_assetPathToReferenceList.TryGetValue(assetItem.info, out List<string> referenceAssetList))
                {
                    // 场景忽略不计
                    if (!assetItem.info.EndsWith(".unity"))
                    {
                        // Prefab使用单独字段
                        if (!assetItem.info.EndsWith(".prefab"))
                        {
                            assetItem.isUnused = true;
                            bundleItem.isUnused = true;
                        }
                        else
                        {
                            assetItem.isPrefabUnused = true;
                            bundleItem.isPrefabUnused = true;
                        }
                    }

                    continue;
                }

                // 被其他包引用提示
                foreach (string refAssetPath in referenceAssetList)
                {
                    ManifestBundleInfo bundleInfo = _manifest.GetBundleInfo(refAssetPath);
                    if (bundleInfo != null && bundleInfo.ID != bundleInfoOfItem.ID)
                    {
                        assetItem.hasOtherBundleReference = true;
                        bundleItem.hasOtherBundleReference = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 获取Item所在组名字
        /// </summary>
        string GetGroupName(BuildAnalyzerTreeViewItem item)
        {
            if (item.depth == 0)
                return item.info;

            return GetGroupName(item.parent as BuildAnalyzerTreeViewItem);
        }

        /// <summary>
        /// 选中指定的item
        /// </summary>
        public bool PingItem(int id)
        {
            if (FindItem(id, _root) == null)
                return false;

            SetFocus();
            FrameItem(id);
            SetSelection(new List<int>() { id });
            RefreshSelectionInfo();
            return true;
        }

        /// <summary>
        /// 行UI显示
        /// </summary>
        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                BuildAnalyzerTreeViewItem item = (BuildAnalyzerTreeViewItem)args.item;

                Rect cellRect = args.GetCellRect(i);
                switch (args.GetColumn(i))
                {
                    case 0: // 第一列:名字
                        cellRect.xMin += GetContentIndent(item);
                        Rect iconRect = Rect.zero;
                        if (item.icon)
                        {
                            iconRect = new Rect(cellRect.x + 1, cellRect.y + 1, cellRect.height - 2, cellRect.height - 2);
                            UnityEngine.GUI.DrawTexture(iconRect, item.icon, ScaleMode.ScaleToFit);
                        }

                        Rect itemInfoRect = new Rect(cellRect.x + iconRect.width + 1, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                        if (item.illegalDepBundleCount < _parentWindow.BundleDependenceWarningNum)
                            EditorGUI.LabelField(itemInfoRect, item.info, DefaultStyles.label);
                        else
                            EditorGUI.LabelField(itemInfoRect, item.info, warningLabelStyle);

                        if (item.itemType == BuildAnalyzerTreeViewItem.ItemType.Group)
                        {
                            if (item.hasChildren)
                            {
                                Rect countRect = args.GetCellRect(1);
                                countRect = new Rect(countRect.x - 100, countRect.y, countRect.width, countRect.height);
                                DefaultGUI.Label(countRect, $"个数:{item.children.Count}", args.selected, args.focused);
                            }
                        }
                        else if (!string.IsNullOrEmpty(item.ExtraStateTips))
                        {
                            if (item.itemType == BuildAnalyzerTreeViewItem.ItemType.Bundle)
                            {
                                Rect countRect = args.GetCellRect(1);
                                countRect = new Rect(countRect.x - 50, countRect.y, countRect.width, countRect.height);
                                DefaultGUI.Label(countRect, item.ExtraStateTips, args.selected, args.focused);

                                // 有无引用资源时, 添加删除按钮
                                if (item.ExtraStateTips.Contains("无"))
                                {
                                    Rect btnRect = new Rect(countRect.x - 50, countRect.y, 50, countRect.height);
                                    if (UnityEngine.GUI.Button(btnRect, "删无"))
                                        _needDeleteUnusedAssetsBundleItem = item;
                                }
                            }
                            else if (item.itemType == BuildAnalyzerTreeViewItem.ItemType.Asset)
                            {
                                Rect countRect = args.GetCellRect(1);
                                countRect = new Rect(countRect.x - 50, countRect.y, countRect.width, countRect.height);
                                DefaultGUI.Label(countRect, item.ExtraStateTips, args.selected, args.focused);
                            }
                        }

                        break;
                    case 1: // 第二列:大小
                        DefaultGUI.Label(cellRect, Utility.FormatBytes(item.size), args.selected, args.focused);
                        break;
                }
            }

            // 删除指定包下面的无用资源
            if (_needDeleteUnusedAssetsBundleItem != null)
            {
                var childrenList = _needDeleteUnusedAssetsBundleItem.children;
                int childCount = childrenList.Count;
                for (int j = childCount - 1; j >= 0; j--)
                {
                    BuildAnalyzerTreeViewItem child = childrenList[j] as BuildAnalyzerTreeViewItem;
                    if (!child.isUnused && (!child.isPrefabUnused || !BuildAnalyzerWindow.IsShowPrefabUnused))
                        continue;

                    AssetDatabase.DeleteAsset(child.info);
                    childrenList.RemoveAt(j);
                    EditorUtility.DisplayProgressBar($"删除无用资源中", Path.GetFileName(child.info), (float)(childCount - j) / childCount);
                }

                _needDeleteUnusedAssetsBundleItem.isUnused = false;
                if (BuildAnalyzerWindow.IsShowPrefabUnused)
                    _needDeleteUnusedAssetsBundleItem.isPrefabUnused = false;
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                Reload();
                _parentWindow.Repaint();
                _needDeleteUnusedAssetsBundleItem = null;
            }
        }

        /// <summary>
        /// 刷新选中的item应显示的信息
        /// </summary>
        void RefreshSelectionInfo()
        {
            BuildAnalyzerTreeViewItem[] clickedItems = Array.ConvertAll(GetSelection().ToArray(), id => FindItem(id, _root) as BuildAnalyzerTreeViewItem);

            DependenceTreeInfo dependentBundleTreeInfo = null; // 我依赖的Bundle信息
            DependenceTreeInfo referenceBundleTreeInfo = null; // 依赖我的Bundle信息
            DependenceTreeInfo dependentAssetTreeInfo = null;  // 我依赖的资源信息
            DependenceTreeInfo referenceAssetTreeInfo = null;  // 依赖我的资源信息

            int bundleDepBundleCount = 0; // Bundle类型item_我依赖的Bundle数量
            long bundleDepBundleSize = 0; // Bundle类型item_我依赖的Bundle大小
            int bundleRefBundleCount = 0; // Bundle类型item_依赖我的Bundle数量

            int assetDepBundleCount = 0; // Asset类型item_我依赖的Bundle数量
            long assetDepBundleSize = 0; // Asset类型item_我依赖的Bundle大小
            int assetRefBundleCount = 0; // Asset类型item_依赖我的Bundle数量
            int assetDepAssetCount = 0;  // Asset类型item_我依赖的Asset数量
            int assetRefAssetCount = 0;  // Asset类型item_依赖我的Asset数量

            string analyzingText = isStartedAnalyzedAssetDep ? "分析中..." : "未分析";

            foreach (BuildAnalyzerTreeViewItem item in clickedItems)
            {
                if (item.itemType == BuildAnalyzerTreeViewItem.ItemType.Bundle)
                {
                    #region 我依赖的Bundle

                    dependentBundleTreeInfo ??= new DependenceTreeInfo() { title = "我依赖的Bundle", dataList = new List<string>() };

                    List<string> dependentBundleDataList = dependentBundleTreeInfo.dataList;
                    if (_bundleNameToDependenceList.TryGetValue(item.info, out List<string> dependentBundleList))
                    {
                        foreach (string bundleName in dependentBundleList)
                        {
                            if (dependentBundleDataList.Contains(bundleName) || dependentBundleDataList.Contains("*" + bundleName))
                                continue;

                            ManifestBundleInfo depBundleInfo = _manifest.GetBundleInfoByBundleName(bundleName);
                            if (depBundleInfo == null)
                                continue;

                            bundleDepBundleCount++;
                            bundleDepBundleSize += depBundleInfo.Size;
                            if (IsLegalDependent(item, bundleName))
                                dependentBundleDataList.Add(bundleName);
                            else
                                dependentBundleDataList.Add("*" + bundleName);
                        }
                    }

                    if (bundleDepBundleCount > 0)
                        dependentBundleTreeInfo.title = $"我依赖的Bundle({bundleDepBundleCount}个, 共{Utility.FormatBytes(bundleDepBundleSize)})";

                    #endregion

                    #region 依赖我的Bundle

                    referenceBundleTreeInfo ??= new DependenceTreeInfo() { title = "依赖我的Bundle", dataList = new List<string>() };

                    List<string> referenceBundleDataList = referenceBundleTreeInfo.dataList;
                    if (_bundleNameToReferenceList.TryGetValue(item.info, out List<string> referenceBundleList))
                    {
                        foreach (string bundleName in referenceBundleList)
                        {
                            if (!referenceBundleDataList.Contains(bundleName))
                            {
                                bundleRefBundleCount++;
                                referenceBundleDataList.Add(bundleName);
                            }
                        }

                        referenceBundleDataList.Sort();
                    }

                    if (bundleRefBundleCount > 0)
                        referenceBundleTreeInfo.title = $"依赖我的Bundle({bundleRefBundleCount}个)";

                    #endregion
                }
                else if (item.itemType == BuildAnalyzerTreeViewItem.ItemType.Asset)
                {
                    #region 我依赖的Bundle

                    dependentBundleTreeInfo ??= new DependenceTreeInfo() { title = "我依赖的Bundle", dataList = new List<string>() };

                    List<string> dependentBundleDataList = dependentBundleTreeInfo.dataList;
                    if (IsAssetDepAnalyzeFinished)
                    {
                        if (_assetPathToDependenceList.TryGetValue(item.info, out List<string> dependenceAssetList))
                        {
                            ManifestBundleInfo bundleInfoOfItem = _manifest.GetBundleInfo(item.info);
                            // 资源的父节点就是bundle节点, 获取依赖的Bundle是否合法
                            BuildAnalyzerTreeViewItem bundleItem = item.parent as BuildAnalyzerTreeViewItem;
                            foreach (string depAssetPath in dependenceAssetList)
                            {
                                ManifestBundleInfo bundleInfo = _manifest.GetBundleInfo(depAssetPath);
                                if (bundleInfo != null && bundleInfo.ID != bundleInfoOfItem.ID && !dependentBundleDataList.Contains(bundleInfo.Name) && !dependentBundleDataList.Contains("*" + bundleInfo.Name))
                                {
                                    assetDepBundleCount++;
                                    assetDepBundleSize += bundleInfo.Size;
                                    if (IsLegalDependent(bundleItem, bundleInfo.Name))
                                        dependentBundleDataList.Add(bundleInfo.Name);
                                    else
                                        dependentBundleDataList.Add("*" + bundleInfo.Name);
                                }
                            }
                        }
                    }
                    else
                    {
                        ManifestBundleInfo bundleInfoOfItem = _manifest.GetBundleInfo(item.info);
                        foreach (string depPath in BuildUtils.GetDependencies(item.info))
                        {
                            ManifestBundleInfo bundleInfo = _manifest.GetBundleInfo(depPath);
                            // 资源的父节点就是bundle节点, 获取依赖的Bundle是否合法
                            BuildAnalyzerTreeViewItem bundleItem = item.parent as BuildAnalyzerTreeViewItem;
                            if (bundleInfo != null && bundleInfo.ID != bundleInfoOfItem.ID && !dependentBundleDataList.Contains(bundleInfo.Name) && !dependentBundleDataList.Contains("*" + bundleInfo.Name))
                            {
                                assetDepBundleCount++;
                                assetDepBundleSize += bundleInfo.Size;
                                if (IsLegalDependent(bundleItem, bundleInfo.Name))
                                    dependentBundleDataList.Add(bundleInfo.Name);
                                else
                                    dependentBundleDataList.Add("*" + bundleInfo.Name);
                            }
                        }
                    }

                    if (assetDepBundleCount > 0)
                        dependentBundleTreeInfo.title = $"我依赖的Bundle({assetDepBundleCount}个, 共{Utility.FormatBytes(assetDepBundleSize)})";

                    #endregion

                    #region 依赖我的Bundle

                    referenceBundleTreeInfo ??= new DependenceTreeInfo() { title = "依赖我的Bundle", dataList = new List<string>() };

                    List<string> referenceBundleDataList = referenceBundleTreeInfo.dataList;
                    if (IsAssetDepAnalyzeFinished)
                    {
                        if (_assetPathToReferenceList.TryGetValue(item.info, out List<string> referenceAssetList))
                        {
                            ManifestBundleInfo bundleInfoOfItem = _manifest.GetBundleInfo(item.info);
                            foreach (string refAssetPath in referenceAssetList)
                            {
                                ManifestBundleInfo bundleInfo = _manifest.GetBundleInfo(refAssetPath);
                                if (bundleInfo != null && bundleInfo.ID != bundleInfoOfItem.ID && !referenceBundleDataList.Contains(bundleInfo.Name))
                                {
                                    assetRefBundleCount++;
                                    referenceBundleDataList.Add(bundleInfo.Name);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!referenceBundleDataList.Contains(analyzingText))
                            referenceBundleDataList.Add(analyzingText);
                    }

                    referenceBundleDataList.Sort();

                    if (assetRefBundleCount > 0)
                        referenceBundleTreeInfo.title = $"依赖我的Bundle({assetRefBundleCount}个)";

                    #endregion

                    #region 我依赖的资源

                    dependentAssetTreeInfo ??= new DependenceTreeInfo() { title = "我依赖的资源", dataList = new List<string>() };

                    string[] dependentAssetList = null;

                    if (IsAssetDepAnalyzeFinished)
                    {
                        if (_assetPathToDependenceList.TryGetValue(item.info, out List<string> depAssetList))
                            dependentAssetList = depAssetList.ToArray();
                    }
                    else
                        dependentAssetList = BuildUtils.GetDependencies(item.info);

                    if (dependentAssetList != null)
                    {
                        List<string> dependentAssetDataList = dependentAssetTreeInfo.dataList;

                        BuildAnalyzerTreeViewItem bundleItem = item.parent as BuildAnalyzerTreeViewItem;
                        ManifestBundleInfo bundleInfoOfItem = _manifest.GetBundleInfo(item.info);
                        foreach (string assetPath in dependentAssetList)
                        {
                            if (dependentAssetDataList.Contains(assetPath) || dependentAssetDataList.Contains("*" + assetPath))
                                continue;

                            assetDepAssetCount++;
                            ManifestBundleInfo depBundleInfo = _manifest.GetBundleInfo(assetPath);
                            if (depBundleInfo != null && depBundleInfo.ID != bundleInfoOfItem.ID && !IsLegalDependent(bundleItem, depBundleInfo.Name))
                                dependentAssetDataList.Add("*" + assetPath); // 依赖其他包的资源标记*号方便查找
                            else
                                dependentAssetDataList.Add(assetPath);
                        }

                        dependentAssetDataList.Sort();
                    }

                    if (assetDepAssetCount > 0)
                        dependentAssetTreeInfo.title = $"我依赖的资源({assetDepAssetCount}个)";

                    #endregion

                    #region 依赖我的资源

                    referenceAssetTreeInfo ??= new DependenceTreeInfo() { title = "依赖我的资源", dataList = new List<string>() };

                    List<string> referenceAssetDataList = referenceAssetTreeInfo.dataList;

                    if (IsAssetDepAnalyzeFinished)
                    {
                        if (_assetPathToReferenceList.TryGetValue(item.info, out List<string> referenceAssetList))
                        {
                            ManifestBundleInfo bundleInfoOfItem = _manifest.GetBundleInfo(item.info);
                            foreach (string assetPath in referenceAssetList)
                            {
                                if (referenceAssetDataList.Contains(assetPath) || referenceAssetDataList.Contains("*" + assetPath))
                                    continue;

                                assetRefAssetCount++;
                                ManifestBundleInfo bundleInfo = _manifest.GetBundleInfo(assetPath);
                                string bundleGroupName = string.Empty;
                                TreeViewItem bundleItem = FindItem(bundleInfo.Name.GetHashCode(), _root);
                                if (bundleItem != null)
                                    bundleGroupName = GetGroupName(bundleItem as BuildAnalyzerTreeViewItem);
                                if (bundleInfo != null && bundleInfo.ID != bundleInfoOfItem.ID && !string.IsNullOrEmpty(bundleGroupName) && bundleItem != null && !IsLegalDependent(bundleItem as BuildAnalyzerTreeViewItem, bundleInfoOfItem.Name))
                                    referenceAssetDataList.Add("*" + assetPath);
                                else
                                    referenceAssetDataList.Add(assetPath);
                            }
                        }
                    }
                    else
                    {
                        // 此处在analyzingText后边加个空格, 可和'依赖我的Bundle'中的analyzingText区别开来(因此处itemID根据文字变化)
                        if (!referenceAssetDataList.Contains(analyzingText + " "))
                            referenceAssetDataList.Add(analyzingText + " ");
                    }

                    referenceAssetDataList.Sort();

                    if (assetRefAssetCount > 0)
                        referenceAssetTreeInfo.title = $"依赖我的资源({assetRefAssetCount}个)";

                    #endregion
                }
            }

            List<DependenceTreeInfo> dependenceTreeInfoList = new List<DependenceTreeInfo>();
            if (dependentBundleTreeInfo != null)
            {
                dependentBundleTreeInfo.dataList.Sort();
                dependenceTreeInfoList.Add(dependentBundleTreeInfo);
            }

            if (dependentAssetTreeInfo != null)
                dependenceTreeInfoList.Add(dependentAssetTreeInfo);
            if (referenceBundleTreeInfo != null)
                dependenceTreeInfoList.Add(referenceBundleTreeInfo);
            if (referenceAssetTreeInfo != null)
                dependenceTreeInfoList.Add(referenceAssetTreeInfo);
            _parentWindow.DependenceTreeView.SetDependenceInfo(dependenceTreeInfoList);
        }

        /// <summary>
        /// 显示列表排序
        /// </summary>
        static List<TreeViewItem> OrderTreeViewList<T>(List<TreeViewItem> treeView, bool ascending, Func<TreeViewItem, T> sortFunc)
        {
            foreach (TreeViewItem item in treeView)
                if (item.hasChildren)
                    item.children = OrderTreeViewList(item.children, ascending, sortFunc);

            return ascending ? treeView.OrderBy(sortFunc).ToList() : treeView.OrderByDescending(sortFunc).ToList();
        }

        /// <summary>
        /// 排序变化处理
        /// </summary>
        void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            if (!_root.hasChildren)
                return;

            int curIndex = multiColumnHeader.sortedColumnIndex;
            if (curIndex == -1)
                return;

            bool ascending = multiColumnHeader.IsSortedAscending(curIndex);

            if (curIndex == 0) // 名字
                _root.children = OrderTreeViewList(_root.children, ascending, item => ((BuildAnalyzerTreeViewItem)item).info);
            else if (curIndex == 1) // 大小
                _root.children = OrderTreeViewList(_root.children, ascending, item => ((BuildAnalyzerTreeViewItem)item).size);

            Reload();
        }

        /// <summary>
        /// item选中改变处理
        /// </summary>
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            RefreshSelectionInfo();

            BuildAnalyzerTreeViewItem[] clickedItems = Array.ConvertAll(selectedIds.ToArray(), id => FindItem(id, _root) as BuildAnalyzerTreeViewItem);

            // 展开所有选中的item, 方便搜索时点击后查找
            foreach (BuildAnalyzerTreeViewItem item in clickedItems)
            {
                TreeViewItem parent = item.parent;
                while (parent != null && parent.depth != -1)
                {
                    SetExpanded(parent.id, true);
                    parent = parent.parent;
                }
            }
        }

        /// <summary>
        /// 双击item处理
        /// </summary>
        protected override void DoubleClickedItem(int id)
        {
            BuildAnalyzerTreeViewItem item = FindItem(id, _root) as BuildAnalyzerTreeViewItem;
            if (item.itemType == BuildAnalyzerTreeViewItem.ItemType.Asset)
            {
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
        }

        /// <summary>
        /// 右键处理, 弹出菜单复制内容
        /// </summary>
        protected override void ContextClickedItem(int id)
        {
            if (!(FindItem(id, rootItem) is BuildAnalyzerTreeViewItem item))
                return;

            string itemInfo = item.info;
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("复制"), false, () => { GUIUtility.systemCopyBuffer = itemInfo; });

            // 资源类型的Item才显示删除按钮, 并排除原始资源
            if (item.itemType == BuildAnalyzerTreeViewItem.ItemType.Asset && item.info.StartsWith("Assets/"))
            {
                menu.AddItem(new GUIContent("删除"), false, () =>
                {
                    BuildAnalyzerTreeViewItem[] clickedItems = Array.ConvertAll(GetSelection().ToArray(), id => FindItem(id, _root) as BuildAnalyzerTreeViewItem);
                    int clickedItemCount = clickedItems.Length;
                    for (int i = 0; i < clickedItemCount; i++)
                    {
                        BuildAnalyzerTreeViewItem item = clickedItems[i];
                        if (item.itemType != BuildAnalyzerTreeViewItem.ItemType.Asset)
                            continue;

                        item.parent.children.Remove(item);
                        AssetDatabase.DeleteAsset(item.info);
                        EditorUtility.DisplayProgressBar($"删除资源中", Path.GetFileName(item.info), (float)(i + 1) / clickedItemCount);
                    }

                    AssetDatabase.Refresh();
                    EditorUtility.ClearProgressBar();
                    Reload();
                    _parentWindow.Repaint();
                });
            }
            // Bundle类型的Item支持复制ab文件名
            else if (item.itemType == BuildAnalyzerTreeViewItem.ItemType.Bundle)
            {
                menu.AddItem(new GUIContent("复制(包括ab文件名)"), false, () =>
                {
                    BuildAnalyzerTreeViewItem[] clickedItems = Array.ConvertAll(GetSelection().ToArray(), id => FindItem(id, _root) as BuildAnalyzerTreeViewItem);
                    int clickedItemCount = clickedItems.Length;
                    for (int i = 0; i < clickedItemCount; i++)
                    {
                        BuildAnalyzerTreeViewItem item = clickedItems[i];
                        if (item.itemType == BuildAnalyzerTreeViewItem.ItemType.Bundle)
                        {
                            GUIUtility.systemCopyBuffer = item.displayName;
                            break;
                        }
                    }
                });
            }

            menu.ShowAsContext();
        }
    }
}