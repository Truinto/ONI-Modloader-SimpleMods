using Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeGeyser
{
    //[HarmonyPatch(typeof(AllResourcesScreen), "SpawnCategoryRow")]
    public class Fixes
    {
        public static void Unused_Prefix(Tag categoryTag, AllResourcesScreen __instance, Dictionary<Tag, GameObject> ___categoryRows)
        {
            if (!___categoryRows.ContainsKey(categoryTag))
            {
                __instance.currentlyDisplayedRows.Remove(categoryTag);
                __instance.units.Remove(categoryTag);
            }
        }

        public static IEnumerable<CodeInstruction> Unused_Transpiler(IEnumerable<CodeInstruction> instr)
        {
            var original = AccessTools.Method(typeof(Dictionary<Tag, GameUtil.MeasureUnit>), nameof(Dictionary<Tag, GameUtil.MeasureUnit>.Add));

            int i = 0;
            foreach (var line in instr)
            {
                if (line.Calls(original))
                {
                    Helpers.Print("Transpiler AllResourcesScreen at " + i);
                    line.ReplaceCall(typeof(Dictionary<Tag, GameUtil.MeasureUnit>), "set_Item");
                    Helpers.Print($"Transpiler AllResourcesScreen at {i} {(line.operand as MethodInfo)?.FullDescription()}");
                }

                yield return line;
                i++;
            }
        }
    }
}
