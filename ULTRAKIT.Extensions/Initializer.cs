using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions.Data;

namespace ULTRAKIT.Extensions
{
    public class Initializer
    {
        /// <summary>
        /// Internal. Initializes the Extensions.
        /// </summary>
        public static void Initialize()
        {
            ConfigData.LoadConfig();
            Harmony harmony = new Harmony("ULTRAKIT.Extensions");
            harmony.PatchAll();
        }
    }
}
