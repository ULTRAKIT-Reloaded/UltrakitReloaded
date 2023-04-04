using GameConsole;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using ULTRAKIT.Loader.Injectors;
using ULTRAKIT.Loader.Loaders;
using UnityEngine.SceneManagement;
using BepInEx;

namespace ULTRAKIT.Loader
{
    public class Initializer
    {
        public static bool isUMMInstalled = false;
        public static bool isWaffle = false;

        /// <summary>
        /// Internal. Initializes the loaders.
        /// </summary>
        public static void Initialize()
        {
            Harmony harmony = new Harmony("ULTRAKIT.Loader");
            harmony.PatchAll();
            HatLoader.Init();
            // Too early, caused bloodstains to turn into squares
            //SpawnablesInjector.Init();
            UltrakitInputManager.UpdateKeyBinds();
            OptionsLoader.RegisterCheckbox("ULTRAKIT Reloaded", "Tweaks", "Enable Overhead Health Bars", "ultrakit.healthBars", false);
        }
    }
}
