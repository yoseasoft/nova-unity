/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-18
/// 功能描述：
/// </summary>

namespace Game
{
    /// <summary>
    /// Battle场景地图组件
    /// </summary>
    public class BattleMapComponent : GameEngine.CComponent
    {
        public int[] grids;
        public int grid_count;

        public float width_size;
        public float height_size;
        public byte width_count;
        public byte height_count;
    }
}
