# Customize Elements

Base changes
----------
Changeable in json file below!
* Oxygen is slightly lighter, causing it to float over Contaminated Oxygen
* Metals can be substituted for Ores while building
* Abyssalite can be used for most buildings

Settings found here (after running mod)
----------
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\CustomizeElements.json
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/CustomizeElements.json
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/CustomizeElements.json

List of Settings
----------
* Id: name of element to be changed
* specificHeatCapacity: Heat Capacity
* thermalConductivity: Thermal Conductivity
* molarMass: weight of element
* strength: no idea?
* flow: no idea?
* maxMass: pressure before walls are cracked
* maxCompression: no idea?
* viscosity: no idea?
* minHorizontalFlow: no idea?
* minVerticalFlow: no idea?
* solidSurfaceAreaMultiplier: no idea?
* liquidSurfaceAreaMultiplier: no idea?
* gasSurfaceAreaMultiplier: no idea?
* hardness: how difficult to mine
* lowTemp: temperature it freezes/condenses
* lowTempTransitionTarget: what element it changes to
* lowTempTransitionOreID: what ore should be produced when freezing/condensing
* lowTempTransitionOreMassConversion: ratio of ore to split
* highTemp: temperature it melts/evaporates
* highTempTransitionTarget: what element it changes to
* highTempTransitionOreID: what ore should be produced when melting/evaporating
* highTempTransitionOreMassConversion: ratio of ore to split
* sublimateId: what element should this sublimate to
* convertId: probably off-gas element
* sublimateFX: what FX to play
* sublimateRate: how much to sublimate
* sublimateEfficiency: how much is lost during sublimation
* sublimateProbability: how often does the sublimation occur
* offGasPercentage: probably how often off-gas occurs
* lightAbsorptionFactor: how much does this tile absorb light
* radiationAbsorptionFactor: how much does this tile absorb radiation
* radiationPer1000Mass: mh?
* toxicity: probably not used
* materialCategory: main category
* oreTags: list of sub categories, used for building
* buildMenuSort: index in building menu

Note
These options are identical with the element-yaml files: SteamApps\common\OxygenNotIncluded\OxygenNotIncluded_Data\StreamingAssets\elements
Main benefit of this mod is that you don't need to edit the assets every update.

GitHub
If Steam Download does not work or you need an older version, you find the files also here: [GitHub](https://github.com/Truinto/ONI-Modloader-SimpleMods/tree/master/Mods/)

