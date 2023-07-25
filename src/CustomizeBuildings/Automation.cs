using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using Common;

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

    [HarmonyPatch(typeof(SolidTransferArmConfig), nameof(SolidTransferArmConfig.DoPostConfigureComplete))]
    internal class SolidTransferArmConfig_DoPostConfigureComplete
    {
        private static void Postfix(GameObject go)
        {
            go.AddOrGet<SolidTransferArm>().pickupRange = CustomizeBuildingsState.StateManager.State.AutoSweeperRange;
        }
    }

    [HarmonyPatch(typeof(SolidTransferArmConfig), nameof(SolidTransferArmConfig.DoPostConfigureComplete))]
    internal class SolidTransferArmConfig_DoPostConfigureComplete2
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AutoSweeperSlider;
        }
        private static void Postfix(GameObject go)
        {
            go.AddOrGet<UserControlledTransferArm>().Max = CustomizeBuildingsState.StateManager.State.AutoSweeperRange;
        }
    }

    [HarmonyPatch(typeof(SolidTransferArmConfig), nameof(SolidTransferArmConfig.AddVisualizer))]
    internal class SolidTransferArmConfig_AddVisualizer
    {
        private static bool Prefix(GameObject prefab, bool movable)
        {
            int range = CustomizeBuildingsState.StateManager.State.AutoSweeperRange;

            if (range > 30)
                range = 1;

            RangeVisualizer rangeVisualizer = prefab.AddOrGet<RangeVisualizer>();
            rangeVisualizer.RangeMin.x = -range;
            rangeVisualizer.RangeMin.y = -range;
            rangeVisualizer.RangeMax.x = range;
            rangeVisualizer.RangeMax.y = range;
            rangeVisualizer.BlockingTileVisible = true;
            return false;
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
}