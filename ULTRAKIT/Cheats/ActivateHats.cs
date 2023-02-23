using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Extensions;
using ULTRAKIT.Loader;
using UnityEngine.SceneManagement;
using ULTRAKIT.Loader.Loaders;

namespace ULTRAKIT.Core.Cheats
{
    public class ActivateHats : ICheat
    {
        public string LongName => "Toggle Top Hats";
        public string Identifier => "ULTRAKIT.toggle_hats";
        public string ButtonEnabledOverride => "Hats Enabled";
        public string ButtonDisabledOverride => "Hats Disabled";
        public bool DefaultState => false;
        public bool IsActive => false;
        public string Icon => "warning";
        public StatePersistenceMode PersistenceMode => StatePersistenceMode.Persistent;

        public void Enable()
        {
            HatLoader.SetAllActive("ultrakit.tophat", true);
        }

        public void Disable()
        {
            HatLoader.SetAllActive("ultrakit.tophat", false);
        }

        public void Update() { }
    }
}