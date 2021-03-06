﻿using System.Collections.Generic;
using Newtonsoft.Json;
using PeterHan.PLib;

namespace CustomizeBuildings
{
    [PeterHan.PLib.Options.ConfigFile("CustomizeBuildings.json", true, true)]
    [PeterHan.PLib.Options.RestartRequired]
    public class CustomizeBuildingsState
    {
        public static void OnLoad()
        {
            PeterHan.PLib.Options.POptions.RegisterOptions(typeof(CustomizeBuildingsState));
        }

        public int version { get; set; } = 29;

        #region Reset Button
        [Option("Reset To Klei Default", "Leave Menu with the CANCEL button!")]
        [JsonIgnore]
        public System.Action ResetToKleiDefault
        {
            get => delegate ()
            {
                StateManager.State.BatterySmartKJ = 20000f;
                StateManager.State.BatterySmartNoRunOff = false;
                StateManager.State.BatteryLargeKJ = 40000f;
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
                StateManager.State.ElectrolizerMaxPressure = 1.8f;
                StateManager.State.AirfilterDropsCanisters = false;
                StateManager.State.NewRecipeRockCrusher = false;
                StateManager.State.AirConditionerAbsoluteOutput = false;
                StateManager.State.SpaceHeaterTargetTemperature = false;
                StateManager.State.NoDupeValves = false;
                StateManager.State.NoDupeSwitches = false;
                StateManager.State.NoDupeToogleBuildings = false;
                StateManager.State.NoDupeToogleDoors = false;
                StateManager.State.ScannerInterferenceRadius = 15;
                StateManager.State.ScannerWorstWarningTime = 1f;
                StateManager.State.ScannerBestWarningTime = 200f;
                StateManager.State.ScannerBestNetworkSize = 6;
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
                StateManager.State.NoDupeSuitFabricator = false;
                StateManager.State.NoDupeSupermaterialRefinery = false;
                StateManager.State.NoDupeSludgePress = false;
                StateManager.State.NoDupeCompost = false;
                StateManager.State.NoDupeDesalinator = false;
                StateManager.State.NoDupeOilRefinery = false;
                StateManager.State.NoDupeOilWellCap = false;
                StateManager.State.NoDupeIceCooledFan = false;
                StateManager.State.NoDupeRanchStation = false;
                StateManager.State.SkillStationEnabled = false;
                StateManager.State.SkillStationCostTime = 180f;
                StateManager.State.SkillStationCostReset = 0f;
                StateManager.State.SkillStationCostRemoveTrait = 10000f;
                StateManager.State.SkillStationCostAddTrait = 10000f;
                StateManager.State.SkillStationCostBadTrait = -10000f;
                StateManager.State.SkillStationCostAddAptitude = 10000f;
                StateManager.State.SkillStationCostAddAttribute = 5000f;
                StateManager.State.BuildingBaseSettingGlobalFlag = false;
                StateManager.TrySaveConfigurationState();
            };
        }

        [Option("Reset To Custom Default", "Leave Menu with the CANCEL button!")]
        [JsonIgnore]
        public System.Action ResetToCustomDefault
        {
            get => delegate ()
            {
                StateManager.TrySaveConfigurationState(new CustomizeBuildingsState());
            };
        }

        #endregion

        #region Power
        [Option("Smart Battery KJ", "", "Power")]
        public float BatterySmartKJ { get; set; } = 200000f;
        [Option("Smart Battery remove runoff", "If true, smart battery won't lose charge over time.", "Power")]
        public bool BatterySmartNoRunOff { get; set; } = true;
        [Option("Large Battery KJ", "", "Power")]
        public float BatteryLargeKJ { get; set; } = 400000f;

        [Option("Solar Max Power", "Limit of how much light can be converted. Does not increase energy per lumen.", "Power")]
        public float SolarMaxPower { get; set; } = 600f;
        [Option("Solar Energy Multiplier", "Multiplies power generation. Limited up to 'Solar Max Power'.", "Power")]
        public float SolarEnergyMultiplier { get; set; } = 1f;
        #endregion

