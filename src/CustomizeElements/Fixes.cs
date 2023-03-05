using Common;
using HarmonyLib;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeElements
{
    [HarmonyPatch(typeof(MinionVitalsPanel), nameof(MinionVitalsPanel.GetIrrigationLabel))]
    public class Fixes
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            try
            {
                var data = new TranspilerTool(instructions, generator, original);

                data.Seek(typeof(ElementLoader), nameof(ElementLoader.GetElement));
                data.InsertAfter(Patch);

                return data;
            }
            catch (Exception e)
            {
                Helpers.Print(e.ToString());
                return instructions;
            }
        }

        public static Element Patch(Element _stack, [LocalParameter(indexByType: 0)] PlantElementAbsorber.ConsumeInfo consumeInfo)
        {
            return _stack ?? new Element() { name = consumeInfo.tag.ToString() };
        }
    }
}
