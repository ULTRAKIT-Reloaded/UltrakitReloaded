using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using UnityEngine.UI;
using System.Linq;
using UMM;

namespace ULTRAKIT.Loader.Injectors
{
    [HarmonyPatch(typeof(GunSetter), "ResetWeapons")]
    public class GunSetterPatch
    {
        public static List<List<GameObject>> modSlots = new List<List<GameObject>>();
        public static Dictionary<GameObject, bool> equippedDict = new Dictionary<GameObject, bool>();

        static void Postfix(GunSetter __instance)
        {
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
            foreach (var pair in WeaponLoader.registry)
            {
                foreach (var weap in pair.Value)
                {
                    if (!weap.Unlocked)
                    {
                        continue;
                    }

                    var slot = new List<GameObject>();

                    string loadOrder = string.Join(",", weap.equipOrder);
                    string equippedStatus = string.Join(",", weap.equipStatus);

                    UKMod.SetPersistentModData($@"{weap.modName}.{weap.id}.load", loadOrder, "ULTRAKIT");
                    UKMod.SetPersistentModData($@"{weap.modName}.{weap.id}.equip", equippedStatus, "ULTRAKIT");

                    for (int i = 0; i < weap.All_Variants.Length; i++)
                    {
                        var variant = weap.All_Variants[i];

                        if (!equippedDict.ContainsKey(variant))
                        {
                            int s = (int)Mathf.Repeat(i, weap.Variants.Length);
                            bool equipped = (i < weap.Variants.Length && weap.equipStatus[s] == 1) || (i >= weap.Variants.Length && weap.equipStatus[s] == 2);
                            equippedDict.Add(variant, equipped);
                        }
                    }
                    for (int i = 0; i < weap.Variants.Length; i++)
                    {
                        var variant = weap.All_Variants[weap.equipOrder[i]];
                        if (weap.equipStatus[weap.equipOrder[i]] == 2)
                        {
                            variant = weap.All_Variants[weap.equipOrder[i] + weap.Variants.Length];
                        }

                        if (!equippedDict[variant])
                        {
                            continue;
                        }

                        var go = GameObject.Instantiate(variant, __instance.transform);
                        go.SetActive(false);

                        foreach (var c in go.GetComponentsInChildren<Renderer>(true))
                        {
                            c.gameObject.layer = LayerMask.NameToLayer("AlwaysOnTop");

                            var glow = c.gameObject.GetComponent<Glow>();

                            if (glow)
                            {
                                c.material.shader = Shader.Find("psx/railgun");
                                c.material.SetFloat("_EmissivePosition", 5);
                                c.material.SetFloat("_EmissiveStrength", glow.glowIntensity);
                                c.material.SetColor("_EmissiveColor", glow.glowColor);
                            }
                            else
                            {
                                c.material.shader = Shader.Find(c.material.shader.name);
                            }   
                        }

                        var wi = go.AddComponent<WeaponIcon>();
                        wi.weaponIcon = weap.Icons[i];
                        wi.glowIcon = weap.Icons[i];
                        wi.variationColor = i;
                        wi.SetPrivate("variationColoredMaterials", go.GetComponentsInChildren<Material>().Where(k => k.name.Contains(".var")).ToArray() ?? new Material[0]);
                        wi.SetPrivate("variationColoredRenderers", go.GetComponentsInChildren<Renderer>().Where(k => k.material.name.Contains(".var")).ToArray() ?? new Renderer[0]);
                        wi.SetPrivate("variationColoredImages", new Image[0]);

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
        }
    }

    [HarmonyPatch(typeof(GunControl))]
    class GunControlPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void StartPrefix(GunControl __instance)
        {
            if (PlayerPrefs.GetInt("CurSlo", 1) > __instance.slots.Count)
            {
                PlayerPrefs.SetInt("CurSlo", 1);
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePostfix(GunControl __instance)
        {
            if (MonoSingleton<InputManager>.Instance.InputSource.Slot7.WasPerformedThisFrame && __instance.slots.Count >= 7 && (__instance.slots[6]?.Count > 0 || __instance.currentSlot != 7))
            {
                if (__instance.slots[6]?.Count > 0 && __instance.slots[6][0] != null)
                {
                    __instance.SwitchWeapon(7, __instance.slots[6]);
                }
            }

            if (MonoSingleton<InputManager>.Instance.InputSource.Slot8.WasPerformedThisFrame && __instance.slots.Count >= 8 && (__instance.slots[7]?.Count > 0 || __instance.currentSlot != 8))
            {
                if (__instance.slots[7]?.Count > 0 && __instance.slots[7][0] != null)
                {
                    __instance.SwitchWeapon(8, __instance.slots[7]);
                }
            }

            if (MonoSingleton<InputManager>.Instance.InputSource.Slot9.WasPerformedThisFrame && __instance.slots.Count >= 9 && (__instance.slots[8]?.Count > 0 || __instance.currentSlot != 9))
            {
                if (__instance.slots[8]?.Count > 0 && __instance.slots[8][0] != null)
                {
                    __instance.SwitchWeapon(9, __instance.slots[8]);
                }
            }
        }
    }
}
