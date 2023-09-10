using Common;
using HarmonyLib;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PipedEverything
{
    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(ElementConverter), nameof(ElementConverter.ConvertMass))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ElementConverter_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(ElementConverter.OutputElement), nameof(ElementConverter.OutputElement.storeOutput));
            data.InsertAfter(shouldStore);

            return data;

            bool shouldStore(bool storeOutput, ElementConverter __instance, [LocalParameter(IndexByType = 0)] Element element)
            {
                return storeOutput || __instance.GetComponent<PortDisplayController>()?.CanStore(element) == true;
            }
        }

        [HarmonyPatch(typeof(EnergyGenerator), nameof(EnergyGenerator.Emit))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EnergyGenerator_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(EnergyGenerator.OutputItem), nameof(EnergyGenerator.OutputItem.store));
            data.InsertAfter(shouldStore);

            return data;

            bool shouldStore(bool storeOutput, EnergyGenerator __instance, [LocalParameter(IndexByType = 0)] Element element)
            {
                return storeOutput || __instance.GetComponent<PortDisplayController>()?.CanStore(element) == true;
            }
        }

        [HarmonyPatch(typeof(AutoStorageDropper.Instance), nameof(AutoStorageDropper.Instance.Drop))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> AutoStorageDropper_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.InsertBefore(getController);
            data.Seek(typeof(AutoStorageDropper.Instance), nameof(AutoStorageDropper.Instance.AllowedToDrop));
            data.InsertAfter(shouldDrop);

            return data;

            void getController(AutoStorageDropper.Instance __instance, [LocalParameter("controller")] ref PortDisplayController controller)
            {
                controller = __instance.GetComponent<PortDisplayController>();
            }

            bool shouldDrop(bool shouldDrop, AutoStorageDropper.Instance __instance, [LocalParameter(IndexByType = 0)] PrimaryElement element, [LocalParameter("controller")] PortDisplayController controller)
            {
                return shouldDrop && controller?.CanStore(element.Element) != true;
            }
        }

        [HarmonyPatch(typeof(BuildingElementEmitter), nameof(BuildingElementEmitter.Sim200ms))]
        [HarmonyPrefix]
        public static bool BuildingElementEmitter_Prefix(float dt, BuildingElementEmitter __instance)
        {
            // do nothing, if not operational
            if (!__instance.simActive)
                return true;

            var controller = __instance.GetComponent<PortDisplayController>();
            if (controller == null)
                return true;

            // check if port is connected; if not, maybe clean
            bool emitting = __instance.statusHandle != Guid.Empty;
            var element = __instance.element.ToElement();
            var port = controller.GetPort(false, element.GetConduitType(), element.id);
            if (!port.IsConnected())
            {
                if (!emitting)
                    __instance.dirty = true;
                return true;
            }

            // try store; if not, maybe clean
            float mass = __instance.emitRate * 0.2f;
            if (!port.TryStore(element, mass, __instance.temperature))
            {
                if (!emitting)
                    __instance.dirty = true;
                return true;
            }

            // trick sim to not emit
            if (emitting)
                __instance.dirty = true;
            __instance.simActive = false;
            __instance.UnsafeUpdate(dt);
            __instance.simActive = true;

            Game.Instance.accumulators.Accumulate(__instance.accumulator, mass);
            if (__instance.element == SimHashes.Oxygen)
                ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, mass, __instance.gameObject.GetProperName());
            return false;
        }

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

    public class Patches_AdvancedGenerators
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            Helpers.PrintDebug($"Patches_AdvancedGenerators {original}");

            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(EnergyGenerator.OutputItem), nameof(EnergyGenerator.OutputItem.store));
            data.InsertAfter(shouldStore);

            return data;

            bool shouldStore(bool storeOutput, KMonoBehaviour __instance, [LocalParameter(IndexByType = 0)] Element element)
            {
                return storeOutput || __instance.GetComponent<PortDisplayController>()?.CanStore(element) == true;
            }
        }
    }
}
