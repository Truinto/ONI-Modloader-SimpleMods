using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Common;
using static Storage;

namespace PipedEverything
{
    public static class AddLogic
    {
        public static void TryAddLogic(BuildingDef def)
        {
            if (def?.PrefabID == null)
                return;

            foreach (var config in PipedEverythingState.StateManager.State.Configs.Where(w => w.Id == def.PrefabID || w.Id == def.Name.StripLinks()))
            {
                var conduitType = ConduitType.None;
                var offset = new CellOffset(config.OffsetX, config.OffsetY);
                var color = config.Color;
                var filters = new List<SimHashes>();
                bool isToxic = false;
                Helpers.PrintDebug($"AddLogic adding {config.Id} {offset}");
                foreach (var filter in config.Filter)
                {
                    var element = filter.ToElement();
                    if (element == null)
                    {
                        Helpers.PrintDialog($"Unable to resolve: {filter} in {config.Id}");
                        continue;
                    }

                    if (conduitType == ConduitType.None)
                        conduitType = element.IsGas ? ConduitType.Gas : element.IsLiquid ? ConduitType.Liquid : ConduitType.Solid;
                    else if (conduitType == ConduitType.Gas && !element.IsGas
                        || conduitType == ConduitType.Liquid && !element.IsLiquid
                        || conduitType == ConduitType.Solid && !element.IsSolid)
                    {
                        Helpers.PrintDialog($"Element does not match conduit type {conduitType}: {filter} in {config.Id}");
                        continue;
                    }

                    if (element.sublimateId != 0)
                        isToxic = true;

                    color ??= GetColor(element);
                    filters.Add(element.id);
                }

                if (conduitType == ConduitType.None)
                {
                    Helpers.PrintDialog($"No valid filter for {config.Id}");
                    continue;
                }

                var portInfo = new PortDisplayInfo(filters.ToArray(), conduitType, offset, config.Input, color, config.StorageIndex, config.StorageCapacity);
                def.BuildingComplete.AddOrGet<PortDisplayController>().AssignPort(def.BuildingComplete, portInfo);
                def.BuildingUnderConstruction.AddOrGet<PortDisplayController>().AssignPort(def.BuildingUnderConstruction, portInfo);
                def.BuildingPreview.AddOrGet<PortDisplayController>().AssignPort(def.BuildingPreview, portInfo);

                // ensure enough room for new elements and is sealed
                def.BuildingComplete.AddOrGet<Storage>();
                var storage = def.BuildingComplete.GetComponents<Storage>()[portInfo.StorageIndex];
                storage.capacityKg += portInfo.StorageCapacity * portInfo.filter.Length;
                if (isToxic && !storage.defaultStoredItemModifers.Contains(StoredItemModifier.Seal))
                    storage.defaultStoredItemModifers.Add(StoredItemModifier.Seal);

                // add conduit consumer/dispenser
                if (config.Input)
                {
                    if (conduitType != ConduitType.Solid)                    
                        def.BuildingComplete.AddComponent<ConduitConsumerOptional>().AssignPort(portInfo);                    
                    else
                        def.BuildingComplete.AddComponent<ConduitConsumerOptionalSolid>().AssignPort(portInfo);
                }
                else
                {
                    if (conduitType != ConduitType.Solid)
                        def.BuildingComplete.AddComponent<ConduitDispenserOptional>().AssignPort(portInfo);
                    else
                        def.BuildingComplete.AddComponent<ConduitDispenserOptionalSolid>().AssignPort(portInfo);
                }

                // fix for gourment station; ComplexFabricator drops ingredients from inStorage, if they are not in the selected recipe (like carbon dioxide)
                if (config.Id == GourmetCookingStationConfig.ID)
                {
                    def.BuildingComplete.GetComponent<ComplexFabricator>().keepAdditionalTag = SimHashes.CarbonDioxide.ToTag();
                }

                Helpers.PrintDebug($"Controller added port {config.Id} {conduitType} {offset} input={config.Input}");
            }
        }

        public static Color32 GetColor(Element element)
        {
            Color32 color = element.substance.conduitColour;
            color.a = 255; // for some reason the alpha channel is set to invisible for some elements (hydrogen only?)

            if (color.r == 0 && color.g == 0 && color.b == 0)
            {
                // avoid completely black icons since the background is black
                color.r = 25;
                color.g = 25;
                color.b = 25;
            }
            return color;
        }
    }
}
