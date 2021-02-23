using Harmony;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;

namespace CustomizeBuildings
{
    public class Miscellaneous
    {
        public static void OnLoad()
        {
#if DLC1
            if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitDecay))
                TUNING.EQUIPMENT.SUITS.ATMOSUIT_DECAY = CustomizeBuildingsState.StateManager.State.TuningAtmosuitDecay;
            if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitAthletics))
                TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS = (int)CustomizeBuildingsState.StateManager.State.TuningAtmosuitAthletics;
            if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningOxygenMaskDecay))
                TUNING.EQUIPMENT.SUITS.OXYGEN_MASK_DECAY = CustomizeBuildingsState.StateManager.State.TuningOxygenMaskDecay;
#endif
            if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitScalding))
                TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING = (int)CustomizeBuildingsState.StateManager.State.TuningAtmosuitScalding;
            if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitInsulation))
                TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION = (int)CustomizeBuildingsState.StateManager.State.TuningAtmosuitInsulation;
            if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitThermalConductivityBarrier))
                TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER = CustomizeBuildingsState.StateManager.State.TuningAtmosuitThermalConductivityBarrier;
        }
    }

    public class Time_test
    {
        public static float daylength = 6000f;

        public static void OnXXXLoad()
        {
            var harmony = HarmonyInstance.Create("test");
            var transpiler = new HarmonyMethod(typeof(Time_test).GetMethod(nameof(Transpiler)));

            var originals = AccessTools.GetDeclaredMethods(typeof(GameClock));
            originals.AddRange(AccessTools.GetDeclaredMethods(typeof(TimerSideScreen)));
            originals.AddRange(AccessTools.GetDeclaredMethods(typeof(ReportManager)));
            //originals.AddRange(AccessTools.GetDeclaredMethods(typeof(EconomyDetails)));
            originals.AddRange(AccessTools.GetDeclaredMethods(typeof(TimeRangeSideScreen)));

            originals.Add(AccessTools.Method(typeof(GameUtil), nameof(GameUtil.ApplyTimeSlice)));
            originals.Add(AccessTools.Method("ColonyDiagnosticScreen.DiagnosticRow:DiagnosticRow"));

            foreach (var original in originals)
            {
                if (original == null)
                {
                    Debug.Log("wups it's a null");
                    continue;
                }

                try
                {
                    Debug.Log("[CustomGameClock] Patching " + original.Name);
                    harmony.Patch(original, transpiler: transpiler);
                }
                catch (Exception)
                {
                }
            }

            Debug.Log("[CustomGameClock] DONE");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            int i = 0;
            foreach (var line in instr)
            {
                if (line.opcode == OpCodes.Ldc_R4 && line.operand != null && (float)line.operand == 600f)
                {
                    line.operand = daylength;
                    Debug.Log("[CustomGameClock] Patched at line: " + i);
                }
                i++;
            }
            return instr;
        }
    }

    //[HarmonyPatch(typeof(TileConfig), "ConfigureBuildingTemplate")]
    //internal class TileDryWallReplacement
    //{
    //    internal static bool Prepare()
    //    {
    //        return true;
    //    }

    //    internal static void Postfix(GameObject go)
    //    {

    //    }
    //}


    //[HarmonyPatch(typeof(ExteriorWallConfig), "CreateBuildingDef", new Type[] { })]
    //internal class TileDryWallReplacement
    //{
    //    internal static bool Prepare()
    //    {
    //        return true;
    //    }

    //    internal static void Postfix(ref BuildingDef __result)
    //    {
    //        BuildingTemplates.CreateLadderDef(__result);
    //    }
    //}

}