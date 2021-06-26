README V1

Settings here:
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\CustomizeCritter.json
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/CustomizeCritter.json
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/CustomizeCritter.json

IMPORTANT:
- For the main categories of settings (critter_settings, egg_settings, trait_settings, and egg_modifiers) any missing option is ignored (except for id).
- If you use an id that doesn't already exists in the game, then a new type will be generated to accommodate it. Missing options for these will be set to an default value, if it would otherwise be guaranteed to cause a crash. These defaults might not fit the critter you indented to create at all.
- A lot of settings are not fully implemented and don't work. Settings marked with an asterisk are very likely to cause issues. Try to avoid them.

General settings:
- debug: verbose log output
- print_all: If this flag is true during bootup, it will search for all registered entities (including from other mods) and overwrite critter_settings, egg_settings, trait_settings, and egg_modifiers with their extracted default values. Any previous modifications you made to these settings will be lost during this process.
- print_verbose: During the 'print_all' procedure, it will print the chore_table as well (which doesn't work currently).
- alwaysHungry: Any food intake will have critters stomach set to 50%, causing them to constantly eat.
- cantStarve: Tame critters cannot starve.
- eggWildness: Disabled if negativ. Otherwise increase wildness on newly hatched eggs by this percentage (0.0 to 100.0).
- babyGrowupTime_inDays: Disabled if 0 or negative. Otherwise time for babies to mature into adulthood in days.
- acceleratedLifecycle: Reduces incubation time and fertility time to almost 0. Caution: This is only for testing. Critters will multiply exponentially, causing huge lag and crashes after a couple minutes.


critter_settings: (properties of all critter types, including robots)
- id: unique id of critter
- name: display name
- desc: displayed description
- *anim_file: kanim of critter
- *is_baby: whenever of not a critter is a baby; affects growing up (don't use in conjunction with is_adult)
- *is_adult: whenever of not a critter is an adult; affects reproduction (don't use in conjunction with is_baby)
- traitId: id of trait to be used (may define new trait in trait_settings)
- *override_prefix: prefix of KAnim and CreatureBrain (behavior)
- space_requirement: tile needed before critter is overcrowded
- lifespan: negative values makes critter immortal; positive values do nothing; for actual life time see traits
- *mass: weight of critter
- *width: horizontal space
- *height: vertical space
- decor: range and amount of decor
- *navGridName: logic on how critter tries to move
-- WalkerNavGrid1x1
-- WalkerBabyNavGrid
-- DreckoNavGrid
-- DreckoBabyNavGrid
-- FlyerNavGrid1x1
-- FlyerNavGrid2x2
-- DiggerNavGrid
-- FloaterNavGrid
-- SwimmerNavGrid
-- SquirrelNavGrid
- *navi:
-- Floor
-- Hover
-- Swim
- moveSpeed: speed of movement
- dropOnDeath: list of items dropped after death (standard weight per unit)
- *canDrown: whenever a critter can breath liquids
- *canCrushed: whenever a critter dies when burrowed by sand
- *canBurrow: whenever a critter burrows at night
- *canTunnel: whenever a critter can burrow through tiles
- *canFall: whenever a critter is affected by gravity
- *canHoverOverWater: whenever a critter floats on liquids (overrides canFall)
- tempLowDeath: low temperature limit before the critter dies
- tempLowWarning: comfortable temperature range
- tempBorn: temperature at which the critter spawns from an egg
- tempHighWarning: comfortable temperature range
- tempHighDeath: high temperature limit before the critter dies
- pickup_only_from_top: perhaps animation relevant
- pickup_allow_mark: whenever can be marked for bagging (otherwise trap needed)
- pickup_use_gun: perhaps animation relevant
- *tags: list of tags to be added to critter ("R:SomeTag" removes existing tags, "C:" clears all tags)
- *faction: group the critter belongs to
-- Prey
-- Pest
- *species: creature type the critter belongs to (interacts with other logics)
- lures: list of elements the critter can be lured by
- attackValue: attack value; note: whenever a critter uses this attack is dependend on the chore_table
- *chore_table: NOT WORKING! "chore_table" is a list of logic components applied to the critter; if automatically_populate is true, the mod will fill out missing components automatically; Sweepbot is disabled because of bugs; note: use print_all and print_verbose to extract the chore_table; note2: due to bugs a lot doesn't work as intended, use OVERRIDE_ID to copy a working chore_table from base creature
- adultId: only is_baby; critter the baby will evolve into
- *eggId: only relevant for discover (so it shows up in menus)
- *babyId: only relevant for discover (so it shows up in menus)
- dropOnMature: only is_baby; item drop when critter evolves
- fertility_cycles: only is_adult; time between laying eggs
- egg_chances: table of eggs and how likely they are (modifiers defined in egg_modifiers)
- is_ranchable: whenever a critter can be groomed
- canBeNamed: whenever a critter can be renamed (like a sweepbot)
- min_poop_calories: how often a critter relieves itself
- diet_list: list of diets
-- consumedTags: list of elements for this diet
-- producedTag: output element of this diet
-- caloriesPerKg: how many calories this diet is worth per KG of food
-- producedConversionRate: rate of conversion in percent (0.0 to 1.0)
-- diseaseId: name of disease produced (FoodPoisoning, SlimeLung, PollenGerms, ZombieSpores)
-- diseasePerKgProduced: amount of disease produced (if diseaseId given)
- feedsOnPickupables: true if eats plants or solids; false if eats gases or liquids
- light:
-- color: color by RGB with alpha channel (transparency)
-- range: size of light (normal 5)
-- lux: brightness of light
- scales:
-- drop: item dropped when sheared
-- mass: amount of item dropped
-- growthRate: speed of scales growth (in progress per second; done when 1.0 is reached)
-- atmosphere: gas type the scales grow in ("Void" will grow in any gas)
-- levelCount: unknown?
- expulsion: (as Morbs do)
-- element: gas/liquid expulsed
-- probability: probability of expulsion (0.0 to 100.0)
-- cellTargetMass: unknown?
-- massPerDirt: no idea?
-- onDeath: mass dropped when killed
-- diseaseId: disease released (FoodPoisoning, SlimeLung, PollenGerms, ZombieSpores)
-- diseaseAmount: amount od disease released


egg_settings: (properties of all eggs)
- eggId: unique id for egg; new eggs can have any name and can be used in critter_settings and egg_modifiers
- eggName: display name
- eggDesc: displayed description
- anim_egg: kanim of egg; custom kanims must be loaded with a different mod (to my knowledge none avaiable)
- egg_mass: weight of eggs
- babyId: critter that hatches from the egg
- incubation_cycles: number of cycles until the egg hatches
- egg_sortorder: sorted by number, then alphanumerical
- is_fish: whenever of not the egg is from a fish
- egg_scale: how large the egg is rendered


trait_settings: (properties of all traits, including critter and duplicant traits)
Some of these are for duplicants. You can change them, but a bunch of them have no editable settings. Although you can still add new attributes to these traits as normal.
- traidId: unique name of trait; new traits can have any name and can be used in critter_settings
- name: display name
- attributes: list of attribute IDs and their value; multiplier defines whenever the value is summed or multiplied; interesting attributes below:
-- CaloriesMax: Stomach size of critter.
-- CaloriesDelta: Amount of calories lost per tick.
-- HitPointsMax: Maximal amount of hitpoints.
-- AgeMax: Amount of cycles before critter dies. Values ≤ 0 causes them to instantly die.
-- AirConsumptionRate: Only dupes; amount of oxygen used per second.


egg_modifiers: (modifiers of egg probabilities; only one TYPE can be used at once, but you can set up multiple modifiers for the same eggTag)
- id: unique name of modifier; new modifier can have any name
- eggTag: egg that will be more (or less) likely; applies to all critters that can lay this egg
- weight: amount of change per update; this option varies greatly between different types
- foodTags: FOOD TYPE: list of elements that apply [weight] when eaten
- nearbyCreature: NEARBY TYPE: creature that applies [weight] when nearby
- minTemperature and maxTemperature: TEMPERATURE TYPE: applies [weight] when critter is in that temperature range
- alsoInvert: only works for NEARBY TYPE and TEMPERATURE TYPE; in addition to normal behavior, also inverts the [weight] when condition is not met

wildEffect, tameEffect, happyWildEffect, happyTameEffect, unhappyWildEffect, unhappyTameEffect:
Similiar to traits, these settings control attribute modifiers on happy, unhappy, wild, or tame critters. They take the same attribute ids. wildEffect and tameEffect are only applied, if the critter ID is present in critter_settings.

* may not work



