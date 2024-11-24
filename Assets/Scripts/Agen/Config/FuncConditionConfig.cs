
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
    public sealed partial class FuncConditionConfig : BeanBase
    {
        public FuncConditionConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            condType = (FuncCondType)buf.ReadInt();
            isHook = buf.ReadBool();
            {int n0 = System.Math.Min(buf.ReadSize(), buf.Size);condParams = new System.Collections.Generic.List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = buf.ReadInt(); condParams.Add(_e0);}}

            PostInit();
        }

        public static FuncConditionConfig DeserializeFuncConditionConfig(ByteBuf buf)
        {
            return new FuncConditionConfig(buf);
        }

        /// <summary>
        /// 条件id
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 条件类型
        /// </summary>
        public readonly FuncCondType condType;

        /// <summary>
        /// 是否调试
        /// </summary>
        public readonly bool isHook;

        /// <summary>
        /// 条件参数
        /// </summary>
        public readonly System.Collections.Generic.List<int> condParams;

        public const int Id = -1169401671;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "condType:" + condType + ","
            + "isHook:" + isHook + ","
            + "condParams:" + Luban.StringUtil.CollectionToString(condParams) + ","
            + "}";
        }

        partial void PostInit();
    }
}
