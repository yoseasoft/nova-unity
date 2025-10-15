using Luban;

namespace Game.Config
{
    /// <summary>
    /// 符号品质奖池配置
    /// </summary>
    public sealed partial class SymbolQualityRewardPoolConfig : BeanBase
    {
        public SymbolQualityRewardPoolConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            generalProbability = buf.ReadInt();
            superiorProbability = buf.ReadInt();
            rarityProbability = buf.ReadInt();
            extraordinaryProbability = buf.ReadInt();
            legendProbability = buf.ReadInt();

            PostInit();
        }

        public static SymbolQualityRewardPoolConfig DeserializeSymbolQualityRewardPoolConfig(ByteBuf buf)
        {
            return new SymbolQualityRewardPoolConfig(buf);
        }

        /// <summary>
        /// 奖池标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 普通概率
        /// </summary>
        public readonly int generalProbability;

        /// <summary>
        /// 精良概率
        /// </summary>
        public readonly int superiorProbability;

        /// <summary>
        /// 稀有概率
        /// </summary>
        public readonly int rarityProbability;

        /// <summary>
        /// 非凡概率
        /// </summary>
        public readonly int extraordinaryProbability;

        /// <summary>
        /// 传说概率
        /// </summary>
        public readonly int legendProbability;

        public const int Id = 332787028;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "generalProbability:" + generalProbability + ","
            + "superiorProbability:" + superiorProbability + ","
            + "rarityProbability:" + rarityProbability + ","
            + "extraordinaryProbability:" + extraordinaryProbability + ","
            + "legendProbability:" + legendProbability + ","
            + "}";
        }

        partial void PostInit();
    }
}