
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
    public sealed partial class HouseConfig : BeanBase
    {
        public HouseConfig(ByteBuf buf)
        {
            id = buf.ReadInt();
            type = (HouseType)buf.ReadInt();
            if(buf.ReadBool()){ subtype = buf.ReadInt(); } else { subtype = null; }
            attrId = buf.ReadInt();
            initFashionId = buf.ReadInt();

            PostInit();
        }

        public static HouseConfig DeserializeHouseConfig(ByteBuf buf)
        {
            return new HouseConfig(buf);
        }

        /// <summary>
        /// 房屋ID
        /// </summary>
        public readonly int id;

        /// <summary>
        /// 类型
        /// </summary>
        public readonly HouseType type;

        /// <summary>
        /// 二级类型
        /// </summary>
        public readonly int? subtype;

        /// <summary>
        /// 属性引用ID
        /// </summary>
        public readonly int attrId;

        /// <summary>
        /// 初始时装引用ID
        /// </summary>
        public readonly int initFashionId;

        /// <summary>
        /// 初始时装引用ID
        /// </summary>
        public HouseFashionConfig InitFashionConfig => HouseFashionConfigTable.Instance.GetOrDefault(initFashionId);

        public const int Id = -1861089694;

        public override int GetTypeId() => Id;

        public override string ToString()
        {
            return "{ "
            + "id:" + id + ","
            + "type:" + type + ","
            + "subtype:" + subtype + ","
            + "attrId:" + attrId + ","
            + "initFashionId:" + initFashionId + ","
            + "}";
        }

        partial void PostInit();
    }
}