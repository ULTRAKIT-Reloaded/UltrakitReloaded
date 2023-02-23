using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using UnityEngine;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Managers;
using ULTRAKIT.Loader.Loaders;

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
            foreach (HatRegistry registry in Registries.hat_registries)
            {
                manager.LoadHat(registry);
            }
            HatLoader.managerInstances.Add(manager);
            ClearSeasonal(manager);
            foreach (string hat in Registries.hat_activeHats)
                manager.SetHatActive(hat, true);
        }

        private static void ClearSeasonal(HatsManager manager)
        {
            manager.SetHatActive("christmas", false);
            manager.SetHatActive("halloween", false);
            manager.SetHatActive("easter", false);
        }
    }
}
