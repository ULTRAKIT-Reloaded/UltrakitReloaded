using GameConsole;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Loader.Injectors;
using UMM;

namespace ULTRAKIT.Loader
{
    public class Initializer
    {
        public static void Initialize()
        {
            File.Delete($@"{ConsolePatch.ModDirectory}\LogOutput.txt");
            Harmony harmony = new Harmony("ULTRAKIT.Loader");
            harmony.PatchAll();
            UltrakitInputManager.UpdateKeyBinds();
            HatLoader.Init();
            SpawnerInjector.Init();
        }
    }

    [HarmonyPatch(typeof(GameConsole.Console))]
    public class ConsolePatch
    {
        public static string ModDirectory;
        public static bool _enabled = false;
        [HarmonyPatch("InsertLog")]
        [HarmonyPostfix]
        public static void InsertLogPostfix(CapturedLog log)
        {
            if (!_enabled) return;
            using (StreamWriter file = new StreamWriter($@"{ModDirectory}\LogOutput.txt", true))
            {
                file.WriteLine(log.message);
                if (log.stackTrace != null && log.stackTrace != string.Empty)
                    file.WriteLine(log.stackTrace.Remove(log.stackTrace.LastIndexOf('\n')));
                file.WriteLine("----------------");
            }
        }
    }
}
