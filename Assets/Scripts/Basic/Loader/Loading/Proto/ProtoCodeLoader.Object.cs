/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 对象类的结构信息
    /// </summary>
    public class ObjectCodeInfo : EntityCodeInfo
    {
        /// <summary>
        /// 对象名称
        /// </summary>
        private string m_objectName;
        /// <summary>
        /// 对象功能类型
        /// </summary>
        private int m_funcType;

        public string ObjectName { get { return m_objectName; } internal set { m_objectName = value; } }
        public int FuncType { get { return m_funcType; } internal set { m_funcType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Object = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("Name = {0}, ", m_objectName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("FuncType = {0}, ", m_funcType);
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 程序集中原型对象的分析处理类，对业务层载入的所有原型对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class ProtoCodeLoader
    {
        /// <summary>
        /// 对象类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, ObjectCodeInfo> s_objectCodeInfos = new Dictionary<string, ObjectCodeInfo>();

        [OnProtoClassLoadOfTarget(typeof(CObject))]
        private static bool LoadObjectClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CObject).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CObject' interface, load it failed.", symClass.FullName);
                return false;
            }

            ObjectCodeInfo info = new ObjectCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(DeclareObjectClassAttribute) == attrType)
                {
                    DeclareObjectClassAttribute _attr = (DeclareObjectClassAttribute) attr;
                    info.ObjectName = _attr.ObjectName;
                    info.FuncType = _attr.FuncType;
                }
                else
                {
                    LoadEntityClassByAttributeType(symClass, info, attr);
                }
            }

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                LoadObjectMethod(symClass, info, symMethod);
            }

            if (string.IsNullOrEmpty(info.ObjectName))
            {
                const string OBJECT_TAG = "Object";
                string objectName = symClass.ClassName;
                if (objectName.Length > OBJECT_TAG.Length)
                {
                    // 判断是否为“Object”后缀
                    if (objectName.Substring(objectName.Length - OBJECT_TAG.Length).Equals(OBJECT_TAG))
                    {
                        // 裁剪掉“Object”后缀
                        string prefixName = objectName.Substring(0, objectName.Length - OBJECT_TAG.Length);
                        if (prefixName.Length > 0)
                        {
                            info.ObjectName = prefixName;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(info.ObjectName))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The object '{0}' name must be non-null or empty space.", symClass.FullName);
                info.ObjectName = symClass.ClassName;
            }

            if (s_objectCodeInfos.ContainsKey(info.ObjectName))
            {
                if (reload)
                {
                    s_objectCodeInfos.Remove(info.ObjectName);
                }
                else
                {
                    Debugger.Warn("The object name '{0}' was already existed, repeat added it failed.", info.ObjectName);
                    return false;
                }
            }

            s_objectCodeInfos.Add(info.ObjectName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CObject' code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        private static void LoadObjectMethod(Symboling.SymClass symClass, ObjectCodeInfo codeInfo, Symboling.SymMethod symMethod)
        {
            // 静态函数直接忽略
            if (symMethod.IsStatic)
            {
                return;
            }

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];

                LoadEntityMethodByAttributeType(symClass, codeInfo, symMethod, attr);
            }
        }

        [OnProtoClassCleanupOfTarget(typeof(CObject))]
        private static void CleanupAllObjectClasses()
        {
            s_objectCodeInfos.Clear();
        }

        [OnProtoCodeInfoLookupOfTarget(typeof(CObject))]
        private static ObjectCodeInfo LookupObjectCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, ObjectCodeInfo> pair in s_objectCodeInfos)
            {
                if (pair.Value.ClassType == symClass.ClassType)
                {
                    return pair.Value;
                }
            }

            return null;
        }
    }
}
