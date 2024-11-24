/// <summary>
/// 2024-06-06 Game Framework Code By Hurley
/// </summary>

using System;
using System.Threading;

using Cysharp.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// 测试异步任务
    /// </summary>
    public class TestAsyncTask : ITestCase
    {
        public void Startup()
        {
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
                    TestSendMessage(1001, "hello world");
                }
                else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.B))
                {
                    TestRecvMessage(1001, "good look");
                }
            }
        }

        private int m_sendID = 0;
        private UniTaskCompletionSource<string> m_taskCompletionSource;
        private CancellationTokenSource m_cancellationTokenSource;

        private async void TestSendMessage(int id, string message)
        {
            Debugger.Warn($"test send message id = {id}, content = {message} began ...");

            //UniTask<string> task = SendMessage(id, message);
            //await task;
            var (failed, result) = await SendMessage(id, message).TimeoutWithoutException<string>(TimeSpan.FromMilliseconds(5000));
            if (failed)
            {
                Debugger.Warn("发送消息超时失败!");
                return;
            }

            Debugger.Warn($"test send message id = {id}, recv data = {result} ended ...");
        }

        private void TestRecvMessage(int id, string message)
        {
            if (m_sendID == 0 || m_sendID != id)
            {
                Debugger.Warn("当前接收到的消息标识与待命标识不一致，接收的内容无法识别！");
                return;
            }

            m_taskCompletionSource.TrySetResult(message);

            // m_cancellationTokenSource.Cancel();
            m_cancellationTokenSource.Dispose();
            m_cancellationTokenSource = null;
            m_sendID = 0;
            m_taskCompletionSource = null;

            Debugger.Warn($"接收到指定的消息标识 {id} 及内容 {message}.");
        }

        private async UniTask<string> SendMessage(int id, string message)
        {
            if (m_sendID > 0)
            {
                Debugger.Warn("当前已存在一个有效的发送标识在等待回复，不支持同时存在多个待命标识！", m_sendID);
                throw new System.OperationCanceledException();
            }

            UniTaskCompletionSource<string> source = new UniTaskCompletionSource<string>();
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            m_sendID = id;
            m_taskCompletionSource = source;
            m_cancellationTokenSource = tokenSource;

            source.OnCompleted(o => { Debugger.Warn($"当前待命消息标识被重置 {m_sendID}."); m_sendID = 0; m_taskCompletionSource = null; }, id, 1);

            //using (tokenSource.Token.Register(() => source.TrySetCanceled()))
            //{
            //    await UniTask.Delay(2000, cancellationToken: tokenSource.Token);
            //    if (false == tokenSource.IsCancellationRequested)
            //    {
            //        Debugger.Warn("超过延迟时间，此次会话通信关闭!");
            //        m_sendID = 0;
            //        m_taskCompletionSource = null;
            //        m_cancellationTokenSource.Dispose();
            //        m_cancellationTokenSource = null;
            //    }
            //}
            //tokenSource.Token.Register(() => { Debugger.Warn($"当前待命消息标识被撤销 {m_sendID}."); source.TrySetResult(null); source.TrySetCanceled(); });
            //tokenSource.CancelAfter(5000);

            return await source.Task;
        }
    }
}
