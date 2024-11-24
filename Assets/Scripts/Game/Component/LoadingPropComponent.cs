using GameEngine;

namespace Game
{
    /// <summary>
    /// Loading场景成员属性组件
    /// </summary>
    public class LoadingPropComponent : CComponent
    {
        // 场景加载信息
        public AssetModule.Scene loadingScene;

        public float loadingProgress = 0;
    }
}
