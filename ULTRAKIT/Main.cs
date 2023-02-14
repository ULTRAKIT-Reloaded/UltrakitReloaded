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

namespace ULTRAKIT
{
    [BepInPlugin("ULTRAKIT.core_module", "ULTRAKIT Reloaded", "1.5.4")]
    [BepInDependency("UMM", "0.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin plugin;

        private void Awake()
        {
            plugin = this;
            Initializer.Init();
        }

        private static void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("CurSlo", 1);
        }

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
