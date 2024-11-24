/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemLayoutKind = System.Runtime.InteropServices.LayoutKind;

namespace NovaEngine.ObjectPool
{
    /// <summary>
    /// 类型和名称组合的数据结构类，定义了一个捆绑类型和名称对应关系的数据结构<br/>
    /// 您可以在反射或查找等逻辑中使用该数据结构来做查询索引
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(SystemLayoutKind.Auto)]
    internal struct TypeNamePair : System.IEquatable<TypeNamePair>
    {
        /// <summary>
        /// 关联的映射类型
        /// </summary>
        private readonly SystemType m_type;
        /// <summary>
        /// 关联的映射名称
        /// </summary>
        private readonly string m_name;

        /// <summary>
        /// 获取映射类型
        /// </summary>
        public SystemType Type { get { return m_type; } }

        /// <summary>
        /// 获取映射名称
        /// </summary>
        public string Name { get { return m_name; } }

        /// <summary>
        /// 类型名称映射结构的初始构造函数
        /// </summary>
        /// <param name="type">映射类型</param>
        public TypeNamePair(SystemType type) : this(type, null)
        {
        }

        /// <summary>
        /// 类型名称映射结构的初始构造函数
        /// </summary>
        /// <param name="type">映射类型</param>
        /// <param name="name">映射名称</param>
        /// <exception cref="CException"></exception>
        public TypeNamePair(SystemType type, string name)
        {
            if (null == type)
            {
                throw new CException("Type is invalid.");
            }

            m_type = type;
            m_name = name ?? string.Empty;
        }

        /// <summary>
        /// 获取当前对象的哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return m_type.GetHashCode() ^ m_name.GetHashCode();
        }

        /// <summary>
        /// 比较当前对象与指定对象是否相同
        /// </summary>
        /// <param name="other">目标对象实例</param>
        /// <returns>若两个实例相同则返回true，否则返回false</returns>
        public bool Equals(TypeNamePair other)
        {
            return m_type == other.m_type && m_name == other.m_name;
        }

        /// <summary>
        /// 比较当前对象与指定对象是否相同
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        /// <returns>若两个实例相同则返回true，否则返回false</returns>
        public override bool Equals(object obj)
        {
            return (obj is TypeNamePair) && Equals((TypeNamePair) obj);
        }

        /// <summary>
        /// 获取类型和名称组合值的显示字符串信息
        /// </summary>
        /// <returns>返回该实例的字符串信息</returns>
        public override string ToString()
        {
            if (null == m_type)
            {
                throw new CException("Type is invalid.");
            }

            string typeName = m_type.FullName;
            return string.IsNullOrEmpty(m_name) ? typeName : Utility.Text.Format("{0}.{1}", typeName, m_name);
        }

        /// <summary>
        /// 判断两个对象实例是否相等
        /// </summary>
        /// <param name="a">值a</param>
        /// <param name="b">值b</param>
        /// <returns>若两个值相等则返回true，否则返回false</returns>
        public static bool operator ==(TypeNamePair a, TypeNamePair b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 判断两个对象实例是否不相等
        /// </summary>
        /// <param name="a">值a</param>
        /// <param name="b">值b</param>
        /// <returns>若两个值不相等则返回true，否则返回false</returns>
        public static bool operator !=(TypeNamePair a, TypeNamePair b)
        {
            return !(a == b);
        }
    }
}
