using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULTRAKIT.Extensions
{
    public static class GunSetterExtension
    {
        public static void RefreshWeapons(this GunSetter gs)
        {
            try
            {
                var storedSlot = gs.gunc.currentSlot;
                var storedVariant = gs.gunc.currentVariation;
                gs.ResetWeapons();
                gs.gunc.currentSlot = storedSlot;
                gs.gunc.currentVariation = storedVariant;
                gs.gunc.YesWeapon();
            }
            catch
            {
            }
        }
    }
}
