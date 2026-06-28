using HarmonyLib;
using Shared;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeBuildings
{
    public class TidalTurbine : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == ReefGeneratorConfig.ID;
        }

        public void EditDef(BuildingDef def)
        {
            def.GeneratorWattageRating = CustomizeBuildingsState.Instance.TidalTurbinePower;
        }

        public void EditGO(BuildingDef def)
        {
        }
    }

    [HarmonyPatch(typeof(SmallReefGeyserConfig), nameof(SmallReefGeyserConfig.CreatePrefab))]
    public static class TidalTurbineReefPatch
    {
        public static float ProducePerSecond;
        public static Tag[] RestrictTags = [];

        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.TidalTurbineInhaleRate != 500f
                || CustomizeBuildingsState.Instance.TidalTurbineExhaleRate != 166.66667f;
        }

        public static void Postfix(ref GameObject __result)
        {
            var inhaleRate = Math.Max(0.001f, CustomizeBuildingsState.Instance.TidalTurbineInhaleRate);
            float exhaleRate = Math.Max(0.001f, CustomizeBuildingsState.Instance.TidalTurbineExhaleRate);

            var consumer = __result.GetComponent<ElementConsumer>();
            consumer.consumptionRate = inhaleRate;

            var breath = __result.GetDef<BreathingGeyser.Def>();
            breath.inhaleRate = inhaleRate;
            breath.exhaleRate = exhaleRate;

            if (CustomizeBuildingsState.Instance.TidalTurbineExhaleRate <= 0f)
            {
                FumiKMod.instance?.Patch(typeof(BreathingGeyser.Instance).GetMethod(nameof(BreathingGeyser.Instance.ExhaleUpdate)),
                    prefix: new(exhaleNull));

                static bool exhaleNull()
                {
                    return false;
                }
            }
            else if (CustomizeBuildingsState.Instance.TidalTurbineProduce > 0f)
            {
                ProducePerSecond = CustomizeBuildingsState.Instance.TidalTurbineProduce;
                RestrictTags = CustomizeBuildingsState.Instance.TidalTurbineProduceFilter.Split(';').Select(s => (Tag)s).ToArray();
                if (RestrictTags.FirstOrDefault() == default)
                    RestrictTags = [];

                FumiKMod.instance?.Patch(typeof(BreathingGeyser.Instance).GetMethod(nameof(BreathingGeyser.Instance.ExhaleUpdate)),
                    transpiler: new(exhaleTranspiler));

                static IEnumerable<CodeInstruction> exhaleTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
                {
                    var tool = new TranspilerTool(instructions, generator, original);
                    tool.ReplaceAllCalls(typeof(Pickupable), nameof(Pickupable.Take), patch);
                    return tool;
                }

                static Pickupable patch(Pickupable instance, float amount)
                {
                    var units = instance.primaryElement.Units;
                    if (units > ProducePerSecond && instance.primaryElement.HasAnyTags(RestrictTags))
                    {
                        instance.primaryElement.Units = units + Math.Min(amount, ProducePerSecond);
                    }
                    return instance.Take(amount);
                }
            }
        }
    }
}
