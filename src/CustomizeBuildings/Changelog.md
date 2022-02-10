# Changelog

## [1.0.61.0] U40-494396-S
- added CompostFreshnessPercent and CompostCaloriesPerDupe which can automatically mark stale food for compost

## [1.0.60.0] U40-493827-S U40-494396-S
- fixed empty settings when switching language; will now save settings separately for each language
- fixed building substitution strings if they had no hyperlink
- changed that pressure settings are also applied to "Supply Teleporter" (WarpConduitSender)

## [1.0.59.0]
- updated language file; thanks to Ventulus-lab

## [1.0.58.0] U39-490405-S
- added rocket height tuning
- fixed some tuning settings not being applied

## [1.0.57.0] U39-490405-S
- allowing ingame names for BuildingAdvancedOutputTemp
- fixed crash with 3rd party mods that add buildings without proper naming
- no-dupe ice cooler does now accept ice and brine ice
- removed NewRecipeRockCrusher, this functionality became its own mod: Customize Recipe
- fixed issue with resolving translated Tags introduced in 1.0.54.0
- ignoring case for IDs in BuildingBaseSettings, BuildingAdvancedMachineMultiplier, BuildingAdvancedOutputTemp, and BuildingAdvancedMaterial
- updated tuning texts, removed obsolete settings, added new settings
- changed it so setting TuningGlobal to false will reset all tuning values to the defaults during the next bootup
- updated language file; thanks to Ventulus-lab

## [1.0.56.0] 
- updated language file; thanks to Ventulus-lab

## [1.0.55.0] U39-490405-S
- fixed issue that would reset settings, when using the ingame settings while not using English

## [1.0.54.0] U39-490405-S
- ~~fixed formatting issue while loading translations~~
- fixed language code when using official translations (will now load 'strings_zh.pot', instead of looking for 'strings_zh_klei.pot')

## [1.0.53.0]
- added chinese translation; thanks to Ventulus-lab

## [1.0.52.0] U39-490405-S
- ElectrolizerMaxPressure also applies to Rust Deoxidizer

## [1.0.51.0] U39-490405-S
- added PowerTransformerLarge, PowerTransformerSmall in the Power Cable category

## [1.0.50.0] U39-490405-S
- added Radbolt chamber capacity (RadBattery)

## [1.0.49.0] U39-489821-S

### Added
- SealInsulateStorages to apply sealed and insulated attribute to storages (default off)
- NoDupePayloadOpener to reduce unpackaging time to 0 (default off)

## [1.0.48.0] U38-486708-S U38-487223-S

### Added
- added option to change Robo Miner speed

## [1.0.47.0] U37-484114-S

### Fixed
- updated to new release

## [1.0.46.0] U34-477203-S

### Fixed
- fixed LadderCometInvincibility AGAIN

## [1.0.45.0] U34-476059-S U34-476542-S

### Updated

## [1.0.44.0] U33-474321-S U34-475368-S

### Changed
- selecting an attribute on the skill station will repeat the jobs, as long as the dupe has enough exp

### Fixed
- fixed skill-station crash when dupe is selected
- skill-station now applies its attribute increases instantly
- excluded critters from AutoSweeperPickupAnything to prevent crashes
- fixed a bug that set the drillcone capacity to 1 (instead of 1000), making it impossible to mine

### Removed
- removed non existing TuningEnginePowerLateWeak

## [1.0.43.0] U33-473720-S U33-474141-S

### Added
- added drill cone and various launch port capacity settings
- added no dupe diamond press

### Fixed
- fixed an incompatibility issue with PeterHan.AIImprovements

## [1.0.42.0] EX1 S14-471883-S

### Added
- BuildingAdvancedOutputTemp which allows setting output material temperature of converters

### Changed
- renamed MachineMultiplier to BuildingAdvancedMachineMultiplier; it now requires BuildingAdvancedGlobalFlag to be true
- renamed BuildingAdvanced to BuildingAdvancedMaterial
- added menu entries for BuildingBaseSettingGlobalFlag and BuildingAdvancedGlobalFlag

### Fixed
- added legacy version for CS-469300 and older

## [1.0.41.0] EX1 S14-471883-S

### Fixed
- fixed crash when changing state of self sealing doors while nodupedoors was off

## [1.0.40.0] EX1 S14-471883-S

### Fixed
- fixed bug where idle dupes would rarely get entombed in doors

## [1.0.39.0] EX1 S14-471883-S

