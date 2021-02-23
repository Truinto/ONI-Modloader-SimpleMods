using System;

namespace CustomizeBuildings
{
    public static class LOCSTRINGS
    {
        public static class OPTIONS
        {
            #region Reset Button
            public static LocString ResetToKleiDefault_Title = new LocString("Reset To Klei Default");
            public static LocString ResetToKleiDefault_ToolTip = new LocString("Leave Menu with the CANCEL button!");
            
            public static LocString ResetToCustomDefault_Title = new LocString("Reset To Klei Default");
            public static LocString ResetToCustomDefault_ToolTip = new LocString("Leave Menu with the CANCEL button!");
            #endregion

            #region Power
            public static LocString BatterySmartKJ_Title = new LocString("Smart Battery KJ");
            public static LocString BatterySmartKJ_ToolTip = new LocString("");

            public static LocString BatterySmartNoRunOff_Title = new LocString("Smart Battery remove runoff");
            public static LocString BatterySmartNoRunOff_ToolTip = new LocString("If true, smart battery won't lose charge over time.");
            
            public static LocString BatteryLargeKJ_Title = new LocString("Large Battery KJ");
            public static LocString BatteryLargeKJ_ToolTip = new LocString("");
            
            public static LocString SolarMaxPower_Title = new LocString("Solar Max Power");
            public static LocString SolarMaxPower_ToolTip = new LocString("Limit of how much light can be converted. Does not increase energy per lumen.");
            
            public static LocString SolarEnergyMultiplier_Title = new LocString("Solar Energy Multiplier");
            public static LocString SolarEnergyMultiplier_ToolTip = new LocString("Multiplies power generation. Limited up to 'Solar Max Power'.");
            #endregion

            #region Storage
            public static LocString LockerKG_Title = new LocString("Storage Locker Capacity");
            public static LocString LockerKG_ToolTip = new LocString("");
            
            public static LocString LockerSmartKG_Title = new LocString("Smart Storage Locker Capacity");
            public static LocString LockerSmartKG_ToolTip = new LocString("");
            
            public static LocString GasReservoirKG_Title = new LocString("Gas Reservoir Capacity");
            public static LocString GasReservoirKG_ToolTip = new LocString("");
            
            public static LocString LiquidReservoirKG_Title = new LocString("Liquid Reservoir Capacity");
            public static LocString LiquidReservoirKG_ToolTip = new LocString("");
            
            public static LocString RationBoxKG_Title = new LocString("Ration Box Capacity");
            public static LocString RationBoxKG_ToolTip = new LocString("Ration Box Capacity");
            
            public static LocString FridgeKG_Title = new LocString("Fridge Capacity");
            public static LocString FridgeKG_ToolTip = new LocString("");
            
            public static LocString CritterFeederKG_Title = new LocString("Critter Feeder Capacity");
            public static LocString CritterFeederKG_ToolTip = new LocString("");
            
            public static LocString FishFeederKG_Title = new LocString("Fish Feeder Capacity");
            public static LocString FishFeederKG_ToolTip = new LocString("");
            
            public static LocString CanisterFillerKG_Title = new LocString("Canister Filler Capacity");
            public static LocString CanisterFillerKG_ToolTip = new LocString("");
            
            public static LocString ConveyorLoaderKG_Title = new LocString("Conveyor Loader Capacity");
            public static LocString ConveyorLoaderKG_ToolTip = new LocString("");
            
            public static LocString ConveyorReceptacleKG_Title = new LocString("Conveyor Receptacle Capacity");
            public static LocString ConveyorReceptacleKG_ToolTip = new LocString("");
            
            public static LocString IUserControlledMax_Title = new LocString("Conveyor Receptacle Controller");
            public static LocString IUserControlledMax_ToolTip = new LocString("Set same as in Conveyor Receptacle Capacity.");
            
            public static LocString ReservoirNoGround_Title = new LocString("Reservoir No Ground");
            public static LocString ReservoirNoGround_ToolTip = new LocString("Reservoirs can be placed in the air.");
            
            public static LocString ReservoirManualDelivery_Title = new LocString("Reservoir Manual Delivery");
            public static LocString ReservoirManualDelivery_ToolTip = new LocString("Dupes may store material in reservoirs.\n- dupes will deliver selected liquids/gases until the capacity is at the slider amount\n- liquid/gas pipes can still deliver any element and will ignore the slider limit\n- activating, then deactivating an element checkbox drops it on the floor, for easy removal of rogue elements");
            #endregion

