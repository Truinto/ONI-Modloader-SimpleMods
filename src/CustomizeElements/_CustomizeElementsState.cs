//#define LOCALE
using System.Collections.Generic;
using Newtonsoft.Json;
using Common;
using HarmonyLib;
using System.IO;

namespace CustomizeElements
{
    public class CustomizeElementsState
    {
        public int version { get; set; } = 1; // TODO make _FumiKMod.cs and set ModName

        public List<ElementContainer> Elements { get; set; } = new List<ElementContainer>()
        {
            new ElementContainer("Oxygen") { molarMass = 15.9f },
            new ElementContainer("Aluminum", "BuildableAny", "Metal"),
            new ElementContainer("Copper", "BuildableAny", "Metal"),
            new ElementContainer("DepletedUranium", "BuildableAny", "Metal"),
            new ElementContainer("Gold", "BuildableAny", "Metal"),
            new ElementContainer("Iron", "BuildableAny", "Metal"),
            new ElementContainer("Cobalt", "BuildableAny", "Metal"),
            new ElementContainer("Lead", "BuildableAny", "Metal"),
            new ElementContainer("Tungsten", "BuildableAny", "Metal", "Plumbable"),
            new ElementContainer("Katairite", "BuildableRaw", "BuildableAny"),
        };

        public static Config.Manager<CustomizeElementsState> StateManager = new Config.Manager<CustomizeElementsState>(Config.PathHelper.CreatePath("CustomizeElements"), true, OnUpdate, null);

        public static bool OnUpdate(CustomizeElementsState state)
        {
            return true;
        }
    }
}