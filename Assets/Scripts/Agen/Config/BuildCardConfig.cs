
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
    public sealed partial class BuildCardConfig : BeanBase
    {
        public BuildCardConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            name = buf.ReadString();
            desc = buf.ReadString();
            cardType = (CardType)buf.ReadInt();
            needUnlock = buf.ReadBool();
            limitNum = buf.ReadInt();
            effectId = buf.ReadInt();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);preCards = new System.Collections.Generic.List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); preCards.Add(_e0);}}
            baseWeight = buf.ReadInt();
            passWeight = buf.ReadInt();

            PostInit();
        }

        public static BuildCardConfig DeserializeBuildCardConfig(ByteBuf buf)
        {
            return new BuildCardConfig(buf);
        }

        /// <summary>
        /// 卡牌id
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 名字
        /// </summary>
        public readonly string name;

        /// <summary>
        /// 描述
        /// </summary>
        public readonly string desc;

        /// <summary>
        /// 卡牌类型
        /// </summary>
        public readonly CardType cardType;

        /// <summary>
        /// 是否需要解锁
        /// </summary>
        public readonly bool needUnlock;

        /// <summary>
        /// 最大数量
        /// </summary>
        public readonly int limitNum;

        /// <summary>
        /// 效果id
        /// </summary>
        public readonly int effectId;

        /// <summary>
        /// 效果id
        /// </summary>
        public readonly System.Collections.Generic.List<int> preCards;

        /// <summary>
        /// 基础权重
        /// </summary>
        public readonly int baseWeight;

        /// <summary>
        /// 通关权重
        /// </summary>
        public readonly int passWeight;

        public const int Id = 586418016;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "name:" + name + ","
            + "desc:" + desc + ","
            + "cardType:" + cardType + ","
            + "needUnlock:" + needUnlock + ","
            + "limitNum:" + limitNum + ","
            + "effectId:" + effectId + ","
            + "preCards:" + Luban.StringUtil.CollectionToString(preCards) + ","
            + "baseWeight:" + baseWeight + ","
            + "passWeight:" + passWeight + ","
            + "}";
        }

        partial void PostInit();
    }
}