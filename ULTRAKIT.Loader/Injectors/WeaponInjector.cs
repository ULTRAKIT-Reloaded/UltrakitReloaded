using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using UnityEngine.UI;
using System.Linq;
using ULTRAKIT.Extensions.Data;
using ULTRAKIT.Extensions.Classes;

namespace ULTRAKIT.Loader.Injectors
{
    [HarmonyPatch(typeof(GunSetter))]
    public class GunSetterPatch
    {
        public static List<List<GameObject>> modSlots = new List<List<GameObject>>();
        public static Dictionary<GameObject, bool> equippedDict = new Dictionary<GameObject, bool>();

        // Registers variants in the prefs manager
        static void SetPref(string weapon, bool alt)
        {
            if (!PrefsManager.Instance.prefMap.ContainsKey($"weapon.{weapon}"))
                PrefsManager.Instance.prefMap.Add($"weapon.{weapon}", 1);
            if (!GameProgressSaverPatch.loadedWeapons.Contains(weapon))
                GameProgressSaverPatch.loadedWeapons.Add(weapon);
            string altText = weapon.Remove(weapon.Length - 1) + "alt";
            if (alt && !GameProgressSaverPatch.loadedWeapons.Contains(altText))
                GameProgressSaverPatch.loadedWeapons.Add(altText);
        }

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void StartPrefix(GunSetter __instance)
        {
            foreach (ReplacementWeapon weapon in Registries.weap_replacements.Values)
            {
                int slot = weapon.Alt ? 1 : 0;
                weapon.Prefab.SetActive(false);
                weapon.Prefab.AddComponent<RenderFixer>().LayerName = "AlwaysOnTop";
                // Finds the correct weapon type, then the correct variant array, copies it, replaces the prefab with the replacement (or adds it if missing), then puts it back into the gun setter
                // The gun control will now grab the replacement prefab from the gun setter registry
                switch (weapon.WeaponType)
                {
                    case WeaponType.Revolver: 
                        {
                            switch (weapon.Variant)
                            {
                                case 0: List<GameObject> buffer1 = __instance.revolverPierce.ToList(); buffer1.ReplaceOrAddTo(weapon.Prefab, slot); __instance.revolverPierce = buffer1.ToArray(); SetPref("rev0", weapon.Alt); break;
                                case 1: List<GameObject> buffer2 = __instance.revolverRicochet.ToList(); buffer2.ReplaceOrAddTo(weapon.Prefab, slot); __instance.revolverRicochet = buffer2.ToArray(); SetPref("rev2", weapon.Alt); break;
                                case 2: List<GameObject> buffer3 = __instance.revolverBerserker.ToList(); buffer3.ReplaceOrAddTo(weapon.Prefab, slot); __instance.revolverBerserker = buffer3.ToArray(); SetPref("rev1", weapon.Alt); break;
                            }
                        } break;
                    case WeaponType.Shotgun:
                        {
                            switch (weapon.Variant)
                            {
                                case 0: List<GameObject> buffer1 = __instance.shotgunGrenade.ToList(); buffer1.ReplaceOrAddTo(weapon.Prefab, slot); __instance.shotgunGrenade = buffer1.ToArray(); SetPref("sho0", weapon.Alt) ; break;
                                case 1: List<GameObject> buffer2 = __instance.shotgunPump.ToList(); buffer2.ReplaceOrAddTo(weapon.Prefab, slot); __instance.shotgunPump = buffer2.ToArray(); SetPref("sho1", weapon.Alt); break;
                                case 2: List<GameObject> buffer3 = __instance.shotgunRed.ToList(); buffer3.ReplaceOrAddTo(weapon.Prefab, slot); __instance.shotgunRed = buffer3.ToArray(); SetPref("sho2", weapon.Alt); break;
                            }
                        }
                        break;
                    case WeaponType.Nailgun:
                        {
                            switch (weapon.Variant)
                            {
                                case 0: List<GameObject> buffer1 = __instance.nailMagnet.ToList(); buffer1.ReplaceOrAddTo(weapon.Prefab, slot); __instance.nailMagnet = buffer1.ToArray(); SetPref("nai0", weapon.Alt) ; break;
                                case 1: List<GameObject> buffer2 = __instance.nailOverheat.ToList(); buffer2.ReplaceOrAddTo(weapon.Prefab, slot); __instance.nailOverheat = buffer2.ToArray(); SetPref("nai1", weapon.Alt) ; break;
                                case 2: List<GameObject> buffer3 = __instance.nailRed.ToList(); buffer3.ReplaceOrAddTo(weapon.Prefab, slot); __instance.nailRed = buffer3.ToArray(); SetPref("nai2", weapon.Alt) ; break;
                            }
                        }
                        break;
                    case WeaponType.Railcannon:
                        {
                            switch (weapon.Variant)
                            {
                                case 0: List<GameObject> buffer1 = __instance.railCannon.ToList(); buffer1.ReplaceOrAddTo(weapon.Prefab, slot); __instance.railCannon = buffer1.ToArray(); SetPref("rai0", weapon.Alt) ; break;
                                case 1: List<GameObject> buffer2 = __instance.railHarpoon.ToList(); buffer2.ReplaceOrAddTo(weapon.Prefab, slot); __instance.railHarpoon = buffer2.ToArray(); SetPref("rai1", weapon.Alt) ; break;
                                case 2: List<GameObject> buffer3 = __instance.railMalicious.ToList(); buffer3.ReplaceOrAddTo(weapon.Prefab, slot); __instance.railMalicious = buffer3.ToArray(); SetPref("rai2", weapon.Alt) ; break;
                            }
                        }
                        break;
                    case WeaponType.RocketLauncher:
                        {
                            switch (weapon.Variant)
                            {
                                case 0: List<GameObject> buffer1 = __instance.rocketBlue.ToList(); buffer1.ReplaceOrAddTo(weapon.Prefab, slot); __instance.rocketBlue = buffer1.ToArray(); SetPref("rock0", weapon.Alt) ; break;
                                case 1: List<GameObject> buffer2 = __instance.rocketGreen.ToList(); buffer2.ReplaceOrAddTo(weapon.Prefab, slot); __instance.rocketGreen = buffer2.ToArray(); SetPref("rock1", weapon.Alt) ; break;
                                case 2: List<GameObject> buffer3 = __instance.rocketRed.ToList(); buffer3.ReplaceOrAddTo(weapon.Prefab, slot); __instance.rocketRed = buffer3.ToArray(); SetPref("rock2", weapon.Alt) ; break;
                            }
                        }
                        break;
                }
            }
        }

