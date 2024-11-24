using System;
using System.Text;
using NovaEngine;

namespace Game
{
    /// <summary>
    /// 世界管理器封装类
    /// </summary>
    public static class WorldManager
    {
        public static Soldier soldier = null;
        public static Monster monster = null;

        public static void InitWorldObjects()
        {
            RemoveSoldier();
            soldier = NE.ObjectHandler.CreateObject<Soldier>();
            {
                TransformComponent transformComponent = soldier.GetComponent<TransformComponent>();
                Debugger.Assert(transformComponent != null, "Invalid component.");

                transformComponent.position = UnityEngine.Vector3.zero;
                transformComponent.rotation = UnityEngine.Vector3.zero;
                transformComponent.scale = UnityEngine.Vector3.zero;

                AttributeComponent attributeComponent = soldier.GetComponent<AttributeComponent>();
                Debugger.Assert(attributeComponent != null, "Invalid component.");

                attributeComponent.health = 100;
                attributeComponent.speed = 5;
                attributeComponent.attack = 10;
                attributeComponent.defense = 2;
            }

            RemoveMonster();
            monster = NE.ObjectHandler.CreateObject<Monster>();
            {
                TransformComponent transformComponent = monster.GetComponent<TransformComponent>();
                Debugger.Assert(transformComponent != null, "Invalid component.");

                transformComponent.position = UnityEngine.Vector3.zero;
                transformComponent.rotation = UnityEngine.Vector3.zero;
                transformComponent.scale = UnityEngine.Vector3.zero;

                AttributeComponent attributeComponent = monster.GetComponent<AttributeComponent>();
                Debugger.Assert(attributeComponent != null, "Invalid component.");

                attributeComponent.health = 30;
                attributeComponent.speed = 3;
                attributeComponent.attack = 5;
                attributeComponent.defense = 1;
            }
        }

        public static void RemoveSoldier()
        {
            if (null != soldier)
            {
                NE.ObjectHandler.DestroyObject(soldier);
                soldier = null;
            }
        }

        public static void RemoveMonster()
        {
            if (null != monster)
            {
                NE.ObjectHandler.DestroyObject(monster);
                monster = null;
            }
        }

        public static void CleanupWorldObjects()
        {
            RemoveSoldier();
            RemoveMonster();
        }

        public static void PrintAllObjects()
        {
            if (null != soldier) PrintObject(soldier);
            if (null != monster) PrintObject(monster);
        }

        private static void PrintObject(Actor actor)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{actor.GetType().FullName} = {{ ");

            TransformComponent transformComponent = actor.GetComponent<TransformComponent>();
            if (null != transformComponent)
            {
                sb.Append("Transform = { ");
                sb.Append($"Position = {{ x = {transformComponent.position.x}, y = {transformComponent.position.y}, z = {transformComponent.position.z} }}, ");
                sb.Append($"Rotation = {{ x = {transformComponent.rotation.x}, y = {transformComponent.rotation.y}, z = {transformComponent.rotation.z} }}, ");
                sb.Append($"Scale = {{ x = {transformComponent.scale.x}, y = {transformComponent.scale.y}, z = {transformComponent.scale.z} }}, ");
                sb.Append("}, ");
            }

            AttributeComponent attributeComponent = actor.GetComponent<AttributeComponent>();
            if (null != attributeComponent)
            {
                sb.Append("Attribute = { ");
                sb.Append("Attribute = { ");
                sb.Append($"health = {attributeComponent.health}, ");
                sb.Append($"speed = {attributeComponent.speed}, ");
                sb.Append($"attack = {attributeComponent.attack}, ");
                sb.Append($"defense = {attributeComponent.defense}, ");
                sb.Append("}, }, ");
            }

            sb.Append($"}}");

            Debugger.Info(sb.ToString());
        }
    }
}
