/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-19
/// 功能描述：
/// </summary>

using UnityEngine;

namespace Game
{
    /// <summary>
    /// 移动组件逻辑
    /// </summary>
    public static class HardHurtComponentSystem
    {
        public static void OnHurt(this HardHurtComponent self, int damage)
        {
            AttributeComponent attributeComponent = self.GetComponent<AttributeComponent>();
            IdentityComponent identityComponent = self.GetComponent<IdentityComponent>();
            attributeComponent.health -= damage;
            if (attributeComponent.health <= 0)
            {
                attributeComponent.health = 0;
                attributeComponent.is_die = true;

                GameEngine.GameApi.Send(new BattleActorDieNotify() { uid = identityComponent.objectId });
            }
            else
            {
                GameEngine.GameApi.Send(new BattleActorHurtNotify() { uid = identityComponent.objectId });
            }
        }
    }
}
