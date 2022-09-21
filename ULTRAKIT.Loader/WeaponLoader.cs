using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Data;
using HarmonyLib;
using UMM;

namespace ULTRAKIT.Loader
{
    public static class WeaponLoader
    {
        public static Dictionary<string, List<Weapon>> registry = new Dictionary<string, List<Weapon>>();
        public static List<Weapon> allWeapons = new List<Weapon>();

        public static Weapon[] LoadWeapons(AssetBundle bundle)
        {
            string name = bundle.name;
            registry.Add(name, new List<Weapon>());
            Weapon[] weapons = bundle.LoadAllAssets<Weapon>();

            foreach (Weapon weapon in weapons)
            {
                weapon.modName = name;
                string data = UKMod.RetrieveStringPersistentModData($@"{weapon.modName}.{weapon.id}.load", "ULTRAKIT");
                string data2 = UKMod.RetrieveStringPersistentModData($@"{weapon.modName}.{weapon.id}.equip", "ULTRAKIT");
                string unlockData = UKMod.RetrieveStringPersistentModData($@"{weapon.modName}.{weapon.id}.unlock", "ULTRAKIT");

                if (data == null)
                {
                    data = "0,1,2";
                    UKMod.SetPersistentModData($@"{weapon.modName}.{weapon.id}.load", data, "ULTRAKIT");
                }
                if (data2 == null)
                {
                    data2 = "1,1,1";
                    UKMod.SetPersistentModData($@"{weapon.modName}.{weapon.id}.equip", data2, "ULTRAKIT");
                }
                if (unlockData == null)
                {
                    unlockData = weapon.Unlocked.ToString();
                    UKMod.SetPersistentModData($@"{weapon.modName}.{weapon.id}.unlock", unlockData, "ULTRAKIT");
                }

                weapon.equipOrder = data.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                weapon.equipStatus = data2.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                weapon.Unlocked = Convert.ToBoolean(unlockData);

                List<GameObject> variants = new List<GameObject>();
                variants.AddRange(weapon.Variants);
                variants.AddRange(weapon.AltVariants);
                weapon.All_Variants = variants.ToArray();
            }

            registry[name].AddRange(weapons);
            allWeapons.AddRange(weapons);

            Debug.Log($"Loaded weapons from {name}");

            return weapons;
        }

        public static Weapon idToWeapon(string bundleName, string weaponId)
        {
            List<Weapon> weapons = new List<Weapon>();

            try
            {
                weapons = registry[bundleName];
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.Log($@"No bundle of name {bundleName} found.");
                return null;
            }

            foreach (Weapon weapon in weapons)
            {
                if (weapon.id == weaponId)
                {
                    return weapon;
                }
            }

            Debug.Log($@"No weapon {weaponId} found.");
            return null;
        }

        public static bool SetWeaponUnlock(string bundleName, string weaponId, bool state)
        {
            Weapon weapon = idToWeapon(bundleName, weaponId);
            if (weapon == null)
            {
                Debug.Log("Weapon not found");
                return false;
            }

            weapon.Unlocked = state;
            UKMod.SetPersistentModData($@"{weapon.modName}.{weapon.id}.unlock", state.ToString(), "ULTRAKIT");
            MonoSingleton<GunSetter>.Instance.ResetWeapons();
            return true;
        }

        public static Harmony harmony = new Harmony("ULTRAKIT.Loader");

        public static void Initialize()
        {
            harmony.PatchAll();
        }
    }
}
