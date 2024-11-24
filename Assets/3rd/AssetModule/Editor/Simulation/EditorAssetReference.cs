using System.Collections.Generic;

namespace AssetModule.Editor.Simulation
{
    /// <summary>
    /// 编辑器下已使用资源记录, 记录后资源卸载(Resources.UnloadAsset)时先判断是否有使用, 可避免误卸载
    /// </summary>
    public static class EditorAssetReference
    {
        static readonly Dictionary<string, int> References = new();

        public static bool IsUsing(string key)
        {
            return References.TryGetValue(key, out int num) && num > 0;
        }

        public static void AddReference(string key)
        {
            if (!References.TryAdd(key, 1))
                References[key]++;
        }

        public static void ReleaseReference(string key)
        {
            if (References.ContainsKey(key))
                References[key]--;
        }
    }
}