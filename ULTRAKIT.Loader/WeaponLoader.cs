using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Data;
using HarmonyLib;

namespace ULTRAKIT.Loader
{
    public static class WeaponLoader
    {
        public static Dictionary<string, List<Weapon>> registry = new Dictionary<string, List<Weapon>>();

        public static Weapon[] LoadWeapons(AssetBundle bundle)
        {
            string name = bundle.name;
            registry.Add(name, new List<Weapon>());
            Weapon[] weapons = bundle.LoadAllAssets<Weapon>();
            registry[name].AddRange(weapons);

            Debug.Log($"Loaded weapons from {name}");

            return weapons;
        }

        public static Harmony harmony = new Harmony("ULTRAKIT.Loader");

        public static void Initialize()
        {
            harmony.PatchAll();
        }
    }
}
