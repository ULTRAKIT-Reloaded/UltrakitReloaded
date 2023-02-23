using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Extensions;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ULTRAKIT.Loader
{
    public static class HatLoader
    {
        public static List<HatsManager> managerInstances;
        private static List<HatRegistry> registries => Registries.hat_registries;
        private static List<string> activeHats => Registries.hat_activeHats;
        public static bool Persistent = false;

        internal static void Init()
        {
            SceneManager.sceneUnloaded += ClearInstances;
            if (managerInstances == null) managerInstances = new List<HatsManager>();
            SetSeasonals();
        }

        public static void LoadHats(AssetBundle bundle)
        {
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
            UKLogger.Log($"Loaded hats from {bundle.name}");
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
            if (active && !activeHats.Contains(hatID))
                activeHats.Add(hatID);
            if (!active && activeHats.Contains(hatID))
                activeHats.Remove(hatID);

            managerInstances.RemoveRange(toRemove);
        }

        private static void ClearInstances(Scene scene)
        {
            managerInstances.Clear();
        }

        private static DateTime GetEaster(int year)
        {
            int num = year % 19;
            int num2 = year / 100;
            int num3 = (num2 - num2 / 4 - (8 * num2 + 13) / 25 + 19 * num + 15) % 30;
            int num4 = num3 - num3 / 28 * (1 - num3 / 28 * (29 / (num3 + 1)) * ((21 - num) / 11));
            int num5 = num4 - (year + year / 4 + num4 + 2 - num2 + num2 / 4) % 7;
            int num6 = 3 + (num5 + 40) / 44;
            int day = num5 + 28 - 31 * (num6 / 4);
            return new DateTime(year, num6, day);
        }

        public static void SetSeasonals()
        {
            DateTime time = DateTime.Now;
            switch (time.Month)
            {
                case 12:
                    if (time.Day >= 22 && time.Day <= 28)
                    {
                        activeHats.Add("christmas");
                    }
                    return;
                case 10:
                    if (time.Day >= 25 && time.Day <= 31)
                    {
                        activeHats.Add("halloween");
                    }
                    return;
            }
            DateTime dateTime = GetEaster(time.Year);
            if (time.DayOfYear >= dateTime.DayOfYear - 2 && time.DayOfYear <= dateTime.DayOfYear)
            {
                activeHats.Add("easter");
            }
        }
    }
}
