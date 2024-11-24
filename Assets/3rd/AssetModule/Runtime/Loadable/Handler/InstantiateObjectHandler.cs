using System;

namespace AssetModule
{
    /// <summary>
    /// 实例化对象管理
    /// </summary>
    public static class InstantiateObjectHandler
    {
        /// <summary>
        /// 同步实例化对象
        /// </summary>
        internal static InstantiateObject Instantiate(string address)
        {
            InstantiateObject instantiateObject = new InstantiateObject { address = address };
            instantiateObject.Load();
            instantiateObject.LoadImmediately();
            return instantiateObject;
        }

        /// <summary>
        /// 异步实例化对象
        /// </summary>
        internal static InstantiateObject InstantiateAsync(string address, Action<InstantiateObject> completed = null)
        {
            InstantiateObject instantiateObject = new InstantiateObject { address = address };
            if (completed != null)
                instantiateObject.completed += completed;
            instantiateObject.Load();
            return instantiateObject;
        }
    }
}