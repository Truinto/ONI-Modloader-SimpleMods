using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection.Emit;
using System.Reflection;
using Common;

namespace CarePackageMod
{
    public class DuplicantStats
    {
        public static void OnLoad()
        {
            var array = new int[13];
            for (int i = 0; i < array.Length; i++)
                array[i] = 1;
            array[0] = 7;
            array[1] = 3;

            AccessTools.Field(typeof(TUNING.DUPLICANTSTATS), nameof(TUNING.DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES))
                .SetValue(null, array);
        }
    }

    //[HarmonyPatch(typeof(Immigration), "ConfigureCarePackages")]
    [HarmonyPatch(typeof(Immigration), "RandomCarePackage")]
    public class CarePackageMod
    {
        public static bool dirty = true;
        public static List<CarePackageInfo> carePackages;

        public static void Prefix(ref CarePackageInfo[] ___carePackages)
        {
            if (!CarePackageState.StateManager.State.loadPackages)
                return;

            if (carePackages == null)
            {
                carePackages = new List<CarePackageInfo>();

                Helpers.Print("Setting up care packages:");

                for (int i = 0; i < CarePackageState.StateManager.State.CarePackages.Count(); i++)
                {
                    var container = CarePackageState.StateManager.State.CarePackages[i];
                    container.multiplier = CarePackageState.StateManager.State.multiplier;

                    if (Assets.TryGetPrefab(container.ID) != null)
                    {
                        Helpers.Print($" id: {container.ID} quantity: {container.amount} multiplier: {container.multiplier}");
                        carePackages.Add(container.ToInfo());
                    }
                    else
                    {
                        Helpers.Print("Illegal Prefab: " + container.ID);
                    }
                }
                dirty = true;
            }

            if (dirty)
            {
                ___carePackages = carePackages.ToArray();
                dirty = false;
            }
        }
    }

    [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
    public class InitializeContainers
    {
        public static bool Prepare()
        {
            if (!CarePackageState.StateManager.State.biggerRoster)
                return false;
            Total = CarePackageState.StateManager.State.rosterDupes + CarePackageState.StateManager.State.rosterPackages;
            CarePackages = CarePackageState.StateManager.State.rosterPackages;
            return true;
        }

        public static int Total = 4;
        public static int CarePackages = 1;


        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> codeList = instr.ToList();

            var referenceMethod = AccessTools.Method(typeof(UnityEngine.Random), "Range", new Type[] { typeof(int), typeof(int) });

            bool flag1 = true;
            bool flag2 = true;
            bool flag3 = true;
            int flag = -1;
            for (int i = 0; i < codeList.Count; i++)
            {
                CodeInstruction line = codeList[i];

                if (flag < 0 && line.opcode == OpCodes.Call && (line.operand as MethodInfo) == referenceMethod)
                {
                    //51	0080	call	int32 [UnityEngine.CoreModule]UnityEngine.Random::Range(int32, int32)
                    flag = i + 15;
                }

                if (flag < 0 || i > flag)
                    continue;

                if (flag1 && line.opcode == OpCodes.Ldc_I4_1)            //54	0089	ldc.i4.1
                {
                    // line.opcode = OpCodes.Ldc_I4;
                    // line.operand = CarePackageState.StateManager.State.rosterPackages;
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = typeof(InitializeContainers).GetField(nameof(InitializeContainers.CarePackages));
                    Helpers.Print($"Patched InitializeContainers:Ldc_I4_1 at {i} with {line.operand}");
                    flag1 = false;
                }
                else if (flag2 && line.opcode == OpCodes.Ldc_I4_2)                 //56	008C	ldc.i4.2
                {
                    // line.opcode = OpCodes.Ldc_I4;
                    // line.operand = CarePackageState.StateManager.State.rosterPackages;
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = typeof(InitializeContainers).GetField(nameof(InitializeContainers.CarePackages));
                    Helpers.Print($"Patched InitializeContainers:Ldc_I4_2 at {i} with {line.operand}");
                    flag2 = false;
                }
                else if (flag3 && line.opcode == OpCodes.Ldc_I4_4)            //59	0093	ldc.i4.4
                {
                    // line.opcode = OpCodes.Ldc_I4;
                    // line.operand = CarePackageState.StateManager.State.rosterDupes + CarePackageState.StateManager.State.rosterPackages;
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = typeof(InitializeContainers).GetField(nameof(InitializeContainers.Total));
                    Helpers.Print($"Patched InitializeContainers:Ldc_I4_4 at {i} with {line.operand}");
                    flag3 = false;
                }
            }

            if (flag1 || flag2 || flag3)
                Helpers.Print($"Error patch InitializeContainers failed {flag1}:{flag2}:{flag3}");

            if (CarePackageState.StateManager.State.rosterIsOrdered && codeList.Count >= 128)
            {
                for (int i = 119; i <= 128; i++)
                    codeList[i].opcode = OpCodes.Nop;
            }

            return codeList;
        }

    }
}