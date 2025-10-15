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
    /// <summary>
    /// Loading场景资源组件
    /// </summary>
    public class LoadingResComponent : GameEngine.CComponent
    {
        public struct LoadingAssetInfo
        {
            public string name;
            public string url;
            public GooAsset.Scene scene;
            public bool loaded;
        }

        public IList<LoadingAssetInfo> assets;

        public bool loadable;
    }
}
