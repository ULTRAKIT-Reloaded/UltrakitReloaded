using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ULTRAKIT.Extensions
{
    public static class CheatsManagerExtension
    {
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
                return (cheats[_id].IsActive);
            }
            Debug.LogWarning($"Could not find cheat with id: '{_id}'");
            return false;
        }

        public static bool GetCheatState(this CheatsManager manager, string _id)
        {
            Dictionary<String, ICheat> cheats = manager.GetPrivate<Dictionary<String, ICheat>>("idToCheat");
            if (cheats.ContainsKey(_id))
            {
                return (cheats[_id].IsActive);
            }
            Debug.LogWarning($"Could not find cheat with id: '{_id}'");
            return false;
        }
    }
}
