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
            if (CustomizeBuildingsState.Instance.WireSmallWatts == 1000 &&
                CustomizeBuildingsState.Instance.WireRefinedWatts == 2000 &&
                CustomizeBuildingsState.Instance.WireHeavyWatts == 20000 &&
                CustomizeBuildingsState.Instance.WireRefinedHeavyWatts == 50000)
                return;

            __result = rating switch
            {
                Wire.WattageRating.Max1000 => CustomizeBuildingsState.Instance.WireSmallWatts,
                Wire.WattageRating.Max2000 => CustomizeBuildingsState.Instance.WireRefinedWatts,
                Wire.WattageRating.Max20000 => CustomizeBuildingsState.Instance.WireHeavyWatts,
                Wire.WattageRating.Max50000 => CustomizeBuildingsState.Instance.WireRefinedHeavyWatts,
                _ => __result,
            };
        }
    }

    public class Transformer_Patch : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == PowerTransformerConfig.ID && CustomizeBuildingsState.Instance.PowerTransformerLarge != 4000
                || id == PowerTransformerSmallConfig.ID && CustomizeBuildingsState.Instance.PowerTransformerSmall != 1000;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            int cap = def.PrefabID == PowerTransformerConfig.ID
                ? CustomizeBuildingsState.Instance.PowerTransformerLarge 
                : CustomizeBuildingsState.Instance.PowerTransformerSmall;

            def.GeneratorWattageRating = cap;
            def.GeneratorBaseCapacity = cap;
            var battery = def.BuildingComplete.GetComponent<Battery>();
            battery.capacity = cap;
            battery.chargeWattage = cap;
        }
    }
}
