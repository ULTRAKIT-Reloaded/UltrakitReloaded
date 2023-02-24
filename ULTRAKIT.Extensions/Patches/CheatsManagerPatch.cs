using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULTRAKIT.Extensions.Patches
{
    [HarmonyPatch(typeof(CheatsManager))]
    public class CheatsManagerPatch
    {
        public static CheatStateChangedEvent CheatStateChanged
        {
            get { return Events.CheatStateChanged; }
            set { Events.CheatStateChanged = value; }
        }

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void StartPrefix(CheatsManager __instance)
        {
            if (CheatStateChanged == null)
            {
                CheatStateChanged = new CheatStateChangedEvent();
            }
        }

        [HarmonyPatch("WrappedSetState")]
        [HarmonyPostfix]
        public static void Postfix(CheatsManager __instance, ICheat targetCheat)
        {
            CheatStateChanged.Invoke(targetCheat.Identifier);
        }
    }
}
