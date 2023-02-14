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
using UMM;
using UnityEngine.SceneManagement;

namespace ULTRAKIT.Loader
{
    public class Initializer
    {
        public static void Initialize()
        {
            Harmony harmony = new Harmony("ULTRAKIT.Loader");
            harmony.PatchAll();
            HatLoader.Init();
            SpawnerInjector.Init();
            UltrakitInputManager.UpdateKeyBinds();
        }
    }
}
