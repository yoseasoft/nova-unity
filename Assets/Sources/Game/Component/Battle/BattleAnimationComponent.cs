/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-18
/// 功能描述：
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Battle场景动画组件
    /// </summary>
    public class BattleAnimationComponent : GameEngine.CComponent
    {
        public GameObject camera;
        public GameObject light;

        public GameObject[] grids;

        public IDictionary<int, GameObject> models;
    }
}
