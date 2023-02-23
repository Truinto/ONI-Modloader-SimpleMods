Customize Geyser

Let's you add or edit all kinds of geysers/vents/volcanoes.

Settings found here (after running mod)
----------
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\Customize Geyser.json
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/Customize Geyser.json
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/Customize Geyser.json

Custom Geysers
----------
- this includes geysers not normally occurring in the game
- any new custom geyser has a chance to spawn in place of a normally generated geyser; [s]this works for existing saves as long as the geyser is in the fog of war[/s] since the DLC geysers are mostly predetermined 
- after uninstalling, if you continue a save with a custom geyser spawned, the affected geyser will get removed (since it cannot be loaded by the game); normal geysers are unaffected by this

Existing Geysers
----------
- this includes geysers normally occurring with or without mod
- all changes are retroactively
- no risk of deleting the geyser
- uninstalling the mod will revert the geyser to its former properties

Default additions
----------
- Ethanol Geyser
- Aluminum Volcano
- Tungsten Volcano
- Steel Volcano
- Glass Volcano
- Super Coolant Geyser

Worldgen changes
----------
[s]If you get the world trait "Volcanoes", there is a chance to get any volcano instead of just magma volcanoes. Does not work with existing saves.[s/] This feature is discontinued since the DLC.

Configurating the mod
----------
If you want to change the properties of geysers, look for the config file here:
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\Customize Geyser.json
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/Customize Geyser.json
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/Customize Geyser.json
For existing geysers, all properties but id are optional. For the id, look at the default geyser IDs.txt. The Name and Description cannot be changed, this must be done by translation-mod.
For new geysers, all properties but id and element are optional, although it will take average values for missing properties.

Randomize Settings
----------
The table below sets weights of how likely certain geysers occur. Geysers with 0 weight will stop spawning altogether. This does not allow you to change the type of geysers retroactively.
- RandomizerEnabled: if set to false will disable all other randomize settings
- RandomizerUsesMapSeed: if set to true, the same geysers will spawn for a certain set of settings; you still get different results when you change the weights or add new geyser types; if set to false, reloading and rediscovery a geyser may reveal a different geyser
- RandomizerRerollsCycleRate: if set to true and RandomizerUsesMapSeed set to false will also change the percentage of cycle output; otherwise output stays consistent
- RandomizerPopupGeyserDiscoveryInfo: generates a popup whenever a geyser is discovered; useful for rerolling/testing
Randomize settings do not affect pre-defined geysers, which are some steam, methane, and oil geysers.

Morph Settings
----------
Allows you to change the type of existing geysers into any other type.
- GeyserMorphEnabled: When enabled, shows two buttons in the geyser menu. The first will request a scientist dupe to work on it, the second will define which geyser it should be morphed into. Click the second button to cycle through the options.
- GeyserMorphWorktime: How long a scientist dupe will need to work on the geyser to morph it.

[Github](https://github.com/Truinto/ONI-Modloader-SimpleMods/tree/master/Mods/Customize%20Geysers)
