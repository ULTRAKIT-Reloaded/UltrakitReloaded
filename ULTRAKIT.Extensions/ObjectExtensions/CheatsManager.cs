using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using HarmonyLib;

namespace ULTRAKIT.Extensions
{
    public static class CheatsManagerExtension
    {   
        public static void Initialize()
        {
            Harmony harmony = new Harmony("ULTRAKIT.Extensions");
            harmony.PatchAll();
        }

        public static void PrintCheatIDs(this CheatsManager manager)
        {
            foreach (KeyValuePair<String, ICheat> cheat in manager.GetPrivate<Dictionary<String, ICheat>>("idToCheat"))
            {
                Debug.Log(cheat.Key);
            }
        }

        public static bool SetCheatState(this CheatsManager manager, string _id, bool _state)
        {
            Dictionary<String, ICheat> cheats = manager.GetPrivate<Dictionary<String, ICheat>>("idToCheat");
            if (cheats.ContainsKey(_id))
            {
                manager.WrappedSetState(cheats[_id], _state);
                manager.UpdateCheatState(cheats[_id]);
                return true;
            }
            Debug.LogWarning($"Could not find cheat with id: '{_id}'");
            return false;
        }
    }

    [HarmonyPatch(typeof(CheatsManager))]
    public class CheatsManagerPatch
    {
        public static UnityEvent CheatStateChanged;

        [HarmonyPatch("Start")]
        static void StartPrefix()
        {
            if (CheatStateChanged == null)
            {
                CheatStateChanged = new UnityEvent();
            }
        }

        [HarmonyPatch("WrappedSetState")]
        static void Postfix(CheatsManager __instance)
        {
            CheatStateChanged.Invoke();
        }
    }
}
