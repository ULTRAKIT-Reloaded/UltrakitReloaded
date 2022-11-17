using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ULTRAKIT.Loader
{
    public static class HatLoader
    {
        public static List<HatsManager> managerInstances;
        public static List<HatRegistry> registries;

        internal static void Init()
        {
            SceneManager.sceneUnloaded += ClearInstances;
            if (registries == null) registries = new List<HatRegistry>();
            if (managerInstances == null) managerInstances = new List<HatsManager>();
        }

        public static void LoadHats(AssetBundle bundle)
        {
            Init();
            Hat[] hats = bundle.LoadAllAssets<Hat>();
            HatRegistry[] regis = bundle.LoadAllAssets<HatRegistry>();
            foreach (HatRegistry hatRegistry in regis)
            {
                hatRegistry.hatDict = new Dictionary<EnemyType, Hat>();
                foreach (Hat hat in hatRegistry.hats)
                {
                    hatRegistry.hatDict.Add(hat.enemyType, hat);
                }
            }
            registries.AddRange(regis);
        }

        public static void SetAllActive(string hatID, bool active)
        {
            List<HatsManager> toRemove = new List<HatsManager>();
            foreach (HatsManager manager in managerInstances)
            {
                if (manager == null)
                {
                    toRemove.Add(manager);
                    continue;
                }

                if (manager.isActiveAndEnabled)
                    manager.SetHatActive(hatID, active);
            }
            managerInstances.RemoveRange(toRemove);
        }

        private static void ClearInstances(Scene scene)
        {
            managerInstances.Clear();
        }
    }
}
