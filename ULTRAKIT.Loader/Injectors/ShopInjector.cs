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
using TMPro;

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

                // Initializes templates
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
                // Sets vanilla buttons to also disable modded panels
                foreach (var panel in panels.Values)
                {
                    vanillaButton.toDeactivate = vanillaButton.toDeactivate.AddItem(panel).ToArray();
                }
                res[0].Add(vanillaButton.gameObject);
            }

            var allWeaps = Registries.weap_allWeapons;
            int curPage = 1;
            int curWeap = 0;
            res.Add(new List<GameObject>());
            foreach (var weap in allWeaps)
            {
                // Skips locked weapons
                if (!weap.Unlocked)
                {
                    continue;
                }
                
                // Adds a button for each new weapon
                res[curPage].Add(CreateWeaponButton(parent, weap, curWeap));

                // Loops over to new page
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
            // Puts the button behind panels
            rect.SetAsFirstSibling();

            // Base panel is labelled using the first weapon name
            go.GetComponentInChildren<TextMeshProUGUI>().text = weap.Names[0].ToUpper();

            go.GetComponent<ShopButton>().deactivated = true;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                // Deactivates base panel on the right holding variant panels for all weapons, except enables for the clicked weapon button
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

                    // Deactates variant panels for all weapons, except enabled for the clicked weapon button
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
            var allWeaps = Registries.weap_allWeapons;
            foreach (var weap in allWeaps)
            {
                var go = GameObject.Instantiate(panelTemplate, parent.transform);
                foreach (Transform child in go.transform)
                {
                    // Clears the revolver variants from the cloned template
                    GameObject.Destroy(child.gameObject);
                }

                for (int i = 0; i < weap.Variants.Length; i++)
                {
                    CreateVariantOption(go, weap, i);
                }

                // Searches about four steps down the heirarchy of each variant panel for the equip order text, then sets it to the weapon's saved order data
                TextMeshProUGUI[] orderInfo = go.GetComponentsInChildren<TextMeshProUGUI>(true).Where(k => k.transform.parent.name == "Order").ToArray();
                for (int n = 0; n < weap.Variants.Length; n++)
                {
                    orderInfo[n].text = (weap.equipOrder[n] + 1).ToString();
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

            // Sets variant name
            info.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = weapon.Names[i];
            // Disables "Already Owned" text - Possible use in future
            info.transform.Find("Text (1)").gameObject.SetActive(false);

            // Equip/Alt buttons
            var equipmentStuffs = info.transform.Find("EquipmentStuffs");
            Button lb = equipmentStuffs.Find("PreviousButton").GetComponent<Button>();
            Button rb = equipmentStuffs.Find("NextButton").GetComponent<Button>();

            // Order buttons
            // Disables WeaponOrderController to manage it internally
            Transform ord = equipmentStuffs.Find("Order");
            ord.GetComponent<WeaponOrderController>().enabled = false;
            GameObject ordUp = ord.Find("UpButton").gameObject;
            GameObject ordDown = ord.Find("DownButton").gameObject;
            Image ordIcon = ord.Find("Image").GetComponent<Image>();

            Image img = info.equipButton.transform.GetChild(0).GetComponent<Image>();

            // Equipped, Alternate, Unequipped
            img.sprite = info.equipSprites[weapon.equipStatus[i]];

            // Left equip button
            UnityAction delL = () =>
            {
                // Loops equipped status, checking if an alternate exists
                int add = (weapon.AltVariants.Length > i) && (weapon.AltVariants[i] != null) ? 1 : 0;
                weapon.equipStatus[i] = (int)Mathf.Repeat(weapon.equipStatus[i] - 1, 2 + add);

                // Equips a standard variant if set to standard, or disables it otherwise.
                // Then checks the alternate variant and equips it if set to alt, or disables it otherwise.
                GunSetterPatch.equippedDict[weapon.All_Variants[i]] = weapon.equipStatus[i] == 1;
                if (add == 1) GunSetterPatch.equippedDict[weapon.All_Variants[i + weapon.Variants.Length]] = weapon.equipStatus[i] == 2;
                img.sprite = info.equipSprites[weapon.equipStatus[i]];

                MonoSingleton<GunSetter>.Instance.ResetWeapons();
            };

            // Right equip button
            UnityAction delR = () =>
            {
                // Loops equipped status, checking if an alternate exists
                int add = (weapon.AltVariants.Length > i) && (weapon.AltVariants[i] != null) ? 1 : 0;
                weapon.equipStatus[i] = (int)Mathf.Repeat(weapon.equipStatus[i] + 1, 2 + add);

                // Equips a standard variant if set to standard, or disables it otherwise.
                // Then checks the alternate variant and equips it if set to alt, or disables it otherwise.
                GunSetterPatch.equippedDict[weapon.All_Variants[i]] = weapon.equipStatus[i] == 1;
                if (add == 1) GunSetterPatch.equippedDict[weapon.All_Variants[i + weapon.Variants.Length]] = weapon.equipStatus[i] == 2;
                img.sprite = info.equipSprites[weapon.equipStatus[i]];

                MonoSingleton<GunSetter>.Instance.ResetWeapons();
            };

            // Top order button
            UnityAction ordU = () =>
            {
                TextMeshProUGUI[] orderInfo = info.transform.parent.GetComponentsInChildren<TextMeshProUGUI>().Where(k => k.transform.parent.name == "Order").ToArray();

                // Finds the target number (current + 1), sets the weapon currently at the target number to current number, then sets current to the target number
                int target = (int)Mathf.Repeat(weapon.equipOrder[i] + 1, weapon.Variants.Length);
                weapon.equipOrder[weapon.equipOrder.FindIndexOf(target)] = weapon.equipOrder[i];
                weapon.equipOrder[i] = target;

                for (int n = 0; n < weapon.Variants.Length; n++)
                {
                    orderInfo[n].text = (weapon.equipOrder[n] + 1).ToString();
                }

                MonoSingleton<GunSetter>.Instance.ResetWeapons();
            };

            // Bottom order button
            UnityAction ordD = () =>
            {
                TextMeshProUGUI[] orderInfo = info.transform.parent.GetComponentsInChildren<TextMeshProUGUI>().Where(k => k.transform.parent.name == "Order").ToArray();

                // Finds the target number (current - 1), sets the weapon currently at the target number to current number, then sets current to the target number
                int target = (int)Mathf.Repeat(weapon.equipOrder[i] - 1, weapon.Variants.Length);
                weapon.equipOrder[weapon.equipOrder.FindIndexOf(target)] = weapon.equipOrder[i];
                weapon.equipOrder[i] = target;

                for (int n = 0; n < weapon.Variants.Length; n++)
                {
                    orderInfo[n].text = (weapon.equipOrder[n] + 1).ToString();
                }

                MonoSingleton<GunSetter>.Instance.ResetWeapons();
            };

            UnityEvent orderClickUp = new UnityEvent();
            UnityEvent orderClickDown = new UnityEvent();
            orderClickUp.AddListener(ordU);
            orderClickDown.AddListener(ordD);

            // Reflects the internal ControllerPointer component to replace its function with internal order management
            Type controllerPointer = ReflectionExt.GetInternalType("ControllerPointer");
            var btu = ordUp.GetComponent(controllerPointer);
            var field = btu.GetType().GetField("onPressed", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(btu, orderClickUp);

            var btd = ordDown.GetComponent(controllerPointer);
            var field2 = btd.GetType().GetField("onPressed", BindingFlags.NonPublic | BindingFlags.Instance);
            field2.SetValue(btd, orderClickDown);

            lb.onClick.AddListener(delL);
            rb.onClick.AddListener(delR);

            // Sets each variant panel to display variant icon
            ordIcon.sprite = weapon.Icons[i];
            ordIcon.color = new Color(MonoSingleton<ColorBlindSettings>.Instance.variationColors[i].r, MonoSingleton<ColorBlindSettings>.Instance.variationColors[i].g, MonoSingleton<ColorBlindSettings>.Instance.variationColors[i].b, ordIcon.color.a);
            ord.gameObject.SetActive(true);

            go.GetComponent<RectTransform>().offsetMax = new Vector2(0, 160 - (i * variantHeight));
            go.GetComponent<RectTransform>().offsetMin = new Vector2(-360, 80 - (i * variantHeight));

            return go;
        }

        static GameObject CreatePageButton()
        {
            var pageGo = GameObject.Instantiate(buttonTemplate, buttonTemplate.transform.parent);

            var pageTop = (-10) - (buttonHeight * 4);
            var pageRect = pageGo.GetComponent<RectTransform>();
            pageRect.offsetMax = new Vector2(-100, -130 + (Initializer.isWaffle ? buttonHeight * 2 : 0));
            pageRect.offsetMin = new Vector2(-260, -160 + (Initializer.isWaffle ? buttonHeight * 2 : 0));
            // Keeps the button behind popup panels
            pageRect.SetAsFirstSibling();

            pageGo.GetComponentInChildren<TextMeshProUGUI>().text = $"PAGE {page + 1}";

            // Replaces button template function
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
            // Loops page if necessary
            page = (int)Mathf.Repeat(page, pages.Count);

            pageButton.GetComponentInChildren<TextMeshProUGUI>(true).text = $"PAGE {page + 1}";
            var index = 0;
            foreach (var pageList in pages)
            {
                foreach (var pageObj in pageList)
                {
                    // BROKEN HERE
                    if (pageObj != null)
                        pageObj.SetActive(page == index);
                }
                index++;
            }
        }
    }
}
