using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Collections;
using AssetModule.Editor.Build;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using Unity.EditorCoroutines.Editor;
using System.Text.RegularExpressions;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 版本对比窗口
    /// </summary>
    public class VersionContrastWindow : EditorWindow
    {
        /// <summary>
        /// 打开此窗口
        /// </summary>
        public static VersionContrastWindow Open()
        {
            VersionContrastWindow window = GetWindow<VersionContrastWindow>("历史版本对比");
            window.minSize = new Vector2(980, 500);
            return window;
        }

        /// <summary>
        /// 工具栏高度
        /// </summary>
        const int ToolBarHeight = 20;

        /// <summary>
        /// 最大下拉框长度
        /// </summary>
        const float MaxDropDownButtonWidth = 360;

        /// <summary>
        /// 未选择提示
        /// </summary>
        const string UnselectedTips = "未选择";

        /// <summary>
        /// 旧版本(左边)内容文件名
        /// </summary>
        string _oldBuildRecordFileName = string.Empty;

        /// <summary>
        /// 新版本(右边)内容文件名
        /// </summary>
        string _newBuildRecordFileName = string.Empty;

        /// <summary>
        /// 旧版本(左边)下拉框显示名字
        /// </summary>
        string _oldBuildRecordShowName = UnselectedTips;

        /// <summary>
        /// 新版本(右边)下拉框显示名字
        /// </summary>
        string _newBuildRecordShowName = UnselectedTips;

        /// <summary>
        /// 旧版本(左边)内容
        /// </summary>
        BuildRecord _oldBuildRecord;

        /// <summary>
        /// 新版本(右边)内容
        /// </summary>
        BuildRecord _newBuildRecord;

        /// <summary>
        /// 两个下拉框的GUIContent, 此处声明仅为了不一直在OnGUI中new
        /// </summary>
        readonly GUIContent _recordSelectGUIContent = new("未选择");

        /// <summary>
        /// 版本文件名字列表
        /// </summary>
        readonly List<string> _versionFileNameList = new();

        /// <summary>
        /// 版本记录文件名对应的备注
        /// </summary>
        readonly Dictionary<string, string> _recordFileNameToComment = new();

        /// <summary>
        /// 对比列表
        /// </summary>
        VersionContrastGroupTreeView _versionContrastGroupTreeView;

        /// <summary>
        /// 富文本样式
        /// </summary>
        GUIStyle _richTextStyle;

        /// <summary>
        /// 总览提示文本
        /// </summary>
        string _overviewTipsText;

        /// <summary>
        /// 加载记录数据的协程
        /// </summary>
        EditorCoroutine _loadRecordCoroutine;

        /// <summary>
        /// 加载进度
        /// </summary>
        float _loadRecordProgress;

        /// <summary>
        /// 加载记录的总占比
        /// </summary>
        internal const float LoadRecordProportion = 0.3f;

        /// <summary>
        /// 搜索框
        /// </summary>
        SearchField _searchField;

        void OnEnable()
        {
            ResetValues();
            RefreshVersionFileNameList();
            StartContrastNewestVersions();
        }

        void OnDestroy()
        {
            if (HasOpenInstances<VersionRecordManagementWindow>())
                GetWindow<VersionRecordManagementWindow>().Close();
        }

        void OnGUI()
        {
            if (_versionFileNameList.Count < 2)
            {
                GUILayout.Label("暂无2个或以上的打包版本记录文件, 无法进行对比");
                return;
            }

            // 工具栏
            Rect toolBarRect = new Rect(0, 0, position.width, ToolBarHeight);
            DrawToolBar(toolBarRect);

            // 加载记录数据
            if (_loadRecordCoroutine != null)
            {
                Rect rect = new Rect(position.width * 0.5f - position.width * 0.4f, position.height * 0.46f, position.width * 0.8f, 20);
                EditorGUI.ProgressBar(rect, _loadRecordProgress, "初始化中");
                return;
            }
            else if (!string.IsNullOrEmpty(_oldBuildRecordFileName) && !string.IsNullOrEmpty(_newBuildRecordFileName) && (_oldBuildRecord == null || _newBuildRecord == null))
            {
                _loadRecordProgress = 0;
                _loadRecordCoroutine = this.StartCoroutine(LoadBuildRecordAsync());
                Rect rect = new Rect(position.width * 0.5f - position.width * 0.4f, position.height * 0.46f, position.width * 0.8f, 20);
                EditorGUI.ProgressBar(rect, _loadRecordProgress, "初始化中");
                return;
            }

            // 提示栏
            Rect tipsBarRect = new Rect(0, ToolBarHeight + 1, position.width, ToolBarHeight);
            DrawTipsBar(tipsBarRect);

            // 展示数据的列表
            _versionContrastGroupTreeView ??= new VersionContrastGroupTreeView(this);
            if (_oldBuildRecord == null || _newBuildRecord == null || _oldBuildRecord.timestamp > _newBuildRecord.timestamp)
            {
                _versionContrastGroupTreeView.Reset();
                return;
            }

            _versionContrastGroupTreeView.SetRecord(_oldBuildRecord, _newBuildRecord);
            if (!_versionContrastGroupTreeView.IsInitFinished)
            {
                Rect rect = new Rect(position.width * 0.5f - position.width * 0.4f, position.height * 0.46f, position.width * 0.8f, 20);
                EditorGUI.ProgressBar(rect, LoadRecordProportion + (1 - LoadRecordProportion) * _versionContrastGroupTreeView.InitProgress, "初始化中");
                return;
            }

            _versionContrastGroupTreeView.OnGUI(new Rect(0, tipsBarRect.yMax, position.width, position.height - tipsBarRect.yMax));
        }

        /// <summary>
        /// 重置参数
        /// </summary>
        public void ResetValues()
        {
            _oldBuildRecord = null;
            _newBuildRecord = null;
        }

        /// <summary>
        /// 刷新版本名字列表
        /// </summary>
        public void RefreshVersionFileNameList()
        {
            VersionContrastUtils.GetVersionFileNameList(_versionFileNameList);
            VersionContrastUtils.LoadCommentDataAndRefreshCommentDictionary(_recordFileNameToComment);

            // 有备注的优先排列
            _versionFileNameList.Sort((a, b) =>
            {
                if (_recordFileNameToComment.ContainsKey(a) && !_recordFileNameToComment.ContainsKey(b))
                    return -1;
                if (!_recordFileNameToComment.ContainsKey(a) && _recordFileNameToComment.ContainsKey(b))
                    return 1;
                return 0;
            });

            if (!string.IsNullOrEmpty(_oldBuildRecordFileName))
            {
                if (_versionFileNameList.Contains(_oldBuildRecordFileName))
                    _oldBuildRecordShowName = VersionContrastUtils.ToShowName(_oldBuildRecordFileName, _recordFileNameToComment);
                else
                {
                    _oldBuildRecord = null;
                    _oldBuildRecordFileName = string.Empty;
                    _oldBuildRecordShowName = UnselectedTips;
                }
            }

            if (!string.IsNullOrEmpty(_newBuildRecordFileName))
            {
                if (_versionFileNameList.Contains(_newBuildRecordFileName))
                    _newBuildRecordShowName = VersionContrastUtils.ToShowName(_newBuildRecordFileName, _recordFileNameToComment);
                else
                {
                    _newBuildRecord = null;
                    _newBuildRecordFileName = string.Empty;
                    _newBuildRecordShowName = UnselectedTips;
                }
            }

            if (HasOpenInstances<VersionRecordManagementWindow>())
                GetWindow<VersionRecordManagementWindow>().Refresh();
        }

        /// <summary>
        /// 对比两个最新的版本
        /// </summary>
        public void StartContrastNewestVersions(bool forceRefresh = false)
        {
            if (!forceRefresh && !string.IsNullOrEmpty(_oldBuildRecordFileName))
                return;

            string buildRecordFolderPath = BuildUtils.TranslateToBuildPath(BuildUtils.BuildRecordFolderName);
            DirectoryInfo directoryInfo = new DirectoryInfo(buildRecordFolderPath);
            if (!directoryInfo.Exists)
                return;

            List<long> fileBuildTimeList = new List<long>();
            Dictionary<long, string> buildTimeToFileName = new Dictionary<long, string>();

            FileInfo[] fileInfoList = directoryInfo.GetFiles("*.json");
            foreach (FileInfo fileInfo in fileInfoList)
            {
                string fileName = fileInfo.Name;
                if (!fileName.StartsWith(BuildUtils.BuildRecordFilePrefix) || !fileName.EndsWith(".json"))
                    continue;

                long fileBuildTime = VersionContrastUtils.GetVersionBuildTime(fileName);
                fileBuildTimeList.Add(fileBuildTime);
                buildTimeToFileName.Add(fileBuildTime, fileName);
            }

            int fileCount = fileBuildTimeList.Count;
            if (fileCount < 2)
                return;

            fileBuildTimeList.Sort();

            _oldBuildRecord = null;
            _oldBuildRecordFileName = buildTimeToFileName[fileBuildTimeList[fileCount - 2]];
            _oldBuildRecordShowName = VersionContrastUtils.ToShowName(_oldBuildRecordFileName, _recordFileNameToComment);
            _newBuildRecord = null;
            _newBuildRecordFileName = buildTimeToFileName[fileBuildTimeList[fileCount - 1]];
            _newBuildRecordShowName = VersionContrastUtils.ToShowName(_newBuildRecordFileName, _recordFileNameToComment);
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
                    float oldTextWidth = GetStrLengthWithoutLabel(_oldBuildRecordShowName) * 7.5f + 15;
                    if (oldTextWidth > MaxDropDownButtonWidth)
                        oldTextWidth = MaxDropDownButtonWidth;
                    Rect manifestDropDownRect = new Rect(0, 0, oldTextWidth, ToolBarHeight);
                    _recordSelectGUIContent.text = _oldBuildRecordShowName;
                    if (EditorGUI.DropdownButton(manifestDropDownRect, _recordSelectGUIContent, FocusType.Keyboard, EditorStyles.toolbarDropDown))
                    {
                        RefreshVersionFileNameList();

                        GenericMenu menu = new GenericMenu();
                        if (_oldBuildRecordShowName != UnselectedTips)
                        {
                            menu.AddItem(new GUIContent("清空选择"), false, () =>
                            {
                                _oldBuildRecord = null;
                                _oldBuildRecordFileName = string.Empty;
                                _oldBuildRecordShowName = UnselectedTips;
                                _newBuildRecord = null;
                                _newBuildRecordFileName = string.Empty;
                                _newBuildRecordShowName = UnselectedTips;
                            });
                        }

                        foreach (string fileName in _versionFileNameList)
                        {
                            string showName = VersionContrastUtils.ToShowName(fileName, _recordFileNameToComment);
                            bool isSame = _oldBuildRecordFileName.Equals(fileName);
                            menu.AddItem(new GUIContent(showName), isSame, () =>
                            {
                                if (isSame)
                                    return;

                                _oldBuildRecord = null;
                                _oldBuildRecordFileName = fileName;
                                _oldBuildRecordShowName = showName;

                                if (!string.IsNullOrEmpty(_newBuildRecordFileName))
                                {
                                    long oldSelectBuildTime = VersionContrastUtils.GetVersionBuildTime(_oldBuildRecordFileName);
                                    long newSelectBuildTime = VersionContrastUtils.GetVersionBuildTime(_newBuildRecordFileName);
                                    if (oldSelectBuildTime >= newSelectBuildTime)
                                    {
                                        _newBuildRecord = null;
                                        _newBuildRecordFileName = string.Empty;
                                        _newBuildRecordShowName = UnselectedTips;
                                    }
                                }
                            });
                        }

                        menu.DropDown(manifestDropDownRect);
                    }

                    float newTextWidth = GetStrLengthWithoutLabel(_newBuildRecordShowName) * 7.5f + 15;
                    if (newTextWidth > MaxDropDownButtonWidth)
                        newTextWidth = MaxDropDownButtonWidth;
                    manifestDropDownRect = new Rect(oldTextWidth, 0, newTextWidth, 20);
                    _recordSelectGUIContent.text = _newBuildRecordShowName;
                    if (EditorGUI.DropdownButton(manifestDropDownRect, _recordSelectGUIContent, FocusType.Keyboard, EditorStyles.toolbarDropDown))
                    {
                        RefreshVersionFileNameList();

                        GenericMenu menu = new GenericMenu();
                        if (_newBuildRecordShowName != UnselectedTips)
                        {
                            menu.AddItem(new GUIContent("清空选择"), false, () =>
                            {
                                _newBuildRecord = null;
                                _newBuildRecordFileName = string.Empty;
                                _newBuildRecordShowName = UnselectedTips;
                            });
                        }

                        if (!string.IsNullOrEmpty(_oldBuildRecordFileName))
                        {
                            int itemCount = 0;
                            long oldSelectBuildTime = VersionContrastUtils.GetVersionBuildTime(_oldBuildRecordFileName);
                            foreach (string fileName in _versionFileNameList)
                            {
                                // 只显示比左边更新的文件
                                long fileBuildTime = VersionContrastUtils.GetVersionBuildTime(fileName);
                                if (fileBuildTime <= oldSelectBuildTime)
                                    continue;

                                itemCount++;
                                string showName = VersionContrastUtils.ToShowName(fileName, _recordFileNameToComment);
                                bool isSame = _newBuildRecordFileName.Equals(fileName);
                                menu.AddItem(new GUIContent(showName), isSame, () =>
                                {
                                    if (isSame)
                                        return;

                                    _newBuildRecord = null;
                                    _newBuildRecordFileName = fileName;
                                    _newBuildRecordShowName = showName;
                                });
                            }

                            if (itemCount == 0)
                                menu.AddItem(new GUIContent("没有更新的版本可选择(请重新选择左侧版本)"), false, () => { });
                        }
                        else
                            menu.AddItem(new GUIContent("请先选择左侧版本"), false, () => { });

                        menu.DropDown(manifestDropDownRect);
                    }

                    Rect versionRecordManagementBtnRect = new Rect(manifestDropDownRect.xMax, 0, 83, 20);
                    if (UnityEngine.GUI.Button(versionRecordManagementBtnRect, "版本记录管理", EditorStyles.toolbarButton))
                        VersionRecordManagementWindow.Open();

                    // 搜索框
                    Rect searchFieldRect = new Rect(position.width - 202, 2, 200, 20);
                    if (_searchField == null)
                    {
                        _searchField = new SearchField();
                        _searchField.downOrUpArrowKeyPressed += () => { _versionContrastGroupTreeView?.SetFocusAndEnsureSelectedItem(); };
                    }

                    if (_versionContrastGroupTreeView != null)
                        _versionContrastGroupTreeView.searchString = _searchField.OnToolbarGUI(searchFieldRect, _versionContrastGroupTreeView.searchString);
                    else
                        _searchField.OnToolbarGUI(searchFieldRect, string.Empty);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// 绘制提示栏
        /// </summary>
        void DrawTipsBar(Rect tipsBarRect)
        {
            GUILayout.BeginArea(tipsBarRect, EditorStyles.toolbar);
            {
                if (string.IsNullOrEmpty(_oldBuildRecordFileName) || string.IsNullOrEmpty(_newBuildRecordFileName))
                    EditorGUI.LabelField(new Rect(2, 0, 200, 20), "请选择版本进行对比~"); // 此处不使用GUILayout, 因为删除版本时刷新会导致错误:ArgumentException: Getting control 0's position in a group with only 0 controls when doing Repaint.参考:https://www.cnblogs.com/fzuljz/p/11138096.html
                else
                {
                    _richTextStyle ??= new GUIStyle(UnityEngine.GUI.skin.label) { richText = true };
                    GUILayout.Label(_overviewTipsText, _richTextStyle);
                }

                // 关闭排序按钮
                if (_versionContrastGroupTreeView != null && _versionContrastGroupTreeView.multiColumnHeader != null && _versionContrastGroupTreeView.multiColumnHeader.sortedColumnIndex >= 0)
                {
                    Rect closeSortBtnRect = new Rect(position.width - 66, 0, 66, 20);
                    if (UnityEngine.GUI.Button(closeSortBtnRect, "关闭排序", EditorStyles.toolbarButton))
                    {
                        ResetValues();
                        _versionContrastGroupTreeView?.CloseSorting();
                    }
                }
            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// 异步加载记录数据
        /// </summary>
        IEnumerator LoadBuildRecordAsync()
        {
            if (_oldBuildRecord == null)
            {
                string fullPath = AssetPath.CombinePath(BuildUtils.TranslateToBuildPath(BuildUtils.BuildRecordFolderName), $"{_oldBuildRecordFileName}");
                if (File.Exists(fullPath))
                    _oldBuildRecord = JsonUtility.FromJson<BuildRecord>(File.ReadAllText(fullPath));
                if (_oldBuildRecord == null || _oldBuildRecord.recordManifestList == null)
                {
                    _oldBuildRecordFileName = string.Empty;
                    _oldBuildRecordShowName = UnselectedTips;
                    RefreshVersionFileNameList();
                    ShowNotification(new GUIContent("左边版本文件加载失败, 无法进行对比"));
                    _loadRecordCoroutine = null;
                    yield break;
                }
            }

            _loadRecordProgress = 0.45f * LoadRecordProportion;
            Repaint();
            yield return null;

            if (_newBuildRecord == null)
            {
                string fullPath = AssetPath.CombinePath(BuildUtils.TranslateToBuildPath(BuildUtils.BuildRecordFolderName), $"{_newBuildRecordFileName}");
                if (File.Exists(fullPath))
                    _newBuildRecord = JsonUtility.FromJson<BuildRecord>(File.ReadAllText(fullPath));
                if (_newBuildRecord?.recordManifestList == null)
                {
                    _newBuildRecordFileName = string.Empty;
                    _newBuildRecordShowName = UnselectedTips;
                    RefreshVersionFileNameList();
                    ShowNotification(new GUIContent("右边版本文件加载失败, 无法进行对比"));
                    _loadRecordCoroutine = null;
                    yield break;
                }
            }

            _loadRecordProgress = 0.9f * LoadRecordProportion;
            Repaint();
            yield return null;

            (int oldFileCount, long oldTotalSize) = GetRecordFileCountAndTotalSize(_oldBuildRecord);
            (int newFileCount, long newTotalSize) = GetRecordFileCountAndTotalSize(_newBuildRecord);
            (int updateCount, long updateSize) = GetUpdateSizeBetweenTwoRecords(_oldBuildRecord, _newBuildRecord);

            long changedSize = newTotalSize - oldTotalSize;
            string changedSizeTips = string.Empty;
            if (changedSize > 0)
                changedSizeTips = $"(<color=red>↑{Utility.FormatBytes(changedSize)}</color>)";
            else if (changedSize < 0)
                changedSizeTips = $"(<color=lime>↓{Utility.FormatBytes(Math.Abs(changedSize))}</color>)";

            _overviewTipsText = $"总大小:{Utility.FormatBytes(oldTotalSize)} → {Utility.FormatBytes(newTotalSize)}{changedSizeTips}     " +
                                $"总文件数:{oldFileCount} → {newFileCount}     " +
                                $"更新大小:{Utility.FormatBytes(updateSize)}     " +
                                $"更新文件数:{updateCount}";

            _loadRecordCoroutine = null;
            _loadRecordProgress = 0; // 加载完就清零
        }

        /// <summary>
        /// 根据记录对象获取文件总数和总大小
        /// </summary>
        static (int, long) GetRecordFileCountAndTotalSize(BuildRecord buildRecord)
        {
            int fileCount = 0;
            long totalSize = 0;
            // 版本和清单文件
            foreach (VersionFileInfo versionFileInfo in buildRecord.versionFileInfoList)
            {
                fileCount++;
                totalSize += versionFileInfo.size;
            }

            // 资源文件
            foreach (RecordManifest recordManifest in buildRecord.recordManifestList)
            {
                if (recordManifest.recordBundleInfoList == null)
                    continue;

                foreach (RecordBundleInfo bundleInfo in recordManifest.recordBundleInfoList)
                {
                    fileCount++;
                    totalSize += bundleInfo.Size;
                }
            }

            return (fileCount, totalSize);
        }

        /// <summary>
        /// 根据两个记录获取需要更新的大小
        /// </summary>
        static (int, long) GetUpdateSizeBetweenTwoRecords(BuildRecord oldRecord, BuildRecord newRecord)
        {
            int updateCount = 0;
            long updateSize = 0;

            // 计算版本和清单文件
            Dictionary<string, string> oldVersionFileNameToHash = new Dictionary<string, string>();
            foreach (VersionFileInfo oldVersionFileInfo in oldRecord.versionFileInfoList)
                oldVersionFileNameToHash.Add(oldVersionFileInfo.name, oldVersionFileInfo.hash);
            foreach (VersionFileInfo newVersionFileInfo in newRecord.versionFileInfoList)
            {
                if (!oldVersionFileNameToHash.TryGetValue(newVersionFileInfo.name, out string hash) || hash != newVersionFileInfo.hash)
                {
                    updateCount++;
                    updateSize += newVersionFileInfo.size;
                }
            }

            // 计算资源文件
            Dictionary<string, string> oldFileNameToHash = new Dictionary<string, string>();
            foreach (RecordManifest recordManifest in oldRecord.recordManifestList)
            {
                if (recordManifest.recordBundleInfoList == null)
                    continue;

                foreach (RecordBundleInfo bundleInfo in recordManifest.recordBundleInfoList)
                {
                    if (oldFileNameToHash.ContainsKey(bundleInfo.Name))
                    {
                        Debug.LogError($"版本{oldRecord.version}中出现重复包名:{bundleInfo.Name}");
                        continue;
                    }

                    oldFileNameToHash.Add(bundleInfo.Name, bundleInfo.Hash);
                }
            }

            foreach (RecordManifest recordManifest in newRecord.recordManifestList)
            {
                if (recordManifest.recordBundleInfoList == null)
                    continue;

                foreach (RecordBundleInfo bundleInfo in recordManifest.recordBundleInfoList)
                {
                    if (!oldFileNameToHash.TryGetValue(bundleInfo.Name, out string hash) || hash != bundleInfo.Hash)
                    {
                        updateCount++;
                        updateSize += bundleInfo.Size;
                    }
                }
            }

            return (updateCount, updateSize);
        }

        /// <summary>
        /// 获取字符串长度, 中文当两个字符(参考:https://www.cnblogs.com/xu-yi/p/10541748.html)
        /// </summary>
        static int GetStrLength(string content)
        {
            int tabLength = new Regex("\t").Matches(content).Count;            // 计算\t个数(全角空格), \t不计入打字字数
            int signalCount = new Regex("(:|\\(|\\))").Matches(content).Count; // 计算冒号和括号的个数, 两个冒号或括号视为占用一个位置
            int contentLength = 0;
            byte[] s = new ASCIIEncoding().GetBytes(content);
            for (int i = 0; i < s.Length; i++)
                contentLength += s[i] == 63 ? 2 : 1;
            return contentLength - tabLength - signalCount / 2;
        }

        /// <summary>
        /// 获取去除富文本标签后的字符串长度
        /// </summary>
        static int GetStrLengthWithoutLabel(string content)
        {
            int ignoreLength = 0;

            MatchCollection mc = Regex.Matches(content, "<\\w+?=.+?>"); // 匹配标签头, 例如:<color=#AC7CFF>
            foreach (Match m in mc)
                ignoreLength += GetStrLength(m.ToString());

            // 有匹配到标签头时才需要匹配标签尾
            if (ignoreLength > 0)
            {
                mc = Regex.Matches(content, "</\\w+?>"); // 匹配标签尾, 例如</color>
                foreach (Match m in mc)
                    ignoreLength += GetStrLength(m.ToString());
            }

            return GetStrLength(content) - ignoreLength;
        }
    }
}