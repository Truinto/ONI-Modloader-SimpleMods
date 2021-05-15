using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Harmony;
using Klei.AI;

namespace CustomizePlants
{
    [HarmonyPatch(typeof(MutantPlant), nameof(MutantPlant.ApplyMutations))]
    public class MutantPlant_AnalyzePatch
    {
        public static bool Prepare()
        {
            return CustomizePlantsState.StateManager.State.CheatMutationAnalyze;
        }

        public static void Postfix(MutantPlant __instance)
        {
            __instance.Analyze();
        }
    }

    //[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
    public class Db_Initialize_MutationsPatch
    {
        public static void Postfix()
        {
            var mutations = Db.Get().PlantMutations.resources;

            foreach (var mutation in mutations)
            {
                //Helpers.PrintDebug($"{mutation.SelfModifiers}");
            }
        }
    }

}
