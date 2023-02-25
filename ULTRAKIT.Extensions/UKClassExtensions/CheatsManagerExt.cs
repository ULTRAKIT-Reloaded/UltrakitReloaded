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
        /// <summary>
        /// Prints all registered cheat IDs to the console (can be opened in-game using F8).
        /// </summary>
        /// <param name="manager"></param>
        public static void PrintCheatIDs(this CheatsManager manager)
        {
            foreach (KeyValuePair<String, ICheat> cheat in manager.GetPrivate<Dictionary<String, ICheat>>("idToCheat"))
            {
                Debug.Log(cheat.Key);
            }
        }

        /// <summary>
        /// <para>Enables/disables a cheat with the given ID. </para>
        /// Checks if the cheat is registered and combines WrappedSetState and UpdateCheatState.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns>`true` if the cheat is registered, `false` otherwise.</returns>
        public static bool SetCheatState(this CheatsManager manager, string id, bool enabled)
        {
            Dictionary<String, ICheat> cheats = manager.GetPrivate<Dictionary<String, ICheat>>("idToCheat");
            if (cheats.ContainsKey(id))
            {
                manager.WrappedSetState(cheats[id], enabled);
                manager.UpdateCheatState(cheats[id]);
                return true;
            }
            Debug.LogWarning($"Could not find cheat with id: '{id}'");
            return false;
        }
    }
}
