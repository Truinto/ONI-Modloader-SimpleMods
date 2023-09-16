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
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            Access.possibleDepositTagsList(plantablePlot).Clear();
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
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            Access.accepts_irrigation(plantablePlot) = false;
            Access.accepts_fertilizer(plantablePlot) = false;
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

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(IrrigationMonitor.Instance), nameof(IrrigationMonitor.Instance.UpdateIrrigation));
            yield return AccessTools.Method(typeof(IrrigationMonitor.Instance), nameof(IrrigationMonitor.Instance.UpdateAbsorbing));
            yield return AccessTools.Method(typeof(FertilizationMonitor.Instance), nameof(FertilizationMonitor.Instance.UpdateFertilization));
            yield return AccessTools.Method(typeof(FertilizationMonitor.Instance), nameof(FertilizationMonitor.Instance.StartAbsorbing));
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(Klei.AI.AttributeInstance), nameof(Klei.AI.AttributeInstance.GetTotalValue));
            data.InsertAfter(getTotalValue);

            return data;

            float getTotalValue(float modifier, StateMachine.Instance __instance)
            {
                if (__instance is FertilizationMonitor.Instance fert && fert.storage.GetComponent<KPrefabID>().PrefabTag == FlowerVaseTag)
                    return 0f;
                if (__instance is IrrigationMonitor.Instance irr && irr.storage.GetComponent<KPrefabID>().PrefabTag == FlowerVaseTag)
                    return 0f;
                return modifier;
            }
        }
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