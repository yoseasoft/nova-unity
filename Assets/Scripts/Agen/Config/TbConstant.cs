
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using System.Collections.Generic;

namespace Game.Config
{
    [Config]
    public partial class ConstantConfigTable : ConfigSingleton<ConstantConfigTable>
    {

        private readonly ConstantConfig _data;

        public ConstantConfigTable(ByteBuf buf)
        {
            int n = buf.ReadSize();
            if (n != 1) throw new SerializationException("table mode=one, but size != 1");
            _data = ConstantConfig.DeserializeConstantConfig(buf);
        }


        /// <summary>
        /// 版本号
        /// </summary>
        public string version => _data.version;
        /// <summary>
        /// 批量获取邮件最大数量
        /// </summary>
        public int batchGetMailMaxNum => _data.batchGetMailMaxNum;
        /// <summary>
        /// 批量读取邮件最大数量
        /// </summary>
        public int batchReadMailMaxNum => _data.batchReadMailMaxNum;
        /// <summary>
        /// 批量领取邮件最大数量
        /// </summary>
        public int batchReceiveMailMaxNum => _data.batchReceiveMailMaxNum;
        /// <summary>
        /// 批量删除邮件最大数量
        /// </summary>
        public int batchDeleteMailMaxNum => _data.batchDeleteMailMaxNum;
        /// <summary>
        /// 邮件最大数量
        /// </summary>
        public int mailMaxNum => _data.mailMaxNum;
        /// <summary>
        /// 邮件过期时间
        /// </summary>
        public int mailExpireSec => _data.mailExpireSec;
        /// <summary>
        /// 背包满邮件id
        /// </summary>
        public int bagFullMailid => _data.bagFullMailid;
        /// <summary>
        /// 初始物品列表
        /// </summary>
        public System.Collections.Generic.List<DItem> INITITEMLIST => _data.INITITEMLIST;
        /// <summary>
        /// 体力恢复间隔秒
        /// </summary>
        public int ENERGYRECOVERINTERVAL => _data.ENERGYRECOVERINTERVAL;
        /// <summary>
        /// 体力恢复点数
        /// </summary>
        public int ENERGYRECOVERPOINTS => _data.ENERGYRECOVERPOINTS;

        partial void PostInit();
    }
}
