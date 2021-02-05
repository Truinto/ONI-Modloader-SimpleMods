using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Harmony;

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
        public static void Prefix(ref int __result)
        {
            __result += CarePackageState.StateManager.State.attributeBonusChance;
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]
    public class Reshuffle4
    {
        public static bool Prepare()
        {
            NumberOfInterests = CarePackageState.StateManager.State.always3Interests ? 3 : 1;
            return true;
        }

        public static int NumberOfInterests = 1;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach (var line in instr)
            {
                if (line.opcode == OpCodes.Ldc_I4_1)
                {
                    // line.opcode = OpCodes.Ldc_I4;
                    // line.operand = 3;
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = typeof(Reshuffle4).GetField(nameof(Reshuffle4.NumberOfInterests));
                    Debug.Log($"[CarePackageMod] Patched GenerateAptitudes Ldc_I4_1 with {line.operand}");
                    break;
                }
            }
            return instr;
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
