/// -------------------------------------------------------------------------------
/// AppEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System;
using System.IO;
using AssetModule;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace AppEngine
{
    /// <summary>
    /// 应用程序启动流程管理类，用于对应用程序启动流程及脚本加载提供接口函数
    /// </summary>
    public static class AppStart
    {
        /// <summary>
        /// Api库
        /// </summary>
        // public const string ApiDllName = "Api";

        /// <summary>
        /// 配置、协议库名
        /// </summary>
        public const string AgenDllName = "Agen";

        /// <summary>
        /// Nova运行库名
        /// </summary>
        public const string NovaDllName = "Nova";

        /// <summary>
        /// 基础运行库名
        /// </summary>
        public const string BasicDllName = "Basic";

        /// <summary>
        /// 游戏运行库名
        /// </summary>
        public const string GameDllName = "Game";

        /// <summary>
        /// 游戏运行库Hotfix库名
        /// </summary>
        public const string GameHotfixDllName = "GameHotfix";

        /// <summary>
        /// Dll资源目录
        /// </summary>
        public const string CodeAssetPath = "Assets/_Resources/Code";

        public static readonly List<string> allAssemblyNameList = new() { NovaDllName, BasicDllName, "Gimp", AgenDllName, GameDllName, GameHotfixDllName };

        /// <summary>
        /// 需要加载的程序集名字列表
        /// </summary>
        public static readonly List<string> assemblyNameList = new() { NovaDllName, BasicDllName, "Gimp", AgenDllName, GameDllName, GameHotfixDllName };

        /// <summary>
        /// 名字对应的程序集
        /// </summary>
        static readonly Dictionary<string, Assembly> s_name2Assembly = new();

        /// <summary>
        /// 主控制器实例
        /// </summary>
        private static AppController appController;

        /// <summary>
        /// 获取已加载的程序集
        /// </summary>
        public static Assembly GetLoadedAssembly(string name)
        {
            return s_name2Assembly.GetValueOrDefault(name);
        }

        [RuntimeInitializeOnLoadMethod]
        static void Start()
        {
            // Main场景才运行
            if (SceneManager.GetActiveScene().name.ToLower() != "main")
            {
                return;
            }

            StartAsync().Forget();
        }

        /// <summary>
        /// 运行启动流程
        /// </summary>
        static async UniTaskVoid StartAsync()
        {
            bool success = await LoadChannelSettingsAsync();
            if (!success)
            {
                return;
            }

            await AssetManagement.InitAsync().Task;

            await LoadAssembliesAsync();

            StartEngine();
        }

        /// <summary>
        /// 异步加载渠道设置
        /// </summary>
        static async UniTask<bool> LoadChannelSettingsAsync()
        {
            string localProtocol;
            RuntimePlatform pf = Application.platform;
            if (pf != RuntimePlatform.OSXEditor && pf != RuntimePlatform.OSXPlayer && pf != RuntimePlatform.IPhonePlayer)
                localProtocol = pf is RuntimePlatform.WindowsEditor or RuntimePlatform.WindowsPlayer ? "file:///" : string.Empty;
            else
                localProtocol = "file://";

            string channelSettingsPath = localProtocol + Path.Combine(Application.streamingAssetsPath, "ChannelSettings");
            UnityWebRequest request = UnityWebRequest.Get(channelSettingsPath);
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"获取渠道设置失败，错误原因：{request.error}");
                return false;
            }

            AppChannelSettings settings;
            try
            {
                settings = JsonUtility.FromJson<AppChannelSettings>(request.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.LogError("解析渠道设置失败:" + e);
                return false;
            }

            AppChannelSettings.Instance = settings;
            return true;
        }

        /// <summary>
        /// 异步加载程序集
        /// </summary>
        static async UniTask LoadAssembliesAsync()
        {
            await LoadMetadataAsync();

            if (Application.isEditor && !AppSettings.Instance.enableDll)
            {
                LoadAssembliesFromCurrentDomain();
                return;
            }

            await LoadAssembliesFromAssetsAsync();
        }

        static IDictionary<string, string> LoadEnvironmentVariables()
        {
            IDictionary<string, string> vars = new Dictionary<string, string>();

            AppSettings settings = AppSettings.Instance;

            // SetProperty
            vars.Add(nameof(settings.editorMode), settings.editorMode.ToString());
            vars.Add(nameof(settings.debugMode), settings.debugMode.ToString());
            vars.Add(nameof(settings.debugLevel), settings.debugLevel.ToString());
            vars.Add(nameof(settings.cryptMode), settings.cryptMode.ToString());
            vars.Add(nameof(settings.applicationName), settings.applicationName.ToString());
            vars.Add(nameof(settings.applicationCode), settings.applicationCode.ToString());

            // SetVariable(应用设置)
            vars.Add(nameof(settings.updateMode), settings.updateMode.ToString());
            vars.Add(nameof(settings.frameRate), settings.frameRate.ToString());
            vars.Add(nameof(settings.animationRate), settings.animationRate.ToString());
            vars.Add(nameof(settings.designResolutionWidth), settings.designResolutionWidth.ToString());
            vars.Add(nameof(settings.designResolutionHeight), settings.designResolutionHeight.ToString());
            foreach (ExtraParam extraParam in settings.extraParams)
            {
                vars.Add(extraParam.key, extraParam.value);
            }

            // SetVariable(渠道设置)
            AppChannelSettings channelSettings = AppChannelSettings.Instance;
            vars.Add(nameof(channelSettings.platformId), channelSettings.platformId.ToString());
            vars.Add(nameof(channelSettings.channelId), channelSettings.channelId.ToString());
            vars.Add(nameof(channelSettings.channelName), channelSettings.channelName);
            foreach (ExtraParam extraParam in channelSettings.extraParams)
            {
                vars.Add(extraParam.key, extraParam.value);
            }

            return vars;
        }

        /// <summary>
        /// 添加基础运行控制组件
        /// </summary>
        static void StartEngine()
        {
            Type type = s_name2Assembly[BasicDllName]?.GetType(AppConst.AppControllerClassName);
            if (type is null)
            {
                Debug.LogError($"加载Type:{AppConst.AppControllerClassName}失败");
                return;
            }

            UnityEngine.GameObject gameObject = new UnityEngine.GameObject { name = AppConst.AppControllerGameObjectName };
            appController = gameObject.AddComponent<AppController>();

            // 确保该脚本不会被移除
            UnityEngine.Object.DontDestroyOnLoad(appController);

            IDictionary<string, string> vars = LoadEnvironmentVariables();

            type.GetMethod("Start", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { appController, vars });
        }

        static void ReloadEngine()
        {
            Type type = s_name2Assembly[BasicDllName]?.GetType(AppConst.AppControllerClassName);
            if (type is null)
            {
                Debug.LogError($"加载Type:{AppConst.AppControllerClassName}失败");
                return;
            }

            type.GetMethod("Reload", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { appController });
        }

        /// <summary>
        /// 异步补充元数据
        /// </summary>
        static async UniTask LoadMetadataAsync()
        {
            if (Application.isEditor)
            {
                return;
            }

            List<string> aotDllNames = new() { "System.Core.dll", "System.dll", "mscorlib.dll", "UnityEngine.CoreModule.dll" };
            var dllAssets = new Asset[aotDllNames.Count];
            for (int i = 0; i < aotDllNames.Count; i++)
            {
                var asset = AssetManagement.LoadAssetAsync($"Assets/_Resources/Aot/{aotDllNames[i]}.bytes", typeof(TextAsset));
                dllAssets[i] = asset;
            }
            await UniTask.WaitUntil(() => { return dllAssets.All(asset => asset.IsDone); });

            foreach (var asset in dllAssets)
            {
                byte[] assBytes = (asset.result as TextAsset)?.bytes;
                HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(assBytes, HybridCLR.HomologousImageMode.SuperSet);
                asset.Release();
            }
            Debug.Log($"补元数加载完成  ======  aot dll load  ---- count:{dllAssets.Length}");
        }

        /// <summary>
        /// 从当前运行环境加载Dll
        /// </summary>
        static void LoadAssembliesFromCurrentDomain()
        {
            foreach (string assemblyName in assemblyNameList)
            {
                s_name2Assembly.Add(assemblyName, Assembly.Load(assemblyName));
            }
        }

        /// <summary>
        /// 从资源中加载Dll
        /// </summary>
        static async UniTask LoadAssembliesFromAssetsAsync()
        {
            Dictionary<string, Asset> name2DllAssets = new();
            foreach (string dllName in assemblyNameList)
            {
                string fileName = $"{dllName}.dll";
                Asset asset = AssetManagement.LoadAssetAsync($"{CodeAssetPath}/{fileName}.bytes", typeof(TextAsset));
                name2DllAssets.Add(fileName, asset);

                fileName = $"{dllName}.pdb";
                asset = AssetManagement.LoadAssetAsync($"{CodeAssetPath}/{fileName}.bytes", typeof(TextAsset));
                name2DllAssets.Add(fileName, asset);
            }

            Asset[] dllAssets = name2DllAssets.Values.ToArray();
            await UniTask.WaitUntil(() => { return dllAssets.All(asset => asset.IsDone); });

            foreach (string dllName in assemblyNameList)
            {
#if UNITY_EDITOR
                // 因编辑器工具需要引用, 编辑器下跳过加载配置表库, 使用Unity默认加载
                if (dllName == AgenDllName)
                {
                    s_name2Assembly.Add(dllName, Assembly.Load(dllName));
                    continue;
                }
#endif

                byte[] assBytes = AESEncryptProvider.Decrypt((name2DllAssets[$"{dllName}.dll"].result as TextAsset).bytes, Uqs3, Dnw4);
                byte[] pdbBytes = AESEncryptProvider.Decrypt((name2DllAssets[$"{dllName}.pdb"].result as TextAsset).bytes, Uqs3, Dnw4);
                s_name2Assembly.Add(dllName, Assembly.Load(assBytes, pdbBytes));
            }

            foreach (Asset asset in dllAssets)
            {
                asset.Release();
            }
        }

        /// <summary>
        /// 重载所有程序集并进行通知
        /// </summary>
        public async static UniTaskVoid ReloadAssembliesAsync()
        {
            await ReloadAssembliesFromAssetsAsync();

            ReloadEngine();
        }

        /// <summary>
        /// 重载程序集
        /// </summary>
        public async static UniTask ReloadAssembliesFromAssetsAsync()
        {
            Asset dllAsset = AssetManagement.LoadAssetAsync($"{CodeAssetPath}/GameHotfix.dll.bytes", typeof(TextAsset));
            Asset pdbAsset = AssetManagement.LoadAssetAsync($"{CodeAssetPath}/GameHotfix.pdb.bytes", typeof(TextAsset));
            await dllAsset;
            await pdbAsset;
            byte[] dllBytes = AESEncryptProvider.Decrypt((dllAsset.result as TextAsset).bytes, Uqs3, Dnw4);
            byte[] pdbBytes = AESEncryptProvider.Decrypt((pdbAsset.result as TextAsset).bytes, Uqs3, Dnw4);
            s_name2Assembly[GameHotfixDllName] = Assembly.Load(dllBytes, pdbBytes);
        }

        const string Uqs3 = "TmKYR6VVLBjsdfXh4fEpRc9yG6Z73sqU";
        const string Dnw4 = "Nzcfvd2Nc0Tx4Wnd";

#if UNITY_EDITOR
        public static (string, string) GetKey()
        {
            return (Uqs3, Dnw4);
        }
#endif
    }
}