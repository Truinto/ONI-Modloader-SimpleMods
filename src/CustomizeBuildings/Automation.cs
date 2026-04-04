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
using Config;

namespace CustomizeBuildings
{
    #region Autosweeper
    [HarmonyPatch(typeof(SolidTransferArm), nameof(SolidTransferArm.IsPickupableRelevantToMyInterests))]
    public class SolidTransferArm_MoveAnything
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.AutoSweeperPickupAnything;
        }

        public static bool Prefix(ref bool __result, SolidTransferArm __instance, KPrefabID prefabID, int storage_cell)
        {
            if (!__instance.IsCellReachable(storage_cell))
                __result = false;
            else if (Assets.IsTagSolidTransferArmConveyable(prefabID.PrefabTag))
                __result = true;
            else if (prefabID.HasAnyTags(tagsCreatures))
                __result = false;
            else if (prefabID.GetComponent<Pickupable>().targetWorkable is not (null or Pickupable))
                __result = false;
            else
                __result = true;

            //Helpers.Print($"MoveAnything: target={prefabID.GetComponent<Pickupable>().targetWorkable?.GetType()} tag={prefabID.PrefabTag} tags={prefabID.tags.Join()}");

            return false;
        }

        public static Tag[] tagsCreatures = [GameTags.BagableCreature, GameTags.SwimmingCreature];
    }

    [HarmonyPatch(typeof(SolidTransferArm), nameof(SolidTransferArm.OnPrefabInit))]
    public class SolidTransferArm_AutoSweeperCapacity
    {
        public static void Prefix(ref float ___max_carry_weight)
        {
            ___max_carry_weight = CustomizeBuildingsState.Instance.AutoSweeperCapacity;
        }
    }

    [HarmonyPatch]
    public class SolidTransferArmConfig_AutoSweeperRange
    {
        public static bool Prepare(MethodBase original)
        {
            return CustomizeBuildingsState.Instance.AutoSweeperRange > 4;
        }

        [HarmonyPatch(typeof(SolidTransferArmConfig), nameof(SolidTransferArmConfig.DoPostConfigureComplete))]
        [HarmonyPostfix]
        private static void Postfix1(GameObject go)
        {
            // set range logic
            go.AddOrGet<SolidTransferArm>().pickupRange = CustomizeBuildingsState.Instance.AutoSweeperRange;

            // add slider
            if (CustomizeBuildingsState.Instance.AutoSweeperSlider)
                go.AddOrGet<UserControlledTransferArm>().Max = CustomizeBuildingsState.Instance.AutoSweeperRange;
        }

        [HarmonyPatch(typeof(SolidTransferArmConfig), nameof(SolidTransferArmConfig.AddVisualizer))]
        [HarmonyPrefix]
        private static bool Prefix2(GameObject prefab)
        {
            // set range visual
            int range = CustomizeBuildingsState.Instance.AutoSweeperRange;
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

    // broke with update, not interested right now
    public class SolidTransferArm_ReachThrough
    {
        public static Tag[] BuildingsIgnoreLOS = [FarmTileConfig.ID, HydroponicFarmConfig.ID];

        // IMPORTANT: overwrite visualizer in UserControlledTransferArm.OnSpawn!
    }

    #endregion

    #region Robominer
    [HarmonyPatch]
    public class AutoMiner_Range
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.RoboMinerWidth != 16
                || CustomizeBuildingsState.Instance.RoboMinerHeight != 9
                || CustomizeBuildingsState.Instance.RoboMinerOffset != 0;
        }

        [HarmonyPatch(typeof(AutoMinerConfig), nameof(AutoMinerConfig.DoPostConfigureComplete))]
        [HarmonyPostfix]
        public static void Postfix1(GameObject go)
        {
            int width = CustomizeBuildingsState.Instance.RoboMinerWidth;
            int height = CustomizeBuildingsState.Instance.RoboMinerHeight;
            int offset = CustomizeBuildingsState.Instance.RoboMinerOffset;

            AutoMiner autoMiner = go.AddOrGet<AutoMiner>();
            autoMiner.x = 1 - (width / 2);
            autoMiner.y = offset;
            autoMiner.width = width;
            autoMiner.height = height;
            autoMiner.vision_offset = new CellOffset(0, 1);
        }

        [HarmonyPatch(typeof(AutoMinerConfig), nameof(AutoMinerConfig.AddVisualizer))]
        [HarmonyPostfix]
        public static void Postfix2(GameObject prefab)
        {
            int width = CustomizeBuildingsState.Instance.RoboMinerWidth;
            int height = CustomizeBuildingsState.Instance.RoboMinerHeight;
            int offset = CustomizeBuildingsState.Instance.RoboMinerOffset;

            RangeVisualizer rangeVisualizer = prefab.GetComponent<RangeVisualizer>();
            if (rangeVisualizer != null)
            {
                if (width * height > 400)
                {
                    width = 1;
                    height = 2;
                }

                rangeVisualizer.RangeMin.x = 1 - (width / 2);
                rangeVisualizer.RangeMin.y = offset - 1;
                rangeVisualizer.RangeMax.x = rangeVisualizer.RangeMin.x + width;
                rangeVisualizer.RangeMax.y = height - 2;

                //choreRangeVisualizer.vision_offset = new CellOffset(0, 1);
                //choreRangeVisualizer.movable = movable;
                //choreRangeVisualizer.blocking_tile_visible = false;
            }
        }
    }

    [HarmonyPatch]
    public class AutoMiner_RoboMinerDig_AnyTile_ThroughGlass
    {
        public static void OnLoad()
        {
            if (CustomizeBuildingsState.Instance.RoboMinerDigAnyTile && CustomizeBuildingsState.Instance.RoboMinerDigThroughGlass)
                AutoMiner.DigBlockingCB = DigBlockingCB_AnyTile_ThroughGlass;
            else if (CustomizeBuildingsState.Instance.RoboMinerDigAnyTile)
                AutoMiner.DigBlockingCB = DigBlockingCB_AnyTile;
            else if (CustomizeBuildingsState.Instance.RoboMinerDigThroughGlass)
                AutoMiner.DigBlockingCB = DigBlockingCB_ThroughGlass;
        }

        public static Func<int, bool> DigBlockingCB = delegate (int cell)
        {
            bool hasDoor = Grid.HasDoor[cell] && Grid.Foundation[cell] && Grid.ObjectLayers[(int)ObjectLayer.FoundationTile].ContainsKey(cell);
            if (hasDoor)
            {
                var door = Grid.ObjectLayers[(int)ObjectLayer.FoundationTile][cell].GetComponent<Door>();
                hasDoor = door != null && door.IsOpen() && !door.IsPendingClose();
            }
            if (Grid.Foundation[cell] && Grid.Solid[cell] && !hasDoor)
            {
                return true;
            }
            return Grid.Element[cell].hardness >= 150;
        };

        public static Func<int, bool> DigBlockingCB_AnyTile = delegate (int cell)
        {
            bool hasDoor = Grid.HasDoor[cell] && Grid.Foundation[cell] && Grid.ObjectLayers[(int)ObjectLayer.FoundationTile].ContainsKey(cell);
            if (hasDoor)
            {
                var door = Grid.ObjectLayers[(int)ObjectLayer.FoundationTile][cell].GetComponent<Door>();
                if (door != null)
                    return !door.IsOpen() || door.IsPendingClose();
            }
            if (Grid.Foundation[cell] && Grid.Solid[cell])
                return true;
            return Grid.Element[cell].hardness >= 255;
        };

        public static Func<int, bool> DigBlockingCB_ThroughGlass = delegate (int cell)
        {
            bool hasDoor = Grid.HasDoor[cell] && Grid.Foundation[cell] && Grid.ObjectLayers[(int)ObjectLayer.FoundationTile].ContainsKey(cell);
            if (hasDoor)
            {
                var door = Grid.ObjectLayers[(int)ObjectLayer.FoundationTile][cell].GetComponent<Door>();
                if (door != null)
                    return !door.IsOpen() || door.IsPendingClose();
            }
            if (Grid.Foundation[cell] && Grid.Solid[cell] && !Grid.Transparent[cell])
                return true;
            return Grid.Element[cell].hardness >= 150;
        };

        public static Func<int, bool> DigBlockingCB_AnyTile_ThroughGlass = delegate (int cell)
        {
            bool hasDoor = Grid.HasDoor[cell] && Grid.Foundation[cell] && Grid.ObjectLayers[(int)ObjectLayer.FoundationTile].ContainsKey(cell);
            if (hasDoor)
            {
                var door = Grid.ObjectLayers[(int)ObjectLayer.FoundationTile][cell].GetComponent<Door>();
                if (door != null)
                    return !door.IsOpen() || door.IsPendingClose();
            }
            if (Grid.Foundation[cell] && Grid.Solid[cell] && !Grid.Transparent[cell])
                return true;
            return Grid.Element[cell].hardness >= 255;
        };

        public static bool Prepare(MethodBase original)
        {
            return CustomizeBuildingsState.Instance.RoboMinerDigAnyTile;
        }

        [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.ValidDigCell))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);
            data.InsertAfterAll(typeof(Element), nameof(Element.hardness), patch);
            return data;

            static byte patch(byte __stack)
            {
                if (__stack < 255)
                    return 1;
                return __stack;
            }
        }

        //[HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.DigBlockingCB))]
        //[HarmonyTranspiler]
        //public static IEnumerable<CodeInstruction> Transpiler1(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        //{
        //    var data = new TranspilerTool(instructions, generator, original);
        //    data.InsertAfterAll(typeof(Element), nameof(Element.hardness), patch);
        //    return data;
        //    static byte patch(byte __stack)
        //    {
        //        if (__stack < 255)
        //            return 1;
        //        return __stack;
        //    }
        //}
    }

    //[HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.DigBlockingCB))]
    //public class AutoMiner_RoboMinerDigThroughGlass
    //{
    //    public static bool Prepare(MethodBase original)
    //    {
    //        return CustomizeBuildingsState.Instance.RoboMinerDigThroughGlass;
    //    }
    //    public static void Postfix(int cell, ref bool __result)
    //    {
    //        if (__result)
    //            __result = !Grid.Transparent[cell];
    //    }
    //}

    [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.UpdateDig))]
    public class AutoMiner_RegolithTurbo
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.RoboMinerRegolithTurbo;
        }

        public static void Prefix(ref float dt, int ___dig_cell)
        {
            try
            {
                if (Grid.Element[___dig_cell].id == SimHashes.Regolith)
                    dt *= 6f;
            } catch (Exception) { }
        }
    }

    [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.UpdateDig))]
    public class AutoMiner_Speed
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.RoboMinerSpeedMult != 1f;
        }

        public static void Prefix(ref float dt)
        {
            dt *= CustomizeBuildingsState.Instance.RoboMinerSpeedMult;
        }
    }
    #endregion

    #region Drill Cone
    [HarmonyPatch]
    public static class DrillCone_Consumption1
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.DrillConeConsumption != 0.05f;
        }

        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.ConsumeDiamond))]
        [HarmonyPrefix]
        public static void Prefix1(ref float amount)
        {
            amount = amount / 0.05f * CustomizeBuildingsState.Instance.DrillConeConsumption;
        }

        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.GetMaxExtractKGFromDiamondAvailable))]
        [HarmonyPostfix]
        public static void Postfix2(ref float __result)
        {
            var con = CustomizeBuildingsState.Instance.DrillConeConsumption;
            __result = con == 0f ? float.PositiveInfinity : __result * 0.05f / con;
        }
    }

    public class DrillCone_Speed : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == NoseconeHarvestConfig.ID && CustomizeBuildingsState.Instance.DrillConeSpeed != 7.5f;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            def.BuildingComplete.GetDef<ResourceHarvestModule.Def>().harvestSpeed = CustomizeBuildingsState.Instance.DrillConeSpeed;
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
            __result.GetComponents<Storage>()[1].capacityKg = CustomizeBuildingsState.Instance.SweepBotKG;
        }

        [HarmonyPatch(typeof(SweepBotStationConfig), nameof(SweepBotStationConfig.ConfigureBuildingTemplate))]
        [HarmonyPostfix]
        public static void SweepyStation_Storage(GameObject go)
        {
            go.GetComponent<SweepBotStation>().sweepStorage.capacityKg = CustomizeBuildingsState.Instance.SweepyStationKG;
        }

        [HarmonyPatch(typeof(SweepStates), nameof(SweepStates.TryMop))]
        [HarmonyPrefix]
        public static void Sweepy_Mop(ref float dt)
        {
            dt = CustomizeBuildingsState.Instance.SweepBotKG;
        }

        [HarmonyPatch(typeof(SweepStates), nameof(SweepStates.TryStore))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Sweepy_Sweep(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            int count = data.ReplaceAllConstant(10f, CustomizeBuildingsState.Instance.SweepBotKG);
            if (count != 2)
                PostBootDialog.ToDialog($"Sweepy_Sweep replaced {count} constants, instead of 2");

            return data;
        }
    }
    #endregion
}
