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

namespace ULTRAKIT.Loader.Loaders
{
    public static class WeaponLoader
    {
        private static Dictionary<string, List<Weapon>> registry => Registries.weap_registry;
        private static List<Weapon> allWeapons => Registries.weap_allWeapons;
        private static Dictionary<Tuple<WeaponType, int>, ReplacementWeapon> replacements => Registries.weap_replacements;

        /// <summary>
        /// Loads weapons automatically from a loaded asset bundle.
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns>An array of all loaded weapons</returns>
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

                // Retrieves or initializes weapon data; saves when the weapons are loaded by the gun control
                if (!SaveData.data.weapon_order.TryGetValue($@"{weapon.modName}.{weapon.id}", out orderData))
                    orderData = new int[] {0, 1, 2};
                if (!SaveData.data.weapon_status.TryGetValue($@"{weapon.modName}.{weapon.id}", out statusData))
                    statusData = new int[] {1, 1, 1};
                if (!SaveData.data.weapon_unlock.TryGetValue($@"{weapon.modName}.{weapon.id}", out unlockData))
                    unlockData = weapon.Unlocked;

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

        /// <summary>
        /// Loads replacement/additional vanilla weapons automatically from a loaded asset bundle.
        /// </summary>
        /// <param name="bundle"></param>
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

        /// <summary>
        /// Unloads weapons registered under the specified bundle name.
        /// </summary>
        /// <param name="bundleName"></param>
        public static void UnloadWeapons(string bundleName)
        {
            List<Tuple<WeaponType, int>> deletionQueue= new List<Tuple<WeaponType, int>>();
            // Queues for deletion since items cannot be deleted from an iterating list and continue iterating
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

        /// <summary>
        /// Searches for a weapon with the given ID in the stated bundle.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="weaponId"></param>
        /// <returns>A Weapon</returns>
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

        /// <summary>
        /// Locks/unlocks a weapon from the specified bundle with the given ID.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="weaponId"></param>
        /// <param name="state"></param>
        /// <returns>`true` if successful, otherwise `false`.</returns>
        public static bool SetWeaponUnlock(string bundleName, string weaponId, bool state)
        {
            Weapon weapon = idToWeapon(bundleName, weaponId);
            if (weapon == null)
            {
                UKLogger.LogWarning("Weapon not found");
                return false;
            }

            weapon.Unlocked = state;
            SaveData.Internal_SetValue(SaveData.data.weapon_unlock, $@"{weapon.modName}.{weapon.id}", state);
            MonoSingleton<GunSetter>.Instance.RefreshWeapons();
            return true;
        }

        /// <summary>
        /// Locks/unlocks the given weapon.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="state"></param>
        /// <returns>`true` on success, otherwise `false`.</returns>
        public static bool SetWeaponUnlock(Weapon weapon, bool state)
        {
            if (weapon == null)
            {
                UKLogger.LogWarning("Weapon not found");
                return false;
            }

            weapon.Unlocked = state;
            SaveData.Internal_SetValue(SaveData.data.weapon_unlock, $@"{weapon.modName}.{weapon.id}", state);
            MonoSingleton<GunSetter>.Instance.RefreshWeapons();
            return true;
        }
    }
}
