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

        public static bool FixUnhardened;

        public static void LoadConfig()
        {
            FixUnhardened = config.Bind<bool>("Magenta", "Fix Unhardened Bundles Registry", true, "Replaces the empty unhardenedBundles.json with a correctly populated version (only needs to be run once).").Value;
        }
    }
}
