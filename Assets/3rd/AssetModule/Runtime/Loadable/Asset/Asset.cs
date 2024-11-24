using System;
using System.Collections;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace AssetModule
{
    /// <summary>
    /// 资源对象基类
    /// </summary>
    public class Asset : Loadable, IEnumerator
    {
        /// <summary>
        /// 加载完成对象
        /// </summary>
        public Object result;

        /// <summary>
        /// 加载类型
        /// </summary>
        public Type type;

        /// <summary>
        /// 加载完成回调
        /// </summary>
        public Action<Asset> completed;

        /// <summary>
        /// 提供给await使用
        /// </summary>
        public Task<Asset> Task
        {
            get
            {
                TaskCompletionSource<Asset> tcs = new();
                completed += _ => tcs.SetResult(this);
                return tcs.Task;
            }
        }

        /// <summary>
        /// 加载完成时由子类调用
        /// </summary>
        protected void OnAssetLoaded(Object obj)
        {
            result = obj;
            Finish(result == null ? "加载出的object为空??" : null);
        }

        protected override void OnComplete()
        {
            if (completed == null)
                return;

            Action<Asset> func = completed;
            completed = null;

            try
            {
                func(this);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        protected override void OnUnused()
        {
            completed = null;
        }

        protected override void OnUnload()
        {
            AssetHandler.RemoveCache(address);
        }

        #region IEnumerator

        public object Current => null;

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
        }

        #endregion
    }
}