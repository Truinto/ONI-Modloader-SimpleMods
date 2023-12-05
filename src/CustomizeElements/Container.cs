using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static Common.Helpers;

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
        public string sublimateId; // reused
        public string convertId; // no use
        public string sublimateFX;
        public float? sublimateRate; // reused
        public float? sublimateEfficiency; // no use
        public float? sublimateProbability; // no use
        public float? offGasPercentage; // no use
        public float? lightAbsorptionFactor;
        public float? radiationAbsorptionFactor;
        public float? radiationPer1000Mass;
        public float? toxicity;
        //public Dictionary<string, float> elementComposition;
        public string materialCategory;
        public List<string> oreTags;
        public int? buildMenuSort;
        public List<AttributeContainer> attributeModifiers;

        public float? sublimate_min; // min amount to transform
        public float? sublimate_overpressure; // max pressure to sublimate in
        public float? sublimate_pwr; // exponent in formular

        public ElementContainer()
        { }

        public ElementContainer(string Id, params string[] tags)
        {
            this.Id = Id;
            this.oreTags = tags == null || tags.Length == 0 ? null : tags.ToList();
        }

        public ElementContainer(string Id, params AttributeContainer[] attributes)
        {
            this.Id = Id;
            this.attributeModifiers = attributes == null || attributes.Length == 0 ? null : attributes.ToList();
        }
    }
}