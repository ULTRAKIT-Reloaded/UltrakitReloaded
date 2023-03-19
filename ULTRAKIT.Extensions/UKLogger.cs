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
        /// <summary>
        /// Logs a message to the Unity console prefixed with the name of the calling assembly.
        /// </summary>
        /// <param name="obj"></param>
        public static void Log(object obj)
        {
            string callName = Assembly.GetCallingAssembly().GetName().Name;
            Debug.Log($@"[{callName}] {obj}");
        }

        /// <summary>
        /// Logs a warning to the Unity console prefixed with the name of the calling assembly.
        /// </summary>
        /// <param name="obj"></param>
        public static void LogWarning(object obj)
        {
            string callName = Assembly.GetCallingAssembly().GetName().Name;
            Debug.LogWarning($@"[{callName}] {obj}");
        }

        /// <summary>
        /// Logs an error to the Unity console prefixed with the name of the calling assembly.
        /// </summary>
        /// <param name="obj"></param>
        public static void LogError(object obj)
        {
            string callName = Assembly.GetCallingAssembly().GetName().Name;
            Debug.LogError($@"[{callName}] {obj}");
        }

        private static int counter = 0;
        public static void LogCounter()
        {
            string callName = Assembly.GetCallingAssembly().GetName().Name;
            Debug.LogError($@"[{callName}] {counter}");
            counter++;
        }
    }
}
