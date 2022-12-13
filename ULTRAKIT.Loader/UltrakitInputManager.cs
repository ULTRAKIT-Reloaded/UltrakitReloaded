using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using NewBlood;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;

namespace ULTRAKIT.Loader
{
    public static class UltrakitInputManager
    {
        public static UKKeyBind Slot7;
        public static UKKeyBind Slot8;
        public static UKKeyBind Slot9;
        public static UKKeyBind Slot10;
        public static UKKeyBind Slot11;
        public static UKKeyBind Slot12;
        public static UKKeyBind Slot13;
        public static UKKeyBind Slot14;
        public static UKKeyBind Slot15;
        public static UKKeyBind Slot16;
        public static UKKeyBind Slot17;
        public static UKKeyBind Slot18;
        public static UKKeyBind Slot19;
        public static UKKeyBind Slot20;

        public static void UpdateKeyBinds()
        {
            int weapons = WeaponLoader.allWeapons.Count + 6;

            if (weapons < 7) return;
            Slot7 = UKAPI.GetKeyBind("Slot 7", KeyCode.Alpha7);
            if (weapons < 8) return;
            Slot8 = UKAPI.GetKeyBind("Slot 8", KeyCode.Alpha8);
            if (weapons < 9) return;
            Slot9 = UKAPI.GetKeyBind("Slot 9", KeyCode.Alpha9);
            if (weapons < 10) return;
            Slot10 = UKAPI.GetKeyBind("Slot 10", KeyCode.Alpha0);
            if (weapons < 11) return;
            Slot11 = UKAPI.GetKeyBind("Slot 11", KeyCode.Keypad0);
            if (weapons < 12) return;
            Slot12 = UKAPI.GetKeyBind("Slot 12", KeyCode.Keypad1);
            if (weapons < 13) return;
            Slot13 = UKAPI.GetKeyBind("Slot 13", KeyCode.Keypad2);
            if (weapons < 14) return;
            Slot14 = UKAPI.GetKeyBind("Slot 14", KeyCode.Keypad3);
            if (weapons < 15) return;
            Slot15 = UKAPI.GetKeyBind("Slot 15", KeyCode.Keypad4);
            if (weapons < 16) return;
            Slot16 = UKAPI.GetKeyBind("Slot 16", KeyCode.Keypad5);
            if (weapons < 17) return;
            Slot17 = UKAPI.GetKeyBind("Slot 17", KeyCode.Keypad6);
            if (weapons < 18) return;
            Slot18 = UKAPI.GetKeyBind("Slot 18", KeyCode.Keypad7);
            if (weapons < 19) return;
            Slot19 = UKAPI.GetKeyBind("Slot 19", KeyCode.Keypad8);
            if (weapons < 20) return;
            Slot20 = UKAPI.GetKeyBind("Slot 20", KeyCode.Keypad9);
        }
    }
}
