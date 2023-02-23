using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Interfaces;
using UnityEngine;

namespace ULTRAKIT.Loader.Loaders
{
    public static class BuffLoader
    {
        private static List<IBuff> buffRegistry => Registries.buff_buffRegistry;

        public static void RegisterBuff(IBuff buff)
        {
            UKLogger.Log($"Loading buff {buff.id} into {Registries.buff_buffRegistry}");
            buffRegistry.Add(buff);
        }
    }
}
