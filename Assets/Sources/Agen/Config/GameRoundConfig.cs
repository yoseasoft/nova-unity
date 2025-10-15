using Luban;

namespace Game.Config
{
    /// <summary>
    /// 轮次配置
    /// </summary>
    public sealed partial class GameRoundConfig : BeanBase
    {
        public GameRoundConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            roundIndex = buf.ReadInt();
            nextRoundId = buf.ReadInt();
            infinited = buf.ReadBool();
            roundCount = buf.ReadInt();
            requireCoins = buf.ReadInt();
            rewards = buf.ReadInt();
            rewardPoolId = buf.ReadInt();
            emergencyRuleId = buf.ReadInt();
            roundBeganText = buf.ReadString();
            roundEndedText = buf.ReadString();

            PostInit();
        }

        public static GameRoundConfig DeserializeGameRoundConfig(ByteBuf buf)
        {
            return new GameRoundConfig(buf);
        }

        /// <summary>
        /// 轮次标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 轮次索引
        /// </summary>
        public readonly int roundIndex;

        /// <summary>
        /// 下一轮次标识
        /// </summary>
        public readonly int nextRoundId;

        /// <summary>
        /// 无限模式
        /// </summary>
        public readonly bool infinited;

        /// <summary>
        /// 轮次回合数
        /// </summary>
        public readonly int roundCount;

        /// <summary>
        /// 完成所需硬币
        /// </summary>
        public readonly int requireCoins;

        /// <summary>
        /// 奖励列表
        /// </summary>
        public readonly int rewards;

        /// <summary>
        /// 奖池引用标识
        /// </summary>
        public readonly int rewardPoolId;

        /// <summary>
        /// 奖池引用标识
        /// </summary>
        public SymbolQualityRewardPoolConfig RewardPoolConfig => SymbolQualityRewardPoolConfigTable.Get(rewardPoolId);

        /// <summary>
        /// 突发事件引用标识
        /// </summary>
        public readonly int emergencyRuleId;

        /// <summary>
        /// 突发事件引用标识
        /// </summary>
        public EmergencyRuleConfig EmergencyRuleConfig => EmergencyRuleConfigTable.Get(emergencyRuleId);

        /// <summary>
        /// 轮次开始文本
        /// </summary>
        public readonly string roundBeganText;

        /// <summary>
        /// 轮次结束文本
        /// </summary>
        public readonly string roundEndedText;

        public const int Id = 1031325438;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "roundIndex:" + roundIndex + ","
            + "nextRoundId:" + nextRoundId + ","
            + "infinited:" + infinited + ","
            + "roundCount:" + roundCount + ","
            + "requireCoins:" + requireCoins + ","
            + "rewards:" + rewards + ","
            + "rewardPoolId:" + rewardPoolId + ","
            + "emergencyRuleId:" + emergencyRuleId + ","
            + "roundBeganText:" + roundBeganText + ","
            + "roundEndedText:" + roundEndedText + ","
            + "}";
        }

        partial void PostInit();
    }
}