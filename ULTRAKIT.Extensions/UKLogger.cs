using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ULTRAKIT.Extensions
{
    public static class UKLogger
    {
        public static void Log(object obj)
        {
            string callName = Assembly.GetCallingAssembly().GetName().Name;
            Debug.Log($@"[{callName}] {obj}");
        }

        public static void LogWarning(object obj)
        {
            string callName = Assembly.GetCallingAssembly().GetName().Name;
            Debug.LogWarning($@"[{callName}] {obj}");
        }

        public static void LogError(object obj)
        {
            string callName = Assembly.GetCallingAssembly().GetName().Name;
            Debug.LogError($@"[{callName}] {obj}");
        }
    }
}