            #region Miscellaneous
            public static LocString ElectrolizerMaxPressure_Title = new LocString("Electrolizer Max Pressure");
            public static LocString ElectrolizerMaxPressure_ToolTip = new LocString("Pressure the electrolizer will stop producing oxygen/hydrogen.");

            public static LocString AirfilterDropsCanisters_Title = new LocString("Airfilter Canisters");
            public static LocString AirfilterDropsCanisters_ToolTip = new LocString("On deconstruction, air-filters (as well as all other buildings) drop gas canisters instead of venting stored gases.");
            
            public static LocString NewRecipeRockCrusher_Title = new LocString("New Recipe: Rock-Crusher");
            public static LocString NewRecipeRockCrusher_ToolTip = new LocString("Adds regolith to sand recipe.");
            
            public static LocString AirConditionerAbsoluteOutput_Title = new LocString("Air Conditioner Absolute Output");
            public static LocString AirConditionerAbsoluteOutput_ToolTip = new LocString("If true, Air Conditioner and Aquatuner get a temperature setting. Output temperature will always be that temperature, instead of -14°C. Also adds a DPU setting, which limits how fast the building heats up. Energy consumption will scale on cooling factor.");
            
            public static LocString SpaceHeaterTargetTemperature_Title = new LocString("Space Heater Target Temperature");
            public static LocString SpaceHeaterTargetTemperature_ToolTip = new LocString("If true, Space Heater gets a temperature setting to control target temperature.");
            #endregion

            #region Switches
            public static LocString NoDupeValves_Title = new LocString("No Dupe Valves");
            public static LocString NoDupeValves_ToolTip = new LocString("Valves are set instantly without dupe interaction.");
            
            public static LocString NoDupeSwitches_Title = new LocString("No Dupe Switches");
            public static LocString NoDupeSwitches_ToolTip = new LocString("Switches are set instantly without dupe interaction.");
            
            public static LocString NoDupeToogleBuildings_Title = new LocString("No Dupe Toogle Buildings");
            public static LocString NoDupeToogleBuildings_ToolTip = new LocString("Buildings are disabled instantly without dupe interaction.");
            
            public static LocString NoDupeToogleDoors_Title = new LocString("No Dupe Toogle Doors");
            public static LocString NoDupeToogleDoors_ToolTip = new LocString("Doors open/close instantly without dupe interaction.");
            #endregion

            #region Space Scanner
            public static LocString ScannerInterferenceRadius_Title = new LocString("Scanner Interference Radius");
            public static LocString ScannerInterferenceRadius_ToolTip = new LocString("Radius looking for heavy machinery and free sky tiles.");
            
            public static LocString ScannerWorstWarningTime_Title = new LocString("Scanner Worst Warning Time");
            public static LocString ScannerWorstWarningTime_ToolTip = new LocString("Worst time before a network will detect incoming meteors.");
            
            public static LocString ScannerBestWarningTime_Title = new LocString("Scanner Best Warning Time");
            public static LocString ScannerBestWarningTime_ToolTip = new LocString("Best time before a network will detect incoming meteors.");
            
            public static LocString ScannerBestNetworkSize_Title = new LocString("Scanner Best Network Size");
            public static LocString ScannerBestNetworkSize_ToolTip = new LocString("Amount of scanners needed for best warning time.");
            
            public static LocString LadderCometInvincibility_Title = new LocString("Ladder Comet Invincibility");
            public static LocString LadderCometInvincibility_ToolTip = new LocString("Comets will no longer deal damage to standard ladders (does not include plastic ones). Known to cause issues on Linux.");
            #endregion

            #region Steam Turbine
            public static LocString SteamTurbineEnabled_Title = new LocString("Steam Turbine Enabled");
            public static LocString SteamTurbineEnabled_ToolTip = new LocString("Whenever or not to change the steam turbine at all. If false, ignores other settings.");
            
            public static LocString SteamTurbineWattage_Title = new LocString("Steam Turbine Wattage");
            public static LocString SteamTurbineWattage_ToolTip = new LocString("Wattage produced by Steam Turbine under optimal conditions.");
            
