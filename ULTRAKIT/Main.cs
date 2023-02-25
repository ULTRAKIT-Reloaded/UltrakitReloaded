using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
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

namespace ULTRAKIT.Core
{
    [BepInPlugin("ULTRAKIT.core_module", "ULTRAKIT Reloaded", "2.0.0")]
    [BepInDependency("UMM", "0.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin plugin;

        private void Awake()
        {
            plugin = this;
            Initializer.Init();
        }

        // If the mod is disabled while a modded slot was last selected, the game gets angry, so this avoids that
        private static void OnApplicationQuit()
        {
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
