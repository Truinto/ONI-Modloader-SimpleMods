using Common;
using Config;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PipedEverything
{
    public class PipedEverythingState : BaseSettings<PipedEverythingState>
    {
        public override int Version { get; set; } = 13;

        public bool GeyserPipes { get; set; } = false;
        public bool GeyserPipesUnlimited { get; set; } = true;
        public float GeyserStorageKG { get; set; } = 1000f;
        public float GeyserGasStorageKG { get; set; } = 100f;

        public float SolidPipeOutput { get; set; } = 20f;

        public List<PipeConfig> Configs { get; set; } = new()
        {
            // Power
            new PipeConfig(GeneratorConfig.ID, true, x: 0, y: 0, SimHashes.Carbon),
            new PipeConfig(GeneratorConfig.ID, false, x: 1, y: 2, SimHashes.CarbonDioxide),

            new PipeConfig(WoodGasGeneratorConfig.ID, true, x: 0, y: 0, SimHashes.WoodLog),
            new PipeConfig(WoodGasGeneratorConfig.ID, false, x: 0, y: 1, SimHashes.CarbonDioxide),

            new PipeConfig(PetroleumGeneratorConfig.ID, false, x: 0, y: 3, SimHashes.CarbonDioxide),
            new PipeConfig(PetroleumGeneratorConfig.ID, false, x: 1, y: 1, SimHashes.DirtyWater),

            new PipeConfig(MethaneGeneratorConfig.ID, false, x: 2, y: 2, SimHashes.CarbonDioxide),
            new PipeConfig(MethaneGeneratorConfig.ID, false, x: 1, y: 1, SimHashes.DirtyWater),

            new PipeConfig(PeatGeneratorConfig.ID, true, x: 0, y: 0, SimHashes.Peat) { StorageCapacity = float.PositiveInfinity },
            new PipeConfig(PeatGeneratorConfig.ID, false, x: 1, y: 1, SimHashes.CarbonDioxide),
            new PipeConfig(PeatGeneratorConfig.ID, false, x: 1, y: 0, SimHashes.DirtyWater),

            // Refinement
            new PipeConfig(OilRefineryConfig.ID, false, x: -1, y: 3, SimHashes.Methane),

            new PipeConfig(FertilizerMakerConfig.ID, true, x: 0, y: 0, SimHashes.Dirt, SimHashes.Phosphorite),
            new PipeConfig(FertilizerMakerConfig.ID, false, x: 2, y: 1, SimHashes.Fertilizer),
            new PipeConfig(FertilizerMakerConfig.ID, false, x: 2, y: 2, SimHashes.Methane),

            new PipeConfig(EthanolDistilleryConfig.ID, true, x: 2, y: 0, SimHashes.WoodLog),
            new PipeConfig(EthanolDistilleryConfig.ID, false, x: 0, y: 0, SimHashes.ToxicSand) { Color = Color.gray },
            new PipeConfig(EthanolDistilleryConfig.ID, false, x: 2, y: 2, SimHashes.CarbonDioxide),

            //new PipeConfig(PolymerizerConfig.ID, false, x: 0, y: 1, SimHashes.CarbonDioxide),
            new PipeConfig(PolymerizerConfig.ID, false, x: 1, y: 0, SimHashes.Steam),

            new PipeConfig(MilkFatSeparatorConfig.ID, false, x: 1, y: 3, SimHashes.CarbonDioxide),
            //new PipeConfig(MilkFatSeparatorConfig.ID, false, x: 3, y: 2, SimHashes.MilkFat) { Color = new(242,118,215,255) },
            //new PipeConfig(MilkFatSeparatorConfig.ID, false, x: 3, y: 3, SimHashes.Brine) { Color = Color.cyan },

            //new PipeConfig(DesalinatorConfig.ID, false, x: 0, y: 0, SimHashes.Salt),

            new PipeConfig(AlgaeDistilleryConfig.ID, false, x: 1, y: 0, SimHashes.Algae),

            new PipeConfig(WaterPurifierConfig.ID, true, x: 0, y: 0, SimHashes.Sand, SimHashes.Regolith),
            new PipeConfig(WaterPurifierConfig.ID, false, x: 1, y: 0, SimHashes.ToxicSand),
            new PipeConfig(WaterPurifierConfig.ID, true, x: -1, y: 2, (string[])[]) { OriginalPort = Port.Utility },
            new PipeConfig(WaterPurifierConfig.ID, false, x: 2, y: 2, (string[])[]) { OriginalPort = Port.Utility },

            // Oxygen
            new PipeConfig(AlgaeHabitatConfig.ID, true, x: 0, y: 1, SimHashes.Water),
            new PipeConfig(AlgaeHabitatConfig.ID, false, x: 0, y: 0, SimHashes.DirtyWater) { StorageIndex = 1, StorageCapacity = 800f },
            new PipeConfig(AlgaeHabitatConfig.ID, false, x: 0, y: 1, SimHashes.Oxygen),

            new PipeConfig(ElectrolyzerConfig.ID, false, x: 1, y: 1, SimHashes.Oxygen),
            new PipeConfig(ElectrolyzerConfig.ID, false, x: 0, y: 1, SimHashes.Hydrogen),

            new PipeConfig(RustDeoxidizerConfig.ID, false, x: 1, y: 1, SimHashes.Oxygen),
            new PipeConfig(RustDeoxidizerConfig.ID, false, x: 0, y: 1, SimHashes.ChlorineGas),
            new PipeConfig(RustDeoxidizerConfig.ID, false, x: 1, y: 0, SimHashes.IronOre),
            new PipeConfig(RustDeoxidizerConfig.ID, true, x: 0, y: 0, SimHashes.Rust, SimHashes.Salt) { StorageCapacity = 780f },

            new PipeConfig(MineralDeoxidizerConfig.ID, false, x: 0, y: 1, SimHashes.Oxygen),
            new PipeConfig(MineralDeoxidizerConfig.ID, true, x: 0, y: 0, SimHashes.Algae) { StorageCapacity = 300f },

            new PipeConfig(SublimationStationConfig.ID, false, x: 0, y: 0, SimHashes.ContaminatedOxygen),

            // Research
            new PipeConfig(ResearchCenterConfig.ID, true, x: 0, y: 0, SimHashes.Dirt),
            new PipeConfig(AdvancedResearchCenterConfig.ID, true, x: 0, y: 0, SimHashes.Water),

            // ComplexFabricator            
            new PipeConfig(GourmetCookingStationConfig.ID, false, x: 1, y: 2, SimHashes.CarbonDioxide) { StorageIndex = 0 },

            new PipeConfig(MicrobeMusherConfig.ID, true, x: 1, y: 0, SimHashes.Water),

            //new PipeConfig(KilnConfig.ID, true, x: 0, y: 0, SimHashes.Carbon, SimHashes.Clay) { StorageCapacity = 500f },
            //new PipeConfig(KilnConfig.ID, false, x: 1, y: 0, SimHashes.Ceramic, SimHashes.RefinedCarbon) { StorageIndex = 2 },

            new PipeConfig(MetalRefineryConfig.ID, true, x: 0, y: 0, SimHashes.AluminumOre, SimHashes.Cuprite, SimHashes.Electrum, SimHashes.IronOre, SimHashes.GoldAmalgam, SimHashes.Cobaltite, SimHashes.FoolsGold, SimHashes.Wolframite, SimHashes.Lime, SimHashes.RefinedCarbon, SimHashes.Iron) { StorageCapacity = 500f },
            new PipeConfig(MetalRefineryConfig.ID, false, x: 1, y: 0, SimHashes.Aluminum, SimHashes.Copper, SimHashes.Iron, SimHashes.Gold, SimHashes.Cobalt, SimHashes.Lead, SimHashes.Tungsten, SimHashes.Steel) { StorageIndex = 2 },

            new PipeConfig(GlassForgeConfig.ID, true, x: 1, y: 0, SimHashes.Sand) { StorageCapacity = 500f },

            new PipeConfig(SupermaterialRefineryConfig.ID, true, x: 0, y: 0, "Solid") { StorageCapacity = 500f },

            new PipeConfig(ApothecaryConfig.ID, true, x: 0, y: 0, SimHashes.Water, SimHashes.LiquidGunk),
            new PipeConfig(ApothecaryConfig.ID, true, x: 0, y: 1, "Solid") { StorageCapacity = 500f },
            new PipeConfig(ApothecaryConfig.ID, false, x: 1, y: 0, SimHashes.DirtyWater),
            new PipeConfig(ApothecaryConfig.ID, false, x: 1, y: 1, "Solid"),

            new PipeConfig(UraniumCentrifugeConfig.ID, true, x: 0, y: 0, SimHashes.UraniumOre) { StorageCapacity = 500f },
            new PipeConfig(UraniumCentrifugeConfig.ID, false, x: 0, y: 0, SimHashes.EnrichedUranium) { StorageIndex = 2 },

            new PipeConfig(MilkPressConfig.ID, true, x: 0, y: 0, SimHashes.Water),
            new PipeConfig(MilkPressConfig.ID, true, x: 1, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig(MilkPressConfig.ID, false, x: 0, y: 1, "Liquid") { Color = new(244,212,237,255) },
            new PipeConfig(MilkPressConfig.ID, false, x: 1, y: 1, "Solid") { Color = new(181,173,101,255) },

            // Utility & other
            new PipeConfig(OilWellCapConfig.ID, false, x: 2, y: 1, SimHashes.CrudeOil),
            new PipeConfig(OilWellCapConfig.ID, false, x: 1, y: 1, SimHashes.Methane),

            new PipeConfig(IceKettleConfig.ID, true, x: 0, y: 0, SimHashes.WoodLog) { StorageIndex = 0 },
            new PipeConfig(IceKettleConfig.ID, true, x: 0, y: 1, SimHashes.Ice) { StorageIndex = 1 },
            new PipeConfig(IceKettleConfig.ID, false, x: 1, y: 0, SimHashes.Water) { StorageIndex = 2 },

            new PipeConfig(CampfireConfig.ID, true, x: 0, y: 0, SimHashes.WoodLog),
            new PipeConfig(CampfireConfig.ID, false, x: 0, y: 1, SimHashes.CarbonDioxide),

            new PipeConfig(IceMachineConfig.ID, true, x: 0, y: 0, SimHashes.Water) { StorageIndex = 0 },
            new PipeConfig(IceMachineConfig.ID, false, x: 1, y: 0, SimHashes.Ice, SimHashes.Snow) { StorageIndex = 1 },

            new PipeConfig(AirFilterConfig.ID, false, x: 0, y: 0, SimHashes.Clay),
            new PipeConfig(DecontaminationShowerConfig.ID, false, x: 0, y: 0, SimHashes.DirtyWater),
            new PipeConfig(WallToiletConfig.ID, false, x: 0, y: 1, SimHashes.DirtyWater),
            new PipeConfig(HydroponicFarmConfig.ID, true, x: 0, y: 0, "Solid"),
            new PipeConfig(StorageLockerConfig.ID, true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig(StorageLockerSmartConfig.ID, true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig(SweepBotStationConfig.ID, false, x: 0, y: 0, "Solid") { StorageIndex = 1 },
            new PipeConfig(SweepBotStationConfig.ID, false, x: 0, y: 0, "Liquid") { StorageIndex = 1 },
            new PipeConfig(RefrigeratorConfig.ID, true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig(EspressoMachineConfig.ID, true, x: 0, y: 0, "Solid"),
            new PipeConfig(SodaFountainConfig.ID, true, x: 0, y: 1, SimHashes.CarbonDioxide),
            new PipeConfig(WaterCoolerConfig.ID, true, x: 0, y: 1, SimHashes.Water),

            // Advanced Generator+
            new PipeConfig("RefinedCarbonGenerator", true, x: 0, y: 0, SimHashes.RefinedCarbon),
            new PipeConfig("RefinedCarbonGenerator", false, x: 1, y: 2, SimHashes.CarbonDioxide),

            new PipeConfig("NaphthaGenerator", false, x: 0, y: 3, SimHashes.CarbonDioxide) { StorageIndex = 1 },

            new PipeConfig("EcoFriendlyMethaneGenerator", true, x: 0, y: 0, SimHashes.Sand),
            new PipeConfig("EcoFriendlyMethaneGenerator", false, x: 2, y: 2, SimHashes.CarbonDioxide),
            new PipeConfig("EcoFriendlyMethaneGenerator", false, x: 1, y: 1, SimHashes.Water) { StorageIndex = 1 },

            // Big Storage: Restoraged
            new PipeConfig("BigRefrigerator", true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig("BigBeautifulStorageLocker", true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig("BigSmartStorageLocker", true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig("BigStorageLocker", true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig("BigStorageTile", true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },
            new PipeConfig("StorageTile", true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity },

        };

        public List<string> API_Blacklist { get; set; } = new();

        #region _implementation

        public override string DefaultPath => Path.Combine(Util.RootFolder(), "mods", $"{FumiKMod.ModName}.json");

        protected override string BeforeUpdate(int oldVersion, string json)
        {
            if (oldVersion < 12)
            {
                json = json.Replace("\"version\"", "\"Version\"");
            }
            return json;
        }

        protected override bool OnUpdate()
        {
            if (Version < 5)
            {
                Configs.RemoveAll(a => a.Id == PolymerizerConfig.ID && a.Filter.FirstOrDefault() == SimHashes.CarbonDioxide.ToString());
            }

            if (Version < 7)
            {
                foreach (var config in Configs)
                {
                    for (int i = 0; i < config.Filter.Length; i++)
                        if (config.Filter[i] == "Creature")
                            config.Filter[i] = SimHashes.WoodLog.ToString();
                    if (config.Id == GourmetCookingStationConfig.ID && config.Filter.Contains(SimHashes.CarbonDioxide.ToString()))
                        config.StorageIndex = 0;
                }

                if (!Configs.Any(a => a.Id == IceKettleConfig.ID))
                {
                    Configs.Add(new PipeConfig(IceKettleConfig.ID, true, x: 0, y: 0, SimHashes.WoodLog) { StorageIndex = 0 });
                    Configs.Add(new PipeConfig(IceKettleConfig.ID, true, x: 0, y: 1, SimHashes.Ice) { StorageIndex = 1 });
                    Configs.Add(new PipeConfig(IceKettleConfig.ID, false, x: 1, y: 0, SimHashes.Water) { StorageIndex = 2 });
                }
            }

            if (Version < 8)
            {
                Configs.FirstOrDefault(f => f.Id == AlgaeHabitatConfig.ID && f.Filter.Length == 1 && f.Filter[0] == SimHashes.DirtyWater.ToString())
                    ?.StorageCapacity = 800f;
            }

            if (Version < 9)
            {
                foreach (var config in Configs)
                    config.RemoveMaxAtmosphere = null;
            }

            if (Version < 10)
            {
                addIfNew(new PipeConfig(MilkPressConfig.ID, true, x: 0, y: 0, SimHashes.Water));
                addIfNew(new PipeConfig(MilkPressConfig.ID, true, x: 1, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity });
                addIfNew(new PipeConfig(MilkPressConfig.ID, false, x: 0, y: 1, "Liquid") { Color = new(244, 212, 237, 255) });
                addIfNew(new PipeConfig(MilkPressConfig.ID, false, x: 1, y: 1, "Solid") { Color = new(181, 173, 101, 255) });
                addIfNew(new PipeConfig(PeatGeneratorConfig.ID, true, x: 0, y: 0, "Solid") { StorageCapacity = float.PositiveInfinity });
                addIfNew(new PipeConfig(PeatGeneratorConfig.ID, false, x: 1, y: 1, SimHashes.CarbonDioxide));
                addIfNew(new PipeConfig(PeatGeneratorConfig.ID, false, x: 1, y: 0, SimHashes.DirtyWater));
            }

            if (Version < 13)
            {
                foreach (var config in Configs)
                {
                    if (config.Filter.Length == 1 && config.Filter[0] == SpiceNutConfig.ID)
                        config.Filter[0] = "Solid";
                }
            }

            return true;

            void addIfNew(PipeConfig config)
            {
                if (!Configs.Any(a => a.Id == config.Id && a.Input == config.Input && (a.OffsetX == config.OffsetX && a.OffsetY == config.OffsetY || a.Filter.SequenceEqual(config.Filter))))
                    Configs.Add(config);
            }
        }

        protected override bool OnError(Exception e)
        {
            var text = e.ToString();
            Helpers.Print(text);
            if (text.Length > 1000)
                text = text.Substring(0, 1000);
            PostBootDialog.ToDialog(text);
            return false;
        }

        #endregion
    }
}
