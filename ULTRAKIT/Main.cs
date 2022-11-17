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
    [UKPlugin("Ultrakit Reloaded", "1.2.0B", "A library for weapon loading and common functions", false, false)]
    public class Plugin : UKMod
    {
        public override void OnModLoaded()
        {
            WeaponLoader.Initialize();
            CheatsManagerExtension.Initialize();
            SceneManager.sceneLoaded += OnSceneLoaded;
            AssetBundle topHats = AssetBundle.LoadFromMemory(Properties.Resources.ultrakit_tophat);
            HatLoader.LoadHats(topHats);
        }

        public override void OnModUnload()
        {
            PlayerPrefs.SetInt("CurSlo", 1);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CheatsManager.Instance.RegisterCheat(Refresher.cheat, "ULTRAKIT");
            CheatsManager.Instance.RegisterCheat(ActivateHats.cheat, "ULTRAKIT");
            Invoke(GunSetter.Instance.RefreshWeapons, 0.05f);
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
    }
}
