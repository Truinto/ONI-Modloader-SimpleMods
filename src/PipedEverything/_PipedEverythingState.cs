//#define LOCALE
using System.Collections.Generic;
using Newtonsoft.Json;
using Common;
using HarmonyLib;
using System.IO;
using System;
using UnityEngine;
using static STRINGS.ELEMENTS;
using static STRINGS.BUILDINGS.PREFABS;

namespace PipedEverything
{
    public class PipedEverythingState
    {
        public int version { get; set; } = 3;

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
            //new PipeConfig(FertilizerMakerConfig.ID, true, x: 1, y: 0, SimHashes.Phosphorite),
            new PipeConfig(FertilizerMakerConfig.ID, false, x: 2, y: 1, SimHashes.Fertilizer),
            new PipeConfig(FertilizerMakerConfig.ID, false, x: 2, y: 2, SimHashes.Methane),

            new PipeConfig(EthanolDistilleryConfig.ID, true, x: 2, y: 0, SimHashes.Creature), // ?
            new PipeConfig(EthanolDistilleryConfig.ID, false, x: 0, y: 0, SimHashes.ToxicSand, Color.gray),
            new PipeConfig(EthanolDistilleryConfig.ID, false, x: 2, y: 2, SimHashes.CarbonDioxide),

            new PipeConfig(PolymerizerConfig.ID, false, x: 0, y: 1, SimHashes.CarbonDioxide),
            new PipeConfig(PolymerizerConfig.ID, false, x: 1, y: 0, SimHashes.Steam),

            new PipeConfig(MilkFatSeparatorConfig.ID, false, x: 2, y: 2, SimHashes.CarbonDioxide),

            new PipeConfig(DesalinatorConfig.ID, false, x: 0, y: 0, SimHashes.Salt),

            new PipeConfig(AlgaeDistilleryConfig.ID, false, x: 1, y: 0, SimHashes.Algae),

            // Oxygen
            new PipeConfig(AlgaeHabitatConfig.ID, true, x: 0, y: 1, SimHashes.Water),
            new PipeConfig(AlgaeHabitatConfig.ID, false, x: 0, y: 0, SimHashes.DirtyWater, storageIndex: 1),
            new PipeConfig(AlgaeHabitatConfig.ID, false, x: 0, y: 1, SimHashes.Oxygen),

            new PipeConfig(ElectrolyzerConfig.ID, false, x: 1, y: 1, SimHashes.Oxygen),
            new PipeConfig(ElectrolyzerConfig.ID, false, x: 0, y: 1, SimHashes.Hydrogen),

            new PipeConfig(RustDeoxidizerConfig.ID, false, x: 1, y: 1, SimHashes.Oxygen),
            new PipeConfig(RustDeoxidizerConfig.ID, false, x: 0, y: 1, SimHashes.ChlorineGas),

            new PipeConfig(MineralDeoxidizerConfig.ID, false, x: 0, y: 1, SimHashes.Oxygen),

            new PipeConfig(SublimationStationConfig.ID, false, x: 0, y: 0, SimHashes.ContaminatedOxygen),

            // Cooking            
            new PipeConfig(GourmetCookingStationConfig.ID, false, x: 1, y: 2, SimHashes.CarbonDioxide),

            // Utility & other
            new PipeConfig(OilWellCapConfig.ID, false, x: 2, y: 1, SimHashes.CrudeOil),
            new PipeConfig(OilWellCapConfig.ID, false, x: 1, y: 1, SimHashes.Methane),

            new PipeConfig(DecontaminationShowerConfig.ID, false, x: 0, y: 0, SimHashes.DirtyWater),

            new PipeConfig(WallToiletConfig.ID, false, x: -1, y: 0, SimHashes.DirtyWater),

            //new PipeConfig(HydroponicFarmConfig.ID, true, x: -1, y: 0, "Liquid"),

            //StorageLocker any input
            //Kiln

            // Advanced Generator+
            new PipeConfig("RefinedCarbonGenerator", true, x: 0, y: 0, SimHashes.RefinedCarbon),
            new PipeConfig("RefinedCarbonGenerator", false, x: 1, y: 2, SimHashes.CarbonDioxide),

            new PipeConfig("NaphthaGenerator", false, x: 0, y: 3, SimHashes.CarbonDioxide, storageIndex: 1),

            new PipeConfig("EcoFriendlyMethaneGenerator", true, x: 0, y: 0, SimHashes.Sand),
            new PipeConfig("EcoFriendlyMethaneGenerator", false, x: 2, y: 2, SimHashes.CarbonDioxide),
            new PipeConfig("EcoFriendlyMethaneGenerator", false, x: 1, y: 1, SimHashes.Water, storageIndex: 1),

        };

        #region _implementation

        public static Config.Manager<PipedEverythingState> StateManager;

        public static void BeforeUpdate()
        {
        }

        public static bool OnUpdate(PipedEverythingState state)
        {
            if (state.version < 2)
            {
                state.Configs.AddRange(new PipeConfig[] {
                    // Advanced Generator+
                    new PipeConfig("RefinedCarbonGenerator", true, x: 0, y: 0, SimHashes.RefinedCarbon),
                    new PipeConfig("RefinedCarbonGenerator", false, x: 1, y: 2, SimHashes.CarbonDioxide),

                    new PipeConfig("NaphthaGenerator", false, x: 0, y: 3, SimHashes.CarbonDioxide, storageIndex: 1),

                    new PipeConfig("EcoFriendlyMethaneGenerator", true, x: 0, y: 0, SimHashes.Sand),
                    new PipeConfig("EcoFriendlyMethaneGenerator", false, x: 2, y: 2, SimHashes.CarbonDioxide),
                    new PipeConfig("EcoFriendlyMethaneGenerator", false, x: 1, y: 1, SimHashes.Water, storageIndex: 1),
                });
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

        public static void AddConfig(string id, bool input, int x, int y, string[] filter, Color32? color = null, int? storageIndex = null, int? storageCapacity = null)
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
