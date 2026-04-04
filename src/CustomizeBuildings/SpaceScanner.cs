using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;


namespace CustomizeBuildings
{
    [HarmonyPatch]
    public static class Comet_UpdateNetworkQualityFor
    {
        [HarmonyPatch(typeof(SpaceScannerNetworkManager), nameof(SpaceScannerNetworkManager.CalcWorldNetworkQuality))]
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            __result *= CustomizeBuildingsState.Instance.ScannerQualityMultiplier;
        }
    }

    [HarmonyPatch(typeof(ClusterTelescopeConfig), nameof(ClusterTelescopeConfig.ConfigureBuildingTemplate))]
    public class ClusterTelescopeConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.TelescopeClearCellRadius != 5
                || CustomizeBuildingsState.Instance.TelescopeAnalyzeRadius != 3
                || (CustomizeBuildingsState.Instance.NoDupeGlobal && CustomizeBuildingsState.Instance.NoDupeTelescope);
        }

        public static void Postfix(GameObject go)
        {
            ClusterTelescope.Def def = go.AddOrGetDef<ClusterTelescope.Def>();
            def.clearScanCellRadius = CustomizeBuildingsState.Instance.TelescopeClearCellRadius;
            def.analyzeClusterRadius = CustomizeBuildingsState.Instance.TelescopeAnalyzeRadius;

            if (CustomizeBuildingsState.Instance.NoDupeGlobal && CustomizeBuildingsState.Instance.NoDupeTelescope)
                go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
        }
    }

    [HarmonyPatch(typeof(ClusterTelescope.Instance), nameof(ClusterTelescope.Instance.HasSkyVisibility))]
    public class ClusterTelescope_HasSkyVisibility
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.TelescopeClearCellRadius <= 0;
        }

        public static bool Prefix(ref float ___m_percentClear, ref bool __result)
        {
            ___m_percentClear = 1;
            __result = true;
            return false;
        }
    }
}