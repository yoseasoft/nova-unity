namespace AssetModule
{
    /// <summary>
    /// 引用计数
    /// </summary>
    public class Reference
    {
        /// <summary>
        /// 引用数量
        /// </summary>
        int _count;

        /// <summary>
        /// 引用数量
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 增加引用
        /// </summary>
        internal void Increase()
        {
            _count++;
        }

        /// <summary>
        /// 减少引用
        /// </summary>
        internal void Decrease()
        {
            _count--;
        }

        /// <summary>
        /// 重置引用
        /// </summary>
        internal void Reset()
        {
            _count = 0;
        }

        /// <summary>
        /// 是否无引用
        /// </summary>
        public bool IsUnused => _count <= 0;
    }
}