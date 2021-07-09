using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;
using Common;
using System.Linq;

namespace CustomizeBuildings
{
    public class ElementConverterContainer
    {
        public Dictionary<string, float> Inputs = new Dictionary<string, float>();
        public Dictionary<string, float> Outputs = new Dictionary<string, float>();
        public float[] OutputTemperature;
        public bool[] OutputStore;
        public float MassMultiplier = 1.0f;

        public ElementConverterContainer Input(string element, float value)
        {
            Inputs.Add(element, value);
            return this;
        }

        public ElementConverterContainer Output(string element, float value)
        {
            Outputs.Add(element, value);
            return this;
        }

        public ElementConverterContainer Temperature(params float[] temp)
        {
            OutputTemperature = temp;
            return this;
        }

        public ElementConverterContainer Store(params bool[] store)
        {
            OutputStore = store;
            return this;
        }

        public ElementConverter Set(ElementConverter ec)
        {
            var input = new ElementConverter.ConsumedElement[this.Inputs.Count];
            for (int i = 0; i < input.Length; i++)
                input[i] = new ElementConverter.ConsumedElement(this.Inputs.ElementAt(i).Key, this.Inputs.ElementAt(i).Value);

            var output = new ElementConverter.OutputElement[this.Outputs.Count];
            for (int i = 0; i < output.Length; i++)
            {
                float temp = this.OutputTemperature != null && this.OutputTemperature.Length > i ? this.OutputTemperature[i] : 0f;
                bool store = this.OutputStore != null && this.OutputStore.Length > i ? this.OutputStore[i] : true;

                output[i] = new ElementConverter.OutputElement(this.Outputs.ElementAt(i).Value, this.Outputs.ElementAt(i).Key.ToSimHash(), temp, temp <= 0f, store);
            }

            ec.consumedElements = input;
            ec.outputElements = output;
            ec.OutputMultiplier = this.MassMultiplier;

            return ec;
        }
    }
}