using System.Net;
using System.Resources;
using GameEngine;
using Unity.VisualScripting;
/// <summary>
/// 2023-09-06 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 账号管理器封装对象类
    /// </summary>
    public static class AccountManager
    {
        public static int ChannelID { get; set; }

        /// <summary>
        /// 网络连接请求函数
        /// 具体连接的网络类型参数设置方式可以参考<see cref="NovaEngine.NetworkServiceType"/>
        /// </summary>
        /// <param name="protocol">网络协议类型</param>
        /// <param name="name">网络名称</param>
        /// <param name="address">网络地址</param>
        /// <returns>若网络连接请求发送成功返回对应的通道标识，否则返回0</returns>
        public static void Connect(int protocol, string name, string address)
        {
            GameEngine.MessageChannel channel = NE.NetworkHandler.Connect(protocol, name, address);
            ChannelID = channel.ChannelID;

            Debugger.Log("请求连接返回 ChannelID=" + ChannelID);
        }
    }
}
