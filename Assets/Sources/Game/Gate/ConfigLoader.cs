using Luban;
using System;
using GameEngine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

using GooAsset;

namespace Game
{
    /// <summary>
    /// ConfigLoader会扫描所有的有ConfigAttribute标签的配置, 加载进来
    /// </summary>
    public static class ConfigLoader
    {
        /// <summary>
        /// 异步加载所有配置
        /// </summary>
        public static async UniTask LoadAsync()
        {
            List<Asset> configAssets = new();
            IEnumerable<Type> configTypes = GetConfigTableTypes();

            foreach (Type type in configTypes)
            {
                string fileName = type.Name[..^11].ToLower(); // 去除最后11个字符:ConfigTable  //"tb" + type.Name[..^11].ToLower(); // 去除最后11个字符:ConfigTable
                Asset asset = ResourceHandler.Instance.LoadAssetAsync<TextAsset>($"Assets/_Resources/Config/{fileName}.bytes", obj =>
                {
                    IConfigRegister table = Activator.CreateInstance(type, new ByteBuf((obj as TextAsset)?.bytes)) as IConfigRegister;
                    table?.Register();
                });
                configAssets.Add(asset);
            }

            await UniTask.WaitUntil(() =>
            {
                foreach (Asset asset in configAssets)
                {
                    if (!asset.IsDone)
                    {
                        return false;
                    }
                }

                return true;
            });

            foreach (Asset asset in configAssets)
            {
                ResourceHandler.Instance.UnloadAsset(asset);
            }
        }

        /// <summary>
        /// 重载配置表
        /// </summary>
        public static async UniTask ReloadAsync()
        {
            await LoadAsync();

            // 重载本地化文字
            // Localization.Reload();
        }

        /// <summary>
        /// 获取所有配置表的类
        /// </summary>
        static IEnumerable<Type> GetConfigTableTypes()
        {
            List<Type> configTypes = new();

            Type[] types = typeof(ConfigSingleton<>).Assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                object[] attrs = type.GetCustomAttributes(typeof(ConfigAttribute), false);
                if (attrs.Length > 0)
                {
                    configTypes.Add(type);
                }
            }

            return configTypes.ToArray();
        }
    }
}
