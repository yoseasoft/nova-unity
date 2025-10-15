using System.IO;
using UnityEditor;

namespace SVNUnityExtension
{
    /// <summary>
    /// SVN菜单
    /// </summary>
    public static class SvnMenuItems
    {
        /// <summary>
        /// Assets目录下的资源更新
        /// </summary>
        [MenuItem("Assets/SVN更新", false, 2000)]
        static void UpdateAsset()
        {
            string[] assetGuidList = Selection.assetGUIDs;
            if (assetGuidList.Length == 0)
                return;

            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuidList[^1]);
            SvnHelper.Update(Directory.Exists(assetPath) ? assetPath : new FileInfo(assetPath).DirectoryName);
        }

        /// <summary>
        /// 更新菜单按钮验证方法
        /// </summary>
        [MenuItem("Assets/SVN更新", true, 2000)]
        static bool UpdateAssetValidate()
        {
            return Selection.assetGUIDs.Length > 0;
        }

        /// <summary>
        /// Assets目录下的资源提交
        /// </summary>
        [MenuItem("Assets/SVN提交", false, 2001)]
        static void CommitAsset()
        {
            string[] assetGuidList = Selection.assetGUIDs;
            if (assetGuidList.Length == 0)
                return;

            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuidList[^1]);
            SvnHelper.Commit(Directory.Exists(assetPath) ? assetPath : new FileInfo(assetPath).DirectoryName);
        }

        /// <summary>
        /// 提交菜单按钮验证方法
        /// </summary>
        [MenuItem("Assets/SVN提交", true, 2001)]
        static bool CommitAssetValidate()
        {
            return Selection.assetGUIDs.Length > 0;
        }

        /// <summary>
        /// 更新工程
        /// </summary>
        [MenuItem("SVN/更新工程 _F5")]
        internal static void UpdateProject()
        {
            SvnHelper.Update("./");
        }
    }
}