using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    /// <summary>
    /// 符号配置
    /// </summary>
    public sealed partial class SymbolConfig : BeanBase
    {
        public SymbolConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            name = buf.ReadString();
            qualityType = (QualityType)buf.ReadInt();
            universal = buf.ReadBool();
            unsupportedGrab = buf.ReadBool();
            locked = buf.ReadBool();
            deprecated = buf.ReadBool();
            unrepeatableRemoved = buf.ReadBool();
            growable = buf.ReadBool();
            maxGrowValue = buf.ReadInt();
            usedStatType = (SymbolUsingOperationType)buf.ReadInt();
            usedCount = buf.ReadInt();
            removeOnUsingCompleted = buf.ReadBool();
            initCoin = buf.ReadInt();
            roundActionType = buf.ReadInt();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);roundActionParams = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); roundActionParams.Add(_e0);}}
            positiveText = buf.ReadString();
            negativeText = buf.ReadString();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);callFuncs = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); callFuncs.Add(_e0);}}
            thumbnail = buf.ReadInt();
            icon = buf.ReadInt();
            weight = buf.ReadInt();
            iconPath = buf.ReadString();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);combineIdList = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); combineIdList.Add(_e0);}}
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);effectIdList = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); effectIdList.Add(_e0);}}

            PostInit();
        }

        public static SymbolConfig DeserializeSymbolConfig(ByteBuf buf)
        {
            return new SymbolConfig(buf);
        }

        /// <summary>
        /// 符号标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 符号名称
        /// </summary>
        public readonly string name;

        /// <summary>
        /// 品质类型
        /// </summary>
        public readonly QualityType qualityType;

        /// <summary>
        /// 通用符号
        /// </summary>
        public readonly bool universal;

        /// <summary>
        /// 不支持抓取
        /// </summary>
        public readonly bool unsupportedGrab;

        /// <summary>
        /// 锁定状态
        /// </summary>
        public readonly bool locked;

        /// <summary>
        /// 反对状态
        /// </summary>
        public readonly bool deprecated;

        /// <summary>
        /// 不可重复消除
        /// </summary>
        public readonly bool unrepeatableRemoved;

        /// <summary>
        /// 可成长标识
        /// </summary>
        public readonly bool growable;

        /// <summary>
        /// 最大成长值
        /// </summary>
        public readonly int maxGrowValue;

        /// <summary>
        /// 消耗统计类型
        /// </summary>
        public readonly SymbolUsingOperationType usedStatType;

        /// <summary>
        /// 消耗次数
        /// </summary>
        public readonly int usedCount;

        /// <summary>
        /// 消耗完成移除状态
        /// </summary>
        public readonly bool removeOnUsingCompleted;

        /// <summary>
        /// 初始硬币
        /// </summary>
        public readonly int initCoin;

        /// <summary>
        /// 回合前行为类型
        /// </summary>
        public readonly int roundActionType;

        /// <summary>
        /// 回合前行为参数
        /// </summary>
        public readonly List<int> roundActionParams;

        /// <summary>
        /// 正面信息描述
        /// </summary>
        public readonly string positiveText;

        /// <summary>
        /// 负面信息描述
        /// </summary>
        public readonly string negativeText;

        /// <summary>
        /// 触发功能列表
        /// </summary>
        public readonly List<int> callFuncs;

        /// <summary>
        /// 符号缩略图
        /// </summary>
        public readonly int thumbnail;

        /// <summary>
        /// 符号图标
        /// </summary>
        public readonly int icon;

        /// <summary>
        /// 权重
        /// </summary>
        public readonly int weight;

        /// <summary>
        /// 图标路径
        /// </summary>
        public readonly string iconPath;

        /// <summary>
        /// 合成列表
        /// </summary>
        public readonly List<int> combineIdList;

        /// <summary>
        /// 效果列表
        /// </summary>
        public readonly List<int> effectIdList;

        public const int Id = 2022745882;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "name:" + name + ","
            + "qualityType:" + qualityType + ","
            + "universal:" + universal + ","
            + "unsupportedGrab:" + unsupportedGrab + ","
            + "locked:" + locked + ","
            + "deprecated:" + deprecated + ","
            + "unrepeatableRemoved:" + unrepeatableRemoved + ","
            + "growable:" + growable + ","
            + "maxGrowValue:" + maxGrowValue + ","
            + "usedStatType:" + usedStatType + ","
            + "usedCount:" + usedCount + ","
            + "removeOnUsingCompleted:" + removeOnUsingCompleted + ","
            + "initCoin:" + initCoin + ","
            + "roundActionType:" + roundActionType + ","
            + "roundActionParams:" + StringUtil.CollectionToString(roundActionParams) + ","
            + "positiveText:" + positiveText + ","
            + "negativeText:" + negativeText + ","
            + "callFuncs:" + StringUtil.CollectionToString(callFuncs) + ","
            + "thumbnail:" + thumbnail + ","
            + "icon:" + icon + ","
            + "weight:" + weight + ","
            + "iconPath:" + iconPath + ","
            + "combineIdList:" + StringUtil.CollectionToString(combineIdList) + ","
            + "effectIdList:" + StringUtil.CollectionToString(effectIdList) + ","
            + "}";
        }

        partial void PostInit();
    }
}