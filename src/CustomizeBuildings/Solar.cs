using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(SolarPanel), "EnergySim200ms")]
    internal class SolarPanel_SolarMaxPower
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SolarMaxPower != 380f;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> code = instr.ToList();

            foreach (CodeInstruction codeInstruction in code)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_R4 && (float)codeInstruction.operand == 380f)
                {
                    codeInstruction.operand = CustomizeBuildingsState.StateManager.State.SolarMaxPower;
                }
                yield return codeInstruction;
            }
        }
    }

    [HarmonyPatch(typeof(SolarPanelConfig), "CreateBuildingDef")]
    internal class SolarPanelConfig_CreateBuildingDef
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SolarMaxPower != 380f;
        }

        internal static void Postfix(BuildingDef __result)
        {
            __result.GeneratorWattageRating = CustomizeBuildingsState.StateManager.State.SolarMaxPower;
        }
    }

    [HarmonyPatch(typeof(SolarPanel), "EnergySim200ms")]
    internal class SolarPanel_SolarEnergyMultiplier
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SolarEnergyMultiplier != 1f;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> code = instr.ToList();

            foreach (CodeInstruction codeInstruction in code)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_R4 && (float)codeInstruction.operand == 0.00053f)
                {
                    codeInstruction.operand = 0.00053f * CustomizeBuildingsState.StateManager.State.SolarEnergyMultiplier;
                }
                yield return codeInstruction;
            }
        }
    }
}