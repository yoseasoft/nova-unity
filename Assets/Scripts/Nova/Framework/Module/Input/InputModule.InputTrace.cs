/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

using System.Collections.Generic;

using UnityInput = UnityEngine.Input;
using UnityTouch = UnityEngine.Touch;
using UnityKeyCode = UnityEngine.KeyCode;

namespace NovaEngine
{
    /// <summary>
    /// 输入管理器，处理通过键盘、鼠标及触屏等方式产生的事件通知访问接口
    /// </summary>
    public sealed partial class InputModule : ModuleObject
    {
        /// <summary>
        /// 当前帧按下操作产生的按键编码
        /// </summary>
        private IList<UnityKeyCode> m_keycodePressedOnThisFrame;

        /// <summary>
        /// 当前帧长按操作产生的按键编码
        /// </summary>
        private IList<UnityKeyCode> m_keycodeMovedOnThisFrame;

        /// <summary>
        /// 当前帧释放操作产生的按键编码
        /// </summary>
        private IList<UnityKeyCode> m_keycodeReleasedOnThisFrame;

        /// <summary>
        /// 上一帧按下操作保留的按键编码
        /// </summary>
        private IList<UnityKeyCode> m_keycodeChangedOnPreviousFrame;

        /// <summary>
        /// 输入信息相关属性初始化函数
        /// </summary>
        private void InitInputTrace()
        {
            m_keycodePressedOnThisFrame = new List<UnityKeyCode>();
            m_keycodeMovedOnThisFrame = new List<UnityKeyCode>();
            m_keycodeReleasedOnThisFrame = new List<UnityKeyCode>();
            m_keycodeChangedOnPreviousFrame = new List<UnityKeyCode>();
        }

        /// <summary>
        /// 输入信息相关属性清理函数
        /// </summary>
        private void CleanupInputTrace()
        {
            RemoveAllInputKeycodes();
            m_keycodePressedOnThisFrame = null;
            m_keycodeMovedOnThisFrame = null;
            m_keycodeReleasedOnThisFrame = null;

            m_keycodeChangedOnPreviousFrame.Clear();
            m_keycodeChangedOnPreviousFrame = null;
        }

        /// <summary>
        /// 记录当前帧按下操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodePressed(UnityKeyCode code)
        {
            m_keycodePressedOnThisFrame.Add(code);
        }

        /// <summary>
        /// 取消当前帧按下操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeUnpressed(UnityKeyCode code)
        {
            m_keycodePressedOnThisFrame.Remove(code);
        }

        /// <summary>
        /// 记录当前帧长按操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeMoved(UnityKeyCode code)
        {
            m_keycodeMovedOnThisFrame.Add(code);
        }

        /// <summary>
        /// 记录当前帧释放操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeReleased(UnityKeyCode code)
        {
            m_keycodeReleasedOnThisFrame.Add(code);
        }

        /// <summary>
        /// 取消当前帧释放操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeUnreleased(UnityKeyCode code)
        {
            m_keycodeReleasedOnThisFrame.Remove(code);
        }

        /// <summary>
        /// 记录上一帧和当前帧发生变化的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeChanged(UnityKeyCode code)
        {
            m_keycodeChangedOnPreviousFrame.Add(code);
        }

        /// <summary>
        /// 取消上一帧和当前帧发生变化的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeUnchanged(UnityKeyCode code)
        {
            m_keycodeChangedOnPreviousFrame.Remove(code);
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的录入操作，包括按下，长按及释放
        /// </summary>
        /// <returns>若触发了任意按键编码的录入操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeInputed()
        {
            if (IsAnyKeycodePressed() || IsAnyKeycodeMoved() || IsAnyKeycodeReleased())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的录入操作，包括按下，长按及释放
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的录入操作则返回true，否则返回false</returns>
        public bool IsKeycodeInputed(UnityKeyCode code)
        {
            if (IsKeycodePressed(code) || IsKeycodeMoved(code) || IsKeycodeReleased(code))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的按下操作
        /// </summary>
        /// <returns>若触发了任意按键编码的按下操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodePressed()
        {
            return (m_keycodePressedOnThisFrame.Count > 0);
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的按下操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的按下操作则返回true，否则返回false</returns>
        public bool IsKeycodePressed(UnityKeyCode code)
        {
            return m_keycodePressedOnThisFrame.Contains(code);
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的长按操作
        /// </summary>
        /// <returns>若触发了任意按键编码的长按操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeMoved()
        {
            return (m_keycodeMovedOnThisFrame.Count > 0);
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的长按操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的长按操作则返回true，否则返回false</returns>
        public bool IsKeycodeMoved(UnityKeyCode code)
        {
            return m_keycodeMovedOnThisFrame.Contains(code);
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的释放操作
        /// </summary>
        /// <returns>若触发了任意按键编码的释放操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeReleased()
        {
            return (m_keycodeReleasedOnThisFrame.Count > 0);
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的释放操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的释放操作则返回true，否则返回false</returns>
        public bool IsKeycodeReleased(UnityKeyCode code)
        {
            return m_keycodeReleasedOnThisFrame.Contains(code);
        }

        /// <summary>
        /// 检测上一帧和当前帧是否触发了任意按键编码的改变操作
        /// </summary>
        /// <returns>若触发了任意按键编码的改变操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeChanged()
        {
            return (m_keycodeChangedOnPreviousFrame.Count > 0);
        }

        /// <summary>
        /// 检测上一帧和当前帧是否触发了指定按键编码的改变操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的改变操作则返回true，否则返回false</returns>
        public bool IsKeycodeChanged(UnityKeyCode code)
        {
            return m_keycodeChangedOnPreviousFrame.Contains(code);
        }

        /// <summary>
        /// 移除当前帧记录的全部按键编码信息
        /// </summary>
        private void RemoveAllInputKeycodes()
        {
            m_keycodePressedOnThisFrame.Clear();
            m_keycodeMovedOnThisFrame.Clear();
            m_keycodeReleasedOnThisFrame.Clear();
        }
    }
}
