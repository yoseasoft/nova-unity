/// <summary>
/// 2017-10-09 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 上下文属性定义类，对外提供当前引擎的上下文属性值，及系统预定值
    /// </summary>
    public static class AppContext
    {
        //
        // The GameObject layer mask
        //
        public const int GAME_OBJECT_LAYER_DEFAULT       =  0;
        public const int GAME_OBJECT_LAYER_GUI           = 11;
        public const int GAME_OBJECT_LAYER_GUI_MODEL     = 12;
        public const int GAME_OBJECT_LAYER_TERRAIN       = 13;
        public const int GAME_OBJECT_LAYER_GROUND        = 14;
        public const int GAME_OBJECT_LAYER_FLOOR         = 15;
        public const int GAME_OBJECT_LAYER_BUILD         = 16;
        public const int GAME_OBJECT_LAYER_OBSTRUCTOR    = 17;
        public const int GAME_OBJECT_LAYER_SKYBOX        = 18;
        public const int GAME_OBJECT_LAYER_WEATHER       = 19;
        public const int GAME_OBJECT_LAYER_PARTICLE      = 20;
        public const int GAME_OBJECT_LAYER_HERO          = 21;
        public const int GAME_OBJECT_LAYER_FAIRY         = 22;
        public const int GAME_OBJECT_LAYER_NPC           = 23;
        public const int GAME_OBJECT_LAYER_MONSTER       = 24;
        public const int GAME_OBJECT_LAYER_BOSS          = 25;
        public const int GAME_OBJECT_LAYER_SCENE_HERO    = 26;
        public const int GAME_OBJECT_LAYER_SCENE_FAIRY   = 27;
        public const int GAME_OBJECT_LAYER_SCENE_NPC     = 28;
        public const int GAME_OBJECT_LAYER_SCENE_MONSTER = 29;
        public const int GAME_OBJECT_LAYER_SCENE_BOSS    = 30;
        public const int GAME_OBJECT_LAYER_SCENE_VIEW    = 31;

        //
        // The GameObject tag name
        //
        public const string GAME_OBJECT_TAG_CAMERA       = "Camera";
        public const string GAME_OBJECT_TAG_MAIN_CAMERA  = "MainCamera";
        public const string GAME_OBJECT_TAG_PLAYER       = "Player";
        public const string GAME_OBJECT_TAG_GUI          = "Gui";
        public const string GAME_OBJECT_TAG_GUI_MODEL    = "GuiModel";
        public const string GAME_OBJECT_TAG_TERRAIN      = "Terrain";
        public const string GAME_OBJECT_TAG_GROUND       = "Ground";
        public const string GAME_OBJECT_TAG_FLOOR        = "Floor";
        public const string GAME_OBJECT_TAG_BUILD        = "Build";
        public const string GAME_OBJECT_TAG_OBSTRUCTOR   = "Obstructor";
        public const string GAME_OBJECT_TAG_SKYBOX       = "Skybox";
        public const string GAME_OBJECT_TAG_WEATHER      = "Weather";
        public const string GAME_OBJECT_TAG_PARTICLE     = "Particle";
        public const string GAME_OBJECT_TAG_HERO         = "Hero";
        public const string GAME_OBJECT_TAG_FAIRY        = "Fairy";
        public const string GAME_OBJECT_TAG_NPC          = "Npc";
        public const string GAME_OBJECT_TAG_MONSTER      = "Monster";
        public const string GAME_OBJECT_TAG_BOSS         = "Boss";
        public const string GAME_OBJECT_TAG_VIEW         = "View";

        //
        // The file suffix name
        //
        public static class SUFFIX_NAME
        {
            public const string PNG = ".png";
            public const string DDS = ".dds";
            public const string MAT = ".mat";
            public const string FBX = ".fbx";
            public const string CONTROLLER = ".controller";
            public const string PREFAB = ".prefab";
            public const string MP3 = ".mp3";
            public const string MP4 = ".mp4";
            public const string OGG = ".ogg";
            public const string UNITY = ".unity";
            public const string UNITY3D = ".unity3d";
            public const string CS = ".cs";
            public const string JS = ".js";
            public const string LUA = ".lua";
            public const string BYTES = ".bytes";
            public const string SHADER = ".shader";
            public const string PB = ".pb";
            public const string DLL = ".dll";
            public const string SO = ".so";
            public const string A = ".a";
            public const string TXT = ".txt";
            public const string INI = ".ini";
            public const string DAT = ".dat";
            public const string CSV = ".csv";
            public const string XML = ".xml";
            public const string JSON = ".json";
            public const string PLIST = ".plist";
        }

        ///
        /// The standard directory name
        ///
        public static class STANDARD_DIRECTORY_NAME
        {
            public const string CONFIG = "conf";
            public const string DATA = "data";
            public const string CACHE = "cache";
            public const string FONT = "font";
            public const string SHADER = "shader";
            public const string MATERIAL = "material";
            public const string TEXTURE = "texture";
            public const string SOUND = "sound";
            public const string CAMERA = "camera";
            public const string SCENE = "scene";
            public const string SKYBOX = "skybox";
            public const string UI = "ui";
            public const string MAP = "map";
            public const string MODEL = "model";
            public const string EFFECT = "effect";
            public const string LIGHT = "light";
            public const string AUDIO = "audio";
            public const string VIDEO = "video";
            public const string PARTICLE = "particle";
        }
    }
}
