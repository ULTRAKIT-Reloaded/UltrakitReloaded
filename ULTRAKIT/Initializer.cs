using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine.SceneManagement;
using UnityEngine;
using ULTRAKIT.Extensions;
using ULTRAKIT.Loader;
using System.IO;
using ULTRAKIT.Loader.Injectors;
using System.Reflection;
using ULTRAKIT.Extensions.Data;

namespace ULTRAKIT
{
    internal class Initializer
    {
        public static void Init()
        {
            ConfigData.config = Plugin.plugin.Config;
            Extensions.Initializer.Initialize();
            SetSpawnerSprites();
            Plugin.plugin.StartCoroutine(InitializeComponents());
        }

        public static IEnumerator InitializeComponents()
        {
            while (!UKAPI.triedLoadingBundle)
            {
                yield return new WaitForSeconds(0.2f);
            }
            Loader.Initializer.Initialize();
            LoadCommands();
            LoadHats();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            yield return null;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LoadCheats();
            Plugin.Invoke(GunSetter.Instance.RefreshWeapons, 0.05f);
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            if (!HatLoader.Persistent)
                HatLoader.activeHats.Clear();
        }

        private static void SetSpawnerSprites()
        {
            SpawnerInjector.fpeye = PeterExtensions.CreateSprite(Properties.Resources.fpeye_jpg);
            SpawnerInjector.fpface = PeterExtensions.CreateSprite(Properties.Resources.fpface_jpg);
            SpawnerInjector.levi = PeterExtensions.CreateSprite(Properties.Resources.levi_jpg);
            SpawnerInjector.minos = PeterExtensions.CreateSprite(Properties.Resources.minos_jpg);
            SpawnerInjector.wicked = PeterExtensions.CreateSprite(Properties.Resources.wicked_jpg);
        }

        // Loaders
        private static void LoadHats()
        {
            AssetBundle topHats = AssetBundle.LoadFromMemory(Properties.Resources.ultrakit_tophat);
            HatLoader.LoadHats(topHats);
        }

        private static void LoadCheats()
        {
            CheatsManager.Instance.RegisterCheat(Refresher.cheat, "ULTRAKIT");
            CheatsManager.Instance.RegisterCheat(ActivateHats.cheat, "ULTRAKIT");
        }

        private static void LoadCommands()
        {
            GameConsole.Console.Instance.RegisterCommand(new AltSetter());
        }
    }
}
