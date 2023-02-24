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
        public static CheatStateChangedEvent CheatStateChanged;
        public static EnemySpawnedEvent EnemySpawned;
        public static EnemyDiedEvent EnemyDied;
        public static UnityEvent ArenaActivated;
        public static UnityEvent ArenaCompleted;
    }

    // Event Types
    [Serializable]
    public class CheatStateChangedEvent : UnityEvent<string> { }

    [Serializable]
    public class EnemySpawnedEvent : UnityEvent<EnemyIdentifier> { }
    [Serializable]
    public class EnemyDiedEvent : UnityEvent<EnemyIdentifier> { }
}
