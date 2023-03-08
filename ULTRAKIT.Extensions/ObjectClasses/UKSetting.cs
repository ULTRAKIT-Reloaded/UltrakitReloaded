using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Humanizer;

namespace ULTRAKIT.Extensions.ObjectClasses
{
    [Serializable]
    public class SettingChangedEvent : UnityEvent<UKSetting> { }

    [Serializable]
    public class UKSetting
    {
        public SettingChangedEvent OnValueChanged = new SettingChangedEvent();
        public string Section { get; internal set; }
        public string Heading { get; internal set; }
        public string Name { get; internal set; }
        public string ID { get; internal set; }
    }

    public class UKCheckbox : UKSetting
    {
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
        public string Value { get; internal set; }
        public string[] Options { get; internal set; }

        public UKPicker(string section, string heading, string name, string[] options, int startingIndex)
        {
            Section = section;
            Heading = heading;
            Name = name;
            ID = "picker." + name.Dehumanize();
            Options = options;
            Value = options[startingIndex];
        }

        public string GetValue()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            Value = value;
            OnValueChanged.Invoke(this);
        }
    }

    public class UKSlider : UKSetting
    {
        public float Value { get; internal set; }
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
        public UKKeyBinding Binding { get; internal set; }

        public UKKeySetting(string heading, string name, KeyCode defaultKey)
        {
            Section = "CONTROLS";
            Heading = heading;
            Name = name;
            ID = "keybind." + name.Dehumanize();
            Key = defaultKey;
            Binding = new UKKeyBinding(defaultKey);
        }

        public KeyCode GetValue()
        {
            return Key;
        }

        public void SetValue(KeyCode key)
        {
            Key = key;
            Binding.Key = key;
            OnValueChanged.Invoke(this);
        }

        public UKKeyBinding GetBinding()
        {
            return Binding;
        }
    }

    public class UKKeyBinding : InputActionState
    {
        public class OnBindingChangedEvent : UnityEvent<KeyCode> { }

        public KeyCode Key;
        public UnityEvent OnKeyPressed = new UnityEvent();
        public OnBindingChangedEvent OnBindingChanged = new OnBindingChangedEvent();

        internal UKKeyBinding(KeyCode defaultKey)
        {

        }
    }
}
