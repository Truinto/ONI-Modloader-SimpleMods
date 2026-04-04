using Common;
using HarmonyLib;
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
            return CustomizeBuildingsState.Instance.SteamTurbineEnabled;
        }

        internal static void Postfix(ref BuildingDef __result)
        {
            isGas = ElementLoader.FindElementByName(CustomizeBuildingsState.Instance.SteamTurbineOutputElement)?.IsGas ?? false;
            __result.OutputConduitType = isGas ? ConduitType.Gas : ConduitType.Liquid;

            if (CustomizeBuildingsState.Instance.SteamTurbineWattage != 850f)
            {
                SteamTurbineConfig2.MAX_WATTAGE = CustomizeBuildingsState.Instance.SteamTurbineWattage;
                __result.GeneratorWattageRating = CustomizeBuildingsState.Instance.SteamTurbineWattage;
                __result.GeneratorBaseCapacity = CustomizeBuildingsState.Instance.SteamTurbineWattage;
            }
        }
    }

    [HarmonyPatch(typeof(SteamTurbineConfig2), "DoPostConfigureComplete")]
    internal class SteamTurbineConfig2_DoPostConfigureComplete
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.SteamTurbineEnabled;
        }

        internal static void Postfix(GameObject go)
        {
            SteamTurbine steamTurbine = go.GetComponent<SteamTurbine>();
            steamTurbine.srcElem = CustomizeBuildingsState.Instance.SteamTurbineSourceElement.ToSimHash(SimHashes.Steam); //SimHashes.Steam
            steamTurbine.destElem = CustomizeBuildingsState.Instance.SteamTurbineOutputElement.ToSimHash(SimHashes.Water); //SimHashes.Water
            steamTurbine.pumpKGRate = CustomizeBuildingsState.Instance.SteamTurbinePumpRateKG; //2f
            steamTurbine.wasteHeatToTurbinePercent = CustomizeBuildingsState.Instance.SteamTurbineHeatTransferPercent; //0.1f

            //steamTurbine.requiredMass = CustomizeBuildingsState.Instance.SteamTurbineMinMass; //1f / 1000f;
            steamTurbine.minActiveTemperature = CustomizeBuildingsState.Instance.SteamTurbineMinActiveTemperature; //398.15f;
            steamTurbine.idealSourceElementTemperature = CustomizeBuildingsState.Instance.SteamTurbineIdealTemperature; //473.15f;
            steamTurbine.outputElementTemperature = CustomizeBuildingsState.Instance.SteamTurbineOutputTemperature; //368.15f;
            steamTurbine.maxBuildingTemperature = CustomizeBuildingsState.Instance.SteamTurbineOverheatTemperature; //373.15f;

            ConduitDispenser conduitDispenser = go.GetComponent<ConduitDispenser>();
            conduitDispenser.elementFilter = [];
            conduitDispenser.conduitType = SteamTurbineConfig2_CreateBuildingDef.isGas ? ConduitType.Gas : ConduitType.Liquid;
        }
    }

    //[HarmonyPatch(typeof(Turbine.Instance), nameof(Turbine.Instance.UpdateStatusItems))]
    //internal class SteamTurbineInputBlockedFix_Patch
    //{
    //    internal static bool? isLiquid;
    //    internal static void Prefix(Turbine.Instance __instance)
    //    {
    //        if (!isLiquid.HasValue) isLiquid = ElementLoader.FindElementByName(CustomizeBuildingsState.Instance.SteamTurbineSourceElement)?.IsLiquid ?? false;
    //        if (isLiquid.Value)
    //            __instance.isInputBlocked = false;
    //    }
    //}
}
