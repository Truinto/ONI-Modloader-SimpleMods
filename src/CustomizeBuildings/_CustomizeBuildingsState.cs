//#define LOCALE
using System.Collections.Generic;
using Newtonsoft.Json;
using Common;
using HarmonyLib;
using PeterHan.PLib.Options;
using System.IO;
using System;

namespace CustomizeBuildings
{
    [ConfigFile("CustomizeBuildings.json", true, true)]
    [RestartRequired]
    [ModInfo(null, collapse: true)]
    public class CustomizeBuildingsState
    {
        public int version { get; set; } = 46;

        #region $Reset Button
        [JsonIgnore]
        [Option("CustomizeBuildings.LOCSTRINGS.Header_Title", "CustomizeBuildings.LOCSTRINGS.Header_ToolTip", "", null)]
        public LocText Header => null;

        [Option("CustomizeBuildings.LOCSTRINGS.ResetToKleiDefault_Title", "CustomizeBuildings.LOCSTRINGS.ResetToKleiDefault_ToolTip")]
        [JsonIgnore]
        public System.Action<object> ResetToKleiDefault => delegate (object nix)
        {
            StateManager.State.BatterySmartKJ = 20000f;
            StateManager.State.BatterySmartNoRunOff = false;
            StateManager.State.BatteryLargeKJ = 40000f; 
            StateManager.State.SealInsulateStorages = false;
            StateManager.State.LockerKG = 20000f;
            StateManager.State.LockerSmartKG = 20000f;
            StateManager.State.GasReservoirKG = 150f;
            StateManager.State.LiquidReservoirKG = 5000f;
            StateManager.State.RationBoxKG = 150f;
            StateManager.State.FridgeKG = 100f;
            StateManager.State.CritterFeederKG = 2000f;
            StateManager.State.FishFeederKG = 200f;
            StateManager.State.CanisterFillerKG = 10f;
            StateManager.State.ConveyorLoaderKG = 1000f;
            StateManager.State.ConveyorReceptacleKG = 100f;
            StateManager.State.IUserControlledMax = 20000f;
            StateManager.State.ReservoirNoGround = false;
            StateManager.State.ReservoirManualDelivery = false;
            StateManager.State.RailgunMaxLaunch = 200f;
            StateManager.State.DrillConeKG = 1000f;
            StateManager.State.RocketPortGas = 1f;
            StateManager.State.RocketPortLiquid = 10f;
            StateManager.State.RocketPortSolid = 20f;
            StateManager.State.RadBattery = 1000f;
            StateManager.State.ElectrolizerMaxPressure = 1.8f;
            StateManager.State.AirfilterDropsCanisters = false;
            StateManager.State.NewRecipeRockCrusher = false;
            StateManager.State.AirConditionerAbsoluteOutput = false;
            StateManager.State.SpaceHeaterTargetTemperature = false;
            StateManager.State.AlgaeTerrariumPatch = false;
            StateManager.State.DoorSelfSealing = false;
            StateManager.State.NoDupeValves = false;
            StateManager.State.NoDupeSwitches = false;
            StateManager.State.NoDupeToogleBuildings = false;
            StateManager.State.NoDupeToogleDoors = false;
            StateManager.State.ScannerInterferenceRadius = 15;
            StateManager.State.ScannerWorstWarningTime = 1f;
            StateManager.State.ScannerBestWarningTime = 200f;
            StateManager.State.ScannerBestNetworkSize = 6;
            StateManager.State.TelescopeClearCellRadius = 5;
            StateManager.State.TelescopeAnalyzeRadius = 3;
            StateManager.State.LadderCometInvincibility = false;
            StateManager.State.SolarMaxPower = 380f;
            StateManager.State.SolarEnergyMultiplier = 1f;
            StateManager.State.SteamTurbineEnabled = false;
            StateManager.State.SteamTurbineWattage = 850f;
            StateManager.State.SteamTurbineSourceElement = "Steam";
            StateManager.State.SteamTurbineOutputElement = "Water";
            StateManager.State.SteamTurbinePumpRateKG = 2f;
            StateManager.State.SteamTurbineMaxSelfHeat = 64f;
            StateManager.State.SteamTurbineHeatTransferPercent = 0.1f;
            StateManager.State.SteamTurbineMinActiveTemperature = 398.15f;
            StateManager.State.SteamTurbineIdealTemperature = 473.15f;
            StateManager.State.SteamTurbineOutputTemperature = 368.15f;
            StateManager.State.SteamTurbineOverheatTemperature = 373.15f;
            StateManager.State.PipeThroughputPercent = 1.0f;
            StateManager.State.PipeLiquidMaxPressure = 10f;
            StateManager.State.PipeGasMaxPressure = 1f;
            StateManager.State.PipeValvePressureButtonShow = false;
            StateManager.State.PipeLiquidPump = 10f;
            StateManager.State.PipeGasPump = 0.5f;
            StateManager.State.PipeLiquidPumpMini = 1f;
            StateManager.State.PipeGasPumpMini = 0.05f;
            StateManager.State.ConveyorRailPackageSize = 20f;
            StateManager.State.ConveyorLoaderHasSlider = false;
            StateManager.State.ConveyorReceptacleHasSlider = false;
            StateManager.State.ConveyorLoaderAcceptLiquidsGas = false;
            StateManager.State.AutoSweeperCapacity = 1000f;
            StateManager.State.AutoSweeperRange = 4;
            StateManager.State.AutoSweeperSlider = false;
            StateManager.State.AutoSweeperPickupAnything = false;
            StateManager.State.RoboMinerWidth = 16;
            StateManager.State.RoboMinerHeight = 9;
            StateManager.State.RoboMinerOffset = 0;
            StateManager.State.RoboMinerSpeedMult = 1f;
            StateManager.State.RoboMinerRegolithTurbo = false;
            StateManager.State.RoboMinerDigThroughGlass = false;
            StateManager.State.RoboMinerDigAnyTile = false;
            StateManager.State.WireSmallWatts = 1000;
            StateManager.State.WireRefinedWatts = 2000;
            StateManager.State.WireHeavyWatts = 20000;
            StateManager.State.WireRefinedHeavyWatts = 50000;
            StateManager.State.TransitTubeAnywhere = false;
            StateManager.State.TransitTubeUTurns = false;
            StateManager.State.TransitTubeJoulesPerLaunch = 10000f;
            StateManager.State.TransitTubeJouleCapacity = 40000f;
            StateManager.State.NoDupeGlobal = false;
            StateManager.State.NoDupeApothecary = false;
            StateManager.State.NoDupeClothingFabricator = false;
            StateManager.State.NoDupeCookingStation = false;
            StateManager.State.NoDupeGourmetCookingStation = false;
            StateManager.State.NoDupeEggCracker = false;
            StateManager.State.NoDupeGlassForge = false;
            StateManager.State.NoDupeMetalRefinery = false;
            StateManager.State.NoDupeMicrobeMusher = false;
            StateManager.State.NoDupeRockCrusher = false;
            StateManager.State.NoDupeDiamondPress = false;
            StateManager.State.NoDupeSuitFabricator = false;
            StateManager.State.NoDupeSupermaterialRefinery = false;
            StateManager.State.NoDupeSludgePress = false;
            StateManager.State.NoDupeCompost = false;
            StateManager.State.NoDupeDesalinator = false;
            StateManager.State.NoDupeOilRefinery = false;
            StateManager.State.NoDupeOilWellCap = false;
            StateManager.State.NoDupeIceCooledFan = false;
            StateManager.State.NoDupeRanchStation = false;
            StateManager.State.NoDupeTelescope = false;
            StateManager.State.NoDupeAlgaeTerrarium = false;
            StateManager.State.NoDupePayloadOpener = false;
            StateManager.State.SkillStationEnabled = false;
            StateManager.State.SkillStationCostTime = 180f;
            StateManager.State.SkillStationCostReset = 0f;
            StateManager.State.SkillStationCostRemoveTrait = 10000f;
            StateManager.State.SkillStationCostAddTrait = 10000f;
            StateManager.State.SkillStationCostBadTrait = -10000f;
            StateManager.State.SkillStationCostAddAptitude = 10000f;
            StateManager.State.SkillStationCostAddAttribute = 5000f;
            StateManager.State.BuildingBaseSettingGlobalFlag = false;
            StateManager.State.BuildingAdvancedGlobalFlag = false;

            StateManager.State.TuningAtmosuitDecay = -0.1f;
            StateManager.State.TuningOxygenMaskDecay = -0.2f;
            StateManager.State.TuningAtmosuitAthletics = -6f;
            StateManager.State.TuningAtmosuitScalding = 1000f;
            StateManager.State.TuningAtmosuitInsulation = 50f;
            StateManager.State.TuningAtmosuitThermalConductivityBarrier = 0.2f;
            StateManager.State.TuningLeadsuitRadiationShielding = 0.66f;
            StateManager.State.TuningLeadsuitAthletics = -8;
            StateManager.State.TuningLeadsuitStrength = 10;
            StateManager.State.TuningLeadsuitInsulation = 50;
            StateManager.State.TuningLeadsuitThermalConductivityBarrier = 0.3f;
            StateManager.State.TuningMissionDurationScale = 1800f;
            StateManager.State.TuningMassPenaltyExponent = 3.2f;
            StateManager.State.TuningMassPenaltyDivisor = 300f;
            StateManager.State.TuningResearchEvergreen = 10;
            StateManager.State.TuningResearchBasic = 50;
            StateManager.State.TuningAnalysisDiscovered = 50;
            StateManager.State.TuningAnalysisComplete = 100;
            StateManager.State.TuningAnalysisDefaultCyclesPerDiscovery = 0.5f;
            StateManager.State.TuningThrustCostsLow = 3;
            StateManager.State.TuningThrustCostsMid = 5;
            StateManager.State.TuningThrustCostsHigh = 7;
            StateManager.State.TuningThrustCostsVeryHigh = 9;
            StateManager.State.TuningClusterFowPointsToReveal = 100f;
            StateManager.State.TuningClusterFowDefaultCyclesPerReveal = 0.5f;
            StateManager.State.TuningEngineEfficiencyWeak = 20f;
            StateManager.State.TuningEngineEfficiencyMedium = 40f;
            StateManager.State.TuningEngineEfficiencyStrong = 60f;
            StateManager.State.TuningEngineEfficiencyBooster = 30f;
            StateManager.State.TuningOxidizerEfficiencyVeryLow = 1f;
            StateManager.State.TuningOxidizerEfficiencyLow = 2f;
            StateManager.State.TuningOxidizerEfficiencyHigh = 4f;
            StateManager.State.TuningCargoContainerMassStaticMass = 1000f;
            StateManager.State.TuningCargoContainerMassPayloadMass = 1000f;
            StateManager.State.TuningBurdenInsignificant = 1;
            StateManager.State.TuningBurdenMinor = 2;
            StateManager.State.TuningBurdenMinorPlus = 3;
            StateManager.State.TuningBurdenModerate = 4;
            StateManager.State.TuningBurdenModeratePlus = 5;
            StateManager.State.TuningBurdenMajor = 6;
            StateManager.State.TuningBurdenMajorPlus = 7;
            StateManager.State.TuningBurdenMega = 9;
            StateManager.State.TuningEnginePowerEarlyWeak = 12;
            StateManager.State.TuningEnginePowerEarlyStrong = 20;
            StateManager.State.TuningEnginePowerMidVeryStrong = 42;
            StateManager.State.TuningEnginePowerMidStrong = 27;
            StateManager.State.TuningEnginePowerMidWeak = 6;
            StateManager.State.TuningEnginePowerLateStrong = 48;
            StateManager.State.TuningFuelCostPerDistanceVeryLow = 0.055555556f;
            StateManager.State.TuningFuelCostPerDistanceLow = 0.0625f;
            StateManager.State.TuningFuelCostPerDistanceMedium = 0.125f;
            StateManager.State.TuningFuelCostPerDistanceHigh = 0.166666672f;
            StateManager.State.TuningFuelCostPerDistanceVeryHigh = 0.25f;
            StateManager.State.TuningFuelCostPerDistanceGasLow = 0.027777778f;
            StateManager.State.TuningFuelCostPerDistanceGasHigh = 0.0416666679f;

            StateManager.State.BuildingAdvancedMachineMultiplier?.Clear();

            StateManager.TrySaveConfigurationState();

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };

