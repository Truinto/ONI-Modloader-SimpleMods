using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;

namespace LessIrrigation
{
    [HarmonyPatch(typeof(PrickleFlowerConfig), "CreatePrefab")]
    public static class PrickleFlowerConfigMod
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();
            foreach (CodeInstruction codeInstruction in code)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_R4
                    && (float)codeInstruction.operand == (float)0.03333334f)
                {
                    codeInstruction.operand = 0.00833333f;
                }
                yield return codeInstruction;
            }
        }
    }

    [HarmonyPatch(typeof(ColdWheatConfig), "CreatePrefab")]
    public static class ColdWheatConfigMod
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();
            foreach (CodeInstruction codeInstruction in code)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_R4
                    && (float)codeInstruction.operand == (float)0.03333334f)
                {
                    codeInstruction.operand = 0.00833333f;
                }
                else if (codeInstruction.opcode == OpCodes.Ldc_R4
                    && (float)codeInstruction.operand == (float)0.008333334f)
                {
                    codeInstruction.operand = 0.00000001f;
                }
                yield return codeInstruction;
            }
        }
    }

    [HarmonyPatch(typeof(SpiceVineConfig), "CreatePrefab")]
    public static class SpiceVineConfigMod
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();
            foreach (CodeInstruction codeInstruction in code)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_R4
                    && (float)codeInstruction.operand == (float)0.05833333f)
                {
                    codeInstruction.operand = 0.0116667f;
                }
                yield return codeInstruction;
            }
        }
    }

    [HarmonyPatch(typeof(ForestTreeConfig), "CreatePrefab")]
    public static class ForestTreeConfigMod
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();
            foreach (CodeInstruction codeInstruction in code)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_R4
                    && (float)codeInstruction.operand == (float)0.1166667f)
                {
                    codeInstruction.operand = 0.0166667f;
                }
                yield return codeInstruction;
            }
        }
    }
}
