using System;
using System.IO;
using UnityEngine;
using AssetModule.Editor.Build;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 版本备注数据
    /// </summary>
    [Serializable]
    public class RecordComment
    {
        /// <summary>
        /// 已备注的版本文件名列表
        /// </summary>
        public string[] recordFileNameList;

        /// <summary>
        /// 对应的备注列表
        /// </summary>
        public string[] recordCommentList;

        /// <summary>
        /// 版本备注数据记录文件名
        /// </summary>
        public const string RecordCommentFileName = "recordComment.json";

        /// <summary>
        /// 加载备注数据
        /// </summary>
        public static RecordComment LoadRecordComment()
        {
            string recordCommentFilePath = BuildUtils.TranslateToBuildPath(AssetPath.CombinePath(BuildUtils.BuildRecordFolderName, RecordCommentFileName));
            if (!File.Exists(recordCommentFilePath))
                return null;

            RecordComment recordComment = JsonUtility.FromJson<RecordComment>(File.ReadAllText(recordCommentFilePath));
            return recordComment;
        }
    }
}