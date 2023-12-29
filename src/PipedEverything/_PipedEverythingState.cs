//#define LOCALE
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;

namespace PipedEverything
{
    public class PipedEverythingState
    {
        public int version { get; set; } = 5;

        public List<PipeConfig> Configs { get; set; } = new()
        {
            // Power
            new PipeConfig(GeneratorConfig.ID, true, x: 0, y: 0, SimHashes.Carbon),
            new PipeConfig(GeneratorConfig.ID, false, x: 1, y: 2, SimHashes.CarbonDioxide),

            new PipeConfig(WoodGasGeneratorConfig.ID, true, x: 0, y: 0, SimHashes.Creature), // ?
            new PipeConfig(WoodGasGeneratorConfig.ID, false, x: 0, y: 1, SimHashes.CarbonDioxide),

            new PipeConfig(PetroleumGeneratorConfig.ID, false, x: 0, y: 3, SimHashes.CarbonDioxide),
            new PipeConfig(PetroleumGeneratorConfig.ID, false, x: 1, y: 1, SimHashes.DirtyWater),

            new PipeConfig(MethaneGeneratorConfig.ID, false, x: 2, y: 2, SimHashes.CarbonDioxide),
            new PipeConfig(MethaneGeneratorConfig.ID, false, x: 1, y: 1, SimHashes.DirtyWater),

            // Refinement
            new PipeConfig(OilRefineryConfig.ID, false, x: -1, y: 3, SimHashes.Methane),

            new PipeConfig(FertilizerMakerConfig.ID, true, x: 0, y: 0, SimHashes.Dirt, SimHashes.Phosphorite),
            new PipeConfig(FertilizerMakerConfig.ID, false, x: 2, y: 1, SimHashes.Fertilizer),
            new PipeConfig(FertilizerMakerConfig.ID, false, x: 2, y: 2, SimHashes.Methane),

            new PipeConfig(EthanolDistilleryConfig.ID, true, x: 2, y: 0, SimHashes.Creature), // ?
            new PipeConfig(EthanolDistilleryConfig.ID, false, x: 0, y: 0, SimHashes.ToxicSand) { Color = Color.gray },
            new PipeConfig(EthanolDistilleryConfig.ID, false, x: 2, y: 2, SimHashes.CarbonDioxide),

            //new PipeConfig(PolymerizerConfig.ID, false, x: 0, y: 1, SimHashes.CarbonDioxide),
            new PipeConfig(PolymerizerConfig.ID, false, x: 1, y: 0, SimHashes.Steam),

            new PipeConfig(MilkFatSeparatorConfig.ID, false, x: 2, y: 2, SimHashes.CarbonDioxide),

            //new PipeConfig(DesalinatorConfig.ID, false, x: 0, y: 0, SimHashes.Salt),

            new PipeConfig(AlgaeDistilleryConfig.ID, false, x: 1, y: 0, SimHashes.Algae),

            new PipeConfig(WaterPurifierConfig.ID, true, x: 0, y: 0, SimHashes.Sand, SimHashes.Regolith),
            new PipeConfig(WaterPurifierConfig.ID, false, x: 1, y: 0, SimHashes.ToxicSand),

            // Oxygen
            new PipeConfig(AlgaeHabitatConfig.ID, true, x: 0, y: 1, SimHashes.Water),
            new PipeConfig(AlgaeHabitatConfig.ID, false, x: 0, y: 0, SimHashes.DirtyWater) { StorageIndex = 1 },
            new PipeConfig(AlgaeHabitatConfig.ID, false, x: 0, y: 1, SimHashes.Oxygen),

            new PipeConfig(ElectrolyzerConfig.ID, false, x: 1, y: 1, SimHashes.Oxygen),
            new PipeConfig(ElectrolyzerConfig.ID, false, x: 0, y: 1, SimHashes.Hydrogen),

            new PipeConfig(RustDeoxidizerConfig.ID, false, x: 1, y: 1, SimHashes.Oxygen),
            new PipeConfig(RustDeoxidizerConfig.ID, false, x: 0, y: 1, SimHashes.ChlorineGas),

            new PipeConfig(MineralDeoxidizerConfig.ID, false, x: 0, y: 1, SimHashes.Oxygen),

            new PipeConfig(SublimationStationConfig.ID, false, x: 0, y: 0, SimHashes.ContaminatedOxygen),

            // Research
            new PipeConfig(ResearchCenterConfig.ID, true, x: 0, y: 0, SimHashes.Dirt),
            new PipeConfig(AdvancedResearchCenterConfig.ID, true, x: 0, y: 0, SimHashes.Water),

            // ComplexFabricator            
            new PipeConfig(GourmetCookingStationConfig.ID, false, x: 1, y: 2, SimHashes.CarbonDioxide),

            new PipeConfig(MicrobeMusherConfig.ID, true, x: 1, y: 0, SimHashes.Water),

            new PipeConfig(KilnConfig.ID, true, x: 0, y: 0, SimHashes.Carbon, SimHashes.Clay) { StorageCapacity = 500f },
            new PipeConfig(KilnConfig.ID, false, x: 1, y: 0, SimHashes.Ceramic, SimHashes.RefinedCarbon) { StorageIndex = 2 },

            new PipeConfig(MetalRefineryConfig.ID, true, x: 0, y: 0, SimHashes.AluminumOre, SimHashes.Cuprite, SimHashes.Electrum, SimHashes.IronOre, SimHashes.GoldAmalgam, SimHashes.Cobaltite, SimHashes.FoolsGold, SimHashes.Wolframite, SimHashes.Lime, SimHashes.RefinedCarbon, SimHashes.Iron) { StorageCapacity = 500f },
            new PipeConfig(MetalRefineryConfig.ID, false, x: 1, y: 0, SimHashes.Aluminum, SimHashes.Copper, SimHashes.Iron, SimHashes.Gold, SimHashes.Cobalt, SimHashes.Lead, SimHashes.Tungsten, SimHashes.Steel) { StorageIndex = 2 },

            new PipeConfig(GlassForgeConfig.ID, true, x: 1, y: 0, SimHashes.Sand) { StorageCapacity = 500f },

            new PipeConfig(SupermaterialRefineryConfig.ID, true, x: 0, y: 0, "Solid") { StorageCapacity = 500f },

            new PipeConfig(UraniumCentrifugeConfig.ID, true, x: 0, y: 0, SimHashes.UraniumOre) { StorageCapacity = 500f },
            new PipeConfig(UraniumCentrifugeConfig.ID, false, x: 0, y: 0, SimHashes.EnrichedUranium) { StorageIndex = 2 },

            // Utility & other
            new PipeConfig(OilWellCapConfig.ID, false, x: 2, y: 1, SimHashes.CrudeOil),
            new PipeConfig(OilWellCapConfig.ID, false, x: 1, y: 1, SimHashes.Methane),

            new PipeConfig(DecontaminationShowerConfig.ID, false, x: 0, y: 0, SimHashes.DirtyWater),
            new PipeConfig(WallToiletConfig.ID, false, x: 0, y: 1, SimHashes.DirtyWater),
            new PipeConfig(HydroponicFarmConfig.ID, true, x: 0, y: 0, "Solid"),
            new PipeConfig(StorageLockerConfig.ID, true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig(StorageLockerSmartConfig.ID, true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig(SweepBotStationConfig.ID, false, x: 0, y: 0, "Solid") { StorageIndex = 1 },
            new PipeConfig(SweepBotStationConfig.ID, false, x: 0, y: 0, "Liquid") { StorageIndex = 1 },

            // Advanced Generator+
            new PipeConfig("RefinedCarbonGenerator", true, x: 0, y: 0, SimHashes.RefinedCarbon),
            new PipeConfig("RefinedCarbonGenerator", false, x: 1, y: 2, SimHashes.CarbonDioxide),

            new PipeConfig("NaphthaGenerator", false, x: 0, y: 3, SimHashes.CarbonDioxide) { StorageIndex = 1 },

            new PipeConfig("EcoFriendlyMethaneGenerator", true, x: 0, y: 0, SimHashes.Sand),
            new PipeConfig("EcoFriendlyMethaneGenerator", false, x: 2, y: 2, SimHashes.CarbonDioxide),
            new PipeConfig("EcoFriendlyMethaneGenerator", false, x: 1, y: 1, SimHashes.Water) { StorageIndex = 1 },

        };

        #region _implementation

        public static Config.Manager<PipedEverythingState> StateManager;

        public static void BeforeUpdate()
        {
        }

        public static bool OnUpdate(PipedEverythingState state)
        {
            if (state.version < 5)
            {
                state.Configs.RemoveAll(a => a.Id == PolymerizerConfig.ID && a.Filter.FirstOrDefault() == SimHashes.CarbonDioxide.ToString());
            }

            return true;
        }

        public object ReadSettings()
        {
            return StateManager.State;
        }

        public void WriteSettings(object settings)
        {
            if (settings is PipedEverythingState state)
                StateManager.TrySaveConfigurationState(state);
            else
                StateManager.TrySaveConfigurationState();
        }

        public string GetConfigPath()
        {
            return GetStaticConfigPath();
        }

        public static string GetStaticConfigPath()
        {
            string path = FumiKMod.ModName;
            if (Helpers.ActiveLocale.NotEmpty() && Helpers.ActiveLocale != "en")
                path += "_" + Helpers.ActiveLocale;
            return Config.PathHelper.CreatePath(path);
        }

        #endregion

        #region _api

        [Obsolete]
        private static void AddConfig(string id, bool input, int x, int y, string[] filter, Color32? color, int? storageIndex, int? storageCapacity)
        {
            StateManager.State.Configs.Add(new PipeConfig() { Id = id, Input = input, OffsetX = x, OffsetY = y, Filter = filter, Color = color, StorageIndex = storageIndex, StorageCapacity = storageCapacity });
        }

        public static void AddConfig(string id, bool input, int x, int y, string[] filter, Color32? color = null, int? storageIndex = null, float? storageCapacity = null)
        {
            StateManager.State.Configs.Add(new PipeConfig() { Id = id, Input = input, OffsetX = x, OffsetY = y, Filter = filter, Color = color, StorageIndex = storageIndex, StorageCapacity = storageCapacity });
        }

        public static void RemoveConfig(string id, int x, int y)
        {
            StateManager.State.Configs.RemoveAll(r => r.Id == id && r.OffsetX == x && r.OffsetY == y);
        }

        #endregion
    }
}
