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
        public int version { get; set; } = 1;

        public List<ElementContainer> Elements { get; set; } = new List<ElementContainer>()
        {
            new ElementContainer() { Id = "Oxygen", molarMass = 15.9f }
        };

        public static Config.Manager<CustomizeElementsState> StateManager = new Config.Manager<CustomizeElementsState>(Config.PathHelper.CreatePath("CustomizeElements"), true, OnUpdate, null);

        public static bool OnUpdate(CustomizeElementsState state)
        {
            return true;
        }
    }
}