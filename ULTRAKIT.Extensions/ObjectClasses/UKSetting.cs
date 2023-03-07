using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ULTRAKIT.Extensions.ObjectClasses
{
    public class UKSetting
    {
        public string Section;
        public string Heading;
    }

    public class UKCheckbox : UKSetting
    {
        public bool Value;

        public UKCheckbox(string section, string heading, bool defaultValue)
        {
            Section = section;
            Heading = heading;
            Value = defaultValue;
        }

        public bool GetValue()
        {
            return Value;
        }
    }

    public class UKPicker : UKSetting
    {
        public string Value;
        public string[] Options;

        public UKPicker(string section, string heading, string[] options, int startingIndex)
        {
            Section = section;
            Heading = heading;
            Options = options;
            Value = options[startingIndex];
        }

        public string GetValue()
        {
            return Value;
        }
    }

    public class UKSlider : UKSetting
    {
        public float Value;
        private Tuple<float, float> valueRange;

        public UKSlider(string section, string heading, float min, float max, float defaultValue)
        {
            Section = section;
            Heading = heading;
            valueRange = new Tuple<float, float>(min, max);
            Value = defaultValue;
        }

        public float GetValue()
        {
            return Value;
        }
    }
}