        #region Storage
        [Option("Storage Locker Capacity", "", "Storage")]
        public float LockerKG { get; set; } = 400000f;
        [Option("Smart Storage Locker Capacity", "", "Storage")]
        public float LockerSmartKG { get; set; } = 400000f;
        [Option("Gas Reservoir Capacity", "", "Storage")]
        public float GasReservoirKG { get; set; } = 99000f;
        [Option("Liquid Reservoir Capacity", "", "Storage")]
        public float LiquidReservoirKG { get; set; } = 99000f;
        [Option("Ration Box Capacity", "", "Storage")]
        public float RationBoxKG { get; set; } = 400000f;
        [Option("Fridge Capacity", "", "Storage")]
        public float FridgeKG { get; set; } = 400000f;
        [Option("Critter Feeder Capacity", "", "Storage")]
        public float CritterFeederKG { get; set; } = 400000f;
        [Option("Fish Feeder Capacity", "", "Storage")]
        public float FishFeederKG { get; set; } = 400000f;
        [Option("Canister Filler Capacity", "", "Storage")]
        public float CanisterFillerKG { get; set; } = 400000f;
        [Option("Conveyor Loader Capacity", "", "Storage")]
        public float ConveyorLoaderKG { get; set; } = 400000f;
        [Option("Conveyor Receptacle Capacity", "", "Storage")]
        public float ConveyorReceptacleKG { get; set; } = 400000f;
        [Option("Conveyor Receptacle Controller", "Set same as in Conveyor Receptacle Capacity.", "Storage")]
        public float IUserControlledMax { get; set; } = 400000f;

        [Option("Reservoir No Ground", "Reservoirs can be placed in the air.", "Storage")]
        public bool ReservoirNoGround { get; set; } = true;
        [Option("Reservoir Manual Delivery", "Dupes may store material in reservoirs.\n- dupes will deliver selected liquids/gases until the capacity is at the slider amount\n- liquid/gas pipes can still deliver any element and will ignore the slider limit\n- activating, then deactivating an element checkbox drops it on the floor, for easy removal of rogue elements", "Storage")]
        public bool ReservoirManualDelivery { get; set; } = true;
        #endregion

        #region Miscellaneous
        [Option("Electrolizer Max Pressure", "Pressure the electrolizer will stop producing oxygen/hydrogen.", "Miscellaneous")]
        public float ElectrolizerMaxPressure { get; set; } = 5f;
        [Option("Airfilter Canisters", "On deconstruction, air-filters (as well as all other buildings) drop gas canisters instead of venting stored gases.", "Miscellaneous")]
        public bool AirfilterDropsCanisters { get; set; } = true;

        [Option("New Recipe: Rock-Crusher", "Adds regolith to sand recipe.", "Miscellaneous")]
        public bool NewRecipeRockCrusher { get; set; } = true;

        [Option("Air Conditioner Absolute Output", "If true, Air Conditioner and Aquatuner get a temperature setting. Output temperature will always be that temperature, instead of -14°C. Also adds a DPU setting, which limits how fast the building heats up. Energy consumption will scale on cooling factor.", "Miscellaneous")]
        public bool AirConditionerAbsoluteOutput { get; set; } = true;
        [Option("Space Heater Target Temperature", "If true, Space Heater gets a temperature setting to control target temperature.", "Miscellaneous")]
        public bool SpaceHeaterTargetTemperature { get; set; } = true;
        #endregion

        #region Switches
        [Option("No Dupe Valves", "Valves are set instantly without dupe interaction.", "Switches")]
        public bool NoDupeValves { get; set; } = true;
        [Option("No Dupe Switches", "Switches are set instantly without dupe interaction.", "Switches")]
        public bool NoDupeSwitches { get; set; } = true;
        [Option("No Dupe Toogle Buildings", "Buildings are disabled instantly without dupe interaction.", "Switches")]
        public bool NoDupeToogleBuildings { get; set; } = true;
        [Option("No Dupe Toogle Doors", "Doors open/close instantly without dupe interaction.", "Switches")]
        public bool NoDupeToogleDoors { get; set; } = true;
        #endregion

        #region Space Scanner
        [Option("Scanner Interference Radius", "Radius looking for heavy machinery and free sky tiles.", "Space Scanner")]
        public int ScannerInterferenceRadius { get; set; } = 5;
        [Option("Scanner Worst Warning Time", "Worst time before a network will detect incoming meteors.", "Space Scanner")]
        public float ScannerWorstWarningTime { get; set; } = 30f;
        [Option("Scanner Best Warning Time", "Best time before a network will detect incoming meteors.", "Space Scanner")]
        public float ScannerBestWarningTime { get; set; } = 60f;
        [Option("Scanner Best Network Size", "Amount of scanners needed for best warning time.", "Space Scanner")]
        public int ScannerBestNetworkSize { get; set; } = 2;
        [Option("Ladder Comet Invincibility", "Comets will no longer deal damage to standard ladders (does not include plastic ones). Known to cause issues on Linux.", "Space Scanner")]
        public bool LadderCometInvincibility { get; set; } = true;
        #endregion

