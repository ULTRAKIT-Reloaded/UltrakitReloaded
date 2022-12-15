using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Extensions;

namespace ULTRAKIT
{
    public static class Refresher
    {
        private static void Enable()
        {
            GunSetter.Instance.RefreshWeapons();
            cheat.Disable();
        }

        private static void Disable()
        {
        }

        private static void OnUpdate()
        {
        }

        public static Cheat cheat = new Cheat
        {
            LongName = "Refresh Weapons",
            Identifier = "ULTRAKIT.refresh_weapons",
            ButtonEnabledOverride = "Refreshing",
            ButtonDisabledOverride = "Refresh",
            DefaultState = false,
            PersistenceMode = StatePersistenceMode.NotPersistent,
            EnableScript = Enable,
            DisableScript = Disable,
            UpdateScript = OnUpdate,
        };
    }
}
