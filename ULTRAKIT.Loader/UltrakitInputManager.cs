using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Classes;
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
            if (weapons > 6)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 7", KeyCode.Alpha7);
                OptionsLoader.GetKeyBind("Slot 7", out Slot7);
            }
            if (weapons > 7)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 8", KeyCode.Alpha8);
                OptionsLoader.GetKeyBind("Slot 8", out Slot8);
            }
            if (weapons > 8)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 9", KeyCode.Alpha9);
                OptionsLoader.GetKeyBind("Slot 9", out Slot9);
            }
            if (weapons > 9)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 10", KeyCode.Alpha0);
                OptionsLoader.GetKeyBind("Slot 10", out Slot10);
            }
            if (weapons > 10)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 11", KeyCode.Keypad0);
                OptionsLoader.GetKeyBind("Slot 11", out Slot11);
            }
            if (weapons > 11)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 12", KeyCode.Keypad1);
                OptionsLoader.GetKeyBind("Slot 12", out Slot12);
            }
            if (weapons > 12)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 13", KeyCode.Keypad2);
                OptionsLoader.GetKeyBind("Slot 13", out Slot13);
            }
            if (weapons > 13)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 14", KeyCode.Keypad3);
                OptionsLoader.GetKeyBind("Slot 14", out Slot14);
            }
            if (weapons > 14)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 15", KeyCode.Keypad4);
                OptionsLoader.GetKeyBind("Slot 15", out Slot15);
            }
            if (weapons > 15)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 16", KeyCode.Keypad5);
                OptionsLoader.GetKeyBind("Slot 16", out Slot16);
            }
            if (weapons > 16)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 17", KeyCode.Keypad6);
                OptionsLoader.GetKeyBind("Slot 17", out Slot17);
            }
            if (weapons > 17)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 18", KeyCode.Keypad7);
                OptionsLoader.GetKeyBind("Slot 18", out Slot18);
            }
            if (weapons > 18)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 19", KeyCode.Keypad8);
                OptionsLoader.GetKeyBind("Slot 19", out Slot19);
            }
            if (weapons > 19)
            {
                OptionsLoader.SetKeyBind("ULTRAKIT Reloaded", "Slot 20", KeyCode.Keypad9);
                OptionsLoader.GetKeyBind("Slot 20", out Slot20);
            }
            //OptionsInjector.Rebuild();
        }
    }
}
