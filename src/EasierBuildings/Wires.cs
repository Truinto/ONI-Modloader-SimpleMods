using Harmony;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(Wire), "GetMaxWattageAsFloat")]
    internal class Wire_GetMaxWattageAsFloat
    {

        private static void Postfix(Wire.WattageRating rating, ref float __result)
        {
            if (CustomizeBuildingsState.StateManager.State.WireSmallWatts == 1000 &&
                CustomizeBuildingsState.StateManager.State.WireRefinedWatts == 2000 &&
                CustomizeBuildingsState.StateManager.State.WireHeavyWatts == 20000 &&
                CustomizeBuildingsState.StateManager.State.WireRefinedHeavyWatts == 50000)
                return;

            switch (rating)
            {
                case Wire.WattageRating.Max500:
                    __result = 500f;
                    break;
                case Wire.WattageRating.Max1000:
                    __result = CustomizeBuildingsState.StateManager.State.WireSmallWatts;
                    break;
                case Wire.WattageRating.Max2000:
                    __result = CustomizeBuildingsState.StateManager.State.WireRefinedWatts;
                    break;
                case Wire.WattageRating.Max20000:
                    __result = CustomizeBuildingsState.StateManager.State.WireHeavyWatts;
                    break;
                case Wire.WattageRating.Max50000:
                    __result = CustomizeBuildingsState.StateManager.State.WireRefinedHeavyWatts;
                    break;
                default:
                    __result = 0.0f;
                    break;
            }
        }
    }
}