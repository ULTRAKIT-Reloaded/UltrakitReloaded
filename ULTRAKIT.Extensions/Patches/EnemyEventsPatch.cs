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
        public static EnemySpawnedEvent EnemySpawned;

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
        public static EnemyDiedEvent EnemyDied;

        [HarmonyPatch("Death"), HarmonyPostfix]
        static void DeathPostfix(EnemyIdentifier __instance)
        {
            if (EnemyDied == null)
                EnemyDied = new EnemyDiedEvent();
            EnemyDied.Invoke(__instance);
        }
    }
}
