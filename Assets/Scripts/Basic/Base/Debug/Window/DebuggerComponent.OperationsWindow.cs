/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

using UnitySleepTimeout = UnityEngine.SleepTimeout;
using UnityResolution = UnityEngine.Resolution;
using UnityRect = UnityEngine.Rect;
using UnityGUILayout = UnityEngine.GUILayout;
using UnityScreen = UnityEngine.Screen;
using UnityCursor = UnityEngine.Cursor;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 操作信息展示窗口的对象类
        /// </summary>
        private sealed class OperationsWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Operations</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    if (UnityGUILayout.Button("Shutdown Game Framework (None)", UnityGUILayout.Height(30f)))
                    {
                        // GameEntry.Shutdown(ShutdownType.None);
                    }
                    if (UnityGUILayout.Button("Shutdown Game Framework (Restart)", UnityGUILayout.Height(30f)))
                    {
                        // GameEntry.Shutdown(ShutdownType.Restart);
                    }
                    if (UnityGUILayout.Button("Shutdown Game Framework (Quit)", UnityGUILayout.Height(30f)))
                    {
                        // GameEntry.Shutdown(ShutdownType.Quit);
                    }
                }
                UnityGUILayout.EndVertical();
            }
        }
    }
}
