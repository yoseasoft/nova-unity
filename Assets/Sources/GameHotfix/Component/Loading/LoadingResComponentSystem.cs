/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-16
/// 功能描述：
/// </summary>

using System.Collections.Generic;

namespace Game
{
    public struct LoadingResourceProgressInfo
    {
        public int index;
        public string name;
        public float progress;
        public bool final;
    }

    /// <summary>
    /// Loading场景资源组件逻辑处理类
    /// </summary>
    public static class LoadingResComponentSystem
    {
        [OnAwake]
        private static void Awake(this LoadingResComponent self)
        {
            self.assets = new List<LoadingResComponent.LoadingAssetInfo>();
            self.loadable = false;
        }

        [OnUpdate]
        private static void Update(this LoadingResComponent self)
        {
            if (!self.loadable) return;

            for (int n = 0; n < self.assets.Count; ++n)
            {
                if (self.assets[n].loaded)
                {
                    // 加载完成
                    continue;
                }
                else if (null != self.assets[n].scene)
                {
                    float progress = self.assets[n].scene.Progress * (100f / self.assets.Count) + n * (100f / self.assets.Count);
                    //Debugger.Log("目标场景‘{%s}’加载进度为：{%d}%，当前所有场景总加载进度为：{%d}%！", self.assets[n].name, self.assets[n].scene.Progress, progress);

                    LoadingResourceProgressInfo progress_info = new LoadingResourceProgressInfo { index = n, name = self.assets[n].name, progress = progress, final = false };
                    GameEngine.GameApi.Send(progress_info);

                    if (self.assets[n].scene.IsDone)
                    {
                        Debugger.Log("目标场景‘{%s}’加载完成！", self.assets[n].name);

                        LoadingResComponent.LoadingAssetInfo info = self.assets[n];
                        //info.scene = null;
                        info.loaded = true;

                        self.assets[n] = info;
                    }

                    return;
                }
                else
                {
                    LoadingResComponent.LoadingAssetInfo info = self.assets[n];
                    info.scene = NE.SceneHandler.LoadSceneAsset(info.name, info.url);
                    self.assets[n] = info;

                    return;
                }
            }

            Debugger.Log("场景全部预加载完成！");

            LoadingResourceProgressInfo final_info = new LoadingResourceProgressInfo { final = true };
            GameEngine.GameApi.Send(final_info);

            self.loadable = false;
        }

        [OnDestroy]
        public static void Destroy(this LoadingResComponent self)
        {
            for (int n = 0; n < self.assets.Count; ++n)
            {
                if (self.assets[n].loaded)
                {
                    LoadingResComponent.LoadingAssetInfo info = self.assets[n];
                    NE.SceneHandler.UnloadSceneAsset(info.name);
                    //GameEngine.ResourceHandler.UnloadScene(info.scene);
                    info.scene = null;
                    info.loaded = false;
                    self.assets[n] = info;

                    Debugger.Info("卸载预载入资源‘{%s}’成功！", info.name);
                }
            }
        }

        [GameEngine.MessageListenerBindingOfTarget(typeof(Proto.PingResp))]
        private static void OnResourceInfoMessage(this LoadingResComponent self, Proto.PingResp message)
        {
            Debugger.Warn("Loading场景接收消息‘{%t}’，标签为{%d}，内容为：[{2},{3},{4}]。", message, Proto.ProtoOpcode.PingResp, message.Str, message.SecTime, message.MilliTime);

            self.assets.Clear();
            self.assets.Add(new LoadingResComponent.LoadingAssetInfo { name = "101", url = "Assets/_Resources/Scene/Level/101.unity", scene = null, loaded = false });
            self.assets.Add(new LoadingResComponent.LoadingAssetInfo { name = "102", url = "Assets/_Resources/Scene/Level/102.unity", scene = null, loaded = false });
            self.assets.Add(new LoadingResComponent.LoadingAssetInfo { name = "103", url = "Assets/_Resources/Scene/Level/103.unity", scene = null, loaded = false });
            self.assets.Add(new LoadingResComponent.LoadingAssetInfo { name = "104", url = "Assets/_Resources/Scene/Level/104.unity", scene = null, loaded = false });
            self.assets.Add(new LoadingResComponent.LoadingAssetInfo { name = "105", url = "Assets/_Resources/Scene/Level/105.unity", scene = null, loaded = false });

            self.loadable = true;
        }
    }
}

