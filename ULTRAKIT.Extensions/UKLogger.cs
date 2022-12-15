using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ULTRAKIT.Extensions
{
    public static class UKLogger
    {
        public static void Log(object obj)
        {
            Debug.Log($@"[ULTRAKIT] {obj}");
        }

        public static void LogWarning(object obj)
        {
            Debug.LogWarning($@"[ULTRAKIT] {obj}");
        }

        public static void LogError(object obj)
        {
            Debug.LogError($@"[ULTRAKIT] {obj}");
        }
    }
}
