/// <summary>
/// 2024-05-15 Game Framework Code By Hurley
/// </summary>

using System.Collections;
using System.Reflection;
using System.Linq;
using GameEngine;
using Game.Proto;

namespace Game
{
    /// <summary>
    /// 测试网络连接
    /// </summary>
    public class TestNetworkConn : ITestCase
    {
        private GameEngine.WebSocketMessageChannel channel = null;

        // private const string URL = "ws://136.55.98.66:8181";
        // private const string URL = "ws://127.0.0.1:8181";
        private const string URL = "ws://10.1.90.94:24082";

        public void Startup()
        {
            channel = NE.NetworkHandler.Connect((int) NovaEngine.NetworkServiceType.WebSocket, "Main", URL) as WebSocketMessageChannel;

            Debugger.Warn("==================================================> TestNetworkConn end <==================================================");
        }

        public void Shutdown()
        {
        }

        public void Update()
        {
            if (UnityEngine.Input.anyKey)
            {
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A))
                {
                    //string text = "[hello, world! welcome to Unity3D world.]";
                    //channel.Send(System.Text.Encoding.UTF8.GetBytes(text));
                    //text = "[1234567890]";
                    //channel.Send(System.Text.Encoding.UTF8.GetBytes(text));
                    //Debugger.Log("-------------------------!!!!!!!!!!!!!!!!xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

                    channel.Send(new PingReq { Str = "hello" });
                    channel.Send(new PingReq { Str = "world" });
                    channel.Send(new PingReq { Str = "unity" });
                }
            }
        }
    }
}
