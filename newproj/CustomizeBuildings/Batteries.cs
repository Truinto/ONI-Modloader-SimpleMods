using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using TUNING;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(BatterySmartConfig), "DoPostConfigureComplete")]
    internal class BatterySmartConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            BatterySmart batterySmart = go.AddOrGet<BatterySmart>();
            batterySmart.capacity = (float)CustomizeBuildingsState.StateManager.State.BatterySmartKJ;
            if (CustomizeBuildingsState.StateManager.State.BatterySmartNoRunOff)
                batterySmart.joulesLostPerSecond = 0f;
        }
    }

    [HarmonyPatch(typeof(BatteryMediumConfig), "DoPostConfigureComplete")]
    internal class BatteryMediumConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Battery battery = go.AddOrGet<Battery>();
            battery.capacity = (float)CustomizeBuildingsState.StateManager.State.BatteryLargeKJ;
        }
    }

    //[HarmonyPatch(typeof(BatteryMediumConfig), "CreateBuildingDef", new Type[] { })]
    //internal class BatteryMediumConfig_CreateBuildingDef
    //{
    //    private static bool Prefix(BatteryMediumConfig __instance, ref BuildingDef __result)
    //    {
    //    }
    //}

    public class BatteryModuleConfig_Mod : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == BatteryModuleConfig.ID && CustomizeBuildingsState.StateManager.State.SpaceBattery != 100000f;
        }

        public void Edit(BuildingDef def)
        {
            var ModuleBattery = def.BuildingComplete.GetComponent<ModuleBattery>();
            if (ModuleBattery != null)
            {
                ModuleBattery.capacity = CustomizeBuildingsState.StateManager.State.SpaceBattery;
                ModuleBattery.joulesLostPerSecond = 0f;
            }
        }

        public void Undo(BuildingDef def)
        {
            throw new NotImplementedException();
        }
    }
}