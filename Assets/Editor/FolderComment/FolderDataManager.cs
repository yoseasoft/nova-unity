using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace EditorFolderComment
{
    /// <summary>
    /// 文件注释数据
    /// </summary>
    internal class FolderData
    {
        /// <summary>
        /// 该路径仅作为参考(方便查看配置文本)，并无实际意义
        /// </summary>
        internal string assetPath;

        /// <summary>
        /// 注释
        /// </summary>
        internal string comment;
    }

    /// <summary>
    /// 编辑器文件夹绘制使用的相关数据管理器
    /// </summary>
    [InitializeOnLoad]
    public static class FolderDataManager
    {
        /// <summary>
        /// 读取、保存路径
        /// </summary>
        static readonly string s_saveDataPath = Application.dataPath + "/../ProjectSettings/EditorFolderCommentData.txt";

        /// <summary>
        /// 文件夹注释数据{[guid] = folderData{assetPath,comment}}
        /// </summary>
        static readonly Dictionary<string, FolderData> s_guid2FolderData = new();

        /// <summary>
        /// 静态构造函数,标记InitializeOnLoad后,会由Unity在启动或代码编译完成时自动调用
        /// </summary>
        static FolderDataManager()
        {
            LoadFromFile(s_saveDataPath);
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        }

        /// <summary>
        /// 根据路径获取注释
        /// <param name="assetPath">文件夹路径</param>
        /// <return>文件夹注释，如果没有则返回空字符串</return>
        /// </summary>
        public static string AssetPathToComment(string assetPath)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            return AssetGuidToComment(guid);
        }

        /// <summary>
        /// 根据资源唯一ID获取注释
        /// <param name="guid">文件夹唯一id</param>
        /// <return>文件夹注释，如果没有则返回空字符串</return>
        /// </summary>
        static string AssetGuidToComment(string guid)
        {
            return s_guid2FolderData.TryGetValue(guid, out FolderData findData) ? findData.comment : string.Empty;
        }

        /// <summary>
        /// 获取文件夹数据
        /// <param name="guid">文件夹唯一id</param>
        /// <return>文件夹数据，没有则返回空</return>
        /// </summary>
        static FolderData GetFolderData(string guid)
        {
            s_guid2FolderData.TryGetValue(guid, out FolderData findData);
            return findData;
        }

        /// <summary>
        /// 设置文件夹注释
        /// <param name="assetPath">文件夹路径</param>
        /// <param name="comment">文件夹注释</param>
        /// </summary>
        public static void SetComment(string assetPath, string comment)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("FolderDataManager SetComment error: assetPath is empty");
                return;
            }

            if (!assetPath.StartsWith("Assets"))
            {
                Debug.LogError("FolderDataManager SetComment error: not Unity project path=" + assetPath);
                return;
            }

            string guid = AssetDatabase.AssetPathToGUID(assetPath);

            // 如果没有这个数据记录，则添加记录
            if (!s_guid2FolderData.TryGetValue(guid, out FolderData findData))
            {
                findData = new FolderData { assetPath = assetPath, comment = comment };
                s_guid2FolderData.Add(guid, findData);
            }
            // 否则更改或删除记录
            else
            {
                // 填空时删除记录
                if (string.IsNullOrEmpty(comment))
                    s_guid2FolderData.Remove(guid);
                // 修改记录
                else
                {
                    findData.assetPath = assetPath;
                    findData.comment = comment;
                }
            }
        }

        /// <summary>
        /// 保存文件夹信息到文件中
        /// </summary>
        public static void SaveToFile()
        {
            SaveToFile(s_saveDataPath);
        }

        /// <summary>
        /// 保存文件夹信息到文件中
        /// <param name="path">保存路径</param>
        /// </summary>
        static void SaveToFile(string path)
        {
            if (s_guid2FolderData.Count == 0)
            {
                File.Delete(path);
                return;
            }

            var saveStr = new StringBuilder();
            foreach (var iter in s_guid2FolderData)
            {
                saveStr.Append(iter.Key);
                saveStr.Append('\n');
                saveStr.Append(iter.Value.assetPath);
                saveStr.Append('\n');
                saveStr.Append(iter.Value.comment);
                saveStr.Append('\n');
            }

            if (saveStr.Length > 0)
                saveStr.Remove(saveStr.Length - 1, 1);

            var pathDirectory = new DirectoryInfo(path).Parent;
            if (pathDirectory is { Exists: false })
                pathDirectory.Create();

            File.WriteAllText(path, saveStr.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 从文件读取文件夹信息
        /// <param name="path">保存路径</param>
        /// </summary>
        static void LoadFromFile(string path)
        {
            s_guid2FolderData.Clear();

            if (!File.Exists(path))
                return;

            try
            {
                using var readStream = new StreamReader(path);
                while (true)
                {
                    string guid = readStream.ReadLine();
                    if (string.IsNullOrEmpty(guid))
                        break;

                    string assetPath = readStream.ReadLine();
                    if (string.IsNullOrEmpty(assetPath))
                        throw new Exception("FolderDataManager LoadFromFile error: not found assetPath, guid=" + guid);

                    string comment = readStream.ReadLine();
                    if (string.IsNullOrEmpty(comment))
                        throw new Exception("FolderDataManager LoadFromFile error: not found comment, name=" + assetPath);

                    var folderAssetData = new FolderData { assetPath = assetPath, comment = comment };
                    s_guid2FolderData.Add(guid, folderAssetData);
                }

                readStream.Close();
            }
            catch (Exception e)
            {
                File.Delete(path);
                Debug.LogError("FolderDataManager LoadFromFile exception: e=" + e);
            }
        }

        /// <summary>
        /// 文件夹在树状列表时(或文件夹视图滑动条最小时)的显示风格
        /// </summary>
        static GUIStyle s_guiStyleLabelTree;

        /// <summary>
        /// 文件夹在非树状列表时(且不是文件夹视图滑动条最小时)的显示风格
        /// </summary>
        static GUIStyle s_guiStyleLabelNotTree;

        /// <summary>
        /// 每个资源在Project视图上的OnGUI通知
        /// </summary>
        /// <param name="guid">资源唯一id</param>
        /// <param name="rect">在视图中的大小、位置信息</param>
        static void ProjectWindowItemOnGUI(string guid, Rect rect)
        {
            var folderData = GetFolderData(guid);
            if (folderData == null || string.IsNullOrEmpty(folderData.comment))
                return;

            if (s_guiStyleLabelTree == null)
            {
                s_guiStyleLabelTree = new GUIStyle(EditorStyles.label) { fontSize = 12 };
                s_guiStyleLabelNotTree = new GUIStyle(EditorStyles.label) { fontSize = 10 };
            }

            GUIContent commentContent = new GUIContent(folderData.comment);
            bool isTree = IsTreeView(rect);
            GUIStyle labelStyle = isTree ? s_guiStyleLabelTree : s_guiStyleLabelNotTree;
            Vector2 labelSize = labelStyle.CalcSize(commentContent);
            float offsetYWhenCurrentSelectAsset = 0;

            if (!isTree)
            {
                IsIconSmall(ref rect);

                if (Selection.objects != null)
                {
                    if (null != Array.Find(Selection.objects, v => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(v)) == guid))
                        offsetYWhenCurrentSelectAsset = -labelSize.y * 0.167f;
                }
            }

            Rect textRect = new Rect(
                rect.x + Mathf.Max(0, (rect.width - labelSize.x) * 0.5f),
                rect.yMax + (isTree ? -labelSize.y - labelSize.y * 0.167f : labelSize.y * 0.33f - labelSize.y) + offsetYWhenCurrentSelectAsset,
                labelSize.x, labelSize.y);

            if (isTree)
            {
                textRect.width = Math.Min(labelSize.x, rect.width / 3);
                textRect.x = rect.xMax - textRect.width;
                textRect.y = rect.y;
            }

            commentContent.text = CropText(labelStyle, commentContent.text, rect.width);
            EditorGUI.LabelField(textRect, commentContent, labelStyle);
        }

        /// <summary>
        /// 判断是否在树状列表上(或文件视图滑动条最小时)
        /// </summary>
        static bool IsTreeView(Rect rect)
        {
            return rect.height <= 21f;
        }

        // https://github.com/PhannGor/unity3d-rainbow-folders/blob/master/Assets/Plugins/RainbowFolders/Editor/Scripts/RainbowFoldersBrowserIcons.cs
        static void IsIconSmall(ref Rect rect)
        {
            if (rect.width > rect.height)
                rect.width = rect.height;
            else
                rect.height = rect.width;
        }

        static System.Reflection.MethodInfo s_getNumCharactersThatFitWithinWidth;

        /// <summary>
        /// UnityEditor.ObjectListArea - GetCroppedLabelText (G:1211)
        /// </summary>
        static string CropText(GUIStyle self, string text, float cropWidth, string symbol = "…")
        {
            if (null == s_getNumCharactersThatFitWithinWidth)
            {
                s_getNumCharactersThatFitWithinWidth = typeof(GUIStyle).GetMethod("GetNumCharactersThatFitWithinWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (null == s_getNumCharactersThatFitWithinWidth)
                    return "";
            }

            int num;
            int thatFitWithinWidth = (int)s_getNumCharactersThatFitWithinWidth.Invoke(self, new object[] { text, cropWidth });
            switch (thatFitWithinWidth)
            {
                case -1:
                    return text;
                case 0:
                case 1:
                    num = 0;
                    break;
                default:
                    num = thatFitWithinWidth != text.Length ? 1 : 0;
                    break;
            }

            text = num == 0 ? text : text[..(thatFitWithinWidth - 1)] + symbol;
            return text;
        }
    }
}