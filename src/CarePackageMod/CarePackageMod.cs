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

    [HarmonyPatch(typeof(SaveLoader), "OnSpawn")]
    public class FixReload
    {
        public static void Postfix()
        {
            CarePackageMod.dirty = true;
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

            var methodRandomRange = AccessTools.Method(typeof(UnityEngine.Random), "Range", new Type[] { typeof(int), typeof(int) });
            var methodSetSiblingIndex = AccessTools.Method(typeof(Transform), nameof(Transform.SetSiblingIndex));

            bool flag1 = true;
            bool flag2 = true;
            bool flag3 = true;
            bool flag4 = CarePackageState.StateManager.State.rosterIsOrdered;
            int index = -1;
            for (int i = 0; i < codeList.Count; i++)
            {
                CodeInstruction line = codeList[i];

                if (flag4 && line.opcode == OpCodes.Callvirt && (line.operand as MethodInfo) == methodSetSiblingIndex)
                {
                    //line.opcode = OpCodes.Call; // crashes?!
                    //line.operand = AccessTools.Method(typeof(InitializeContainers), nameof(NullReplacement));
                    flag4 = false;
                }

                if (index < 0 && line.opcode == OpCodes.Call && (line.operand as MethodInfo) == methodRandomRange)
                {
                    //37	006B	call	int32 [UnityEngine.CoreModule]UnityEngine.Random::Range(int32, int32)
                    index = i + 15;
                }

                if (index < 0 || i > index)   // only change code from Random.Range() and 15 lines after
                    continue;

                // change this code to new number of packages:
                //  this.numberOfCarePackageOptions = ((Random.Range(0, 101) > 70) ? 2 : 1);
                //  this.numberOfDuplicantOptions = 4 - this.numberOfCarePackageOptions;

                if (flag1 && line.opcode == OpCodes.Ldc_I4_1)
                {
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = AccessTools.Field(typeof(InitializeContainers), nameof(InitializeContainers.CarePackages));
                    Helpers.Print($"Patched InitializeContainers:Ldc_I4_1 at {i} with {line.operand}");
                    flag1 = false;
                }
                else if (flag2 && line.opcode == OpCodes.Ldc_I4_2)
                {
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = AccessTools.Field(typeof(InitializeContainers), nameof(InitializeContainers.CarePackages));
                    Helpers.Print($"Patched InitializeContainers:Ldc_I4_2 at {i} with {line.operand}");
                    flag2 = false;
                }
                else if (flag3 && line.opcode == OpCodes.Ldc_I4_4)
                {
                    line.opcode = OpCodes.Ldsfld;
                    line.operand = AccessTools.Field(typeof(InitializeContainers), nameof(InitializeContainers.Total));
                    Helpers.Print($"Patched InitializeContainers:Ldc_I4_4 at {i} with {line.operand}");
                    flag3 = false;
                }
            }

            if (flag1 || flag2 || flag3 || flag4)
                Helpers.Print($"Error patch InitializeContainers failed {flag1}:{flag2}:{flag3}:{flag4}");

            return codeList;
        }

        public static void NullReplacement(int nix)
        {
        }

    }
}