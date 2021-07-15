using System;
using System.Collections.Generic;
using Common;
using HarmonyLib;

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
    }
}