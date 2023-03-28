using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using ULTRAKIT.Extensions;
using UnityEditor;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions.Data;
using ULTRAKIT.Extensions.Classes;
using HarmonyLib;

namespace ULTRAKIT.Loader.Injectors
{
    public static class SpawnablesInjector
    {
        public static List<SpawnableObject> _enemies = new List<SpawnableObject>();
        //static GameObject go;

        // ULTRAKILL enemies added to the spawner arm by default
        static Dictionary<string, EnemyType> SpawnList = new Dictionary<string, EnemyType>
        {
            { "DroneFlesh", EnemyType.Drone },
            { "DroneSkull Variant", EnemyType.Drone },
            { "MinosBoss", EnemyType.Minos },
            { "Wicked", EnemyType.Wicked },
            { "Drone Variant", EnemyType.Drone },
            { "DroneFleshCamera Variant", EnemyType.Drone }
            // Same health as miniboss version { "SwordsMachine", EnemyType.Swordsmachine },
            // Empty fields, needs to be done similar to Leviathan, TODO { "MinosArm" , EnemyType.Minos },

        };

        public static void Init()
        {
            //go = AssetLoader.AssetFind<GameObject>("Super Projectile Zombie.prefab");
            PrepLeviathan();

            foreach (var pair in SpawnList)
            {
                SpawnableObject spawnable = ScriptableObject.CreateInstance<SpawnableObject>();
                spawnable.identifier = pair.Key;
                spawnable.spawnableObjectType = SpawnableObject.SpawnableObjectDataType.Enemy;
                spawnable.objectName = pair.Key;
                spawnable.type = "Enemy";
                spawnable.enemyType = pair.Value;
                spawnable.spawnableType = SpawnableType.SimpleSpawn;
                spawnable.gameObject = GrabEnemy($"{pair.Key}.prefab");
                spawnable.preview = new GameObject();
                spawnable.gridIcon = Registries.spawn_sprites[pair.Key];

                _enemies.Add(spawnable);
            }
        }

