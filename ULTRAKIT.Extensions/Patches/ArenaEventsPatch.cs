using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace ULTRAKIT.Extensions.Patches
{
    [HarmonyPatch(typeof(ActivateArena))]
    public class ActivateArenaPatch
    {
        public static UnityEvent ArenaActivated
        {
            get { return Events.ArenaActivated; }
            set { Events.ArenaActivated = value; }
        }

        [HarmonyPatch("Activate"), HarmonyPostfix]
        static void ActivatePostfix()
        {
            if (ArenaActivated == null)
                ArenaActivated = new UnityEvent();
            ArenaActivated.Invoke();
        }
    }

    [HarmonyPatch(typeof(ActivateNextWave))]
    public class ActivateNextWavePatch
    {
        public static UnityEvent ArenaCompleted
        {
            get { return Events.ArenaCompleted; }
            set { Events.ArenaCompleted = value; }
        }

        [HarmonyPatch("EndWaves"), HarmonyPostfix]
        static void EndWavesPostfix()
        {
            if (ArenaCompleted == null)
                ArenaCompleted = new UnityEvent();
            ArenaCompleted.Invoke();
        }
    }

    [HarmonyPatch(typeof(ActivateNextWaveHP))]
    public class ActivateNextWaveHPPatch
    {
        public static UnityEvent ArenaCompleted
        {
            get { return Events.ArenaCompleted; }
            set { Events.ArenaCompleted = value; }
        }

        [HarmonyPatch("EndWaves"), HarmonyPostfix]
        static void EndWavesPostfix()
        {
            if (ArenaCompleted == null)
                ArenaCompleted = new UnityEvent();
            ArenaCompleted.Invoke();
        }
    }
}
