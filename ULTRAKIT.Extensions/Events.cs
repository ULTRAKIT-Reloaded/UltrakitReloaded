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
        public static CheatStateChangedEvent CheatStateChanged { get; set; }
        public static EnemySpawnedEvent EnemySpawned { get; set; }
        public static EnemyDiedEvent EnemyDied { get; set; }
        public static UnityEvent ArenaActivated { get; set; }
        public static UnityEvent ArenaCompleted { get; set; }
    }

    // Event Types
    [Serializable]
    public class CheatStateChangedEvent : UnityEvent<string> { }
    [Serializable]
    public class EnemySpawnedEvent : UnityEvent<EnemyIdentifier> { }
    [Serializable]
    public class EnemyDiedEvent : UnityEvent<EnemyIdentifier> { }
}
