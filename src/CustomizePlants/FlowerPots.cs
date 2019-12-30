using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace CustomizePlants
{
    public static class FlowerPotPatch
    {
        public static void SetPlantablePlotAllTags(GameObject go, bool hasLiquidPiping = false)
        {
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            (AccessTools.Field(typeof(PlantablePlot), "possibleDepositTagsList").GetValue(plantablePlot) as List<Tag>)?.Clear();
            plantablePlot.AddDepositTag(GameTags.CropSeed);
            plantablePlot.AddDepositTag(GameTags.WaterSeed);
            plantablePlot.AddDepositTag(GameTags.DecorSeed);
            plantablePlot.SetFertilizationFlags(true, hasLiquidPiping);
        }

        public static void EnableCheats(GameObject go)
        {
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            AccessTools.Field(typeof(PlantablePlot), "accepts_irrigation").SetValue(plantablePlot, false);
            AccessTools.Field(typeof(PlantablePlot), "accepts_fertilizer").SetValue(plantablePlot, false);
        }
    }

    [HarmonyPatch(typeof(FlowerVaseConfig), "ConfigureBuildingTemplate")]
    public static class FlowerVaseConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (CustomizePlants.CustomizePlantsState.StateManager.State.SeedsGoIntoAnyFlowerPots)
                FlowerPotPatch.SetPlantablePlotAllTags(go);

            if (CustomizePlantsState.StateManager.State.CheatFlowerVase)
                FlowerPotPatch.EnableCheats(go);
        }
    }

    [HarmonyPatch(typeof(PlanterBoxConfig), "ConfigureBuildingTemplate")]
    public static class PlanterBoxConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (CustomizePlants.CustomizePlantsState.StateManager.State.SeedsGoIntoAnyFlowerPots)
                FlowerPotPatch.SetPlantablePlotAllTags(go);
        }
    }

    [HarmonyPatch(typeof(FarmTileConfig), "ConfigureBuildingTemplate")]
    public static class FarmTileConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (CustomizePlants.CustomizePlantsState.StateManager.State.SeedsGoIntoAnyFlowerPots)
                FlowerPotPatch.SetPlantablePlotAllTags(go);
        }
    }

    [HarmonyPatch(typeof(HydroponicFarmConfig), "ConfigureBuildingTemplate")]
    public static class HydroponicFarmConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (CustomizePlants.CustomizePlantsState.StateManager.State.SeedsGoIntoAnyFlowerPots)
                FlowerPotPatch.SetPlantablePlotAllTags(go, true);
        }
    }

    [HarmonyPatch(typeof(FlowerVaseHangingConfig), "ConfigureBuildingTemplate")]
    public static class FlowerVaseHangingConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (CustomizePlants.CustomizePlantsState.StateManager.State.SeedsGoIntoAnyFlowerPots)
                FlowerPotPatch.SetPlantablePlotAllTags(go);
        }
    }

    [HarmonyPatch(typeof(FlowerVaseHangingFancyConfig), "ConfigureBuildingTemplate")]
    public static class FlowerVaseHangingFancyConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (CustomizePlants.CustomizePlantsState.StateManager.State.SeedsGoIntoAnyFlowerPots)
                FlowerPotPatch.SetPlantablePlotAllTags(go);
        }
    }

    [HarmonyPatch(typeof(FlowerVaseWallConfig), "ConfigureBuildingTemplate")]
    public static class FlowerVaseWallConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (CustomizePlants.CustomizePlantsState.StateManager.State.SeedsGoIntoAnyFlowerPots)
                FlowerPotPatch.SetPlantablePlotAllTags(go);
        }
    }
}