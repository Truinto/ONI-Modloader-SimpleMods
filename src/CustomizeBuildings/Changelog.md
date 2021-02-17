# Changelog

## [1.0.28.0] EX1 S6-449549

### Added
- pressured valve option that adds a button to limit the output pressure; very niche application

### Fixed
? fixed non-DLC config being in the wrong folder
- oil well hack will no longer load if "Piped Output" mod is installed

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
