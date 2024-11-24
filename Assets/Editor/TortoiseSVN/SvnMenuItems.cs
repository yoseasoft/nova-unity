using System.IO;
using UnityEditor;

namespace SVNUnityExtension
{
    /// <summary>
    /// SVN菜单
    /// </summary>
    public class SvnMenuItems
    {
        /// <summary>
        /// Assets目录下的资源更新
        /// </summary>
        [MenuItem("Assets/SVN更新", false, 2000)]
        static void UpdateAsset()
        {
            string[] assetGUIDList = Selection.assetGUIDs;
            if (assetGUIDList.Length == 0)
                return;

            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDList[assetGUIDList.Length - 1]);
            if (Directory.Exists(assetPath))
                SvnHelper.Update(assetPath);
            else
                SvnHelper.Update(new FileInfo(assetPath).DirectoryName);
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
            string[] assetGUIDList = Selection.assetGUIDs;
            if (assetGUIDList.Length == 0)
                return;

            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDList[assetGUIDList.Length - 1]);
            if (Directory.Exists(assetPath))
                SvnHelper.Commit(assetPath);
            else
                SvnHelper.Commit(new FileInfo(assetPath).DirectoryName);
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
        [MenuItem("SVN/更新工程")]
        internal static void UpdateProject()
        {
            // 字符开头直接星号分隔可代表当前目录(即./), 所以下面无论前面有没有都直接分割
            string updateDir = string.Empty;

            if (Directory.Exists("Assets/_Resources/.svn"))
                updateDir += "*Assets/_Resources/";

            if (Directory.Exists("Assets/Lua/game/.svn"))
                updateDir += "*Assets/Lua/game/";

            if (Directory.Exists("Assets/Lua/nova/.svn"))
                updateDir += "*Assets/Lua/nova/";

            if (Directory.Exists("Assets/Scripts/nova/.svn"))
                updateDir += "*Assets/Scripts/nova/";

            if (Directory.Exists("Assets/Editor/nova/.svn"))
                updateDir += "*Assets/Editor/nova/";

            // 至少更新当前目录
            if (string.IsNullOrEmpty(updateDir))
                updateDir = "*";

            SvnHelper.Update(updateDir);
        }
    }
}