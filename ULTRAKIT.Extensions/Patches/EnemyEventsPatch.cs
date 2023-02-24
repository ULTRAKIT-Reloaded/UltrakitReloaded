using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULTRAKIT.Extensions.Patches
{
    [HarmonyPatch(typeof(EnemyTracker))]
    public class EnemyTrackerPatch
    {
        public static EnemySpawnedEvent EnemySpawned
        {
            get { return Events.EnemySpawned; }
            set { Events.EnemySpawned = value; }
        }

        [HarmonyPatch("AddEnemy"), HarmonyPostfix]
        static void AddEnemyPostfix(EnemyTracker __instance, EnemyIdentifier eid)
        {
            if (EnemySpawned == null)
                EnemySpawned = new EnemySpawnedEvent();
            EnemySpawned.Invoke(eid);
        }
    }

    [HarmonyPatch(typeof(EnemyIdentifier))]
    public class EnemyIdentifierPatch
    {
        public static EnemyDiedEvent EnemyDied
        {
            get { return Events.EnemyDied; }
            set { Events.EnemyDied = value; }
        }

        [HarmonyPatch("Death"), HarmonyPostfix]
        static void DeathPostfix(EnemyIdentifier __instance)
        {
            if (EnemyDied == null)
                EnemyDied = new EnemyDiedEvent();
            EnemyDied.Invoke(__instance);
        }
    }
}
