using System.Collections.Generic;

namespace CustomizeBuildings
{
    public class CustomizeBuildingsState
    {
        public int version { get; set; } = 26;
        //public bool overwrite_all_with_Klei_default { get; set; } = false;

        public float BatterySmartKJ { get; set; } = 200000f;
        public bool BatterySmartNoRunOff { get; set; } = true;
        public float BatteryLargeKJ { get; set; } = 400000f;
        public float LockerKG { get; set; } = 400000f;
        public float LockerSmartKG { get; set; } = 400000f;
        public float GasReservoirKG { get; set; } = 99000f;
        public float LiquidReservoirKG { get; set; } = 99000f;
        public float RationBoxKG { get; set; } = 400000f;
        public float FridgeKG { get; set; } = 400000f;
        public float CritterFeederKG { get; set; } = 400000f;
        public float FishFeederKG { get; set; } = 400000f;
        public float CanisterFillerKG { get; set; } = 400000f;
        public float ConveyorLoaderKG { get; set; } = 400000f;
        public float ConveyorReceptacleKG { get; set; } = 400000f;
        public float IUserControlledMax { get; set; } = 400000f;

        public float ElectrolizerMaxPressure { get; set; } = 5f;
        public bool AirfilterDropsCanisters { get; set; } = true;

        public bool NewRecipeRockCrusher { get; set; } = true;

        public bool ReservoirNoGround { get; set; } = true;
        public bool ReservoirManualDelivery { get; set; } = false;

        public bool NoDupeValves { get; set; } = true;
        public bool NoDupeSwitches { get; set; } = true;
        public bool NoDupeToogleBuildings { get; set; } = true;
        public bool NoDupeToogleDoors { get; set; } = true;

        public int ScannerInterferenceRadius { get; set; } = 5;
        public float ScannerWorstWarningTime { get; set; } = 30f;
        public float ScannerBestWarningTime { get; set; } = 60f;
        public int ScannerBestNetworkSize { get; set; } = 2;
        public bool LadderCometInvincibility { get; set; } = true;

        public float SolarMaxPower { get; set; } = 600f;
        public float SolarEnergyMultiplier { get; set; } = 1f;

        public bool SteamTurbineEnabled { get; set; } = false;
        public float SteamTurbineWattage { get; set; } = 850f;
        public string SteamTurbineSourceElement { get; set; } = "Steam";
        public string SteamTurbineOutputElement { get; set; } = "Water";
        public float SteamTurbinePumpRateKG { get; set; } = 2f;
        public float SteamTurbineMaxSelfHeat { get; set; } = 64f;
        public float SteamTurbineHeatTransferPercent { get; set; } = 0.1f;
        public float SteamTurbineMinActiveTemperature { get; set; } = 398.15f;
        public float SteamTurbineIdealTemperature { get; set; } = 473.15f;
        public float SteamTurbineOutputTemperature { get; set; } = 368.15f;
        public float SteamTurbineOverheatTemperature { get; set; } = 373.15f;

        public float PipeThroughputPercent { get; set; } = 1.0f;
        public float PipeLiquidMaxPressure { get; set; } = 10f;
        public float PipeGasMaxPressure { get; set; } = 1f;
        public float PipeLiquidPump { get; set; } = 10f;
        public float PipeGasPump { get; set; } = 0.5f;
        public float ConveyorRailPackageSize { get; set; } = 20f;
        public bool ConveyorLoaderHasSlider { get; set; } = true;
        public bool ConveyorReceptacleHasSlider { get; set; } = false;
        public float AutoSweeperCapacity { get; set; } = 1000f;
        public int AutoSweeperRange { get; set; } = 4;
        public bool AutoSweeperSlider { get; set; } = true;
        public int RoboMinerWidth { get; set; } = 16;
        public int RoboMinerHeight { get; set; } = 9;
        public int RoboMinerOffset { get; set; } = 0;
        public bool RoboMinerRegolithTurbo { get; set; } = true;
        public bool RoboMinerDigThroughGlass { get; set; } = true;
        public bool RoboMinerDigAnyTile { get; set; } = true;

        public int WireSmallWatts { get; set; } = 1000;
        public int WireRefinedWatts { get; set; } = 2000;
        public int WireHeavyWatts { get; set; } = 20000;
        public int WireRefinedHeavyWatts { get; set; } = 50000;

        public bool TransitTubeAnywhere { get; set; } = true;
        public bool TransitTubeUTurns { get; set; } = true;
        public float TransitTubeJoulesPerLaunch { get; set; } = 10000f;
        public float TransitTubeJouleCapacity { get; set; } = 40000f;
        //public float TransitTubeSpeed { get; set; } = 18f;

