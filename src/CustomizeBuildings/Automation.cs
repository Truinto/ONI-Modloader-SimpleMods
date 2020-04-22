using Harmony;
using UnityEngine;
using System;

namespace CustomizeBuildings
{

    [HarmonyPatch(typeof(SolidTransferArm), "OnPrefabInit")]
    internal class SolidTransferArm_OnPrefabInit
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AutoSweeperCapacity != 1000f;
        }
        private static void Prefix(ref float ___max_carry_weight)
        {
            ___max_carry_weight = CustomizeBuildingsState.StateManager.State.AutoSweeperCapacity;
            //__instance.pickupRange = 12;
        }
    }

    [HarmonyPatch(typeof(SolidTransferArmConfig), "DoPostConfigureComplete")]
    internal class SolidTransferArmConfig_DoPostConfigureComplete
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AutoSweeperRange != 4;
        }
        private static void Postfix(GameObject go)
        {
            go.AddOrGet<SolidTransferArm>().pickupRange = CustomizeBuildingsState.StateManager.State.AutoSweeperRange;
        }
    }
    
    [HarmonyPatch(typeof(SolidTransferArmConfig), "DoPostConfigureComplete")]
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

    [HarmonyPatch(typeof(SolidTransferArmConfig), "AddVisualizer")]
    internal class SolidTransferArmConfig_AddVisualizer
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AutoSweeperRange != 4;
        }
        private static bool Prefix(GameObject prefab, bool movable)
        {
            int range = CustomizeBuildingsState.StateManager.State.AutoSweeperRange;
            StationaryChoreRangeVisualizer choreRangeVisualizer = prefab.AddOrGet<StationaryChoreRangeVisualizer>();
            choreRangeVisualizer.x = -range;
            choreRangeVisualizer.y = -range;
            choreRangeVisualizer.width = range * 2 + 1;
            choreRangeVisualizer.height = range * 2 + 1;
            choreRangeVisualizer.movable = movable;
            return false;
        }
    }

    [HarmonyPatch(typeof(AutoMinerConfig), "DoPostConfigureComplete")]
    internal class AutoMinerConfig_OnPrefabInit
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerWidth != 16
                || CustomizeBuildingsState.StateManager.State.RoboMinerHeight != 9
                || CustomizeBuildingsState.StateManager.State.RoboMinerOffset != 0;
        }
        private static void Postfix(GameObject go)
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

    [HarmonyPatch(typeof(AutoMinerConfig), "AddVisualizer")]
    internal class AutoMinerConfig_AddVisualizer
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerWidth != 16
                || CustomizeBuildingsState.StateManager.State.RoboMinerHeight != 9
                || CustomizeBuildingsState.StateManager.State.RoboMinerOffset != 0;
        }
        private static void Postfix(GameObject prefab, bool movable)
        {
            int width = CustomizeBuildingsState.StateManager.State.RoboMinerWidth;
            int height = CustomizeBuildingsState.StateManager.State.RoboMinerHeight;
            int offset = CustomizeBuildingsState.StateManager.State.RoboMinerOffset;

            StationaryChoreRangeVisualizer choreRangeVisualizer = prefab.GetComponent<StationaryChoreRangeVisualizer>();
            if (choreRangeVisualizer != null)
            {
                choreRangeVisualizer.x = 1 - (width / 2);
                choreRangeVisualizer.y = offset;
                choreRangeVisualizer.width = width;
                choreRangeVisualizer.height = height;
                //choreRangeVisualizer.vision_offset = new CellOffset(0, 1);
                //choreRangeVisualizer.movable = movable;
                //choreRangeVisualizer.blocking_tile_visible = false;
            }
        }
    }
    
    [HarmonyPatch(typeof(AutoMiner), "DigBlockingCB")]
    internal class AutoMiner_DigBlockingCB
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerDigAnyTile || CustomizeBuildingsState.StateManager.State.RoboMinerDigThroughGlass;
        }

        internal static bool Prefix(int cell, ref bool __result)
        {
            try
            {
                __result = (Grid.Foundation[cell] && !Grid.Transparent[cell] || !CustomizeBuildingsState.StateManager.State.RoboMinerDigThroughGlass) || Grid.Element[cell].hardness >= (CustomizeBuildingsState.StateManager.State.RoboMinerDigAnyTile ? (byte)255 : (byte)150);
                return false;
            }
            catch (Exception) { }
            return true;
        }
    }

    [HarmonyPatch(typeof(AutoMiner), "ValidDigCell")]
    internal class AutoMiner_ValidDigCell
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerDigAnyTile;
        }

        internal static bool Prefix(int cell, ref bool __result)
        {
            try {
                __result = Grid.Solid[cell] && !Grid.Foundation[cell] && Grid.Element[cell].hardness < (byte) 255;
                return false;
            } catch (Exception) { }
            return true;
        }
    }


    [HarmonyPatch(typeof(AutoMiner), "UpdateDig")]
    internal class AutoMiner_UpdateDig
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerRegolithTurbo;
        }

        internal static void Prefix(ref float dt, int ___dig_cell)
        {
            try {
                if (Grid.Element[___dig_cell].id == SimHashes.Regolith)
                    dt *= 6f;
            } catch (Exception) { }
        }
    }

}