/// <summary>
/// 2023-09-06 Game Framework Code By Hurley
/// </summary>

using Cysharp.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// 消息构建管理器封装对象类，对网络消息进行加工，组合及封装等处理
    /// </summary>
    public static partial class MsgBuilder
    {
        public static void Send(ProtoBuf.Extension.IMessage msg)
        {
            GameEngine.SocketMessageChannel channel = NE.NetworkHandler.GetChannel(AccountManager.ChannelID) as GameEngine.SocketMessageChannel;
            channel.Send(msg);
        }

        public static UniTask<ProtoBuf.Extension.IMessage> SendAwait(ProtoBuf.Extension.IMessage msg)
        {
            GameEngine.SocketMessageChannel channel = NE.NetworkHandler.GetChannel(AccountManager.ChannelID) as GameEngine.SocketMessageChannel;
            return channel.SendAwait(msg);
        }
    }
}
