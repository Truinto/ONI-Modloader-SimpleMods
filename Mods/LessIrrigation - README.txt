README

The settings work with ingame IDs. These differ from the displayed name. Here some examples:
Name                ID
Nosh Sprout         BeanPlant
Sleet Wheat         ColdWheat
Arbor Tree          ForestTree
Bristle Blossom     PrickleFlower
Help me complete the list.

Groups:
RemoveIrrigationFromPlants - any matching plant ID will not load the vanilla irrigation, if you want to change existing irrigation then add it here
AddIrrigiation - adds tags to matching plant IDs, liquids will be automatically set to irrigation, everything else to fertilization; consumption is per second, so if you want 5kg divide it by 600 first = 0,00166667; note that if you use more than one liquid you cannot use the Hydroponic Farm, other farms work
SetIllumination - any matching plant ID will change the illumination setting; -1 = needs darkness, 0 = works in either, 1 = needs light

ExampleAllPlants - lists all valid plant IDs