            public static LocString SteamTurbineSourceElement_Title = new LocString("Steam Turbine Source Element");
            public static LocString SteamTurbineSourceElement_ToolTip = new LocString("Steam Turbine input element.");
            
            public static LocString SteamTurbineOutputElement_Title = new LocString("Steam Turbine Output Element");
            public static LocString SteamTurbineOutputElement_ToolTip = new LocString("Steam Turbine output element.");
            
            public static LocString SteamTurbinePumpRateKG_Title = new LocString("Steam Turbine Pump-Rate");
            public static LocString SteamTurbinePumpRateKG_ToolTip = new LocString("Steam Turbine amount of gas absorbed at once.");
            
            public static LocString SteamTurbineMaxSelfHeat_Title = new LocString("Steam Turbine Max Self-Heat");
            public static LocString SteamTurbineMaxSelfHeat_ToolTip = new LocString("Steam Turbine maximum amount of heat tranfered from gas to machine.");
            
            public static LocString SteamTurbineHeatTransferPercent_Title = new LocString("Steam Turbine Heat Transfer Percent");
            public static LocString SteamTurbineHeatTransferPercent_ToolTip = new LocString("Steam Turbine percent of heat transfered. Limited up to MAX.");
            
            public static LocString SteamTurbineMinActiveTemperature_Title = new LocString("Steam Turbine Min Active Temperature");
            public static LocString SteamTurbineMinActiveTemperature_ToolTip = new LocString("Minimal temperature at which Steam Turbine turns on.");
            
            public static LocString SteamTurbineIdealTemperature_Title = new LocString("Steam Turbine Ideal Temperature");
            public static LocString SteamTurbineIdealTemperature_ToolTip = new LocString("Best temperature at which Steam Turbine produces full energy.");
            
            public static LocString SteamTurbineOutputTemperature_Title = new LocString("Steam Turbine Output Temperature");
            public static LocString SteamTurbineOutputTemperature_ToolTip = new LocString("Steam Turbine pipe output temperature.");
            
            public static LocString SteamTurbineOverheatTemperature_Title = new LocString("Steam Turbine Overheat Temperature");
            public static LocString SteamTurbineOverheatTemperature_ToolTip = new LocString("Temperature where the Steam Turbine shuts off.");
            #endregion

            #region Pipes
            public static LocString PipeThroughputPercent_Title = new LocString("Pipe Throughput Percent");
            public static LocString PipeThroughputPercent_ToolTip = new LocString("Percent of pipe usage for Aquatuner and Air Conditioner.");
            
            public static LocString PipeLiquidMaxPressure_Title = new LocString("Pipe Liquid Max Pressure");
            public static LocString PipeLiquidMaxPressure_ToolTip = new LocString("Liquid pipe pressure.");
            
            public static LocString PipeGasMaxPressure_Title = new LocString("Pipe Gas Max Pressure");
            public static LocString PipeGasMaxPressure_ToolTip = new LocString("Gas pipe pressure.");
            
            public static LocString PipeValvePressureButtonShow_Title = new LocString("Pipe Valve Pressure Button Show");
            public static LocString PipeValvePressureButtonShow_ToolTip = new LocString("If true, will display a 'limit' button in the building window. While limited, output tile cannot exceed flow rate.");
            
            public static LocString PipeLiquidPump_Title = new LocString("Pipe Liquid Pump");
            public static LocString PipeLiquidPump_ToolTip = new LocString("Amount of liquid pumped at once from Liquid Pumps.");
            
            public static LocString PipeGasPump_Title = new LocString("Pipe Gas Pump");
            public static LocString PipeGasPump_ToolTip = new LocString("Amount of gas pumped at once from Gas Pumps.");
            
            public static LocString ConveyorRailPackageSize_Title = new LocString("Conveyor Rail Package Size");
            public static LocString ConveyorRailPackageSize_ToolTip = new LocString("Maximum size of packages on Conveyor Rails.");
            
            public static LocString ConveyorLoaderHasSlider_Title = new LocString("Conveyor Loader Has Slider");
            public static LocString ConveyorLoaderHasSlider_ToolTip = new LocString("If true, will show the capacity slider for Conveyor Loaders.");
            
