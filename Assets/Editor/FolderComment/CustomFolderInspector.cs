using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace EditorFolderComment
{
    /// <summary>
    /// 自定义文件夹Inspector
    /// </summary>
    [CustomEditor(typeof(DefaultAsset)), CanEditMultipleObjects]
    public class CustomFolderInspector : Editor
    {
        /// <summary>
        /// 备注
        /// </summary>
        string _comment = string.Empty;

        /// <summary>
        /// 备注是否改变
        /// </summary>
        bool _isCommentChanged;

        /// <summary>
        /// 备注标题文字大小
        /// </summary>
        static GUIStyle s_commentTitleStyle;

        /// <summary>
        /// 当前选中文件夹(可多选)的路径
        /// </summary>
        readonly List<string> _selectedFoldersPath = new();

        void OnEnable()
        {
            _selectedFoldersPath.Clear();

            // 支持多个同时设置备注
            for (int i = 0; i < targets.Length; ++i)
            {
                var assetPath = AssetDatabase.GetAssetPath(targets[i]);
                // 仅支持Assets目录下的文件夹
                if (AssetDatabase.IsValidFolder(assetPath) && assetPath.StartsWith("Assets"))
                    _selectedFoldersPath.Add(assetPath);
            }

            if (_selectedFoldersPath.Count > 0)
                _comment = FolderDataManager.AssetPathToComment(_selectedFoldersPath[0]);
        }

        void OnDestroy()
        {
            if (_isCommentChanged)
                FolderDataManager.SaveToFile();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_selectedFoldersPath.Count == 0)
                return;

            GUI.enabled = true;
            DrawFolder();
        }

        /// <summary>
        /// 绘制文件夹inspector内容
        /// </summary>
        void DrawFolder()
        {
            s_commentTitleStyle ??= new GUIStyle(EditorStyles.label) { fontSize = 12 };

            EditorGUILayout.PrefixLabel("文件夹备注:", s_commentTitleStyle);
            GUI.changed = false;
            _comment = EditorGUILayout.TextField(_comment);
            if (GUI.changed)
            {
                foreach (string assetPath in _selectedFoldersPath)
                    FolderDataManager.SetComment(assetPath, _comment);

                _isCommentChanged = true;
                EditorUtility.SetDirty(target);
                EditorApplication.RepaintProjectWindow();
            }
        }
    }
}