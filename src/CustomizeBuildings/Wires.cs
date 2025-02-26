using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(Wire), nameof(Wire.GetMaxWattageAsFloat))]
    [HarmonyPriority(Priority.Low)]
    public class Wire_GetMaxWattageAsFloat
    {
        public static void Postfix(Wire.WattageRating rating, ref float __result)
        {
            if (CustomizeBuildingsState.StateManager.State.WireSmallWatts == 1000 &&
                CustomizeBuildingsState.StateManager.State.WireRefinedWatts == 2000 &&
                CustomizeBuildingsState.StateManager.State.WireHeavyWatts == 20000 &&
                CustomizeBuildingsState.StateManager.State.WireRefinedHeavyWatts == 50000)
                return;

            __result = rating switch
            {
                Wire.WattageRating.Max1000 => CustomizeBuildingsState.StateManager.State.WireSmallWatts,
                Wire.WattageRating.Max2000 => CustomizeBuildingsState.StateManager.State.WireRefinedWatts,
                Wire.WattageRating.Max20000 => CustomizeBuildingsState.StateManager.State.WireHeavyWatts,
                Wire.WattageRating.Max50000 => CustomizeBuildingsState.StateManager.State.WireRefinedHeavyWatts,
                _ => __result,
            };
        }
    }

    public class Transformer_Patch : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == PowerTransformerConfig.ID && CustomizeBuildingsState.StateManager.State.PowerTransformerLarge != 4000
                || id == PowerTransformerSmallConfig.ID && CustomizeBuildingsState.StateManager.State.PowerTransformerSmall != 1000;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            int cap = def.PrefabID == PowerTransformerConfig.ID
                ? CustomizeBuildingsState.StateManager.State.PowerTransformerLarge 
                : CustomizeBuildingsState.StateManager.State.PowerTransformerSmall;

            def.GeneratorWattageRating = cap;
            def.GeneratorBaseCapacity = cap;
            var battery = def.BuildingComplete.GetComponent<Battery>();
            battery.capacity = cap;
            battery.chargeWattage = cap;
        }
    }
}
