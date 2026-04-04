using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Common;
using System.Reflection;
using Shared;

namespace CustomizePlants
{
    public static class FlowerPotHelper
    {
        public static void SetPlantablePlotAllTags(GameObject go)
        {
            var plantablePlot = go.AddOrGet<PlantablePlot>();
            plantablePlot.possibleDepositTagsList.Clear();
            plantablePlot.AddDepositTag(GameTags.CropSeed);
            plantablePlot.AddDepositTag(GameTags.WaterSeed);
            plantablePlot.AddDepositTag(GameTags.DecorSeed);
            plantablePlot.SetFertilizationFlags(true, plantablePlot.has_liquid_pipe_input);

            var storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            storage.capacityKg = 2000f;

            go.RemoveComponent<FlowerVase>();
            go.AddOrGet<PlanterBox>();
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Farm;
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGet<AnimTileable>();
        }

        public static void EnableCheats(GameObject go)
        {
            var plantablePlot = go.AddOrGet<PlantablePlot>();
            plantablePlot.accepts_irrigation = false;
            plantablePlot.accepts_fertilizer = false;
        }
    }

    [HarmonyPatch(typeof(PlantablePlot), nameof(PlantablePlot.RegisterWithPlant))]
    public class FlowerVase_Wild
    {
        public static bool Prepare()
        {
            return CustomizePlantsState.StateManager.State.WildFlowerVase;
        }

        public static bool Prefix(PlantablePlot __instance)
        {
            if (__instance.PrefabID() == FlowerVaseConfig.ID)
            {
                //Debug.Log($"prevented link to {__instance.PrefabID()}");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch]
    public static class FlowerVaseConfig_Cheat
    {
        public static Tag FlowerVaseTag = new(FlowerVaseConfig.ID);

        public static bool Prepare()
        {
            return CustomizePlantsState.StateManager.State.CheatFlowerVase;
        }

        [HarmonyPatch(typeof(IrrigationMonitor.Instance), nameof(IrrigationMonitor.Instance.UpdateIrrigation))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> IrrigationTranspiler1(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
            => IrrigationTranspiler2(instructions, generator, original);

        [HarmonyPatch(typeof(IrrigationMonitor.Instance), nameof(IrrigationMonitor.Instance.UpdateAbsorbing))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> IrrigationTranspiler2(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            try
            {
                data.Seek(typeof(Klei.AI.AttributeInstance), nameof(Klei.AI.AttributeInstance.GetTotalValue), []);
                data.InsertAfter(getTotalValue);
            } catch (Exception e)
            {
                Helpers.PrintDialog($"Please report this error: {e}");
            }

            return data;

            static float getTotalValue(float modifier, IrrigationMonitor.Instance __instance)
            {
                if (__instance.storage.GetComponent<KPrefabID>().PrefabTag == FlowerVaseTag)
                    return 0f;
                return modifier;
            }
        }


        [HarmonyPatch(typeof(FertilizationMonitor.Instance), nameof(FertilizationMonitor.Instance.UpdateFertilization))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> FertilizationTranspiler1(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
            => FertilizationTranspiler2(instructions, generator, original);

        [HarmonyPatch(typeof(FertilizationMonitor.Instance), nameof(FertilizationMonitor.Instance.StartAbsorbing))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> FertilizationTranspiler2(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);
            
            data.Last();
            data.Rewind(typeof(FertilizationMonitor.Instance), nameof(FertilizationMonitor.Instance.consumedElements));
            data.ReplaceCall(patch);

            return data;

            static PlantElementAbsorber.ConsumeInfo[] patch(FertilizationMonitor.Instance instance)
            {
                if (instance.storage.GetComponent<KPrefabID>().PrefabTag == FlowerVaseTag)
                    return _EmptyConsume;
                return instance.consumedElements;
            }
        }
        private static readonly PlantElementAbsorber.ConsumeInfo[] _EmptyConsume = [ new(SimHashes.Vacuum.ToTag(), 0f) ];
    }

    [HarmonyPatch]
    public class FlowerPots_Patches
    {
        public static bool Prepare()
        {
            return CustomizePlantsState.StateManager.State.SeedsGoIntoAnyFlowerPots;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return typeof(FlowerVaseConfig).GetMethod("ConfigureBuildingTemplate");
            yield return typeof(PlanterBoxConfig).GetMethod("ConfigureBuildingTemplate");
            yield return typeof(FarmTileConfig).GetMethod("ConfigureBuildingTemplate");
            yield return typeof(HydroponicFarmConfig).GetMethod("ConfigureBuildingTemplate");
            yield return typeof(FlowerVaseHangingConfig).GetMethod("ConfigureBuildingTemplate");
            yield return typeof(FlowerVaseHangingFancyConfig).GetMethod("ConfigureBuildingTemplate");
            yield return typeof(FlowerVaseWallConfig).GetMethod("ConfigureBuildingTemplate");
        }

        public static void Postfix(GameObject go)
        {
            FlowerPotHelper.SetPlantablePlotAllTags(go);
        }
    }
}
