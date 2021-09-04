using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CustomizeElements
{
    public class ElementContainer
    {
        public string Id;
        public float? specificHeatCapacity;
        public float? thermalConductivity;
        public float? molarMass;
        public float? strength;
        public float? flow;
        public float? maxMass;
        public float? maxCompression;
        public float? viscosity;
        public float? minHorizontalFlow;
        public float? minVerticalFlow;
        public float? solidSurfaceAreaMultiplier;
        public float? liquidSurfaceAreaMultiplier;
        public float? gasSurfaceAreaMultiplier;
        public Element.State? state;
        public byte? hardness;
        public float? lowTemp;
        public string lowTempTransitionTarget;
        public string lowTempTransitionOreID;
        public float? lowTempTransitionOreMassConversion;
        public float? highTemp;
        public string highTempTransitionTarget;
        public string highTempTransitionOreID;
        public float? highTempTransitionOreMassConversion;
        public string sublimateId;
        public string convertId;
        public string sublimateFX;
        public float? sublimateRate;
        public float? sublimateEfficiency;
        public float? sublimateProbability;
        public float? offGasPercentage;
        public float? lightAbsorptionFactor;
        public float? radiationAbsorptionFactor;
        public float? radiationPer1000Mass;
        public float? toxicity;
        //public Dictionary<string, float> elementComposition;
        public string materialCategory;
        public List<string> oreTags;
        public int? buildMenuSort;

        public ElementContainer()
        { }

        public ElementContainer(string Id, params string[] tags)
        {
            this.Id = Id;
            this.oreTags = tags == null || tags.Length == 0 ? null : tags.ToList();
        }
    }

    public class OreContainer
    {
        public bool? DecayStorage;
        [JsonConverter(typeof(StringEnumConverter))]
        public SpawnFXHashes FxHash;

        [JsonConverter(typeof(StringEnumConverter))]
        public SimHashes SublimatedElement;
        public float? SublimationRate;
        public float? MinSublimationAmount;
        public float? MaxDestinationMass;
        public float? MassPower;
        public byte? DiseaseIdx;
        public int? DiseaseCount;
    }
}