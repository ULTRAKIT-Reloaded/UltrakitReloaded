using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULTRAKIT.Extensions
{
    public class Initializer
    {
        public static void Initialize()
        {
            Harmony harmony = new Harmony("ULTRAKIT.Extensions");
            harmony.PatchAll();
        }
    }
}
