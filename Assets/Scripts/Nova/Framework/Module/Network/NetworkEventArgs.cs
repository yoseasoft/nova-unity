/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

namespace NovaEngine
{
    /// <summary>
    /// 网络对象事件参数基类定义
    /// </summary>
    public sealed class NetworkEventArgs : ModuleEventArgs
    {
        /// <summary>
        /// 事件参数类型标识
        /// </summary>
        public override int ID => (int) ModuleObject.EEventType.Network;

        /// <summary>
        /// 网络基础指令协议类型
        /// </summary>
        private int m_type;

        /// <summary>
        /// 网络通道标识
        /// </summary>
        private int m_channelID = 0;

        /// <summary>
        /// 网络数据实体
        /// </summary>
        private object m_data = null;

        /// <summary>
        /// 网络事件参数对象的新实例构建接口
        /// </summary>
        public NetworkEventArgs()
        {
        }

        public int Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public int ChannelID
        {
            get { return m_channelID; }
            set { m_channelID = value; }
        }

        public object Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        /// <summary>
        /// 事件参数对象初始化接口
        /// </summary>
        public override void Initialize()
        {
            m_type = 0;
            m_channelID = 0;
            m_data = null;
        }

        /// <summary>
        /// 事件参数对象清理接口
        /// </summary>
        public override void Cleanup()
        {
            m_type = 0;
            m_channelID = 0;
            m_data = null;
        }

        /// <summary>
        /// 事件参数克隆接口
        /// </summary>
        /// <param name="args">模块事件参数对象</param>
        public override void CopyTo(ModuleEventArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}
