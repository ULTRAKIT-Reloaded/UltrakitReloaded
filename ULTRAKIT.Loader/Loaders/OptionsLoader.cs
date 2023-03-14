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
using UnityEngine;
using UnityEngine.Events;

namespace ULTRAKIT.Loader.Loaders
{
    public static class OptionsLoader
    {
        public static UKCheckbox RegisterCheckbox(string section, string heading, string name, bool defaultValue)
        {
            UKCheckbox checkbox;
            string internal_name = Assembly.GetCallingAssembly().GetName().Name + ".checkbox." + name.Dehumanize();
            if (SaveData.data.settings_check.ContainsKey(internal_name))
            {
                checkbox = SaveData.data.settings_check[internal_name] as UKCheckbox;
                Registries.RegisterSetting(checkbox);
                return checkbox;
            }
            checkbox = new UKCheckbox(section, heading, name, defaultValue);
            Registries.RegisterSetting(checkbox);
            SaveData.Internal_SetValue(SaveData.data.settings_check, internal_name, checkbox);
            return checkbox;
        }

        public static UKSlider RegisterSlider(string section, string heading, string name, float min, float max, float defaultValue)
        {
            UKSlider slider;
            string internal_name = Assembly.GetCallingAssembly().GetName().Name + ".slider." + name.Dehumanize();
            if (SaveData.data.settings_slide.ContainsKey(internal_name))
            {
                slider = SaveData.data.settings_slide[internal_name] as UKSlider;
                Registries.RegisterSetting(slider);
                return slider;
            }
            slider = new UKSlider(section, heading, name, min, max, defaultValue);
            Registries.RegisterSetting(slider);
            SaveData.Internal_SetValue(SaveData.data.settings_slide, internal_name, slider);
            return slider;
        }

        public static UKPicker RegisterPicker(string section, string heading, string name, string[] options, int defaultIndex)
        {
            UKPicker picker;
            string internal_name = Assembly.GetCallingAssembly().GetName().Name + ".picker." + name.Dehumanize();
            if (SaveData.data.settings_pick.ContainsKey(internal_name))
            {
                picker = SaveData.data.settings_pick[internal_name] as UKPicker;
                Registries.RegisterSetting(picker);
                return picker;
            }
            picker = new UKPicker(section, heading, name, options, defaultIndex);
            Registries.RegisterSetting(picker);
            SaveData.Internal_SetValue(SaveData.data.settings_pick, internal_name, picker);
            return picker;
        }

        public static bool GetCheckbox(string name, out UKCheckbox checkbox, bool doOverride = false)
        {
            string id = name;
            if (!doOverride)
                id = Assembly.GetCallingAssembly().GetName().Name + ".checkbox." + name.Dehumanize();

            bool success = SaveData.data.settings_check.TryGetValue(id, out checkbox);
            if (success)
                return true;
            checkbox = default;
            return false;
        }

        public static bool GetSlider(string name, out UKSlider slider, bool doOverride = false)
        {
            string id = name;
            if (!doOverride)
                id = Assembly.GetCallingAssembly().GetName().Name + ".slider." + name.Dehumanize();

            bool success = SaveData.data.settings_slide.TryGetValue(id, out slider);
            if (success)
                return true;
            slider = default;
            return false;
        }

        public static bool GetPicker(string name, out UKPicker picker, bool doOverride = false)
        {
            string id = name;
            if (!doOverride)
                id = Assembly.GetCallingAssembly().GetName().Name + ".picker." + name.Dehumanize();

            bool success = SaveData.data.settings_pick.TryGetValue(id, out picker);
            if (success)
                return true;
            picker = default;
            return false;
        }
    }

    public static class KeybindsLoader
    {
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
    }
}
