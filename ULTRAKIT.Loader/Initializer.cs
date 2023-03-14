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

        /// <summary>
        /// Internal. Initializes the loaders.
        /// </summary>
        public static void Initialize()
        {

            Harmony harmony = new Harmony("ULTRAKIT.Loader");
            harmony.PatchAll();
            HatLoader.Init();
            SpawnablesInjector.Init();
            UltrakitInputManager.UpdateKeyBinds();

            // DELETE
            OptionsInjector.TestSystem();
        }
    }
}
