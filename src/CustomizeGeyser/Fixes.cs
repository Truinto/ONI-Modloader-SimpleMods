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
    [HarmonyPatch]
    public class Fixes
    {
        [HarmonyPatch(typeof(AllResourcesScreen), "SpawnCategoryRow")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler1(IEnumerable<CodeInstruction> instr)
        {
            var original = AccessTools.Method(typeof(Debug), nameof(Debug.LogError), new Type[] { typeof(object) });

            foreach (var line in instr)
            {
                if (line.Calls(original))
                    line.ReplaceCall(Patch);

                yield return line;
            }
        }

        public static void Patch(object obj)
        {
            Debug.Log(obj);
        }
    }
}