        #region Steam Turbine
        [Option("Steam Turbine Enabled", "Whenever or not to change the steam turbine at all. If false, ignores other settings.", "Steam Turbine")]
        public bool SteamTurbineEnabled { get; set; } = false;
        [Option("Steam Turbine Wattage", "Wattage produced by Steam Turbine under optimal conditions.", "Steam Turbine")]
        public float SteamTurbineWattage { get; set; } = 850f;
        [Option("Steam Turbine Source Element", "Steam Turbine input element.", "Steam Turbine")]
        public string SteamTurbineSourceElement { get; set; } = "Steam";
        [Option("Steam Turbine Output Element", "Steam Turbine output element.", "Steam Turbine")]
        public string SteamTurbineOutputElement { get; set; } = "Water";
        [Option("Steam Turbine Pump-Rate", "Steam Turbine amount of gas absorbed at once.", "Steam Turbine")]
        public float SteamTurbinePumpRateKG { get; set; } = 2f;
        [Option("Steam Turbine Max Self-Heat", "Steam Turbine maximum amount of heat tranfered from gas to machine.", "Steam Turbine")]
        public float SteamTurbineMaxSelfHeat { get; set; } = 64f;
        [Option("Steam Turbine Heat Transfer Percent", "Steam Turbine percent of heat transfered. Limited up to MAX.", "Steam Turbine")]
        public float SteamTurbineHeatTransferPercent { get; set; } = 0.1f;
        [Option("Steam Turbine Min Active Temperature", "Minimal temperature at which Steam Turbine turns on.", "Steam Turbine")]
        public float SteamTurbineMinActiveTemperature { get; set; } = 398.15f;
        [Option("Steam Turbine Ideal Temperature", "Best temperature at which Steam Turbine produces full energy.", "Steam Turbine")]
        public float SteamTurbineIdealTemperature { get; set; } = 473.15f;
        [Option("Steam Turbine Output Temperature", "Steam Turbine pipe output temperature.", "Steam Turbine")]
        public float SteamTurbineOutputTemperature { get; set; } = 368.15f;
        [Option("Steam Turbine Overheat Temperature", "Temperature where the Steam Turbine shuts off.", "Steam Turbine")]
        public float SteamTurbineOverheatTemperature { get; set; } = 373.15f;
        #endregion

        #region Pipes
        [Option("Pipe Throughput Percent", "Percent of pipe usage for Aquatuner and Air Conditioner.", "Pipes")]
        public float PipeThroughputPercent { get; set; } = 1.0f;
        [Option("Pipe Liquid Max Pressure", "Liquid pipe pressure.", "Pipes")]
        public float PipeLiquidMaxPressure { get; set; } = 10f;
        [Option("Pipe Gas Max Pressure", "Gas pipe pressure.", "Pipes")]
        public float PipeGasMaxPressure { get; set; } = 1f;
        [Option("Pipe Valve Pressure Button Show", "If true, will display a 'limit' button in the building window. While limited, output tile cannot exceed flow rate.", "Pipes")]
        public bool PipeValvePressureButtonShow { get; set; } = false;
        [Option("Pipe Liquid Pump", "Amount of liquid pumped at once from Liquid Pumps.", "Pipes")]
        public float PipeLiquidPump { get; set; } = 10f;
        [Option("Pipe Gas Pump", "Amount of gas pumped at once from Gas Pumps.", "Pipes")]
        public float PipeGasPump { get; set; } = 0.5f;
        [Option("Conveyor Rail Package Size", "Maximum size of packages on Conveyor Rails.", "Pipes")]
        public float ConveyorRailPackageSize { get; set; } = 20f;
        [Option("Conveyor Loader Has Slider", "If true, will show the capacity slider for Conveyor Loaders.", "Pipes")]
        public bool ConveyorLoaderHasSlider { get; set; } = true;
        [Option("Conveyor Receptacle Has Slider", "If true, will show the capacity slider for Conveyor Receptacles.", "Pipes")]
        public bool ConveyorReceptacleHasSlider { get; set; } = false;

        [Option("Conveyor Loader Accepts Liquids Gases", "If true, add liquids and gases to the filter list.", "Pipes")]
        public bool ConveyorLoaderAcceptLiquidsGas { get; set; } = false;
        #endregion

        #region Auto Sweeper
        [Option("Auto Sweeper Capacity", "Storage capacity of auto sweepers.", "Auto Sweeper")]
        public float AutoSweeperCapacity { get; set; } = 1000f;
        [Option("Auto Sweeper Range", "Range of Auto Sweeper. Very large numbers will cause lag.", "Auto Sweeper")]
        public int AutoSweeperRange { get; set; } = 4;
        [Option("Auto Sweeper Slider", "If true, will show a slider to reduce the Sweeper's range.", "Auto Sweeper")]
        public bool AutoSweeperSlider { get; set; } = true;
        [Option("Auto Sweeper Pickup Anything", "If true, Auto Sweeper can move/store anything that dupes can.", "Auto Sweeper")]
        public bool AutoSweeperPickupAnything { get; set; } = false;
        #endregion

