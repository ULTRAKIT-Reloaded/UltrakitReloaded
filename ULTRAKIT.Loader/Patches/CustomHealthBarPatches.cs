using HarmonyLib;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions.Classes;
using ULTRAKIT.Loader.Loaders;
using UnityEngine;

namespace ULTRAKIT.Loader.Patches
{
    [HarmonyPatch(typeof(BossHealthBar))]
    public static class BossHealthBarPatch
    {
        [HarmonyPatch("Awake"), HarmonyPrefix]
        public static void AwakePrefix(BossHealthBar __instance)
        {
            OptionsLoader.GetCheckbox("ultrakit.healthBars", out UKCheckbox checkbox);
            if (checkbox.GetValue())
            {
                CustomHealthbarPos cust = __instance.gameObject.AddComponent<CustomHealthbarPos>();
                cust.offset = Vector3.up * 6;
                cust.enabled = false;
            }
        }

        [HarmonyPatch("Awake"), HarmonyPostfix]
        public static void AwakePostfix(BossHealthBar __instance, EnemyIdentifier ___eid, GameObject ___bossBar)
        {
            CustomHealthbarPos cust = __instance.gameObject.GetComponent<CustomHealthbarPos>();
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
        // Keeps Minos from flying high in the sky when spawned and positions the health bar
        static float minosHeight = 600, minosOffset = 200;

        [HarmonyPatch("Start"), HarmonyPrefix]
        public static void StartPrefix(MinosBoss __instance)
        {
            CustomHealthbarPos cust = __instance.GetComponentInChildren<CustomHealthbarPos>(true);
            if (cust)
            {
                cust.offset = Vector3.up * minosHeight * 1.5f;
                cust.size = 0.25f;
            }

            if (__instance.GetComponentInChildren<SandboxEnemy>())
            {
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
        // Positions the leviathan health bar when spawned
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

            SkinnedMeshRenderer renderer = __instance.transform.Find("Leviathan_SplineHook_Basic/ArmR").GetComponent<SkinnedMeshRenderer>();
            renderer.material.color = Color.white;
            renderer.material.mainTexture = renderer.material.mainTexture;
        }
    }
}
