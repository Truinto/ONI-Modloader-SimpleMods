# Changelog

## [1.0.28.0] U56-677228-SCRP (release)
- fix for 'Fast Track'

## [1.0.27.1] U56-677228-SCRP (release)
- added defaults for Plant Pulverizer and Peat Burner
- added GeyserPipes switch, which adds one pipe on every geyser
- added GeyserPipesUnlimited, which allows geysers overload the input node (use multiple pipes on the same port)
- added GeyserStorageKG and GeyserGasStorageKG to set storage size of geysers (only while pipe connected)
- added null sanitation to storage, fix #85
- changed solid pipes to respect CustomizeBuildings pipe size

## [1.0.26.0] U54-652372-SCR (release)
- new attempt to fix #47 #81

## [1.0.25.0] U54-652372-SCR (release)
- fixed AdvancedGenerators not being detected, because the assembly has a new name >:(
  you still need to have AdvancedGenerators first in load order!

## [1.0.24.0] U54-650327-SCR (release)
- hopefully fixed very rare crash #77

## [1.0.23.0] U53-644960-SC (release)
- added options to move original ports

## [1.0.22.0] U52-626616-SC (release)
- removed RemoveMaxAtmosphere from all configs
- added conditional patch to disable pressure checks, if all output ports are connected

## [1.0.21.0] U52-626616-SC (release)
- changed fabricators to not drop ingredients, if at least one input port is connected

## [1.0.20.0] U52-626616-SC (release)
- fixed infinite capacity

## [1.0.19.0] U52-626616-SC (release)
- fixed Polymerizer.Emit ignoring pipes (probably)
- readded default capacity, as this broke more stuff than it fixed
- changed RemoveMaxAtmosphere to 100kg, instead of infinite

## [1.0.18.0] U52-626616-SC (release)
- removed default capacity; undefined storages stay unchanged
- fixed building ceasing operation (from v1.0.3) if storage full while pipe disconnected

## [1.0.17.0] U52-626616-SC (release)
- increased default liquid capacity to 400kg
- increased AlgaeHabitat capacity to 800kg, fixing manual emptying

## [1.0.16.0] U52-626616-SC (release)
- added Ice Maker

## [1.0.15.0] U52-626616-SC (release)
- added support for all Tags (no longer restricted to elements)
- updated timber to new WoodLog id
- added Ice Kettle, Campfire, Espresso Machine, and SodaFountain config
- fixed storage index for Gas Range

## [1.0.14.1] U52-622509-SC (release)
- fixed crash for zero mass objects

## [1.0.14.0] U52-619020-SC (preview)
- preview branch

## [1.0.13.0] U51-600112-S
- fixed element specific storage for catch-alls ('Solid', 'Liquid', and 'Gas')

## [1.0.12.0] U51-600112-S
- added ColorBackground and ColorBorder to have mixed colored icons  #61
- added more ports #62

## [1.0.11.0] U51-600112-S
- added option to disable atmosphere checks (for oil refinery, oxygen generators)

## [1.0.10.0] U50-587362-S
- supress warning messages when playing without DLC

## [1.0.9.0] U50-587362-S
- removed faulty Desalinator from config
- fixed reservoirs always insulating #53
- removed duplicate port on polymer press

## [1.0.8.0] U50-581979-S
- update

## [1.0.7.0] U49-577063-S
- fixed rare crash #47, really

## [1.0.6.0] U49-577063-S
- fixed rare crash #47

## [1.0.5.0] U48-568201-S
- fixed issue with removing buildings
- added support for ComplexFabricator
- allowing 'Solid', 'Liquid', and 'Gas' as general filter options

## [1.0.4.0] U48-568201-S
- fixed crash with displaying building preview

## [1.0.3.0] U48-568201-S
- changed building to cease operation, if storage is full (output blocked)

## [1.0.2.0] U48-568201-S
- added patches for Advanced Generators+ (https://steamcommunity.com/sharedfiles/filedetails/?id=1934934314)

## [1.0.0.0] U48-568201-S
- new project
