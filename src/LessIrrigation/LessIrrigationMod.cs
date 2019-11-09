using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace LessIrrigation
{
    public static class PLANTS
    {
        public static string[] NAMES = {
            BasicFabricMaterialPlantConfig.ID,
            BasicSingleHarvestPlantConfig.ID,
            BeanPlantConfig.ID,
            BulbPlantConfig.ID,
            CactusPlantConfig.ID,
            ColdBreatherConfig.ID,
            ColdWheatConfig.ID,
            EvilFlowerConfig.ID,
            ForestTreeConfig.ID,
            GasGrassConfig.ID,
            LeafyPlantConfig.ID,
            MushroomPlantConfig.ID,
            OxyfernConfig.ID,
            PrickleFlowerConfig.ID,
            PrickleGrassConfig.ID,
            SaltPlantConfig.ID,
            SpiceVineConfig.ID,
            SwampLilyConfig.ID
        };
    }

    #region plant configs

    [HarmonyPatch(typeof(BasicSingleHarvestPlantConfig), "CreatePrefab")]
    public static class BasicSingleHarvestPlantConfigMod
    {

        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(SeaLettuceConfig), "CreatePrefab")]
    public static class SeaLettuceMod
    {

        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(BasicFabricMaterialPlantConfig), "CreatePrefab")]
    public static class BasicFabricMaterialPlantConfigMod
    {

        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }
    
    [HarmonyPatch(typeof(BeanPlantConfig), "CreatePrefab")]
    public static class BeanPlantConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(BulbPlantConfig), "CreatePrefab")]
    public static class BulbPlantConfigMod
    {

        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(CactusPlantConfig), "CreatePrefab")]
    public static class CactusPlantConfigMod
    {

        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }
    
    [HarmonyPatch(typeof(ColdBreatherConfig), "CreatePrefab")]
    public static class ColdBreatherConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            ColdBreather coldBreather = __result.GetComponent<ColdBreather>();
            if (coldBreather != null)
            {
                coldBreather.deltaEmitTemperature = LessIrrigationState.StateManager.State.WheezewortTempDelta;
            }
            else Debug.LogWarning("ERROR: coldbreather was null!");

            PlantHelper.ProcessPlant(__result);

            //PlantHelper.ChangeIrrigation(__result, SimHashes.Phosphorite.CreateTag(), 0.9876f, true);
        }
    }

    [HarmonyPatch(typeof(ColdWheatConfig), "CreatePrefab")]
    public static class ColdWheatConfigMod
    {

        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(EvilFlowerConfig), "CreatePrefab")]
    public static class EvilFlowerConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(ForestTreeConfig), "CreatePrefab")]
    public static class ForestTreeConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(GasGrassConfig), "CreatePrefab")]
    public static class GasGrassConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(LeafyPlantConfig), "CreatePrefab")]
    public static class LeafyPlantConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(MushroomPlantConfig), "CreatePrefab")]
    public static class MushroomPlantConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(OxyfernConfig), "CreatePrefab")]
    public static class OxyfernConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);

            float oxyfernOxygenPerSecond = LessIrrigationState.StateManager.State.OxyfernOxygenPerSecond;
            if (oxyfernOxygenPerSecond != 0.03125f)
            {
                ElementConverter elementConverter = __result.GetComponent<ElementConverter>();
                if (elementConverter == null)
                {
                    Debug.LogWarning("OxyfernConfig_CreatePrefab elementConverter was null");
                    return;
                }
                for (int index = 0; index < elementConverter.consumedElements.Count(); index++)
                {
                    if (elementConverter.consumedElements[index].tag == SimHashes.CarbonDioxide.ToString().ToTag())
                    {
                        elementConverter.consumedElements[index].massConsumptionRate = oxyfernOxygenPerSecond / 50f;
                        Debug.Log("OxyfernConfigMod massConsumptionRate: " + elementConverter.consumedElements[index].massConsumptionRate
                            + " == " + oxyfernOxygenPerSecond/50f);
                    }
                }
                for (int index = 0; index < elementConverter.outputElements.Count(); index++)
                {
                    if (elementConverter.outputElements[index].elementHash == SimHashes.Oxygen)
                    {
                        elementConverter.outputElements[index].massGenerationRate = oxyfernOxygenPerSecond;
                        Debug.Log("OxyfernConfigMod massGenerationRate: " + elementConverter.outputElements[index].massGenerationRate
                            + " == " + oxyfernOxygenPerSecond);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Oxyfern), "SetConsumptionRate")]
    public static class Oxyfern_SetConsumptionRate
    {
        private static bool Prefix(Oxyfern __instance)
        {
            float oxyfernOxygenPerSecond = LessIrrigationState.StateManager.State.OxyfernOxygenPerSecond;
            if (oxyfernOxygenPerSecond == 0.03125f) return true;

            ElementConsumer elementConsumer = __instance.GetComponent<ElementConsumer>();
            if (elementConsumer == null)
            {
                Debug.LogWarning("Oxyfern_SetConsumptionRate elementConsumer was null");
                return true;
            }

            elementConsumer.consumptionRate = oxyfernOxygenPerSecond;

            //if (((AccessTools.Field(typeof(ReceptacleMonitor), "receptacleMonitor").GetValue(__instance)) as ReceptacleMonitor).Replanted)
            //    elementConsumer.consumptionRate = oxyfernOxygenPerSecond;
            //else
            //    elementConsumer.consumptionRate = oxyfernOxygenPerSecond / 4f;

            return false;
    }
    }

    [HarmonyPatch(typeof(PrickleFlowerConfig), "CreatePrefab")]
    public static class PrickleFlowerConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(PrickleGrassConfig), "CreatePrefab")]
    public static class PrickleGrassConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(SaltPlantConfig), "CreatePrefab")]
    public static class SaltPlantConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(SpiceVineConfig), "CreatePrefab")]
    public static class SpiceVineConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    [HarmonyPatch(typeof(SwampLilyConfig), "CreatePrefab")]
    public static class SwampLilyConfigMod
    {
        private static void Postfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }
    }

    #endregion

    #region plant entity templates

    [HarmonyPatch(typeof(EntityTemplates), "ExtendPlantToFertilizable")]
    public static class EntityTemplates_ExtendPlantToFertilizable
    {
        private static bool Prefix(ref GameObject template)
        {
            return !PlantHelper.IsPlantToRemove(template.name);
        }
    }

    [HarmonyPatch(typeof(EntityTemplates), "ExtendPlantToIrrigated")]
    [HarmonyPatch(new Type[] { typeof(GameObject), typeof(PlantElementAbsorber.ConsumeInfo[]) })]
    public static class EntityTemplates_ExtendPlantToIrrigated
    {
        private static bool Prefix(ref GameObject template)
        {
            
            return !PlantHelper.IsPlantToRemove(template.name);
        }
    }


    [HarmonyPatch(typeof(EntityTemplates), "ExtendEntityToBasicPlant")]
    public static class EntityTemplates_ExtendEntityToBasicPlant
    {
        public static bool bCropVal = false;

        private static bool Prefix(
            ref GameObject template,
            ref float temperature_lethal_low,
            ref float temperature_warning_low,
            ref float temperature_warning_high,
            ref float temperature_lethal_high,
            ref SimHashes[] safe_elements,
            ref bool pressure_sensitive,
            ref float pressure_lethal_low,
            ref float pressure_warning_low,
            ref string crop_id,
            ref bool can_drown,
            ref bool can_tinker,
            ref bool require_solid_tile,
            ref bool should_grow_old,
            ref float max_age)
        {
            if (bCropVal == false)
            {
                PlantHelper.ProcessCropVal();
                bCropVal = true;
            }

            PlantHelper.ProcessTempAtmosphere(
                ref template,
                ref temperature_lethal_low,
                ref temperature_warning_low,
                ref temperature_warning_high,
                ref temperature_lethal_high,
                ref safe_elements,
                ref pressure_sensitive,
                ref pressure_lethal_low,
                ref pressure_warning_low,
                ref crop_id,
                ref can_drown,
                ref can_tinker,
                ref require_solid_tile,
                ref should_grow_old,
                ref max_age);
            return true;
        }
    }

    #endregion

    #region farming tiles

    [HarmonyPatch(typeof(FlowerVaseConfig), "ConfigureBuildingTemplate")]
    public static class FlowerVaseConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (!LessIrrigationState.StateManager.State.SeedsGoIntoAnyFlowerPots) return;
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            //plantablePlot.AddDepositTag(GameTags.DecorSeed);
            //plantablePlot.SetFertilizationFlags(true, false);
            plantablePlot.AddDepositTag(GameTags.CropSeed);
        }
    }

    [HarmonyPatch(typeof(PlanterBoxConfig), "ConfigureBuildingTemplate")]
    public static class PlanterBoxConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (!LessIrrigationState.StateManager.State.SeedsGoIntoAnyFlowerPots) return;
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            //plantablePlot.AddDepositTag(GameTags.CropSeed);
            //plantablePlot.SetFertilizationFlags(true, false);
            plantablePlot.AddDepositTag(GameTags.DecorSeed);
            plantablePlot.SetFertilizationFlags(true, false);
        }
    }

    [HarmonyPatch(typeof(FarmTileConfig), "ConfigureBuildingTemplate")]
    public static class FarmTileConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (!LessIrrigationState.StateManager.State.SeedsGoIntoAnyFlowerPots) return;
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            //plantablePlot.AddDepositTag(GameTags.CropSeed);
            //plantablePlot.AddDepositTag(GameTags.WaterSeed);
            //plantablePlot.SetFertilizationFlags(true, false);
            plantablePlot.AddDepositTag(GameTags.DecorSeed);
        }
    }

    [HarmonyPatch(typeof(HydroponicFarmConfig), "ConfigureBuildingTemplate")]
    public static class HydroponicFarmConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (!LessIrrigationState.StateManager.State.SeedsGoIntoAnyFlowerPots) return;
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            //plantablePlot.AddDepositTag(GameTags.CropSeed);
            //plantablePlot.AddDepositTag(GameTags.WaterSeed);
            //plantablePlot.SetFertilizationFlags(true, true);
            plantablePlot.AddDepositTag(GameTags.DecorSeed);
        }
    }

    [HarmonyPatch(typeof(FlowerVaseHangingConfig), "ConfigureBuildingTemplate")]
    public static class FlowerVaseHangingConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (!LessIrrigationState.StateManager.State.SeedsGoIntoAnyFlowerPots) return;
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            plantablePlot.AddDepositTag(GameTags.CropSeed);
            plantablePlot.SetFertilizationFlags(true, false);
        }
    }

    [HarmonyPatch(typeof(FlowerVaseHangingFancyConfig), "ConfigureBuildingTemplate")]
    public static class FlowerVaseHangingFancyConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (!LessIrrigationState.StateManager.State.SeedsGoIntoAnyFlowerPots) return;
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            plantablePlot.AddDepositTag(GameTags.CropSeed);
            plantablePlot.SetFertilizationFlags(true, false);
        }
    }

    [HarmonyPatch(typeof(FlowerVaseWallConfig), "ConfigureBuildingTemplate")]
    public static class FlowerVaseWallConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            if (!LessIrrigationState.StateManager.State.SeedsGoIntoAnyFlowerPots) return;
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            plantablePlot.AddDepositTag(GameTags.CropSeed);
            plantablePlot.SetFertilizationFlags(true, false);
        }
    }

    #endregion

    //[HarmonyPatch(typeof(LeafyPlantConfig), "CreatePrefab")]
    //internal class LeafyPlantConfig_CreatePrefab2
    //{
    //    private static void Postfix(GameObject __result)
    //    {
    //        //UnityEngine.Object.DestroyImmediate(__result.AddOrGet<PrickleGrass>());

    //        EntityTemplates.ExtendEntityToBasicPlant(__result, 218.15f, 278.15f, 303.15f, 398.15f, new SimHashes[3]
    //        {
    //            SimHashes.Oxygen,
    //            SimHashes.ContaminatedOxygen,
    //            SimHashes.CarbonDioxide
    //        }, true, 0.0f, 0.15f, "Algae", true, true, true, true, 2400f);

    //        __result.AddOrGet<StandardCropPlant>();
    //    }
    //}

}



/*public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
{
    List<CodeInstruction> code = instructions.ToList();
    foreach (CodeInstruction codeInstruction in code)
    {
        if (codeInstruction.opcode == OpCodes.Ldc_R4
            && (float)codeInstruction.operand == (float)0.03333334f)
        {
            codeInstruction.operand = 0.00833333f;
        }
        yield return codeInstruction;
    }
}*/
