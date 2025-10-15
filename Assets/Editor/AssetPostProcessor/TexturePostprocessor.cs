using UnityEngine;
using UnityEditor;
using System.Reflection;

/// <summary>
/// 纹理导入处理
/// </summary>
public class TexturePostprocessor : AssetPostprocessor
{
    public static bool isTextureChecker = false;

    private const int AndroidDefaultMaxSize = 512;
    private const int WebGLDefaultMaxSize = 512;
    private const string StrTerrainControlTexPostfix = "_TCT";
    private const string StrBigTexPostfix = "_Big";
    private const string StrBigSizeTexPostfix = "_BigSize";

    // 纹理导入之前调用
    public void OnPreprocessTexture()
    {
        if (assetPath.StartsWith("Assets/_Resources/Gui/") || assetPath.StartsWith("Assets/_Resources/Texture/"))
        {
            ProcessUITexture();
        }
        else if (assetPath.StartsWith("Assets/_Resources"))
        {
            ProcessCommonTexture();
        }
    }

    /// <summary>
    /// 处理UI纹理
    /// (此项目处理方式为:关闭mipmap和针对平台设置图片格式,其他参数保持默认值不进行设置)
    /// </summary>
    void ProcessUITexture()
    {
        TextureImporter import = assetImporter as TextureImporter;

        if (import.textureType != TextureImporterType.Sprite)
        {
            // UI贴图不需要进行2次幂处理
            import.npotScale = TextureImporterNPOTScale.None;
        }

        import.mipmapEnabled = false;
        import.filterMode = FilterMode.Bilinear;

        // 透明贴图处理
        if (import.DoesSourceTextureHaveAlpha())
        {
            import.alphaIsTransparency = true;
        }

        // 平台设置
        // windows
        var windows = import.GetPlatformTextureSettings("Standalone");
        windows.overridden = true;
        windows.format = TextureImporterFormat.BC7;

        // android
        var android = new TextureImporterPlatformSettings { name = "Android", overridden = true, format = TextureImporterFormat.ASTC_6x6 };

        // ios
        var ios = new TextureImporterPlatformSettings { name = "iPhone", overridden = true, format = TextureImporterFormat.ASTC_6x6 };

        import.SetPlatformTextureSettings(windows);
        import.SetPlatformTextureSettings(android);
        import.SetPlatformTextureSettings(ios);
    }

    /// <summary>
    /// 通用处理贴图方法
    /// </summary>
    void ProcessCommonTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;

        string texName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        bool isBigTex = texName.EndsWith(StrBigTexPostfix);
        bool isBigSizeTex = texName.EndsWith(StrBigSizeTexPostfix);
        if (isBigSizeTex)
        {
            isBigTex = true;
        }

        bool isTerrainControlTex = texName.EndsWith(StrTerrainControlTexPostfix);
        if (!isTextureChecker && !isTerrainControlTex)
        {
            importer.isReadable = false;
        }

        if (importer.textureType != TextureImporterType.Sprite)
        {
            importer.npotScale = IsPowerOfTwo(importer) ? TextureImporterNPOTScale.None : TextureImporterNPOTScale.ToNearest;
        }

#if UNITY_ANDROID || UNITY_WEBGL
            importer.mipmapEnabled = false;
#endif

        TextureImporterPlatformSettings settings = importer.GetDefaultPlatformTextureSettings();
        importer.SetPlatformTextureSettings(settings);

        // Windows
        settings = importer.GetPlatformTextureSettings("Standalone");
        ProcessWindowsCommonTextureMaxSize(settings, importer);
        settings.overridden = true;

        // HDR贴图(后缀.hdr or .exr)
        if (assetPath.EndsWith(".hdr") || assetPath.EndsWith(".exr"))
            settings.format = TextureImporterFormat.BC6H;
        else
        {
            settings.format =
                (importer.DoesSourceTextureHaveAlpha() && importer.alphaSource != TextureImporterAlphaSource.None) ||
                importer.textureType == TextureImporterType.NormalMap
                    ? TextureImporterFormat.DXT5
                    : TextureImporterFormat.DXT1;

            if (isTerrainControlTex)
            {
                settings.format =
                    (importer.DoesSourceTextureHaveAlpha() && importer.alphaSource != TextureImporterAlphaSource.None)
                        ? TextureImporterFormat.RGBA32
                        : TextureImporterFormat.DXT1;
            }
        }

        importer.mipmapEnabled = false;
        importer.SetPlatformTextureSettings(settings);

        // Android
        settings = importer.GetPlatformTextureSettings("Android");
        ProcessMobileCommonTextureMaxSize(settings, importer, isBigSizeTex);
        settings.overridden = true;
        settings.allowsAlphaSplitting = false;
        if (isTerrainControlTex)
        {
            settings.format = TextureImporterFormat.RGB24;
            settings.maxTextureSize = texName.Contains(StrBigTexPostfix) ? 512 : 256;
        }
        else
        {
            if (isBigTex)
            {
                settings.format = isBigSizeTex ? TextureImporterFormat.ASTC_6x6 : TextureImporterFormat.ASTC_4x4;
            }
            else
            {
                settings.format = TextureImporterFormat.ASTC_12x12;
            }
        }
        importer.SetPlatformTextureSettings(settings);

