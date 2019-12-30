using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace CustomizePlants
{
    [HarmonyPatch(typeof(ColdBreatherConfig), "CreatePrefab")]
    public static class ColdBreatherConfigMod
    {
        public static void Postfix(GameObject __result)
        {
            ColdBreather coldBreather = __result.GetComponent<ColdBreather>();
            if (coldBreather != null)
                coldBreather.deltaEmitTemperature = CustomizePlantsState.StateManager.State.WheezewortTempDelta;
            else
                Debug.LogWarning("ERROR: coldbreather was null!");
        }
    }
}