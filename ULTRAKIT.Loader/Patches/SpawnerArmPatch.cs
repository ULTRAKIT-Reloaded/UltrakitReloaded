using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Classes;
using ULTRAKIT.Loader.Injectors;
using ULTRAKIT.Loader.Loaders;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ULTRAKIT.Loader.Patches
{
    [HarmonyPatch(typeof(SpawnMenu))]
    public static class SpawnMenuPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AwakePrefix(SpawnMenu __instance, SpawnableObjectsDatabase ___objects)
        {
            // Creates a constant copy of vanilla database for loader use
            // The database persists across loads/scenes, so doing otherwise would keep adding spawnables to a list that already has them
            if (!SpawnablesLoader.init)
            {
                // Doing it here does not cause the bloodstains to turn into squares
                SpawnablesInjector.Init();

                Registries.spawn_spawnablesDatabase.enemies = ___objects.enemies;
                Registries.spawn_spawnablesDatabase.objects = ___objects.objects;
                Registries.spawn_spawnablesDatabase.sandboxTools = ___objects.sandboxTools;
                SpawnablesLoader.init = true;
            }

            SpawnablesLoader.InjectSpawnables(__instance);
            ___objects.sandboxTools = Registries.spawn_tools;
            ___objects.enemies = Registries.spawn_enemies;
            ___objects.objects = Registries.spawn_objects;
        }
    }

    [HarmonyPatch(typeof(LeviathanTail))]
    public static class LeviathanTailPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(LeviathanTail __instance)
        {
            SkinnedMeshRenderer renderer = __instance.transform.Find("Leviathan_SplineHook_Basic/ArmR").GetComponent<SkinnedMeshRenderer>();
            renderer.material.color = Color.white;
            renderer.material.mainTexture = renderer.material.mainTexture;
        }
    }

    [HarmonyPatch(typeof(ComplexSplasher))]
    public static class LeviathanTeasePatch
    {
        [HarmonyPatch("Awake"), HarmonyPostfix]
        public static void AwakePostfix(ComplexSplasher __instance)
        {
            Transform armature = __instance.transform.Find("Leviathan_SplineHook_Basic/ArmR");
            if (armature == null) return;
            SkinnedMeshRenderer renderer = armature.GetComponent<SkinnedMeshRenderer>();
            renderer.material.color = Color.white;
            renderer.material.mainTexture = renderer.material.mainTexture;
        }
    }

    [HarmonyPatch(typeof(ObjectActivator))]
    public static class LeviathanTeasePatch2
    {
        [HarmonyPatch("Start"), HarmonyPostfix]
        public static void StartPostfix(ObjectActivator __instance)
        {
            Transform armature = __instance.transform.Find("Leviathan_HeadFix/Leviathan");
            if (armature == null) return;
            SkinnedMeshRenderer renderer = armature.GetComponent<SkinnedMeshRenderer>();
            renderer.material.color = Color.white;
            renderer.material.mainTexture = renderer.material.mainTexture;
        }
    }

    [HarmonyPatch(typeof(Wicked))]
    public static class WickedPatch
    {
        [HarmonyPatch("GetHit")]
        [HarmonyPrefix]
        public static bool GetHitPrefix(Wicked __instance)
        {
            // Keeps Something Wicked from getting mad and breaking when there is nowhere to teleport
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
