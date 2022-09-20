using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

namespace ULTRAKIT.Loader.Injectors
{
    [HarmonyPatch(typeof(VariationInfo))]
    public static class VariationInfoInjections
    {
        [HarmonyPatch("ChangeEquipment")]
        [HarmonyPrefix]
        public static bool ChangeEquipmentPrefix(VariationInfo __instance)
        {
            return __instance.enabled;
        }
    }

    [HarmonyPatch(typeof(ShopGearChecker))]
    public static class ShopInjector
    {
        static GameObject buttonTemplate, panelTemplate, variantTemplate;
        static int buttonHeight = 30, variantHeight = 80;

        static Dictionary<Weapon, GameObject> panels;
        static List<List<GameObject>> pages;
        static int page;

        static GameObject pageButton;

        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        public static void TurnOnPostFix(ShopGearChecker __instance)
        {
            if (!pageButton)
            {
                page = 0;

                buttonTemplate = __instance.GetComponentInChildren<ShopButton>(true).gameObject;
                variantTemplate = __instance.GetComponentInChildren<VariationInfo>(true).gameObject;
                panelTemplate = variantTemplate.transform.parent.gameObject;

                panels = CreatePanels(__instance.gameObject);
                pages = CreatePages(__instance.gameObject);
                pageButton = CreatePageButton();
            }

            UpdatePage();
        }

        static List<List<GameObject>> CreatePages(GameObject parent)
        {
            var res = new List<List<GameObject>>();

            res.Add(new List<GameObject>());
            foreach (var vanillaButton in parent.gameObject.GetComponentsInChildren<ShopButton>())
            {
                foreach (var panel in panels.Values)
                {
                    vanillaButton.toDeactivate = vanillaButton.toDeactivate.AddItem(panel).ToArray();
                }
                res[0].Add(vanillaButton.gameObject);
            }

            var allWeaps = WeaponLoader.allWeapons;
            int curPage = 1;
            int curWeap = 0;
            res.Add(new List<GameObject>());
            foreach (var weap in allWeaps)
            {
                res[curPage].Add(CreateWeaponButton(parent, weap, curWeap));

                curWeap++;
                if (curWeap > 5)
                {
                    res.Add(new List<GameObject>());
                    curPage++;
                    curWeap = 0;
                }
            }

            return res;
        }

        static GameObject CreateWeaponButton(GameObject parent, Weapon weap, int i)
        {
            var go = GameObject.Instantiate(buttonTemplate, parent.transform);

            var top = (80) - (buttonHeight * i);
            var rect = go.GetComponent<RectTransform>();
            rect.offsetMax = new Vector2(-100, top);
            rect.offsetMin = new Vector2(-260, top - buttonHeight);

            go.GetComponentInChildren<Text>().text = weap.Names[i].ToUpper();

            go.GetComponent<ShopButton>().deactivated = true;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (var pair in panels)
                {
                    pair.Value.SetActive(panels[weap] == pair.Value);
                }

                foreach (Transform obj in parent.transform)
                {
                    if (obj == parent.transform)
                    {
                        continue;
                    }

                    if (obj.GetComponentInChildren<VariationInfo>(true) != null)
                    {
                        obj.gameObject.SetActive(panels[weap] == obj.gameObject);
                    }
                }
            });

