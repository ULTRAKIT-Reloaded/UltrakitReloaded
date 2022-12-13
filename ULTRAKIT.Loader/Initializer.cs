using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULTRAKIT.Loader
{
    public class Initializer
    {
        public static void Initialize()
        {
            Harmony harmony = new Harmony("ULTRAKIT.Loader");
            harmony.PatchAll();
            UltrakitInputManager.UpdateKeyBinds();
            HatLoader.Init();
        }
    }
}
