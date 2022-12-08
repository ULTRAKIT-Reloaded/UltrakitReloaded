using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using UnityEngine;
using ULTRAKIT.Extensions;

namespace ULTRAKIT.Loader.Injectors
{
    [HarmonyPatch(typeof(SeasonalHats))]
    public static class SeasonalHatsPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void StartPrefix(SeasonalHats __instance)
        {
            HatsManager manager = __instance.gameObject.AddComponent<HatsManager>();
            foreach (HatRegistry registry in HatLoader.registries)
            {
                manager.LoadHat(registry);
            }
            HatLoader.managerInstances.Add(manager);
            foreach (string hat in HatLoader.activeHats)
                manager.SetHatActive(hat, true);
        }
    }
}
