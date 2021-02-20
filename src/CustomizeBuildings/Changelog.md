# Changelog

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
