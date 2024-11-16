using Common;
using HarmonyLib;
using Shared;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PipedEverything
{
    public class Patches_AdvancedGenerators
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            Helpers.PrintDebug($"Patches_AdvancedGenerators {original}");

            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(EnergyGenerator.OutputItem), nameof(EnergyGenerator.OutputItem.store));
            data.InsertAfter(shouldStore);

            return data;

            bool shouldStore(bool storeOutput, KMonoBehaviour __instance, [LocalParameter(IndexByType = 0)] Element element)
            {
                return storeOutput || __instance.GetComponent<PortDisplayController>()?.IsOutputConnected(element) == true;
            }
        }
    }
}
