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
    [HarmonyPatch(typeof(FertilityMonitor.Instance), nameof(FertilityMonitor.Instance.ShowEgg))]
    public class Patch_EggWildness
    {
        public static bool Prepare()
        {
            return CustomizeCritterState.StateManager.State.eggWildness >= 0f;
        }

        public static void Prefix(GameObject ___egg, GameObject __state)
        {
            __state = ___egg;
        }

        public static void Postfix(GameObject __state)
        {
            __state.GetAmounts().Get(Db.Get().Amounts.Wildness.Id).SetValue(
                __state.GetAmounts().Get(Db.Get().Amounts.Wildness.Id).value - CustomizeCritterState.StateManager.State.eggWildness);
        }
    }


}