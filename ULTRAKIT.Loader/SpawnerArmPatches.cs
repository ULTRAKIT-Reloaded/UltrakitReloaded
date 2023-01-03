using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using ULTRAKIT.Loader.Injectors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ULTRAKIT.Loader
{
    [HarmonyPatch(typeof(CameraController))]
    public static class CameraControllerPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static bool AwakePrefix()
        {
            if (SpawnerInjector._init)
                return true;
            return false;
        }

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static bool StartPrefix()
        {
            if (SpawnerInjector._init)
                return true;
            return false;
        }

        [HarmonyPatch("OnEnable")]
        [HarmonyPrefix]
        public static bool OnEnablePrefix()
        {
            if (SpawnerInjector._init)
                return true;
            return false;
        }
    }

    [HarmonyPatch(typeof(SkyboxEnabler))]
    public static class SkyboxEnablerPatch
    {
        [HarmonyPatch("OnEnable")]
        [HarmonyPrefix]
        public static bool OnEnablePrefix()
        {
            if (CameraController.Instance)
                return true;
            return false;
        }
    }

    [HarmonyPatch(typeof(SpawnMenu))]
    public static class SpawnMenuPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AwakePrefix(SpawnMenu __instance, SpawnableObjectsDatabase ___objects)
        {
            if (!SpawnerArmLoader.init)
            {
                SpawnerArmLoader.spawnablesDatabase.enemies = ___objects.enemies;
                SpawnerArmLoader.spawnablesDatabase.objects = ___objects.objects;
                SpawnerArmLoader.spawnablesDatabase.sandboxTools = ___objects.sandboxTools;
                SpawnerArmLoader.init = true;
            }

            SpawnerArmLoader.InjectSpawnables(__instance);
            ___objects.sandboxTools = SpawnerArmLoader._tools;
            ___objects.enemies = SpawnerArmLoader._enemies;
            ___objects.objects = SpawnerArmLoader._objects;
        }
    }

    [HarmonyPatch(typeof(BossHealthBar))]
    public static class BossHealthBarPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(BossHealthBar __instance, EnemyIdentifier ___eid, GameObject ___bossBar)
        {
            var cust = __instance.gameObject.GetComponent<CustomHealthbarPos>();
            if (cust)
            {
                cust.barObj = ___bossBar;
                cust.enemy = ___eid.gameObject;
                cust.enabled = true;
            }
        }
    }

    [HarmonyPatch(typeof(MinosBoss))]
    public static class MinosPatch
    {
        static float minosHeight = 600, minosOffset = 200;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void StartPrefix(MinosBoss __instance)
        {
            var cust = __instance.GetComponentInChildren<CustomHealthbarPos>(true);
            if (cust)
            {
                cust.offset = Vector3.up * minosHeight * 1.5f;
                cust.size = 0.25f;
                var plr = MonoSingleton<NewMovement>.Instance.transform;
                __instance.transform.position = plr.position + Vector3.down * minosHeight;
                __instance.transform.position += plr.forward * minosOffset;
                __instance.transform.forward = Vector3.ProjectOnPlane((plr.position - __instance.transform.position).normalized, Vector3.up);
            }
        }
    }

    [HarmonyPatch(typeof(LeviathanHead))]
    public static class LeviathanPatch
    {
        static float leviHeight = 50;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void StartPrefix(LeviathanHead __instance)
        {
            var cust = __instance.transform.parent.GetComponentInChildren<CustomHealthbarPos>(true);
            if (cust)
            {
                cust.offset = Vector3.up * leviHeight * 1.5f;
                cust.size = 0.25f;
            }
        }
    }

    [HarmonyPatch(typeof(Wicked))]
    public static class WickedPatch
    {
        [HarmonyPatch("GetHit")]
        [HarmonyPrefix]
        public static bool GetHitPrefix(Wicked __instance)
        {
            if (__instance.patrolPoints[0] == null)
            {
                var oldAud = __instance.hitSound.GetComponent<AudioSource>();

                var newAudObj = new GameObject();
                newAudObj.transform.position = __instance.transform.position;

                var newAud = newAudObj.AddComponent<AudioSource>();
                newAud.playOnAwake = false;
                newAud.clip = oldAud.clip;
                newAud.volume = oldAud.volume;
                newAud.pitch = oldAud.pitch;
                newAud.spatialBlend = oldAud.spatialBlend;
                newAud.Play();

                GameObject.Destroy(newAudObj, newAud.clip.length);

                GameObject.Destroy(__instance.gameObject);
                return false;
            }

            return true;
        }
    }
}
