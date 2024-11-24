using System.Linq;
using System.Collections.Generic;

namespace AssetModule
{
    /// <summary>
    /// Dependency对象管理
    /// </summary>
    public static class DependencyHandler
    {
        /// <summary>
        /// 依赖对象缓存
        /// </summary>
        static readonly Dictionary<string, Dependency> Cache = new();

        /// <summary>
        /// 加载Dependency对象
        /// </summary>
        public static Dependency LoadAsync(string address)
        {
            if (!Cache.TryGetValue(address, out Dependency dependency))
            {
                dependency = new Dependency { address = address };
                Cache.Add(address, dependency);
            }

            dependency.Load();

            return dependency;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        internal static void RemoveCache(string address)
        {
            Cache.Remove(address);
        }

        /// <summary>
        /// 清除缓存并全部卸载
        /// </summary>
        internal static void ClearCache()
        {
            Dependency[] dependencies = Cache.Values.ToArray();
            foreach (Dependency dependency in dependencies)
            {
                dependency.FullyRelease();
                dependency.Unload();
            }

            Cache.Clear();
        }
    }
}