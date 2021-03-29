# Changelog

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
