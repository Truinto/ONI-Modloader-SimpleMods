using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Common;
using HarmonyLib;
using Shared;

namespace CarePackageMod
{
    [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.SetReshufflingState))]
    public class Reshuffle1
    {
        public static void Prefix(ref bool enable)
        {
            if (CarePackageState.StateManager.State.allowReshuffle)
                enable = true;
        }
    }

    [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.GenerateCharacter))]
    public class Reshuffle2
    {
        public static void Prefix(ref bool is_starter)
        {
            if (CarePackageState.StateManager.State.removeStarterRestriction)
                is_starter = false;
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), nameof(MinionStartingStats.GenerateTraits))]
    public class Reshuffle3
    {
        public static void Postfix(ref int __result)
        {
            __result += CarePackageState.StateManager.State.attributeBonusChance;
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), nameof(MinionStartingStats.GenerateAptitudes))]
    public class Reshuffle4
    {
        public static bool Prepare()
        {
            MinInterests = Math.Max(1, CarePackageState.StateManager.State.minNumberofInterests);
            MaxInterests = Math.Max(MinInterests, Math.Min(CarePackageState.StateManager.State.maxNumberofInterests, TUNING.DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES.Length));
            MaxInterests += 1; // upper range is non inclusive
            return true;
        }

        public static int MinInterests = 1;
        public static int MaxInterests = 4;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator generator, MethodBase original)
        {
            var tool = new TranspilerTool(code, generator, original);

            tool.Seek(typeof(UnityEngine.Random), nameof(UnityEngine.Random.Range), [typeof(int), typeof(int)]);
            tool.ReplaceCall(patch);

            return tool;

            int patch(int minInclusive, int maxExclusive)
            {
                return UnityEngine.Random.Range(MinInterests, MaxInterests);
            }
        }
    }

    [HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.AddDeliverable))]
    public class FixReshuffle
    {
        public static bool Prepare()
        {
            return CarePackageState.StateManager.State.allowReshuffle;
        }

        public static void Prefix(CharacterSelectionController __instance)
        {
            __instance.RemoveLast();
        }
    }
}
