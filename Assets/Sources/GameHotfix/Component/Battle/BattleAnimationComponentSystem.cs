/// <summary>
/// Game Framework
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-18
/// 功能描述：
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 角色进入通知
    /// </summary>
    public struct BattleActorJoinNotify
    {
        public int uid;
        public string asset_url;
    }

    /// <summary>
    /// 角色离开通知
    /// </summary>
    public struct BattleActorLeaveNotify
    {
        public int uid;
        public int index;
    }

    /// <summary>
    /// 角色移动通知
    /// </summary>
    public struct BattleActorMoveToNotify
    {
        public int uid;
    }

    /// <summary>
    /// 角色攻击通知
    /// </summary>
    public struct BattleActorAttackNotify
    {
        public int uid;
        public string skill;
    }

    /// <summary>
    /// 角色受击通知
    /// </summary>
    public struct BattleActorHurtNotify
    {
        public int uid;
    }

    /// <summary>
    /// 角色死亡通知
    /// </summary>
    public struct BattleActorDieNotify
    {
        public int uid;
    }

    /// <summary>
    /// Battle场景动画组件逻辑处理类
    /// </summary>
    public static class BattleAnimationComponentSystem
    {
        [OnAwake]
        private static void Awake(this BattleAnimationComponent self)
        {
            self.camera = new GameObject("MAIN_CAMERA");
            self.camera.AddComponent<Camera>();
            self.camera.transform.position = new Vector3(4.5f, 4.5f, -1.5f);
            self.camera.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

            //self.light = new GameObject("MAIN_LIGHT");
            //self.light.AddComponent<Light>().type = LightType.Directional;
            //self.light.transform.position = Vector3.up * 10f;
            //self.light.transform.rotation = Quaternion.identity;

            BattleMapComponent map = self.GetComponent<BattleMapComponent>();

            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.white;

            self.grids = new GameObject[map.grid_count];
            for (int n = 0; n < map.grid_count; ++n)
            {
                GameObject go = new GameObject($"GO_{n}");
                go.transform.position = map.CalcGridPositionByIndex(n);

                MeshFilter meshFilter = go.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();

                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                meshRenderer.sharedMaterial = mat; // Resources.GetBuiltinResource<Material>("Default-Diffuse.mat");

                self.grids[n] = go;
            }

            self.models = new Dictionary<int, GameObject>();
        }

        [OnStart]
        private static void Start(this BattleAnimationComponent self)
        {
        }

        [OnUpdate]
        private static void Update(this BattleAnimationComponent self)
        {
        }

        [OnDestroy]
        private static void Destroy(this BattleAnimationComponent self)
        {
            GameObject.Destroy(self.camera);
            self.camera = null;

            for (int n = 0; n < self.grids.Length; ++n)
            {
                GameObject.Destroy(self.grids[n]);
                self.grids[n] = null;
            }
            self.grids = null;

            foreach (KeyValuePair<int, GameObject> kvp in self.models)
            {
                GameObject.Destroy(kvp.Value);
            }
            self.models.Clear();
        }

        [GameEngine.EventSubscribeBindingOfTarget(typeof(BattleActorJoinNotify))]
        private static void OnRecvEventByType(this BattleAnimationComponent self, BattleActorJoinNotify eventData)
        {
            BattleMapComponent map = self.GetComponent<BattleMapComponent>();
            int index = map.FindGridByUid(eventData.uid);
            if (index < 0)
            {
                Debugger.Error("没有找到该角色的位置？");
                return;
            }

            BattleLoadComponent load = self.GetComponent<BattleLoadComponent>();
            Actor actor = load.FindActorByUid(eventData.uid);

            TransformComponent transformComponent = actor.GetComponent<TransformComponent>();

            Object obj = GameEngine.ResourceHandler.Instance.LoadAsset(eventData.asset_url, typeof(GameObject));
            GameObject go = go = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity, self.grids[index].transform) as GameObject;
            go.transform.localPosition = transformComponent.position;
            go.transform.localRotation = Quaternion.Euler(transformComponent.rotation);
            self.models.Add(eventData.uid, go);

            Animator animator = go.GetComponent<Animator>();
            animator.Play("stand");

            GameEngine.ResourceHandler.Instance.UnloadAsset(obj);
        }

        [GameEngine.EventSubscribeBindingOfTarget(typeof(BattleActorLeaveNotify))]
        private static void OnRecvEventByType(this BattleAnimationComponent self, BattleActorLeaveNotify eventData)
        {
            GameObject go = self.models[eventData.uid];
            GameObject.Destroy(go);
        }

        [GameEngine.EventSubscribeBindingOfTarget(typeof(BattleActorMoveToNotify))]
        private static void OnRecvEventByType(this BattleAnimationComponent self, BattleActorMoveToNotify eventData)
        {
            BattleMapComponent map = self.GetComponent<BattleMapComponent>();
            int index = map.FindGridByUid(eventData.uid);
            if (index < 0)
            {
                Debugger.Error("没有找到该角色的位置？");
                return;
            }

            GameObject go = self.models[eventData.uid];
            go.transform.SetParent(self.grids[index].transform, false);

            Animator animator = go.GetComponent<Animator>();
            animator.Play("stand", -1, 0);
        }

        [GameEngine.EventSubscribeBindingOfTarget(typeof(BattleActorAttackNotify))]
        private static void OnRecvEventByType(this BattleAnimationComponent self, BattleActorAttackNotify eventData)
        {
            BattleMapComponent map = self.GetComponent<BattleMapComponent>();
            int index = map.FindGridByUid(eventData.uid);
            if (index < 0)
            {
                Debugger.Error("没有找到该角色的位置？");
                return;
            }

            GameObject go = self.models[eventData.uid];
            Animator animator = go.GetComponent<Animator>();
            animator.Play(eventData.skill, -1, 0);
        }

        [GameEngine.EventSubscribeBindingOfTarget(typeof(BattleActorHurtNotify))]
        private static void OnRecvEventByType(this BattleAnimationComponent self, BattleActorHurtNotify eventData)
        {
            Debugger.Warn($"目标对象{eventData.uid}受到攻击！");
        }

        [GameEngine.EventSubscribeBindingOfTarget(typeof(BattleActorDieNotify))]
        private static void OnRecvEventByType(this BattleAnimationComponent self, BattleActorDieNotify eventData)
        {
            BattleMapComponent map = self.GetComponent<BattleMapComponent>();
            int index = map.FindGridByUid(eventData.uid);
            if (index < 0)
            {
                Debugger.Error("没有找到该角色的位置？");
                return;
            }

            Debugger.Warn($"目标对象{eventData.uid}死亡！");

            BattleLoadComponent load = self.GetComponent<BattleLoadComponent>();
            load.OnMonsterLeave(load.FindActorByUid(eventData.uid));
        }

    }
}