            public static LocString ConveyorReceptacleHasSlider_Title = new LocString("Conveyor Receptacle Has Slider");
            public static LocString ConveyorReceptacleHasSlider_ToolTip = new LocString("If true, will show the capacity slider for Conveyor Receptacles.");
            
            public static LocString ConveyorLoaderAcceptLiquidsGas_Title = new LocString("Conveyor Loader Accepts Liquids Gases");
            public static LocString ConveyorLoaderAcceptLiquidsGas_ToolTip = new LocString("If true, add liquids and gases to the filter list.");
            #endregion

            #region Auto Sweeper
            public static LocString AutoSweeperCapacity_Title = new LocString("Auto Sweeper Capacity");
            public static LocString AutoSweeperCapacity_ToolTip = new LocString("Storage capacity of auto sweepers.");
            
            public static LocString AutoSweeperRange_Title = new LocString("Auto Sweeper Range");
            public static LocString AutoSweeperRange_ToolTip = new LocString("Range of Auto Sweeper. Very large numbers will cause lag.");
            
            public static LocString AutoSweeperSlider_Title = new LocString("Auto Sweeper Slider");
            public static LocString AutoSweeperSlider_ToolTip = new LocString("If true, will show a slider to reduce the Sweeper's range.");
            
            public static LocString AutoSweeperPickupAnything_Title = new LocString("Auto Sweeper Pickup Anything");
            public static LocString AutoSweeperPickupAnything_ToolTip = new LocString("If true, Auto Sweeper can move/store anything that dupes can.");
            #endregion

            #region Robo Miner
            public static LocString RoboMinerWidth_Title = new LocString("Robo Miner Width");
            public static LocString RoboMinerWidth_ToolTip = new LocString("Mining width.");
            
            public static LocString RoboMinerHeight_Title = new LocString("Robo Miner Height");
            public static LocString RoboMinerHeight_ToolTip = new LocString("Mining height.");
            
            public static LocString RoboMinerOffset_Title = new LocString("Robo Miner Offset");
            public static LocString RoboMinerOffset_ToolTip = new LocString("Offset at which the Robo Miner works (negative numbers allowed).");
            
            public static LocString RoboMinerRegolithTurbo_Title = new LocString("Robo Miner Regolith Turbo");
            public static LocString RoboMinerRegolithTurbo_ToolTip = new LocString("If true, Regolith will be mined 6 times faster.");
            
            public static LocString RoboMinerDigThroughGlass_Title = new LocString("Robo Miner Dig Through Glass");
            public static LocString RoboMinerDigThroughGlass_ToolTip = new LocString("Robo Miner dig through glass tiles, but only from glass (not diamonds)");
            
            public static LocString RoboMinerDigAnyTile_Title = new LocString("Robo Miner Dig Any Tile");
            public static LocString RoboMinerDigAnyTile_ToolTip = new LocString("Robo Miner can dig any material.");
            #endregion

            #region Power Cable
            public static LocString WireSmallWatts_Title = new LocString("Wire Small Watts");
            public static LocString WireSmallWatts_ToolTip = new LocString("");
            
            public static LocString WireRefinedWatts_Title = new LocString("Wire Refined Watts");
            public static LocString WireRefinedWatts_ToolTip = new LocString("");
            
            public static LocString WireHeavyWatts_Title = new LocString("Wire Heavy Watts");
            public static LocString WireHeavyWatts_ToolTip = new LocString("");
            
            public static LocString WireRefinedHeavyWatts_Title = new LocString("Wire Refined Heavy Watts");
            public static LocString WireRefinedHeavyWatts_ToolTip = new LocString("");
            #endregion

            #region Transit Tube            
            public static LocString TransitTubeAnywhere_Title = new LocString("Transit Tube Anywhere");
            public static LocString TransitTubeAnywhere_ToolTip = new LocString("Transit Tubes can be placed in the background.");
            
            public static LocString TransitTubeUTurns_Title = new LocString("Transit Tube U-Turns");
            public static LocString TransitTubeUTurns_ToolTip = new LocString("Removes all transit tube layout restrictions.");
            
            public static LocString TransitTubeJoulesPerLaunch_Title = new LocString("Transit Tube Joules Per Launch");
            public static LocString TransitTubeJoulesPerLaunch_ToolTip = new LocString("Cost per launch, normally 10000 joules.");
            
