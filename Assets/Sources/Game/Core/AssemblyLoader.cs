/// <summary>
/// 2023-08-29 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    public static class AssemblyLoader
    {
        private readonly static string[] WaitingLoadAssemblyNames = { "Agen", "Game", "GameHotfix" };

        public static void Load()
        {
            LoadAllAssemblies();
        }

        public static void Unload()
        {
        }

        public static void Reload()
        {
            LoadAllAssemblies(true);
        }

        /// <summary>
        /// 加载所有程序集
        /// </summary>
        private static void LoadAllAssemblies(bool reload = false)
        {
            /**
             * System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
             * foreach (System.Reflection.Assembly assembly in assemblies)
             * {
             *     GameEngine.CodeLoader.LoadFromAssembly(assembly);
             * }
             */

            for (int n = 0; n < WaitingLoadAssemblyNames.Length; ++n)
            {
                // System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(WaitingLoadAssemblyNames[n]);
                // System.Reflection.Assembly assembly = AppEngine.AppStart.GetLoadedAssembly(WaitingLoadAssemblyNames[n]);
                System.Reflection.Assembly assembly = NovaEngine.Utility.Assembly.GetAssembly(WaitingLoadAssemblyNames[n]);
                if (null == assembly)
                {
                    Debugger.Error("Could not found any assembly from target name '{%s}'.", WaitingLoadAssemblyNames[n]);
                    continue;
                }

                GameEngine.GameLibrary.LoadFromAssembly(assembly, reload);
            }
        }
    }
}
