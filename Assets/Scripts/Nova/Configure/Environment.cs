/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemFieldInfo = System.Reflection.FieldInfo;

namespace NovaEngine
{
    /// <summary>
    /// 基础环境属性定义类，对当前引擎运行所需的环境成员属性进行设置及管理
    /// </summary>
    public static partial class Environment
    {
        /// <summary>
        /// 编辑器模式，用于项目资源调试
        /// 在编辑器模式下，资源访问路径可以允许优先从Resource目录读取，若读取失败则从打包目录下读取，并且引擎默认选用编辑器配置参数
        /// 非编辑器模式直接从打包目录读取，不考虑Resource目录，并且引擎默认选用本地配置文件设置参数
        /// </summary>
        public static readonly bool editorMode = false;

        /// <summary>
        /// 调试模式，用于项目内部测试
        /// 在调试模式下，默认打开全部日志输入级别，同时开放全部第三方调试插件
        /// 非调试模式下所有第三方调试插件将全部关闭
        /// </summary>
        public static readonly bool debugMode = false;

        /// <summary>
        /// 调试级别，用于项目内部测试
        /// 若关闭调试模式，该属性将没有任何效果
        /// 仅在调试模式下，程序将根据级别参数进行对应的调试输出
        /// 
        /// 引擎调试级别参数宏，具体参数定义如下：
        /// - 0：仅提供错误和致命级别的日志标准输出
        /// - 1：提供错误和致命级别的日志追踪输出，警告级别的日志标准输出
        /// - 2：提供警告，错误和致命级别的日志追踪输出，普通级别的日志标准输出
        /// - 3：提供警告，错误和致命级别的日志追踪输出，调试和普通级别的日志标准输出
        /// - 4：提供调试，普通，警告，错误和致命级别的日志追踪输出
        /// </summary>
        public static readonly int debugLevel = 0;

        /// <summary>
        /// 加密模式，项目发布阶段打开该选项，打包加密资源以避免数据泄露
        /// 在加密模式下，对本地资源及网络协议进行加密/解密处理（Resource目录资源除外）
        /// 在非加密模式下，所有资源及协议均直接访问，不考虑加密情况
        /// </summary>
        public static readonly bool cryptMode = false;

        /// <summary>
        /// 程序名称，此处可设置为标识，通过本地化文件显示程序实际别名
        /// </summary>
        public static readonly string applicationName = "unknown";

        /// <summary>
        /// 程序编码，对应程序名称在应用平台上的唯一标识
        /// </summary>
        public static readonly int applicationCode = 0;

        /// <summary>
        /// 全局环境参数映射表
        /// </summary>
        private static readonly IDictionary<string, string> s_variables = new Dictionary<string, string>();

        /// <summary>
        /// 设置环境成员属性的值，通过查找与指定字符串相匹配的成员属性设定其对应值
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        /// <param name="fieldValue">属性值</param>
        public static void SetProperty(string fieldName, object fieldValue)
        {
            SystemType type = typeof(Environment);
            SystemFieldInfo field = type.GetField(fieldName, SystemBindingFlags.Static | SystemBindingFlags.Public);
            if (null == field)
            {
                Logger.Error("Could not found Environment field name '{%s}', set target property value failed.", fieldName);
                return;
            }

            field.SetValue(null, fieldValue);
        }

        /// <summary>
        /// 获取环境成员属性的值，通过查找与指定字符串相匹配的成员属性获取其对应值
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        /// <returns>获取属性名称的对应值</returns>
        public static object GetProperty(string fieldName)
        {
            SystemType type = typeof(Environment);
            SystemFieldInfo field = type.GetField(fieldName, SystemBindingFlags.Public | SystemBindingFlags.Static);
            if (null == field)
            {
                Logger.Error("Could not found Environment field name '{%s}', get target property value failed.", fieldName);
                return null;
            }

            return field.GetValue(null);
        }

        /// <summary>
        /// 检查当前环境成员属性是否有字段与目标串相匹配的名称
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        /// <returns>若存在目标字段名称则返回true，否则返回false</returns>
        public static bool IsPropertyExists(string fieldName)
        {
            SystemType type = typeof(Environment);
            SystemFieldInfo field = type.GetField(fieldName, SystemBindingFlags.Public | SystemBindingFlags.Static);
            return (null != field);
        }

        /// <summary>
        /// 清掉全局环境参数
        /// </summary>
        public static void CleanupAllVariables()
        {
            s_variables.Clear();
        }

        /// <summary>
        /// 设置全局环境参数，通过指定键值对映射
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <param name="value">环境参数值</param>
        public static void SetVariable(string key, string value)
        {
            if (s_variables.ContainsKey(key))
            {
                Logger.Info("The environment variable key {%s} was already exists, repeat setting it will be override old value.", key);

                s_variables[key] = value;
            }
            else
            {
                s_variables.Add(key, value);
            }
        }

        /// <summary>
        /// 通过指定键名获取对应的环境参数值
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <returns>若存在指定环境参数则返回对应值，否则返回null</returns>
        public static string GetVariable(string key)
        {
            if (s_variables.ContainsKey(key))
            {
                return s_variables[key];
            }

            return null;
        }

