using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Data;
using ULTRAKIT.Extensions.ObjectClasses;
using ULTRAKIT.Loader.Injectors;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Events;

namespace ULTRAKIT.Loader.Loaders
{
    public static class OptionsLoader
    {
        public static UKCheckbox RegisterCheckbox(string section, string heading, string name, string id, bool defaultValue, bool overwrite = false)
        {
            UKCheckbox checkbox;
            string internal_name = "checkbox." + id.Dehumanize();
            if (!overwrite && SaveData.data.settings_check.ContainsKey(internal_name))
            {
                checkbox = SaveData.data.settings_check[internal_name];
                Registries.RegisterSetting(checkbox);
                return checkbox;
            }
            checkbox = new UKCheckbox(section, heading, name, id, defaultValue);
            Registries.RegisterSetting(checkbox);
            SaveData.Internal_SetValue(SaveData.data.settings_check, internal_name, checkbox);
            return checkbox;
        }

        public static UKSlider RegisterSlider(string section, string heading, string name, string id, float min, float max, float defaultValue, bool overwrite = false)
        {
            UKSlider slider;
            string internal_name = "slider." + id.Dehumanize();
            if (!overwrite && SaveData.data.settings_slide.ContainsKey(internal_name))
            {
                slider = SaveData.data.settings_slide[internal_name];
                Registries.RegisterSetting(slider);
                return slider;
            }
            slider = new UKSlider(section, heading, name, id, min, max, defaultValue);
            Registries.RegisterSetting(slider);
            SaveData.Internal_SetValue(SaveData.data.settings_slide, internal_name, slider);
            return slider;
        }

        public static UKPicker RegisterPicker(string section, string heading, string name, string id, string[] options, int defaultIndex, bool overwrite = false)
        {
            UKPicker picker;
            string internal_name = "picker." + id.Dehumanize();
            if (!overwrite && SaveData.data.settings_pick.ContainsKey(internal_name))
            {
                picker = SaveData.data.settings_pick[internal_name];
                Registries.RegisterSetting(picker);
                return picker;
            }
            picker = new UKPicker(section, heading, name, id, options, defaultIndex);
            Registries.RegisterSetting(picker);
            SaveData.Internal_SetValue(SaveData.data.settings_pick, internal_name, picker);
            return picker;
        }

        public static bool GetCheckbox(string id, out UKCheckbox checkbox)
        {
            id = "checkbox." + id.Dehumanize();

            bool success = SaveData.data.settings_check.TryGetValue(id, out checkbox);
            if (success)
                return true;
            checkbox = default;
            return false;
        }

        public static bool GetSlider(string id, out UKSlider slider)
        {
            id = "slider." + id.Dehumanize();

            bool success = SaveData.data.settings_slide.TryGetValue(id, out slider);
            if (success)
                return true;
            slider = default;
            return false;
        }

        public static bool GetPicker(string id, out UKPicker picker)
        {
            id = "picker." + id.Dehumanize();

            bool success = SaveData.data.settings_pick.TryGetValue(id, out picker);
            if (success)
                return true;
            picker = default;
            return false;
        }

        public static UKKeySetting SetKeyBind(string heading, string name, KeyCode defaultKey)
        {
            string id = "keybind." + name.Dehumanize();
            if (Registries.key_registry.ContainsKey(id))
            {
                return Registries.key_registry[id];
            }

            UKKeySetting keybind = new UKKeySetting(heading, name, defaultKey);
            Registries.key_registry.Add(keybind.ID, keybind);
            Registries.RegisterSetting(keybind);
            return keybind;
        }

        public static bool GetKeyBind(string name, out InputActionState actionState)
        {
            string id = "keybind." + name.Dehumanize();
            if (Registries.key_states.ContainsKey(id))
            {
                actionState = Registries.key_states[id];
                return true;
            }
            actionState = null;
            return false;
        }

        public static void CreateMenu(string name)
        {
            Registries.options_menusToAdd.Add(name);
            if (CanvasController.Instance)
                OptionsInjector.Rebuild();
        }

        public static (GameObject button, GameObject menu) GetMenu(string name)
        {
            string internal_name = name.Dehumanize();
            GameObject btn = Registries.options_buttons[internal_name] ?? default;
            GameObject mn = Registries.options_menus[internal_name] ?? default;
            return (btn, mn);
        }
    }
}