        // iOS
        settings = importer.GetPlatformTextureSettings("iPhone");
        ProcessMobileCommonTextureMaxSize(settings, importer);
        settings.overridden = true;
        settings.allowsAlphaSplitting = false;
        if (isTerrainControlTex)
        {
            settings.format = TextureImporterFormat.ASTC_4x4;
        }
        else
        {
            settings.format = isBigTex ? TextureImporterFormat.ASTC_6x6 : TextureImporterFormat.ASTC_12x12;
        }

        importer.SetPlatformTextureSettings(settings);

        // WebGL
        settings = importer.GetPlatformTextureSettings("WebGL");
        ProcessWebGLCommonTextureMaxSize(settings, importer);
        settings.overridden = true;
        settings.allowsAlphaSplitting = false;
        if (isTerrainControlTex)
        {
            settings.format = TextureImporterFormat.ASTC_4x4;
        }
        else
        {
            settings.format = isBigTex ? TextureImporterFormat.ASTC_6x6 : TextureImporterFormat.ASTC_12x12;
        }
        importer.SetPlatformTextureSettings(settings);
    }

    /// <summary>
    /// Windows平台的maxSize设置
    /// </summary>
    void ProcessWindowsCommonTextureMaxSize(TextureImporterPlatformSettings settings, TextureImporter importer)
    {
        // 获取Default的值, 对比下最大大小再设置, 和美术约定通用的话只设Default即可
        TextureImporterPlatformSettings defaultSettings = importer.GetPlatformTextureSettings("Default");
        int defaultSettingMaxSize = defaultSettings.maxTextureSize; // Default设置的MaxSize
        int textureMaxSize = GetTextureMaxSize(importer);           // 图片原大小的MaxSize

        // 图片原大小大于设置的最大大小时才需要设置
        if (textureMaxSize > defaultSettingMaxSize || textureMaxSize > settings.maxTextureSize)
        {
            // 平台的maxSize可以比Default小, 不能比Default大, 需要变大的话需要Default设置大, 各个平台可以设置更小
            if (settings.maxTextureSize > defaultSettingMaxSize)
            {
                settings.maxTextureSize = defaultSettingMaxSize;
            }
        }
    }

    /// <summary>
    /// 移动平台的maxSize设置
    /// </summary>
    void ProcessMobileCommonTextureMaxSize(TextureImporterPlatformSettings settings, TextureImporter importer, bool texBigSize = false)
    {
        // 获取Default的值, 对比下最大大小再设置, 和美术约定通用的话只设Default即可
        TextureImporterPlatformSettings defaultSettings = importer.GetPlatformTextureSettings("Default");
        int defaultSettingMaxSize = defaultSettings.maxTextureSize;
        int textureMaxSize = GetTextureMaxSize(importer);
        int newAndroidDefaultMaxSize = texBigSize ? AndroidDefaultMaxSize * 2 : AndroidDefaultMaxSize;
        if (defaultSettingMaxSize > newAndroidDefaultMaxSize || textureMaxSize > newAndroidDefaultMaxSize)
        {
            if (settings.maxTextureSize > newAndroidDefaultMaxSize)
            {
                settings.maxTextureSize = newAndroidDefaultMaxSize;
            }
        }
    }

    void ProcessWebGLCommonTextureMaxSize(TextureImporterPlatformSettings settings, TextureImporter importer)
    {
        // 获取Default的值, 对比下最大大小再设置, 和美术约定通用的话只设Default即可
        TextureImporterPlatformSettings defaultSettings = importer.GetPlatformTextureSettings("Default");
        int defaultSettingMaxSize = defaultSettings.maxTextureSize; // Default设置的MaxSize
        int textureMaxSize = GetTextureMaxSize(importer);           // 图片原大小的MaxSize

        if (defaultSettingMaxSize > WebGLDefaultMaxSize || textureMaxSize > WebGLDefaultMaxSize)
        {
            if (settings.maxTextureSize > WebGLDefaultMaxSize)
            {
                settings.maxTextureSize = WebGLDefaultMaxSize;
            }
        }
    }

    /// <summary>
    /// 获取贴图本来的最大Size
    /// </summary>
    int GetTextureMaxSize(TextureImporter importer)
    {
        (int width, int height) = GetTextureImporterSize(importer);
        int maxSize = assetPath.Contains("Effect/Ui") ? 1024 : 512;
        if (IsPowerOfTwo(importer))
            maxSize = Mathf.Max(width, height);
        return maxSize;
    }

    /// <summary>
    /// 判断指定贴图的长宽是否符合2的次幂
    /// </summary>
    static bool IsPowerOfTwo(TextureImporter importer)
    {
        (int width, int height) = GetTextureImporterSize(importer);
        return IsPowerOfTwo(width) && IsPowerOfTwo(height);
    }

    /// <summary>
    /// 判断一个数字是不是2的次幂
    /// </summary>
    static bool IsPowerOfTwo(int number)
    {
        if (number <= 0)
            return false;

        while (number % 2 == 0)
        {
            number /= 2;
        }

        return number == 1;
    }

    /// <summary>
    /// 获取导入图片的宽高
    /// </summary>
    static (int, int) GetTextureImporterSize(TextureImporter importer)
    {
        if (importer != null)
        {
            object[] args = new object[2];
            MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(importer, args);
            return ((int)args[0], (int)args[1]);
        }

        return (0, 0);
    }
}