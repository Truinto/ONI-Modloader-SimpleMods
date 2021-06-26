using HarmonyLib;
using TUNING;
using UnityEngine;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(ElectrolyzerConfig), "ConfigureBuildingTemplate")]
    internal class ElectrolyzerConfig_ConfigureBuildingTemplate
    {

        private static void Postfix(GameObject go)
        {
            Electrolyzer electrolyzer = go.GetComponent<Electrolyzer>();
            if (electrolyzer == null) return;
            electrolyzer.maxMass = CustomizeBuildingsState.StateManager.State.ElectrolizerMaxPressure;
        }
    }
}