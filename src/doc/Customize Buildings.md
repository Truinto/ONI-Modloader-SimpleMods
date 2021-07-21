# Customize Buildings

QoL changes, more storage, less duplicant requirement. Transit tube anywhere!

Settings found here (after running mod)
----------
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\CustomizeBuildings.json
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/CustomizeBuildings.json
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/CustomizeBuildings.json

Most settings also accessible through ingame menu.

Full content
----------
ALL setting are optional and can be turned off.

* change smart and heavy battery capacity
* remove smart battery no runoff
* change solar pannel power
* a11 storage containers customizable capacity, including reservoirs
* reservoirs act like storage bins
* reservoirs don't need ground below
* change electrolizer maximum pressure
* air filters will drop leftover polluted gas as canisters
* rock crusher new regolith to sand recipe
* air conditioner absolute mode (see readme for more details)
* change space heater shutoff temperature individually
* valves, switches, building, and door toggles don't require dupes
* change space scanner properties (like how many you need)
* ladder don't get hit by comets
* change steam engine properties (like temperature and elements)
* change pipe and conveyor rail capacity
* change pump speed
* change auto sweeper range individually, as well as speed
* change robo miner range
* robo miner can dig any material
* robo miner work through glass tiles
* robo miner digs regolith faster
* change power wire wattage
* change transit tube power properties
* transit tubes can be placed behind tiles/buildings and make U-turns
* all crafting stations, as well as some other buildings, can be set to require no duplicant (metal refinery, rock crusher and more)
* use skill scrubber to add/remove duplicant traits and skills (optional exp cost)
* change atmosuit/oxymask decay rate and other properties
* change some rocket properties (undocumented, please help)
* advanced settings
* :may change any of the following settings for all buildings (including buildings added from other mods)
* :requires manual json editing
* :read README for explanation
* - power consumption
* - building material
* - base overheat temperature
* - heat generated
* - generator output watts
* - decor values
* - where buildings are allowed to be build e.g. in the background, on the floor, anywhere
* - enable rotation for any building; might result in funny graphical bugs

