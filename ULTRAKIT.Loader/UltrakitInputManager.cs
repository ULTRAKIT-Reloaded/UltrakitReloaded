using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.ObjectClasses;
using ULTRAKIT.Loader.Injectors;
using ULTRAKIT.Loader.Loaders;

namespace ULTRAKIT.Loader
{
    public static class UltrakitInputManager
    {
        // Reserves these keybinds for weapon swapping (no one should end up with more weapons than this at once)
        // Works with UFG as well
        public static InputActionState Slot7;
        public static InputActionState Slot8;
        public static InputActionState Slot9;
        public static InputActionState Slot10;
        public static InputActionState Slot11;
        public static InputActionState Slot12;
        public static InputActionState Slot13;
        public static InputActionState Slot14;
        public static InputActionState Slot15;
        public static InputActionState Slot16;
        public static InputActionState Slot17;
        public static InputActionState Slot18;
        public static InputActionState Slot19;
        public static InputActionState Slot20;

        /// <summary>
        /// Internal. Refreshes keybinds based on the currently equipped weapons. Registers only as many slots as are needed to avoid spamming the controls menu.
        /// </summary>
        public static void UpdateKeyBinds()
        {
            int weapons = Registries.weap_allWeapons.Count + 6;
            if (GunControl.Instance != null)
            {
                weapons = GunControl.Instance.slots.Count;
            }

            // How I wish I could make a loop; damn you keycodes
            // I mean I could just make an array of keycodes but that's not really much better
            if (weapons < 7) return;
            Slot7 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 7", KeyCode.Alpha7).ID];
            if (weapons < 8) return;
            Slot8 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 8", KeyCode.Alpha8).ID];
            if (weapons < 9) return;
            Slot9 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 9", KeyCode.Alpha9).ID];
            if (weapons < 10) return;
            Slot10 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 10", KeyCode.Alpha0).ID];
            if (weapons < 11) return;
            Slot11 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 11", KeyCode.Keypad0).ID];
            if (weapons < 12) return;
            Slot12 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 12", KeyCode.Keypad1).ID];
            if (weapons < 13) return;
            Slot13 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 13", KeyCode.Keypad2).ID];
            if (weapons < 14) return;
            Slot14 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 14", KeyCode.Keypad3).ID];
            if (weapons < 15) return;
            Slot15 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 15", KeyCode.Keypad4).ID];
            if (weapons < 16) return;
            Slot16 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 16", KeyCode.Keypad5).ID];
            if (weapons < 17) return;
            Slot17 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 17", KeyCode.Keypad6).ID];
            if (weapons < 18) return;
            Slot18 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 18", KeyCode.Keypad7).ID];
            if (weapons < 19) return;
            Slot19 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 19", KeyCode.Keypad8).ID];
            if (weapons < 20) return;
            Slot20 = Registries.key_states[OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 20", KeyCode.Keypad9).ID];
        }
    }
}
