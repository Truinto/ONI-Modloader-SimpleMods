using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Common;
using System.Reflection;

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

    [HarmonyPatch(typeof(PlantablePlot), "RegisterWithPlant")]
    public class FlowerVase_Wild
    {
        public static bool Prepare()
        {
            return CustomizePlants.CustomizePlantsState.StateManager.State.WildFlowerVase;
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

    [HarmonyPatch(typeof(FlowerVaseConfig), "ConfigureBuildingTemplate")]
    public static class FlowerVaseConfig_Cheat
    {
        public static bool Prepare()
        {
            return CustomizePlantsState.StateManager.State.CheatFlowerVase;
        }

        public static void Postfix(GameObject go)
        {
            FlowerPotHelper.EnableCheats(go);
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