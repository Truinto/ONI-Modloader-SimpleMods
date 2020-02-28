using Harmony;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection.Emit;

namespace CarePackageMod
{
    [HarmonyPatch(typeof(Immigration), "ConfigureCarePackages")]
    public class CarePackageMod
    {
        public static CarePackageInfo[] carePackages = null;

        internal static bool Prefix(Immigration __instance, ref CarePackageInfo[] ___carePackages)
        {
            if (!CarePackageState.StateManager.State.enabled) return true;

            if (carePackages == null)
            {
                List<CarePackageInfo> list = new List<CarePackageInfo>();
				carePackages = new CarePackageInfo[CarePackageState.StateManager.State.CarePackages.Count()];
                float multiplier = CarePackageState.StateManager.State.multiplier;
				
                Debug.Log("Setting up care packages:");
				
				for (int i = 0; i < carePackages.Count(); i++)
				{
					CarePackageContainer container = CarePackageState.StateManager.State.CarePackages[i];
					container.amount = (int)Math.Max(Math.Round(container.amount * multiplier, 0), 1f);
                    Debug.Log(" id: " + container.ID + " quantity: " + container.amount);
					carePackages[i] = container.ToInfo();
				}
                // foreach (CarePackageContainer container in CarePackageState.StateManager.State.CarePackages)
                // {
                //     container.amount = (int)Math.Max(Math.Round(container.amount * multiplier, 0), 1f);
                //     Debug.Log(" id: " + container.ID + " quantity: " + container.amount);
                //     list.Add(container.ToInfo());
                // }
                // carePackages = list.ToArray();
            }

            ___carePackages = carePackages;

            return false;
        }
    }
	
	public class CarePackageAPI
	{
		/// Loads the config State
		public static bool Reload()
		{
			if (CarePackageState.StateManager == null || CarePackageState.StateManager.State == null) return false;
			return OverridePackages(CarePackageState.StateManager.State.CarePackages);
		}
		
		/// creates CarePackageInfo array and overrides vanilla values
		/// should work at any point and persist loads
		/// returns true when load successful
		public static bool OverridePackages(List<CarePackageContainer> containerList)
		{
			return OverridePackages(containerList.ToArray());
		}
		
		/// creates CarePackageInfo array and overrides vanilla values
		/// should work at any point and persist loads
		/// returns true when load successful
		public static bool OverridePackages(CarePackageContainer[] containerList)
		{
			if (containerList == null) return false;
			
			CarePackageMod.carePackages = new CarePackageInfo[containerList.Count()];
			
			for (int i = 0; i < CarePackageMod.carePackages.Count(); i++)
				CarePackageMod.carePackages[i] = containerList[i].ToInfo();
			
			OverridePackages(CarePackageMod.carePackages, false);	// doesn't need to save, since we loaded everything into the save already
			return true;
		}
		
		/// directly overrides values
		/// if save == true will apply values whenever needed, otherwise might get overriden on load of a save-file
		public static void OverridePackages(CarePackageInfo[] packageList, bool save = true)
		{
			if (save) CarePackageMod.carePackages = packageList;	// saves the list so the config doesn't override it back
			
			if (Immigration.Instance == null) return;	// instance must be loaded; if Instance == null and save == true then it will load it later
			
			AccessTools.Field(typeof(Immigration), "carePackages").SetValue(Immigration.Instance, packageList);
		}
		
		/// returns the currently set CarePackageInfo array, is null if not initialized AND not set
		public static CarePackageInfo[] GetPackages()
		{
			if (Immigration.Instance == null) return CarePackageMod.carePackages;
			else return (CarePackageInfo[])AccessTools.Field(typeof(Immigration), "carePackages").GetValue(Immigration.Instance);
		}
	}
	
    [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
    internal class CharacterSelectionController_InitializeContainers
    {
        private static bool Prepare()
        {
            return CarePackageState.StateManager.State.biggerRoster;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> code = instr.ToList();

            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction codeInstruction = code[i];
                //Debug.Log(codeInstruction.ToString());
                if (i >= 49)
                    continue;
                
                if (codeInstruction.opcode == OpCodes.Ldc_I4_2)                 //40    0086    ldc.i4.2
                {
                    //codeInstruction.opcode = CarePackageState.StateManager.State.rosterPackages.ToOpCode();
                    codeInstruction.opcode = OpCodes.Ldc_I4;
                    codeInstruction.operand = CarePackageState.StateManager.State.rosterPackages;
                }
                else if (codeInstruction.opcode == OpCodes.Ldc_I4_1)            //42	008C	ldc.i4.1
                {
                    //codeInstruction.opcode = CarePackageState.StateManager.State.rosterPackages.ToOpCode();
                    codeInstruction.opcode = OpCodes.Ldc_I4;
                    codeInstruction.operand = CarePackageState.StateManager.State.rosterPackages;
                }
                else if (codeInstruction.opcode == OpCodes.Ldc_I4_4)            //45	007E	ldc.i4.4
                {
                    //codeInstruction.opcode = (CarePackageState.StateManager.State.rosterDupes + CarePackageState.StateManager.State.rosterPackages).ToOpCode();
                    codeInstruction.opcode = OpCodes.Ldc_I4;
                    codeInstruction.operand = CarePackageState.StateManager.State.rosterDupes + CarePackageState.StateManager.State.rosterPackages;
                }
            }

            if (CarePackageState.StateManager.State.rosterIsOrdered)
            {
                for (int i = 94; i <= 103; i++)
                    code[i].opcode = OpCodes.Nop;
            }

            return (IEnumerable<CodeInstruction>)code;
        }

    }

    public static class MyExtensions
    {
        public static OpCode ToOpCode(this int integer)
        {
            switch (integer)
            {
                case 0:
                    return OpCodes.Ldc_I4_0;
                case 1:
                    return OpCodes.Ldc_I4_1;
                case 2:
                    return OpCodes.Ldc_I4_2;
                case 3:
                    return OpCodes.Ldc_I4_3;
                case 4:
                    return OpCodes.Ldc_I4_4;
                case 5:
                    return OpCodes.Ldc_I4_5;
                case 6:
                    return OpCodes.Ldc_I4_6;
                case 7:
                    return OpCodes.Ldc_I4_7;
                case 8:
                    return OpCodes.Ldc_I4_8;
                default:
                    return OpCodes.Ldc_I4_1;
            }
        }
    }

}