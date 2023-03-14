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

namespace ULTRAKIT.Extensions.ObjectClasses
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

        public UKCheckbox(string section, string heading, string name, bool defaultValue)
        {
            Section = section;
            Heading = heading;
            Name = name;
            ID = "checkbox." + name.Dehumanize();
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

        public UKPicker(string section, string heading, string name, string[] options, int startingIndex)
        {
            Section = section;
            Heading = heading;
            Name = name;
            ID = "picker." + name.Dehumanize();
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

        public UKSlider(string section, string heading, string name, float min, float max, float defaultValue)
        {
            Section = section;
            Heading = heading;
            Name = name;
            ID = "slider." + name.Dehumanize();
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
            

            // Goodbye evil reflection, you will be missed

            /*Type infoType = ReflectionExt.GetInternalType("InputManager+BindingInfo");
            object info = Activator.CreateInstance(infoType);

            info.SetFieldValue("Action", action);
            info.SetFieldValue("Name", name.Dehumanize());
            info.SetFieldValue("Offset", 0);
            info.SetFieldValue("DefaultKey", defaultKey);

            Binding = info;*/

            InputManager.BindingInfo info = new InputManager.BindingInfo();
            info.Action = action;
            info.Name = name.Dehumanize();
            info.Offset = 0;
            info.DefaultKey = defaultKey;

            Binding = info;

            if (InputManager.Instance && InputManager.Instance.Inputs.ContainsKey(info.Name))
                Key = InputManager.Instance.Inputs[info.Name];
        }

        public KeyCode GetValue()
        {
            return Key;
        }

        public void SetValue(KeyCode key)
        {
            Key = key;
            //Binding.Key = key;
            OnValueChanged.Invoke(this);
        }

        //public UKKeyBinding GetBinding()
        //{
        //    return Binding;
        //}
    }

    /*public class UKKeyBinding : InputActionState
    {
        public class KeyChangedEvent : UnityEvent<KeyCode> { }

        public KeyCode Key;
        public UnityEvent OnKeyPressed = new UnityEvent();
        public KeyChangedEvent OnBindingChanged = new KeyChangedEvent();

        internal UKKeyBinding(KeyCode defaultKey, string name)
        {
            Key = defaultKey;
            InputAction action = new InputAction(name, InputActionType.Button);
            var info = ReflectionExt.GetInternalType("BindingInfo");
            info.SetPrivate("Action", action);
            info.SetPrivate("Name", name);
            info.SetPrivate("Offset", 0);
            info.SetPrivate("DefaultKey", Key);
        }
    }*/
}
