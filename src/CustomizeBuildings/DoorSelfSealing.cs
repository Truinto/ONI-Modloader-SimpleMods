using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;
using Common;
using System.Linq;
using System.Reflection;

namespace CustomizeBuildings
{
    /// <summary>
    /// Unstuck dupes in doors.
    /// </summary>
    [HarmonyPatch(typeof(FallMonitor.Instance), nameof(FallMonitor.Instance.UpdateFalling))]
    [HarmonyPriority(Priority.Low)]
    public static class DoorEntomb_Patch
    {
        public static bool Prepare()
        {
            if (Helpers.IsModActive("PeterHan.AIImprovements", true))
            {
                Helpers.Print("Didn't patch DoorEntomb_Patch because PeterHan.AIImprovements is enabled.");
                return false;
            }

            return CustomizeBuildingsState.Instance.DoorSelfSealing;
        }

        public static void Postfix(FallMonitor.Instance __instance)
        {
            if (!__instance.navigator.IsMoving() && Grid.HasDoor[Grid.PosToCell(__instance.transform.GetPosition())])
            {
                __instance.sm.isEntombed.Set(false, __instance);
                __instance.sm.isFalling.Set(false, __instance);
            }
        }
    }

    //[HarmonyPatch(typeof(Door), nameof(Door.OnCleanUp))]
    //public static class DoorOnCleanUp_Patch
    //{
    //    public static void Postfix(Door __instance)
    //    {
    //        foreach (int cell in __instance.building.PlacementCells)
    //        {
    //            SimMessages.ClearCellProperties(cell, 3);
    //        }
    //    }
    //}

    /// <summary>
    /// While dupe is stuck in door, they can breath. This is to not trigger the air alert in this brief moment.
    /// </summary>
    [HarmonyPatch(typeof(SimTemperatureTransfer), nameof(SimTemperatureTransfer.OnCellChanged))]
    public static class DuplicantVacuum_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.DoorSelfSealing;
        }

        public static bool Prefix(SimTemperatureTransfer __instance)
        {
            if (__instance is CreatureSimTemperatureTransfer)
            {
                int cell = Grid.PosToCell(__instance);
                if (Grid.IsValidCell(cell) && Grid.HasDoor[cell])
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// ReplaceAndDisplaceElement => makes cells block elements
    /// SetInsulation => temperature transfer multiplier where 1.0 is normal and 0.0 is no transfer
    /// </summary>
    //[HarmonyPatch(typeof(Door), nameof(Door.SetSimState))]
    [HarmonyPatch(typeof(Door), nameof(Door.SetSimState))]
    public static class DoorSelfSealing_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.DoorSelfSealing;
        }

        public static bool Prefix(bool is_door_open, IList<int> cells, Door __instance)
        {
            if (__instance == null || __instance.doorType == Door.DoorType.Internal || __instance.CurrentState == Door.ControlState.Opened)
                return true;

            var pElement = __instance.GetComponent<PrimaryElement>();
            float mass = pElement.Mass / cells.Count;
            for (int i = 0; i < cells.Count; i++)
            {
                int cell = cells[i];
                World.Instance.groundRenderer.MarkDirty(cell);
                SimMessages.SetCellProperties(cell, 4);
                SimMessages.SetInsulation(cell, 0f);
                if (is_door_open)
                {
                    var handleOpen = Game.Instance.callbackManager.Add(new Game.CallbackInfo(__instance.OnSimDoorOpened, false));
                    SimMessages.ReplaceAndDisplaceElement(cell, pElement.ElementID, CellEventLogger.Instance.DoorOpen, mass, pElement.Temperature, byte.MaxValue, 0, handleOpen.index);
                }
                else
                {
                    var handleClose = Game.Instance.callbackManager.Add(new Game.CallbackInfo(__instance.OnSimDoorClosed, false));
                    SimMessages.ReplaceAndDisplaceElement(cell, pElement.ElementID, CellEventLogger.Instance.DoorClose, mass, pElement.Temperature, byte.MaxValue, 0, handleClose.index);
                }
            }
            return false;
        }
    }

    public class DoorPressureMod : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == PressureDoorConfig.ID;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var door = def.BuildingComplete.GetComponent<Door>();
            if (door != null)
            {
                door.insulationModifier = CustomizeBuildingsState.Instance.DoorPressureInsulationFactor;
                door.poweredAnimSpeed = CustomizeBuildingsState.Instance.DoorPressureSpeedPowered;
                door.unpoweredAnimSpeed = CustomizeBuildingsState.Instance.DoorPressureSpeedUnpowered;
            }
        }
    }

    public class DoorBunkerMod : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == BunkerDoorConfig.ID;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var door = def.BuildingComplete.GetComponent<Door>();
            if (door != null)
            {
                door.poweredAnimSpeed = CustomizeBuildingsState.Instance.DoorBunkerSpeedPowered;
                door.unpoweredAnimSpeed = CustomizeBuildingsState.Instance.DoorBunkerSpeedUnpowered;
            }
        }
    }

    public class DoorManualMod : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == ManualPressureDoorConfig.ID;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var door = def.BuildingComplete.GetComponent<Door>();
            if (door != null)
            {
                door.unpoweredAnimSpeed = CustomizeBuildingsState.Instance.DoorManualSpeed;
            }
        }
    }

    public class DoorInsulatedMod : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == InsulatedDoorConfig.ID;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var door = def.BuildingComplete.GetComponent<Door>();
            if (door != null)
            {
                door.unpoweredAnimSpeed = CustomizeBuildingsState.Instance.DoorInsulatedSpeed;
            }
        }
    }
}
