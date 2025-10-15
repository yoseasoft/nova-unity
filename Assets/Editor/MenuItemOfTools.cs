using System.IO;
using UnityEditor;

using CoreEngine.Editor;
using GenerateConfig;
using GenerateProtobuf;
using SVNUnityExtension;

/// <summary>
/// Tools菜单
/// </summary>
static class MenuItemOfTools
{
    /// <summary>
    /// 协议目录设置
    /// </summary>
    [MenuItem("Tools/协议目录设置", priority = 1001)]
    static void SetProtobufFilePath()
    {
        ProtoPathSettingWindow.Open();
    }

    /// <summary>
    /// 配置目录设置
    /// </summary>
    [MenuItem("Tools/Excel目录设置", priority = 1002)]
    static void SetConfigFilePath()
    {
        ConfigPathSettingWindow.Open();
    }

    /// <summary>
    /// 编译代码
    /// </summary>
    [MenuItem("Tools/编译代码 _F6", priority = 2001)]
    static void CompileDll()
    {
        AssemblyCompile.CompileDlls();
    }

    /// <summary>
    /// 代码自动编译开关
    /// </summary>
    [MenuItem("Tools/自动编译?", priority = 2002)]
    static void SwitchAutoCompile()
    {
        bool isAutoCompile = AssemblyCompile.IsAutoCompile;
        isAutoCompile = !isAutoCompile;
        AssemblyCompile.IsAutoCompile = isAutoCompile;
        Menu.SetChecked("Tools/自动编译?", isAutoCompile);
    }

    /// <summary>
    /// 启用唯一程序集编译方式开关
    /// </summary>
    [MenuItem("Tools/编译名字唯一?", priority = 2003)]
    static void SwitchCompileNameUnique()
    {
        bool enableUniqueCompile = AssemblyCompile.IsUniqueCompile;
        enableUniqueCompile = !enableUniqueCompile;
        AssemblyCompile.IsUniqueCompile = enableUniqueCompile;
        Menu.SetChecked("Tools/编译名字唯一?", enableUniqueCompile);
    }

    /// <summary>
    /// Unity启动时设置勾选
    /// </summary>
    [InitializeOnLoadMethod]
    static void SetAutoCompileMenuChecked()
    {
        EditorApplication.delayCall += () =>
        {
            Menu.SetChecked("Tools/自动编译?", AssemblyCompile.IsAutoCompile);
            Menu.SetChecked("Tools/编译名字唯一?", AssemblyCompile.IsUniqueCompile);
        };
    }

    /// <summary>
    /// 生成协议
    /// </summary>
    [MenuItem("Tools/生成协议", priority = 2004)]
    static void Proto2Cs()
    {
        Proto2CsHelper.Start();
    }

    /// <summary>
    /// 生成协议
    /// </summary>
    [MenuItem("Tools/更新&&生成协议", priority = 2005)]
    static void UpdateAndProto2Cs()
    {
        string protobufPath = Proto2CsHelper.ProtobufPath;
        if (string.IsNullOrEmpty(protobufPath))
            return;

        SvnHelper.Update(protobufPath, Proto2CsHelper.Start);
    }

    /// <summary>
    /// 配置转换
    /// </summary>
    [MenuItem("Tools/导出Excel _F1", priority = 2101)]
    static void GenerateConfig()
    {
        bool genQuickly = EditorPrefs.GetBool(GenerateConfigHelper.IsGenLocalConfigQuicklyKey);
        if (genQuickly)
        {
            GenerateConfigHelper.StartQuickly();
        }
        else
        {
            GenerateConfigHelper.Start();
        }
    }

    /// <summary>
    /// 更新配置并进行转换
    /// </summary>
    [MenuItem("Tools/更新&&导出Excel _F2", priority = 2102)]
    static void UpdateAndGenerateConfig()
    {
        string configPath = GenerateConfigHelper.ConfigPath;
        if (string.IsNullOrEmpty(configPath))
            return;

        SvnHelper.Update(configPath, GenerateConfigHelper.Start);
    }

    /// <summary>
    /// 提交导出的后端配置
    /// </summary>
    [MenuItem("Tools/提交后端配置 _F3", priority = 2103)]
    static void CommitServerConfig()
    {
        string configPath = GenerateConfigHelper.ConfigPath;
        if (string.IsNullOrEmpty(configPath))
            return;

        string outputConfigPath = Path.Combine(configPath, "Output");
        if (Directory.Exists(outputConfigPath))
        {
            SvnHelper.Commit(outputConfigPath);
        }
    }

    /// <summary>
    /// 是否顺便导出服务器配置
    /// </summary>
    [MenuItem("Tools/导出服务器配置?", priority = 2104)]
    static void SwitchServerConfigGeneration()
    {
        bool isGenerateServer = EditorPrefs.GetBool(GenerateConfigHelper.IsGenerateServerConfigKey);
        isGenerateServer = !isGenerateServer;
        EditorPrefs.SetBool(GenerateConfigHelper.IsGenerateServerConfigKey, isGenerateServer);
        Menu.SetChecked("Tools/导出服务器配置?", isGenerateServer);
    }

    /// <summary>
    /// 是否启用本地快速转表
    /// 备忘:启用后按F1会跳过逻辑检查和服务端配置转换
    /// </summary>
    [MenuItem("Tools/本地快速转表?", priority = 2105)]
    static void SwitchGenLocalConfigQuickly()
    {
        bool genQuickly = EditorPrefs.GetBool(GenerateConfigHelper.IsGenLocalConfigQuicklyKey);
        genQuickly = !genQuickly;
        EditorPrefs.SetBool(GenerateConfigHelper.IsGenLocalConfigQuicklyKey, genQuickly);
        Menu.SetChecked("Tools/本地快速转表?", genQuickly);
    }

    [MenuItem("HybridCLR/CopyAotDlls")]
    public static void CopyAotDlls()
    {
        HybridCLREditor.CopyAotDlls();
    }

    /// <summary>
    /// Unity启动时设置勾选
    /// </summary>
    [InitializeOnLoadMethod]
    static void SetGenLocalConfigQuicklyMenuChecked()
    {
        EditorApplication.delayCall += () =>
        {
            Menu.SetChecked("Tools/导出服务器配置?", EditorPrefs.GetBool(GenerateConfigHelper.IsGenerateServerConfigKey));
            Menu.SetChecked("Tools/本地快速转表?", EditorPrefs.GetBool(GenerateConfigHelper.IsGenLocalConfigQuicklyKey));
        };
    }
}
