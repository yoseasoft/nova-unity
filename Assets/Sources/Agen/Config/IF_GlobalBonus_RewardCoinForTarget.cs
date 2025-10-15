using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    public sealed partial class IF_GlobalBonus_RewardCoinForTarget : ItemEffect
    {
        public IF_GlobalBonus_RewardCoinForTarget(ByteBuf buf) : base(buf)
        {
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            fixedCoins = buf.ReadInt();
            fixedMultipler = buf.ReadInt();

            PostInit();
        }

        public static IF_GlobalBonus_RewardCoinForTarget DeserializeIF_GlobalBonus_RewardCoinForTarget(ByteBuf buf)
        {
            return new IF_GlobalBonus_RewardCoinForTarget(buf);
        }

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 固定金币
        /// </summary>
        public readonly int fixedCoins;

        /// <summary>
        /// 固定倍率
        /// </summary>
        public readonly int fixedMultipler;

        public const int Id = -1964780715;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "fixedCoins:" + fixedCoins + ","
            + "fixedMultipler:" + fixedMultipler + ","
            + "}";
        }

        partial void PostInit();
    }
}