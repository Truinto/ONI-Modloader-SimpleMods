Have storage bins and refrigerators share their contents. A background task will periodically check items and evenly spread them to eligible storages.

### Settings here
Windows: %userprofile%\documents\Klei\OxygenNotIncluded\mods\SharedStorage.json \
Mac: ~Library/Application Support/unity.Klei.Oxygen Not Included/mods/SharedStorage.json \
Ubuntu: ~/.config/unity3d/Klei/Oxygen Not Included/mods/SharedStorage.json

### Format
* Enabled: bool | If false, stops the background task from transfering items.
* AllowCrossWorld: bool | Allow storages to transfer items across worlds.
* OnlySamePriority: bool | If false, allow transfer between any storages. If true, allow transfer to equal priority buildings only.
* AcceptInputAnywhere: bool | If true, any storage can accept any item. Misplaced items are transfered to other storages.
* MinGeneral: float | Minimum amount of items to move.
* MinFood: float | Minimum amount of food to move.
* MinClothes: float | Minimum amount of clothing to move.
* RefreshRate: int | How often storages try to transfer items. Lower is more often.
* Blacklist: string[] | Matching items will not be transfered by the background task.

### Source
[Github](https://github.com/Truinto/ONI-Modloader-SimpleMods)

### Notes
* Options can be changed while playing. Click the printing pod, then look for the 'Shared Storage Option' button on the right.
* While 'Accept Input Anywhere' is turn on, unticking a stored material will transfer it to another eligible storage instead of dropping it.
* To fully stop all features, disable 'Enabled' and 'Accept Input Anywhere'.
* This mod does not change the fetch logic. If a dupe decides to take a detour, it's not my fault.
