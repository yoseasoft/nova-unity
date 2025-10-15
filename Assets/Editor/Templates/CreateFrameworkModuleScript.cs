/// -------------------------------------------------------------------------------
/// NovaEngine Editor Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using UnityEditor;
using UnityEngine;

namespace NovaEngine.Editor
{
    /// <summary>
    /// 创建基于框架模块的自定义脚本文件的定义类
    /// </summary>
    public class CreateFrameworkModuleScript : ScriptableObject
    {
        //[MenuItem("Nova Framework/Script/Proto/Scene Script", false, 11010)]
        //[MenuItem("Assets/Create/Nova Framework/Script/Proto/Scene Script", false, 11010)]
        //static void CreateSceneScripte()
        //{
        //    string path = Utility.Path.GetRegularPath(Utility.Path.TruncatePath(GetScriptPath(), 1)) + "/Proto/NewSceneTemplate.cs.txt";

        //    ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "NewScene.cs");
        //}

        private static string GetScriptPath()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(CreateInstance<CreateFrameworkModuleScript>());

            // 获取脚本在 Assets 中的相对路径
            string scriptRelativePath = AssetDatabase.GetAssetPath(monoScript);

            Debug.LogWarning($"script relative path = {scriptRelativePath}");

            return scriptRelativePath;
        }
    }
}
