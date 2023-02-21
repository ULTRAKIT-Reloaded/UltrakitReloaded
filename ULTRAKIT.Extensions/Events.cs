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
    }

    // Event Types
    [System.Serializable]
    public class CheatStateChangedEvent : UnityEvent<string> { }
}
