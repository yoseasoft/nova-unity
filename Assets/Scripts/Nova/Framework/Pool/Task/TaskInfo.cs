/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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
    /// 任务信息基类
    /// </summary>
    public struct TaskInfo
    {
        private readonly int m_serialID;
        private readonly int m_priority;
        private readonly TaskStatus m_status;
        private readonly string m_description;

        /// <summary>
        /// 初始化任务信息的新实例
        /// </summary>
        /// <param name="serialID">任务的序列编号</param>
        /// <param name="priority">任务的优先级</param>
        /// <param name="status">任务状态</param>
        /// <param name="description">任务描述</param>
        public TaskInfo(int serialID, int priority, TaskStatus status, string description)
        {
            this.m_serialID = serialID;
            this.m_priority = priority;
            this.m_status = status;
            this.m_description = description;
        }

        /// <summary>
        /// 获取任务的序列编号
        /// </summary>
        public int SerialID
        {
            get { return m_serialID; }
        }

        /// <summary>
        /// 获取任务的优先级
        /// </summary>
        public int Priority
        {
            get { return m_priority; }
        }

        /// <summary>
        /// 获取任务状态
        /// </summary>
        public TaskStatus Status
        {
            get { return m_status; }
        }

        /// <summary>
        /// 获取任务描述
        /// </summary>
        public string Description
        {
            get { return m_description; }
        }
    }
}