        [HarmonyPatch("ResetWeapons")]
        [HarmonyPostfix]
        static void ResetWeaponsPostfix(GunSetter __instance)
        {
            // Clears the slots and refreshes anew; used mostly in case a weapon is removed (i.e. the source mod is disabled)
            foreach (var slot in modSlots)
            {
                foreach (var item in slot)
                {
                    if (item)
                    {
                        GameObject.Destroy(item);
                    }
                }

                if (__instance.gunc?.slots?.Contains(slot) ?? false)
                {
                    __instance.gunc.slots.Remove(slot);
                }
            }
            modSlots.Clear();
            foreach (var pair in Registries.weap_registry)
            {
                foreach (var weap in pair.Value)
                {
                    if (!weap.Unlocked)
                    {
                        continue;
                    }

                    var slot = new List<GameObject>();

                    // Saves the weapon equip data to a persistent file (changed in the ShopInjector)
                    SaveData.Internal_SetValue(SaveData.data.weapon_order, $@"{weap.modName}.{weap.id}", weap.equipOrder);
                    SaveData.Internal_SetValue(SaveData.data.weapon_status, $@"{weap.modName}.{weap.id}", weap.equipStatus);

                    for (int i = 0; i < weap.All_Variants.Length; i++)
                    {
                        var variant = weap.All_Variants[i];

                        if (!equippedDict.ContainsKey(variant))
                        {
                            // Alt weapons are placed in the array after standard weapons; this reads the same variant order for standard and alts (i.e. standard 1 is 1, alt 1 is 1, alt 2 is 2, etc.)
                            int s = (int)Mathf.Repeat(i, weap.Variants.Length);
                            // Checks to see if this is standard and standard is equipped, this is alt and alt is equipped, or neither
                            bool equipped = (i < weap.Variants.Length && weap.equipStatus[s] == 1) || (i >= weap.Variants.Length && weap.equipStatus[s] == 2);
                            equippedDict.Add(variant, equipped);
                        }
                    }
                    for (int i = 0; i < weap.Variants.Length; i++)
                    {
                        // Searches for the index of the weapon whos equip order is `i` (starting at 0, then 1, then 2) to grab that variant
                        int index = weap.equipOrder.FindIndexOf(i);
                        // Checks the equip status to grab either the standard or alt
                        var variant = weap.equipStatus[index] == 2 ? weap.AltVariants[index] : weap.Variants[index];

                        if (!equippedDict[variant])
                        {
                            continue;
                        }

                        var go = GameObject.Instantiate(variant, __instance.transform);
                        go.SetActive(false);

                        go.transform.RenderObject(LayerMask.NameToLayer("AlwaysOnTop"));
                        
                        // Sets the weapon icon to the icon specified in the editor and sets the variation color.
                        // Grabs any variant color materials/renderers by searching for ".var" in the name, gives it an empty array if none found. Necessary to prevent null reference exceptions.
                        var wi = go.AddComponent<WeaponIcon>();
                        wi.weaponIcon = weap.Icons[i];
                        wi.glowIcon = weap.Icons[i];
                        wi.variationColor = i;
                        wi.SetFieldValue("variationColoredMaterials", go.GetComponentsInChildren<Material>().Where(k => k.name.Contains(".var")).ToArray() ?? new Material[0], true);
                        wi.SetFieldValue("variationColoredRenderers", go.GetComponentsInChildren<Renderer>().Where(k => k.material.name.Contains(".var")).ToArray() ?? new Renderer[0], true);
                        // Likely used for the shop, though the color setting is done manually in the ShopInjector
                        wi.SetFieldValue("variationColoredImages", new Image[0], true);

                        // Adds weapons to the style freshness list, which makes freshness functional and keeps the game from throwing a warning every frame
                        var field = typeof(StyleHUD).GetField("weaponFreshness", BindingFlags.NonPublic | BindingFlags.Instance);
                        Dictionary<GameObject, float> freshnessList = field.GetValue(MonoSingleton<StyleHUD>.Instance) as Dictionary<GameObject, float>;
                        freshnessList.Add(go, 10f);
                        field.SetValue(MonoSingleton<StyleHUD>.Instance, freshnessList);

                        slot.Add(go);

                        __instance.gunc.allWeapons.Add(go);
                    }

                    __instance.gunc.slots.Add(slot);
                    modSlots.Add(slot);
                }
            }
            UltrakitInputManager.UpdateKeyBinds();
        }
    }

