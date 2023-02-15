using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ULTRAKIT.Extensions.Data
{
    public static class SaveData
    {
        private const string folderName = "data";
        private static string dataFilePath = GetDataPath("save.ultradata");

        public static PersistentData data
        {
            get
            {
                if (_data == null)
                    Load();
                return _data;
            }
            set
            {
                if (value != null && value != _data)
                {
                    _data = value;
                    Save();
                    return;
                }
            }
        }

        private static PersistentData _data;

        public static string GetDataPath(params string[] subpath)
        {
            string modDir = Assembly.GetExecutingAssembly().Location;
            modDir = Path.GetDirectoryName(modDir);
            string localPath = Path.Combine(modDir, folderName);

            if (subpath.Length > 0)
            {
                string subLocalPath = Path.Combine(subpath);
                localPath = Path.Combine(localPath, subLocalPath);
            }

            return localPath;
        }

        public static bool Internal_SetValue<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
                Save();
                return true;
            }
            dict.Add(key, value);
            Save();
            return false;
        }

        private static bool Internal_SetPrivateValue<TKey, TValue>(Dictionary<string, Dictionary<TKey, TValue>> dict, TKey key, TValue value, string assemblyName)
        {
            if (dict.ContainsKey(assemblyName))
            {
                return Internal_SetValue(dict[assemblyName], key, value);
            }
            dict.Add(assemblyName, new Dictionary<TKey, TValue>());
            Internal_SetValue(dict[assemblyName], key, value);
            return false;
        }

        public static void Save()
        {
            if (!Directory.Exists(GetDataPath()))
                Directory.CreateDirectory(GetDataPath());

            string json = JsonConvert.SerializeObject(_data);
            File.WriteAllText(dataFilePath, json);
            UKLogger.Log("Saved persistent data");
        }

        public static void Load()
        {
            UKLogger.Log("Loading persistent data...");
            if (!File.Exists(dataFilePath))
            {
                _data = PersistentData.Default;
                Save();
                return;
            }

            string json;
            using (StreamReader reader = new StreamReader(dataFilePath))
            {
                json = reader.ReadToEnd();
            }
            _data = JsonConvert.DeserializeObject<PersistentData>(json);
            return;
        }

        public static bool SetPersistent(string key, object value, bool global)
        {
            if (global)
            {
                if (value is string)
                    { Internal_SetValue(data.g_string_data, key, (string)value); return true; }
                if (value is int)
                    { Internal_SetValue(data.g_int_data, key, (int)value); return true; }
                if (value is float)
                    { Internal_SetValue(data.g_float_data, key, (float)value); return true; }
                if (value is bool)
                    { Internal_SetValue(data.g_bool_data, key, (bool)value); return true; }
                if (value is string[])
                    { Internal_SetValue(data.g_string_data_array, key, (string[])value); return true; }
                if (value is int[])
                    { Internal_SetValue(data.g_int_data_array, key, (int[])value); return true; }
                if (value is float[])
                    { Internal_SetValue(data.g_float_data_array, key, (float[])value); return true; }
                if (value is bool[])
                    { Internal_SetValue(data.g_bool_data_array, key, (bool[])value); return true; }
                return false;
            }
            string assembly = Assembly.GetCallingAssembly().GetName().Name;
            if (value is string)
                { Internal_SetPrivateValue(data.p_string_data, key, (string)value, assembly); return true; }
            if (value is int)
                { Internal_SetPrivateValue(data.p_int_data, key, (int)value, assembly); return true; }
            if (value is float)
                { Internal_SetPrivateValue(data.p_float_data, key, (float)value, assembly); return true; }
            if (value is bool)
                { Internal_SetPrivateValue(data.p_bool_data, key, (bool)value, assembly); return true; }
            if (value is string[])
                { Internal_SetPrivateValue(data.p_string_data_array, key, (string[])value, assembly); return true; }
            if (value is int[])
                { Internal_SetPrivateValue(data.p_int_data_array, key, (int[])value, assembly); return true; }
            if (value is float[])
                { Internal_SetPrivateValue(data.p_float_data_array, key, (float[])value, assembly); return true; }
            if (value is bool[])
                { Internal_SetPrivateValue(data.p_bool_data_array, key, (bool[])value, assembly); return true; }
            return false;
        }

        public static bool GetPersistent<T>(string key, bool global, out object value)
        {
            Type type = typeof(T);
            if (global)
            {
                if (type.IsEquivalentTo(typeof(string)))
                {
                    string outValue;
                    bool success = data.g_string_data.TryGetValue(key, out outValue);
                    value = outValue;
                    return success;
                }
                if (type.IsEquivalentTo(typeof(int)))
                {
                    int outValue;
                    bool success = data.g_int_data.TryGetValue(key, out outValue);
                    value = outValue;
                    return success;
                }
                if (type.IsEquivalentTo(typeof(float)))
                {
                    float outValue;
                    bool success = data.g_float_data.TryGetValue(key, out outValue);
                    value = outValue;
                    return success;
                }
                if (type.IsEquivalentTo(typeof(bool)))
                {
                    bool outValue;
                    bool success = data.g_bool_data.TryGetValue(key, out outValue);
                    value = outValue;
                    return success;
                }
                if (type.IsEquivalentTo(typeof(string[])))
                {
                    string[] outValue;
                    bool success = data.g_string_data_array.TryGetValue(key, out outValue);
                    value = outValue;
                    return success;
                }
                if (type.IsEquivalentTo(typeof(int[])))
                {
                    int[] outValue;
                    bool success = data.g_int_data_array.TryGetValue(key, out outValue);
                    value = outValue;
                    return success;
                }
                if (type.IsEquivalentTo(typeof(float[])))
                {
                    float[] outValue;
                    bool success = data.g_float_data_array.TryGetValue(key, out outValue);
                    value = outValue;
                    return success;
                }
                if (type.IsEquivalentTo(typeof(bool[])))
                {
                    bool[] outValue;
                    bool success = data.g_bool_data_array.TryGetValue(key, out outValue);
                    value = outValue;
                    return success;
                }
                value = default(T);
                return false;
            }
            string assembly = Assembly.GetCallingAssembly().GetName().Name;
            if (type.IsEquivalentTo(typeof(string)))
            {
                bool success = false;
                string outValue = default(string);
                Dictionary<string, string> outDict;
                if (data.p_string_data.TryGetValue(assembly, out outDict))
                    success = outDict.TryGetValue(key, out outValue);
                value = outValue;
                return success;
            }
            if (type.IsEquivalentTo(typeof(int)))
            {
                bool success = false;
                int outValue = default(int);
                Dictionary<string, int> outDict;
                if (data.p_int_data.TryGetValue(assembly, out outDict))
                    success = outDict.TryGetValue(key, out outValue);
                value = outValue;
                return success;
            }
            if (type.IsEquivalentTo(typeof(float)))
            {
                bool success = false;
                float outValue = default(float);
                Dictionary<string, float> outDict;
                if (data.p_float_data.TryGetValue(assembly, out outDict))
                    success = outDict.TryGetValue(key, out outValue);
                value = outValue;
                return success;
            }
            if (type.IsEquivalentTo(typeof(bool)))
            {
                bool success = false;
                bool outValue = default(bool);
                Dictionary<string, bool> outDict;
                if (data.p_bool_data.TryGetValue(assembly, out outDict))
                    success = outDict.TryGetValue(key, out outValue);
                value = outValue;
                return success;
            }
            if (type.IsEquivalentTo(typeof(string[])))
            {
                bool success = false;
                string[] outValue = default(string[]);
                Dictionary<string, string[]> outDict;
                if (data.p_string_data_array.TryGetValue(assembly, out outDict))
                    success = outDict.TryGetValue(key, out outValue);
                value = outValue;
                return success;
            }
            if (type.IsEquivalentTo(typeof(int[])))
            {
                bool success = false;
                int[] outValue = default(int[]);
                Dictionary<string, int[]> outDict;
                if (data.p_int_data_array.TryGetValue(assembly, out outDict))
                    success = outDict.TryGetValue(key, out outValue);
                value = outValue;
                return success;
            }
            if (type.IsEquivalentTo(typeof(float[])))
            {
                bool success = false;
                float[] outValue = default(float[]);
                Dictionary<string, float[]> outDict;
                if (data.p_float_data_array.TryGetValue(assembly, out outDict))
                    success = outDict.TryGetValue(key, out outValue);
                value = outValue;
                return success;
            }
            if (type.IsEquivalentTo(typeof(bool[])))
            {
                bool success = false;
                bool[] outValue = default(bool[]);
                Dictionary<string, bool[]> outDict;
                if (data.p_bool_data_array.TryGetValue(assembly, out outDict))
                    success = outDict.TryGetValue(key, out outValue);
                value = outValue;
                return success;
            }
            value = default(T);
            return false;
        }
    }

    [System.Serializable]
    public class PersistentData
    {
        // Internal Data
        public Dictionary<string, int[]> weapon_order;
        public Dictionary<string, int[]> weapon_status;
        public Dictionary<string, bool> weapon_unlock;

        // External Data - Global
        public Dictionary<string, string> g_string_data;
        public Dictionary<string, int> g_int_data;
        public Dictionary<string, float> g_float_data;
        public Dictionary<string, bool> g_bool_data;
        public Dictionary<string, string[]> g_string_data_array;
        public Dictionary<string, int[]> g_int_data_array;
        public Dictionary<string, float[]> g_float_data_array;
        public Dictionary<string, bool[]> g_bool_data_array;

        // External Data - Private
        public Dictionary<string, Dictionary<string, string>> p_string_data;
        public Dictionary<string, Dictionary<string, int>> p_int_data;
        public Dictionary<string, Dictionary<string, float>> p_float_data;
        public Dictionary<string, Dictionary<string, bool>> p_bool_data;
        public Dictionary<string, Dictionary<string, string[]>> p_string_data_array;
        public Dictionary<string, Dictionary<string, int[]>> p_int_data_array;
        public Dictionary<string, Dictionary<string, float[]>> p_float_data_array;
        public Dictionary<string, Dictionary<string, bool[]>> p_bool_data_array;

        internal static readonly PersistentData Default = new PersistentData()
        {
            weapon_order = new Dictionary<string, int[]>(),
            weapon_status = new Dictionary<string, int[]>(),
            weapon_unlock = new Dictionary<string, bool>(),

            g_string_data = new Dictionary<string, string>(),
            g_int_data = new Dictionary<string, int>(),
            g_float_data = new Dictionary<string, float>(),
            g_bool_data = new Dictionary<string, bool>(),
            g_string_data_array = new Dictionary<string, string[]>(),
            g_int_data_array = new Dictionary<string, int[]>(),
            g_float_data_array = new Dictionary<string, float[]>(),
            g_bool_data_array = new Dictionary<string, bool[]>(),

            p_string_data = new Dictionary<string, Dictionary<string, string>>(),
            p_int_data = new Dictionary<string, Dictionary<string, int>>(),
            p_float_data = new Dictionary<string, Dictionary<string, float>>(),
            p_bool_data = new Dictionary<string, Dictionary<string, bool>>(),
            p_string_data_array = new Dictionary<string, Dictionary<string, string[]>>(),
            p_int_data_array = new Dictionary<string, Dictionary<string, int[]>>(),
            p_float_data_array = new Dictionary<string, Dictionary<string, float[]>>(),
            p_bool_data_array = new Dictionary<string, Dictionary<string, bool[]>>()
        };
    }
}
