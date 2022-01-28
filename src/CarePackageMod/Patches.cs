using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Common;
using HarmonyLib;

namespace CarePackageMod
{
    [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.SetReshufflingState))]
    public class Reshuffle1
    {
        public static bool Prepare()
        {
            return CarePackageState.StateManager.State.allowReshuffle;
        }

        public static void Prefix(ref bool enable)
        {
            enable = true;
        }
    }

    [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
    public class Reshuffle2
    {
        public static void Prefix(ref bool is_starter)
        {
            if (CarePackageState.StateManager.State.removeStarterRestriction)
                is_starter = false;
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), "GenerateTraits")]
    public class Reshuffle3
    {
        public static void Postfix(ref int __result)
        {
            __result += CarePackageState.StateManager.State.attributeBonusChance;
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]
    public class Reshuffle4
    {
        public static bool Prepare()
        {
            MinInterests = Math.Max(0, CarePackageState.StateManager.State.minNumberofInterests);
            MaxInterests = Math.Max(MinInterests + 1, CarePackageState.StateManager.State.maxNumberofInterests + 1);
            return true;
        }

        public static int MinInterests = 1;
        public static int MaxInterests = 4;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            var lines = instr.ToList();
            bool flag1 = true;
            for (int i = 0; i < lines.Count - 2; i++)
            {
                if (lines[i].opcode == OpCodes.Ldc_I4_1
                    && lines[i + 1].opcode == OpCodes.Ldc_I4_4
                    && lines[i + 2].opcode == OpCodes.Call)
                {
                    lines[i].opcode = OpCodes.Ldsfld;
                    lines[i].operand = typeof(Reshuffle4).GetField(nameof(Reshuffle4.MinInterests));
                    i++;
                    lines[i].opcode = OpCodes.Ldsfld;
                    lines[i].operand = typeof(Reshuffle4).GetField(nameof(Reshuffle4.MaxInterests));

                    Helpers.Print($"Patched GenerateAptitudes Ldc_I4_1 with {lines[i].operand}");
                    flag1 = false;
                    break;
                }
            }

            if (flag1)
                Helpers.Print("Error patch GenerateAptitudes failed");

            return lines;
        }
    }

    [HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.AddDeliverable))]
    public class FixReshuffle
    {
        public static bool Prepare()
        {
            return CarePackageState.StateManager.State.allowReshuffle;
        }

        public static void Prefix(CharacterSelectionController __instance, List<ITelepadDeliverable> ___selectedDeliverables, ref bool ___allowsReplacing)
        {
            __instance.RemoveLast();

            //var copy = new List<ITelepadDeliverable>(___selectedDeliverables);
            //foreach (var sub in copy)
            //    __instance.RemoveDeliverable(sub);

            //___selectedDeliverables.Clear();
        }
    }
}