    [HarmonyPatch(typeof(GunControl))]
    class GunControlPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void StartPrefix(GunControl __instance)
        {
            // Helps keep the gun control from trying to equip a weapon that doesn't exist
            if (PlayerPrefs.GetInt("CurSlo", 1) > __instance.slots.Count)
            {
                PlayerPrefs.SetInt("CurSlo", 1);
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePostfix(GunControl __instance)
        {
            // Just doing the same thing the game does but with some extra null checking, manually for each slot :(

            if ((UltrakitInputManager.Slot7?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 7 && (__instance.slots[6]?.Count > 0 || __instance.currentSlot != 7))
            {
                if (__instance.slots[6]?.Count > 0 && __instance.slots[6][0] != null)
                {
                    __instance.SwitchWeapon(7, __instance.slots[6]);
                }
            }

            if ((UltrakitInputManager.Slot8?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 8 && (__instance.slots[7]?.Count > 0 || __instance.currentSlot != 8))
            {
                if (__instance.slots[7]?.Count > 0 && __instance.slots[7][0] != null)
                {
                    __instance.SwitchWeapon(8, __instance.slots[7]);
                }
            }

            if ((UltrakitInputManager.Slot9?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 9 && (__instance.slots[8]?.Count > 0 || __instance.currentSlot != 9))
            {
                if (__instance.slots[8]?.Count > 0 && __instance.slots[8][0] != null)
                {
                    __instance.SwitchWeapon(9, __instance.slots[8]);
                }
            }

            if ((UltrakitInputManager.Slot10?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 10 && (__instance.slots[9]?.Count > 0 || __instance.currentSlot != 10))
            {
                if (__instance.slots[9]?.Count > 0 && __instance.slots[9][0] != null)
                {
                    __instance.SwitchWeapon(10, __instance.slots[9]);
                }
            }

            if ((UltrakitInputManager.Slot11?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 11 && (__instance.slots[10]?.Count > 0 || __instance.currentSlot != 11))
            {
                if (__instance.slots[10]?.Count > 0 && __instance.slots[10][0] != null)
                {
                    __instance.SwitchWeapon(11, __instance.slots[10]);
                }
            }

            if ((UltrakitInputManager.Slot12?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 12 && (__instance.slots[11]?.Count > 0 || __instance.currentSlot != 12))
            {
                if (__instance.slots[11]?.Count > 0 && __instance.slots[11][0] != null)
                {
                    __instance.SwitchWeapon(12, __instance.slots[11]);
                }
            }
            
            if ((UltrakitInputManager.Slot13?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 13 && (__instance.slots[12]?.Count > 0 || __instance.currentSlot != 13))
            {
                if (__instance.slots[12]?.Count > 0 && __instance.slots[12][0] != null)
                {
                    __instance.SwitchWeapon(13, __instance.slots[12]);
                }
            }

            if ((UltrakitInputManager.Slot14?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 14 && (__instance.slots[13]?.Count > 0 || __instance.currentSlot != 14))
            {
                if (__instance.slots[13]?.Count > 0 && __instance.slots[13][0] != null)
                {
                    __instance.SwitchWeapon(14, __instance.slots[13]);
                }
            }

            if ((UltrakitInputManager.Slot15?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 15 && (__instance.slots[14]?.Count > 0 || __instance.currentSlot != 15))
            {
                if (__instance.slots[14]?.Count > 0 && __instance.slots[14][0] != null)
                {
                    __instance.SwitchWeapon(15, __instance.slots[14]);
                }
            }

            if ((UltrakitInputManager.Slot16?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 16 && (__instance.slots[15]?.Count > 0 || __instance.currentSlot != 16))
            {
                if (__instance.slots[15]?.Count > 0 && __instance.slots[15][0] != null)
                {
                    __instance.SwitchWeapon(16, __instance.slots[15]);
                }
            }

            if ((UltrakitInputManager.Slot17?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 17 && (__instance.slots[16]?.Count > 0 || __instance.currentSlot != 17))
            {
                if (__instance.slots[16]?.Count > 0 && __instance.slots[16][0] != null)
                {
                    __instance.SwitchWeapon(17, __instance.slots[16]);
                }
            }

            if ((UltrakitInputManager.Slot18?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 18 && (__instance.slots[17]?.Count > 0 || __instance.currentSlot != 18))
            {
                if (__instance.slots[17]?.Count > 0 && __instance.slots[17][0] != null)
                {
                    __instance.SwitchWeapon(18, __instance.slots[17]);
                }
            }

            if ((UltrakitInputManager.Slot19?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 19 && (__instance.slots[18]?.Count > 0 || __instance.currentSlot != 19))
            {
                if (__instance.slots[18]?.Count > 0 && __instance.slots[18][0] != null)
                {
                    __instance.SwitchWeapon(19, __instance.slots[18]);
                }
            }

            if ((UltrakitInputManager.Slot20?.WasPerformedThisFrame ?? false) && __instance.slots.Count >= 20 && (__instance.slots[19]?.Count > 0 || __instance.currentSlot != 20))
            {
                if (__instance.slots[19]?.Count > 0 && __instance.slots[19][0] != null)
                {
                    __instance.SwitchWeapon(20, __instance.slots[19]);
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameProgressSaver))]
    public class GameProgressSaverPatch
    {
        public static List<string> loadedWeapons = new List<string>();

        [HarmonyPatch("CheckGear")]
        [HarmonyPostfix]
        static void ChechGearPostfix(ref object __result, string gear)
        {
            // Tricks the game into thinking everything is unlocked so you can equip modded variants for vanilla weapons
            if (loadedWeapons.Contains(gear))
                __result = 1;
        }
    }
}
