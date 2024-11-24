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

using UnityGameObject = UnityEngine.GameObject;

namespace NovaEngine
{
    /// <summary>
    /// GameObject变量对象实体类
    /// </summary>
    public sealed class CGameObject : Variable<UnityGameObject>
    {
        /// <summary>
        /// GameObject变量对象实体类的新实例构建接口
        /// </summary>
        public CGameObject()
        {
        }

        /// <summary>
        /// GameObject变量对象实体类的新实例构建接口
        /// </summary>
        /// <param name="value">变量值</param>
        public CGameObject(UnityGameObject value) : base(value)
        {
        }

        /// <summary>
        /// 从GameObject值到变量对象实体类的隐式转换
        /// </summary>
        /// <param name="value">变量值</param>
        public static implicit operator CGameObject(UnityGameObject value)
        {
            return new CGameObject(value);
        }

        /// <summary>
        /// 从GameObject变量对象实体类到值的隐式转换
        /// </summary>
        /// <param name="value">变量类实例</param>
        public static implicit operator UnityGameObject(CGameObject value)
        {
            return value.Value;
        }
    }
}