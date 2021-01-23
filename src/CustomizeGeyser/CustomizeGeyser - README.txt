Let's you add or edit all kinds of geysers/vents/volcanos. As well as setting probability or max amount of geysers.

Custom Geysers
- any new custom geyser has a chance to spawn in place of a normally generated geyser; this works for existing saves as long as the geyser is in the fog of war
- after uninstalling, if you continue a save with a custom geyser spawned, the affected geyser will get removed (since it cannot be loaded by the game); normal geysers are unaffected by this

Existing Geysers
- all changes are retroactively
- no risk of deleting the geyser
- uninstalling the mod will revert the geyser to its former properties

Default additions
- Ethanol Geyser
- Aluminum Volcano
- Tungsten Volcano
- Glass Volcano
- Steel Volcano
- Super Coolant Geyser

Worldgen changes
If you get the world trait "Volcanoes", there is a chance to get any volcano instead of just magma volcanoes. Does not work with existing saves.

Configurating the mod
If you don't like the worldgen changes, simple delete the wordgen folder from the mod.
If you want to change the properties of geysers, look for the config file here:
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\Customize Geyser.json
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/Customize Geyser.json
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/Customize Geyser.json
For existing geysers, all properties but id are optional. For the id, look at the default geyser IDs.txt.
For new geysers, all properties but id and element are optional, although it will take average values for missing properties.

Randomize Setting
The table below sets weights of how likely certain geysers occur. Geysers with 0 weight will stop spawning altogether. This does not allow you to change the type of geysers retroactively. Randomize settings do not affect pre-definied geysers, which are some steam, methane, and oil geysers.
- RandomizerEnabled: if set to false will disable all other randomize and Highlander settings
- RandomizerUsesMapSeed: if set to true, the same geysers will spawn for a certain set of settings; you still get different results when you change the weights or add new geyser types; if set to false, reloading and rediscovering a geyser may reveal a different geyser
- RandomizerRerollsCycleRate: if set to true and RandomizerUsesMapSeed set to false will also change the percentage of cycle output; otherwise output stays consistent
- RandomizerPopupGeyserDiscoveryInfo: generates a popup whenever a geyser is discovered; useful for rerolling/testing

Highlander - There can only be ONE
The Randomize table will also determine how many geysers of each type may exist in any one world. If everything is set to 1, then there will be no duplicates. Will not affect basic steam and methane geysers, unless Retroactive mode is also enabled.
- RandomizerHighlanderMode: IMPORTANT, will use this mode if either this is enabled OR your base has "Highlander" in its name
- RandomizerHighlanderRetroactive: if enabled will also affect geysers that are already spawned; this means geysers might get changed to accommodate the table's amount settings

Morphing Geysers
For convenience you may change geysers into other geysers. This is clearly a cheat, but may be needed if you want to remove entries. Remember, the game will permanently delete any geysers it does not know, so if you remove entries the corresponding geyser vanishes.
- GeyserMorphEnabled: When enabled, shows two buttons in the geyser menu. The first will request a scientist dupe to work on it, the second will define which geyser it should be morphed into. Click the second button to cycle through the options.
- GeyserMorphWorktime: time in seconds a scientist dupe will need to work on the geyser to morph it

Github
For manual download and help files.
https://github.com/Truinto/ONI-Modloader-SimpleMods/tree/master/Mods/Customize%20Geysers