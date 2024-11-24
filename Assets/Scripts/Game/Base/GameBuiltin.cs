/// <summary>
/// 2023-08-31 Game Framework Code By Hurley
/// </summary>

namespace Game
{
    /// <summary>
    /// 程序内置的框架接口封装对象类，对引擎底层的接口类进行封装引用
    /// </summary>
    public static class NE
    {
        public static GameEngine.TimerHandler    TimerHandler    => GameEngine.TimerHandler.Instance;
        public static GameEngine.ThreadHandler   ThreadHandler   => GameEngine.ThreadHandler.Instance;
        public static GameEngine.TaskHandler     TaskHandler     => GameEngine.TaskHandler.Instance;
        public static GameEngine.NetworkHandler  NetworkHandler  => GameEngine.NetworkHandler.Instance;
        public static GameEngine.ResourceHandler ResourceHandler => GameEngine.ResourceHandler.Instance;
        public static GameEngine.FileHandler     FileHandler     => GameEngine.FileHandler.Instance;
        public static GameEngine.InputHandler    InputHandler    => GameEngine.InputHandler.Instance;
        public static GameEngine.SceneHandler    SceneHandler    => GameEngine.SceneHandler.Instance;
        public static GameEngine.ObjectHandler   ObjectHandler   => GameEngine.ObjectHandler.Instance;
        public static GameEngine.GuiHandler      GuiHandler      => GameEngine.GuiHandler.Instance;
        public static GameEngine.SoundHandler    SoundHandler    => GameEngine.SoundHandler.Instance;
    }
}
