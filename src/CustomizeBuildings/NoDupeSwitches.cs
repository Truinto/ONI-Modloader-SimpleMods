﻿using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(Switch), "OnMinionToggle")]
    internal class Switch_OnMinionToggle
    {

        private static bool Prefix(Switch __instance)
        {
            if (!CustomizeBuildingsState.StateManager.State.NoDupeSwitches) return true;

            AccessTools.Method(typeof(Switch), "Toggle").Invoke(__instance, null);
            return false;
        }
    }

    [HarmonyPatch(typeof(Valve), "ChangeFlow")]
    internal class Valve_ChangeFlow
    {

        private static bool Prefix(ref Valve __instance, float amount, ref ValveBase ___valveBase, ref float ___desiredFlow)
        {
            if (!CustomizeBuildingsState.StateManager.State.NoDupeValves) return true;

            ___desiredFlow = Mathf.Clamp(amount, 0.0f, ___valveBase.MaxFlow);

            KSelectable component = __instance.GetComponent<KSelectable>();
            component.ToggleStatusItem(Db.Get().BuildingStatusItems.PumpingLiquidOrGas, ___desiredFlow >= 0.0, (object)___valveBase.AccumulatorHandle);

            __instance.UpdateFlow();
            return false;
        }
    }

    [HarmonyPatch(typeof(Toggleable), nameof(Toggleable.QueueToggle))]
    internal class Toggleable_QueueToggle
    {

        private static bool Prefix(int targetIdx, Toggleable __instance)
        {
            if (!CustomizeBuildingsState.StateManager.State.NoDupeToogleBuildings) return true;
            if (__instance.targets[targetIdx].Value != null) return true;

            try
            {
                __instance.targets[targetIdx].Key.HandleToggle();
            }
            catch (System.Exception) { }
            return false;
        }
    }

    [HarmonyPatch(typeof(Door), "QueueStateChange")]
    public class Door_QueueStateChange
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeToogleDoors;
        }

        public static bool Prefix(Door.ControlState nextState, Door __instance, ref Door.ControlState ___requestedState, ref Door.ControlState ___controlState)
        {
            //Debug.Log( nextState.ToString() +" : "+ ___requestedState.ToString() + " : " + ___controlState.ToString() + " : " + __instance.ToString() );
            //if (!CustomizeBuildingsState.StateManager.State.NoDupeToogleDoors) return true;

            if (___requestedState == nextState || ___controlState == nextState) return true;

            ___requestedState = nextState;
            ___controlState = nextState;
            AccessTools.Method(typeof(Door), "RefreshControlState").Invoke(__instance, null);
            AccessTools.Method(typeof(Door), "OnOperationalChanged").Invoke(__instance, new object[] { null });
            __instance.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, false);
            __instance.Open();
            __instance.Close();
            return false;
        }
    }


}