        [Option("CustomizeBuildings.LOCSTRINGS.ResetToCustomDefault_Title", "CustomizeBuildings.LOCSTRINGS.ResetToCustomDefault_ToolTip")]
        [JsonIgnore]
        public System.Action<object> ResetToCustomDefault => delegate (object nix)
        {
            StateManager.TrySaveConfigurationState(new CustomizeBuildingsState());

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };

        #endregion

        #region Power
        [Option("CustomizeBuildings.LOCSTRINGS.BatterySmartKJ_Title", "CustomizeBuildings.LOCSTRINGS.BatterySmartKJ_ToolTip", "Power", "F0")]
        public float BatterySmartKJ { get; set; } = 200000f;
        [Option("CustomizeBuildings.LOCSTRINGS.BatterySmartNoRunOff_Title", "CustomizeBuildings.LOCSTRINGS.BatterySmartNoRunOff_ToolTip", "Power")]
        public bool BatterySmartNoRunOff { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.BatteryLargeKJ_Title", "CustomizeBuildings.LOCSTRINGS.BatteryLargeKJ_ToolTip", "Power", "F0")]
        public float BatteryLargeKJ { get; set; } = 400000f;

        [Option("CustomizeBuildings.LOCSTRINGS.SolarMaxPower_Title", "CustomizeBuildings.LOCSTRINGS.SolarMaxPower_ToolTip", "Power", "F0")]
        public float SolarMaxPower { get; set; } = 600f;
        [Option("CustomizeBuildings.LOCSTRINGS.SolarEnergyMultiplier_Title", "CustomizeBuildings.LOCSTRINGS.SolarEnergyMultiplier_ToolTip", "Power", "F2")]
        public float SolarEnergyMultiplier { get; set; } = 1f;
        #endregion

        #region Storage
        [Option("CustomizeBuildings.LOCSTRINGS.SealInsulateStorages_Title", "CustomizeBuildings.LOCSTRINGS.SealInsulateStorages_ToolTip", "Storage", null)]
        public bool SealInsulateStorages { get; set; } = false;
        [Option("CustomizeBuildings.LOCSTRINGS.LockerKG_Title", "CustomizeBuildings.LOCSTRINGS.LockerKG_ToolTip", "Storage", "F0")]
        public float LockerKG { get; set; } = 400000f;
        [Option("CustomizeBuildings.LOCSTRINGS.LockerSmartKG_Title", "CustomizeBuildings.LOCSTRINGS.LockerSmartKG_ToolTip", "Storage", "F0")]
        public float LockerSmartKG { get; set; } = 400000f;
        [Option("CustomizeBuildings.LOCSTRINGS.GasReservoirKG_Title", "CustomizeBuildings.LOCSTRINGS.GasReservoirKG_ToolTip", "Storage", "F0")]
        public float GasReservoirKG { get; set; } = 99000f;
        [Option("CustomizeBuildings.LOCSTRINGS.LiquidReservoirKG_Title", "CustomizeBuildings.LOCSTRINGS.LiquidReservoirKG_ToolTip", "Storage", "F0")]
        public float LiquidReservoirKG { get; set; } = 99000f;
        [Option("CustomizeBuildings.LOCSTRINGS.RationBoxKG_Title", "CustomizeBuildings.LOCSTRINGS.RationBoxKG_ToolTip", "Storage", "F0")]
        public float RationBoxKG { get; set; } = 400000f;
        [Option("CustomizeBuildings.LOCSTRINGS.FridgeKG_Title", "CustomizeBuildings.LOCSTRINGS.FridgeKG_ToolTip", "Storage", "F0")]
        public float FridgeKG { get; set; } = 400000f;
        [Option("CustomizeBuildings.LOCSTRINGS.CritterFeederKG_Title", "CustomizeBuildings.LOCSTRINGS.CritterFeederKG_ToolTip", "Storage", "F0")]
        public float CritterFeederKG { get; set; } = 400000f;
        [Option("CustomizeBuildings.LOCSTRINGS.FishFeederKG_Title", "CustomizeBuildings.LOCSTRINGS.FishFeederKG_ToolTip", "Storage", "F0")]
        public float FishFeederKG { get; set; } = 400000f;
        [Option("CustomizeBuildings.LOCSTRINGS.CanisterFillerKG_Title", "CustomizeBuildings.LOCSTRINGS.CanisterFillerKG_ToolTip", "Storage", "F0")]
        public float CanisterFillerKG { get; set; } = 400000f;
        [Option("CustomizeBuildings.LOCSTRINGS.ConveyorLoaderKG_Title", "CustomizeBuildings.LOCSTRINGS.ConveyorLoaderKG_ToolTip", "Storage", "F0")]
        public float ConveyorLoaderKG { get; set; } = 400000f;
        [Option("CustomizeBuildings.LOCSTRINGS.ConveyorReceptacleKG_Title", "CustomizeBuildings.LOCSTRINGS.ConveyorReceptacleKG_ToolTip", "Storage", "F0")]
        public float ConveyorReceptacleKG { get; set; } = 400000f;
        [Option("CustomizeBuildings.LOCSTRINGS.IUserControlledMax_Title", "CustomizeBuildings.LOCSTRINGS.IUserControlledMax_ToolTip", "Storage", "F0")]
        public float IUserControlledMax { get; set; } = 400000f;

        [Option("CustomizeBuildings.LOCSTRINGS.ReservoirNoGround_Title", "CustomizeBuildings.LOCSTRINGS.ReservoirNoGround_ToolTip", "Storage")]
        public bool ReservoirNoGround { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.ReservoirManualDelivery_Title", "CustomizeBuildings.LOCSTRINGS.ReservoirManualDelivery_ToolTip", "Storage")]
        public bool ReservoirManualDelivery { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.RailgunMaxLaunch_Title", "CustomizeBuildings.LOCSTRINGS.RailgunMaxLaunch_ToolTip", "Storage", "F0")]
        public float RailgunMaxLaunch { get; set; } = 200f;
        [Option("CustomizeBuildings.LOCSTRINGS.DrillConeKG_Title", "CustomizeBuildings.LOCSTRINGS.DrillConeKG_ToolTip", "Storage", "F0")]
        public float DrillConeKG { get; set; } = 1000f;
        [Option("CustomizeBuildings.LOCSTRINGS.RocketPortGas_Title", "CustomizeBuildings.LOCSTRINGS.RocketPortGas_ToolTip", "Storage", "F0")]
        public float RocketPortGas { get; set; } = 1f;
        [Option("CustomizeBuildings.LOCSTRINGS.RocketPortLiquid_Title", "CustomizeBuildings.LOCSTRINGS.RocketPortLiquid_ToolTip", "Storage", "F0")]
        public float RocketPortLiquid { get; set; } = 10f;
        [Option("CustomizeBuildings.LOCSTRINGS.RocketPortSolid_Title", "CustomizeBuildings.LOCSTRINGS.RocketPortSolid_ToolTip", "Storage", "F0")]
        public float RocketPortSolid { get; set; } = 20f;

        [Option("CustomizeBuildings.LOCSTRINGS.RadBattery_Title", "CustomizeBuildings.LOCSTRINGS.RadBattery_ToolTip", "Storage", "F0")]
        public float RadBattery { get; set; } = 1000f;
        #endregion

        #region Miscellaneous
        [Option("CustomizeBuildings.LOCSTRINGS.ElectrolizerMaxPressure_Title", "CustomizeBuildings.LOCSTRINGS.ElectrolizerMaxPressure_ToolTip", "Miscellaneous", "F1")]
        public float ElectrolizerMaxPressure { get; set; } = 5f;
        [Option("CustomizeBuildings.LOCSTRINGS.AirfilterDropsCanisters_Title", "CustomizeBuildings.LOCSTRINGS.AirfilterDropsCanisters_ToolTip", "Miscellaneous")]
        public bool AirfilterDropsCanisters { get; set; } = true;

        [Option("CustomizeBuildings.LOCSTRINGS.NewRecipeRockCrusher_Title", "CustomizeBuildings.LOCSTRINGS.NewRecipeRockCrusher_ToolTip", "Miscellaneous")]
        public bool NewRecipeRockCrusher { get; set; } = true;

        [Option("CustomizeBuildings.LOCSTRINGS.AirConditionerAbsoluteOutput_Title", "CustomizeBuildings.LOCSTRINGS.AirConditionerAbsoluteOutput_ToolTip", "Miscellaneous")]
        public bool AirConditionerAbsoluteOutput { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.AirConditionerAbsolutePowerFactor_Title", "CustomizeBuildings.LOCSTRINGS.AirConditionerAbsolutePowerFactor_ToolTip", "Miscellaneous")]
        public float AirConditionerAbsolutePowerFactor { get; set; } = 0.6f;
        [Option("CustomizeBuildings.LOCSTRINGS.SpaceHeaterTargetTemperature_Title", "CustomizeBuildings.LOCSTRINGS.SpaceHeaterTargetTemperature_ToolTip", "Miscellaneous")]
        public bool SpaceHeaterTargetTemperature { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.AlgaeTerrariumPatch_Title", "CustomizeBuildings.LOCSTRINGS.AlgaeTerrariumPatch_ToolTip", "Miscellaneous")]
        public bool AlgaeTerrariumPatch { get; set; } = false;
        [Option("CustomizeBuildings.LOCSTRINGS.DoorSelfSealing_Title", "CustomizeBuildings.LOCSTRINGS.DoorSelfSealing_ToolTip", "Miscellaneous")]
        public bool DoorSelfSealing { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.MaterialIgnoreInsufficientMaterial_Title", "CustomizeBuildings.LOCSTRINGS.MaterialIgnoreInsufficientMaterial_ToolTip", "Miscellaneous")]
        public bool MaterialIgnoreInsufficientMaterial { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.MaterialAutoSelect_Title", "CustomizeBuildings.LOCSTRINGS.MaterialAutoSelect_ToolTip", "Miscellaneous")]
        public bool MaterialAutoSelect { get; set; } = true;
        #endregion

        #region Switches
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeValves_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeValves_ToolTip", "Switches")]
        public bool NoDupeValves { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeSwitches_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeSwitches_ToolTip", "Switches")]
        public bool NoDupeSwitches { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeToogleBuildings_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeToogleBuildings_ToolTip", "Switches")]
        public bool NoDupeToogleBuildings { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeToogleDoors_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeToogleDoors_ToolTip", "Switches")]
        public bool NoDupeToogleDoors { get; set; } = true;
        #endregion

        #region Space Scanner
        [Option("CustomizeBuildings.LOCSTRINGS.ScannerInterferenceRadius_Title", "CustomizeBuildings.LOCSTRINGS.ScannerInterferenceRadius_ToolTip", "Space Scanner", "F1")]
        public int ScannerInterferenceRadius { get; set; } = 5;
        [Option("CustomizeBuildings.LOCSTRINGS.ScannerWorstWarningTime_Title", "CustomizeBuildings.LOCSTRINGS.ScannerWorstWarningTime_ToolTip", "Space Scanner", "F1")]
        public float ScannerWorstWarningTime { get; set; } = 30f;
        [Option("CustomizeBuildings.LOCSTRINGS.ScannerBestWarningTime_Title", "CustomizeBuildings.LOCSTRINGS.ScannerBestWarningTime_ToolTip", "Space Scanner", "F1")]
        public float ScannerBestWarningTime { get; set; } = 60f;
        [Option("CustomizeBuildings.LOCSTRINGS.ScannerBestNetworkSize_Title", "CustomizeBuildings.LOCSTRINGS.ScannerBestNetworkSize_ToolTip", "Space Scanner")]
        public int ScannerBestNetworkSize { get; set; } = 2;
        [Option("CustomizeBuildings.LOCSTRINGS.LadderCometInvincibility_Title", "CustomizeBuildings.LOCSTRINGS.LadderCometInvincibility_ToolTip", "Space Scanner")]
        public bool LadderCometInvincibility { get; set; } = true;

        [Option("CustomizeBuildings.LOCSTRINGS.TelescopeClearCellRadius_Title", "CustomizeBuildings.LOCSTRINGS.TelescopeClearCellRadius_ToolTip", "Space Scanner")]
        public int TelescopeClearCellRadius { get; set; } = 5;
        [Option("CustomizeBuildings.LOCSTRINGS.TelescopeAnalyzeRadius_Title", "CustomizeBuildings.LOCSTRINGS.TelescopeAnalyzeRadius_ToolTip", "Space Scanner")]
        public int TelescopeAnalyzeRadius { get; set; } = 3;
        #endregion

        #region Steam Turbine
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineEnabled_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineEnabled_ToolTip", "Steam Turbine")]
        public bool SteamTurbineEnabled { get; set; } = false;
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineWattage_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineWattage_ToolTip", "Steam Turbine", "F0")]
        public float SteamTurbineWattage { get; set; } = 850f;
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineSourceElement_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineSourceElement_ToolTip", "Steam Turbine")]
        public string SteamTurbineSourceElement { get; set; } = "Steam";
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineOutputElement_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineOutputElement_ToolTip", "Steam Turbine")]
        public string SteamTurbineOutputElement { get; set; } = "Water";
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbinePumpRateKG_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbinePumpRateKG_ToolTip", "Steam Turbine", "F2")]
        public float SteamTurbinePumpRateKG { get; set; } = 2f;
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineMaxSelfHeat_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineMaxSelfHeat_ToolTip", "Steam Turbine", "F2")]
        public float SteamTurbineMaxSelfHeat { get; set; } = 64f;
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineHeatTransferPercent_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineHeatTransferPercent_ToolTip", "Steam Turbine", "P2")]
        [Limit(0, 1)]
        public float SteamTurbineHeatTransferPercent { get; set; } = 0.1f;
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineMinActiveTemperature_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineMinActiveTemperature_ToolTip", "Steam Turbine", "F2")]
        public float SteamTurbineMinActiveTemperature { get; set; } = 398.15f;
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineIdealTemperature_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineIdealTemperature_ToolTip", "Steam Turbine", "F2")]
        public float SteamTurbineIdealTemperature { get; set; } = 473.15f;
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineOutputTemperature_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineOutputTemperature_ToolTip", "Steam Turbine", "F2")]
        public float SteamTurbineOutputTemperature { get; set; } = 368.15f;
        [Option("CustomizeBuildings.LOCSTRINGS.SteamTurbineOverheatTemperature_Title", "CustomizeBuildings.LOCSTRINGS.SteamTurbineOverheatTemperature_ToolTip", "Steam Turbine", "F2")]
        public float SteamTurbineOverheatTemperature { get; set; } = 373.15f;
        #endregion

        #region Pipes
        [Option("CustomizeBuildings.LOCSTRINGS.PipeThroughputPercent_Title", "CustomizeBuildings.LOCSTRINGS.PipeThroughputPercent_ToolTip", "Pipes", "P2")]
        [Limit(0, 1)]
        public float PipeThroughputPercent { get; set; } = 1.0f;
        [Option("CustomizeBuildings.LOCSTRINGS.PipeLiquidMaxPressure_Title", "CustomizeBuildings.LOCSTRINGS.PipeLiquidMaxPressure_ToolTip", "Pipes", "F2")]
        public float PipeLiquidMaxPressure { get; set; } = 10f;
        [Option("CustomizeBuildings.LOCSTRINGS.PipeGasMaxPressure_Title", "CustomizeBuildings.LOCSTRINGS.PipeGasMaxPressure_ToolTip", "Pipes", "F2")]
        public float PipeGasMaxPressure { get; set; } = 1.0f;
        [Option("CustomizeBuildings.LOCSTRINGS.PipeValvePressureButtonShow_Title", "CustomizeBuildings.LOCSTRINGS.PipeValvePressureButtonShow_ToolTip", "Pipes")]
        public bool PipeValvePressureButtonShow { get; set; } = false;
        [Option("CustomizeBuildings.LOCSTRINGS.PipeLiquidPump_Title", "CustomizeBuildings.LOCSTRINGS.PipeLiquidPump_ToolTip", "Pipes", "F2")]
        public float PipeLiquidPump { get; set; } = 10.0f;
        [Option("CustomizeBuildings.LOCSTRINGS.PipeGasPump_Title", "CustomizeBuildings.LOCSTRINGS.PipeGasPump_ToolTip", "Pipes", "F2")]
        public float PipeGasPump { get; set; } = 0.5f;
        [Option("CustomizeBuildings.LOCSTRINGS.PipeLiquidPumpMini_Title", "CustomizeBuildings.LOCSTRINGS.PipeLiquidPumpMini_ToolTip", "Pipes", "F2")]
        public float PipeLiquidPumpMini { get; set; } = 1.0f;
        [Option("CustomizeBuildings.LOCSTRINGS.PipeGasPumpMini_Title", "CustomizeBuildings.LOCSTRINGS.PipeGasPumpMini_ToolTip", "Pipes", "F2")]
        public float PipeGasPumpMini { get; set; } = 0.05f;
        [Option("CustomizeBuildings.LOCSTRINGS.ConveyorRailPackageSize_Title", "CustomizeBuildings.LOCSTRINGS.ConveyorRailPackageSize_ToolTip", "Pipes", "F2")]
        public float ConveyorRailPackageSize { get; set; } = 20f;
        [Option("CustomizeBuildings.LOCSTRINGS.ConveyorLoaderHasSlider_Title", "CustomizeBuildings.LOCSTRINGS.ConveyorLoaderHasSlider_ToolTip", "Pipes")]
        public bool ConveyorLoaderHasSlider { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.ConveyorReceptacleHasSlider_Title", "CustomizeBuildings.LOCSTRINGS.ConveyorReceptacleHasSlider_ToolTip", "Pipes")]
        public bool ConveyorReceptacleHasSlider { get; set; } = false;

        [Option("CustomizeBuildings.LOCSTRINGS.ConveyorLoaderAcceptLiquidsGas_Title", "CustomizeBuildings.LOCSTRINGS.ConveyorLoaderAcceptLiquidsGas_ToolTip", "Pipes")]
        public bool ConveyorLoaderAcceptLiquidsGas { get; set; } = false;
        #endregion

        #region Auto Sweeper
        [Option("CustomizeBuildings.LOCSTRINGS.AutoSweeperCapacity_Title", "CustomizeBuildings.LOCSTRINGS.AutoSweeperCapacity_ToolTip", "Auto Sweeper", "F0")]
        public float AutoSweeperCapacity { get; set; } = 1000f;
        [Option("CustomizeBuildings.LOCSTRINGS.AutoSweeperRange_Title", "CustomizeBuildings.LOCSTRINGS.AutoSweeperRange_ToolTip", "Auto Sweeper")]
        public int AutoSweeperRange { get; set; } = 4;
        [Option("CustomizeBuildings.LOCSTRINGS.AutoSweeperSlider_Title", "CustomizeBuildings.LOCSTRINGS.AutoSweeperSlider_ToolTip", "Auto Sweeper")]
        public bool AutoSweeperSlider { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.AutoSweeperPickupAnything_Title", "CustomizeBuildings.LOCSTRINGS.AutoSweeperPickupAnything_ToolTip", "Auto Sweeper")]
        public bool AutoSweeperPickupAnything { get; set; } = false;
        #endregion

        #region Robo Miner
        [Option("CustomizeBuildings.LOCSTRINGS.RoboMinerWidth_Title", "CustomizeBuildings.LOCSTRINGS.RoboMinerWidth_ToolTip", "Robo Miner")]
        public int RoboMinerWidth { get; set; } = 16;
        [Option("CustomizeBuildings.LOCSTRINGS.RoboMinerHeight_Title", "CustomizeBuildings.LOCSTRINGS.RoboMinerHeight_ToolTip", "Robo Miner")]
        public int RoboMinerHeight { get; set; } = 9;
        [Option("CustomizeBuildings.LOCSTRINGS.RoboMinerOffset_Title", "CustomizeBuildings.LOCSTRINGS.RoboMinerOffset_ToolTip", "Robo Miner")]
        public int RoboMinerOffset { get; set; } = 0;
        [Option("CustomizeBuildings.LOCSTRINGS.RoboMinerSpeedMult_Title", "CustomizeBuildings.LOCSTRINGS.RoboMinerSpeedMult_ToolTip", "Robo Miner", "F0")]
        public float RoboMinerSpeedMult { get; set; } = 1f;
        [Option("CustomizeBuildings.LOCSTRINGS.RoboMinerRegolithTurbo_Title", "CustomizeBuildings.LOCSTRINGS.RoboMinerRegolithTurbo_ToolTip", "Robo Miner")]
        public bool RoboMinerRegolithTurbo { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.RoboMinerDigThroughGlass_Title", "CustomizeBuildings.LOCSTRINGS.RoboMinerDigThroughGlass_ToolTip", "Robo Miner")]
        public bool RoboMinerDigThroughGlass { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.RoboMinerDigAnyTile_Title", "CustomizeBuildings.LOCSTRINGS.RoboMinerDigAnyTile_ToolTip", "Robo Miner")]
        public bool RoboMinerDigAnyTile { get; set; } = true;
        #endregion

        #region Power Cable
        [Option("CustomizeBuildings.LOCSTRINGS.WireSmallWatts_Title", "CustomizeBuildings.LOCSTRINGS.WireSmallWatts_ToolTip", "Power Cable")]
        public int WireSmallWatts { get; set; } = 1000;
        [Option("CustomizeBuildings.LOCSTRINGS.WireRefinedWatts_Title", "CustomizeBuildings.LOCSTRINGS.WireRefinedWatts_ToolTip", "Power Cable")]
        public int WireRefinedWatts { get; set; } = 2000;
        [Option("CustomizeBuildings.LOCSTRINGS.WireHeavyWatts_Title", "CustomizeBuildings.LOCSTRINGS.WireHeavyWatts_ToolTip", "Power Cable")]
        public int WireHeavyWatts { get; set; } = 20000;
        [Option("CustomizeBuildings.LOCSTRINGS.WireRefinedHeavyWatts_Title", "CustomizeBuildings.LOCSTRINGS.WireRefinedHeavyWatts_ToolTip", "Power Cable")]
        public int WireRefinedHeavyWatts { get; set; } = 50000;
        #endregion

        #region Transit Tube
        [Option("CustomizeBuildings.LOCSTRINGS.TransitTubeAnywhere_Title", "CustomizeBuildings.LOCSTRINGS.TransitTubeAnywhere_ToolTip", "Transit Tubes")]
        public bool TransitTubeAnywhere { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.TransitTubeUTurns_Title", "CustomizeBuildings.LOCSTRINGS.TransitTubeUTurns_ToolTip", "Transit Tubes")]
        public bool TransitTubeUTurns { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.TransitTubeJoulesPerLaunch_Title", "CustomizeBuildings.LOCSTRINGS.TransitTubeJoulesPerLaunch_ToolTip", "Transit Tubes", "F0")]
        public float TransitTubeJoulesPerLaunch { get; set; } = 10000f;
        [Option("CustomizeBuildings.LOCSTRINGS.TransitTubeJouleCapacity_Title", "CustomizeBuildings.LOCSTRINGS.TransitTubeJouleCapacity_ToolTip", "Transit Tubes", "F0")]
        public float TransitTubeJouleCapacity { get; set; } = 40000f;
        //public float TransitTubeSpeed { get; set; } = 18f;
        #endregion

        #region No Dupe
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeGlobal_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeGlobal_ToolTip", "No Dupe")]
        public bool NoDupeGlobal { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeApothecary_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeApothecary_ToolTip", "No Dupe")]
        public bool NoDupeApothecary { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeClothingFabricator_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeClothingFabricator_ToolTip", "No Dupe")]
        public bool NoDupeClothingFabricator { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeCookingStation_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeCookingStation_ToolTip", "No Dupe")]
        public bool NoDupeCookingStation { get; set; } = false;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeGourmetCookingStation_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeGourmetCookingStation_ToolTip", "No Dupe")]
        public bool NoDupeGourmetCookingStation { get; set; } = false;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeEggCracker_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeEggCracker_ToolTip", "No Dupe")]
        public bool NoDupeEggCracker { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeGlassForge_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeGlassForge_ToolTip", "No Dupe")]
        public bool NoDupeGlassForge { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeMetalRefinery_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeMetalRefinery_ToolTip", "No Dupe")]
        public bool NoDupeMetalRefinery { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeMicrobeMusher_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeMicrobeMusher_ToolTip", "No Dupe")]
        public bool NoDupeMicrobeMusher { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeRockCrusher_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeRockCrusher_ToolTip", "No Dupe")]
        public bool NoDupeRockCrusher { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeDiamondPress_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeDiamondPress_ToolTip", "No Dupe")]
        public bool NoDupeDiamondPress { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeSuitFabricator_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeSuitFabricator_ToolTip", "No Dupe")]
        public bool NoDupeSuitFabricator { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeSupermaterialRefinery_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeSupermaterialRefinery_ToolTip", "No Dupe")]
        public bool NoDupeSupermaterialRefinery { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeSludgePress_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeSludgePress_ToolTip", "No Dupe")]
        public bool NoDupeSludgePress { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeCompost_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeCompost_ToolTip", "No Dupe")]
        public bool NoDupeCompost { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeDesalinator_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeDesalinator_ToolTip", "No Dupe")]
        public bool NoDupeDesalinator { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeOilRefinery_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeOilRefinery_ToolTip", "No Dupe")]
        public bool NoDupeOilRefinery { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeOilWellCap_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeOilWellCap_ToolTip", "No Dupe")]
        public bool NoDupeOilWellCap { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeIceCooledFan_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeIceCooledFan_ToolTip", "No Dupe")]
        public bool NoDupeIceCooledFan { get; set; } = false; // TODO: revisit
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeRanchStation_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeRanchStation_ToolTip", "No Dupe")]
        public bool NoDupeRanchStation { get; set; } = false; // TODO: revisit
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeTelescope_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeTelescope_ToolTip", "No Dupe")]
        public bool NoDupeTelescope { get; set; } = false;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupeAlgaeTerrarium_Title", "CustomizeBuildings.LOCSTRINGS.NoDupeAlgaeTerrarium_ToolTip", "No Dupe")]
        public bool NoDupeAlgaeTerrarium { get; set; } = false;
        [Option("CustomizeBuildings.LOCSTRINGS.NoDupePayloadOpener_Title", "CustomizeBuildings.LOCSTRINGS.NoDupePayloadOpener_ToolTip", "No Dupe", null)]
        public bool NoDupePayloadOpener { get; set; } = false;

        //public bool NoDupeShearingStation { get; set; } = true;
        #endregion

        #region Skill Station
        [Option("CustomizeBuildings.LOCSTRINGS.SkillStationEnabled_Title", "CustomizeBuildings.LOCSTRINGS.SkillStationEnabled_ToolTip", "Skill Station")]
        public bool SkillStationEnabled { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.SkillStationCostTime_Title", "CustomizeBuildings.LOCSTRINGS.SkillStationCostTime_ToolTip", "Skill Station", "F0")]
        public float SkillStationCostTime { get; set; } = 20f;
        [Option("CustomizeBuildings.LOCSTRINGS.SkillStationCostReset_Title", "CustomizeBuildings.LOCSTRINGS.SkillStationCostReset_ToolTip", "Skill Station", "F0")]
        public float SkillStationCostReset { get; set; } = 0f;
        [Option("CustomizeBuildings.LOCSTRINGS.SkillStationCostRemoveTrait_Title", "CustomizeBuildings.LOCSTRINGS.SkillStationCostRemoveTrait_ToolTip", "Skill Station", "F0")]
        public float SkillStationCostRemoveTrait { get; set; } = 10000f;
        [Option("CustomizeBuildings.LOCSTRINGS.SkillStationCostAddTrait_Title", "CustomizeBuildings.LOCSTRINGS.SkillStationCostAddTrait_ToolTip", "Skill Station", "F0")]
        public float SkillStationCostAddTrait { get; set; } = 10000f;
        [Option("CustomizeBuildings.LOCSTRINGS.SkillStationCostBadTrait_Title", "CustomizeBuildings.LOCSTRINGS.SkillStationCostBadTrait_ToolTip", "Skill Station", "F0")]
        public float SkillStationCostBadTrait { get; set; } = -10000f;
        [Option("CustomizeBuildings.LOCSTRINGS.SkillStationCostAddAptitude_Title", "CustomizeBuildings.LOCSTRINGS.SkillStationCostAddAptitude_ToolTip", "Skill Station", "F0")]
        public float SkillStationCostAddAptitude { get; set; } = 10000f;
        [Option("CustomizeBuildings.LOCSTRINGS.SkillStationCostAddAttribute_Title", "CustomizeBuildings.LOCSTRINGS.SkillStationCostAddAttribute_ToolTip", "Skill Station", "F0")]
        public float SkillStationCostAddAttribute { get; set; } = 5000f;
        #endregion

        #region Tuning
        [Option("CustomizeBuildings.LOCSTRINGS.TuningGlobal_Title", "CustomizeBuildings.LOCSTRINGS.TuningGlobal_ToolTip", "Tuning")]
        public bool TuningGlobal { get; set; } = true;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitDecay_Title", "CustomizeBuildings.LOCSTRINGS.TuningAtmosuitDecay_ToolTip", "Tuning", "F2")]
        [Limit(-1, 0)]
        public float TuningAtmosuitDecay { get; set; } = 0f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningOxygenMaskDecay_Title", "CustomizeBuildings.LOCSTRINGS.TuningOxygenMaskDecay_ToolTip", "Tuning", "F2")]
        [Limit(-1, 0)]
        public float TuningOxygenMaskDecay { get; set; } = 0f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitAthletics_Title", "CustomizeBuildings.LOCSTRINGS.TuningAtmosuitAthletics_ToolTip", "Tuning", "F0")]
        public float TuningAtmosuitAthletics { get; set; } = -6f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitScalding_Title", "CustomizeBuildings.LOCSTRINGS.TuningAtmosuitScalding_ToolTip", "Tuning", "F2")]
        public float TuningAtmosuitScalding { get; set; } = 1000f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitInsulation_Title", "CustomizeBuildings.LOCSTRINGS.TuningAtmosuitInsulation_ToolTip", "Tuning", "F2")]
        public float TuningAtmosuitInsulation { get; set; } = 50f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitThermalConductivityBarrier_Title", "CustomizeBuildings.LOCSTRINGS.TuningAtmosuitThermalConductivityBarrier_ToolTip", "Tuning", "F2")]
        public float TuningAtmosuitThermalConductivityBarrier { get; set; } = 0.2f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitRadiationShielding_Title", "CustomizeBuildings.LOCSTRINGS.TuningLeadsuitRadiationShielding_ToolTip", "Tuning", "F2")]
        public float TuningLeadsuitRadiationShielding { get; set; } = 0.66f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitAthletics_Title", "CustomizeBuildings.LOCSTRINGS.TuningLeadsuitAthletics_ToolTip", "Tuning")]
        public int TuningLeadsuitAthletics { get; set; } = -8;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitStrength_Title", "CustomizeBuildings.LOCSTRINGS.TuningLeadsuitStrength_ToolTip", "Tuning")]
        public int TuningLeadsuitStrength { get; set; } = 10;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitInsulation_Title", "CustomizeBuildings.LOCSTRINGS.TuningLeadsuitInsulation_ToolTip", "Tuning")]
        public int TuningLeadsuitInsulation { get; set; } = 50;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitThermalConductivityBarrier_Title", "CustomizeBuildings.LOCSTRINGS.TuningLeadsuitThermalConductivityBarrier_ToolTip", "Tuning", "F2")]
        public float TuningLeadsuitThermalConductivityBarrier { get; set; } = 0.3f;

        [Option("CustomizeBuildings.LOCSTRINGS.TuningMissionDurationScale_Title", "CustomizeBuildings.LOCSTRINGS.TuningMissionDurationScale_ToolTip", "Tuning", "F2")]
        public float TuningMissionDurationScale { get; set; } = 1800f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningMassPenaltyExponent_Title", "CustomizeBuildings.LOCSTRINGS.TuningMassPenaltyExponent_ToolTip", "Tuning", "F2")]
        public float TuningMassPenaltyExponent { get; set; } = 3.2f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningMassPenaltyDivisor_Title", "CustomizeBuildings.LOCSTRINGS.TuningMassPenaltyDivisor_ToolTip", "Tuning", "F2")]
        public float TuningMassPenaltyDivisor { get; set; } = 300f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningResearchEvergreen_Title", "CustomizeBuildings.LOCSTRINGS.TuningResearchEvergreen_ToolTip", "Tuning")]
        public int TuningResearchEvergreen { get; set; } = 10;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningResearchBasic_Title", "CustomizeBuildings.LOCSTRINGS.TuningResearchBasic_ToolTip", "Tuning")]
        public int TuningResearchBasic { get; set; } = 50;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningAnalysisDiscovered_Title", "CustomizeBuildings.LOCSTRINGS.TuningAnalysisDiscovered_ToolTip", "Tuning")]
        public int TuningAnalysisDiscovered { get; set; } = 50;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningAnalysisComplete_Title", "CustomizeBuildings.LOCSTRINGS.TuningAnalysisComplete_ToolTip", "Tuning")]
        public int TuningAnalysisComplete { get; set; } = 100;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningAnalysisDefaultCyclesPerDiscovery_Title", "CustomizeBuildings.LOCSTRINGS.TuningAnalysisDefaultCyclesPerDiscovery_ToolTip", "Tuning", "F2")]
        public float TuningAnalysisDefaultCyclesPerDiscovery { get; set; } = 0.5f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsLow_Title", "CustomizeBuildings.LOCSTRINGS.TuningThrustCostsLow_ToolTip", "Tuning")]
        public int TuningThrustCostsLow { get; set; } = 3;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsMid_Title", "CustomizeBuildings.LOCSTRINGS.TuningThrustCostsMid_ToolTip", "Tuning")]
        public int TuningThrustCostsMid { get; set; } = 5;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsHigh_Title", "CustomizeBuildings.LOCSTRINGS.TuningThrustCostsHigh_ToolTip", "Tuning")]
        public int TuningThrustCostsHigh { get; set; } = 7;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsVeryHigh_Title", "CustomizeBuildings.LOCSTRINGS.TuningThrustCostsVeryHigh_ToolTip", "Tuning")]
        public int TuningThrustCostsVeryHigh { get; set; } = 9;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningClusterFowPointsToReveal_Title", "CustomizeBuildings.LOCSTRINGS.TuningClusterFowPointsToReveal_ToolTip", "Tuning", "F2")]
        public float TuningClusterFowPointsToReveal { get; set; } = 100f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningClusterFowDefaultCyclesPerReveal_Title", "CustomizeBuildings.LOCSTRINGS.TuningClusterFowDefaultCyclesPerReveal_ToolTip", "Tuning", "F2")]
        public float TuningClusterFowDefaultCyclesPerReveal { get; set; } = 0.5f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyWeak_Title", "CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyWeak_ToolTip", "Tuning", "F2")]
        public float TuningEngineEfficiencyWeak { get; set; } = 20f;   //Co2; Steam
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyMedium_Title", "CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyMedium_ToolTip", "Tuning", "F2")]
        public float TuningEngineEfficiencyMedium { get; set; } = 40f; //Kerosene
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyStrong_Title", "CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyStrong_ToolTip", "Tuning", "F2")]
        public float TuningEngineEfficiencyStrong { get; set; } = 60f; //Hydrogen; Sugar
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyBooster_Title", "CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyBooster_ToolTip", "Tuning", "F2")]
        public float TuningEngineEfficiencyBooster { get; set; } = 30f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyVeryLow_Title", "CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyVeryLow_ToolTip", "Tuning", "F2")]
        public float TuningOxidizerEfficiencyVeryLow { get; set; } = 1f;  //Fertilizer
        [Option("CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyLow_Title", "CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyLow_ToolTip", "Tuning", "F2")]
        public float TuningOxidizerEfficiencyLow { get; set; } = 2f;   //Oxyrock
        [Option("CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyHigh_Title", "CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyHigh_ToolTip", "Tuning", "F2")]
        public float TuningOxidizerEfficiencyHigh { get; set; } = 4f;  //Liquidoxygen
        [Option("CustomizeBuildings.LOCSTRINGS.TuningCargoContainerMassStaticMass_Title", "CustomizeBuildings.LOCSTRINGS.TuningCargoContainerMassStaticMass_ToolTip", "Tuning", "F2")]
        public float TuningCargoContainerMassStaticMass { get; set; } = 1000f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningCargoContainerMassPayloadMass_Title", "CustomizeBuildings.LOCSTRINGS.TuningCargoContainerMassPayloadMass_ToolTip", "Tuning", "F2")]
        public float TuningCargoContainerMassPayloadMass { get; set; } = 1000f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningBurdenInsignificant_Title", "CustomizeBuildings.LOCSTRINGS.TuningBurdenInsignificant_ToolTip", "Tuning")]
        public int TuningBurdenInsignificant { get; set; } = 1;   //Unconstructed
        [Option("CustomizeBuildings.LOCSTRINGS.TuningBurdenMinor_Title", "CustomizeBuildings.LOCSTRINGS.TuningBurdenMinor_ToolTip", "Tuning")]
        public int TuningBurdenMinor { get; set; } = 2;   //Gascargo; Nosecone; Oxidizer-Small
        [Option("CustomizeBuildings.LOCSTRINGS.TuningBurdenMinorPlus_Title", "CustomizeBuildings.LOCSTRINGS.TuningBurdenMinorPlus_ToolTip", "Tuning")]
        public int TuningBurdenMinorPlus { get; set; } = 3;  //Co2; Habitat-Small; Liquidcargo-Small; Scanner; Sugar
        [Option("CustomizeBuildings.LOCSTRINGS.TuningBurdenModerate_Title", "CustomizeBuildings.LOCSTRINGS.TuningBurdenModerate_ToolTip", "Tuning")]
        public int TuningBurdenModerate { get; set; } = 4;    //Gascargo; Orbitalcargo; Pioneer; Scout; Solidcargo-Small; Steam
        [Option("CustomizeBuildings.LOCSTRINGS.TuningBurdenModeratePlus_Title", "CustomizeBuildings.LOCSTRINGS.TuningBurdenModeratePlus_ToolTip", "Tuning")]
        public int TuningBurdenModeratePlus { get; set; } = 5;   //Kerosene-Small; Liquidcargo; Liquidfuel; Oxidizer
        [Option("CustomizeBuildings.LOCSTRINGS.TuningBurdenMajor_Title", "CustomizeBuildings.LOCSTRINGS.TuningBurdenMajor_ToolTip", "Tuning")]
        public int TuningBurdenMajor { get; set; } = 6;   //Habitat; Kerosene; Solidcargo
        [Option("CustomizeBuildings.LOCSTRINGS.TuningBurdenMajorPlus_Title", "CustomizeBuildings.LOCSTRINGS.TuningBurdenMajorPlus_ToolTip", "Tuning")]
        public int TuningBurdenMajorPlus { get; set; } = 7;  //Hydrogen
        [Option("CustomizeBuildings.LOCSTRINGS.TuningBurdenMega_Title", "CustomizeBuildings.LOCSTRINGS.TuningBurdenMega_ToolTip", "Tuning")]
        public int TuningBurdenMega { get; set; } = 9;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerEarlyWeak_Title", "CustomizeBuildings.LOCSTRINGS.TuningEnginePowerEarlyWeak_ToolTip", "Tuning")]
        public int TuningEnginePowerEarlyWeak { get; set; } = 12;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerEarlyStrong_Title", "CustomizeBuildings.LOCSTRINGS.TuningEnginePowerEarlyStrong_ToolTip", "Tuning")]
        public int TuningEnginePowerEarlyStrong { get; set; } = 20;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidVeryStrong_Title", "CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidVeryStrong_ToolTip", "Tuning")]
        public int TuningEnginePowerMidVeryStrong { get; set; } = 42;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidStrong_Title", "CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidStrong_ToolTip", "Tuning")]
        public int TuningEnginePowerMidStrong { get; set; } = 27;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidWeak_Title", "CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidWeak_ToolTip", "Tuning")]
        public int TuningEnginePowerMidWeak { get; set; } = 6;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerLateStrong_Title", "CustomizeBuildings.LOCSTRINGS.TuningEnginePowerLateStrong_ToolTip", "Tuning")]
        public int TuningEnginePowerLateStrong { get; set; } = 48;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceVeryLow_Title", "CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceVeryLow_ToolTip", "Tuning", "F2")]
        public float TuningFuelCostPerDistanceVeryLow { get; set; } = 0.055555556f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceLow_Title", "CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceLow_ToolTip", "Tuning", "F2")]
        public float TuningFuelCostPerDistanceLow { get; set; } = 0.0625f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceMedium_Title", "CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceMedium_ToolTip", "Tuning", "F2")]
        public float TuningFuelCostPerDistanceMedium { get; set; } = 0.125f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceHigh_Title", "CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceHigh_ToolTip", "Tuning", "F2")]
        public float TuningFuelCostPerDistanceHigh { get; set; } = 0.166666672f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceVeryHigh_Title", "CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceVeryHigh_ToolTip", "Tuning", "F2")]
        public float TuningFuelCostPerDistanceVeryHigh { get; set; } = 0.25f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceGasLow_Title", "CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceGasLow_ToolTip", "Tuning", "F2")]
        public float TuningFuelCostPerDistanceGasLow { get; set; } = 0.027777778f;
        [Option("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceGasHigh_Title", "CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceGasHigh_ToolTip", "Tuning", "F2")]
        public float TuningFuelCostPerDistanceGasHigh { get; set; } = 0.0416666679f;
        #endregion

        #region Advanced
        [Option("CustomizeBuildings.LOCSTRINGS.BuildingBaseSettingGlobalFlag_Title", "CustomizeBuildings.LOCSTRINGS.BuildingBaseSettingGlobalFlag_ToolTip", "Miscellaneous")]
        public bool BuildingBaseSettingGlobalFlag { get; set; } = true;
        public Dictionary<string, BuildingStruct> BuildingBaseSettings { get; set; } = new Dictionary<string, BuildingStruct> {
            { WireHighWattageConfig.ID, new BuildingStruct(LocationRule: BuildLocationRule.Anywhere) },
            { WireRefinedHighWattageConfig.ID, new BuildingStruct(LocationRule: BuildLocationRule.Anywhere, BaseDecor: -5f, BaseDecorRadius: 1f) },
            { TravelTubeConfig.ID, new BuildingStruct(LocationRule: BuildLocationRule.Anywhere) },
            { BedConfig.ID, new BuildingStruct(Rotations: PermittedRotations.FlipH) },
            { LuxuryBedConfig.ID, new BuildingStruct(Rotations: PermittedRotations.FlipH) },
            { StorageLockerConfig.ID, new BuildingStruct(LocationRule: BuildLocationRule.Anywhere) },
            //{ DoorConfig.ID, new BuildingStruct(IsFoundation: true) },
        };

        [Option("CustomizeBuildings.LOCSTRINGS.BuildingAdvancedGlobalFlag_Title", "CustomizeBuildings.LOCSTRINGS.BuildingAdvancedGlobalFlag_ToolTip", "Miscellaneous")]
        public bool BuildingAdvancedGlobalFlag { get; set; } = false;
        public Dictionary<string, float> BuildingAdvancedMachineMultiplier { get; set; } = new Dictionary<string, float> {
            { "EthanolDistillery", 4f }
        };
        public List<BuildingAdv> BuildingAdvancedMaterial { get; set; } = new List<BuildingAdv>() {
            new BuildingAdv(TravelTubeConfig.ID, null, true, "Glass Ice"),
            new BuildingAdv(TravelTubeWallBridgeConfig.ID, null, true, "Glass Ice"),
        };
        public Dictionary<string, ElementConverterContainer> BuildingAdvancedOutputTemp { get; set; } = new Dictionary<string, ElementConverterContainer>() {
            { ElectrolyzerConfig.ID, new ElementConverterContainer().ModeTemperature().Temperature(1f) },
        };

        public ElementConverterContainer AlgaeTerrarium { get; set; } = new ElementConverterContainer()
            .Input("Dirt", 0.030000001f).Input("Water", 0.3f)
            .Output("Oxygen", 0.040000003f).Output("Algae", 0.29033336f).Store(false, true);

        public Dictionary<string, Dictionary<string, Dictionary<string, object>>> AdvancedSettings { get; set; } = null;
        //    = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>() {
        //        { "LiquidHeater", new Dictionary<string, Dictionary<string, object>> { { "SpaceHeater", new Dictionary<string, object> { { "targetTemperature", 1000f} }  } } }
        //}; // PrefabID, Component, Field, Value
        #endregion

        public static Config.Manager<CustomizeBuildingsState> StateManager = new Config.Manager<CustomizeBuildingsState>(Config.PathHelper.CreatePath("CustomizeBuildings"), true, OnUpdate, null);

        public static bool OnUpdate(CustomizeBuildingsState state)
        {
            if (state.version < 29)
                state.AutoSweeperPickupAnything = false;
            if (state.version < 36)
            {
                string modpath = Path.Combine(Config.PathHelper.ModsDirectory, "config", "CustomizeBuildings.json");
                if (File.Exists(modpath))
                    File.Delete(modpath);
                modpath = Path.Combine(Config.PathHelper.ModsDirectory, "config", "CustomizeBuildingsMerged");
                if (Directory.Exists(modpath))
                    Directory.Delete(modpath, true);
            }
            if (state.version < 44 && state.DrillConeKG < 1000f)
                state.DrillConeKG = 1000f;
            //if (state.version < 45)
            //{
            //    state.Capacities.Clear();
            //    state.Capacities.Add(StorageLockerConfig.ID, state.LockerKG);
            //    state.Capacities.Add(StorageLockerSmartConfig.ID, state.LockerSmartKG);
            //    state.Capacities.Add(GasReservoirConfig.ID, state.GasReservoirKG);
            //    state.Capacities.Add(LiquidReservoirConfig.ID, state.LiquidReservoirKG);
            //    state.Capacities.Add(RationBoxConfig.ID, state.RationBoxKG);
            //    state.Capacities.Add(RefrigeratorConfig.ID, state.FridgeKG);
            //    state.Capacities.Add(CreatureFeederConfig.ID, state.CritterFeederKG);
            //    state.Capacities.Add(FishFeederConfig.ID, state.FishFeederKG);
            //    state.Capacities.Add(GasBottlerConfig.ID, state.CanisterFillerKG);
            //    state.Capacities.Add(SolidConduitInboxConfig.ID, state.ConveyorLoaderKG);
            //    state.Capacities.Add(SolidConduitOutboxConfig.ID, state.ConveyorReceptacleKG);
            //    state.Capacities.Add(NoseconeHarvestConfig.ID, state.DrillConeKG);
            //    state.Capacities.Add(ModularLaunchpadPortGasConfig.ID, state.RocketPortGas);
            //    state.Capacities.Add(ModularLaunchpadPortLiquidConfig.ID, state.RocketPortLiquid);
            //    state.Capacities.Add(ModularLaunchpadPortSolidConfig.ID, state.RocketPortSolid);
            //}

            return true;
        }
    }

    public class CustomStrings
    {
        public static void LoadStrings()
        {
            #region 
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.version", "version");

            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.Header_Title", "_______________________________________________________________________________________");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.Header_ToolTip", "");

            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ResetToKleiDefault_Title", "Reset To Klei Default");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ResetToKleiDefault_ToolTip", "This will discard all changes and set all options to 'off'.");

            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ResetToCustomDefault_Title", "Reset To Mod Preset");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ResetToCustomDefault_ToolTip", "This will discard all changes and enable most options.");
            #endregion
            #region Advanced
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.BuildingBaseSettingGlobalFlag", "BuildingBaseSettingGlobalFlag");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BuildingBaseSettingGlobalFlag_Title", "Base Setting Global Flag");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BuildingBaseSettingGlobalFlag_ToolTip", "If false, will keep disable all basic manual changes.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.BuildingBaseSettings", "BuildingBaseSettings");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.BuildingAdvancedGlobalFlag", "BuildingAdvancedGlobalFlag");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BuildingAdvancedGlobalFlag_Title", "Advanced Global Flag");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BuildingAdvancedGlobalFlag_ToolTip", "If false, will keep disable all advanced manual changes.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.BuildingAdvancedMachineMultiplier", "BuildingAdvancedMachineMultiplier");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.BuildingAdvancedMaterial", "BuildingAdvancedMaterial");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.BuildingAdvancedOutputTemp", "BuildingAdvancedOutputTemp");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AlgaeTerrarium", "AlgaeTerrarium");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AdvancedSettings", "AdvancedSettings");
            #endregion
            #region Auto Sweeper
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AutoSweeperCapacity", "AutoSweeperCapacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AutoSweeperCapacity_Title", "Auto Sweeper Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AutoSweeperCapacity_ToolTip", "Storage capacity of auto sweepers.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AutoSweeperRange", "AutoSweeperRange");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AutoSweeperRange_Title", "Auto Sweeper Range");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AutoSweeperRange_ToolTip", "Range of Auto Sweeper. Very large numbers will cause lag.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AutoSweeperSlider", "AutoSweeperSlider");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AutoSweeperSlider_Title", "Auto Sweeper Slider");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AutoSweeperSlider_ToolTip", "If true, will show a slider to reduce the Sweeper's range.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AutoSweeperPickupAnything", "AutoSweeperPickupAnything");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AutoSweeperPickupAnything_Title", "Auto Sweeper Pickup Anything");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AutoSweeperPickupAnything_ToolTip", "If true, Auto Sweeper can move/store anything that dupes can.");
            #endregion
            #region Miscellaneous
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ElectrolizerMaxPressure", "ElectrolizerMaxPressure");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ElectrolizerMaxPressure_Title", "Electrolizer Max Pressure");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ElectrolizerMaxPressure_ToolTip", "Pressure the electrolizer will stop producing oxygen/hydrogen.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AirfilterDropsCanisters", "AirfilterDropsCanisters");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AirfilterDropsCanisters_Title", "Airfilter Canisters");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AirfilterDropsCanisters_ToolTip", "On deconstruction, air-filters (as well as all other buildings) drop gas canisters instead of venting stored gases.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NewRecipeRockCrusher", "NewRecipeRockCrusher");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NewRecipeRockCrusher_Title", "New Recipe: Rock-Crusher");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NewRecipeRockCrusher_ToolTip", "Adds regolith to sand recipe.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AirConditionerAbsoluteOutput", "AirConditionerAbsoluteOutput");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AirConditionerAbsoluteOutput_Title", "Air Conditioner Absolute Output");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AirConditionerAbsoluteOutput_ToolTip", "If true, Air Conditioner and Aquatuner get a new target temperature slider. If the target temperature is 0 Kelvin, it will retain -14°C behaviour. Otherwise output temperature will match exactly target temperature. Also adds a kJoules slider, which limits how fast the building heats up. Energy consumption will scale on cooling/heating factor.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AirConditionerAbsolutePowerFactor", "AirConditionerAbsolutePowerFactor");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AirConditionerAbsolutePowerFactor_Title", "Air Conditioner Absolute Power Factor");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AirConditionerAbsolutePowerFactor_ToolTip", "% of MaxDPU the Air Conditioner takes full power. Default: 0.1");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SpaceHeaterTargetTemperature", "SpaceHeaterTargetTemperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SpaceHeaterTargetTemperature_Title", "Space Heater Target Temperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SpaceHeaterTargetTemperature_ToolTip", "If true, Space Heater gets a temperature setting to control target temperature.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.AlgaeTerrariumPatch", "AlgaeTerrariumPatch");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AlgaeTerrariumPatch_Title", "Algae Terrarium Grower");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.AlgaeTerrariumPatch_ToolTip", "If true, will modify the Algae Terrarium. By default will convert dirt into algae.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.DoorSelfSealing", "DoorSelfSealing");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.DoorSelfSealing_Title", "Self Sealing Door");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.DoorSelfSealing_ToolTip", "If true, doors will block gas flow while set to automatic.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.MaterialIgnoreInsufficientMaterial", "MaterialIgnoreInsufficientMaterial");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.MaterialIgnoreInsufficientMaterial_Title", "Ignore Insufficient Material");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.MaterialIgnoreInsufficientMaterial_ToolTip", "If true, will allow placing buildings while having insufficient building material.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.MaterialAutoSelect", "MaterialAutoSelect");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.MaterialAutoSelect_Title", "Material No Auto Select");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.MaterialAutoSelect_ToolTip", "If true, will keep selected building material, even if stored amount is insufficient.");
            #endregion
            #region No Dupe
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeGlobal", "NoDupeGlobal");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeGlobal_Title", "<b><color=red>!!! Unlock No Dupe !!!</color></b>");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeGlobal_ToolTip", "If false, will disable all settings in this category.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeApothecary", "NoDupeApothecary");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeApothecary_Title", "No Dupe Apothecary");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeApothecary_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeClothingFabricator", "NoDupeClothingFabricator");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeClothingFabricator_Title", "No Dupe ClothingFabricator");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeClothingFabricator_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeCookingStation", "NoDupeCookingStation");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeCookingStation_Title", "No Dupe Cooking Station");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeCookingStation_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeGourmetCookingStation", "NoDupeGourmetCookingStation");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeGourmetCookingStation_Title", "No Dupe Gourmet Cooking Station");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeGourmetCookingStation_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeEggCracker", "NoDupeEggCracker");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeEggCracker_Title", "No Dupe Egg Cracker");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeEggCracker_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeGlassForge", "NoDupeGlassForge");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeGlassForge_Title", "No Dupe Glass Forge");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeGlassForge_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeMetalRefinery", "NoDupeMetalRefinery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeMetalRefinery_Title", "No Dupe Metal Refinery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeMetalRefinery_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeMicrobeMusher", "NoDupeMicrobeMusher");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeMicrobeMusher_Title", "No Dupe Microbe Musher");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeMicrobeMusher_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeRockCrusher", "NoDupeRockCrusher");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeRockCrusher_Title", "No Dupe Rock Crusher");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeRockCrusher_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeDiamondPress", "NoDupeDiamondPress");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeDiamondPress_Title", "No Dupe Diamond Press");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeDiamondPress_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeSuitFabricator", "NoDupeSuitFabricator");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeSuitFabricator_Title", "No Dupe Suit Fabricator");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeSuitFabricator_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeSupermaterialRefinery", "NoDupeSupermaterialRefinery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeSupermaterialRefinery_Title", "No Dupe Supermaterial Refinery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeSupermaterialRefinery_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeSludgePress", "NoDupeSludgePress");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeSludgePress_Title", "No Dupe Sludge Press");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeSludgePress_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeCompost", "NoDupeCompost");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeCompost_Title", "No Dupe Compost");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeCompost_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeDesalinator", "NoDupeDesalinator");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeDesalinator_Title", "No Dupe Desalinator");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeDesalinator_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeOilRefinery", "NoDupeOilRefinery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeOilRefinery_Title", "No Dupe Oil Refinery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeOilRefinery_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeOilWellCap", "NoDupeOilWellCap");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeOilWellCap_Title", "No Dupe Oil Well Cap");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeOilWellCap_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeIceCooledFan", "NoDupeIceCooledFan");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeIceCooledFan_Title", "No Dupe Ice Cooled Fan");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeIceCooledFan_ToolTip", "If true, will replace the dupe requirement with 120W power inlet. Ice consumption is different.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeRanchStation", "NoDupeRanchStation");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeRanchStation_Title", "No Dupe Ranch Station");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeRanchStation_ToolTip", "If true, will buff the standard duration of grooming to 100 cycles.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeTelescope", "NoDupeTelescope");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeTelescope_Title", "No Dupe Telescope");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeTelescope_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeAlgaeTerrarium", "NoDupeAlgaeTerrarium");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeAlgaeTerrarium_Title", "No Dupe Algae Terrarium");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeAlgaeTerrarium_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupePayloadOpener", "NoDupePayloadOpener");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupePayloadOpener_Title", "No Dupe Payload Opener");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupePayloadOpener_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeShearingStation", "NoDupeShearingStation");
            #endregion
            #region Pipes
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.PipeThroughputPercent", "PipeThroughputPercent");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeThroughputPercent_Title", "Pipe Throughput Percent");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeThroughputPercent_ToolTip", "Percent of pipe usage for Aquatuner and Air Conditioner.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.PipeLiquidMaxPressure", "PipeLiquidMaxPressure");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeLiquidMaxPressure_Title", "Pipe Liquid Max Pressure");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeLiquidMaxPressure_ToolTip", "Liquid pipe pressure.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.PipeGasMaxPressure", "PipeGasMaxPressure");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeGasMaxPressure_Title", "Pipe Gas Max Pressure");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeGasMaxPressure_ToolTip", "Gas pipe pressure.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.PipeValvePressureButtonShow", "PipeValvePressureButtonShow");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeValvePressureButtonShow_Title", "Pipe Valve Pressure Button Show");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeValvePressureButtonShow_ToolTip", "If true, will display a 'limit' button in the building window. While limited, output tile cannot exceed flow rate.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.PipeLiquidPump", "PipeLiquidPump");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeLiquidPump_Title", "Pipe Liquid Pump");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeLiquidPump_ToolTip", "Amount of liquid pumped at once from Liquid Pumps.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.PipeGasPump", "PipeGasPump");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeGasPump_Title", "Pipe Gas Pump");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeGasPump_ToolTip", "Amount of gas pumped at once from Gas Pumps.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.PipeLiquidPumpMini", "PipeLiquidPumpMini");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeLiquidPumpMini_Title", "Pipe Liquid Pump Mini");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeLiquidPumpMini_ToolTip", "Amount of liquid pumped at once from Mini Liquid Pumps.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.PipeGasPumpMini", "PipeGasPumpMini");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeGasPumpMini_Title", "Pipe Gas Pump Mini");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.PipeGasPumpMini_ToolTip", "Amount of gas pumped at once from Mini Gas Pumps.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ConveyorRailPackageSize", "ConveyorRailPackageSize");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorRailPackageSize_Title", "Conveyor Rail Package Size");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorRailPackageSize_ToolTip", "Maximum size of packages on Conveyor Rails.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ConveyorLoaderHasSlider", "ConveyorLoaderHasSlider");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorLoaderHasSlider_Title", "Conveyor Loader Has Slider");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorLoaderHasSlider_ToolTip", "If true, will show the capacity slider for Conveyor Loaders.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ConveyorReceptacleHasSlider", "ConveyorReceptacleHasSlider");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorReceptacleHasSlider_Title", "Conveyor Receptacle Has Slider");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorReceptacleHasSlider_ToolTip", "If true, will show the capacity slider for Conveyor Receptacles.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ConveyorLoaderAcceptLiquidsGas", "ConveyorLoaderAcceptLiquidsGas");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorLoaderAcceptLiquidsGas_Title", "Conveyor Loader Accepts Liquids Gases");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorLoaderAcceptLiquidsGas_ToolTip", "If true, add liquids and gases to the filter list.");
            #endregion
            #region Power
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.BatterySmartKJ", "BatterySmartKJ");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BatterySmartKJ_Title", "Smart Battery KJ");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BatterySmartKJ_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.BatterySmartNoRunOff", "BatterySmartNoRunOff");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BatterySmartNoRunOff_Title", "Smart Battery remove runoff");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BatterySmartNoRunOff_ToolTip", "If true, smart battery won't lose charge over time.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.BatteryLargeKJ", "BatteryLargeKJ");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BatteryLargeKJ_Title", "Large Battery KJ");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.BatteryLargeKJ_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SolarMaxPower", "SolarMaxPower");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SolarMaxPower_Title", "Solar Max Power");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SolarMaxPower_ToolTip", "Limit of how much light can be converted. Does not increase energy per lumen.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SolarEnergyMultiplier", "SolarEnergyMultiplier");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SolarEnergyMultiplier_Title", "Solar Energy Multiplier");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SolarEnergyMultiplier_ToolTip", "Multiplies power generation. Limited up to 'Solar Max Power'.");
            #endregion
            #region Power Cable
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.WireSmallWatts", "WireSmallWatts");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.WireSmallWatts_Title", "Wire Small Watts");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.WireSmallWatts_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.WireRefinedWatts", "WireRefinedWatts");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.WireRefinedWatts_Title", "Wire Refined Watts");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.WireRefinedWatts_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.WireHeavyWatts", "WireHeavyWatts");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.WireHeavyWatts_Title", "Wire Heavy Watts");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.WireHeavyWatts_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.WireRefinedHeavyWatts", "WireRefinedHeavyWatts");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.WireRefinedHeavyWatts_Title", "Wire Refined Heavy Watts");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.WireRefinedHeavyWatts_ToolTip", "");
            #endregion
            #region Robo Miner
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RoboMinerWidth", "RoboMinerWidth");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerWidth_Title", "Robo Miner Width");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerWidth_ToolTip", "Mining width.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RoboMinerHeight", "RoboMinerHeight");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerHeight_Title", "Robo Miner Height");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerHeight_ToolTip", "Mining height.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RoboMinerOffset", "RoboMinerOffset");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerOffset_Title", "Robo Miner Offset");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerOffset_ToolTip", "Offset at which the Robo Miner works (negative numbers allowed).");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RoboMinerSpeedMult", "RoboMinerSpeedMult");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerSpeedMult_Title", "Robo Miner Speed Multiplier");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerSpeedMult_ToolTip", "Determines the speed of the Robo Miner. 2 = twice as fast; 0.5 = half as fast");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RoboMinerRegolithTurbo", "RoboMinerRegolithTurbo");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerRegolithTurbo_Title", "Robo Miner Regolith Turbo");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerRegolithTurbo_ToolTip", "If true, Regolith will be mined 6 times faster.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RoboMinerDigThroughGlass", "RoboMinerDigThroughGlass");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerDigThroughGlass_Title", "Robo Miner Dig Through Glass");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerDigThroughGlass_ToolTip", "Robo Miner dig through glass tiles, but only from glass (not diamonds)");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RoboMinerDigAnyTile", "RoboMinerDigAnyTile");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerDigAnyTile_Title", "Robo Miner Dig Any Tile");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RoboMinerDigAnyTile_ToolTip", "Robo Miner can dig any material.");
            #endregion
            #region Skill Station
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SkillStationEnabled", "SkillStationEnabled");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationEnabled_Title", "Skill Station Enabled");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationEnabled_ToolTip", "If true, will repurpose the Skill Scrubber to change traits/interests/attributes of dupes. If false, fully disables this feature.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SkillStationCostTime", "SkillStationCostTime");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostTime_Title", "Skill Station Time");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostTime_ToolTip", "Time Skill Scrubber needs.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SkillStationCostReset", "SkillStationCostReset");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostReset_Title", "Skill Station Reset");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostReset_ToolTip", "Exp cost to reset Skill Points (select \"None\")");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SkillStationCostRemoveTrait", "SkillStationCostRemoveTrait");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostRemoveTrait_Title", "Skill Station Remove Trait");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostRemoveTrait_ToolTip", "Exp cost to remove any Trait.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SkillStationCostAddTrait", "SkillStationCostAddTrait");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostAddTrait_Title", "Skill Station Add Trait");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostAddTrait_ToolTip", "Exp cost to add a new good Trait.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SkillStationCostBadTrait", "SkillStationCostBadTrait");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostBadTrait_Title", "Skill Station Bad Trait");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostBadTrait_ToolTip", "Exp cost to add a new bad Trait (usually grants Exp).");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SkillStationCostAddAptitude", "SkillStationCostAddAptitude");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostAddAptitude_Title", "Skill Station Add Aptitude");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostAddAptitude_ToolTip", "Exp cost to add a new interest.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SkillStationCostAddAttribute", "SkillStationCostAddAttribute");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostAddAttribute_Title", "Skill Station Add Attribute");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SkillStationCostAddAttribute_ToolTip", "Exp cost to improve an attribute by 1.");
            #endregion
            #region Space Scanner
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ScannerInterferenceRadius", "ScannerInterferenceRadius");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ScannerInterferenceRadius_Title", "Scanner Interference Radius");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ScannerInterferenceRadius_ToolTip", "Radius looking for heavy machinery and free sky tiles.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ScannerWorstWarningTime", "ScannerWorstWarningTime");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ScannerWorstWarningTime_Title", "Scanner Worst Warning Time");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ScannerWorstWarningTime_ToolTip", "Worst time before a network will detect incoming meteors.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ScannerBestWarningTime", "ScannerBestWarningTime");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ScannerBestWarningTime_Title", "Scanner Best Warning Time");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ScannerBestWarningTime_ToolTip", "Best time before a network will detect incoming meteors.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ScannerBestNetworkSize", "ScannerBestNetworkSize");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ScannerBestNetworkSize_Title", "Scanner Best Network Size");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ScannerBestNetworkSize_ToolTip", "Amount of scanners needed for best warning time.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.LadderCometInvincibility", "LadderCometInvincibility");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.LadderCometInvincibility_Title", "Ladder Comet Invincibility");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.LadderCometInvincibility_ToolTip", "Ladders don't get harmed by comets. Does not apply to plastic ladders. Ladders can still melt from heat.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TelescopeClearCellRadius", "TelescopeClearCellRadius");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TelescopeClearCellRadius_Title", "Telescope Clear Cell Radius");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TelescopeClearCellRadius_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TelescopeAnalyzeRadius", "TelescopeAnalyzeRadius");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TelescopeAnalyzeRadius_Title", "Telescope Analyze Radius");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TelescopeAnalyzeRadius_ToolTip", "");
            #endregion
            #region Steam Turbine
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineEnabled", "SteamTurbineEnabled");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineEnabled_Title", "Steam Turbine Enabled");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineEnabled_ToolTip", "Whenever or not to change the steam turbine at all. If false, ignores other settings.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineWattage", "SteamTurbineWattage");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineWattage_Title", "Steam Turbine Wattage");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineWattage_ToolTip", "Wattage produced by Steam Turbine under optimal conditions.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineSourceElement", "SteamTurbineSourceElement");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineSourceElement_Title", "Steam Turbine Source Element");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineSourceElement_ToolTip", "Steam Turbine input element.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineOutputElement", "SteamTurbineOutputElement");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineOutputElement_Title", "Steam Turbine Output Element");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineOutputElement_ToolTip", "Steam Turbine output element.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbinePumpRateKG", "SteamTurbinePumpRateKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbinePumpRateKG_Title", "Steam Turbine Pump-Rate");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbinePumpRateKG_ToolTip", "Steam Turbine amount of gas absorbed at once.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineMaxSelfHeat", "SteamTurbineMaxSelfHeat");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineMaxSelfHeat_Title", "Steam Turbine Max Self-Heat");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineMaxSelfHeat_ToolTip", "Steam Turbine maximum amount of heat tranfered from gas to machine.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineHeatTransferPercent", "SteamTurbineHeatTransferPercent");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineHeatTransferPercent_Title", "Steam Turbine Heat Transfer Percent");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineHeatTransferPercent_ToolTip", "Steam Turbine percent of heat transfered. Limited up to MAX.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineMinActiveTemperature", "SteamTurbineMinActiveTemperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineMinActiveTemperature_Title", "Steam Turbine Min Active Temperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineMinActiveTemperature_ToolTip", "Minimal temperature at which Steam Turbine turns on.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineIdealTemperature", "SteamTurbineIdealTemperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineIdealTemperature_Title", "Steam Turbine Ideal Temperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineIdealTemperature_ToolTip", "Best temperature at which Steam Turbine produces full energy.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineOutputTemperature", "SteamTurbineOutputTemperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineOutputTemperature_Title", "Steam Turbine Output Temperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineOutputTemperature_ToolTip", "Steam Turbine pipe output temperature.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SteamTurbineOverheatTemperature", "SteamTurbineOverheatTemperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineOverheatTemperature_Title", "Steam Turbine Overheat Temperature");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SteamTurbineOverheatTemperature_ToolTip", "Temperature where the Steam Turbine shuts off.");
            #endregion
            #region Storage
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.SealInsulateStorages", "SealInsulateStorages");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SealInsulateStorages_Title", "Seal and insulate storages");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.SealInsulateStorages_ToolTip", "Makes material in storages stop sublimating and transfering heat.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.LockerKG", "LockerKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.LockerKG_Title", "Storage Locker Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.LockerKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.LockerSmartKG", "LockerSmartKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.LockerSmartKG_Title", "Smart Storage Locker Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.LockerSmartKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.GasReservoirKG", "GasReservoirKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.GasReservoirKG_Title", "Gas Reservoir Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.GasReservoirKG_ToolTip", "Capped at 100 tones. Anything higher will get deleted by the game.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.LiquidReservoirKG", "LiquidReservoirKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.LiquidReservoirKG_Title", "Liquid Reservoir Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.LiquidReservoirKG_ToolTip", "Capped at 100 tones. Anything higher will get deleted by the game.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RationBoxKG", "RationBoxKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RationBoxKG_Title", "Ration Box Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RationBoxKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.FridgeKG", "FridgeKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.FridgeKG_Title", "Fridge Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.FridgeKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.CritterFeederKG", "CritterFeederKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.CritterFeederKG_Title", "Critter Feeder Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.CritterFeederKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.FishFeederKG", "FishFeederKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.FishFeederKG_Title", "Fish Feeder Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.FishFeederKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.CanisterFillerKG", "CanisterFillerKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.CanisterFillerKG_Title", "Canister Filler Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.CanisterFillerKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ConveyorLoaderKG", "ConveyorLoaderKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorLoaderKG_Title", "Conveyor Loader Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorLoaderKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ConveyorReceptacleKG", "ConveyorReceptacleKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorReceptacleKG_Title", "Conveyor Receptacle Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ConveyorReceptacleKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.IUserControlledMax", "IUserControlledMax");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.IUserControlledMax_Title", "Conveyor Receptacle Controller");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.IUserControlledMax_ToolTip", "Set same as in Conveyor Receptacle Capacity.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ReservoirNoGround", "ReservoirNoGround");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ReservoirNoGround_Title", "Reservoir No Ground");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ReservoirNoGround_ToolTip", "Reservoirs can be placed in the air.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.ReservoirManualDelivery", "ReservoirManualDelivery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ReservoirManualDelivery_Title", "Reservoir Manual Delivery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.ReservoirManualDelivery_ToolTip", "Dupes may store material in reservoirs.\n- dupes will deliver selected liquids/gases until the capacity is at the slider amount\n- liquid/gas pipes can still deliver any element and will ignore the slider limit\n- activating, then deactivating an element checkbox drops it on the floor, for easy removal of rogue elements");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RailgunMaxLaunch", "RailgunMaxLaunch");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RailgunMaxLaunch_Title", "Railgun Max Launch");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RailgunMaxLaunch_ToolTip", "How much material can be send per launch. Storage is at least twice at much.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.DrillConeKG", "DrillConeKG");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.DrillConeKG_Title", "Drillcone Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.DrillConeKG_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RocketPortGas", "RocketPortGas");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RocketPortGas_Title", "Rocket Port Gas Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RocketPortGas_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RocketPortLiquid", "RocketPortLiquid");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RocketPortLiquid_Title", "Rocket Port Liquid Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RocketPortLiquid_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RocketPortSolid", "RocketPortSolid");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RocketPortSolid_Title", "Rocket Port Solid Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RocketPortSolid_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.RadBattery", "RadBattery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RadBattery_Title", "Radbolt Chamber Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.RadBattery_ToolTip", "");
            #endregion
            #region Switches
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeValves", "NoDupeValves");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeValves_Title", "No Dupe Valves");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeValves_ToolTip", "Valves are set instantly without dupe interaction.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeSwitches", "NoDupeSwitches");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeSwitches_Title", "No Dupe Switches");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeSwitches_ToolTip", "Switches are set instantly without dupe interaction.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeToogleBuildings", "NoDupeToogleBuildings");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeToogleBuildings_Title", "No Dupe Toogle Buildings");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeToogleBuildings_ToolTip", "Buildings are disabled instantly without dupe interaction.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.NoDupeToogleDoors", "NoDupeToogleDoors");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeToogleDoors_Title", "No Dupe Toogle Doors");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.NoDupeToogleDoors_ToolTip", "Doors open/close instantly without dupe interaction.");
            #endregion
            #region Transit Tube
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TransitTubeAnywhere", "TransitTubeAnywhere");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TransitTubeAnywhere_Title", "Transit Tube Anywhere");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TransitTubeAnywhere_ToolTip", "Transit Tubes can be placed in the background.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TransitTubeUTurns", "TransitTubeUTurns");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TransitTubeUTurns_Title", "Transit Tube U-Turns");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TransitTubeUTurns_ToolTip", "Removes all transit tube layout restrictions.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TransitTubeJoulesPerLaunch", "TransitTubeJoulesPerLaunch");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TransitTubeJoulesPerLaunch_Title", "Transit Tube Joules Per Launch");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TransitTubeJoulesPerLaunch_ToolTip", "Cost per launch, normally 10000 joules.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TransitTubeJouleCapacity", "TransitTubeJouleCapacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TransitTubeJouleCapacity_Title", "Transit Tube Joule Capacity");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TransitTubeJouleCapacity_ToolTip", "Capacity, normally 40000 joules.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TransitTubeSpeed", "TransitTubeSpeed");
            #endregion
            #region Tuning
            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningGlobal", "TuningGlobal");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningGlobal_Title", "<b><color=red>!!! Unlock Tuning !!!</color></b>");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningGlobal_ToolTip", "If false, will disable all settings in this category.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningAtmosuitDecay", "TuningAtmosuitDecay");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitDecay_Title", "Atmosuit Decay");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitDecay_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningOxygenMaskDecay", "TuningOxygenMaskDecay");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningOxygenMaskDecay_Title", "Oxygen Mask Decay");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningOxygenMaskDecay_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningAtmosuitAthletics", "TuningAtmosuitAthletics");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitAthletics_Title", "Atmosuit Athletics");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitAthletics_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningAtmosuitScalding", "TuningAtmosuitScalding");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitScalding_Title", "Atmosuit Scalding");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitScalding_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningAtmosuitInsulation", "TuningAtmosuitInsulation");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitInsulation_Title", "Atmosuit Insulation");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitInsulation_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningAtmosuitThermalConductivityBarrier", "TuningAtmosuitThermalConductivityBarrier");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitThermalConductivityBarrier_Title", "Atmosuit Thermal Conductivity Barrier");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAtmosuitThermalConductivityBarrier_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningLeadsuitRadiationShielding", "TuningLeadsuitRadiationShielding");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitRadiationShielding_Title", "Leadsuit Radiation Shielding");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitRadiationShielding_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningLeadsuitAthletics", "TuningLeadsuitAthletics");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitAthletics_Title", "Leadsuit Athletics");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitAthletics_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningLeadsuitStrength", "TuningLeadsuitStrength");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitStrength_Title", "Leadsuit Strength");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitStrength_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningLeadsuitInsulation", "TuningLeadsuitInsulation");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitInsulation_Title", "Leadsuit Insulation");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitInsulation_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningLeadsuitThermalConductivityBarrier", "TuningLeadsuitThermalConductivityBarrier");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitThermalConductivityBarrier_Title", "Leadsuit Thermal Conductivity Barrier");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningLeadsuitThermalConductivityBarrier_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningMissionDurationScale", "TuningMissionDurationScale");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningMissionDurationScale_Title", "Mission Duration Scale");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningMissionDurationScale_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningMassPenaltyExponent", "TuningMassPenaltyExponent");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningMassPenaltyExponent_Title", "Rocket Mass Penalty Exponent");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningMassPenaltyExponent_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningMassPenaltyDivisor", "TuningMassPenaltyDivisor");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningMassPenaltyDivisor_Title", "Rocket Mass Penalty Divisor");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningMassPenaltyDivisor_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningResearchEvergreen", "TuningResearchEvergreen");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningResearchEvergreen_Title", "Research Evergreen");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningResearchEvergreen_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningResearchBasic", "TuningResearchBasic");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningResearchBasic_Title", "Research Basic");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningResearchBasic_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningAnalysisDiscovered", "TuningAnalysisDiscovered");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAnalysisDiscovered_Title", "Analysis Discovered");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAnalysisDiscovered_ToolTip", "At what point/percentage, will the analysis found things (i.e. asteroid). Should not be higher than Analysis Complete.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningAnalysisComplete", "TuningAnalysisComplete");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAnalysisComplete_Title", "Analysis Complete");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAnalysisComplete_ToolTip", "At what point/percentage, will the analysis completed and proceed to next Starmap hex.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningAnalysisDefaultCyclesPerDiscovery", "TuningAnalysisDefaultCyclesPerDiscovery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAnalysisDefaultCyclesPerDiscovery_Title", "Analysis Default Cycles Per Discovery");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningAnalysisDefaultCyclesPerDiscovery_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningThrustCostsLow", "TuningThrustCostsLow");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsLow_Title", "Thrust Costs Low");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsLow_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningThrustCostsMid", "TuningThrustCostsMid");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsMid_Title", "Thrust Costs Mid");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsMid_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningThrustCostsHigh", "TuningThrustCostsHigh");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsHigh_Title", "Thrust Costs High");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsHigh_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningThrustCostsVeryHigh", "TuningThrustCostsVeryHigh");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsVeryHigh_Title", "Thrust Costs Very High");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningThrustCostsVeryHigh_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningClusterFowPointsToReveal", "TuningClusterFowPointsToReveal");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningClusterFowPointsToReveal_Title", "Cluster Fow Points To Reveal");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningClusterFowPointsToReveal_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningClusterFowDefaultCyclesPerReveal", "TuningClusterFowDefaultCyclesPerReveal");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningClusterFowDefaultCyclesPerReveal_Title", "Cluster Fow Default Cycles Per Reveal");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningClusterFowDefaultCyclesPerReveal_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEngineEfficiencyWeak", "TuningEngineEfficiencyWeak");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyWeak_Title", "Engine Efficiency Weak");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyWeak_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEngineEfficiencyMedium", "TuningEngineEfficiencyMedium");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyMedium_Title", "Engine Efficiency Medium");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyMedium_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEngineEfficiencyStrong", "TuningEngineEfficiencyStrong");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyStrong_Title", "Engine Efficiency Strong");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyStrong_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEngineEfficiencyBooster", "TuningEngineEfficiencyBooster");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyBooster_Title", "Engine Efficiency Booster");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEngineEfficiencyBooster_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningOxidizerEfficiencyVeryLow", "TuningOxidizerEfficiencyVeryLow");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyVeryLow_Title", "Oxidizer Efficiency: Fertilizer");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyVeryLow_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningOxidizerEfficiencyLow", "TuningOxidizerEfficiencyLow");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyLow_Title", "Oxidizer Efficiency: Oxylite");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyLow_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningOxidizerEfficiencyHigh", "TuningOxidizerEfficiencyHigh");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyHigh_Title", "Oxidizer Efficiency: Liquid Oxygen");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningOxidizerEfficiencyHigh_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningCargoContainerMassStaticMass", "TuningCargoContainerMassStaticMass");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningCargoContainerMassStaticMass_Title", "Cargo Container Mass Static Mass");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningCargoContainerMassStaticMass_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningCargoContainerMassPayloadMass", "TuningCargoContainerMassPayloadMass");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningCargoContainerMassPayloadMass_Title", "Cargo Container Mass Payload Mass");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningCargoContainerMassPayloadMass_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningBurdenInsignificant", "TuningBurdenInsignificant");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenInsignificant_Title", "Burden Insignificant");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenInsignificant_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningBurdenMinor", "TuningBurdenMinor");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMinor_Title", "Burden Minor");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMinor_ToolTip", "Value applicable to:\n- Small Solid Oxidizer Tank\n- Battery Module\n- Basic Nosecone\n- Gas Cargo Canister");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningBurdenMinorPlus", "TuningBurdenMinorPlus");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMinorPlus_Title", "Burden Minor Plus");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMinorPlus_ToolTip", "Value applicable to:\n- Sugar Engine\n- Solo Spacefarer Nosecone\n- CO2 Engine\n- Liquid Cargo Tank\n- Cargographic Module");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningBurdenModerate", "TuningBurdenModerate");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenModerate_Title", "Burden Moderate");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenModerate_ToolTip", "Value applicable to:\n- Orbital Cargo Module\n- Rover Module\n- Trailblazer Module\n- Large Gas Cargo Canister\n- Cargo Bay\n- Steam Engine");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningBurdenModeratePlus", "TuningBurdenModeratePlus");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenModeratePlus_Title", "Burden Moderate Plus");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenModeratePlus_ToolTip", "Value applicable to:\n- Small Petroleum Engine\n- Large Liquid Fuel Tank\n- Large Liquid Cargo Tank\n- Lange Solid Oxidizer Tank\n- Liquid Oxidizer Tank");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningBurdenMajor", "TuningBurdenMajor");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMajor_Title", "Burden Major");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMajor_ToolTip", "Value applicable to:\n- Large Cargo Bay\n- Petroleum Engine\n- Spacefarer Module");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningBurdenMajorPlus", "TuningBurdenMajorPlus");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMajorPlus_Title", "Burden Major Plus");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMajorPlus_ToolTip", "Value applicable to:\n- Hydrogen Engine");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningBurdenMega", "TuningBurdenMega");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMega_Title", "Burden Mega");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningBurdenMega_ToolTip", "This value have no use in game.");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEnginePowerEarlyWeak", "TuningEnginePowerEarlyWeak");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerEarlyWeak_Title", "Engine Power: Steam Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerEarlyWeak_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEnginePowerEarlyStrong", "TuningEnginePowerEarlyStrong");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerEarlyStrong_Title", "Engine Power: Carbon Dioxide Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerEarlyStrong_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEnginePowerMidVeryStrong", "TuningEnginePowerMidVeryStrong");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidVeryStrong_Title", "Engine Power: Petroleum Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidVeryStrong_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEnginePowerMidStrong", "TuningEnginePowerMidStrong");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidStrong_Title", "Engine Power: Small Petroleum Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidStrong_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEnginePowerMidWeak", "TuningEnginePowerMidWeak");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidWeak_Title", "Engine Power: Sugar Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerMidWeak_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningEnginePowerLateStrong", "TuningEnginePowerLateStrong");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerLateStrong_Title", "Engine Power: Hydrogen Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningEnginePowerLateStrong_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningFuelCostPerDistanceVeryLow", "TuningFuelCostPerDistanceVeryLow");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceVeryLow_Title", "Fuel Cost Per Distance Very Low");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceVeryLow_ToolTip", "Not utilized in game");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningFuelCostPerDistanceLow", "TuningFuelCostPerDistanceLow");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceLow_Title", "Fuel Cost Per Distance Low");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceLow_ToolTip", "Not utilized in game");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningFuelCostPerDistanceMedium", "TuningFuelCostPerDistanceMedium");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceMedium_Title", "Fuel Cost Per Distance: Small Petroleum Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceMedium_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningFuelCostPerDistanceHigh", "TuningFuelCostPerDistanceHigh");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceHigh_Title", "Fuel Cost Per Distance: Hydrogen Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceHigh_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningFuelCostPerDistanceVeryHigh", "TuningFuelCostPerDistanceVeryHigh");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceVeryHigh_Title", "Fuel Cost Per Distance: Petroleum Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceVeryHigh_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningFuelCostPerDistanceGasLow", "TuningFuelCostPerDistanceGasLow");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceGasLow_Title", "Fuel Cost Per Distance: Gas Low");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceGasLow_ToolTip", "Not utilized in game");

            Helpers.StringsAddProperty("CustomizeBuildings.PROPERTY.TuningFuelCostPerDistanceGasHigh", "TuningFuelCostPerDistanceGasHigh");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceGasHigh_Title", "Fuel Cost Per Distance: Steam and CO2 Engine");
            Helpers.StringsAdd("CustomizeBuildings.LOCSTRINGS.TuningFuelCostPerDistanceGasHigh_ToolTip", "");
            #endregion
        }
    }
}
