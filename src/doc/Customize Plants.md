# Customize Plants

Now all plants and compatible with other mods! Remove or add as many irrigation/fertilizer as you like. Change illumination requirements, temperature, atmosphere, harvest rate and amount.

Settings found here (after running mod)
----------
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\CustomizePlants.json
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/CustomizePlants.json
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/CustomizePlants.json

Base changes
----------
Changeable in json file above!
* Mealwood 5kg dirt per cycle (original 10)
* Mushroom grows in 6 cycles, doesn't take slime but consumes CO2 to grow
* Bristle Blossom 5kg water per cycle (original 20kg)
* Waterweed 5kg salt water per cycle (original +500g bleach)
* Nosh Sprout 5kg ethanol (original 20kg +5kg dirt)
* Pincha Pepper 15kg polluted water per cycle (original 35kg)
* Arbor Tree 62.5kg polluted water (original 70kg)
* Wheezewort no irrigation (original Phosphorite)
* Bluff Briar filter chlorine from their surrounding up to 10g/s
* Mirth Leaf drop Algae and Clay, but take water to irrigate
* Jumping Joya filter natural gas from their surrounding up to 10g/s and convert it into CO2
* Sporechid exhale chlorine
* Gas Grass grows when in 100 Lux or more (instead of 20000 Lux)
* Wheezewort custom temperature delta of -20 Kelvin (normally -5 Kelvin)
* hanging flower vases and wall flower vase can be used for farming
* Oxyfern oxygen output changeable (default off)

These changes are primarily to demonstrate the possibilities. You can easily remove any change you don't like and add new ones.

List of Settings
----------
* PlantSettings
* - id: Required. Defines which plant is affected. See ID in readme file.
* - fruitId: Must be valid Tag or listed in SpecialCropSettings.
* - fruit_grow_time: Time for the crop to grow in seconds.
* - fruit_amount: Number of objects or amount in kg, depends on Tag.
* - irrigation: List of irrigation needed. May be any liquid or solid element. Amount in kg per cycle
* - illumination: If equal 0 removes existing component. If less than 0 requires darkness. Otherwise number is light threshold required.
* - safe_elements: List of gas elements plant has to be in. If empty all elements are suitable.
* - temperatures: Array of temperatures in Kelvin. 1) death if lower 2) wilt if lower 3) wilt if higher 4) death if higher; entries after 4 are ignored; may have less than 4 entries
* - pressures: Array of pressures in kg. 1) death if lower 2) wilt if lower 3) wilt if higher 4) death if higher; entries after 4 are ignored; may have less than 4 entries
* - decor_value: Decor score.
* - decor_radius: Range at which the decor score is applied to.
* - submerged_threshold: If equal 0 ignores water. If less than 0 hates water. If higher than 0 needs water. Plant will wilt in bad conditions.
* - can_tinker: Whenever plant can be interacted with farming station.
* - require_solid_tile: Not sure...
* - max_age: If 0 or less, will never auto-harvest itself. Otherwise time in seconds for the plant to auto-harvest and plays bristled animation when at 50%+.
* - disease: Type of disease the plant spreads around it. May be: FoodPoisoning, SlimeLung, PollenGerms, or ZombieSpores.
* - disease_amount: How much disease is spread around it.
* - input_element: Type of gas or liquid plant absorbs from environment. Not compatible with Wheezewort or Oxyfern.
* - input_rate: Amount absorbed per second.
* - output_element: Type of gas or liquid plant expels per second. Not compatible with Wheezewort or Oxyfern.
* - output_rate: Amount expelled per second.
* - radiation_threshold_min: Minimum radiation threshold.
* - radiation_threshold_max: Maximum radiation threshold.
* SpecialCropSettings: Defines a placeholder name and a list of resources. When a plant tries to spawn a matching fruitId it will instead use the list provided. Note that this is only necessary when dealing with more than one resource at once.
* SeedsGoIntoAnyFlowerPots: Whenever or not all seeds go into any flower pots / farm plots.
* WheezewortTempDelta: How much cooling wheezeworts do. Default is -5 Kelvin.
* OxyfernOxygenPerSecond: Amount of oxygen released by oxyferns.
* CheatMutationAnalyze: Immediately reveals mutations.
* CheatFlowerVase: When true, the basic Flower Pot for decoration plants does not need any irrigation at all, no matter which plant is in it.
* WildFlowerVase: When true, the basic Flower Pot grows plants as if they were wild.
* AutomaticallyAddModPlants: When true, will automatically add any plant to the mod, that registers as a plant via ExtendEntityToBasicPlant function. Requires restart after plant was found.
* IgnoreList: List of plants that shouldn't be modified. Does not apply to ModPlants.
* ModPlants: List of classes to patch. This may be extended manually or by enabling AutomaticallyAddModPlants.

GitHub
If Steam Download does not work or you need an older version, you find the files also here: [GitHub](https://github.com/Truinto/ONI-Modloader-SimpleMods/tree/master/Mods/Customize%20Plants)


Notes to unknown IDs:
The mod will print a list of all found plants to Player.log

Notes to patch order:
CustomizePlants will try to modify the plant early while it is generated. This works fine for all the game's plants. Plants added from mods may be not fully compatible, depending on how the code was structured. In that case, activate AutomaticallyAddModPlants and boot the game once. It will generate ModPlants. From here you can pick which plants should be patched early or late. It's patched early, if it's not on IgnoreList. It's patched late, if its class is present in ModPlants. In theory you can patch it early and late, but that's pointless. AutomaticallyAddModPlants disables itself after it ran and added at least one entry.


Notes to PlantLookup:
- the disadvantage is the assembly can be hard to find, e.g. if the author renamed the assembly (mostly suffix '-merged' due to ILMerge)
