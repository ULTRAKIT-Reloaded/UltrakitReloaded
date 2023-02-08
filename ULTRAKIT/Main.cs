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
    [BepInPlugin("petersone1.ultrakitreloaded", "ULTRAKIT Reloaded", "1.5.4")]
    [BepInDependency("UMM", "0.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin plugin;

        private void Awake()
        {
            plugin = this;

            string modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            LoadConfig();

            SpawnerInjector.fpeye = SetSprite(Properties.Resources.fpeye_jpg);
            SpawnerInjector.fpface = SetSprite(Properties.Resources.fpface_jpg);
            SpawnerInjector.levi = SetSprite(Properties.Resources.levi_jpg);
            SpawnerInjector.minos = SetSprite(Properties.Resources.minos_jpg);
            SpawnerInjector.wicked = SetSprite(Properties.Resources.wicked_jpg);

            AssetBundle topHats = AssetBundle.LoadFromMemory(Properties.Resources.ultrakit_tophat);
            HatLoader.LoadHats(topHats);

            ConsolePatch.ModDirectory = modDir;

            plugin.StartCoroutine(InitializeComponents());
        }

        private static void OnDestroy()
        {
            PlayerPrefs.SetInt("CurSlo", 1);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UKLogger.Log("Scene loaded");
            CheatsManager.Instance.RegisterCheat(Refresher.cheat, "ULTRAKIT");
            CheatsManager.Instance.RegisterCheat(ActivateHats.cheat, "ULTRAKIT");
            Invoke(GunSetter.Instance.RefreshWeapons, 0.05f);
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            if (!HatLoader.Persistent)
                HatLoader.activeHats.Clear();
        }

        public static IEnumerator InitializeComponents()
        {
            while (!UKAPI.triedLoadingBundle)
            {
                yield return new WaitForSeconds(0.2f);
            }
            Loader.Initializer.Initialize();
            Extensions.Initializer.Initialize();

            GameConsole.Console.Instance.RegisterCommand(new AltSetter());

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            yield return null;
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

        private static Sprite SetSprite(byte[] bytes)
        {
            Texture2D tex = new Texture2D(128, 128);
            tex.LoadImage(bytes);
            return Sprite.Create(tex, new Rect(0, 0, 128, 128), new Vector2(64, 64));
        }

        private static void SetConfig(string path)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(InitConfigFile._initConfig);
            File.WriteAllText(path, json);
        }

        private static void LoadConfig()
        {
            ConsolePatch._enabled = plugin.Config.Bind<bool>("Custom Console Logging", "Enabled", false, "Enable custom console log").Value;
            SpawnerInjector.RegisterLeviathan = plugin.Config.Bind<bool>("Spawner Arm", "RegisterLeviathan", true, "Loads 5-4 in the background to add the leviathan to the spawner arm.").Value;
        }
    }
}
