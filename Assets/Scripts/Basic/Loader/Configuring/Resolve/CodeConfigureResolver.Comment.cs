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

using SystemType = System.Type;

using SystemXmlNode = System.Xml.XmlNode;
using SystemXmlNodeType = System.Xml.XmlNodeType;

namespace GameEngine.Loader.Configuring
{
    /// <summary>
    /// 对象数据的配置解析类，对外部配置数据的结构信息进行解析和构建
    /// </summary>
    internal static partial class CodeConfigureResolver
    {
        /// <summary>
        /// 加载基础Bean节点的配置数据
        /// </summary>
        /// <param name="node">节点实例</param>
        /// <remarks>返回配置数据的对象实例</remarks>
        [CodeLoader.OnConfigureResolvingCallback(SystemXmlNodeType.Comment, ConfigureNodeName.Comment)]
        private static BaseConfigureInfo LoadGeneralComments(SystemXmlNode node)
        {
            Debugger.Info(LogGroupTag.CodeLoader, "Load general configure comment: {0}", node.Value);
            return null;
        }
    }
}
