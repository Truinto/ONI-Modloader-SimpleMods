using System;
using System.Collections.Generic;

namespace CustomizeGeyser
{
    public class CustomizeGeyserState
    {
        public int version { get; set; } = 4;
        public bool Enabled { get; set; } = true;
        
        public List<GeyserStruct> Geysers { get; set; } = new List<GeyserStruct>() {
            new GeyserStruct(id: "steam", temperature: 378.15f),
            new GeyserStruct(id: "slimy_po2", temperature: 378.15f, Disease: "ZombieSpores", DiseaseCount: 5000),
            new GeyserStruct(id: "molten_tungsten", anim: "geyser_molten_iron_kanim", element: "MoltenTungsten",
                Name: "Tungsten Volcano", Description: "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Tungsten", "MOLTENTUNGSTEN") + ".",
                temperature: 3773.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 150f, minIterationLength: 480f,
                maxIterationLength: 1080f, minIterationPercent: 0.02f, maxIterationPercent: 0.1f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "molten_aluminum", anim: "geyser_molten_iron_kanim", element: "MoltenAluminum",
                Name: "Aluminum Volcano", Description: "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Aluminum", "MOLTENALUMINUM") + ".",
                temperature: 2273.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 150f, minIterationLength: 480f,
                maxIterationLength: 1080f, minIterationPercent: 0.02f, maxIterationPercent: 0.1f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "molten_steel", anim: "geyser_molten_iron_kanim", element: "MoltenSteel",
                Name: "Steel Volcano", Description: "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Steel", "MOLTENSTEEL") + ".",
                temperature: 2773.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 150f, minIterationLength: 480f,
                maxIterationLength: 1080f, minIterationPercent: 0.02f, maxIterationPercent: 0.1f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "molten_glass", anim: "geyser_molten_iron_kanim", element: "MoltenGlass",
                Name: "Glass Volcano", Description: "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Glass", "MOLTENGLASS") + ".",
                temperature: 2273.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 150f, minIterationLength: 480f,
                maxIterationLength: 1080f, minIterationPercent: 0.02f, maxIterationPercent: 0.1f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "liquid_coolant", anim: "geyser_liquid_water_slush_kanim", element: "SuperCoolant",
                Name: "Super Coolant Geyser", Description: "A highly pressurized geyser that periodically erupts with hot " + STRINGS.UI.FormatAsLink("Super Coolant", "SUPERCOOLANT") + ".",
                temperature: 673.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 500f, minIterationLength: 60f,
                maxIterationLength: 1140f, minIterationPercent: 0.1f, maxIterationPercent: 0.9f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "liquid_ethanol", anim: "geyser_liquid_water_filthy_kanim", element: "Ethanol",
                Name: "Ethanol Geyser", Description: "A highly pressurized geyser that periodically erupts with boiling " + STRINGS.UI.FormatAsLink("Ethanol", "ETHANOL") + ".",
                temperature: 343.15f, minRatePerCycle: 2000f, maxRatePerCycle: 4000f, maxPressure: 500f, minIterationLength: 60f,
                maxIterationLength: 1140f, minIterationPercent: 0.1f, maxIterationPercent: 0.9f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f, Disease: "PollenGerms", DiseaseCount: 50)
        };

        public bool RandomizerEnabled { get; set; } = true;
        public bool RandomizerUsesMapSeed { get; set; } = false;
        public bool RandomizerRerollsCycleRate { get; set; } = true;
        public bool RandomizerPopupGeyserDiscoveryInfo { get; set; } = true;
        public bool RandomizerHighlanderMode { get; set; } = false;
        public bool RandomizerHighlanderRetroactive { get; set; } = false;
        public Dictionary<string, int> RNGTable { get; set; } = new Dictionary<string, int> {
            { "steam", 1 },
            { "hot_steam", 1 },
            { "hot_water", 1 },
            { "slush_water", 1 },
            { "filthy_water", 1 },
            { "salt_water", 1 },
            { "small_volcano", 1 },
            { "big_volcano", 1 },
            { "liquid_co2", 1 },
            { "hot_co2", 1 },
            { "hot_hydrogen", 1 },
            { "hot_po2", 1 },
            { "slimy_po2", 1 },
            { "chlorine_gas", 1 },
            { "methane", 1 },
            { "molten_copper", 1 },
            { "molten_iron", 1 },
            { "molten_gold", 1 },
            { "oil_drip", 1 },
            { "molten_tungsten", 1 },
            { "molten_aluminum", 1 },
            { "molten_glass", 1 },
            { "liquid_ethanol", 1 },
            { "molten_steel", 0 },
            { "liquid_coolant", 0 }
        };

        public bool GeyserMorphEnabled { get; set; } = true;
        public int GeyserMorphWorktime { get; set; } = 300;

        public static Config.Manager<CustomizeGeyserState> StateManager = new Config.Manager<CustomizeGeyserState>(Config.Helper.CreatePath("Customize Geyser"), true, UpdateFunction);

        public static bool UpdateFunction(CustomizeGeyserState state)
        {
            switch(state.version)
            {
                case 1:
                    state.RandomizerEnabled = false;
                    break;
                default:
                    break;
            }

            return true;
        }

        public class GeyserStruct
        {
            public string id;
            public string element;
            public string anim;
            public int? width;
            public int? height;
            public float? temperature;
            public float? minRatePerCycle;
            public float? maxRatePerCycle;
            public float? maxPressure;
            public float? minIterationLength;
            public float? maxIterationLength;
            public float? minIterationPercent;
            public float? maxIterationPercent;
            public float? minYearLength;
            public float? maxYearLength;
            public float? minYearPercent;
            public float? maxYearPercent;
            public string Name;
            public string Description;
			public string Disease;
			public int? DiseaseCount;

            public GeyserStruct(
                string id,
                string element = null,
                string anim = null,
                int? width = null,
                int? height = null,
                float? temperature = null,
                float? minRatePerCycle = null,
                float? maxRatePerCycle = null,
                float? maxPressure = null,
                float? minIterationLength = null,
                float? maxIterationLength = null,
                float? minIterationPercent = null,
                float? maxIterationPercent = null,
                float? minYearLength = null,
                float? maxYearLength = null,
                float? minYearPercent = null,
                float? maxYearPercent = null,
                string Name = null,
                string Description = null,
				string Disease = null,
				int? DiseaseCount = null)
            {
                this.id = id;
                this.element = element;
                this.anim = anim;
                this.width = width;
                this.height = height;
                this.temperature = temperature;
                this.minRatePerCycle = minRatePerCycle;
                this.maxRatePerCycle = maxRatePerCycle;
                this.maxPressure = maxPressure;
                this.minIterationLength = minIterationLength;
                this.maxIterationLength = maxIterationLength;
                this.minIterationPercent = minIterationPercent;
                this.maxIterationPercent = maxIterationPercent;
                this.minYearLength = minYearLength;
                this.maxYearLength = maxYearLength;
                this.minYearPercent = minYearPercent;
                this.maxYearPercent = maxYearPercent;
                this.Name = Name;
                this.Description = Description;
				this.Disease = Disease;
				this.DiseaseCount = DiseaseCount;
            }
        }
    }
}