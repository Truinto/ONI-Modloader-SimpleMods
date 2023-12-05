//#define LOCALE
using System.Collections.Generic;
using Newtonsoft.Json;
using Common;
using HarmonyLib;
using System.IO;
using static Common.Helpers;

namespace CustomizeElements
{
    public class CustomizeElementsState
    {
        public int version { get; set; } = 1;

        public List<ElementContainer> Elements { get; set; } =
        [
            new() { Id = "Oxygen", molarMass = 15.9f },
            new("Aluminum", "BuildableAny", "Metal"),
            new("Copper", "BuildableAny", "Metal"),
            new("DepletedUranium", "BuildableAny", "Metal"),
            new("Gold", "BuildableAny", "Metal"),
            new("Iron", "BuildableAny", "Metal"),
            new("Cobalt", "BuildableAny", "Metal"),
            new("Lead", "BuildableAny", "Metal"),
            new("Tungsten", "BuildableAny", "Metal", "Plumbable"),
            new("Katairite", "BuildableRaw", "BuildableAny"),
            new("Niobium", new AttributeContainer("OverheatTemperature", 100f), new AttributeContainer("Decor", 5f)),
            new("TempConductorSolid", new AttributeContainer("OverheatTemperature", 100f), new AttributeContainer("Decor", 5f)),

            new() { Id = "BleachStone", sublimateId = "ChlorineGas", sublimateFX = "BleachStoneEmissionBubbles", 
                sublimateRate = 0.00020000001f, sublimate_min = 0.0025000002f, sublimate_overpressure = 1.8f, sublimate_pwr = 0.5f },

            new() { Id = "DirtyWater", sublimateId = "ContaminatedOxygen", sublimateFX = "ContaminatedOxygenBubbleWater",
                sublimateRate = 4.0000006E-05f, sublimate_min = 0.025f, sublimate_overpressure = 1.8f, sublimate_pwr = 1f },

            new() { Id = "NuclearWaste", sublimateId = "NuclearWaste", sublimateFX = "NuclearWasteDrip",
                sublimateRate = 0.066f, sublimate_min = 6.6f, sublimate_overpressure = 1000f, sublimate_pwr = 0f },

            new() { Id = "OxyRock", sublimateId = "Oxygen", sublimateFX = "OxygenEmissionBubbles",
                sublimateRate = 0.010000001f, sublimate_min = 0.0050000004f, sublimate_overpressure = 1.8f, sublimate_pwr = 0.7f },

            new() { Id = "SlimeMold", sublimateId = "ContaminatedOxygen", sublimateFX = "ContaminatedOxygenBubble",
                sublimateRate = 0.025f, sublimate_min = 0.125f, sublimate_overpressure = 1.8f, sublimate_pwr = 0f },

            new() { Id = "ToxicSand", sublimateId = "ContaminatedOxygen", sublimateFX = "ContaminatedOxygenBubble",
                sublimateRate = 2.0000001E-05f, sublimate_min = 0.05f, sublimate_overpressure = 1.8f, sublimate_pwr = 0.5f },
        ];

        public static Config.Manager<CustomizeElementsState> StateManager;

        public static bool OnUpdate(CustomizeElementsState state)
        {
            return true;
        }
    }
}