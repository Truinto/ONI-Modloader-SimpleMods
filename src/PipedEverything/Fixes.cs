using Common;
using HarmonyLib;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PipedEverything
{
    [HarmonyPatch(typeof(GameUtil), nameof(GameUtil.GetFinalTemperature))]
    public static class Fix_FinalTemperature
    {
        public static void Prefix(ref float m1, ref float m2)
        {
            if (m1 <= 0f && m2 <= 0f)
            {
                m1 = 1f;
                m2 = 1f;
            }
        }
    }

    [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.StartWorkingOrder))]
    public static class Fix_ComplexFabricator_StartWorkingOrder
    {
        /// <summary>
        /// Replacing vanilla method and carving out the exceptions. Hopefully this fixes #81
        /// </summary>
        public static bool Prefix(int index, ComplexFabricator __instance)
        {
            if (__instance.HasWorkingOrder)
            {
                __instance.queueDirty = true;
                return false;
            }
            //Debug.Assert(!__instance.HasWorkingOrder, "machineOrderIdx already set");
            __instance.workingOrderIdx = index;
            if (__instance.recipe_list[__instance.workingOrderIdx].id != __instance.lastWorkingRecipe)
            {
                __instance.orderProgress = 0f;
                __instance.lastWorkingRecipe = __instance.recipe_list[__instance.workingOrderIdx].id;
            }
            __instance.TransferCurrentRecipeIngredientsForBuild();
            if (__instance.openOrderCounts[__instance.workingOrderIdx] <= 0)
            {
                __instance.queueDirty = true;
                return false;
            }
            //Debug.Assert(__instance.openOrderCounts[__instance.workingOrderIdx] > 0, "openOrderCount invalid");
            __instance.openOrderCounts[__instance.workingOrderIdx]--;
            __instance.UpdateChore();
            __instance.Trigger((int)GameHashes.FabricatorOrderStarted, __instance.recipe_list[__instance.workingOrderIdx]);
            __instance.AdvanceNextOrder();
            return false;
        }

        //public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator generator, MethodBase original)
        //{
        //    var tool = new TranspilerTool(code, generator, original);
        //    while (!tool.IsLast)
        //    {
        //        if (tool.Calls(typeof(Debug), nameof(Debug.Assert), [ typeof(bool), typeof(object) ]))
        //            tool.ReplaceCall(patch1, before: true);
        //        tool.Index++;
        //    }
        //    return tool;
        //    static void patch1(bool condition, object message)
        //    {
        //    }
        //}
    }

    [HarmonyPatch(typeof(Polymerizer), nameof(Polymerizer.TryEmit), typeof(PrimaryElement))]
    public static class Fix_Polymerizer
    {
        /// <summary>
        /// Make Polymerizer not emit steam, if a port for steam is connected.
        /// </summary>
        public static bool Prefix(PrimaryElement primary_elem, Polymerizer __instance)
        {
            var controller = __instance.GetComponent<PortDisplayController>();
            if (controller == null)
                return true;

            var element = primary_elem.ElementID;
            bool portIsConnected = controller.outputPorts.Any(a => (a.filter.Contains(SimHashes.Void) || a.filter.Contains(element)) && a.IsConnected());

            return !portIsConnected;
        }
    }

    [HarmonyPatch(typeof(AlgaeHabitat.SMInstance), nameof(AlgaeHabitat.SMInstance.NeedsEmptying))]
    public static class Fix_AlgaeHabitat
    {
        /// <summary>
        /// Fix dead lock, where empty chore would only start when the building is 100% full, but building would be disabled (including chores) when storage full.
        /// Maybe redundant with changes to <see cref="PortDisplay2.IsBlocked"/> checking now if connected.
        /// </summary>
        public static bool Prefix(AlgaeHabitat.SMInstance __instance, ref bool __result)
        {
            __result = __instance.smi.master.pollutedWaterStorage.ExactMassStored() >= 350f;
            return false;
        }
    }

    [HarmonyPatch]
    public static class Fix_Overpressure
    {
        [HarmonyPatch(typeof(Electrolyzer), nameof(Electrolyzer.RoomForPressure), MethodType.Getter)]
        [HarmonyPrefix]
        public static bool Electrolyzer_RoomForPressure(Electrolyzer __instance, ref bool __result)
        {
            var controller = __instance.GetComponent<PortDisplayController>();
            if (controller == null) return true;

            if (controller.outputPorts.All(a => a.IsConnected()))
            {
                __result = true;
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(RustDeoxidizer), nameof(RustDeoxidizer.RoomForPressure), MethodType.Getter)]
        [HarmonyPrefix]
        public static bool RustDeoxidizer_RoomForPressure(RustDeoxidizer __instance, ref bool __result)
        {
            var controller = __instance.GetComponent<PortDisplayController>();
            if (controller == null) return true;

            if (controller.outputPorts.All(a => a.IsConnected()))
            {
                __result = true;
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(OilRefinery.StatesInstance), nameof(OilRefinery.StatesInstance.TestAreaPressure))]
        [HarmonyPrefix]
        public static bool OilRefinery_TestAreaPressure(OilRefinery.StatesInstance __instance)
        {
            var refinery = __instance.smi.master;
            var controller = refinery.GetComponent<PortDisplayController>();
            if (controller == null) return true;

            if (controller.outputPorts.All(a => a.IsConnected()))
            {
                if (refinery.wasOverPressure)
                {
                    refinery.wasOverPressure = false;
                    __instance.sm.isOverPressure.Set(false, __instance);
                }
                return false;
            }

            return true;
        }
    }
}
