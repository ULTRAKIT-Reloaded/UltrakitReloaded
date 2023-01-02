using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using ULTRAKIT.Loader;
using ULTRAKIT.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

namespace ULTRAKIT
{
    [UKPlugin("petersone1.ultrakitreloaded", "Ultrakit Reloaded", "1.4.0", "A library for weapon loading and common functions", false, false)]
    public class Plugin : UKMod
    {
        public override void OnModLoaded()
        {
            Loader.Injectors.SpawnerInjector.fpeye = SetSprite(Properties.Resources.fpeye_jpg);
            Loader.Injectors.SpawnerInjector.fpface = SetSprite(Properties.Resources.fpface_jpg);
            Loader.Injectors.SpawnerInjector.levi = SetSprite(Properties.Resources.levi_jpg);
            Loader.Injectors.SpawnerInjector.minos = SetSprite(Properties.Resources.minos_jpg);
            Loader.Injectors.SpawnerInjector.wicked = SetSprite(Properties.Resources.wicked_jpg);

            Loader.ConsolePatch.ModDirectory = modFolder;
            Loader.Initializer.Initialize();
            Extensions.Initializer.Initialize();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            AssetBundle topHats = AssetBundle.LoadFromMemory(Properties.Resources.ultrakit_tophat);
            HatLoader.LoadHats(topHats);
            GameConsole.Console.Instance.RegisterCommand(new AltSetter());

            Loader.Injectors.SpawnerInjector.Init();
        }

        public override void OnModUnload()
        {
            PlayerPrefs.SetInt("CurSlo", 1);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CheatsManager.Instance.RegisterCheat(Refresher.cheat, "ULTRAKIT");
            CheatsManager.Instance.RegisterCheat(ActivateHats.cheat, "ULTRAKIT");
            Invoke(GunSetter.Instance.RefreshWeapons, 0.05f);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (!HatLoader.Persistent)
                HatLoader.activeHats.Clear();
        }

        public void Invoke(Action func, float delay)
        {
            StartCoroutine(Delay());

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
    }
}
