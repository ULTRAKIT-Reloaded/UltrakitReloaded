using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Managers;
using ULTRAKIT.Extensions.ObjectClasses;
using ULTRAKIT.Loader.Loaders;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ULTRAKIT.Loader.Injectors
{
    [HarmonyPatch(typeof(CanvasController))]
    public class OptionsInjector
    {
        private static GameObject Menu, MenuButtonTemplate, SubmenuTemplate, SliderTemplate, CheckboxTemplate, PickerTemplate, KeyTemplate;
        private static GameObject NewButton;

        private static List<string> ButtonsToMove;
        private static List<GameObject> NewMenus;

        [HarmonyPatch("OnEnable"), HarmonyPostfix]
        static void OnEnablePostfix(CanvasController __instance)
        {
            ButtonsToMove = new List<string>() { "Gameplay", "Controls", "Video", "Audio", "HUD", "Assist", "Colors", "Saves" };
            NewMenus = new List<GameObject>();

            if (Menu == null)
            {
                Menu = CanvasController.Instance.transform.Find("OptionsMenu").gameObject;
                MenuButtonTemplate = Menu.transform.Find("Gameplay").gameObject;
                SubmenuTemplate = Menu.transform.Find("Gameplay Options").gameObject;
                SliderTemplate = SubmenuTemplate.transform.Find("Scroll Rect (1)/Contents/Mouse Sensitivity").gameObject;
                CheckboxTemplate = SubmenuTemplate.transform.Find("Scroll Rect (1)/Contents/Variation Memory").gameObject;
                PickerTemplate = SubmenuTemplate.transform.Find("Scroll Rect (1)/Contents/Weapon Position").gameObject;
                KeyTemplate = Menu.transform.Find("Controls Options/Scroll Rect/Contents/Forward").gameObject;

                foreach (string button in ButtonsToMove)
                {
                    UKLogger.Log(button);
                    Transform t = Menu.transform.Find(button);
                    t.localPosition += new Vector3(0, -35, 0);
                }
            }
            if (NewButton == null)
                NewButton = CreateMenuButton("Mods");
            CreateMenuButton("Test Button");
        }

        static GameObject CreateMenuButton(string name)
        {
            GameObject newBtn = GameObject.Instantiate(MenuButtonTemplate, Menu.transform);
            newBtn.name = name;
            newBtn.GetComponent<RectTransform>().SetAsFirstSibling();
            newBtn.transform.Find("Text").GetComponent<Text>().text = name.ToUpper();
            newBtn.transform.localPosition = new Vector3(-610 - 320, -245, 0);

            GameObject Submenu = CreateSubmenu(name.ToUpper());
            Submenu.SetActive(false);

            Button btn = newBtn.GetComponent<Button>();
            btn.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
            btn.onClick.AddListener(() =>
            {
                SubmenuTemplate.SetActive(false);
                Submenu.SetActive(true);
            });

            foreach (string button in ButtonsToMove)
            {
                Transform t = Menu.transform.Find(button);
                t.localPosition += new Vector3(0, 60, 0);
                t.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Submenu.SetActive(false);
                });
            }
            foreach (GameObject menu in NewMenus)
            {
                btn.onClick.AddListener(() =>
                {
                    menu.SetActive(false);
                });
            }

            ButtonsToMove.Add(name);
            NewMenus.Add(Submenu);

            return newBtn;
        }

        static GameObject CreateSubmenu(string title)
        {
            GameObject sub = GameObject.Instantiate(SubmenuTemplate, Menu.transform);
            sub.name = "Mod Options";
            sub.GetComponent<RectTransform>().SetAsFirstSibling();
            Transform contents = sub.transform.Find("Scroll Rect (1)/Contents");
            for (int i = 0; i < contents.childCount; i++)
            {
                GameObject.Destroy(contents.GetChild(i).gameObject);
            }
            sub.transform.Find("Text").GetComponent<Text>().text = $"--{title}--";
            return sub;
        }
    }

    [HarmonyPatch(typeof(InputManager))]
    public class KeybindsInjector
    {
        [HarmonyPatch("OnEnable"), HarmonyPostfix]
        static void OnEnablePostfix(InputManager __instance)
        {
            __instance.bindings = __instance.bindings.AddRangeToArray(Registries.key_registry.Select(n => n.Value.Binding).ToArray());
            Registries.key_states = Registries.key_registry.ToDictionary(item => item.Key, item => new InputActionState(item.Value.Binding.Action));
            foreach (UKKeySetting setting in Registries.key_registry.Values)
            {
                if (!PrefsManager.Instance.HasKey(setting.Binding.PrefName))
                    PrefsManager.instance.prefMap.Add(setting.Binding.PrefName, setting.Binding.DefaultKey);
                setting.Binding.Action.AddBinding().WithGroup("Keyboard");
                setting.Binding.Action.Enable();
            }
            __instance.UpdateBindings();
            // DELETE
            KeybindsLoader.GetKeyBind("test bind", out HatsManager.state);
        }
    }
}
