using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;


namespace CustomizeBuildings
{

    [HarmonyPatch(typeof(CometDetector.Instance), "RerollAccuracy")]
    public class CometDetector_RerollAccuracy
    {
        public static void Postfix(DetectorNetwork.Def ___detectorNetworkDef)
        {
            if (___detectorNetworkDef == null) return;
            
            ___detectorNetworkDef.interferenceRadius = CustomizeBuildingsState.StateManager.State.ScannerInterferenceRadius;
            ___detectorNetworkDef.worstWarningTime = CustomizeBuildingsState.StateManager.State.ScannerWorstWarningTime;
            ___detectorNetworkDef.bestWarningTime = CustomizeBuildingsState.StateManager.State.ScannerBestWarningTime;
            ___detectorNetworkDef.bestNetworkSize = CustomizeBuildingsState.StateManager.State.ScannerBestNetworkSize;
        }
    }

    [HarmonyPatch(typeof(ClusterTelescopeConfig), nameof(ClusterTelescopeConfig.ConfigureBuildingTemplate))]
    public class ClusterTelescopeConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.TelescopeClearCellRadius != 5
                || CustomizeBuildingsState.StateManager.State.TelescopeAnalyzeRadius != 3
                || (CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeTelescope);
        }

        public static void Postfix(GameObject go)
        {
            ClusterTelescope.Def def = go.AddOrGetDef<ClusterTelescope.Def>();
            def.clearScanCellRadius = CustomizeBuildingsState.StateManager.State.TelescopeClearCellRadius;
            def.analyzeClusterRadius = CustomizeBuildingsState.StateManager.State.TelescopeAnalyzeRadius;

            if (CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeTelescope)
                go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
        }
    }

    [HarmonyPatch(typeof(ClusterTelescope.Instance), nameof(ClusterTelescope.Instance.HasSkyVisibility))]
    public class ClusterTelescope_HasSkyVisibility
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.TelescopeClearCellRadius <= 0;
        }

        public static bool Prefix(ref float ___m_percentClear, ref bool __result)
        {
            ___m_percentClear = 1;
            __result = true;
            return false;
        }
    }
}