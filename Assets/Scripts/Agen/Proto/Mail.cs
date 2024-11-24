using ProtoBuf;
using ProtoBuf.Extension;
using System.Collections.Generic;

namespace Game.Proto
{
    // 消息id 600 - 700
    /// <summary>
    /// 邮件附件信息
    /// </summary>
    [ProtoContract]
    public partial class MailAttachData : Object
    {
        /// <summary>
        /// 物品id
        /// </summary>
        [ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 物品数量
        /// </summary>
        [ProtoMember(2)]
        public int Num { get; set; }

        /// <summary>
        /// 是否绑定
        /// </summary>
        [ProtoMember(3)]
        public bool Bind { get; set; }

        /// <summary>
        /// 最大堆叠数量
        /// </summary>
        [ProtoMember(4)]
        public int Max { get; set; }
    }

    /// <summary>
    /// 邮件数据
    /// </summary>
    [ProtoContract]
    public partial class MailInfo : Object
    {
        /// <summary>
        /// 邮件id
        /// </summary>
        [ProtoMember(1)]
        public int MailUid { get; set; }

        /// <summary>
        /// 邮件标题
        /// </summary>
        [ProtoMember(2)]
        public string Title { get; set; }

        /// <summary>
        /// 邮件作者
        /// </summary>
        [ProtoMember(3)]
        public string Author { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        [ProtoMember(4)]
        public string Content { get; set; }
    }

    /// <summary>
    /// 邮件元数据
    /// </summary>
    [ProtoContract]
    public partial class MailMetaInfo : Object
    {
        /// <summary>
        /// 邮件id
        /// </summary>
        [ProtoMember(1)]
        public int MailUid { get; set; }

        /// <summary>
        /// 模板邮件id >0时客户端自己读表组装邮件信息
        /// </summary>
        [ProtoMember(2)]
        public int Tplid { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>
        [ProtoMember(3)]
        public int ReadTime { get; set; }

        /// <summary>
        /// 领取附件时间
        /// </summary>
        [ProtoMember(4)]
        public int ReceiveTime { get; set; }

        /// <summary>
        /// 邮件发送时间
        /// </summary>
        [ProtoMember(5)]
        public int SendTime { get; set; }

        /// <summary>
        /// 附件数据
        /// </summary>
        [ProtoMember(6)]
        public List<MailAttachData> AttachList { get; set; } = new();
    }

    /// <summary>
    /// 邮件元变化数据
    /// </summary>
    [ProtoContract]
    public partial class MailMetaChangeInfo : Object
    {
        /// <summary>
        /// 邮件id
        /// </summary>
        [ProtoMember(1)]
        public int MailUid { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>
        [ProtoMember(2)]
        public int ReadTime { get; set; }

        /// <summary>
        /// 领取附件时间
        /// </summary>
        [ProtoMember(3)]
        public int ReceiveTime { get; set; }
    }

    /// <summary>
    /// 邮件元信息通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailMetaInfoNotify)]
    public partial class MailMetaInfoNotify : Object, IMessage
    {
        /// <summary>
        /// 邮件元数据
        /// </summary>
        [ProtoMember(1)]
        public List<MailMetaInfo> MailMetaList { get; set; } = new();

        /// <summary>
        /// 脏标识
        /// </summary>
        [ProtoMember(2)]
        public bool DirtyFlag { get; set; }
    }

    /// <summary>
    /// 邮件元信息删除通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailMetaInfoDeletedNotify)]
    public partial class MailMetaInfoDeletedNotify : Object, IMessage
    {
        /// <summary>
        /// 删除的邮件id
        /// </summary>
        [ProtoMember(1)]
        public List<int> MailidList { get; set; } = new();
    }

    /// <summary>
    /// 邮件元数据添加通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailMetaInfoAddNotify)]
    public partial class MailMetaInfoAddNotify : Object, IMessage
    {
        /// <summary>
        /// 添加的邮件元数据
        /// </summary>
        [ProtoMember(1)]
        public List<MailMetaInfo> MailMetaList { get; set; } = new();
    }

    /// <summary>
    /// 邮件元信息变化通知
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailMetaInfoChangedNotify)]
    public partial class MailMetaInfoChangedNotify : Object, IMessage
    {
        /// <summary>
        /// 变化的邮件元信息
        /// </summary>
        [ProtoMember(1)]
        public List<MailMetaChangeInfo> MailMetaList { get; set; } = new();
    }

    /// <summary>
    /// 邮件获取请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailGetReq)]
    [MessageResponseType(ProtoOpcode.MailGetResp)]
    public partial class MailGetReq : Object, IMessage
    {
        /// <summary>
        /// 邮件id列表
        /// </summary>
        [ProtoMember(1)]
        public List<int> MailidList { get; set; } = new();
    }

    /// <summary>
    /// 邮件获取回复
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailGetResp)]
    public partial class MailGetResp : Object, IMessage
    {
        /// <summary>
        /// 邮件列表
        /// </summary>
        [ProtoMember(1)]
        public List<MailInfo> MailInfoList { get; set; } = new();
    }

    /// <summary>
    /// 邮件读取请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailReadReq)]
    public partial class MailReadReq : Object, IMessage
    {
        /// <summary>
        /// 邮件id列表
        /// </summary>
        [ProtoMember(1)]
        public List<int> MailidList { get; set; } = new();
    }

    /// <summary>
    /// 邮件领取请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailReceiveReq)]
    public partial class MailReceiveReq : Object, IMessage
    {
        /// <summary>
        /// 邮件id列表
        /// </summary>
        [ProtoMember(1)]
        public List<int> MailidList { get; set; } = new();
    }

    /// <summary>
    /// 删除邮件
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailDeleteReq)]
    public partial class MailDeleteReq : Object, IMessage
    {
        /// <summary>
        /// 邮件id列表
        /// </summary>
        [ProtoMember(1)]
        public List<int> MailidList { get; set; } = new();
    }

    /// <summary>
    /// 邮件脏标识
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailDirtyNotify)]
    public partial class MailDirtyNotify : Object, IMessage
    {
        /// <summary>
        /// 脏
        /// </summary>
        [ProtoMember(1)]
        public bool DirtyFlag { get; set; }
    }

    /// <summary>
    /// 邮件拉取 dirty_flag为true时 打开邮件界面则发起请求
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailPullReq)]
    public partial class MailPullReq : Object, IMessage
    {
    }

    /// <summary>
    /// 邮件过期检查
    /// </summary>
    [ProtoContract]
    [Message(ProtoOpcode.MailExpireCheckReq)]
    public partial class MailExpireCheckReq : Object, IMessage
    {
    }
}