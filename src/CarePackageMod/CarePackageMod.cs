using Harmony;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection.Emit;
using System.Reflection;

namespace CarePackageMod
{
    //[HarmonyPatch(typeof(Immigration), "ConfigureCarePackages")]
    [HarmonyPatch(typeof(Immigration), "RandomCarePackage")]
    public class CarePackageMod
    {
        public static CarePackageInfo[] carePackages = null;

        public static void Prefix(ref CarePackageInfo[] ___carePackages)
        {
            if (carePackages == null)
            {
                List<CarePackageInfo> list = new List<CarePackageInfo>();

                Debug.Log("Setting up care packages:");

                for (int i = 0; i < CarePackageState.StateManager.State.CarePackages.Count(); i++)
                {
                    CarePackageContainer container = CarePackageState.StateManager.State.CarePackages[i];
                    container.multiplier = CarePackageState.StateManager.State.multiplier;

                    if (Assets.TryGetPrefab(container.ID) != null)
                    {
                        Debug.Log($" id: {container.ID} quantity: {container.amount} multiplier: {container.multiplier}");
                        list.Add(container.ToInfo());
                    }
                    else
                    {
                        Debug.Log("[CarePackageMod] Illegal Prefab: " + container.ID);
                    }
                }

                carePackages = list.ToArray();
            }

            ___carePackages = carePackages;
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

                if (line.opcode == OpCodes.Ldc_I4_1)            //54	0089	ldc.i4.1
                {
                    // line.opcode = OpCodes.Ldc_I4;
                    // line.operand = CarePackageState.StateManager.State.rosterPackages;
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = typeof(InitializeContainers).GetField(nameof(InitializeContainers.CarePackages));
                    Debug.Log($"[CarePackageMod] Patched Ldc_I4_1 at {i} with {line.operand}");
                }
                else if (line.opcode == OpCodes.Ldc_I4_2)                 //56	008C	ldc.i4.2
                {
                    // line.opcode = OpCodes.Ldc_I4;
                    // line.operand = CarePackageState.StateManager.State.rosterPackages;
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = typeof(InitializeContainers).GetField(nameof(InitializeContainers.CarePackages));
                    Debug.Log($"[CarePackageMod] Patched Ldc_I4_2 at {i} with {line.operand}");
                }
                else if (line.opcode == OpCodes.Ldc_I4_4)            //59	0093	ldc.i4.4
                {
                    // line.opcode = OpCodes.Ldc_I4;
                    // line.operand = CarePackageState.StateManager.State.rosterDupes + CarePackageState.StateManager.State.rosterPackages;
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = typeof(InitializeContainers).GetField(nameof(InitializeContainers.Total));
                    Debug.Log($"[CarePackageMod] Patched Ldc_I4_4 at {i} with {line.operand}");
                }
            }

            if (CarePackageState.StateManager.State.rosterIsOrdered && codeList.Count >= 128)
            {
                for (int i = 119; i <= 128; i++)
                    codeList[i].opcode = OpCodes.Nop;
            }

            return codeList;
        }

    }
}