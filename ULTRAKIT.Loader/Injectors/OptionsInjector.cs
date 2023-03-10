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
using UnityEngine;
using UnityEngine.InputSystem;

namespace ULTRAKIT.Loader.Injectors
{
    public static class OptionsInjector
    {
        private static Transform Menu = CanvasController.Instance.transform.Find("OptionsMenu");
        private static GameObject MenuButtonPrefab, ScrollPrefab, SliderPrefab, CheckboxPrefab, PickerPrefab, KeyPrefab;

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
