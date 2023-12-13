using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions.Data;

namespace ULTRAKIT.Extensions
{
    public class Initializer
    {
        /// <summary>
        /// Internal. Initializes the Extensions.
        /// </summary>
        public static void Initialize(string assetFileData)
        {
            CreateEvents();
            AssetLoader.Init(assetFileData);
            Harmony harmony = new Harmony("ULTRAKIT.Extensions");
            harmony.PatchAll();
        }

        private static void CreateEvents()
        {
            Events.CheatStateChanged = new CheatStateChangedEvent();
            Events.EnemySpawned = new EnemySpawnedEvent();
            Events.EnemyDied = new EnemyDiedEvent();
            Events.ArenaActivated = new UnityEngine.Events.UnityEvent();
            Events.ArenaCompleted = new UnityEngine.Events.UnityEvent();
        }
    }
}
