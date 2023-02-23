using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ULTRAKIT.Extensions
{
    public static class Events
    {
        public static CheatStateChangedEvent CheatStateChanged => Patches.CheatsManagerPatch.CheatStateChanged;
        public static EnemySpawnedEvent EnemySpawned => Patches.EnemyTrackerPatch.EnemySpawned;
        public static EnemyDiedEvent EnemyDied => Patches.EnemyIdentifierPatch.EnemyDied;
    }

    // Event Types
    [Serializable]
    public class CheatStateChangedEvent : UnityEvent<string> { }

    [Serializable]
    public class EnemySpawnedEvent : UnityEvent<EnemyIdentifier> { }
    [Serializable]
    public class EnemyDiedEvent : UnityEvent<EnemyIdentifier> { }
}
