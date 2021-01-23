using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using FumLib;
using Klei.AI;

namespace CustomizeCritter
{
    /// Critters are always hungry
    [HarmonyPatch(typeof(CreatureCalorieMonitor.Instance), nameof(CreatureCalorieMonitor.Instance.OnCaloriesConsumed))]
    public class Patch_Hungry2   // TODO: check if useful
    {
        public static bool Prepare()
        {
            return CustomizeCritterState.StateManager.State.alwaysHungry;
        }

        public static void Postfix(CreatureCalorieMonitor.Instance __instance)
        {
            __instance.calories.value = Math.Max(__instance.calories.value, __instance.calories.GetMax() * 0.5f);
        }
    }

    /// Critters are always hungry
    [HarmonyPatch(typeof(CreatureCalorieMonitor.Instance), nameof(CreatureCalorieMonitor.Instance.IsHungry))]
    public class Patch_Hungry   // TODO: check if useful
    {
        public static bool Prepare()
        {
            return CustomizeCritterState.StateManager.State.alwaysHungry;
        }

        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    /// Critters never starve
    [HarmonyPatch(typeof(CreatureCalorieMonitor.Instance), nameof(CreatureCalorieMonitor.Instance.GetDeathTimeRemaining))]
    public class Patch_Starving2
    {
        public static bool Prefix(ref float __result)
        {
            __result = 6000f;
            return false;
        }
    }

    /// Critters never starve
    [HarmonyPatch(typeof(CreatureCalorieMonitor.Instance), nameof(CreatureCalorieMonitor.Instance.IsOutOfCalories))]
    public class Patch_Starving
    {
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}