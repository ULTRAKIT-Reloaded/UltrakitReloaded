using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using UnityEngine;

namespace ULTRAKIT.Loader
{
    public static class HatLoader
    {
        public static List<HatRegistry> registries;

        internal static void Init()
        {
            if (registries == null) registries = new List<HatRegistry>();
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
    }
}
