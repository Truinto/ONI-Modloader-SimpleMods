using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Shared;
using System.Reflection;

namespace CustomizePlants
{
    [HarmonyPatch]
    public static class ColdBreatherConfigMod
    {
        [HarmonyPatch(typeof(ColdBreatherConfig), nameof(ColdBreatherConfig.CreatePrefab))]
        [HarmonyPostfix]
        public static void Postfix1(GameObject __result)
        {
            __result.GetComponent<ColdBreather>().deltaEmitTemperature = CustomizePlantsState.StateManager.State.WheezewortTempDelta;
            __result.AddOrGet<ElementConsumer>().consumptionRate = 1f;
        }

        [HarmonyPatch(typeof(ColdBreather), nameof(ColdBreather.OnReplanted))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);
            data.ReplaceAllConstant(0.25f, 1f);
            return data;
        }
    }
}