            public static LocString TransitTubeJouleCapacity_Title = new LocString("Transit Tube Joule Capacity");
            public static LocString TransitTubeJouleCapacity_ToolTip = new LocString("Capacity, normally 40000 joules.");
            #endregion

            #region No Dupe
            public static LocString NoDupeGlobal_Title = new LocString("No Dupe Global");
            public static LocString NoDupeGlobal_ToolTip = new LocString("If false, will disable all settings in this category.");
            
            public static LocString NoDupeApothecary_Title = new LocString("No Dupe Apothecary");
            public static LocString NoDupeApothecary_ToolTip = new LocString("");
            
            public static LocString NoDupeClothingFabricator_Title = new LocString("No Dupe ClothingFabricator");
            public static LocString NoDupeClothingFabricator_ToolTip = new LocString("");
            
            public static LocString NoDupeCookingStation_Title = new LocString("No Dupe Cooking Station");
            public static LocString NoDupeCookingStation_ToolTip = new LocString("");
            
            public static LocString NoDupeGourmetCookingStation_Title = new LocString("No Dupe Gourmet Cooking Station");
            public static LocString NoDupeGourmetCookingStation_ToolTip = new LocString("");
            
            public static LocString NoDupeEggCracker_Title = new LocString("No Dupe Egg Cracker");
            public static LocString NoDupeEggCracker_ToolTip = new LocString("");
            
            public static LocString NoDupeGlassForge_Title = new LocString("No Dupe Glass Forge");
            public static LocString NoDupeGlassForge_ToolTip = new LocString("");
            
            public static LocString NoDupeMetalRefinery_Title = new LocString("No Dupe Metal Refinery");
            public static LocString NoDupeMetalRefinery_ToolTip = new LocString("");
            
            public static LocString NoDupeMicrobeMusher_Title = new LocString("No Dupe Microbe Musher");
            public static LocString NoDupeMicrobeMusher_ToolTip = new LocString("");
            
            public static LocString NoDupeRockCrusher_Title = new LocString("No Dupe Rock Crusher");
            public static LocString NoDupeRockCrusher_ToolTip = new LocString("");
            
            public static LocString NoDupeSuitFabricator_Title = new LocString("No Dupe Suit Fabricator");
            public static LocString NoDupeSuitFabricator_ToolTip = new LocString("");
            
            public static LocString NoDupeSupermaterialRefinery_Title = new LocString("No Dupe Supermaterial Refinery");
            public static LocString NoDupeSupermaterialRefinery_ToolTip = new LocString("");
            
            public static LocString NoDupeSludgePress_Title = new LocString("No Dupe Sludge Press");
            public static LocString NoDupeSludgePress_ToolTip = new LocString("");
            
            public static LocString NoDupeCompost_Title = new LocString("No Dupe Compost");
            public static LocString NoDupeCompost_ToolTip = new LocString("");
            
            public static LocString NoDupeDesalinator_Title = new LocString("No Dupe Desalinator");
            public static LocString NoDupeDesalinator_ToolTip = new LocString("");
            
            public static LocString NoDupeOilRefinery_Title = new LocString("No Dupe Oil Refinery");
            public static LocString NoDupeOilRefinery_ToolTip = new LocString("");
            
            public static LocString NoDupeOilWellCap_Title = new LocString("No Dupe Oil Well Cap");
            public static LocString NoDupeOilWellCap_ToolTip = new LocString("");
            
            public static LocString NoDupeIceCooledFan_Title = new LocString("No Dupe Ice Cooled Fan");
            public static LocString NoDupeIceCooledFan_ToolTip = new LocString("If true, will replace the dupe requirement with 120W power inlet. Ice consumption is different.");
            
            public static LocString NoDupeRanchStation_Title = new LocString("No Dupe Ranch Station");
            public static LocString NoDupeRanchStation_ToolTip = new LocString("If true, will buff the standard duration of grooming to 100 cycles.");
            #endregion

            #region Skill Station
            public static LocString SkillStationEnabled_Title = new LocString("Skill Station Enabled");
            public static LocString SkillStationEnabled_ToolTip = new LocString("If true, will repurpose the Skill Scrubber to change traits/interests/attributes of dupes. If false, fully disables this feature.");
            