[Github](https://github.com/Truinto/ONI-Modloader-SimpleMods/tree/master/Mods/Customize%20Buildings)

# README v41

If CustomizeBuildings.json is corrupted or otherwise does not work, delete it and restart the game. A new clean file will be generated.

* Reset To Klei Default: will discard current dialog and set all settings to off state
* Reset To Mod Preset: will discard current dialog and set all settings as if re-installed mod
* BatterySmartKJ: 
* BatterySmartNoRunOff: 
* BatteryLargeKJ: 
* BatteryLargeKJ: 
* SolarMaxPower: 
* SolarEnergyMultiplier: 
* LockerKG: 
* LockerSmartKG: 
* GasReservoirKG: 
* LiquidReservoirKG: 
* RationBoxKG: 
* FridgeKG: 
* CritterFeederKG: 
* FishFeederKG: 
* CanisterFillerKG: 
* ConveyorLoaderKG: 
* ConveyorReceptacleKG: 
* IUserControlledMax: set same as ConveyorReceptacleKG
* ReservoirNoGround: if true, reservoirs can be placed in the air
* ReservoirManualDelivery: if true, reservoirs will act like storage bins
*    - dupes will deliver selected liquids/gases until the capacity is at the slider amount
*    - liquid/gas pipes can still deliver any element and will ignore the slider limit
*    - activating, then deactivating an element checkbox drops it on the floor, for easy removal of rogue elements
* RailgunMaxLaunch: 
* ElectrolizerMaxPressure: pressure at which the elecrolizer will stop producing oxygen/hydrogen
* AirfilterDropsCanisters: if true, airfilters (as well as all other buildings) will drop gas canisters on deconstruction (instead of venting stored gases)
* NewRecipeRockCrusher: adds a new recipe to rock crusher -> Regolith To Sand
* AirConditionerAbsoluteOutput: if true, adds a slider to air conditioner/aqua tuner
*    - when set to 0 it will work as normal (-14 °C delta)
*    - otherwise will output material at exactly the desired temperature
*    - will preserve heat as normal, meaning high temperature delta will result in massive heating
*    - additional max DPU setting can be used to limit heating in exchange of lower throughput
*    - energy consumption scales with DPU
* SpaceHeaterTargetTemperature: add a slider to space heater/liquid heater which sets the shutoff temperature; does not prevent overheating
* AlgaeTerrariumPatch: if true, apply AlgaeTerrarium
* AlgaeTerrarium: new input/output elements; only last liquid will be absorbed from environment; will always passively consume CO2
* DoorSelfSealing: if true, doors will block gas flow while set to automatic
* MaterialIgnoreInsufficientMaterial: If true, will allow placing buildings while having insufficient building material.
* MaterialAutoSelect: If true, will keep selected building material, even if stored amount is insufficient.
* NoDupeValves: valves are set instantly without dupe interaction
* NoDupeSwitches: switches are set instantly without dupe interaction
* NoDupeToogleBuildings: buildings are enabled/disabled instantly without dupe interaction
* NoDupeToogleDoors: doors are set instantly without dupe interaction  
* ScannerInterferenceRadius: radius looking for heavy machinery and free sky tiles
* ScannerWorstWarningTime: worst time before a network will detect incoming meteors
* ScannerBestWarningTime: best time before a network will detect incoming meteors
* ScannerBestNetworkSize: amount of scanners needed for best warning time
* LadderCometInvincibility: comets will no longer deal damage to standard ladders (does not include plastic ones)
* TelescopeClearCellRadius: 
* TelescopeAnalyzeRadius: 
* SteamTurbineEnabled: 
* SteamTurbineWattage: 
* SteamTurbineSourceElement: 
* SteamTurbineOutputElement: 
* SteamTurbinePumpRateKG: 
* SteamTurbineMaxSelfHeat: 
* SteamTurbineHeatTransferPercent: 
* SteamTurbineMinActiveTemperature: 
* SteamTurbineIdealTemperature: 
* SteamTurbineOutputTemperature: 
* SteamTurbineOverheatTemperature: 
* PipeThroughputPercent: 
* PipeLiquidMaxPressure: 
* PipeGasMaxPressure: 
* PipeValvePressureButtonShow: 
* PipeLiquidPump: 
* PipeGasPump: 
* ConveyorRailPackageSize: 
* ConveyorLoaderHasSlider: 
* ConveyorReceptacleHasSlider: 
* ConveyorLoaderAcceptLiquidsGas: 
* AutoSweeperCapacity: 
* AutoSweeperRange: 
* AutoSweeperSlider: 
* AutoSweeperPickupAnything: auto sweeper can move bottles
* RoboMinerWidth: 
* RoboMinerHeight: 
* RoboMinerOffset: 
* RoboMinerRegolithTurbo: robo miner digs regolith 6 times as fast
* RoboMinerDigThroughGlass: robo miner dig through glass tiles, but only from glass (not diamonds)
* RoboMinerDigAnyTile: 
* WireSmallWatts: 
* WireRefinedWatts: 
* WireHeavyWatts: 
* WireRefinedHeavyWatts: 
* TransitTubeAnywhere: transit tubes can be placed in the background; this is experimental, please provide bug reports
* TransitTubeUTurns: removes all transit tube layout restrictions
* TransitTubeJoulesPerLaunch: cost per launch, normally 10000 joules
* TransitTubeJouleCapacity: internal energy capacity, normally 40000 joules
* NoDupeGlobal: if false, disable this feature altogether
* NoDupeApothecary: buildings will not require a dupe to run
* NoDupeClothingFabricator: buildings will not require a dupe to run
* NoDupeCookingStation: buildings will not require a dupe to run
* NoDupeGourmetCookingStation: buildings will not require a dupe to run
* NoDupeEggCracker: buildings will not require a dupe to run
* NoDupeGlassForge: buildings will not require a dupe to run
* NoDupeMetalRefinery: buildings will not require a dupe to run
* NoDupeMicrobeMusher: buildings will not require a dupe to run
* NoDupeRockCrusher: buildings will not require a dupe to run
* NoDupeSuitFabricator: buildings will not require a dupe to run
* NoDupeSupermaterialRefinery: buildings will not require a dupe to run
* NoDupeSludgePress: buildings will not require a dupe to run
* NoDupeCompost: buildings will not require a dupe to run
* NoDupeDesalinator: buildings will not require a dupe to run
* NoDupeOilRefinery: buildings will not require a dupe to run
* NoDupeOilWellCap: buildings will not require a dupe to run
* NoDupeIceCooledFan: adds power plug for automated operation; cooling factor is different
* NoDupeRanchStation: grooming lasts for 100 cycles instead of 1
* NoDupeTelescope: 
* NoDupeAlgaeTerrarium: 
* SkillStationEnabled: if false, disable this feature altogether
* SkillStationCostTime: time dupe spends until process is done
* SkillStationCostReset: exp cost for resets (usually free)
* SkillStationCostRemoveTrait: exp cost to remove any trait
* SkillStationCostAddTrait: exp cost to add a new trait
* SkillStationCostBadTrait: exp cost to add a new negative trait
* SkillStationCostAddAptitude: exp cost to add an interest
* SkillStationCostAddAttribute: exp cost to permanently increase an attribute score
* TuningAtmosuitDecay: 
* TuningOxygenMaskDecay: 
* TuningAtmosuitAthletics: 
* TuningAtmosuitScalding: 
* TuningAtmosuitInsulation: 
* TuningAtmosuitThermalConductivityBarrier: 
* TuningLeadsuitRadiationShielding: 
* TuningLeadsuitAthletics: 
* TuningLeadsuitStrength: 
* TuningLeadsuitInsulation: 
* TuningLeadsuitThermalConductivityBarrier: 
* TuningMissionDurationScale: 
* TuningMassPenaltyExponent: 
* TuningMassPenaltyDivisor: 
* TuningResearchEvergreen: 
* TuningResearchBasic: 
* TuningAnalysisDiscovered: 
* TuningAnalysisComplete: 
* TuningAnalysisDefaultCyclesPerDiscovery: 
* TuningThrustCostsLow: 
* TuningThrustCostsMid: 
* TuningThrustCostsHigh: 
* TuningThrustCostsVeryHigh: 
* TuningClusterFowPointsToReveal: 
* TuningClusterFowDefaultCyclesPerReveal: 
* TuningEngineEfficiencyWeak: 
* TuningEngineEfficiencyMedium: 
* TuningEngineEfficiencyStrong: 
* TuningEngineEfficiencyBooster: 
* TuningOxidizerEfficiencyVeryLow: 
* TuningOxidizerEfficiencyLow: 
* TuningOxidizerEfficiencyHigh: 
* TuningCargoContainerMassStaticMass: 
* TuningCargoContainerMassPayloadMass: 
* TuningBurdenInsignificant: 
* TuningBurdenMinor: 
* TuningBurdenMinorPlus: 
* TuningBurdenModerate: 
* TuningBurdenModeratePlus: 
* TuningBurdenMajor: 
* TuningBurdenMajorPlus: 
* TuningBurdenMega: 
* TuningEnginePowerEarlyWeak: 
* TuningEnginePowerEarlyStrong: 
* TuningEnginePowerMidVeryStrong: 
* TuningEnginePowerMidStrong: 
* TuningEnginePowerMidWeak: 
* TuningEnginePowerLateWeak: 
* TuningEnginePowerLateStrong: 
* TuningFuelCostPerDistanceVeryLow: 
* TuningFuelCostPerDistanceLow: 
* TuningFuelCostPerDistanceMedium: 
* TuningFuelCostPerDistanceHigh: 
* TuningFuelCostPerDistanceVeryHigh: 
* TuningFuelCostPerDistanceGasLow: 
* TuningFuelCostPerDistanceGasHigh:
* BuildingBaseSettingGlobalFlag: if false, disable this feature altogether
* BuildingBaseSettings: can change a list of BuildingDef options of any building (even from other mods)
*    - this may be defined with either building ID or ingame name (case sensitive)
*    - null will keep the original setting
*    - all these settings are OPTIONAL and can be removed
*    - PowerConsumption: amount of power needed, some buildings will crash when set to 0 Watts, notably filters/shutoffs
*    - OverheatTemperature: temperature the building will stop working in Kelvin
*    - ExhaustKilowattsWhenActive: amount of heat released from the building (will heat building in vacuum instead)
*    - SelfHeatKilowattsWhenActive: amount of heat directly applied to the building;
*    - WARNING, if KilowattsWhenActive is a negative value and the temperature reaches below 0 Kelvin, the game will crash instantly
*    - GeneratorWattageRating: amount of power generated from building
*    - BaseCapacity: used for power-transformator, amout of power transformed
*    - BaseDecor: decor value
*    - BaseDecorRadius: range of decor value
*    - LocationRule: use number as seen below as to where the building is allowed to be placed
*        - null will keep the original setting
*        - Anywhere = 0,
*        - OnFloor = 1,
*        - OnFloorOverSpace = 2,
*        - OnCeiling = 3,
*        - OnWall = 4,
*        - InCorner = 5,
*        - Tile = 6,
*        - NotInTiles = 7,
*        - Conduit = 8,
*        - LogicBridge = 9,
*        - WireBridge = 10,
*        - HighWattBridgeTile = 11,
*        - BuildingAttachPoint = 12,
*        - OnFloorOrBuildingAttachPoint = 13,
*        - OnFoundationRotatable = 14
*    - Rotations:
*        - null will keep the original setting
*        - Unrotatable = 0,
*        - R90 = 1,
*        - R360 = 2,
*        - FlipH = 3,
*        - FlipV = 4
*    - Floodable: whenever or not a building works underwater
*    - IsFoundation: not sure what this does
*    - MaterialCategory: single string, separate multiple with spacebar; must match count of elements; each entry must be a valid Tag like: Metal, RefinedMetal, Alloy, BuildableRaw, BuildableProcessed, PreciousRock, Farmable, Plastic, Glass, Transparent, BuildingFiber, FlyingCritterEdible, or BuildableAny
*    - ConstructionMass: array of float; must match count of elements; how much kg are needed to build it
*    - ThermalConductivity: thermal multiplier, probably only works on buildings already messing with temperatures; 0..1 will reduce the temperature transfer, while 1..100 will increase it; normal settings are 0.01 for insulated tiles and 2 for radiant pipes
* BuildingAdvancedMachineMultiplier: a list of building IDs; will multiply all storage and element converting properties of a building, which might break some stuff, so be mindfull
* BuildingAdvancedGlobalFlag: if false, disable this feature altogether
* BuildingAdvancedMaterial: more complex patches to buildings
*    - Id: building ID
*    - Index: material index that should be modified; applies to all if null
*    - MaterialOverride: valid build elements to add for this building, multiple split with spacebar
* BuildingAdvancedOutputTemp: allows output temperature to be set; if the number of element do not match, it will take the first entry for all values; values of 0.0 will use the building temperature and otherwise the input element's temperature
* AdvancedSettings: not supported









































