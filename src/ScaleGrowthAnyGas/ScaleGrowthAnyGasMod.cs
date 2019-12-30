using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace ScaleGrowthAnyGas
{
	[HarmonyPatch(typeof(Game), "OnPrefabInit")]
	internal class ScaleGrowthAnyGasMod_Game_OnPrefabInit
	{
		private static void Postfix(Game __instance)
		{
			Debug.Log(" === ScaleGrowthAnyGasMod_Game_OnPrefabInit Postfix === ");
		}
    }

    [HarmonyPatch(typeof(ScaleGrowthMonitor), "IsInCorrectAtmosphere")]
    internal class ScaleGrowthAnyGas_ScaleGrowthMonitor_IsInCorrectAtmosphere
    {
        private static void Postfix(ref bool __result, ScaleGrowthMonitor.Instance smi)
        {
            __result = true;
        }
    }
}
