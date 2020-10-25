using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(SteamTurbineConfig2), "CreateBuildingDef")]
    internal class SteamTurbineConfig2_CreateBuildingDef
    {
        internal static bool isGas;

        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SteamTurbineEnabled;
        }

        internal static void Postfix(ref BuildingDef __result)
        {
            isGas = ElementLoader.FindElementByName(CustomizeBuildingsState.StateManager.State.SteamTurbineOutputElement)?.IsGas ?? false;
            __result.OutputConduitType = isGas ? ConduitType.Gas : ConduitType.Liquid;

            if (CustomizeBuildingsState.StateManager.State.SteamTurbineWattage != SteamTurbineConfig2.MAX_WATTAGE) //850f
            {
                __result.GeneratorWattageRating = CustomizeBuildingsState.StateManager.State.SteamTurbineWattage;
                __result.GeneratorBaseCapacity = CustomizeBuildingsState.StateManager.State.SteamTurbineWattage;
            }
        }
    }

    [HarmonyPatch(typeof(SteamTurbineConfig2), "DoPostConfigureComplete")]
    internal class SteamTurbineConfig2_DoPostConfigureComplete
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SteamTurbineEnabled;
        }

        internal static void Postfix(GameObject go)
        {
            SteamTurbine steamTurbine = go.GetComponent<SteamTurbine>();
            steamTurbine.srcElem = CustomizeBuildingsState.StateManager.State.SteamTurbineSourceElement.ToSimHash(SimHashes.Steam); //SimHashes.Steam
            steamTurbine.destElem = CustomizeBuildingsState.StateManager.State.SteamTurbineOutputElement.ToSimHash(SimHashes.Water); //SimHashes.Water
            steamTurbine.pumpKGRate = CustomizeBuildingsState.StateManager.State.SteamTurbinePumpRateKG; //2f
            steamTurbine.maxSelfHeat = CustomizeBuildingsState.StateManager.State.SteamTurbineMaxSelfHeat; //64f
            steamTurbine.wasteHeatToTurbinePercent = CustomizeBuildingsState.StateManager.State.SteamTurbineHeatTransferPercent; //0.1f
            
            //steamTurbine.requiredMass = CustomizeBuildingsState.StateManager.State.SteamTurbineMinMass; //1f / 1000f;
            steamTurbine.minActiveTemperature = CustomizeBuildingsState.StateManager.State.SteamTurbineMinActiveTemperature; //398.15f;
            steamTurbine.idealSourceElementTemperature = CustomizeBuildingsState.StateManager.State.SteamTurbineIdealTemperature; //473.15f;
            steamTurbine.outputElementTemperature = CustomizeBuildingsState.StateManager.State.SteamTurbineOutputTemperature; //368.15f;
            steamTurbine.maxBuildingTemperature = CustomizeBuildingsState.StateManager.State.SteamTurbineOverheatTemperature; //373.15f;
        
            ConduitDispenser conduitDispenser = go.GetComponent<ConduitDispenser>();
            conduitDispenser.elementFilter = new SimHashes[0];
            conduitDispenser.conduitType = SteamTurbineConfig2_CreateBuildingDef.isGas ? ConduitType.Gas : ConduitType.Liquid;
        }
    }

    //[HarmonyPatch(typeof(Turbine.Instance), nameof(Turbine.Instance.UpdateStatusItems))]
    //internal class SteamTurbineInputBlockedFix_Patch
    //{
    //    internal static bool? isLiquid;
    //    internal static void Prefix(Turbine.Instance __instance)
    //    {
    //        if (!isLiquid.HasValue) isLiquid = ElementLoader.FindElementByName(CustomizeBuildingsState.StateManager.State.SteamTurbineSourceElement)?.IsLiquid ?? false;
    //        if (isLiquid.Value)
    //            __instance.isInputBlocked = false;
    //    }
    //}
}