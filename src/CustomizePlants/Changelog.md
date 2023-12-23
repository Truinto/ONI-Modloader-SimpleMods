# Changelog

## [1.0.42.0]  U50-587362-S
- small patch to increase compatiblity with Fast Track

## [1.0.41.0] U48-568201-S
- fixed CheatFlowerVase, it no longer consumes material

## [1.0.40.0] U46-550759-S
- fix disease settings

## [1.0.39.0] U44-535842-S
- updated

## [1.0.38.0] U43-531669-S
- changed irrigation tag to accept any tag instead of just basic ones

## [1.0.37.0] U43-526233-S
- added archive version

## [1.0.35.0] U42-514967-S
- fixed for new version

## [1.0.34.0] U42-514967-S
- changed so plant id will also work with plant's literal name

## [1.0.33.0] U40-494396-S
- fixed read all button, will now convert irrigation to kg/cycle instead of kg/second
- fixed empty settings when switching language; will now save settings separately for each language

## [1.0.32.0] U39-490405-S

### Fixed
- MutantPlantsDropSeeds now actually makes mutant plants drop seeds

## [1.0.31.0] U37-484114-S

### Fixed
- fixed mutant drop seeds

## [1.0.30.0] U37-484114-S

### Fixed
- fixed new mutations not getting generated

## [1.0.29.0] U37-484114-S

### Added
- added setting to allow mutant plants to drop seeds

## [1.0.28.0] U34-476542-S

### Added
- added ingame menu with some basic options
- added option radiation_threshold_min and radiation_threshold_max to PlantSettings
- added translation support

## [1.0.27.0] U33-473720-B

### Fixed
- fixed a crash on vanilla version

## [1.0.26.0] EX1 S14-471883-S

### Fixed
- fixed missing ModPlants initialisation (only needed in special cases)
- set vanilla flag, so it can load without DLC

## [1.0.25.0] EX1 S13-469473

### Changed
- reversed changed config file path and fixed read/write function to use the same path

## [1.0.24.0] EX1 S13-469369

### Fixed
- fixed some weird behavior with SeedsGoIntoAnyFlowerPots

### Changed
- updated to Harmony 2.0
- expanded mutation options

## [1.0.23.0] EX1 S11-464434

### Added
- added radiation setting, if set adds component with rads equal to value
- added radiation_radius setting, if radiation component exists sets the X and Y range to value
- added print_mutations, if true will overwrite MutationSettings with Klei's settings
- added MutationSettings which let's you change the basic attributes of mutations

## [1.0.22.0] EX1 S11-464102

### Fixed
- fixed crash when deco plant with a placeholder-crop and the lice mutation and was harvested

## [1.0.21.0] EX1 S11-464102

### Added
- added cheat to analyze mutations automatically (off by default)

### Fixed
- fixed crash when deco plants with crops tried to apply mutations to seeds

## [1.0.20.0] EX1 S9-458490

### Fixed
- fixed issue with fruit_grow_time not being set

## [1.0.19.0] EX1 S9-458490 CS-455509

### Added
- added warning when safe_element cannot be resolved (aka typo)

### Fixed
- non-DLC not applying safe_elements

## [1.0.18.0] EX1 S9-458490

### Fixed
- fixed decor plants with fruits causing a crash
- fixed safe_elements being overwritten

## [1.0.17.0] EX1 S9-458490 CS-455509

### Fixed
- fixed fruits again
- fixed illumination
- fixed whatever was wrong with vanilla version

## [1.0.16.1] EX1 S9-458490 CS-455509

### Fixed
- patched for new update
- fixed fruits now working with traits; not validated
- fixed hook not working

## [1.0.16.0] EX1 S8-456169 CS-455509

### Added
- FlowerVaseWild: plants in Flower Vase become wild, reducing productivity to 25% and not needing irrigation

### Changed
- added new hook that tries to modify the plant 'early'; that way plants added by mods don't require their assembly to be cross loaded and their assembly qualifier doesn't need to be in ModPlants
- added IgnoreList to not patch certain plants with the former 'early' patch
- plants can still be added to ModPlants and it works as before; I leave this for compatibility reasons, although I am not aware of plants that require this
- shortend AssemblyQualifiedName of ModPlants

### Fixed
- fixed safe_elements not being applied

## [1.0.15.0] EX1 S6-448916

### Added
- added new DLC plants

## [1.0.14.0] EX1 S6-448916

### Fixed
- updated for DLC1
