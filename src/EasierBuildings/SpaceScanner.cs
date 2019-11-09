using Harmony;
using System;
using UnityEngine;
using System.Collections.Generic;


namespace CustomizeBuildings
{

    [HarmonyPatch(typeof(CometDetector.Instance), "RerollAccuracy")]
    internal class CometDetector_RerollAccuracy
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
}