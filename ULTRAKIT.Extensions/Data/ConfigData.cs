using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;

namespace ULTRAKIT.Extensions.Data
{
    public static class ConfigData
    {
        public static ConfigFile config;

        // Spawner Arm
        public static bool Leviathan;

        internal static void LoadConfig()
        {
            Leviathan = config.Bind<bool>("Spawner Arm", "RegisterLeviathan", true, "Loads 5-4 in the background to add the leviathan to the spawner arm.").Value;
        }
    }
}
