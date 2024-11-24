using UnityEngine;
using UnityEditor;

namespace AssetModule.Editor.GUI
{
    /// <summary>
    /// 编辑器UI垂直分割工具类
    /// </summary>
    public class VerticalSplitter
    {
        /// <summary>
        /// 显示区域
        /// </summary>
        public Rect rect;

        /// <summary>
        /// 提供给拖动大小的可操作高度
        /// </summary>
        const int OperateSize = 5;

        /// <summary>
        /// 默认预留原有区域的百分比
        /// </summary>
        float _percent = 0.8f;

        /// <summary>
        /// 最小高度限制
        /// </summary>
        public float MinHeight { get; set; }

        /// <summary>
        /// 是否拖动中
        /// </summary>
        public bool IsResizing { get; private set; }

        /// <summary>
        /// 处理显示区域, 传入需分割的原区域的Rect
        /// </summary>
        public void OnGUI(Rect position)
        {
            rect.y = (int)(position.yMin + position.height * _percent);
            rect.width = position.width;
            rect.height = OperateSize;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                IsResizing = true;

            if (IsResizing)
            {
                float mousePosInRect = Event.current.mousePosition.y - position.yMin;
                _percent = Mathf.Clamp(mousePosInRect / position.height, 0.2f, 0.9f);
                rect.y = (int)(position.yMin + position.height * _percent);

                // 检查最小高度
                if (rect.y > position.yMax - MinHeight)
                {
                    rect.y = position.yMax - MinHeight;
                    _percent = Mathf.Clamp((float)(rect.y - position.yMin) / position.height, 0.2f, 0.9f);
                    rect.y = (int)(position.yMin + position.height * _percent);
                }

                if (Event.current.type == EventType.MouseUp || !position.Contains(Event.current.mousePosition))
                    IsResizing = false;
            }
            else
                _percent = Mathf.Clamp(_percent, 0.2f, 0.9f);
        }
    }
}