        private static void PrepLeviathan()
        {
            GameObject LeviathanBase = new GameObject("Leviathan");
            LeviathanBase.SetActive(false);
            GameObject head = GameObject.Instantiate(AssetLoader.AssetFind<GameObject>("LeviathanHead.prefab"), LeviathanBase.transform);
            GameObject tail = GameObject.Instantiate(AssetLoader.AssetFind<GameObject>("LeviathanTail Variant.prefab"), LeviathanBase.transform);
            GameObject splash = GameObject.Instantiate(AssetLoader.AssetFind<GameObject>("SplashBig.prefab"), LeviathanBase.transform);
            LeviathanController controller = LeviathanBase.AddComponent<LeviathanController>();
            EnemyIdentifier eid = LeviathanBase.AddComponent<EnemyIdentifier>();
            Statue stat = LeviathanBase.AddComponent<Statue>();
            SphereCollider collider = LeviathanBase.AddComponent<SphereCollider>();
            BossHealthBar bar = LeviathanBase.AddComponent<BossHealthBar>();
            Rigidbody rb = LeviathanBase.AddComponent<Rigidbody>();
            BossIdentifier bid = LeviathanBase.AddComponent<BossIdentifier>();

            controller.head = head.GetComponent<LeviathanHead>();
            controller.tail = tail.GetComponent<LeviathanTail>();
            controller.bigSplash = AssetLoader.AssetFind<GameObject>("SplashBig.prefab");
            controller.headWeakPoint = head.transform.Find("Leviathan_SplineHook_Basic/Armature/Bone043/Bone001/Heart");
            controller.tailWeakPoint = tail.transform.Find($"Leviathan_SplineHook_Basic/Armature/{GraphicsUtilities.BonePath(43, 86)}");
            controller.headPartsParent = head.transform.Find("Leviathan_SplineHook_Basic/Armature/Bone043/Bone044");
            controller.tailPartsParent = tail.transform.Find("Leviathan_SplineHook_Basic/Armature/Bone043/Bone044");
            controller.phaseChangeHealth = 100;

            eid.overrideFullName = "Leviathan";
            eid.bigEnemy = true;
            eid.damageBuffModifier = 1.5f;
            eid.enemyClass = EnemyClass.Demon;
            eid.enemyType = EnemyType.Leviathan;
            eid.health = 200;
            eid.healthBuffModifier = 1.5f;
            eid.speedBuffModifier = 1.5f;
            eid.unbounceable = true;
            eid.weakPoint = head.transform.Find("Leviathan_SplineHook_Basic/Armature/Bone043/Bone001/Heart").gameObject;

            stat.affectedByGravity = true;
            stat.bigBlood = true;
            stat.extraDamageMultiplier = 3;
            stat.extraDamageZones = new List<GameObject> { head.transform.Find("Leviathan_SplineHook_Basic/Armature/Bone043/Bone001/Heart").gameObject };
            stat.health = 200;
            stat.specialDeath = true;

            bar.bossName = "LEVIATHAN";
            BossBarManager.HealthLayer b1 = new BossBarManager.HealthLayer();
            BossBarManager.HealthLayer b2 = new BossBarManager.HealthLayer();
            b1.health = 100;
            b2.health = 100;
            bar.healthLayers = new BossBarManager.HealthLayer[] { b1, b2 };

            rb.isKinematic = true;

            controller.head.shootPoint = head.transform.Find("Leviathan_SplineHook_Basic/Armature/Bone043/Bone001/ShootPoint");
            controller.head.projectileSpreadAmount = 5;
            controller.head.tracker = head.transform.Find("Leviathan_SplineHook_Basic/Armature/Bone043/Bone001");
            controller.head.tailBone = head.transform.Find($"Leviathan_SplineHook_Basic/Armature/{GraphicsUtilities.BonePath(43, 56)}");
            controller.head.lookAtPlayer = true;
            controller.head.biteSwingCheck = head.transform.Find("Leviathan_SplineHook_Basic/Armature/Bone043/Bone001/SwingCheck").GetComponent<SwingCheck2>();
            controller.head.warningFlash = AssetLoader.AssetFind<GameObject>("V2FlashUnparriable.prefab");

            controller.gameObject.tag = "Enemy";

            GameObject.DontDestroyOnLoad(LeviathanBase);

            SpawnableObject spawnable = ScriptableObject.CreateInstance<SpawnableObject>();
                spawnable.identifier = "leviathan";
                spawnable.spawnableObjectType = SpawnableObject.SpawnableObjectDataType.Enemy;
                spawnable.objectName = "leviathan";
                spawnable.type = "Enemy";
                spawnable.enemyType = EnemyType.Leviathan;
                spawnable.spawnableType = SpawnableType.SimpleSpawn;
                spawnable.gameObject = LeviathanBase;
                //spawnable.gameObject = AssetLoader.AssetFind<GameObject>("LeviathanHead.prefab");
                spawnable.preview = AssetLoader.AssetFind<GameObject>("Leviathan Preview Variant.prefab");
                spawnable.gridIcon = Registries.spawn_sprites["Leviathan"];

            SetHealthBar(LeviathanBase, "Leviathan");
            _enemies.Add(spawnable);
        }

        public static GameObject GrabEnemy(string enemy)
        {
            GameObject obj = AssetLoader.AssetFind<GameObject>(enemy);
            if (obj != null)
            {
                SetHealthBar(obj, enemy);
                return obj;
            }
            UKLogger.LogWarning($"Could not find enemy {enemy}");
            return null;
        }

        public static void SetHealthBar(GameObject obj, string enemy)
        {
            BossHealthBar bhb = obj.GetComponentInChildren<BossHealthBar>();
            if (bhb == null && (enemy == "MinosBoss" || enemy == "Leviathan"))
            {
                bhb = obj.GetComponentInChildren<EnemyIdentifier>(true).gameObject.AddComponent<BossHealthBar>();
                bhb.bossName = "";
            }

            // Sets health bars to remain over the boss's head to avoid covering the entire screen
            CustomHealthbarPos cust = bhb?.gameObject.AddComponent<CustomHealthbarPos>();
            if (cust)
            {
                cust.offset = Vector3.up * 6;
                cust.enabled = false;
            }
        }
    }
}
