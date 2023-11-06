using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipedEverything
{
    [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.StartWorkingOrder))]
    public static class Fixes
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
    }
}
