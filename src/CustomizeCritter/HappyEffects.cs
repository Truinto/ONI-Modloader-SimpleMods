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
    [HarmonyPatch(typeof(HappinessMonitor), nameof(HappinessMonitor.InitializeStates))]
    public class Patch_HappyEffect
    {
        public static void Postfix(Effect ___happyWildEffect, Effect ___happyTameEffect, Effect ___unhappyWildEffect, Effect ___unhappyTameEffect)
        {
            if (CustomizeCritterState.StateManager.State.happyWildEffect != null)
            {
                ___happyWildEffect.SelfModifiers.Clear();
                foreach (var attribute in CustomizeCritterState.StateManager.State.happyWildEffect)
                    ___happyWildEffect.Add(new AttributeModifier(attribute.id, attribute.value, STRINGS.CREATURES.MODIFIERS.HAPPY.NAME, attribute.multiplier ?? false));
            }
            
            if (CustomizeCritterState.StateManager.State.happyTameEffect != null)
            {
                ___happyTameEffect.SelfModifiers.Clear();
                foreach (var attribute in CustomizeCritterState.StateManager.State.happyTameEffect)
                    ___happyTameEffect.Add(new AttributeModifier(attribute.id, attribute.value, STRINGS.CREATURES.MODIFIERS.HAPPY.NAME, attribute.multiplier ?? false));
            }
            
            if (CustomizeCritterState.StateManager.State.unhappyWildEffect != null)
            {
                ___unhappyWildEffect.SelfModifiers.Clear();
                foreach (var attribute in CustomizeCritterState.StateManager.State.unhappyWildEffect)
                    ___unhappyWildEffect.Add(new AttributeModifier(attribute.id, attribute.value, STRINGS.CREATURES.MODIFIERS.HAPPY.NAME, attribute.multiplier ?? false));
            }
            
            if (CustomizeCritterState.StateManager.State.unhappyTameEffect != null)
            {
                ___unhappyTameEffect.SelfModifiers.Clear();
                foreach (var attribute in CustomizeCritterState.StateManager.State.unhappyTameEffect)
                    ___unhappyTameEffect.Add(new AttributeModifier(attribute.id, attribute.value, STRINGS.CREATURES.MODIFIERS.HAPPY.NAME, attribute.multiplier ?? false));
            }
        }
    }
}