/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-18
/// 功能描述：
/// </summary>

using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// Battle场景加载组件
    /// </summary>
    public class BattleLoadComponent : GameEngine.CComponent
    {
        public int uid_generator;

        public Soldier player;
        public IList<Monster> monsters;
    }
}
