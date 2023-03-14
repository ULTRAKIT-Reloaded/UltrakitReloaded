using HarmonyLib;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Managers;
using ULTRAKIT.Extensions.ObjectClasses;
using ULTRAKIT.Loader.Loaders;
using UnityEditor;
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
        private static GameObject Menu, MenuButtonTemplate, SubmenuTemplate, TextTemplate, SliderTemplate, CheckboxTemplate, PickerTemplate, KeyTemplate;
        private static GameObject NewButton;

        private static List<string> ButtonsToMove;
        private static List<GameObject> NewMenus;
        private static string CachedHeader;
        private static string CachedModHeader;
        private static string CachedMenu;

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
                TextTemplate = Menu.transform.Find("Controls Options/Scroll Rect/Contents/Weapons Settings/Text (1)").gameObject;
                SliderTemplate = SubmenuTemplate.transform.Find("Scroll Rect (1)/Contents/Mouse Sensitivity").gameObject;
                CheckboxTemplate = SubmenuTemplate.transform.Find("Scroll Rect (1)/Contents/Variation Memory").gameObject;
                PickerTemplate = SubmenuTemplate.transform.Find("Scroll Rect (1)/Contents/Weapon Position").gameObject;
                KeyTemplate = Menu.transform.Find("Controls Options/Scroll Rect/Contents/Forward").gameObject;

                for (int i = 0; i < ButtonsToMove.Count; i++)
                {
                    string button = ButtonsToMove[i];
                    if (button == "Saves")
                        continue;
                    if (button == "Colors")
                    {
                        RegisterMenu(button, Menu.transform.Find("ColorBlindness Options").gameObject);
                        continue;
                    }
                    RegisterMenu(button, Menu.transform.Find($"{button} Options").gameObject);
                }

                foreach (string button in ButtonsToMove)
                {
                    Transform t = Menu.transform.Find(button);
                    t.localPosition += new Vector3(0, -35, 0);
                }
            }

            if (NewButton == null)
                NewButton = CreateMenuButton("Mods");
            CreateMenuButton("Test Button");
            foreach (UKSetting setting in Registries.options_registry.Values)
            {
                CreateSetting(setting);
            }
        }

        static GameObject CreateMenuButton(string name)
        {
            string internal_name = name.Dehumanize();
            GameObject newBtn = GameObject.Instantiate(MenuButtonTemplate, Menu.transform);
            newBtn.name = internal_name;
            newBtn.GetComponent<RectTransform>().SetAsFirstSibling();
            newBtn.transform.Find("Text").GetComponent<Text>().text = name.ToUpper();
            newBtn.transform.localPosition = new Vector3(-610 - 320, -245, 0);

            GameObject Submenu = CreateSubmenu(name);
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

            ButtonsToMove.Add(internal_name);
            NewMenus.Add(Submenu);

            return newBtn;
        }

        static GameObject CreateSubmenu(string title)
        {
            string internal_name = title.Dehumanize();
            GameObject sub = GameObject.Instantiate(SubmenuTemplate, Menu.transform);
            sub.name = $"{internal_name} Options";
            sub.GetComponent<RectTransform>().SetAsFirstSibling();
            Transform contents = sub.transform.Find("Scroll Rect (1)/Contents");
            for (int i = 0; i < contents.childCount; i++)
            {
                GameObject.Destroy(contents.GetChild(i).gameObject);
            }
            sub.transform.Find("Text").GetComponent<Text>().text = $"--{title.ToUpper()}--";
            RegisterMenu(internal_name, sub);
            return sub;
        }

        static void CreateSetting(UKSetting setting)
        {
            GameObject submenu;
            bool isModSubmenu = false;
            if (!Registries.options_menus.ContainsKey(setting.Section))
            {
                submenu = Registries.options_menus["Mods"];
                isModSubmenu = true;
            }
            else
                submenu = Registries.options_menus[setting.Section];

            Transform parent = submenu.transform.Find("Scroll Rect (1)/Contents") ?? submenu.transform.Find("Scroll Rect/Contents") ?? submenu.transform.Find("Image");

            if (isModSubmenu && CachedModHeader != setting.Section)
            {
                Text t = GameObject.Instantiate(TextTemplate, parent).GetComponent<Text>();
                t.text = $"--{setting.Section}--";
                t.fontSize += 6;
            }

            if (CachedHeader != setting.Heading || (CachedMenu != setting.Section && !isModSubmenu))
            {
                GameObject.Instantiate(TextTemplate, parent).GetComponent<Text>().text = $"--{setting.Heading}--";
                CachedHeader = setting.Heading;
                CachedMenu = setting.Section;
            }

            Type settingType = setting.GetType();
            if (settingType.IsEquivalentTo(typeof(UKCheckbox)))
            {
                CreateCheckbox(setting as UKCheckbox, parent);
            }
            if (settingType.IsEquivalentTo(typeof(UKSlider)))
            {
                CreateSlider(setting as UKSlider, parent);
            }
            if (settingType.IsEquivalentTo(typeof(UKPicker)))
            {
                CreatePicker(setting as UKPicker, parent);
            }
            if (settingType.IsEquivalentTo(typeof(UKKeySetting)))
            {
                CreateKey(setting as UKKeySetting, parent);
            }
        }

        static void CreateCheckbox(UKCheckbox setting, Transform contents)
        {
            GameObject box = GameObject.Instantiate(CheckboxTemplate, contents);
            Toggle toggle = box.transform.Find("Toggle").GetComponent<Toggle>();

            toggle.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
            toggle.onValueChanged.AddListener((bool value) =>
            {
                setting.SetValue(value);
            });

            box.transform.Find("Text").GetComponent<Text>().text = setting.Name.Humanize();
        }

        static void CreateSlider(UKSlider setting, Transform contents)
        {
            GameObject box = GameObject.Instantiate(SliderTemplate, contents);
            Slider slider = box.transform.Find("Button/Slider (1)").GetComponent<Slider>();
            Text currentValueText = slider.transform.Find("Text (2)").GetComponent<Text>();

            slider.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
            slider.onValueChanged.AddListener((float value) =>
            {
                setting.SetValue(value);
                currentValueText.text = value.ToString();
            });

            box.transform.Find("Text").GetComponent<Text>().text = setting.Name.Humanize();
        }
        
        static void CreatePicker(UKPicker setting, Transform contents)
        {
            GameObject box = GameObject.Instantiate(PickerTemplate, contents);
            Dropdown dropdown = box.transform.Find("Dropdown").GetComponent<Dropdown>();

            dropdown.ClearOptions();
            dropdown.AddOptions(setting.Options.ToList());

            dropdown.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
            dropdown.onValueChanged.AddListener((int value) =>
            {
                setting.SetValue(value);
            });

            box.transform.Find("Text").GetComponent<Text>().text = setting.Name.Humanize();
        }

        static void CreateKey(UKKeySetting setting, Transform contents)
        {
            GameObject box = GameObject.Instantiate(KeyTemplate, contents);
            GameObject button = box.transform.Find("W").gameObject;

            button.name = setting.Binding.PrefName;

            box.transform.Find("Text").GetComponent<Text>().text = setting.Name.Humanize();
        }

        static void RegisterMenu(string buttonName, GameObject menu)
        {
            if (!Registries.options_menus.ContainsKey(buttonName))
                Registries.options_menus.Add(buttonName, menu);
            else
                Registries.options_menus[buttonName] = menu;
        }

        // DELETE
        public static void TestSystem()
        {
            UKCheckbox checkbox = new UKCheckbox("Mods", "Testing Section", "Test Checkbox", false);
            UKSlider slider = new UKSlider("Mods", "Testing Section", "Test Slider", 0f, 1f, 0.2f);
            UKPicker picker = new UKPicker("Mods", "Testing Section", "Test Picker", new string[] { "Option1", "Option2", "Option3" }, 0);

            Registries.options_registry.Add(checkbox.Heading, checkbox);
            Registries.options_registry.Add(slider.Heading, slider);
            Registries.options_registry.Add(picker.Heading, picker);

            KeybindsLoader.SetKeyBind("Test", "test bind", KeyCode.G);
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

    [HarmonyPatch(typeof(ControlsOptions))]
    public class ControlsOptionsPatch
    {
        public static UKKeySetting currentKey;

        [HarmonyPatch("OnGUI"), HarmonyPostfix]
        static void OnGUIPostix()
        {
            if (currentKey != null)
                currentKey.SetValue(InputManager.instance.Inputs[currentKey.Binding.PrefName]);
        }
    }
}
