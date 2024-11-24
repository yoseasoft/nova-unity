
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;

namespace Game.Config
{
    public sealed partial class MainLevelConfig : BeanBase
    {
        public MainLevelConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            name = buf.ReadString();
            description = buf.ReadString();
            lv = buf.ReadInt();
            gridFile = buf.ReadString();
            houseFile = buf.ReadString();
            bgSoundId = buf.ReadInt();
            iconId = buf.ReadInt();
            levelType = (LevelType)buf.ReadInt();
            consumeEnergy = buf.ReadInt();
            unlockCondition = buf.ReadInt();
            raidFlag = buf.ReadBool();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);spawnMonsterRounds = new System.Collections.Generic.List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); spawnMonsterRounds.Add(_e0);}}
            settleType = (SettleType)buf.ReadInt();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);passProgressRewards = new System.Collections.Generic.List<CondReward>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { CondReward _e0;  _e0 = CondReward.DeserializeCondReward(buf); passProgressRewards.Add(_e0);}}

            PostInit();
        }

        public static MainLevelConfig DeserializeMainLevelConfig(ByteBuf buf)
        {
            return new MainLevelConfig(buf);
        }

        /// <summary>
        /// 关卡ID
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 关卡名称
        /// </summary>
        public readonly string name;

        /// <summary>
        /// 关卡描述
        /// </summary>
        public readonly string description;

        /// <summary>
        /// 等级
        /// </summary>
        public readonly int lv;

        /// <summary>
        /// 网格文件名称
        /// </summary>
        public readonly string gridFile;

        /// <summary>
        /// 建筑文件名称
        /// </summary>
        public readonly string houseFile;

        /// <summary>
        /// 背景音乐ID
        /// </summary>
        public readonly int bgSoundId;

        /// <summary>
        /// 图标资源ID
        /// </summary>
        public readonly int iconId;

        /// <summary>
        /// 关卡类型
        /// </summary>
        public readonly LevelType levelType;

        /// <summary>
        /// 消耗体力
        /// </summary>
        public readonly int consumeEnergy;

        /// <summary>
        /// 解锁条件
        /// </summary>
        public readonly int unlockCondition;

        /// <summary>
        /// 可否扫荡
        /// </summary>
        public readonly bool raidFlag;

        /// <summary>
        /// 刷怪波次列表
        /// </summary>
        public readonly System.Collections.Generic.List<int> spawnMonsterRounds;

        /// <summary>
        /// 结算类型
        /// </summary>
        public readonly SettleType settleType;

        /// <summary>
        /// 通关进度奖励
        /// </summary>
        public readonly System.Collections.Generic.List<CondReward> passProgressRewards;

        public const int Id = 1881216077;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "name:" + name + ","
            + "description:" + description + ","
            + "lv:" + lv + ","
            + "gridFile:" + gridFile + ","
            + "houseFile:" + houseFile + ","
            + "bgSoundId:" + bgSoundId + ","
            + "iconId:" + iconId + ","
            + "levelType:" + levelType + ","
            + "consumeEnergy:" + consumeEnergy + ","
            + "unlockCondition:" + unlockCondition + ","
            + "raidFlag:" + raidFlag + ","
            + "spawnMonsterRounds:" + Luban.StringUtil.CollectionToString(spawnMonsterRounds) + ","
            + "settleType:" + settleType + ","
            + "passProgressRewards:" + Luban.StringUtil.CollectionToString(passProgressRewards) + ","
            + "}";
        }

        partial void PostInit();
    }
}