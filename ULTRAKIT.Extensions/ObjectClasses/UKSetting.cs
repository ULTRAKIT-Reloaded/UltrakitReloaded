using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Humanizer;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Reflection;
using Newtonsoft.Json;
using ULTRAKIT.Extensions.Data;

namespace ULTRAKIT.Extensions.Classes
{
    [Serializable]
    public class SettingChangedEvent : UnityEvent<UKSetting> { }

    [Serializable]
    public class UKSetting
    {
        [JsonProperty("OnValueChanged")]
        public SettingChangedEvent OnValueChanged = new SettingChangedEvent();
        [JsonProperty("Section")]
        public string Section { get; internal set; }
        [JsonProperty("Heading")]
        public string Heading { get; internal set; }
        [JsonProperty("Name")]
        public string Name { get; internal set; }
        [JsonProperty("ID")]
        public string ID { get; internal set; }
    }

    public class UKCheckbox : UKSetting
    {
        [JsonProperty("ValueCheck")]
        public bool Value { get; internal set; }

        public UKCheckbox(string section, string heading, string name, string id, bool defaultValue)
        {
            Section = section;
            Heading = heading;
            Name = name;
            ID = id;
            Value = defaultValue;
        }

        public bool GetValue()
        {
            return Value;
        }

        public void SetValue(bool value)
        {
            Value = value;
            OnValueChanged.Invoke(this);
        }
    }

    public class UKPicker : UKSetting
    {
        [JsonProperty("ValuePick")]
        public int Value { get; internal set; }
        [JsonProperty("Options")]
        public string[] Options { get; internal set; }

        public UKPicker(string section, string heading, string name, string id, string[] options, int startingIndex)
        {
            Section = section;
            Heading = heading;
            Name = name;
            ID = id;
            Options = options;
            Value = startingIndex;
        }

        public int GetValue()
        {
            return Value;
        }

        public void SetValue(int value)
        {
            Value = value;
            OnValueChanged.Invoke(this);
        }
    }

    public class UKSlider : UKSetting
    {
        [JsonProperty("ValueSlide")]
        public float Value { get; internal set; }
        [JsonProperty("Range")]
        public Tuple<float, float> Range { get; internal set; }

        public UKSlider(string section, string heading, string name, string id, float min, float max, float defaultValue)
        {
            Section = section;
            Heading = heading;
            Name = name;
            ID = id;
            Range = new Tuple<float, float>(min, max);
            Value = defaultValue;
        }

        public float GetValue()
        {
            return Value;
        }

        public void SetValue(float value)
        {
            Value = value;
            OnValueChanged.Invoke(this);
        }
    }

    public class UKKeySetting : UKSetting
    {
        public KeyCode Key { get; internal set; }
        public InputManager.BindingInfo Binding { get; internal set; }

        public UKKeySetting(string heading, string name, KeyCode defaultKey)
        {
            Section = "Controls";
            Heading = heading;
            Name = name;
            ID = "keybind." + name.Dehumanize();
            Key = defaultKey;
            InputAction action = new InputAction(name, InputActionType.Button);
            
            InputManager.BindingInfo info = new InputManager.BindingInfo();
            info.Action = action;
            info.Name = name.Dehumanize();
            info.Offset = 0;
            info.DefaultKey = defaultKey;

            Binding = info;

            // Goodbye evil reflection, you will be missed

            /*Type infoType = ReflectionExt.GetInternalType("InputManager+BindingInfo");
            object info = Activator.CreateInstance(infoType);

            info.SetFieldValue("Action", action);
            info.SetFieldValue("Name", name.Dehumanize());
            info.SetFieldValue("Offset", 0);
            info.SetFieldValue("DefaultKey", defaultKey);

            Binding = info;*/
        }

        public KeyCode GetValue()
        {
            return Key;
        }

        public void SetValue(KeyCode key)
        {
            Key = key;
            OnValueChanged.Invoke(this);
        }
    }
}
