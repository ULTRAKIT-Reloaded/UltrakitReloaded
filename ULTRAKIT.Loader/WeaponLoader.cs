using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using HarmonyLib;
using UMM;
using ULTRAKIT.Extensions.Data;

namespace ULTRAKIT.Loader
{
    public static class WeaponLoader
    {
        public static Dictionary<string, List<Weapon>> registry = new Dictionary<string, List<Weapon>>();
        public static List<Weapon> allWeapons = new List<Weapon>();
        public static Dictionary<Tuple<WeaponType, int>, ReplacementWeapon> replacements = new Dictionary<Tuple<WeaponType, int>, ReplacementWeapon>();

        public static Weapon[] LoadWeapons(AssetBundle bundle)
        {
            string name = bundle.name;
            registry.Add(name, new List<Weapon>());
            Weapon[] weapons = bundle.LoadAllAssets<Weapon>();
            foreach (Weapon weapon in weapons)
            {
                weapon.modName = name;
                int[] orderData;
                int[] statusData;
                bool unlockData;
                if (!SaveData.data.weapon_order.TryGetValue($@"{weapon.modName}.{weapon.id}", out orderData))
                    orderData = new int[] {0, 1, 2};
                if (!SaveData.data.weapon_status.TryGetValue($@"{weapon.modName}.{weapon.id}", out statusData))
                    statusData = new int[] {1, 1, 1};
                if (!SaveData.data.weapon_unlock.TryGetValue($@"{weapon.modName}.{weapon.id}", out unlockData))
                    unlockData = weapon.Unlocked;
                /*string data = UKMod.RetrieveStringPersistentModData($@"{weapon.modName}.{weapon.id}.load", "ULTRAKIT");
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
                weapon.Unlocked = Convert.ToBoolean(unlockData);*/
                weapon.equipOrder = orderData;
                weapon.equipStatus = statusData;
                weapon.Unlocked = unlockData;
                List<GameObject> variants = new List<GameObject>();
                variants.AddRange(weapon.Variants);
                variants.AddRange(weapon.AltVariants);
                weapon.All_Variants = variants.ToArray();
            }
            registry[name].AddRange(weapons);
            allWeapons.AddRange(weapons);

            UKLogger.Log($"Loaded weapons from {name}");

            UltrakitInputManager.UpdateKeyBinds();
            return weapons;
        }

        public static void LoadReplacements(AssetBundle bundle)
        {
            string name = bundle.name;
            ReplacementWeapon[] weapons = bundle.LoadAllAssets<ReplacementWeapon>();
            foreach (ReplacementWeapon weapon in weapons)
            {
                weapon.modName = name;
                Tuple<WeaponType, int> key = new Tuple<WeaponType, int>(weapon.WeaponType, weapon.Variant);
                if (replacements.ContainsKey(key))
                {
                    UKLogger.LogWarning($"{{{name}}} Failed to load weapon: {weapon.WeaponType} variant {weapon.Variant} replacement already loaded.");
                    continue;
                }
                replacements.Add(key, weapon);
            }
        }

        public static void UnloadWeapons(string bundleName)
        {
            List<Tuple<WeaponType, int>> deletionQueue= new List<Tuple<WeaponType, int>>();
            foreach (var pair in replacements)
            {
                if (pair.Value.modName == bundleName)
                    deletionQueue.Add(pair.Key);
            }
            foreach (var item in deletionQueue)
            {
                replacements.Remove(item);
            }

            foreach (Weapon weapon in registry[bundleName])
            {
                allWeapons.Remove(weapon);
            }
            registry.Remove(bundleName);
            AssetBundle[] bundles = AssetBundle.GetAllLoadedAssetBundles().Where(b => b.name == bundleName).ToArray();
            foreach (AssetBundle bundle in bundles)
            {
                bundle.Unload(true);
            }

            UltrakitInputManager.UpdateKeyBinds();
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
                UKLogger.LogWarning($@"No bundle of name {bundleName} found.");
                return null;
            }

            foreach (Weapon weapon in weapons)
            {
                if (weapon.id == weaponId)
                {
                    return weapon;
                }
            }

            UKLogger.LogWarning($@"No weapon {weaponId} found.");
            return null;
        }

        public static bool SetWeaponUnlock(string bundleName, string weaponId, bool state)
        {
            Weapon weapon = idToWeapon(bundleName, weaponId);
            if (weapon == null)
            {
                UKLogger.LogWarning("Weapon not found");
                return false;
            }

            weapon.Unlocked = state;
            //UKMod.SetPersistentModData($@"{weapon.modName}.{weapon.id}.unlock", state.ToString(), "ULTRAKIT");
            SaveData.Internal_SetValue(SaveData.data.weapon_unlock, $@"{weapon.modName}.{weapon.id}", state);
            MonoSingleton<GunSetter>.Instance.RefreshWeapons();
            return true;
        }

        public static bool SetWeaponUnlock(Weapon weapon, bool state)
        {
            if (weapon == null)
            {
                UKLogger.LogWarning("Weapon not found");
                return false;
            }

            weapon.Unlocked = state;
            //UKMod.SetPersistentModData($@"{weapon.modName}.{weapon.id}.unlock", state.ToString(), "ULTRAKIT");
            SaveData.Internal_SetValue(SaveData.data.weapon_unlock, $@"{weapon.modName}.{weapon.id}", state);
            MonoSingleton<GunSetter>.Instance.RefreshWeapons();
            return true;
        }
    }
}
