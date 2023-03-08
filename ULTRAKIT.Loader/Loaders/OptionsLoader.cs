using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions.ObjectClasses;
using UnityEngine;
using UnityEngine.Events;

namespace ULTRAKIT.Loader.Loaders
{
    public static class OptionsLoader
    {
        public static UKKey SetKeyBind(string heading, string name, KeyCode defaultKey)
        {
            string id = "keybind." + name.Dehumanize();
            if (Registries.key_registry.ContainsKey(id))
            {
                return Registries.key_registry[id];
            }

            UKKey keybind = new UKKey(heading, name, defaultKey);
            Registries.key_registry.Add(keybind.ID, keybind);
            return keybind;
        }

        public static bool GetKeyBind(string name, out UnityEvent onKeyPressed)
        {
            string id = "keybind." + name.Dehumanize();
            if (Registries.key_registry.ContainsKey(id))
            {
                onKeyPressed = Registries.key_registry[id].OnKeyPressed;
                return true;
            }
            onKeyPressed = null;
            return false;
        }
    }
}
