# Customize Recipe

Let's you change existing recipes or add new ones. This works on any buildings that have a recipe side-screen. The most common use would be to change the input or output amounts for specific recipes.

Settings found here (after running mod)
----------
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\CustomizeRecipe.json
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/CustomizeRecipe.json
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/CustomizeRecipe.json

List of Settings
----------
* CheatFast: If true, all recipes will be speeded up.
* CheatFree: If true, all recipes need no materials.
* AllowZeroInput: Set this to true, if you plan to set ingredients to 0kg. This will prevent the game from crashing.
* RecipeSettings
* - Id: Ingame ID of specific recipe. Can be omitted, if building+inputs+outputs is specified AND is identical to the original recipe (only amounts changed). If Id is omitted and no matching original was found, a new recipe will be created instead.
* - Building: Building at which the recipe can be used.
* - Inputs: List of ingredients.
* - Outputs: List of results.
* - Time: Time needed to craft one recipe.
* - HEP: High Energy Particle, only valid on buildings that have these; will cause crash otherwise
* - Description: Only really useful for new custom recipes.
* RecipeElement
* - material: tag of element or item related to the recipe
* - amount: amount in kg related to the recipe
* - temperatureOperation: AverageTemperature (average of input), Heated (allowed on some buildings, fixed output), Melted (only with liquid outputs)
* - storeElement: whenever or not the material is stored in the building or dumped in the environment 
* - inheritElement: only some recipes can uses this, namely suits

List of Buildings
----------
Apothecary
ClothingFabricator
CookingStation
CraftingTable
DiamondPress
EggCracker
GlassForge
GourmetCookingStation
Kiln
MetalRefinery
MicrobeMusher
RockCrusher
SludgePress
SuitFabricator
SupermaterialRefinery
UraniumCentrifuge

Build your Id
----------
* new recipes can have any ID
* if you want to replace an existing recipe and change the inputs/outputs, the Id is build like this:
  Rock Crusher recipe converts fossil to lime and sedimentary stone: RockCrusher_I_Fossil_O_Lime_SedimentaryRock
  Apothecary recipe converts water and dirt to mud and algae: Apothecary_I_Water_Dirt_O_Mud_Algae

GitHub
If Steam Download does not work or you need an older version, you find the files also here: [GitHub](https://github.com/Truinto/ONI-Modloader-SimpleMods/tree/master/Mods/Customize%20Recipe)