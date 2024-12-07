using HarmonyLib;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PipedEverything
{
    //[HarmonyPatch]
    public static class Patch_OriginalPort
    {
        //[HarmonyPatch(typeof(BuildingConfigManager), nameof(BuildingConfigManager.RegisterBuilding))]
        //[HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> BuildingConfigManager_RegisterBuilding(IEnumerable<CodeInstruction> code, ILGenerator generator, MethodBase original)
        {
            var tool = new TranspilerTool(code, generator, original);

            tool.Seek(typeof(IBuildingConfig), nameof(IBuildingConfig.CreateBuildingDef));
            tool.InsertAfter(buildingDefOverride);

            return tool;

            static BuildingDef buildingDefOverride(BuildingDef buildingDef)
            {
                return buildingDef;
            }
        }
    }
}
