using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Extensions;
using ULTRAKIT.Loader;
using UnityEngine.SceneManagement;

namespace ULTRAKIT
{
    public static class ActivateHats
    {
        private static void Enable()
        {
            HatLoader.SetAllActive("ultrakit.tophat", true);
        }

        private static void Disable()
        {
            HatLoader.SetAllActive("ultrakit.tophat", false);
        }

        private static void OnUpdate() { }

        public static Cheat cheat = new Cheat
        {
            LongName = "Toggle Top Hats",
            Identifier = "ULTRAKIT.toggle_hats",
            ButtonEnabledOverride = "Hats Enabled",
            ButtonDisabledOverride = "Hats Disabled",
            DefaultState = false,
            PersistenceMode = StatePersistenceMode.Persistent,
            EnableScript = Enable,
            DisableScript = Disable,
            UpdateScript = OnUpdate,
        };
    }
}