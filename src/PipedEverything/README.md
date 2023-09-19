Adds liquid, gas, and solid pipes to buildings. Settings can be configured. Can work for modded buildings, but requires editing the settings.

Default settings include all ports from 'Piped Output' and more. If a pipe is connected but blocked, it will store materials internally until 'StorageCapacity' is exceeded. Then it will disable the building.

### Settings here
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\PipedEverything.json \
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/PipedEverything.json \
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/PipedEverything.json

### Format
* Id: string | Building ID or exact localized string (including spaces)
* Input: bool | If true act as input, otherwise as output
* OffsetX: int | Port's X position relative to origin cell
* OffsetY: int | Port's Y position relative to origin cell
* Filter: string[] | Collection of SimHashes that can pass through the port (tags NOT allowed)
* Color: Color32 | Port's color scheme like "Color":{"r": 128,"g": 128,"b": 128,"a": 255}, default based on SimHashes
* StorageIndex: int | Which storage to use, default 0
* StorageCapacity int | How much of each element can be stored, default 100kg (except gas with 2kg)

### Source
[Github](https://github.com/Truinto/ONI-Modloader-SimpleMods) \
Thanks to Nightinggale for the ground work of this project. [Link](https://github.com/Nightinggale/ONI-mods/tree/master)

### Notes
* Filter accepts the string 'Solid', 'Liquid', and 'Gas' for ports; this will process any element of that type
