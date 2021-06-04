# Changelog

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
