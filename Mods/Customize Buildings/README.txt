README v16

Settings Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\CustomizeBuildings.json
Settings MacOS: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/CustomizeBuildings.json
Settings Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/CustomizeBuildings.json

If CustomizeBuildings.json is corrupted or otherwise does not work, delete it and restart the game. A new clean file will be generated.

KleiSettings.json contains the settings necessary to essentially turn off all features. You can work from there and re-enable certain settings as you like.
Otherwise the settings are self explanatory.

Special settings:
  "ElectrolizerMaxPressure": pressure the elecrolizer will stop producing oxygen/hydrogen
  "AirfilterDropsCanisters": on deconstruction, airfilters (as well as all other buildings) drop gas canisters instead of venting stored gases
  "NewRecipeRockCrusher": adds regolith to sand recipe
  "ReservoirNoGround": reservoirs can be placed in the air
  "ReservoirManualDelivery": dupes may store material in reservoirs (off by default)
        - dupes will deliver selected liquids/gases until the capacity is at the slider amount
        - liquid/gas pipes can still deliver any element and will ignore the slider limit
        - activating, then deactivating an element checkbox drops it on the floor, for easy removal of rogue elements
        - furthermore reservoirs get logic output, outputing green when capacity is over slider amount
  "NoDupeValves": valves are set instantly without dupe interaction
  "NoDupeSwitches": same for switches
  "NoDupeToogleBuildings": and buildings
  "NoDupeToogleDoors": and door settings
  
  "ScannerInterferenceRadius": radius looking for heavy machinery and free sky tiles
  "ScannerWorstWarningTime": worst time before a network will detect incoming meteors
  "ScannerBestWarningTime": best time before a network will detect incoming meteors
  "ScannerBestNetworkSize": amount of scanners needed for best warning time
  "LadderCometInvincibility": comets will no longer deal damage to standard ladders (does not include plastic ones)
  
  "RoboMinerDigThroughGlass": Robo Miner dig through glass tiles, but only from glass (not diamonds)

  "TransitTubeAnywhere": transit tubes can be placed in the background; this is experimental, please provide bug reports
  "TransitTubeUTurns": removes all transit tube layout restrictions
  "TransitTubeJoulesPerLaunch": cost per launch, normally 10000 joules
  
  "NoDupeBuildings": buildings will not require a dupe to run
Following settings are not implemented yet:
    "Ice Fan": if set to true, will replace the dupe requirement with 120W power inlet; ice consumption different
    "Grooming Station": if set to true, will buff the standard duration of grooming to 100 cycles
    "Shearing Station": false,
    "Automated Compost": false,

  "BuildingBaseSettings": can change a list of BuildingDef options of any building (even from other mods)
	- this may be defined with either building ID or ingame name (case sensitive)
	- null will keep the original setting
	- all these settings are OPTIONAL and can be removed
	PowerConsumption: amount of power needed, some buildings will crash when set to 0 Watts, notably filters/shutoffs
	OverheatTemperature: temperature the building will stop working in Kelvin
	MaterialCategory: if present this must be one or two entries depending if the original building is building with 1 or 2 materials; each entry must be a valid Tag like: Metal, RefinedMetal, Alloy, BuildableRaw, BuildableProcessed, PreciousRock, Farmable, Plastic, Glass, Transparent, BuildingFiber, FlyingCritterEdible, or BuildableAny
	ExhaustKilowattsWhenActive: amount of heat released from the building (will heat building in vacuum instead)
	SelfHeatKilowattsWhenActive: amount of heat directly applied to the building
	WARNING: if kilowattsWhenActive is a negative value and the temperature reaches below 0 Kelvin, the game will crash instantly
	GeneratorWattageRating: amount of power generated from building
	BaseCapacity: used for power-transformator, amout of power transformed
	BaseDecor: decor value
	BaseDecorRadius: range of decor value
	LocationRule: use number as seen below as to where the building is allowed to be placed:
		null will keep the original setting
		Anywhere = 0,
		OnFloor = 1,
		OnFloorOverSpace = 2,
		OnCeiling = 3,
		OnWall = 4,
		InCorner = 5,
		Tile = 6,
		NotInTiles = 7,
		Conduit = 8,
		LogicBridge = 9,
		WireBridge = 10,
		HighWattBridgeTile = 11,
		BuildingAttachPoint = 12,
		OnFloorOrBuildingAttachPoint = 13,
		OnFoundationRotatable = 14
	Rotations:
		null will keep the original setting
		Unrotatable = 0,
		R90 = 1,
		R360 = 2,
		FlipH = 3,
		FlipV = 4
	Floodable: whenever or not a building works underwater
	IsFoundation: not sure what this does
	ConstructionMass: how much kg are needed to build it
	ThermalConductivity: thermal multiplier, probably only works on buildings already messing with temperatures; 0..1 will reduce the temperature transfer, while 1..100 will increase it; normal settings are 0.01 for insulated tiles and 2 for radiant pipes
  "MachineMultiplier": works with the same ID as above; will multiply all storage and element converting properties of a building, which might breaks some stuff, so be mindfull
