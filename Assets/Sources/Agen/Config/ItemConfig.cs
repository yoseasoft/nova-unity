using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    /// <summary>
    /// 物品配置
    /// </summary>
    public sealed partial class ItemConfig : BeanBase
    {
        public ItemConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            name = buf.ReadString();
            qualityType = (QualityType)buf.ReadInt();
            extractabled = buf.ReadBool();
            disabled = buf.ReadBool();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);effectIdList = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); effectIdList.Add(_e0);}}
            positiveText = buf.ReadString();
            negativeText = buf.ReadString();
            icon = buf.ReadInt();

            PostInit();
        }

        public static ItemConfig DeserializeItemConfig(ByteBuf buf)
        {
            return new ItemConfig(buf);
        }

        /// <summary>
        /// 物品标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 物品名称
        /// </summary>
        public readonly string name;

        /// <summary>
        /// 品质类型
        /// </summary>
        public readonly QualityType qualityType;

        /// <summary>
        /// 可提取状态标识
        /// </summary>
        public readonly bool extractabled;

        /// <summary>
        /// 可禁用状态标识
        /// </summary>
        public readonly bool disabled;

        /// <summary>
        /// 效果列表
        /// </summary>
        public readonly List<int> effectIdList;

        /// <summary>
        /// 正面信息描述
        /// </summary>
        public readonly string positiveText;

        /// <summary>
        /// 负面信息描述
        /// </summary>
        public readonly string negativeText;

        /// <summary>
        /// 物品图标
        /// </summary>
        public readonly int icon;

        public const int Id = -764023723;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "name:" + name + ","
            + "qualityType:" + qualityType + ","
            + "extractabled:" + extractabled + ","
            + "disabled:" + disabled + ","
            + "effectIdList:" + StringUtil.CollectionToString(effectIdList) + ","
            + "positiveText:" + positiveText + ","
            + "negativeText:" + negativeText + ","
            + "icon:" + icon + ","
            + "}";
        }

        partial void PostInit();
    }
}