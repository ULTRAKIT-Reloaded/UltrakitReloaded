using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Extensions;

namespace ULTRAKIT.Core.Cheats
{
    public class Refresher : ICheat
    {
        public string LongName => "Refresh Weapons";
        public string Identifier => "ULTRAKIT.refresh_weapons";
        public string ButtonEnabledOverride => "Refreshing";
        public string ButtonDisabledOverride => "Refresh";
        public bool DefaultState => false;
        public bool IsActive => false;
        public string Icon => "warning";
        public StatePersistenceMode PersistenceMode => StatePersistenceMode.NotPersistent;

        public void Enable()
        {
            GunSetter.Instance.RefreshWeapons();
            Disable();
        }

        public void Disable() { }

        public void Update() { }
    }
}
