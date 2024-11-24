using UnityEngine;

namespace AssetModule
{
    /// <summary>
    /// 打印调试信息
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Info的打印开关
        /// </summary>
        public static bool loggable = true;

        public static void Info(string format, params object[] args)
        {
#if UNITY_EDITOR
            if (loggable)
                Debug.LogFormat(format, args);
#endif
        }

        public static void Warning(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }
    }
}