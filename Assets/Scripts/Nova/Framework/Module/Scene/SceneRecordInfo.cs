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

using UnityObject = UnityEngine.Object;
using UnityScene = UnityEngine.SceneManagement.Scene;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace NovaEngine
{
    /// <summary>
    /// 场景对象运行时信息数据封装对象类，对场景资源及相关使用参数进行统一封装管理
    /// 其数据结构包含场景资源，层级，物件，相关烘焙或光照等
    /// </summary>
    public partial class SceneRecordInfo
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        private string m_name = string.Empty;

        /// <summary>
        /// 场景开启状态标识，若该标识未开启，则无法进行使用
        /// </summary>
        private bool m_enabled = false;

        /// <summary>
        /// 场景不可移除状态标识，该标识仅适用于主场景节点
        /// </summary>
        private bool m_unmovabled = false;

        /// <summary>
        /// 场景数据状态类型，用于标识场景数据当前状态是否可用
        /// </summary>
        private EStateType m_stateType = EStateType.None;

        /// <summary>
        /// 场景元素对象实例
        /// </summary>
        private UnityScene m_scene;

        /// <summary>
        /// 场景资源信息对象实例
        /// </summary>
        private AssetModule.Scene m_assetScene = null;

        /// <summary>
        /// 场景信息对象构造函数
        /// </summary>
        protected SceneRecordInfo()
        {
        }

        /// <summary>
        /// 场景信息对象析构函数
        /// </summary>
        ~SceneRecordInfo()
        {
        }

        /// <summary>
        /// 场景信息对象初始化回调函数
        /// </summary>
        /// <returns>若场景信息对象初始化成功返回true，否则返回false</returns>
        protected bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// 场景信息对象清理回调函数
        /// </summary>
        protected void Cleanup()
        {
            if (m_assetScene != null)
            {
                ResourceModule.UnloadScene(m_assetScene);
                m_assetScene = null;
            }
        }

        /// <summary>
        /// 场景信息对象静态创建函数，通过名称标识创建一个场景对象实例
        /// 场景信息对象创建后默认为关闭状态，需要外部手动启用该场景
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <returns>若场景信息对象创建成功则返回对应的引用实例，否则返回null</returns>
        public static SceneRecordInfo Create(string name)
        {
            SceneRecordInfo ret = new SceneRecordInfo();
            ret.Name = name;

            if (ret.Initialize())
            {
                return ret;
            }

            return null;
        }

        /// <summary>
        /// 场景信息对象销毁函数
        /// </summary>
        public void Destroy()
        {
            Cleanup();
        }

        #region 场景信息对象基础属性Getter/Setter函数

        /// <summary>
        /// 场景名称属性访问Getter/Setter接口
        /// </summary>
        public string Name
        {
            set { m_name = value; }
            get { return m_name; }
        }

        /// <summary>
        /// 场景启用状态标识属性访问Getter/Setter接口
        /// </summary>
        public bool Enabled
        {
            set { m_enabled = value; }
            get { return m_enabled; }
        }

        /// <summary>
        /// 场景不可移除状态标识属性访问Getter/Setter接口
        /// </summary>
        public bool Unmovabled
        {
            set { m_unmovabled = value; }
            get { return m_unmovabled; }
        }

        /// <summary>
        /// 场景资源加载状态标识属性访问Getter/Setter接口
        /// </summary>
        public EStateType StateType
        {
            set { m_stateType = value; }
            get { return m_stateType; }
        }

        /// <summary>
        /// 场景元素对象属性访问Getter/Setter接口
        /// </summary>
        public UnityScene Scene
        {
            set { m_scene = value; }
            get { return m_scene; }
        }

        /// <summary>
        /// 场景资源信息对象属性访问Getter/Setter接口
        /// </summary>
        public AssetModule.Scene AssetScene
        {
            set { m_assetScene = value; }
            get { return m_assetScene; }
        }

        #endregion
    }
}
