# Changelog

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
