using GameEngine;

namespace Game
{
    /// <summary>
    /// 移动组件对象类
    /// </summary>
    [GameEngine.PoolSupported]
    public class MoveComponent : CComponent, GameEngine.IEventActivation, GameEngine.IUpdateActivation
    {
        public MoveComponent()
        {
            Debugger.Warn("MoveComponent Construct Method Running ...");
        }

        ~MoveComponent()
        {
            Debugger.Warn("MoveComponent Destruct Method Running ...");
        }

        public void MoveTo(UnityEngine.Vector3 position)
        {
            TransformComponent attr = GetComponent<TransformComponent>();
            if (null != attr)
            {
                attr.position = position;
            }
        }

        [GameEngine.EventSubscribeBindingOfTarget(1201)]
        public void OnBiuBiuBiu(int eventID, params object[] args)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendFormat("测试'{0} - {1}'对象监听事件标识'{2}'的带参普通成员函数'OnBiuBiuBiu'调用, 参数值:",
                    Entity.GetType().FullName, GetType().FullName, eventID);
            for (int n = 0; null != args && n < args.Length; ++n)
            {
                sb.AppendFormat("[{0}] = {1}, ", n, args[n].ToString());
            }
            sb.Append("}.");
            Debugger.Log(sb.ToString());
        }

        [GameEngine.EventSubscribeBindingOfTarget(1202)]
        public void OnBiuBiuBiuWithNullParameter()
        {
            Debugger.Log("测试'{0} - {1}'对象监听事件标识'1202'的无参普通成员函数'OnBiuBiuBiuWithNullParameter'调用.",
                    Entity.GetType().FullName, GetType().FullName);
        }
    }
}
