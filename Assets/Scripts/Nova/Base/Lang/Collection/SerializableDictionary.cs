/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

using System.Collections.Generic;

namespace NovaEngine
{
    /// <summary>
    /// 序列号字典对象封装类
    /// </summary>
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, UnityEngine.ISerializationCallbackReceiver
    {
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
        private List<TKey> m_keys = new List<TKey>();

        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
        private List<TValue> m_values = new List<TValue>();

        void UnityEngine.ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.Clear();

            for (int n = 0; n < m_keys.Count && n < m_values.Count; ++n)
            {
                this[this.m_keys[n]] = this.m_values[n];
            }
        }

        void UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.m_keys.Clear();
            this.m_values.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                this.m_keys.Add(pair.Key);
                this.m_values.Add(pair.Value);
            }
        }
    }
}
