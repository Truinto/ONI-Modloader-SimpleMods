using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using Common;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Shared;
using TemplateClasses;

namespace CustomizeBuildings
{
    #region Autosweeper
    [HarmonyPatch(typeof(SolidTransferArm), nameof(SolidTransferArm.IsPickupableRelevantToMyInterests))]
    public class SolidTransferarm_MoveAnything
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AutoSweeperPickupAnything;
        }

        public static bool Prefix(ref bool __result, SolidTransferArm __instance, KPrefabID prefabID, int storage_cell)
        {
            __result = !prefabID.HasAnyTags(tagsCreatures) && __instance.IsCellReachable(storage_cell);

            return false;
        }

        public static Tag[] tagsCreatures = { GameTags.BagableCreature, GameTags.SwimmingCreature };
    }

    [HarmonyPatch(typeof(SolidTransferArm), nameof(SolidTransferArm.OnPrefabInit))]
    public class SolidTransferArm_OnPrefabInit
    {
        public static void Prefix(ref float ___max_carry_weight)
        {
            ___max_carry_weight = CustomizeBuildingsState.StateManager.State.AutoSweeperCapacity;
            //__instance.pickupRange = 12;
        }
    }

    [HarmonyPatch]
    public class SolidTransferArmConfig_Patches
    {
        [HarmonyPatch(typeof(SolidTransferArmConfig), nameof(SolidTransferArmConfig.DoPostConfigureComplete))]
        [HarmonyPostfix]
        private static void Postfix1(GameObject go)
        {
            // set range logic
            go.AddOrGet<SolidTransferArm>().pickupRange = CustomizeBuildingsState.StateManager.State.AutoSweeperRange;

            // add slider
            if (CustomizeBuildingsState.StateManager.State.AutoSweeperSlider)
                go.AddOrGet<UserControlledTransferArm>().Max = CustomizeBuildingsState.StateManager.State.AutoSweeperRange;
        }

        [HarmonyPatch(typeof(SolidTransferArmConfig), nameof(SolidTransferArmConfig.AddVisualizer))]
        [HarmonyPrefix]
        private static bool Prefix2(GameObject prefab)
        {
            // set range visual
            int range = CustomizeBuildingsState.StateManager.State.AutoSweeperRange;
            if (range > 30)
                range = 1;
            RangeVisualizer rangeVisualizer = prefab.AddOrGet<RangeVisualizer>();
            rangeVisualizer.RangeMin.x = -range;
            rangeVisualizer.RangeMin.y = -range;
            rangeVisualizer.RangeMax.x = range;
            rangeVisualizer.RangeMax.y = range;
            rangeVisualizer.BlockingTileVisible = true;

            // fix farm tile
            rangeVisualizer.BlockingCb = BlockingCbx;

            return false;
        }

        [HarmonyPatch(typeof(SolidTransferArm), nameof(SolidTransferArm.AsyncUpdate))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler3(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            // fix farm tile
            var data = new TranspilerTool(instructions, generator, original);

            if (1 != data.ReplaceAllCalls(typeof(Grid), nameof(Grid.IsPhysicallyAccessible), patch))
                throw new Exception("");

            return data;

            bool patch(int x, int y, int x2, int y2, bool blocking_tile_visible)
            {
                return Grid.TestLineOfSight(x, y, x2, y2, BlockingCbx, blocking_tile_visible);
            }
        }

        public static Tag[] BuildingsIgnoreLOS = new Tag[] { FarmTileConfig.ID, HydroponicFarmConfig.ID };
        public static bool BlockingCbx(int cell)
        {
            if (!Grid.Solid[cell])
                return false;

            try
            {
                return Grid.Objects[cell, 1]?.GetComponent<KPrefabID>()?.IsAnyPrefabID(BuildingsIgnoreLOS) != true;
            }
            catch (Exception) { }

            return true;
        }
    }

    #endregion

    #region Robominer
    [HarmonyPatch(typeof(AutoMinerConfig), nameof(AutoMinerConfig.DoPostConfigureComplete))]
    public class AutoMinerConfig_OnPrefabInit
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerWidth != 16
                || CustomizeBuildingsState.StateManager.State.RoboMinerHeight != 9
                || CustomizeBuildingsState.StateManager.State.RoboMinerOffset != 0;
        }
        public static void Postfix(GameObject go)
        {
            int width = CustomizeBuildingsState.StateManager.State.RoboMinerWidth;
            int height = CustomizeBuildingsState.StateManager.State.RoboMinerHeight;
            int offset = CustomizeBuildingsState.StateManager.State.RoboMinerOffset;

            AutoMiner autoMiner = go.AddOrGet<AutoMiner>();
            autoMiner.x = 1 - (width / 2);
            autoMiner.y = offset;
            autoMiner.width = width;
            autoMiner.height = height;
            autoMiner.vision_offset = new CellOffset(0, 1);
        }
    }

    [HarmonyPatch(typeof(AutoMinerConfig), nameof(AutoMinerConfig.AddVisualizer))]
    public class AutoMinerConfig_AddVisualizer
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerWidth != 16
                || CustomizeBuildingsState.StateManager.State.RoboMinerHeight != 9
                || CustomizeBuildingsState.StateManager.State.RoboMinerOffset != 0;
        }
        public static void Postfix(GameObject prefab)
        {
            int width = CustomizeBuildingsState.StateManager.State.RoboMinerWidth;
            int height = CustomizeBuildingsState.StateManager.State.RoboMinerHeight;
            int offset = CustomizeBuildingsState.StateManager.State.RoboMinerOffset;

            RangeVisualizer rangeVisualizer = prefab.GetComponent<RangeVisualizer>();
            if (rangeVisualizer != null)
            {
                if (width * height > 400)
                {
                    width = 1;
                    height = 1;
                }

                rangeVisualizer.RangeMin.x = 1 - (width / 2);
                rangeVisualizer.RangeMin.y = 3 - (height / 2) + offset;
                rangeVisualizer.RangeMax.x = (width / 2);
                rangeVisualizer.RangeMax.y = 3 + (height / 2) + offset;

                //choreRangeVisualizer.vision_offset = new CellOffset(0, 1);
                //choreRangeVisualizer.movable = movable;
                //choreRangeVisualizer.blocking_tile_visible = false;
            }
        }
    }

    [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.DigBlockingCB))]
    public class AutoMiner_DigBlockingCB
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerDigAnyTile || CustomizeBuildingsState.StateManager.State.RoboMinerDigThroughGlass;
        }

        public static bool Prefix(int cell, ref bool __result)
        {
            try
            {
                __result = Grid.Foundation[cell] && Grid.Solid[cell]
                    //|| Grid.Transparent[cell] || CustomizeBuildingsState.StateManager.State.RoboMinerDigThroughGlass
                    || Grid.Element[cell].hardness >= (CustomizeBuildingsState.StateManager.State.RoboMinerDigAnyTile ? 255 : 150);
                return false;
            }
            catch (Exception) { }
            return true;
        }
    }

    [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.ValidDigCell))]
    public class AutoMiner_ValidDigCell
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerDigAnyTile;
        }

        public static bool Prefix(int cell, ref bool __result)
        {
            try
            {
                __result = Grid.Solid[cell] && !Grid.Foundation[cell] && Grid.Element[cell].hardness < 255;
                return false;
            }
            catch (Exception) { }
            return true;
        }
    }


    [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.UpdateDig))]
    public class AutoMiner_UpdateDig
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerRegolithTurbo;
        }

        public static void Prefix(ref float dt, int ___dig_cell)
        {
            try
            {
                if (Grid.Element[___dig_cell].id == SimHashes.Regolith)
                    dt *= 6f;
            }
            catch (Exception) { }
        }
    }


    [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.UpdateDig))]
    public class AutoMiner_UpdateDig2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerSpeedMult != 1f;
        }

        public static void Prefix(ref float dt)
        {
            dt *= CustomizeBuildingsState.StateManager.State.RoboMinerSpeedMult;
        }
    }
    #endregion

    #region Drill Cone
    [HarmonyPatch]
    public static class DrillCone_Consumption1
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.DrillConeConsumption != 0.05f;
        }

        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.ConsumeDiamond))]
        [HarmonyPrefix]
        public static void Prefix1(ref float amount)
        {
            amount = amount / 0.05f * CustomizeBuildingsState.StateManager.State.DrillConeConsumption;
        }

        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.GetMaxExtractKGFromDiamondAvailable))]
        [HarmonyPostfix]
        public static void Postfix2(ref float __result)
        {
            var con = CustomizeBuildingsState.StateManager.State.DrillConeConsumption;
            __result = con == 0f ? float.PositiveInfinity : __result * 0.05f / con;
        }
    }

    public class DrillCone_Speed : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == NoseconeHarvestConfig.ID && CustomizeBuildingsState.StateManager.State.DrillConeSpeed != 7.5f;
        }

        public void Edit(BuildingDef def)
        {
            def.BuildingComplete.GetDef<ResourceHarvestModule.Def>().harvestSpeed = CustomizeBuildingsState.StateManager.State.DrillConeSpeed;
        }

        public void Undo(BuildingDef def)
        {
            // 2700f * 10f / 3600f = 7.5f
            def.BuildingComplete.GetDef<ResourceHarvestModule.Def>().harvestSpeed
                = TUNING.ROCKETRY.SOLID_CARGO_BAY_CLUSTER_CAPACITY * TUNING.ROCKETRY.CARGO_CAPACITY_SCALE / new NoseconeHarvestConfig().timeToFill;
        }
    }
    #endregion

    #region Sweepy
    [HarmonyPatch]
    public static class Sweepy_Patches
    {
        [HarmonyPatch(typeof(SweepBotConfig), nameof(SweepBotConfig.CreatePrefab))]
        [HarmonyPostfix]
        public static void Sweepy_Storage(GameObject __result)
        {
            __result.GetComponents<Storage>()[1].capacityKg = CustomizeBuildingsState.StateManager.State.SweepBotKG;
        }

        [HarmonyPatch(typeof(SweepBotStationConfig), nameof(SweepBotStationConfig.ConfigureBuildingTemplate))]
        [HarmonyPostfix]
        public static void SweepyStation_Storage(GameObject go)
        {
            go.GetComponent<SweepBotStation>().sweepStorage.capacityKg = CustomizeBuildingsState.StateManager.State.SweepyStationKG;
        }

        [HarmonyPatch(typeof(SweepStates), nameof(SweepStates.TryMop))]
        [HarmonyPrefix]
        public static void Sweepy_Mop(ref float dt)
        {
            dt = CustomizeBuildingsState.StateManager.State.SweepBotKG;
        }

        [HarmonyPatch(typeof(SweepStates), nameof(SweepStates.TryStore))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Sweepy_Sweep(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            int count = data.ReplaceAllConstant(10f, CustomizeBuildingsState.StateManager.State.SweepBotKG);
            if (count != 2)
                throw new Exception($"Sweepy_Sweep replaced {count} constants, instead of 2");

            return data;
        }
    }
    #endregion
}