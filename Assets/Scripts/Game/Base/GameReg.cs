/// <summary>
/// 2023-08-29 Game Framework Code By Hurley
/// </summary>

using System.IO;

namespace Game
{
    /// <summary>
    /// 程序的注册管理容器封装对象类，对业务层的对象类进行注册绑定操作
    /// </summary>
    public static class GameReg
    {
        private readonly static string[] WaitingLoadAssemblyNames = { "Agen", "Game", "GameHotfix" };

        /// <summary>
        /// 类型注册绑定函数
        /// </summary>
        public static void LoadClass()
        {
            // System.Type[] allTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

            // string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            // Debugger.Log("Assembly class name: {0}, total class count: {1}", assemblyName, allTypes.Length);

            LoadAllConfigures();

            LoadAllAssemblies();
        }

        /// <summary>
        /// 类型注销解绑函数
        /// </summary>
        public static void UnloadClass()
        {
        }

        public static void ReloadClass()
        {
            LoadAllAssemblies(true);
        }

        private static void LoadAllConfigures()
        {
            //XmlDocument document = new XmlDocument();
            //MemoryStream mstream = new MemoryStream();

            //using (FileStream fs = new FileStream(NovaEngine.Utility.Resource.ApplicationDataPath + "/Resources/configure/bean.xml", FileMode.Open, FileAccess.Read))
            //{
            //    byte[] bytes = new byte[fs.Length];
            //    fs.Read(bytes, 0, bytes.Length);
            //    fs.Close();

            //    GameEngine.Loader.CodeLoader.LoadFromConfigure(bytes, 0, bytes.Length);

            //    mstream.Write(bytes, 0, bytes.Length);
            //    mstream.Seek(0, SeekOrigin.Begin);
            //    Debugger.Log($"mstream length = {mstream.Length}, {mstream.Position}, bytes length = {bytes.Length}");
            //    document.Load(mstream);
            //}

            GameEngine.GameLibrary.LoadFromConfigure(NovaEngine.Utility.Resource.ApplicationDataPath + "/Resources/configure/bean.xml");

            //mstream.Dispose();
            //XmlNodeList nodes = document.DocumentElement.ChildNodes;
            //foreach (XmlNode node in nodes)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append(node.Name);
            //    sb.Append("[");
            //    foreach (XmlAttribute attr in node.Attributes)
            //    {
            //        sb.AppendFormat("{0}:{1},", attr.Name, attr.Value);
            //    }
            //    sb.Append("]");
            //    Debugger.Log(sb.ToString());
            //}
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
                System.Reflection.Assembly assembly = AppEngine.AppStart.GetLoadedAssembly(WaitingLoadAssemblyNames[n]);
                if (null == assembly)
                {
                    Debugger.Error("Could not found any assembly from target name '{0}'.", WaitingLoadAssemblyNames[n]);
                    continue;
                }

                GameEngine.GameLibrary.LoadFromAssembly(assembly, reload);
            }
        }
    }
}
