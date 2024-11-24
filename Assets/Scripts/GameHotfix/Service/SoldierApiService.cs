/// <summary>
/// 2024-04-16 Game Framework Code By Hurley
/// </summary>

using UnityEngine;

namespace Game
{
    /// <summary>
    /// 对象api服务
    /// </summary>
    public static class SoldierApiService
    {
        public static void DoSoldierEmpty()
        {
            Debugger.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!_____________________________________________");
        }

        public static int DoSoldierAttack(Soldier soldier, int attackCount)
        {
            int result = 0;
            AttributeComponent attributeComponent = soldier.GetComponent<AttributeComponent>();
            if (null != attributeComponent)
            {
                result = attributeComponent.attack * attackCount;
            }

            return result;
        }
    }
}
