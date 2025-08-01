﻿using Common;
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
        /// <summary>
        /// Prevent item drop, if pipe connnected.
        /// </summary>
        [HarmonyPatch(typeof(ElementConverter), nameof(ElementConverter.ConvertMass))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ElementConverter_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(ElementConverter.OutputElement), nameof(ElementConverter.OutputElement.storeOutput));
            data.InsertAfter(shouldStore);

            return data;

            bool shouldStore(bool storeOutput, KMonoBehaviour __instance, [LocalParameter(IndexByType = 0)] Element element)
            {
                return storeOutput || __instance.GetComponent<PortDisplayController>()?.IsOutputConnected(element) == true;
            }
        }

        /// <summary>
        /// Prevent item drop, if pipe connnected.
        /// </summary>
        [HarmonyPatch(typeof(EnergyGenerator), nameof(EnergyGenerator.Emit))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EnergyGenerator_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(EnergyGenerator.OutputItem), nameof(EnergyGenerator.OutputItem.store));
            data.InsertAfter(shouldStore);

            return data;

            bool shouldStore(bool storeOutput, KMonoBehaviour __instance, [LocalParameter(IndexByType = 0)] Element element)
            {
                return storeOutput || __instance.GetComponent<PortDisplayController>()?.IsOutputConnected(element) == true;
            }
        }

        /// <summary>
        /// Prevent item drop, if pipe connnected.
        /// </summary>
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
                return shouldDrop && controller?.IsOutputConnected(element.Element) != true;
            }
        }

        /// <summary>
        /// Prevent specific item drop, if pipe connnected.
        /// </summary>
        [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.DropExcessIngredients))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ComplexFabricator_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(d => d.IsStloc(typeof(HashSet<Tag>)));
            data.InsertAfter(patch);

            return data;

            void patch(ComplexFabricator __instance, [LocalParameter(IndexByType = 0)] HashSet<Tag> keepTags)
            {
                var controller = __instance.GetComponent<PortDisplayController>();
                if (controller == null)
                    return;

                foreach (var port in controller.outputPorts)
                {
                    if (port.IsConnected())
                        foreach (var element in port.tags) // this won't stop empty filters from dropping
                            keepTags.Add(element);
                }
            }
        }

        /// <summary>
        /// Prevent item drop, if pipe connnected.
        /// </summary>
        [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.SpawnOrderProduct))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ComplexFabricator_Transpiler2(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            if (3 != data.InsertAfterAll(typeof(ComplexFabricator), nameof(ComplexFabricator.storeProduced), patch))
                throw new Exception("storeProduced count invalid");

            return data;

            bool patch(bool forceStore, ComplexFabricator __instance, [LocalParameter(IndexByType = 2)] ComplexRecipe.RecipeElement element3)
            {
                return forceStore || __instance.GetComponent<PortDisplayController>()?.IsOutputConnected(element3.material.ToElement()) == true;
            }
        }

        /// <summary>
        /// Prevent all item drop, if input pipe connnected.
        /// </summary>
        [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.DropExcessIngredients))]
        [HarmonyPrefix]
        public static bool ComplexFabricator_DropExcessIngredients(ComplexFabricator __instance)
        {
            PortDisplayController controller = __instance.GetComponent<PortDisplayController>();
            if (controller != null)
            {
                if (controller.solidOverlay.Any(inputIsConnected) || controller.liquidOverlay.Any(inputIsConnected) || controller.gasOverlay.Any(inputIsConnected))
                    return false;
            }
            return true;
            static bool inputIsConnected(PortDisplay2 port) => port.input && port.IsConnected();
        }

        /// <summary>
        /// Re-route emitter to pipe, if connected.
        /// </summary>
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
            if (port == null || !port.IsConnected())
            {
                if (!emitting)
                    __instance.dirty = true;
                return true;
            }

            // try store; if not, maybe clean
            float mass = __instance.emitRate * dt;
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
        /// Attach pipe logic to geysers.
        /// </summary>
        [HarmonyPatch(typeof(Assets), nameof(Assets.CreatePrefabs))]
        [HarmonyPostfix]
        public static void Assets_CreatePrefabs_Postfix()
        {
            if (!PipedEverythingState.StateManager.State.GeyserPipes)
                return;

            foreach (var prefab in Assets.Prefabs)
            {
                AddLogic.TryAddGeyser(prefab.gameObject);
            }
        }

        /// <summary>
        /// Draw conduit port icons.
        /// </summary>
        [HarmonyPatch(typeof(EntityCellVisualizer), nameof(EntityCellVisualizer.DrawIcons))]
        [HarmonyPrefix]
        public static void DrawPorts_Prefix(EntityCellVisualizer __instance, HashedString mode)
        {
            var controller = __instance.GetComponent<PortDisplayController>();
            controller?.Draw(__instance, mode);
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
