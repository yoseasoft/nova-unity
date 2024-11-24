/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemDateTime = System.DateTime;

namespace GameEngine
{
    /// <summary>
    /// 对象模块统计项对象类，对对象模块访问记录进行单项统计的数据单元
    /// </summary>
    public sealed class ObjectStatInfo : IStatInfo
    {
        /// <summary>
        /// 对象记录索引标识
        /// </summary>
        private readonly int m_uid;
        /// <summary>
        /// 对象名称
        /// </summary>
        private readonly string m_objectName;
        /// <summary>
        /// 对象的哈希码，用来确保对象唯一性
        /// </summary>
        private readonly int m_hashCode;
        /// <summary>
        /// 对象的创建时间
        /// </summary>
        private SystemDateTime m_createTime;
        /// <summary>
        /// 对象的释放时间
        /// </summary>
        private SystemDateTime m_releaseTime;

        public ObjectStatInfo(int uid, string objectName, int hashCode)
        {
            m_uid = uid;
            m_objectName = objectName;
            m_hashCode = hashCode;
            m_createTime = SystemDateTime.MinValue;
            m_releaseTime = SystemDateTime.MinValue;
        }

        public int Uid
        {
            get { return m_uid; }
        }

        public string ObjectName
        {
            get { return m_objectName; }
        }

        public int HashCode
        {
            get { return m_hashCode; }
        }

        public SystemDateTime CreateTime
        {
            get { return m_createTime; }
            set { m_createTime = value; }
        }

        public SystemDateTime ReleaseTime
        {
            get { return m_releaseTime; }
            set { m_releaseTime = value; }
        }
    }
}