            public static LocString SkillStationCostTime_Title = new LocString("Skill Station Time");
            public static LocString SkillStationCostTime_ToolTip = new LocString("Time Skill Scrubber needs.");
            
            public static LocString SkillStationCostReset_Title = new LocString("Skill Station Reset");
            public static LocString SkillStationCostReset_ToolTip = new LocString("Exp cost to reset Skill Points (select \"None\")");
            
            public static LocString SkillStationCostRemoveTrait_Title = new LocString("Skill Station Remove Trait");
            public static LocString SkillStationCostRemoveTrait_ToolTip = new LocString("Exp cost to remove any Trait.");
            
            public static LocString SkillStationCostAddTrait_Title = new LocString("Skill Station Add Trait");
            public static LocString SkillStationCostAddTrait_ToolTip = new LocString("Exp cost to add a new good Trait.");
            
            public static LocString SkillStationCostBadTrait_Title = new LocString("Skill Station Bad Trait");
            public static LocString SkillStationCostBadTrait_ToolTip = new LocString("Exp cost to add a new bad Trait (usually grants Exp).");
            
            public static LocString SkillStationCostAddAptitude_Title = new LocString("Skill Station Add Aptitude");
            public static LocString SkillStationCostAddAptitude_ToolTip = new LocString("Exp cost to add a new interest.");
            
            public static LocString SkillStationCostAddAttribute_Title = new LocString("Skill Station Add Attribute");
            public static LocString SkillStationCostAddAttribute_ToolTip = new LocString("Exp cost to improve an attribute by 1.");
            #endregion

            #region Tuning
            public static LocString TuningAtmosuitDecay_Title = new LocString("Atmosuit Decay");
            public static LocString TuningAtmosuitDecay_ToolTip = new LocString("");
            
            public static LocString TuningOxygenMaskDecay_Title = new LocString("Oxygen Mask Decay");
            public static LocString TuningOxygenMaskDecay_ToolTip = new LocString("");
            
            public static LocString TuningAtmosuitAthletics_Title = new LocString("Atmosuit Athletics");
            public static LocString TuningAtmosuitAthletics_ToolTip = new LocString("");
            
            public static LocString TuningAtmosuitScalding_Title = new LocString("Atmosuit Scalding");
            public static LocString TuningAtmosuitScalding_ToolTip = new LocString("");
            
            public static LocString TuningAtmosuitInsulation_Title = new LocString("Atmosuit Insulation");
            public static LocString TuningAtmosuitInsulation_ToolTip = new LocString("");
            
            public static LocString TuningAtmosuitThermalConductivityBarrier_Title = new LocString("Atmosuit Thermal Conductivity Barrier");
            public static LocString TuningAtmosuitThermalConductivityBarrier_ToolTip = new LocString("");
            #endregion
        }

        public static class BUILDINGS
        {
            #region SkillStation
            public static LocString SkillStationBadTrait = new LocString("Bad Traits");
            public static LocString SkillStationRemoveTraits = new LocString("Remove Traits");
            public static LocString SkillStationGoodTraits = new LocString("Good Traits");
            public static LocString SkillStationGeneTraits = new LocString("Gene Traits");
            public static LocString SkillStationCongenitalTraits = new LocString("Congenital Traits");
            public static LocString SkillStationStressTraits = new LocString("Stress Traits");
            public static LocString SkillStationJoyTraits = new LocString("Joy Traits");
            public static LocString SkillStationAptitudes = new LocString("Aptitudes");
            public static LocString SkillStationAttributes = new LocString("Attributes");
            public static LocString SkillStationFailure = new LocString("{0} wasted time getting their brain fried - Make sure you have enough EXP to trade in");
            public static LocString SkillStationReset = new LocString("{0} got their <style=\"KKeyword\">Skill Points</style> refunded");
            public static LocString SkillStationAddTrait = new LocString("{0} gained a new trait '{1}'");
            public static LocString SkillStationRemoveTrait = new LocString("{0} lost a trait '{1}'");
            public static LocString SkillStationAttributeUp = new LocString("{0} improved their {1} from {2} to {3}.");
            public static LocString SkillStationAptitude = new LocString("{0} sparked new Interests in {1}");
            #endregion
        }
    }
}