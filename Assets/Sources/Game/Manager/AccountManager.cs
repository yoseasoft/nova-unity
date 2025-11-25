/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-11-12
/// 功能描述：
/// </summary>

using Cysharp.Threading.Tasks;
using GameEngine;
using NovaEngine;

namespace Game
{
    /// <summary>
    /// 账号管理器对象类
    /// </summary>
    public static class AccountManager
    {
        static INetworkDispatchListener _listener;

        static bool _isRunning;

        static WebSocketMessageChannel _channel;

        static ConnectionState _state;

        public static WebSocketMessageChannel Channel => _channel;

        public static void Start()
        {
            _listener = new ClientNetworkDispatcher();
            NetworkHandler.Instance.AddNetworkEventDispatchListener(_listener);

            _isRunning = true;
        }

        public static void Stop()
        {
            _isRunning = false;

            if (null != _listener)
            {
                NetworkHandler.Instance.RemoveNetworkEventDispatchListener(_listener);
                _listener = null;
            }
        }

        public static async UniTask Connect(NetworkServiceType protocol, string name, string url)
        {
            Debugger.Assert(_isRunning);

            if (null != _channel)
            {
                Debugger.Warn("当前网络已处于连接状态，请先断开当前连接后再进行新的连接！");
                return;
            }

            _state = ConnectionState.None;

            _channel = NetworkHandler.Instance.Connect(protocol, name, url) as WebSocketMessageChannel;
            await UniTask.WaitUntil(CheckConnectionState);
            Debugger.Info("网络连接结束！");
        }

        public static void Disconnect()
        {
            if (null == _channel)
            {
                Debugger.Warn("当前网络未处于连接状态，断开连接失败！");
                return;
            }

            NetworkHandler.Instance.Disconnect(_channel.ChannelID);
            _channel = null;
        }

        public static void Send(object message)
        {
            if (null == _channel)
            {
                Debugger.Warn("当前网络未处于连接状态，发送数据失败！");
                return;
            }

            _channel.Send(message);
        }

        static bool CheckConnectionState()
        {
            if (ConnectionState.None != _state)
            {
                if (ConnectionState.Error == _state)
                {
                    Disconnect();
                }

                return true;
            }

            return false;
        }

        private enum ConnectionState
        {
            None = 0,
            Connected = 1,
            Disconnected = 2,
            Error = 3,
        }

        private class ClientNetworkDispatcher : INetworkDispatchListener
        {
            public void OnConnectError(MessageChannel channel)
            {
                _state = ConnectionState.Error;
                Debugger.Warn($"网络通道{channel.ChannelID}:{channel.ChannelType}连接发生错误！");
            }

            public void OnConnection(MessageChannel channel)
            {
                _state = ConnectionState.Connected;
                Debugger.Warn($"网络通道{channel.ChannelID}:{channel.ChannelType}连接成功！");
            }

            public void OnDisconnection(MessageChannel channel)
            {
                _state = ConnectionState.Disconnected;
                Debugger.Warn($"网络通道{channel.ChannelID}:{channel.ChannelType}连接失败！");
            }
        }
    }
}