        public bool NoDupeBuildingsGlobal { get; set; } = true;
        public Dictionary<string, bool> NoDupeBuildings { get; set; } = new Dictionary<string, bool> {
            //Fabricators
            { IDApothecary, false },
            { IDClothingFabricator, false },
            { IDCookingStation, false },
            { IDEggCracker, false },
            { IDGlassForge, true },
            { IDGourmetCookingStation, false },
            { IDMetalRefinery, true },
            { IDMicrobeMusher, true },
            { IDRockCrusher, true },
            { IDSuitFabricator, false },
            { IDSupermaterialRefinery, true },
            //other
            { IDOilRefinery, true },
            { IDOilWellCap, true },
            { IDIceCooledFan, false },   //not fully implemented
            { IDRanchStation, false },   //not fully implemented
            { IDShearingStation, false },   //not implemented
            { IDAutomatedCompost, false }   //not implemented
        };

        public bool BuildingBaseSettingGlobalFlag { get; set; } = true;
        public Dictionary<string, BuildingStruct> BuildingBaseSettings { get; set; } = new Dictionary<string, BuildingStruct> {
            { WireHighWattageConfig.ID, new BuildingStruct(LocationRule: BuildLocationRule.Anywhere) },
            { WireRefinedHighWattageConfig.ID, new BuildingStruct(LocationRule: BuildLocationRule.Anywhere, BaseDecor: -5f, BaseDecorRadius: 1f) },
            { TravelTubeConfig.ID, new BuildingStruct(LocationRule: BuildLocationRule.Anywhere) },
            { BedConfig.ID, new BuildingStruct(Rotations: PermittedRotations.FlipH) },
            { LuxuryBedConfig.ID, new BuildingStruct(Rotations: PermittedRotations.FlipH) },
            { StorageLockerConfig.ID, new BuildingStruct(LocationRule: BuildLocationRule.Anywhere) }//,
            //{ DoorConfig.ID, new BuildingStruct(IsFoundation: true) }
        };

        public Dictionary<string, float> MachineMultiplier { get; set; } = new Dictionary<string, float> {
            { "EthanolDistillery", 4f }
        };

        public Dictionary<string, Dictionary<string, Dictionary<string, object>>> AdvancedSettings { get; set; } = null;
        //    = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>() {
        //        { "LiquidHeater", new Dictionary<string, Dictionary<string, object>> { { "SpaceHeater", new Dictionary<string, object> { { "targetTemperature", 1000f} }  } } }
        //}; // PrefabID, Component, Field, Value

        //public static BaseStateManager<CustomizeBuildingsState> StateManager = new BaseStateManager<CustomizeBuildingsState>(new ModFolderPathHelper("CustomizeBuildings", 1818138009L).path, ONI_Common.Paths.GetLogsPath() + ModFolderPathHelper.sep + "CustomizeBuildingsLog.txt");
        public static Config.Manager<CustomizeBuildingsState> StateManager = new Config.Manager<CustomizeBuildingsState>(Config.Helper.CreatePath("CustomizeBuildings"), true, null, Loaded);

        public static void Loaded(CustomizeBuildingsState state)
        {
            return;
            Debug.Log("[CustomizeBuildings] Checking Default settings...");
            //if (state.overwrite_all_with_Klei_default)
                StateManager.TrySaveConfigurationState(KleiSettings);
        }

        public class BuildingStruct
        {
            public float? PowerConsumption;
            public float? OverheatTemperature;
            public string[] MaterialCategory;
            public float? ExhaustKilowattsWhenActive;
            public float? SelfHeatKilowattsWhenActive;
            public float? GeneratorWattageRating;
            public float? GeneratorBaseCapacity;
            public float? BaseDecor;
            public float? BaseDecorRadius;
            public BuildLocationRule? LocationRule;
            public PermittedRotations? Rotations;
            public bool? Floodable;
            public bool? IsFoundation;
            public float? ConstructionMass;
            public float? ThermalConductivity;

            public BuildingStruct(float? PowerConsumption = null, string[] MaterialCategory = null, float? OverheatTemperature = null,
                float? ExhaustKilowattsWhenActive = null, float? SelfHeatKilowattsWhenActive = null, float? GeneratorWattageRating = null,
                float? GeneratorBaseCapacity = null, float? BaseDecor = null, float? BaseDecorRadius = null, BuildLocationRule? LocationRule = null,
                PermittedRotations? Rotations = null, bool? Floodable = null, bool? IsFoundation = null, float? ConstructionMass = null,
                float? ThermalConductivity = null)
            {
                this.PowerConsumption = PowerConsumption;
                this.MaterialCategory = MaterialCategory;
                this.OverheatTemperature = OverheatTemperature;
                this.ExhaustKilowattsWhenActive = ExhaustKilowattsWhenActive;
                this.SelfHeatKilowattsWhenActive = SelfHeatKilowattsWhenActive;
                this.GeneratorWattageRating = GeneratorWattageRating;
                this.GeneratorBaseCapacity = GeneratorBaseCapacity;
                this.BaseDecor = BaseDecor;
                this.BaseDecorRadius = BaseDecorRadius;
                this.LocationRule = LocationRule;
                this.Rotations = Rotations;
                this.Floodable = Floodable;
                this.IsFoundation = IsFoundation;
                this.ConstructionMass = ConstructionMass;
                this.ThermalConductivity = ThermalConductivity;
            }
        }

