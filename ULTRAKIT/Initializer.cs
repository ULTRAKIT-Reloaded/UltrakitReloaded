using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using ULTRAKIT.Extensions;
using ULTRAKIT.Loader;
using System.IO;
using ULTRAKIT.Loader.Injectors;
using System.Reflection;
using ULTRAKIT.Extensions.Data;
using ULTRAKIT.Loader.Loaders;
using HarmonyLib;

namespace ULTRAKIT.Core
{
    internal class Initializer
    {
        /// <summary>
        /// Internal. Initializes ULTRAKIT Reloaded.
        /// </summary>
        public static void Init()
        {
            ConfigData.config = Plugin.plugin.Config;
            Extensions.Initializer.Initialize();
            SetSpawnerSprites();
            Plugin.plugin.StartCoroutine(InitializeComponents());
        }

        /// <summary>
        /// Internal. Continues initializing the mod after the asset bundle loads.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator InitializeComponents()
        {
            // If done before the bundle loads, patched objects don't exist yet and startup fails

            AssetBundle common = null;
            while (common == null)
            {
                AssetLoader.LoadFromLoaded("common", out common);
                string commonAssetBundlePath = Path.Combine(BepInEx.Paths.GameRootPath, "ULTRAKILL_Data\\StreamingAssets\\common");
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(commonAssetBundlePath);
                yield return request;
                int attempts = 0;
                while (request.assetBundle == null)
                {
                    yield return new WaitForSeconds(0.3f);
                    if (attempts > 5)
                    {
                        yield break;
                    }
                    request = AssetBundle.LoadFromFileAsync(commonAssetBundlePath);
                    yield return request;
                    attempts++;
                }
                common = request.assetBundle;
            }

            Loader.Initializer.Initialize();
            LoadCommands();
            LoadHats();

            common.Unload(false);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            yield return null;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Loaded cheats don't persist across scenes; each manager is a clean instance
            LoadCheats();
            // If I don't do this, custom weapons usually (but not always) end up before vanilla weapons
            // not sure why, since this is the same function that gets patched to load them in the first place (approximately)
            Plugin.Invoke(GunSetter.Instance.RefreshWeapons, 0.05f);
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            if (!HatLoader.Persistent)
            {
                Registries.hat_activeHats.Clear();
                HatLoader.SetSeasonals();
            }
        }

        private static void SetSpawnerSprites()
        {
            Sprite fpeye = GraphicsUtilities.CreateSprite(Properties.Resources.fpeye_jpg, 128, 128);
            Sprite fpface = GraphicsUtilities.CreateSprite(Properties.Resources.fpface_jpg, 128, 128);
            Sprite levi = GraphicsUtilities.CreateSprite(Properties.Resources.levi_jpg, 128, 128);
            Sprite minos = GraphicsUtilities.CreateSprite(Properties.Resources.minos_jpg, 128, 128);
            Sprite wicked = GraphicsUtilities.CreateSprite(Properties.Resources.wicked_jpg, 128, 128);

            Registries.spawn_sprites.Add("DroneFlesh", fpeye);
            Registries.spawn_sprites.Add("DroneSkull Variant", fpface);
            Registries.spawn_sprites.Add("MinosBoss", minos);
            Registries.spawn_sprites.Add("Wicked", wicked);
            Registries.spawn_sprites.Add("Leviathan", levi);
        }

        // Loaders

        private static void LoadHats()
        {
            AssetBundle topHats = AssetBundle.LoadFromMemory(Properties.Resources.ultrakit_tophat);
            HatLoader.LoadHats(topHats);
        }

        private static void LoadCheats()
        {
            CheatsManager.Instance.RegisterCheat(new Cheats.Refresher(), "ULTRAKIT");
            CheatsManager.Instance.RegisterCheat(new Cheats.ActivateHats(), "ULTRAKIT");
        }

        private static void LoadCommands()
        {
            GameConsole.Console.Instance.RegisterCommand(new Commands.AltSetter());
        }
    }
}
