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
        /// <summary>
        /// Registers a custom checkbox to the options menu, loading from save data if present. Overwrite will replace old data with new settings.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="heading"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="defaultValue"></param>
        /// <param name="overwrite"></param>
        /// <returns>The generated UKCheckbox object</returns>
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

        /// <summary>
        /// Registers a custom slider to the options menu, loading from save data if present. Overwrite will replace old data with new settings.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="heading"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="defaultValue"></param>
        /// <param name="overwrite"></param>
        /// <returns>The generated UKSlider object</returns>
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

        /// <summary>
        /// Registers a custom picker to the options menu, loading from save data if present. Overwrite will replace old data with new settings.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="heading"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="options"></param>
        /// <param name="defaultIndex"></param>
        /// <param name="overwrite"></param>
        /// <returns>The generated UKPicker object</returns>
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

        /// <summary>
        /// Searches the registry for the specified checkbox, passing it to the 'out' variable if found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkbox"></param>
        /// <returns>'true' on success, 'false' otherwise.</returns>
        public static bool GetCheckbox(string id, out UKCheckbox checkbox)
        {
            id = "checkbox." + id.Dehumanize();

            bool success = SaveData.data.settings_check.TryGetValue(id, out checkbox);
            if (success)
                return true;
            checkbox = default;
            return false;
        }

        /// <summary>
        /// Searches the registry for the specified slider, passing it to the 'out' variable if found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="slider"></param>
        /// <returns>'true' on success, 'false' otherwise.</returns>
        public static bool GetSlider(string id, out UKSlider slider)
        {
            id = "slider." + id.Dehumanize();

            bool success = SaveData.data.settings_slide.TryGetValue(id, out slider);
            if (success)
                return true;
            slider = default;
            return false;
        }

        /// <summary>
        /// Searches the registry for the specified picker, passing it to the 'out' variable if found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="picker"></param>
        /// <returns>'true; on success, 'false' otherwise.</returns>
        public static bool GetPicker(string id, out UKPicker picker)
        {
            id = "picker." + id.Dehumanize();

            bool success = SaveData.data.settings_pick.TryGetValue(id, out picker);
            if (success)
                return true;
            picker = default;
            return false;
        }

        /// <summary>
        /// Registers and injects a custom keybind. 
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="name"></param>
        /// <param name="defaultKey"></param>
        /// <returns>A UKKeySetting, which can be used to read/set the current key and bind to the OnBindingChanged event.</returns>
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

        /// <summary>
        /// Retrieves the InputActionState associated with the registered keybind, if it exists.
        /// If UMM is installed (and the compatibility patch), this will be a UKKeyBind.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="actionState"></param>
        /// <returns>'true' on success, 'false' otherwise.</returns>
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

        /// <summary>
        /// Registers a custom button and menu for injection into the in-game options menu.
        /// </summary>
        /// <param name="name"></param>
        public static void CreateMenu(string name)
        {
            Registries.options_menusToAdd.Add(name);
            if (CanvasController.Instance)
                OptionsInjector.Rebuild();
        }

        /// <summary>
        /// <para>Retrieves a button and menu pair from the options menu if initialized (includes vanilla and modded menus).</para>
        /// <para>Vanilla options are: "Gameplay", "Controls", "Video", "Audio", "HUD", "Assist", "Colors", "Saves".</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A tuple containing the button and menu objects.</returns>
        public static (GameObject button, GameObject menu) GetMenu(string name)
        {
            string internal_name = name.Dehumanize();
            GameObject btn = Registries.options_buttons[internal_name] ?? default;
            GameObject mn = Registries.options_menus[internal_name] ?? default;
            return (btn, mn);
        }
    }
}