            go.AddComponent<HudOpenEffect>();
            return go;
        }

        static Dictionary<Weapon, GameObject> CreatePanels(GameObject parent)
        {
            var res = new Dictionary<Weapon, GameObject>();
            var allWeaps = WeaponLoader.allWeapons;
            foreach (var weap in allWeaps)
            {
                var go = GameObject.Instantiate(panelTemplate, parent.transform);
                foreach (Transform child in go.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                for (int i = 0; i < weap.Variants.Length; i++)
                {
                    CreateVariantOption(go, weap, i);
                }

                res.Add(weap, go);
            }

            return res;
        }

        static GameObject CreateVariantOption(GameObject panel, Weapon weapon, int i)
        {
            var go = GameObject.Instantiate(variantTemplate, panel.transform);
            var info = go.GetComponent<VariationInfo>();
            info.enabled = false;

            var cbget = info.GetComponentInChildren<ColorBlindGet>();
            cbget.GetComponent<Image>().sprite = weapon.Icons[i];
            cbget.variationNumber = i;
            info.transform.Find("Text").GetComponent<Text>().text = weapon.Names[i];
            info.transform.Find("Text (1)").gameObject.SetActive(false);

            var equipmentStuffs = info.transform.Find("EquipmentStuffs");
            Button lb = equipmentStuffs.Find("PreviousButton").GetComponent<Button>();
            Button rb = equipmentStuffs.Find("NextButton").GetComponent<Button>();

            Image img = info.equipButton.transform.GetChild(0).GetComponent<Image>();

            img.sprite = info.equipSprites[weapon.equipStatus[i]];

            UnityAction delL = () =>
            {
                int add = (weapon.AltVariants.Length > i) && (weapon.AltVariants[i] != null) ? 1 : 0;
                weapon.equipStatus[i] = (int)Mathf.Repeat(weapon.equipStatus[i] - 1, 2 + add);
                GunSetterPatch.equippedDict[weapon.All_Variants[i]] = weapon.equipStatus[i] == 1;
                if (add == 1) GunSetterPatch.equippedDict[weapon.All_Variants[i + weapon.Variants.Length]] = weapon.equipStatus[i] == 2;
                img.sprite = info.equipSprites[weapon.equipStatus[i]];
                MonoSingleton<GunSetter>.Instance.ResetWeapons();
            };

            UnityAction delR = () =>
            {
                int add = (weapon.AltVariants.Length > i) && (weapon.AltVariants[i] != null) ? 1 : 0;
                weapon.equipStatus[i] = (int)Mathf.Repeat(weapon.equipStatus[i] + 1, 2 + add);
                GunSetterPatch.equippedDict[weapon.All_Variants[i]] = weapon.equipStatus[i] == 1;
                if (add == 1) GunSetterPatch.equippedDict[weapon.All_Variants[i + weapon.Variants.Length]] = weapon.equipStatus[i] == 2;
                img.sprite = info.equipSprites[weapon.equipStatus[i]];
                MonoSingleton<GunSetter>.Instance.ResetWeapons();
            };

            lb.onClick.AddListener(delL);
            rb.onClick.AddListener(delR);

            go.GetComponent<RectTransform>().offsetMax = new Vector2(0, 160 - (i * variantHeight));
            go.GetComponent<RectTransform>().offsetMin = new Vector2(-360, 80 - (i * variantHeight));

            return go;
        }

        static GameObject CreatePageButton()
        {
            var pageGo = GameObject.Instantiate(buttonTemplate, buttonTemplate.transform.parent);

            var pageTop = (-10) - (buttonHeight * 4);
            var pageRect = pageGo.GetComponent<RectTransform>();
            pageRect.offsetMax = new Vector2(-100, -130);
            pageRect.offsetMin = new Vector2(-360, -160);

            pageGo.GetComponentInChildren<Text>().text = $"PAGE {page + 1}";

            pageGo.GetComponent<ShopButton>().deactivated = true;
            pageGo.GetComponent<Button>().onClick.AddListener(() =>
            {
                page++;
                UpdatePage();
            });

            pageGo.AddComponent<HudOpenEffect>();
            return pageGo;
        }

        static void UpdatePage()
        {
            page = (int)Mathf.Repeat(page, pages.Count);

            pageButton.GetComponentInChildren<Text>().text = $"PAGE {page + 1}";
            var index = 0;
            foreach (var pageList in pages)
            {
                foreach (var pageObj in pageList)
                {
                    pageObj.SetActive(page == index);
                }
                index++;
            }
        }
    }
}
