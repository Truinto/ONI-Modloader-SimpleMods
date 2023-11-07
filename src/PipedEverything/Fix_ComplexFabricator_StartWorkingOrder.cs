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
}
