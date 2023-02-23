using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;
using Common;
using System.Linq;
using System.Reflection;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(FallMonitor.Instance), nameof(FallMonitor.Instance.UpdateFalling))]
    [HarmonyPriority(Priority.Low)]
    public class DoorEntomb_Patch
    {
        public static bool Prepare()
        {
            if (Helpers.IsModActive("PeterHan.AIImprovements", true))
                Helpers.Print("Didn't patch DoorEntomb_Patch because PeterHan.AIImprovements is enabled.");

            return CustomizeBuildingsState.StateManager.State.DoorSelfSealing && !Helpers.IsModActive("PeterHan.AIImprovements", true);
        }

        public static bool Prefix(FallMonitor.Instance __instance, Navigator ___navigator)
        {
            if (___navigator.IsMoving() || ___navigator.CurrentNavType == NavType.Tube || Grid.HasDoor[Grid.PosToCell(__instance.transform.GetPosition())])
            {
                __instance.sm.isEntombed.Set(false, __instance);
                __instance.sm.isFalling.Set(false, __instance);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Door), "OnPrefabInit")]
    public class DoorOnPrefabInit_Patch
    {
        public static void Postfix(ref Door __instance)
        {
            __instance.overrideAnims = new KAnimFile[]
            {
                Assets.GetAnim("anim_use_remote_kanim")
            };
        }
    }

    [HarmonyPatch(typeof(Door), "OnCleanUp")]
    public class DoorOnCleanUp_Patch
    {
        public static void Postfix(Door __instance)
        {
            foreach (int cell in __instance.building.PlacementCells)
            {
                SimMessages.ClearCellProperties(cell, 3);
            }
        }
    }


    [HarmonyPatch(typeof(Door), "SetSimState")]
    public class DoorSelfSealing_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.DoorSelfSealing;
        }

        public static bool Prefix(Door __instance, bool is_door_open, IList<int> cells)
        {
            if (__instance == null || __instance.doorType == Door.DoorType.Internal || __instance.CurrentState == Door.ControlState.Opened)
                return true;

            PrimaryElement pElement = __instance.GetComponent<PrimaryElement>();
            float mass = pElement.Mass / (float)cells.Count;
            for (int i = 0; i < cells.Count; i++)
            {
                int cell = cells[i];
                World.Instance.groundRenderer.MarkDirty(cell);
                SimMessages.SetCellProperties(cell, 4);
                if (is_door_open)
                {
                    var delegateDoorOpen = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, OnSimDoorOpened);
                    var handleOpen = Game.Instance.callbackManager.Add(new Game.CallbackInfo(delegateDoorOpen, false));
                    SimMessages.ReplaceAndDisplaceElement(cell, pElement.ElementID, CellEventLogger.Instance.DoorOpen, mass, pElement.Temperature, byte.MaxValue, 0, handleOpen.index);
                    //SimMessages.ClearCellProperties(cell, 3);
                }
                else
                {
                    var delegateDoorClose = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, OnSimDoorClosed);
                    var handleClose = Game.Instance.callbackManager.Add(new Game.CallbackInfo(delegateDoorClose, false));
                    SimMessages.ReplaceAndDisplaceElement(cell, pElement.ElementID, CellEventLogger.Instance.DoorClose, mass, pElement.Temperature, byte.MaxValue, 0, handleClose.index);
                    //SimMessages.SetCellProperties(cell, 3);
                }
            }
            return false;
        }

        public static MethodInfo OnSimDoorOpened = AccessTools.Method(typeof(Door), "OnSimDoorOpened");
        public static MethodInfo OnSimDoorClosed = AccessTools.Method(typeof(Door), "OnSimDoorClosed");
    }
}