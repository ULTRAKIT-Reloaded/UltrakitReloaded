using System;
using System.Collections.Generic;
using System.Reflection;

namespace ULTRAKIT.Extensions
{
    public static class ReflectionExt
    {
        /// <summary>
        /// Attempts to set the value of a field matching the given name, including private fields.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetFieldValue(this object obj, string name, object value, bool isPrivate = false)
        {
            if (isPrivate)
                obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
            else
                obj.GetType().GetField(name).SetValue(obj, value);
        }

        /// <summary>
        /// Attempts to retrieve the value of a field of the given name, including private fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetFieldValue<T>(this object obj, string name, bool isPrivate = false)
        {
            if (isPrivate)
                return (T)obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
            else
                return (T)obj.GetType().GetField(name).GetValue(obj);
        }

        /// <summary>
        /// Attempts to set the value of a property matching the given name, including private properties.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue(this object obj, string name, object value, bool isPrivate = false)
        {
            if (isPrivate)
                obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
            else
                obj.GetType().GetProperty(name).SetValue(obj, value);
        }

        /// <summary>
        /// Attempts to retrieve the value of a property of the given name, including private properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this object obj, string name, bool isPrivate = false)
        {
            if (isPrivate)
                return (T)obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
            else
                return (T)obj.GetType().GetProperty(name).GetValue(obj);
        }

        /// <summary>
        /// Returns an array of all fields, public and private.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static FieldInfo[] GetFieldInfos(this object obj)
        {
            List<FieldInfo> infos = new List<FieldInfo>();
            Type type = obj.GetType();
            infos.AddRange(type.GetFields());
            infos.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
            return infos.ToArray();
        }

        /// <summary>
        /// Returns an array of all properties, public and private.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertyInfos(this object obj)
        {
            List<PropertyInfo> infos = new List<PropertyInfo>();
            Type type = obj.GetType();
            infos.AddRange(type.GetProperties());
            infos.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
            return infos.ToArray();
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
