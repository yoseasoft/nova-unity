using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AssetModule
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// 不支持平台提示
        /// </summary>
        public const string UnsupportedPlatform = "Unsupported";

        /// <summary>
        /// 每个数据单位对应的字节数
        /// </summary>
        static readonly long[] ByteUnits =
        {
            1073741824, 1048576, 1024, 1
        };

        /// <summary>
        /// 每个数据单位的名字
        /// </summary>
        static readonly string[] ByteUnitsNames =
        {
            "GB", "MB", "KB", "B"
        };

        /// <summary>
        /// 当前平台类型
        /// </summary>
        public static RuntimePlatform CurrentPlatform
        {
            get
            {
#if UNITY_EDITOR
                if (!UnityEngine.Application.isEditor)
                {
#endif
                    return UnityEngine.Application.platform switch
                    {
                        RuntimePlatform.WindowsPlayer => RuntimePlatform.WindowsPlayer,
                        RuntimePlatform.OSXPlayer => RuntimePlatform.OSXPlayer,
                        RuntimePlatform.Android => RuntimePlatform.Android,
                        RuntimePlatform.IPhonePlayer => RuntimePlatform.IPhonePlayer,
                        RuntimePlatform.WebGLPlayer => RuntimePlatform.WebGLPlayer,
                        _ => throw new Exception("Unsupported Platform")
                    };
#if UNITY_EDITOR
                }
                else
                {
                    return UnityEditor.EditorUserBuildSettings.activeBuildTarget switch
                    {
                        UnityEditor.BuildTarget.StandaloneWindows or UnityEditor.BuildTarget.StandaloneWindows64 => RuntimePlatform.WindowsPlayer,
                        UnityEditor.BuildTarget.StandaloneOSX => RuntimePlatform.OSXPlayer,
                        UnityEditor.BuildTarget.Android => RuntimePlatform.Android,
                        UnityEditor.BuildTarget.iOS => RuntimePlatform.IPhonePlayer,
                        UnityEditor.BuildTarget.WebGL => RuntimePlatform.WebGLPlayer,
                        _ => throw new Exception("Unsupported Platform")
                    };
                }
#endif
            }
        }

        /// <summary>
        /// 当前平台名字
        /// </summary>
        public static string CurrentPlatformName
        {
            get
            {
                return CurrentPlatform switch
                {
                    RuntimePlatform.WindowsPlayer => "Windows",
                    RuntimePlatform.OSXPlayer => "MacOS",
                    RuntimePlatform.Android => "Android",
                    RuntimePlatform.IPhonePlayer => "iOS",
                    RuntimePlatform.WebGLPlayer => "WebGL",
                    _ => UnsupportedPlatform
                };
            }
        }

        /// <summary>
        /// 格式化成数据大小形式(GB, MB, KB, B)显示
        /// </summary>
        public static string FormatBytes(long bytes)
        {
            string size = "0 B";
            if (bytes == 0)
                return size;

            for (var i = 0; i < ByteUnits.Length; i++)
            {
                long unit = ByteUnits[i];
                if (bytes >= unit)
                {
                    size = $"{(double)bytes / unit:0.00} {ByteUnitsNames[i]}";
                    break;
                }
            }

            return size;
        }

        /// <summary>
        /// 保证Hash值格式正确
        /// ToString("X2")为C#中的字符串格式控制符(X为十六进制, 2为每次都是两位数)
        /// </summary>
        static string ToHash(IEnumerable<byte> data)
        {
            var sb = new StringBuilder();
            foreach (var t in data)
                sb.Append(t.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// 计算文件流的Hash值
        /// </summary>
        public static string ComputeHash(FileStream stream)
        {
            return ToHash(MD5.Create().ComputeHash(stream));
        }

        /// <summary>
        /// 计算指定文件的Hash值
        /// </summary>
        public static string ComputeHash(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            using FileStream stream = File.OpenRead(filePath);
            return ComputeHash(stream);
        }

        /// <summary>
        /// 计算指定字符串的Hash值
        /// </summary>
        public static string ComputeStringHash(string content)
        {
            return ToHash(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(content)));
        }
    }
}