        #region Robo Miner
        [Option("Robo Miner Width", "Mining width.", "Robo Miner")]
        public int RoboMinerWidth { get; set; } = 16;
        [Option("Robo Miner Height", "Mining height.", "Robo Miner")]
        public int RoboMinerHeight { get; set; } = 9;
        [Option("Robo Miner Offset", "Offset at which the Robo Miner works (negative numbers allowed).", "Robo Miner")]
        public int RoboMinerOffset { get; set; } = 0;
        [Option("Robo Miner Regolith Turbo", "If true, Regolith will be mined 6 times faster.", "Robo Miner")]
        public bool RoboMinerRegolithTurbo { get; set; } = true;
        [Option("Robo Miner Dig Through Glass", "Robo Miner dig through glass tiles, but only from glass (not diamonds)", "Robo Miner")]
        public bool RoboMinerDigThroughGlass { get; set; } = true;
        [Option("Robo Miner Dig Any Tile", "Robo Miner can dig any material.", "Robo Miner")]
        public bool RoboMinerDigAnyTile { get; set; } = true;
        #endregion

        #region Power Cable
        [Option("Wire Small Watts", "", "Power Cable")]
        public int WireSmallWatts { get; set; } = 1000;
        [Option("Wire Refined Watts", "", "Power Cable")]
        public int WireRefinedWatts { get; set; } = 2000;
        [Option("Wire Heavy Watts", "", "Power Cable")]
        public int WireHeavyWatts { get; set; } = 20000;
        [Option("Wire Refined Heavy Watts", "", "Power Cable")]
        public int WireRefinedHeavyWatts { get; set; } = 50000;
        #endregion

        #region Transit Tube
        [Option("Transit Tube Anywhere", "Transit Tubes can be placed in the background.", "Transit Tubes")]
        public bool TransitTubeAnywhere { get; set; } = true;
        [Option("Transit Tube U-Turns", "Removes all transit tube layout restrictions.", "Transit Tubes")]
        public bool TransitTubeUTurns { get; set; } = true;
        [Option("Transit Tube Joules Per Launch", "Cost per launch, normally 10000 joules.", "Transit Tubes")]
        public float TransitTubeJoulesPerLaunch { get; set; } = 10000f;
        [Option("Transit Tube Joule Capacity", "Capacity, normally 40000 joules.", "Transit Tubes")]
        public float TransitTubeJouleCapacity { get; set; } = 40000f;
        //public float TransitTubeSpeed { get; set; } = 18f;
        #endregion

        #region No Dupe
        [Option("No Dupe Global", "", "No Dupe")]
        public bool NoDupeGlobal { get; set; } = true;
        [Option("No Dupe Apothecary", "", "No Dupe")]
        public bool NoDupeApothecary { get; set; } = true;
        [Option("No Dupe ClothingFabricator", "", "No Dupe")]
        public bool NoDupeClothingFabricator { get; set; } = true;
        [Option("No Dupe Cooking Station", "", "No Dupe")]
        public bool NoDupeCookingStation { get; set; } = false;
        [Option("No Dupe Gourmet Cooking Station", "", "No Dupe")]
        public bool NoDupeGourmetCookingStation { get; set; } = false;
        [Option("No Dupe Egg Cracker", "", "No Dupe")]
        public bool NoDupeEggCracker { get; set; } = true;
        [Option("No Dupe Glass Forge", "", "No Dupe")]
        public bool NoDupeGlassForge { get; set; } = true;
        [Option("No Dupe Metal Refinery", "", "No Dupe")]
        public bool NoDupeMetalRefinery { get; set; } = true;
        [Option("No Dupe Microbe Musher", "", "No Dupe")]
        public bool NoDupeMicrobeMusher { get; set; } = true;
        [Option("No Dupe Rock Crusher", "", "No Dupe")]
        public bool NoDupeRockCrusher { get; set; } = true;
        [Option("No Dupe Suit Fabricator", "", "No Dupe")]
        public bool NoDupeSuitFabricator { get; set; } = true;
        [Option("No Dupe Supermaterial Refinery", "", "No Dupe")]
        public bool NoDupeSupermaterialRefinery { get; set; } = true;
        [Option("No Dupe Sludge Press", "", "No Dupe")]
        public bool NoDupeSludgePress { get; set; } = true;
        [Option("No Dupe Compost", "", "No Dupe")]
        public bool NoDupeCompost { get; set; } = true;
        [Option("No Dupe Desalinator", "", "No Dupe")]
        public bool NoDupeDesalinator { get; set; } = true;
        [Option("No Dupe Oil Refinery", "", "No Dupe")]
        public bool NoDupeOilRefinery { get; set; } = true;
        [Option("No Dupe Oil Well Cap", "", "No Dupe")]
        public bool NoDupeOilWellCap { get; set; } = true;
        [Option("No Dupe Ice Cooled Fan", "If true, will replace the dupe requirement with 120W power inlet. Ice consumption is different.", "No Dupe")]
        public bool NoDupeIceCooledFan { get; set; } = false; // TODO: revisit
        [Option("No Dupe Ranch Station", "If true, will buff the standard duration of grooming to 100 cycles.", "No Dupe")]
        public bool NoDupeRanchStation { get; set; } = false; // TODO: revisit
        //public bool NoDupeShearingStation { get; set; } = true;
        //public bool NoDupeCompost { get; set; } = true;
        #endregion

