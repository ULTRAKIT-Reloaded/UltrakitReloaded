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
    [HarmonyPatch(typeof(SeasonalHats), "Start")]
    public static class SeasonalHatsPatch
    {
        static void Prefix(SeasonalHats __instance)
        {
            HatsManager manager = __instance.gameObject.AddComponent<HatsManager>();
            foreach (HatRegistry registry in HatLoader.registries)
            {
                manager.LoadHat(registry);
            }
        }
    }
}