        /// <summary>
        /// 设置环境参数，通过指定键值对映射
        /// 若参数键存在对应的属性，则进行属性设置，具体函数可参考<see cref="NovaEngine.Environment.SetProperty(string,object)"/>
        /// 否则添加到全局参数表中，具体函数可参考<see cref="NovaEngine.Environment.SetVariable(string,string)"/>
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <param name="value">环境参数值</param>
        public static void SetValue(string key, object value)
        {
            if (IsPropertyExists(key))
            {
                SetProperty(key, value);
            }
            else
            {
                SetVariable(key, value.ToString());
            }
        }

        /// <summary>
        /// 通过指定键名获取对应的环境参数值
        /// 该方法将优先搜索对象属性，若存在同名属性则返回属性值
        /// 若同名属性查找失败，则搜索全局参数表，具体函数可参考<see cref="NovaEngine.Environment.GetVariable(string)"/>
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <returns>若存在指定环境参数则返回对应值，否则返回null</returns>
        public static string GetValue(string key)
        {
            if (IsPropertyExists(key))
            {
                // 此处应有非空检查
                return GetProperty(key).ToString();
            }
            else
            {
                return GetVariable(key);
            }
        }

        /// <summary>
        /// 以文件的形式加载配置参数
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <returns>从指定文件中加载配置参数成功返回true，否则返回false</returns>
        public static bool LoadFromFile(string filename)
        {
            string text = Utility.Path.LoadTextAsset(filename);
            if (null == text)
            {
                return false;
            }

            return LoadFromText(text);
        }

        /// <summary>
        /// 以文本数据的形式加载配置参数
        /// </summary>
        /// <param name="textAsset">文本字符串</param>
        /// <returns>从指定文本数据中加载配置参数成功返回true，否则返回false</returns>
        public static bool LoadFromText(string textAsset)
        {
            IO.IniFile file = IO.IniFile.Create();
            if (false == file.LoadFromText(textAsset))
            {
                file.Close();
                return false;
            }

            IDictionary<string, string> conf = file.GetSectionValue();

            file.Close();

            return Load(conf);
        }

        /// <summary>
        /// 从字典数据中加载配置参数
        /// </summary>
        /// <param name="conf">字典数据实例</param>
        /// <returns>从指定字典数据中加载配置参数成功返回true，否则返回false</returns>
        public static bool Load(IDictionary<string, string> conf)
        {
            SystemType type = typeof(Environment);
            foreach (KeyValuePair<string, string> pair in conf)
            {
                SystemFieldInfo field = type.GetField(pair.Key, SystemBindingFlags.Static | SystemBindingFlags.Public);
                if (null == field)
                {
                    // 非预定义属性直接放入全局配置参数表中
                    SetVariable(pair.Key, pair.Value);
                }
                else
                {
                    if (field.FieldType == typeof(bool))
                        SetProperty(pair.Key, Utility.Convertion.StringToBool(pair.Value));
                    else if (field.FieldType == typeof(int))
                        SetProperty(pair.Key, Utility.Convertion.StringToInt(pair.Value));
                    else if (field.FieldType == typeof(long))
                        SetProperty(pair.Key, Utility.Convertion.StringToLong(pair.Value));
                    else if (field.FieldType == typeof(float))
                        SetProperty(pair.Key, Utility.Convertion.StringToFloat(pair.Value));
                    else if (field.FieldType == typeof(double))
                        SetProperty(pair.Key, Utility.Convertion.StringToDouble(pair.Value));
                    else if (field.FieldType == typeof(string))
                        SetProperty(pair.Key, pair.Value);
                    else // 非预定义属性类型，暂时放在全局环境中备用
                    {
                        Logger.Warn("The Environment property '{%s}' field type '{%f}' parse failed.", field.Name, field.FieldType);
                        SetVariable(pair.Key, pair.Value);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 重置当前设定的全部环境参数
        /// </summary>
        public static void Unload()
        {
            SetProperty(nameof(editorMode), false);
            SetProperty(nameof(debugMode), false);
            SetProperty(nameof(debugLevel), 0);
            SetProperty(nameof(cryptMode), false);
            SetProperty(nameof(applicationName), "unknown");
            SetProperty(nameof(applicationCode), 0);

            CleanupAllVariables();
        }

        public static string PrintString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Environment = { PROPERTIES = { ");
            System.Reflection.FieldInfo[] fields = typeof(Environment).GetFields();
            for (int n = 0; n < fields.Length; ++n)
            {
                System.Reflection.FieldInfo field = fields[n];
                sb.AppendFormat("{0} = {1}, ", field.Name, field.GetValue(null));
            }

            sb.Append(" }, VARIABLES = { ");
            foreach (KeyValuePair<string, string> pair in s_variables)
            {
                sb.AppendFormat("{0} = {1}, ", pair.Key, pair.Value);
            }

            sb.Append(" }, ");

            // 打印环境配置同时，后面加入版本信息
            sb.AppendFormat("FRAMEWORK_VERSION = {0}, ", Version.FrameworkVersionName());
            sb.AppendFormat("APPLICATION_VERSION = {0} ", Version.ApplicationVersionName());

            sb.Append(" } }");
            return sb.ToString();
        }
    }
}
