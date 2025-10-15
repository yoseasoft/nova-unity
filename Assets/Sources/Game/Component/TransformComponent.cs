/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-17
/// 功能描述：
/// </summary>

namespace Game
{
    /// <summary>
    /// 变换组件对象类
    /// </summary>
    public class TransformComponent : GameEngine.CComponent
    {
        public UnityEngine.Vector3 position;
        public UnityEngine.Vector3 rotation;
        public UnityEngine.Vector3 scale;
    }
}
