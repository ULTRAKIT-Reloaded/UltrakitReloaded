using System;
using System.Reflection;

namespace ULTRAKIT.Extensions
{
    public static class ReflectionExt
    {
        /// <summary>
        /// Attempts to set the value of a private field matching the given name.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetPrivate(this object obj, string name, object value)
        {
            obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
        }

        /// <summary>
        /// Attempts to retrieve the value of a private field of the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetPrivate<T>(this object obj, string name)
        {
            return (T)obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        }

        /// <summary>
        /// <para>Retrieves an ULTRAKILL type with the given name, including types with the `internal` keyword.</para>
        /// <para>Type can be passed into GetComponent() and the result stored as a Component.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetInternalType(string name)
        {
            Type type = typeof(GunControl).Assembly.GetType(name);
            if (type == null)
                UKLogger.LogWarning($"Could not find type {name}");
            return type ?? default;
        }
    }
}
