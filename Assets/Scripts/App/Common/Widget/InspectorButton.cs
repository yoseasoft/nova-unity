/// -------------------------------------------------------------------------------
/// AppEngine Framework
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

namespace AppEngine
{
    /// <summary>
    /// 导出按钮的属性定义
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class InspectorButtonAttribute : UnityEngine.PropertyAttribute
    {
        /// <summary>
        /// 按钮名称
        /// </summary>
        private readonly string m_buttonName;
        /// <summary>
        /// 按钮回调函数名称
        /// </summary>
        private readonly string m_methodName;

        /// <summary>
        /// 按钮宽度
        /// </summary>
        private float m_buttonWidth;

        public string ButtonName => m_buttonName;
        public string MethodName => m_methodName;

        public float ButtonWidth
        {
            get => m_buttonWidth;
            set { m_buttonWidth = value; }
        }

        public InspectorButtonAttribute(string buttonName, string methodName, float buttonWidth)
        {
            m_buttonName = buttonName;
            m_methodName = methodName;
            m_buttonWidth = buttonWidth;
        }
    }

    #if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private System.Reflection.MethodInfo m_methodInfo = null;

        public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label)
        {
            // base.OnGUI(position, property, label);

            InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute) attribute;
            UnityEngine.Rect buttonRect = new UnityEngine.Rect(position.x + (position.width - inspectorButtonAttribute.ButtonWidth) * 0.5f, position.y,
                                                               inspectorButtonAttribute.ButtonWidth, position.height);
            if (UnityEngine.GUI.Button(buttonRect, label.text))
            {
                System.Type eventOwnerType = property.serializedObject.targetObject.GetType();
                string eventName = inspectorButtonAttribute.MethodName;

                if (null == m_methodInfo)
                {
                    m_methodInfo = eventOwnerType.GetMethod(eventName, System.Reflection.BindingFlags.Instance |
                                                                       System.Reflection.BindingFlags.Static |
                                                                       System.Reflection.BindingFlags.Public |
                                                                       System.Reflection.BindingFlags.NonPublic);
                }

                if (null != m_methodInfo)
                {
                    m_methodInfo.Invoke(property.serializedObject.targetObject, null);
                }
                else
                {
                    UnityEngine.Debug.LogWarningFormat("InspectorButton: Unable to find method {0} in {1}.", eventName, eventOwnerType);
                }
            }
        }
    }
    #endif
}
