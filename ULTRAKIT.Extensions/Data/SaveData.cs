using Newtonsoft.Json;
using System;
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

        public static bool SetValue<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
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
    }

    [System.Serializable]
    public class PersistentData
    {
        // Internal Data
        public Dictionary<string, int[]> weapon_order;
        public Dictionary<string, int[]> weapon_status;
        public Dictionary<string, bool> weapon_unlock;

        // External Data
        

        internal static readonly PersistentData Default = new PersistentData()
        {
            weapon_order = new Dictionary<string, int[]>(),
            weapon_status = new Dictionary<string, int[]>(),
            weapon_unlock = new Dictionary<string, bool>()
        };
    }
}
