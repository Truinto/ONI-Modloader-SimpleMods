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

        public List<ElementContainer> Elements { get; set; } = new List<ElementContainer>()
        {
            new ElementContainer() { Id = "Oxygen", molarMass = 15.9f },
            new ElementContainer("Aluminum", "BuildableAny", "Metal"),
            new ElementContainer("Copper", "BuildableAny", "Metal"),
            new ElementContainer("DepletedUranium", "BuildableAny", "Metal"),
            new ElementContainer("Gold", "BuildableAny", "Metal"),
            new ElementContainer("Iron", "BuildableAny", "Metal"),
            new ElementContainer("Cobalt", "BuildableAny", "Metal"),
            new ElementContainer("Lead", "BuildableAny", "Metal"),
            new ElementContainer("Tungsten", "BuildableAny", "Metal", "Plumbable"),
            new ElementContainer("Katairite", "BuildableRaw", "BuildableAny"),
            new ElementContainer("Niobium", new AttributeContainer("OverheatTemperature", 100f), new AttributeContainer("Decor", 5f)),
            new ElementContainer("TempConductorSolid", new AttributeContainer("OverheatTemperature", 100f), new AttributeContainer("Decor", 5f)),
        };

        public static Config.Manager<CustomizeElementsState> StateManager;

        public static bool OnUpdate(CustomizeElementsState state)
        {
            return true;
        }
    }
}