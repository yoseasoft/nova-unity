using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    /// <summary>
    /// 突发事件规则配置
    /// </summary>
    public sealed partial class EmergencyRuleConfig : BeanBase
    {
        public EmergencyRuleConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);targetSymbols = new List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); targetSymbols.Add(_e0);}}
            symbolCount = buf.ReadInt();

            PostInit();
        }

        public static EmergencyRuleConfig DeserializeEmergencyRuleConfig(ByteBuf buf)
        {
            return new EmergencyRuleConfig(buf);
        }

        /// <summary>
        /// 规则标识
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 目标符号列表
        /// </summary>
        public readonly List<int> targetSymbols;

        /// <summary>
        /// 符号个数
        /// </summary>
        public readonly int symbolCount;

        public const int Id = -1373027921;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "targetSymbols:" + StringUtil.CollectionToString(targetSymbols) + ","
            + "symbolCount:" + symbolCount + ","
            + "}";
        }

        partial void PostInit();
    }
}