### Added
- added option to allow placing building with material you have nothing stored of
- added option to stop the game from switching selected materials due to insufficient material

### Fixed
- fixed TUNING patches not being load
- set vanilla flag, so it can load without DLC

## [1.0.38.0] EX1 S14-471531

### Fixed
- rewrote the SkillStation code; fixing an issue that would reset skills instead of applying new traits/skills
- fixed air conditioner crash caused by latest update

## [1.0.37.0] EX1 S13-469473

### Added
- added BuildingAdvanced that allows to add elements as valid materials to any specific building (split multiple with spacebar); this is an advanced setting which cannot be changed ingame
- added No Dupe Algae Terrarium, which removes the need to empty the dirty water storage
- added Algae Terrarium converter setting; allows for variable input/output

## [1.0.36.0] EX1 S13-469473

### Changed
- reversed changed config file path and fixed read/write function to use the same path

## [1.0.35.0] EX1 S13-469369

### Fixed
- Option menu being too small

## [1.0.34.0] EX1 S13-469369

### Added
- added telescope clear radius, analysis radius, and no-dupe options
- added railgun max launch

### Changed
- updated to Harmony 2.0

## [1.0.33.0] EX1 S12-466411 CS-466292

### Added
- added options for mini pumps

### Fixed
- fixed a Skill Station exploit
- fixed \n not getting resolved in language files

### Changed
- changed default value for AirConditionerAbsolutePowerFactor from 0.1 to 0.6
- air conditioner consumes at least 100W regardless of cooling factor, up to its default value (240W)
- air conditioner will ignore DPU setting, if in -14°C mode (that is, if target temperature is 0 Kelvin)

## [1.0.32.0] EX1 S9-458490 CS-455509

### Fixed
- fixed AirConditionerAbsoluteOutput cooling the building, causing crashes at 0 Kelvin; instead the building will heat at least 4 kJoules
- fixed AirConditionerAbsoluteOutput requiring almost no power when heating the input; power consumption now scales with heat produced

## [1.0.31.0] EX1 S9-458490 CS-455509

### Added
- added tuning global flag to disable the feature altogether
- added AirConditionerAbsolutePowerFactor, which controls load to power consumption

### Fixed
- removed error message when language file is missing
- fixed English language file not loading
- fixed condition NoDupeGlobal to be true for NoDupeCompost and NoDupeDesalinator (NoDupeGlobal must be true for any NoDupe feature to work)

### Removed
- removed tuning settings that were removed from the game and caused crashes

### Changed
- options categories start collapsed
- made 'global' settings more prominent

## [1.0.30.0] EX1 S8-456169 CS-455509

### Added
- added localization support
- added tuning for leadsuits
- added tuning for rockets

### Changed
- material categories can now read two entries (separate by space)

### Fixed
- fixed options menu not showing up without DLC

## [1.0.29.0] EX1 S6-449549 CS-449460

### Added
- added missing options for TUNING values (mostly atmosuit)

### Fixed
- disabled AutoSweeperPickupAnything on default, as it was causing issues
- changed AutoSweeperPickupAnything so it respects IsCellReachable; this might fix the former issue

## [1.0.28.0] EX1 S6-449549 CS-449460

### Added
- pressured valve option that adds a button to limit the output pressure; very niche application
- auto sweeper can pick up any material (notably canisters)
- air conditioner absolute mode; adds slider for output temperature instead of -14°C delta
- space heater target temperature slider
- allow conveyor loader to load liquids/gases
- added options menu, for ingame editing of all simple settings; still requires json for advanced setting (only works with DLC)
- added reset button
- No Dupe Sludge Press
- No Dupe Desalinator
- No Dupe Compost
- Skill Scrubber repurposed as Skill Station, which allows a variety of dupe modifications 

### Changed
- reorganized NoDupe settings to be boolean; some names changed

### Fixed
- oil well hack will no longer load if "Piped Output" mod is installed
- MachineMultiplier will now properly work with large pipes

## [1.0.27.0] EX1 S6-448916

### Fixed
- fixed memory leak caused by faulty transpiler

## [1.0.26.1] CS-447596

### Fixed
- change load order to hopefully fix an issue with PipedOutput

## [1.0.26.0]

### Added
- PipeThroughputPercent: aquatuner and airconditioner throughput relative to pipe pressure in percent (0.0 to 1.0)

### Changed
- PipeLiquidMaxPressure and PipeGasMaxPressure changed from integer to float
