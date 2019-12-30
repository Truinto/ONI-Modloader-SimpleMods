README V7

Settings here:
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\Customize Plants.json
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/Customize Plants.json
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/Customize Plants.json

The settings work with ingame IDs. These differ from the displayed name!

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
Arbor Tree      ForestTreeBranch (branches are their own plant)
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
Elements are valid as fruitId as well

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
For a full list see element IDs.txt

Groups:
- PlantSettings
--  id: Required. Defines which plant is affected. See ID in first table above.
--  fruitId: Must be valid Tag or listed in SpecialCropSettings.
--  fruit_grow_time: Time for the crop to grow in seconds.
--  fruit_amount: Number of objects or amount in kg, depends on Tag.
--  irrigation: List of irrigation needed. May be any liquid or solid element. Amount in kg per cycle
--  illumination: If equal 0 removes existing component. If less than 0 requires darkness. Otherwise number is light threshold required.
--  safe_elements: List of gas elements plant has to be in. If empty all elements are suitable.
--  temperatures: Array of temperatures in Kelvin. 1) death if lower 2) wilt if lower 3) wilt if higher 4) death if higher; entries after 4 are ignored; may have less than 4 entries
--  pressures: Array of pressures in kg. 1) death if lower 2) wilt if lower 3) wilt if higher 4) death if higher; entries after 4 are ignored; may have less than 4 entries
--  submerged_threshold: If equal 0 ignores water. If less than 0 hates water. If higher than 0 needs water. Plant will wilt in bad conditions.
--  can_tinker: Whenever plant can be interacted with farming station.
--  require_solid_tile: Not sure...
--  max_age: If 0 or less, will never auto-harvest itself. Otherwise time in seconds for the plant to auto-harvest and plays bristled animation when at 50%+.
--  disease Type: of disease the plant spreads around it. May be: FoodPoisoning, SlimeLung, PollenGerms, or ZombieSpores.
--  disease_amount: How much disease is spread around it.
--  input_element: Type of gas or liquid plant absorbs from environment. Not compatible with Wheezewort or Oxyfern.
--  input_rate: Amount absorbed per second.
--  output_element: Type of gas or liquid plant expels per second. Not compatible with Wheezewort or Oxyfern.
--  output_rate: Amount expelled per second.
- SpecialCropSettings: Defines a placeholder name and a list of recourses. When a plant tries to spawn a matching fruitId it will instead use the list provided. Note that this is only necessary when dealing with more than one recourse at once.
- SeedsGoIntoAnyFlowerPots: Whenever or not all seeds go into any flower pots / farm plots.
- WheezewortTempDelta: How much cooling wheezeworts do. Default is -5 Kelvin.
- OxyfernOxygenPerSecond: Amount of oxygen released by oxyferns.
- CheatFlowerVase: When true, the basic Flower Pot for decoration plants does not need any irrigation at all, no matter which plant is in it.
- AutomaticallyAddModPlants: When true, will automatically add any plant to the mod, that registers as a plant via ExtendEntityToBasicPlant function. Might require restart after plant was found.
- ModPlants: List of classes to patch besides the default plants. This may be extended manually or by enabling AutomaticallyAddModPlants.

