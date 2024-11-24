using UnityEngine;
using UnityEditor;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 版本记录管理窗口
    /// </summary>
    public class VersionRecordManagementWindow : EditorWindow
    {
        /// <summary>
        /// 打开此窗口
        /// </summary>
        public static void Open()
        {
            GetWindow<VersionRecordManagementWindow>("版本记录管理").minSize = new Vector2(460, 680);
        }

        /// <summary>
        /// 提示高度
        /// </summary>
        const float TipsHeight = 20;

        /// <summary>
        /// 提示图片和文本
        /// </summary>
        GUIContent _tipsContent;

        /// <summary>
        /// 提示的文本样式
        /// </summary>
        GUIStyle _tipsStyle;

        /// <summary>
        /// 备注列表
        /// </summary>
        VersionRecordManagementTreeView _recordCommentTreeView;

        void OnGUI()
        {
            // 提示
            if (_tipsContent == null)
            {
                _tipsStyle = new GUIStyle(UnityEngine.GUI.skin.label) { fontSize = 11 };
                GUIContent consoleInfoIconContent = EditorGUIUtility.IconContent("d_console.infoicon.sml");
                _tipsContent = new GUIContent("温馨提示:1.关闭窗口即可保存备注; 2.在对应版本右键可弹出删除按钮(可多选)", consoleInfoIconContent.image);
            }

            GUILayout.Label(_tipsContent, _tipsStyle, GUILayout.Height(TipsHeight));

            // 列表
            Rect treeViewRect = new Rect(0, TipsHeight, position.width, position.height - TipsHeight);
            _recordCommentTreeView ??= new VersionRecordManagementTreeView();
            _recordCommentTreeView.OnGUI(treeViewRect);
        }

        void OnDestroy()
        {
            _recordCommentTreeView?.SaveComment();
            if (HasOpenInstances<VersionContrastWindow>())
                GetWindow<VersionContrastWindow>().RefreshVersionFileNameList();
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        public void Refresh()
        {
            _recordCommentTreeView?.RefreshCommentList();
        }
    }
}