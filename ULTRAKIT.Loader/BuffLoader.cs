using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using UnityEngine;

namespace ULTRAKIT.Loader
{
    public static class BuffLoader
    {
        public static List<IBuff> buffRegistry = new List<IBuff>();

        public static void RegisterBuff(IBuff buff)
        {
            Debug.Log($"Loading buff {buff.id} into {buffRegistry}");
            buffRegistry.Add(buff);
        }
    }
}
