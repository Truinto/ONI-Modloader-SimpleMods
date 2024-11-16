using Common;
using Epic.OnlineServices;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static bool Prefix(int index, ComplexFabricator __instance)
        {
            // fixed based on #47 ; unable to reproduce or test
            if (__instance.openOrderCounts[index] < 1)
            {
                __instance.nextOrderIsWorkable = false;
                return false;
            }

            return true;
        }

        public static Exception Finalizer(Exception __exception, ComplexFabricator __instance)
        {
            if (__exception == null)
                return null;
            try
            {
                Helpers.PrintDebug($"ComplexFabricator.StartWorkingOrder {__exception.Message}");
                __instance.nextOrderIsWorkable = false;
            }
            catch (Exception) { }
            return null;
        }
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
}
