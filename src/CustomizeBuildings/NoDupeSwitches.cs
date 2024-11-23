using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(Switch), nameof(Switch.OnMinionToggle))]
    internal class Switch_OnMinionToggle
    {

        private static bool Prefix(Switch __instance)
        {
            if (!CustomizeBuildingsState.StateManager.State.NoDupeSwitches) return true;

            AccessTools.Method(typeof(Switch), "Toggle").Invoke(__instance, null);
            return false;
        }
    }

    [HarmonyPatch(typeof(Valve), nameof(Valve.ChangeFlow))]
    internal class Valve_ChangeFlow
    {

        private static bool Prefix(float amount, Valve __instance)
        {
            if (!CustomizeBuildingsState.StateManager.State.NoDupeValves) return true;

            __instance.desiredFlow = Mathf.Clamp(amount, 0.0f, __instance.valveBase.MaxFlow);

            KSelectable component = __instance.GetComponent<KSelectable>();
            component.ToggleStatusItem(Db.Get().BuildingStatusItems.PumpingLiquidOrGas, __instance.desiredFlow >= 0.0, (object)__instance.valveBase.AccumulatorHandle);

            __instance.UpdateFlow();
            return false;
        }
    }

    [HarmonyPatch(typeof(Toggleable), nameof(Toggleable.QueueToggle))]
    internal class Toggleable_QueueToggle
    {

        private static bool Prefix(int targetIdx, Toggleable __instance)
        {
            if (!CustomizeBuildingsState.StateManager.State.NoDupeToogleBuildings)
                return true;
            if (__instance.targets[targetIdx].Value != null)
                return true;

            try
            {
                __instance.targets[targetIdx].Key.HandleToggle();
            }
            catch (System.Exception) { }
            return false;
        }
    }

    [HarmonyPatch(typeof(Door), nameof(Door.QueueStateChange))]
    public class Door_QueueStateChange
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeToogleDoors;
        }

        public static bool Prefix(Door.ControlState nextState, Door __instance)
        {
            if (__instance.requestedState == nextState || __instance.controlState == nextState)
                return true;

            __instance.requestedState = nextState;
            __instance.controlState = nextState;
            __instance.RefreshControlState();
            __instance.OnOperationalChanged(null);
            __instance.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, false);
            __instance.Open();
            __instance.Close();
            return false;
        }
    }
}
