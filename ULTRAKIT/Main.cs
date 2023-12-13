using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using ULTRAKIT.Loader;
using ULTRAKIT.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using ULTRAKIT.Loader.Injectors;
using ULTRAKIT.Data;
using System.Reflection;
using ULTRAKIT.Extensions.Data;
using BepInEx.Bootstrap;
using Humanizer;
using Newtonsoft.Json;

namespace ULTRAKIT.Core
{
    [BepInPlugin("ULTRAKIT.core_module", "ULTRAKIT Reloaded", "2.3.0")]
    [BepInDependency("UMM", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("waffle.ultrakill.extraalts", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin plugin;
        public static bool isUMM = false;
        public static bool isWaffle = false;

        private void Awake()
        {
            ConfigData.config = Config;
            ConfigData.LoadConfig();
            plugin = this;
            Registries.Invoke = Invoke;
            {
                foreach (var mod in Chainloader.PluginInfos)
                {
                    if (mod.Value.Metadata.GUID == "UMM")
                    {
                        isUMM = true;
                        Loader.Initializer.isUMMInstalled = true;
                    }
                    if (mod.Value.Metadata.GUID == "waffle.ultrakill.extraalts")
                    { 
                        isWaffle = true;
                        Loader.Initializer.isWaffle = true;
                    }
                }
            }
            Initializer.Init();
        }

        // If the mod is disabled while a modded slot was last selected, the game gets angry, so this avoids that
        private static void OnApplicationQuit()
        {
            SaveData.Save();
            PlayerPrefs.SetInt("CurSlo", 1);
        }

        /// <summary>
        /// Calls a function after the specified delay
        /// </summary>
        /// <param name="func"></param>
        /// <param name="delay"></param>
        public static void Invoke(Action func, float delay)
        {
            plugin.StartCoroutine(Delay());

            IEnumerator Delay()
            {
                yield return new WaitForSeconds(delay);
                func();
            }
        }
    }
}
