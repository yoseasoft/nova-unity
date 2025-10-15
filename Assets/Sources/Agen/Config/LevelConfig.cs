using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    /// <summary>
    /// 关卡配置
    /// </summary>
    public sealed partial class LevelConfig : BeanBase
    {
        public LevelConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            dimensions = buf.ReadInt();
            rank = buf.ReadInt();
            difficulty = buf.ReadInt();
            rounds = buf.ReadInt();
            totalRoundCount = buf.ReadInt();
            carryingCoins = buf.ReadInt();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);carryingSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); carryingSymbols.Add(_e0);}}
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);carryingItems = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); carryingItems.Add(_e0);}}

            PostInit();
        }

        public static LevelConfig DeserializeLevelConfig(ByteBuf buf)
        {
            return new LevelConfig(buf);
        }

        /// <summary>
        /// 关卡标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 阶数
        /// </summary>
        public readonly int dimensions;

        /// <summary>
        /// 阶数级别
        /// </summary>
        public readonly int rank;

        /// <summary>
        /// 难度级别
        /// </summary>
        public readonly int difficulty;

        /// <summary>
        /// 引用轮次标识
        /// </summary>
        public readonly int rounds;

        /// <summary>
        /// 总轮次数
        /// </summary>
        public readonly int totalRoundCount;

        /// <summary>
        /// 初始携带硬币数
        /// </summary>
        public readonly int carryingCoins;

        /// <summary>
        /// 初始携带符号列表
        /// </summary>
        public readonly List<int> carryingSymbols;

        /// <summary>
        /// 初始携带物品列表
        /// </summary>
        public readonly List<int> carryingItems;

        public const int Id = -1308229690;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "dimensions:" + dimensions + ","
            + "rank:" + rank + ","
            + "difficulty:" + difficulty + ","
            + "rounds:" + rounds + ","
            + "totalRoundCount:" + totalRoundCount + ","
            + "carryingCoins:" + carryingCoins + ","
            + "carryingSymbols:" + StringUtil.CollectionToString(carryingSymbols) + ","
            + "carryingItems:" + StringUtil.CollectionToString(carryingItems) + ","
            + "}";
        }

        partial void PostInit();
    }
}