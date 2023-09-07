using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Common;

namespace PipedEverything
{
    public static class AddLogic
    {
        public static void TryAddLogic(BuildingDef def)
        {
            if (def?.PrefabID == null)
                return;

            foreach (var config in PipedEverythingState.StateManager.State.Configs.Where(w => w.Id == def.PrefabID))
            {
                var conduitType = ConduitType.None;
                var offset = new CellOffset(config.OffsetX, config.OffsetY);
                var color = config.Color;
                var filters = new List<SimHashes>();
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

                    config.Color ??= GetColor(element);
                    filters.Add(element.id);
                }

                if (conduitType == ConduitType.None)
                {
                    Helpers.PrintDialog($"No valid filter for {config.Id}");
                    continue;
                }

                var portInfo = new DisplayConduitPortInfo(filters.ToArray(), conduitType, offset, config.Input, color);
                var controller = def.BuildingComplete.AddOrGet<PortDisplayController>();
                controller.Init(def.BuildingComplete);
                controller.AssignPort(def.BuildingComplete, portInfo);
                controller = def.BuildingUnderConstruction.AddOrGet<PortDisplayController>();
                controller.Init(def.BuildingUnderConstruction);
                controller.AssignPort(def.BuildingUnderConstruction, portInfo);
                controller = def.BuildingPreview.AddOrGet<PortDisplayController>();
                controller.Init(def.BuildingPreview);
                controller.AssignPort(def.BuildingPreview, portInfo);

                def.BuildingComplete.AddOrGet<Storage>();

                if (config.Input)
                {
                    // TODO: add input port logic

                    // if input
                    //ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
                    //conduitConsumer.conduitType = ConduitType.Liquid;
                    //conduitConsumer.consumptionRate = 1f;
                    //conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
                    //conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
                    //SolidConduitConsumer

                    if (conduitType != ConduitType.Solid)
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    if (conduitType != ConduitType.Solid)
                    {
                        var dispenser = def.BuildingComplete.AddComponent<PortConduitDispenserBase>();
                        dispenser.AssignPort(portInfo);
                    }
                    else
                    {
                        // TODO: solid dispenser
                    }
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
