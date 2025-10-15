/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-17
/// 功能描述：
/// </summary>

namespace Game
{
    public enum BattleObjectType : byte
    {
        Unknown,
        Soldier,
        Npc,
        Monster,
    }

    /// <summary>
    /// 身份组件对象类
    /// </summary>
    public class IdentityComponent : GameEngine.CComponent
    {
        public int objectId;
        public BattleObjectType objectType;
    }
}
