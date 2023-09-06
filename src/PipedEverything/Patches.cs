using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PipedEverything
{
    public static class Patches
    {


        /// <summary>
        /// Attach pipe logic to buildings.
        /// </summary>
        [HarmonyPatch(typeof(Assets), nameof(Assets.AddBuildingDef))]
        [HarmonyPrefix]
        public static void AddBuildingDef_Prefix(BuildingDef def)
        {
            AddLogic.TryAddLogic(def);
        }

        /// <summary>
        /// cache variables
        /// </summary>
        public static readonly HashSet<string> DrawBuildings = new();

        /// <summary>
        /// Draw conduit port icons.
        /// </summary>
        [HarmonyPatch(typeof(BuildingCellVisualizer), nameof(BuildingCellVisualizer.DrawIcons))]
        [HarmonyPrefix]
        public static bool DrawPorts_Prefix(BuildingCellVisualizer __instance, HashedString mode)
        {
            if (DrawBuildings.Contains(__instance.building.Def.PrefabID))
            {
                var go = __instance.building.gameObject;
                var controller = go.GetComponent<PortDisplayController>();
                if (controller != null)
                    return controller.Draw(__instance, mode, go);
            }
            return true;
        }

        /// <summary>
        /// Assign cells for ports while building to prevent other buildings from adding ports at the same cells.
        /// </summary>
        [HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.MarkArea))]
        [HarmonyPostfix]
        public static void MarkArea_Postfix(BuildingDef __instance, int cell, Orientation orientation, ObjectLayer layer, GameObject go)
        {
            foreach (PortDisplay2 portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
            {
                var conduitType = portDisplay.type;
                var objectLayer_Conduit = Grid.GetObjectLayerForConduitType(conduitType);
                var cellOffset = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
                int cell_offset = Grid.OffsetCell(cell, cellOffset);
                __instance.MarkOverlappingPorts(Grid.Objects[cell_offset, (int)objectLayer_Conduit], go);
                Grid.Objects[cell_offset, (int)objectLayer_Conduit] = go;
            }
        }

        ///<summary>
        /// Check if ports are blocked prior to building.
        ///</summary>
        [HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.AreConduitPortsInValidPositions))]
        [HarmonyPostfix]
        public static void AreConduitPortsInValidPositions_Postfix(BuildingDef __instance, ref bool __result, GameObject source_go, int cell, Orientation orientation, ref string fail_reason)
        {
            if (!__result)
                return;

            foreach (var portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
            {
                var rotatedCellOffset = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
                int utility_cell = Grid.OffsetCell(cell, rotatedCellOffset);

                __result = __instance.IsValidConduitConnection(source_go, portDisplay.type, utility_cell, ref fail_reason);
                if (!__result)
                    return;
            }
        }
    }
}
