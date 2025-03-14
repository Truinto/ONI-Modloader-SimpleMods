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
    public class SolidTransferArm_MoveAnything
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AutoSweeperPickupAnything;
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
            ___max_carry_weight = CustomizeBuildingsState.StateManager.State.AutoSweeperCapacity;
        }
    }

    [HarmonyPatch]
    public class SolidTransferArmConfig_AutoSweeperRange
    {
        public static bool Prepare(MethodBase original)
        {
            return CustomizeBuildingsState.StateManager.State.AutoSweeperRange > 4;
        }

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

    [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.DigBlockingCB))]
    public class AutoMiner_RoboMinerDigAnyTile
    {
        public static bool Prepare(MethodBase original)
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerDigAnyTile;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);
            data.InsertAfterAll(typeof(Element), nameof(Element.hardness), patch);
            return data;

            static byte patch(byte __stack)
            {
                if (__stack is > 150 and < 255)
                    return 150;
                return __stack;
            }
        }
    }

    [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.ValidDigCell))]
    public class AutoMiner_RoboMinerDigAnyTile2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerDigAnyTile;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            return AutoMiner_RoboMinerDigAnyTile.Transpiler(instructions, generator, original);
        }
    }

    [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.DigBlockingCB))]
    public class AutoMiner_RoboMinerDigThroughGlass
    {
        public static bool Prepare(MethodBase original)
        {
            return CustomizeBuildingsState.StateManager.State.RoboMinerDigThroughGlass;
        }

        [HarmonyPatch(typeof(AutoMiner), nameof(AutoMiner.DigBlockingCB))]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(Grid), nameof(Grid.Solid));
            data.Seek(f => f.IsLdloc(typeof(bool)));
            data.InsertAfter(patch);
            return data;

            static bool patch(bool __stack, int cell)
            {
                if (__stack)
                    return true;
                return Grid.Transparent[cell];
            }
        }

        public static bool Prefix(int cell, ref bool __result)
        {
            try
            {
                if (CustomizeBuildingsState.StateManager.State.RoboMinerDigThroughGlass && Grid.Transparent[cell])
                {
                    __result = false;
                    return false;
                }

                if (Grid.Foundation[cell] && Grid.Solid[cell])
                {
                    __result = true;
                    return false;
                }

                if (Grid.Element[cell].hardness >= (CustomizeBuildingsState.StateManager.State.RoboMinerDigAnyTile ? 255 : 150))
                {
                    __result = true;
                    return false;
                }

                __result = false;
                return false;
            } catch (Exception) { }
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
            } catch (Exception) { }
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

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            def.BuildingComplete.GetDef<ResourceHarvestModule.Def>().harvestSpeed = CustomizeBuildingsState.StateManager.State.DrillConeSpeed;
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
