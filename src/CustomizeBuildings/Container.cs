using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;
using Common;
using System.Linq;
using Newtonsoft.Json;

namespace CustomizeBuildings
{
    public class ElementConverterContainer
    {
        public Dictionary<string, float> Inputs;
        public Dictionary<string, float> Outputs;
        public float[] OutputTemperature;
        public bool[] OutputStore;
        public float? MassMultiplier;

        public ElementConverterContainer ModeTemperature()
        {
            Inputs = null;
            Outputs = null;
            return this;
        }

        public ElementConverterContainer Input(string element, float value)
        {
            Inputs = Inputs ?? new Dictionary<string, float>();
            Inputs.Add(element, value);
            return this;
        }

        public ElementConverterContainer Output(string element, float value)
        {
            Outputs = Outputs ?? new Dictionary<string, float>();
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

        public EnergyGenerator Set(EnergyGenerator eg)
        {
            // mode for only setting temperatures
            if (this.Inputs == null && this.Outputs == null)
            {
                bool tooShort = this.OutputTemperature?.Length != eg.formula.outputs.Length;

                for (int i = 0; i < eg.formula.outputs.Length; i++)
                {
                    eg.formula.outputs[i].minTemperature = tooShort ? this.OutputTemperature[0] : this.OutputTemperature[i];
                }

                return eg;
            }

            return eg;
        }

        public ElementConverter Set(ElementConverter ec)
        {
            // mode for only setting temperatures
            if (this.Inputs == null && this.Outputs == null)
            {
                bool tooShort = this.OutputTemperature?.Length != ec.outputElements.Length;

                for (int i = 0; i < ec.outputElements.Length; i++)
                {
                    ec.outputElements[i].minOutputTemperature = tooShort ? this.OutputTemperature[0] : this.OutputTemperature[i];
                    ec.outputElements[i].useEntityTemperature = (tooShort ? this.OutputTemperature[0] : this.OutputTemperature[i]) <= 0f;
                }

                return ec;
            }

            var input = new ElementConverter.ConsumedElement[this.Inputs?.Count ?? 0];
            for (int i = 0; i < input.Length; i++)
                input[i] = new ElementConverter.ConsumedElement(this.Inputs.ElementAt(i).Key, this.Inputs.ElementAt(i).Value);

            var output = new ElementConverter.OutputElement[this.Outputs?.Count ?? 0];
            for (int i = 0; i < output.Length; i++)
            {
                float temp = this.OutputTemperature != null && this.OutputTemperature.Length > i ? this.OutputTemperature[i] : 0f;
                bool store = this.OutputStore != null && this.OutputStore.Length > i ? this.OutputStore[i] : true;

                output[i] = new ElementConverter.OutputElement(this.Outputs.ElementAt(i).Value, this.Outputs.ElementAt(i).Key.ToSimHash(), temp, temp <= 0f, store);
            }

            ec.consumedElements = input;
            ec.outputElements = output;
            ec.OutputMultiplier = this.MassMultiplier ?? 1f;

            return ec;
        }
    }
    public class BuildingStruct(float? PowerConsumption = null, string MaterialCategory = null, float? OverheatTemperature = null,
            float? ExhaustKilowattsWhenActive = null, float? SelfHeatKilowattsWhenActive = null, float? GeneratorWattageRating = null,
            float? GeneratorBaseCapacity = null, float? BaseDecor = null, float? BaseDecorRadius = null, BuildLocationRule? LocationRule = null,
            PermittedRotations? Rotations = null, bool? Floodable = null, bool? IsFoundation = null, float[] ConstructionMass = null,
            float? ThermalConductivity = null, ObjectLayer? ObjectLayer = null, ObjectLayer? TileLayer = null, ObjectLayer? ReplacementLayer = null,
            Grid.SceneLayer? SceneLayer = null)
    {
        public float? PowerConsumption = PowerConsumption;
        public string MaterialCategory = MaterialCategory;
        public float? OverheatTemperature = OverheatTemperature;
        public float? ExhaustKilowattsWhenActive = ExhaustKilowattsWhenActive;
        public float? SelfHeatKilowattsWhenActive = SelfHeatKilowattsWhenActive;
        public float? GeneratorWattageRating = GeneratorWattageRating;
        public float? GeneratorBaseCapacity = GeneratorBaseCapacity;
        public float? BaseDecor = BaseDecor;
        public float? BaseDecorRadius = BaseDecorRadius;
        public BuildLocationRule? LocationRule = LocationRule;
        public PermittedRotations? Rotations = Rotations;
        public bool? Floodable = Floodable;
        public bool? IsFoundation = IsFoundation;
        public float[] ConstructionMass = ConstructionMass;
        public float? ThermalConductivity = ThermalConductivity;
        public ObjectLayer? ObjectLayer = ObjectLayer;
        public ObjectLayer? TileLayer = TileLayer;
        public ObjectLayer? ReplacementLayer = ReplacementLayer;
        public Grid.SceneLayer? SceneLayer = SceneLayer;
    }
    public class BuildingAdv
    {
        public readonly string Id;
        [JsonIgnore]
        public readonly int hash;
        public int? Index;
        public bool MaterialAppend;
        public string MaterialOverride;

        public BuildingAdv(string Id, int? Index = null, bool MaterialAppend = true, string MaterialOverride = null)
        {
            this.Id = Id;
            this.hash = Hash.SDBMLower(Id);
            this.Index = Index;
            this.MaterialAppend = MaterialAppend;
            this.MaterialOverride = MaterialOverride;
        }

        public bool AppliesTo(Tag tag)
        {
            return this.hash == tag.GetHash();
        }
    }

}
