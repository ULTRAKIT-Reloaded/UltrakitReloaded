using GameConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;

namespace ULTRAKIT.Core.Commands
{
    public class AltSetter : ICommand
    {
        private string[] weaponNames = new string[]
        {
            "rev0",
            "rev1",
            "rev2",
            "sho0",
            "sho1",
            "sho2",
            "nai0",
            "nai1",
            "nai2",
            "rai0",
            "rai1",
            "rai2",
            "rock0",
            "rock1",
            "rock2"
        };

        public string Name => "Set Weapon Alt";
        public string Description => "Sets a weapon to either normal or alternate variants.";
        public string Command => "setalt";

        public void Execute(GameConsole.Console con, string[] args)
        {
            if (args.Length < 2)
            {
                con.PrintLine("Usage: setalt <weapon> <equipStatus>");
                return;
            }
            if (!weaponNames.Contains(args[0]))
            {
                con.PrintLine("Invalid weapon name. Valid names are:");
                foreach (string weap in weaponNames)
                    con.PrintLine(weap);
                return;
            }
            int state = Convert.ToInt32(args[1]);
            if (state != 0 && state != 1 && state != 2)
            {
                con.PrintLine("Invalid weapon state. Valids states are: [0, 1, 2]");
                return;
            }

            if (!PrefsManager.Instance.prefMap.ContainsKey($"weapon.{args[0]}"))
                PrefsManager.Instance.prefMap.Add($"weapon.{args[0]}", state);
            PrefsManager.Instance.prefMap[$"weapon.{args[0]}"] = state;
            GunSetter.Instance.RefreshWeapons();
        }
    }
}
