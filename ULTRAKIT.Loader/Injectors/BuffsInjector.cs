using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using UnityEngine;

namespace ULTRAKIT.Loader.Injectors
{
    [HarmonyPatch(typeof(EnemyIdentifier))]
    public static class EnemyIdentifierPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch(EnemyIdentifier __instance)
        {
            BuffsManager manager = __instance.gameObject.AddComponent<BuffsManager>();
            manager.eid = __instance;
            manager.LoadBuffs(BuffLoader.buffRegistry.ToArray());
        }
    }
}
