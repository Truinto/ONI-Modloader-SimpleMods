README V6

Settings here:
%userprofile%\documents\Klei\OxygenNotIncluded\mods\Steam\1818145851\CustomizeBuildingsState.json

or maybe here (e.g. if you are not playing on Windows):
Steam\SteamApps\common\OxygenNotIncluded\Mods\LessIrrigation\Config\LessIrrigationState.json

The settings work with ingame IDs. These differ from the displayed name!!!

-- Plant ------- plant id, use in json file! ----
Mealwood        BasicSingleHarvestPlant
Dusk Cap        MushroomPlant
Bristle Blossom PrickleFlower
Sleet Wheat     ColdWheat
Waterweed       SeaLettuce
Nosh Sprout     BeanPlant
Pincha Pepper   SpiceVine
Balm Lily       SwampLily
Thimble Reed    BasicFabricPlant
Arbor Tree      ForestTree
Dasha Saltvine  SaltPlant
Gas Grass       GasGrass
Oxyfern         Oxyfern
Wheezewort      ColdBreather
Bluff Briar     PrickleGrass
Buddy Bud       BulbPlant
Mirth Leaf      LeafyPlant
Jumping Joya    CactusPlant
Sporechid       EvilFlower

-- Fruit ------- Fruit id, use in json file! ----
Meal Lice           BasicPlantFood
Mushroom            Mushroom
Bristle Berries     PrickleFruit
Sleet Wheat Grain   ColdWheatSeed
Lettuce             Lettuce
Nosh Bean           BeanPlantSeed
Pincha Peppernut    SpiceNut
Balm Lily Flower    SwampLilyFlower
Reed Fiber          BasicFabric
Lumber              WoodLog
Salt                Salt
Gas Grass Fruit     GasGrassHarvested

-popular elements - ids, use in json file! --
Water            Water
Dirt             Dirt
Slime            SlimeMold
Salt Water       SaltWater
Bleach Stone     BleachStone
Ethanol          Ethanol
Phosphorite      Phosphorite
Polluted Water   DirtyWater
Sand             Sand
Liquid Chlorine  Chlorine
Oxygen           Oxygen
Polluted Oxygen  ContaminatedOxygen
CO2              CarbonDioxide
Gas Chlorine     ChlorineGas
Hydrogen         Hydrogen


Groups:
RemoveIrrigationFromPlants:
 - any matching plant ID will not load the default irrigation
 - if you want to add irrigation to a plant that has a default irrigation, then it must be added here
AddIrrigiation:
 - adds tags to matching plant IDs
 - liquids will be automatically set to irrigation, everything else to fertilization
 - consumption is per cycle
 - note that if you use more than one liquid you cannot use the Hydroponic Farm, other farms work
SetIllumination:
 - any matching plant ID will change the illumination setting
 - -1 = needs darkness, 0 = works in either, 1 = needs light
SetPlantTempAtmosphere:
 - change temperature and atmosphere
 - warning high and low are thresholds where a plant grows
 - lethal high and low are thresholds where a plant turns back into a seed
 - safe_elements is a list of gases the plant can grow in
 - pressure is about the minimum gas pressure the plant will grow in
 - can_drown is whenever a plant grows in liquids or not (unconfirmed)
 - can_tinker is whenever a plant can receive the Farmer's Touch buff (unconfirmed)
 - require_solid_tile ???
 - max_age in seconds is how long it takes after the plant is fully grown to wilt and drop the fruit automatically
SetPlantCrop:
 - cropId must equal the name above it
 - cropDuration = time in seconds until harvest is ready
 - numProduced = number of crops ready per harvest

Tip: Define completely new crop; right now only works on plants that have crop anyway; crop can be any entity or element; simply create a new entry in SetPlantCrop with that elements name and id, then use SetPlantTempAtmosphere to set crop_id to said id
Example, will drop 10kg algae every 600 seconds from bristle blossoms
   "SetPlantTempAtmosphere": {
    "PrickleFlower": {
      "temperature_lethal_low": 218.15,
      "temperature_warning_low": 273.15,
      "temperature_warning_high": 308.15,
      "temperature_lethal_high": 398.15,
      "safe_elements": [
        "Oxygen",
        "ContaminatedOxygen",
        "CarbonDioxide"
      ],
      "pressure_sensitive": true,
      "pressure_lethal_low": 0.0,
      "pressure_warning_low": 0.1,
      "crop_id": "Algae",
      "can_drown": true,
      "can_tinker": true,
      "require_solid_tile": true,
      "should_grow_old": true,
      "max_age": 2400.0
    }
  },
  "SetPlantCrop": {
    "Algae": {
      "cropId": "Algae",
      "cropDuration": 600.0,
      "numProduced": 10,
      "renewable": true
    }
  }