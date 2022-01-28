using Common;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(SolarPanel), "EnergySim200ms")]
    public class SolarPanel_SolarMaxPower
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SolarMaxPower != 380f;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            bool flag1 = true;
            foreach (var codeInstruction in instr)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_R4 && (float)codeInstruction.operand == 380f)
                {
                    codeInstruction.operand = CustomizeBuildingsState.StateManager.State.SolarMaxPower;
                    flag1 = false;
                }
                yield return codeInstruction;
            }

            if (flag1)
                Helpers.Print("Error patch SolarPanel_SolarMaxPower failed");
        }
    }

    [HarmonyPatch(typeof(SolarPanelConfig), "CreateBuildingDef")]
    public class SolarPanelConfig_CreateBuildingDef
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SolarMaxPower != 380f;
        }

        public static void Postfix(BuildingDef __result)
        {
            __result.GeneratorWattageRating = CustomizeBuildingsState.StateManager.State.SolarMaxPower;
        }
    }

    [HarmonyPatch(typeof(SolarPanel), "EnergySim200ms")]
    public class SolarPanel_SolarEnergyMultiplier
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SolarEnergyMultiplier != 1f;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            bool flag1 = true;
            foreach (var codeInstruction in instr)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_R4 && (float)codeInstruction.operand == 0.00053f)
                {
                    codeInstruction.operand = 0.00053f * CustomizeBuildingsState.StateManager.State.SolarEnergyMultiplier;
                    flag1 = false;
                }
                yield return codeInstruction;
            }

            if (flag1)
                Helpers.Print("Error patch SolarPanel_SolarEnergyMultiplier failed");
        }
    }
}