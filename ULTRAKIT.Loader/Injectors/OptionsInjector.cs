using HarmonyLib;
using Humanizer;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Data;
using ULTRAKIT.Extensions.Managers;
using ULTRAKIT.Extensions.Classes;
using ULTRAKIT.Loader.Loaders;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ULTRAKIT.Loader.Injectors
{
    [HarmonyPatch(typeof(CanvasController))]
    public class OptionsInjector
    {
        private static GameObject Menu, MenuButtonTemplate, SubmenuTemplate, TextTemplate, SliderTemplate, CheckboxTemplate, PickerTemplate, KeyTemplate;
        public static GameObject NewButton;
        public static bool Rebuilding = false;
        private static bool inMenu;
        private static bool isLoaded;

        // Cry about it
        private static List<string> ButtonsToMove;
        private static List<string> VanillaButtons = new List<string>() { "Gameplay", "Controls", "Video", "Audio", "HUD", "Assist", "Colors", "Saves" };
        private static Dictionary<string, Vector3> OriginalPos = new Dictionary<string, Vector3>();
        private static List<GameObject> NewMenus;
        private static string CachedHeader;
        private static string CachedModHeader;
        private static string CachedMenu;
        private static float ControlOffset;
        private static float SizeDeltaOffset;
        private static float OriginalScrollHeight;

        [HarmonyPatch("OnEnable"), HarmonyPostfix]
        static void OnEnablePostfix(CanvasController __instance)
        {
            // Localposition is the same in main and level menus, but sets differently (i.e. 930 actually results in 610, but only in the main menu)
            inMenu = SceneManager.GetActiveScene().name == "Main Menu";
            ButtonsToMove = new List<string>();
            ButtonsToMove.AddRange(VanillaButtons);
            NewMenus = new List<GameObject>();
            ClearCaches();

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

                OriginalScrollHeight = Menu.transform.Find("Controls Options/Scroll Rect/Contents").GetComponent<RectTransform>().sizeDelta.y;

                for (int i = 0; i < ButtonsToMove.Count; i++)
                {
                    string button = ButtonsToMove[i];

                    if (OriginalPos.Count < 8)
                    {
                        Vector3 localPos = Menu.transform.Find(button).localPosition;
                        Vector3 pos = new Vector3(-610, localPos.y, localPos.z);
                        OriginalPos.Add(button, pos);
                    }

                    if (button == "Saves")
                        continue;
                    if (button == "Colors")
                    {
                        RegisterButton(button, Menu.transform.Find(button).gameObject);
                        RegisterMenu(button, Menu.transform.Find("ColorBlindness Options").gameObject);
                        continue;
                    }
                    RegisterButton(button, Menu.transform.Find(button).gameObject);
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
            foreach (string menuToAdd in Registries.options_menusToAdd)
                CreateMenuButton(menuToAdd);
            foreach (UKSetting setting in Registries.options_registry.Values)
                CreateSetting(setting);
            Rebuilding = false;
        }

        internal static GameObject CreateMenuButton(string name)
        {
            string internal_name = name.Dehumanize();
            GameObject newBtn = GameObject.Instantiate(MenuButtonTemplate, Menu.transform);
            newBtn.name = internal_name;
            newBtn.GetComponent<RectTransform>().SetAsFirstSibling();
            newBtn.transform.Find("Text").GetComponent<Text>().text = name.ToUpper();
            newBtn.transform.localPosition = new Vector3(inMenu && !Rebuilding ? -930 : -610, -245, 0);

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
            RegisterButton(internal_name, newBtn);
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
            if (Rebuilding && setting.Section == "Controls") return;

            GameObject submenu;
            bool isModSubmenu = false;
            if (!Registries.options_menus.ContainsKey(setting.Section.Dehumanize()))
            {
                submenu = Registries.options_menus["Mods"];
                isModSubmenu = true;
            }
            else
                submenu = Registries.options_menus[setting.Section.Dehumanize()];

            Transform parent = submenu.transform.Find("Scroll Rect (1)/Contents") ?? submenu.transform.Find("Scroll Rect/Contents") ?? submenu.transform.Find("Image");

            if (isModSubmenu && CachedModHeader != setting.Section)
            {
                Text t = GameObject.Instantiate(TextTemplate, parent).GetComponent<Text>();
                t.text = $"--{setting.Section}--";
                t.fontSize += 6;
                t.alignment = TextAnchor.MiddleLeft;
                t.horizontalOverflow = HorizontalWrapMode.Overflow;
                CachedModHeader = setting.Section;
                CachedHeader = null;
            }

            if (!(Initializer.isUMMInstalled && setting.Section == "Controls") && (CachedHeader != setting.Heading || (CachedMenu != setting.Section && !isModSubmenu)))
            {
                Text t = GameObject.Instantiate(TextTemplate, parent).GetComponent<Text>();
                t.text = $"--{setting.Heading}--";
                t.alignment = TextAnchor.MiddleLeft;
                t.horizontalOverflow = HorizontalWrapMode.Overflow;
                CachedHeader = setting.Heading;
                CachedMenu = setting.Section;

                if (setting.Section == "Controls")
                {
                    Transform lastChild = parent.GetChild(parent.childCount - 1);
                    if (lastChild == t.transform)
                        lastChild = parent.GetChild(parent.childCount - 2);

                    RectTransform rect = lastChild.GetComponent<RectTransform>();
                    float t_offset = rect.sizeDelta.y - 210;
                    ControlOffset = lastChild.transform.localPosition.y - t_offset;
                    SizeDeltaOffset = t.GetComponent<RectTransform>().sizeDelta.y;

                    t.transform.localPosition = new Vector3(t.transform.localPosition.x, ControlOffset,  t.transform.localPosition.z);
                }
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
            if (settingType.IsEquivalentTo(typeof(UKKeySetting)) && !Initializer.isUMMInstalled)
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
                SaveData.Save();
            });

            toggle.isOn = setting.Value;
            box.transform.Find("Text").GetComponent<Text>().text = setting.Name.Humanize();
        }

        static void CreateSlider(UKSlider setting, Transform contents)
        {
            GameObject box = GameObject.Instantiate(SliderTemplate, contents);
            Slider slider = box.transform.Find("Button/Slider (1)").GetComponent<Slider>();
            Text currentValueText = slider.transform.Find("Text (2)").GetComponent<Text>();

            slider.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
            slider.minValue = setting.Range.Item1;
            slider.maxValue = setting.Range.Item2;
            slider.onValueChanged.AddListener((float value) =>
            {
                setting.SetValue(value);
                currentValueText.text = value.ToString();
                SaveData.Save();
            });

            slider.value = setting.Value;
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
                SaveData.Save();
            });

            dropdown.value = setting.Value;
            box.transform.Find("Text").GetComponent<Text>().text = setting.Name.Humanize();
        }

        static void CreateKey(UKKeySetting setting, Transform contents)
        {
            GameObject box = GameObject.Instantiate(KeyTemplate, contents);
            GameObject button = box.transform.Find("W").gameObject;

            float y = box.GetComponent<RectTransform>().sizeDelta.y;
            ControlOffset -= y;
            SizeDeltaOffset += y;
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(620f, OriginalScrollHeight + SizeDeltaOffset);
            box.transform.localPosition = new Vector3(box.transform.localPosition.x, ControlOffset, box.transform.localPosition.z);

            button.name = setting.Binding.Name;
            if (InputManager.Instance && InputManager.Instance.Inputs.ContainsKey(setting.Binding.Name))
                setting.SetValue(InputManager.Instance.Inputs[setting.Binding.Name]);

            button.transform.Find("Text").GetComponent<Text>().text = ControlsOptions.GetKeyName(setting.Key);

            box.transform.Find("Text").GetComponent<Text>().text = setting.Name.Humanize();
        }

        static void RegisterMenu(string buttonName, GameObject menu)
        {
            if (!Registries.options_menus.ContainsKey(buttonName))
                Registries.options_menus.Add(buttonName, menu);
            else
                Registries.options_menus[buttonName] = menu;
        }

        static void RegisterButton(string buttonName, GameObject button)
        {
            if (!Registries.options_buttons.ContainsKey(buttonName))
                Registries.options_buttons.Add(buttonName, button);
            else
                Registries.options_buttons[buttonName] = button;
        }

        static void ClearCaches()
        {
            CachedHeader = null;
            CachedModHeader = null;
            CachedMenu = null;
        }

        public static void Rebuild()
        {
            if (!CanvasController.Instance) return;
            UKLogger.LogWarning("Rebuilding options menu");
            Rebuilding = true;
            List<GameObject> toDestroy = new List<GameObject>();
            List<string> btnKeys = new List<string>();
            List<string> menuKeys = new List<string>();

            foreach (var btn in Registries.options_buttons.Where(n => !VanillaButtons.Contains(n.Key)))
            {
                toDestroy.Add(btn.Value);
                btnKeys.Add(btn.Key);
            }
            foreach (var menu in Registries.options_menus.Where(n => !VanillaButtons.Contains(n.Key)))
            {
                toDestroy.Add(menu.Value);
                menuKeys.Add(menu.Key);
            }

            for (int i = 0; i < toDestroy.Count; i++)
            {
                GameObject obj = toDestroy[i];
                obj.SetActive(false);
                GameObject.Destroy(obj);
            }

            foreach (string key in btnKeys)
                Registries.options_buttons.Remove(key);
            foreach (string key in menuKeys)
                Registries.options_menus.Remove(key);

            NewButton = null;
            foreach (var pair in OriginalPos)
            {
                Transform btn = Menu.transform.Find(pair.Key);
                btn.localPosition = pair.Value + new Vector3(0, inMenu  && !Rebuilding ? 0 : -35, 0);
                btn.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            OnEnablePostfix(CanvasController.instance);
            KeybindsInjector.SetKeys(InputManager.instance);
            // Keybinds don't refresh properly on the first load of a level after starting the game;
            // similar issue to requiring two refreshes for guns to load in the proper order
            if (!isLoaded && !inMenu)
            {
                isLoaded = true;
                CanvasController.instance.StartCoroutine(ReloadScene());
            }
        }

        private static IEnumerator ReloadScene()
        {
            yield return new WaitForSeconds(0.5f);
            UKLogger.LogWarning("Reloading active scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield return null;
        }
    }

    [HarmonyPatch(typeof(InputManager))]
    public class KeybindsInjector
    {
        [HarmonyPatch("OnEnable"), HarmonyPostfix]
        static void OnEnablePostfix(InputManager __instance)
        {
            SetKeys(__instance);
        }

        public static void SetKeys(InputManager instance)
        {
            instance.bindings = instance.bindings.AddRangeToArray(Registries.key_registry.Select(n => n.Value.Binding).Where(s => !instance.bindings.Contains(s)).ToArray());

            Registries.key_states = Registries.key_registry.ToDictionary(item => item.Key, item => new InputActionState(item.Value.Binding.Action));

            foreach (UKKeySetting setting in Registries.key_registry.Values)
            {
                if (!PrefsManager.Instance.HasKey(setting.Binding.PrefName))
                    PrefsManager.instance.prefMap.Add(setting.Binding.PrefName, setting.Binding.DefaultKey);

                setting.Binding.Action.AddBinding().WithGroup("Keyboard");
                setting.Binding.Action.Enable();
            }
            instance.UpdateBindings();
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