        public const string IDApothecary = "Apothecary";
        public const string IDClothingFabricator = "Textile Loom";
        public const string IDCookingStation = "Electric Grill";
        public const string IDEggCracker = "Egg Cracker";
        public const string IDGlassForge = "Glass Forge";
        public const string IDGourmetCookingStation = "Gas Range";
        public const string IDMetalRefinery = "Metal Refinery";
        public const string IDMicrobeMusher = "Microbe Musher";
        public const string IDRockCrusher = "Rock Crusher";
        public const string IDSuitFabricator = "Exosuit Forge";
        public const string IDSupermaterialRefinery = "Molecular Forge";
        public const string IDOilRefinery = "Oil Refinery";
        public const string IDOilWellCap = "Oil Well";
        public const string IDAutomatedCompost = "Automated Compost";
        public const string IDIceCooledFan = "Ice Fan";
        public const string IDShearingStation = "Shearing Station";
        public const string IDRanchStation = "Grooming Station";

        public static CustomizeBuildingsState KleiSettings = new CustomizeBuildingsState()
        {
            //overwrite_all_with_Klei_default = false,
            BatterySmartKJ = 20000f,
            BatterySmartNoRunOff = false,
            BatteryLargeKJ = 40000f,
            LockerKG = 20000f,
            LockerSmartKG = 20000f,
            GasReservoirKG = 150f,
            LiquidReservoirKG = 5000f,
            RationBoxKG = 150f,
            FridgeKG = 100f,
            CritterFeederKG = 2000f,
            FishFeederKG = 200f,
            CanisterFillerKG = 10f,
            ConveyorLoaderKG = 1000f,
            ConveyorReceptacleKG = 100f,
            IUserControlledMax = 20000f,
            ElectrolizerMaxPressure = 1.8f,
            AirfilterDropsCanisters = false,
            NewRecipeRockCrusher = false,
            ReservoirNoGround = false,
            ReservoirManualDelivery = false,
            NoDupeValves = false,
            NoDupeSwitches = false,
            NoDupeToogleBuildings = false,
            NoDupeToogleDoors = false,
            ScannerInterferenceRadius = 15,
            ScannerWorstWarningTime = 1f,
            ScannerBestWarningTime = 200f,
            ScannerBestNetworkSize = 6,
            LadderCometInvincibility = false,
            SolarMaxPower = 380f,
            SolarEnergyMultiplier = 1f,
            SteamTurbineEnabled = false,
            SteamTurbineWattage = 850f,
            SteamTurbineSourceElement = "Steam",
            SteamTurbineOutputElement = "Water",
            SteamTurbinePumpRateKG = 2f,
            SteamTurbineMaxSelfHeat = 64f,
            SteamTurbineHeatTransferPercent = 0.1f,
            SteamTurbineMinActiveTemperature = 398.15f,
            SteamTurbineIdealTemperature = 473.15f,
            SteamTurbineOutputTemperature = 368.15f,
            SteamTurbineOverheatTemperature = 373.15f,
            PipeThroughputPercent = 1.0f,
            PipeLiquidMaxPressure = 10f,
            PipeGasMaxPressure = 1f,
            PipeLiquidPump = 10f,
            PipeGasPump = 0.5f,
            ConveyorRailPackageSize = 20f,
            ConveyorLoaderHasSlider = false,
            ConveyorReceptacleHasSlider = false,
            AutoSweeperCapacity = 1000f,
            AutoSweeperRange = 4,
            AutoSweeperSlider = false,
            RoboMinerWidth = 16,
            RoboMinerHeight = 9,
            RoboMinerOffset = 0,
            RoboMinerRegolithTurbo = false,
            RoboMinerDigThroughGlass = false,
            RoboMinerDigAnyTile = false,
            WireSmallWatts = 1000,
            WireRefinedWatts = 2000,
            WireHeavyWatts = 20000,
            WireRefinedHeavyWatts = 50000,
            TransitTubeAnywhere = false,
            TransitTubeUTurns = false,
            TransitTubeJoulesPerLaunch = 10000f,
            TransitTubeJouleCapacity = 40000f,
            NoDupeBuildingsGlobal = false,
            BuildingBaseSettingGlobalFlag = false
        };
    }
}