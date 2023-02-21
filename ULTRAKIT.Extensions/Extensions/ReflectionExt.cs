using System;
using System.Reflection;

namespace ULTRAKIT.Extensions
{
    public static class ReflectionExt
    {
        public static void SetPrivate(this object obj, string name, object value)
        {
            obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
        }

        public static T GetPrivate<T>(this object obj, string name)
        {
            return (T)obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        }

        public static Type GetInternalType(string name)
        {
            Type type = typeof(GunControl).Assembly.GetType(name);
            if (type == null)
                UKLogger.LogWarning($"Could not find type {name}");
            return type ?? default;
        }
    }
}