        #region Skill Station
        [Option("Skill Station Enabled", "If true, will repurpose the Skill Scrubber to change traits/interests/attributes of dupes. If false, fully disables this feature.", "Skill Station")]
        public bool SkillStationEnabled { get; set; } = true;
        [Option("Skill Station Time", "Time Skill Scrubber needs.", "Skill Station")]
        public float SkillStationCostTime { get; set; } = 20f;
        [Option("Skill Station Reset", "Exp cost to reset Skill Points", "Skill Station")]
        public float SkillStationCostReset { get; set; } = 0f;
        [Option("Skill Station Remove Trait", "Exp cost to remove any Trait.", "Skill Station")]
        public float SkillStationCostRemoveTrait { get; set; } = 10000f;
        [Option("Skill Station Add Trait", "Exp cost to add a new good Trait.", "Skill Station")]
        public float SkillStationCostAddTrait { get; set; } = 10000f;
        [Option("Skill Station Bad Trait", "Exp cost to add a new bad Trait (usually grants Exp).", "Skill Station")]
        public float SkillStationCostBadTrait { get; set; } = -10000f;
        [Option("Skill Station Add Aptitude", "Exp cost to add a new interest.", "Skill Station")]
        public float SkillStationCostAddAptitude { get; set; } = 10000f;
        [Option("Skill Station Add Attribute", "Exp cost to improve an attribute by 1.", "Skill Station")]
        public float SkillStationCostAddAttribute { get; set; } = 5000f;
        #endregion

        #region Tuning
        [Option("Atmosuit Decay", "Default: -0.1", "Tuning")]
        public float TuningAtmosuitDecay { get; set; } = 0f;
        [Option("Oxygen Mask Decay", "Default: -0.2", "Tuning")]
        public float TuningOxygenMaskDecay { get; set; } = 0f;
        [Option("Atmosuit Athletics", "Default: -6", "Tuning")]
        public float TuningAtmosuitAthletics { get; set; } = float.NaN;
        [Option("Atmosuit Scalding", "Default: 1000", "Tuning")]
        public float TuningAtmosuitScalding { get; set; } = float.NaN;
        [Option("Atmosuit Insulation", "Default: 50", "Tuning")]
        public float TuningAtmosuitInsulation { get; set; } = float.NaN;
        [Option("Atmosuit Thermal Conductivity Barrier", "Default: 0.2", "Tuning")]
        public float TuningAtmosuitThermalConductivityBarrier { get; set; } = float.NaN;
        #endregion

        #region Advanced
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

        public Dictionary<string, float> MachineMultiplier { get; set; } = new Dictionary<string, float> {
            { "EthanolDistillery", 4f }
        };

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
            return true;
        }

        public class BuildingStruct
        {
            public float? PowerConsumption;
            public float? OverheatTemperature;
            public string MaterialCategory;
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
            public float[] ConstructionMass;
            public float? ThermalConductivity;

            public BuildingStruct(float? PowerConsumption = null, string MaterialCategory = null, float? OverheatTemperature = null,
                float? ExhaustKilowattsWhenActive = null, float? SelfHeatKilowattsWhenActive = null, float? GeneratorWattageRating = null,
                float? GeneratorBaseCapacity = null, float? BaseDecor = null, float? BaseDecorRadius = null, BuildLocationRule? LocationRule = null,
                PermittedRotations? Rotations = null, bool? Floodable = null, bool? IsFoundation = null, float[] ConstructionMass = null,
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

    }
}