using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 上下文加载器管理类
    /// </summary>
    public static class ContextLoader
    {
        public static void Load()
        {
            LoadApplicationConfigures();
            // LoadBeanConfigures();
        }

        public static void Unload()
        {
        }

        public static void Reload()
        {
        }

        private static void LoadApplicationConfigures()
        {
            GameEngine.ApplicationContext.LoadApplicationConfigure(@"application", (path, ms) =>
            {
                string filePath = NovaEngine.Environment.GetSystemPath("BEAN_PROFILE_PATH"); // , $"{path}.xml");
                UnityEngine.TextAsset textAsset = GameEngine.ResourceHandler.Instance.LoadAsset($"{filePath}/{path}.xml", typeof(UnityEngine.TextAsset)) as UnityEngine.TextAsset;
                Debugger.Log("加载配置{%s}", $"{filePath}/{path}.xml");
                string text = textAsset.text;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
                ms.Write(buffer, 0, buffer.Length);
                ms.Seek(0, System.IO.SeekOrigin.Begin);

                GameEngine.ResourceHandler.Instance.UnloadAsset(textAsset);

                return true;
            });
        }

        private static void LoadBeanConfigures()
        {
            GameEngine.ApplicationContext.LoadBeanConfigure(@"bean", (path, ms) =>
            {
                string filePath = NovaEngine.Environment.GetSystemPath("BEAN_PROFILE_PATH"); // , $"{path}.xml");
                UnityEngine.TextAsset textAsset = GameEngine.ResourceHandler.Instance.LoadAsset($"{filePath}/{path}.xml", typeof(UnityEngine.TextAsset)) as UnityEngine.TextAsset;
                string text = textAsset.text;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
                ms.Write(buffer, 0, buffer.Length);
                ms.Seek(0, System.IO.SeekOrigin.Begin);

                GameEngine.ResourceHandler.Instance.UnloadAsset(textAsset);

                return true;
            });
        }
    }
}
