using Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeGeyser
{
    [HarmonyPatch(typeof(AllResourcesScreen), "SpawnCategoryRow")]
    public class Fixes
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            var original = AccessTools.Method(typeof(Dictionary<Tag, GameUtil.MeasureUnit>), nameof(Dictionary<Tag, GameUtil.MeasureUnit>.Add));

            foreach (var line in instr)
            {
                if (line.Calls(original))
                    line.ReplaceCall(typeof(Dictionary<Tag, GameUtil.MeasureUnit>), "set_Item");

                yield return line;
            }
        }
    }
}
