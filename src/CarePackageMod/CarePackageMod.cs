using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection.Emit;
using System.Reflection;
using Common;
using Shared;

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

    [HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.OnSpawn))]
    public class FixReload
    {
        public static void Postfix()
        {
            CarePackageMod.dirty = true;
        }
    }

    [HarmonyPatch(typeof(Immigration), nameof(Immigration.RandomCarePackage))]
    public class CarePackageMod
    {
        public static bool dirty = true;
        public static List<CarePackageInfo> carePackages;

        public static void Prefix(Immigration __instance)
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
                __instance.carePackages = carePackages;
                dirty = false;
            }
        }
    }

    [HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.InitializeContainers))]
    public class InitializeContainers
    {
        public static bool Prepare()
        {
            return true;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator generator, MethodBase original)
        {
            var tool = new TranspilerTool(code, generator, original);

            tool.Last();
            tool.Rewind(OpCodes.Stfld, AccessTools.Field(typeof(CharacterSelectionController), nameof(CharacterSelectionController.numberOfDuplicantOptions)));
            tool.InsertAfter(patch1);

            if (CarePackageState.StateManager.State.rosterIsOrdered)
                tool.ReplaceAllCalls(typeof(UnityEngine.Transform), nameof(UnityEngine.Transform.SetSiblingIndex), patch2);

            return tool;

            void patch1(CharacterSelectionController __instance)
            {
                __instance.numberOfDuplicantOptions = CarePackageState.StateManager.State.rosterDupes;
                __instance.numberOfCarePackageOptions = CarePackageState.StateManager.State.rosterPackages;
            }

            void patch2(UnityEngine.Transform instance, int index)
            {
            }
        }
    }
}