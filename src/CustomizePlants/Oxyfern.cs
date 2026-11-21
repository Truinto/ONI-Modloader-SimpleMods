using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Common;

namespace CustomizePlants
{
    [HarmonyPatch(typeof(OxyfernConfig), "CreatePrefab")]
    public static class OxyfernConfigMod
    {
        public static bool Prepare()
        {
            return CustomizePlantsState.Instance.OxyfernOxygenPerSecond != 0.03125f;
        }

        public static void Postfix(GameObject __result)
        {
            float oxyfernOxygenPerSecond = CustomizePlantsState.Instance.OxyfernOxygenPerSecond;
            ElementConverter elementConverter = __result.GetComponent<ElementConverter>();
            if (elementConverter == null)
            {
                Helpers.Print("OxyfernConfig_CreatePrefab elementConverter was null");
                return;
            }

            if (elementConverter.consumedElements.Count() == 1)
                elementConverter.consumedElements[0].MassConsumptionRate = oxyfernOxygenPerSecond / 50f;

            if (elementConverter.outputElements.Count() == 1)
                elementConverter.outputElements[0].massGenerationRate = oxyfernOxygenPerSecond;
        }
    }

    [HarmonyPatch(typeof(Oxyfern), "SetConsumptionRate")]
    public static class Oxyfern_SetConsumptionRate
    {
        public static bool Prepare()
        {
            return CustomizePlantsState.Instance.OxyfernOxygenPerSecond != 0.03125f;
        }

        public static bool Prefix(Oxyfern __instance, ReceptacleMonitor ___receptacleMonitor)
        {
            float oxyfernOxygenPerSecond = CustomizePlantsState.Instance.OxyfernOxygenPerSecond;

            ElementConsumer elementConsumer = __instance.GetComponent<ElementConsumer>();
            if (elementConsumer != null)
            {
                if (___receptacleMonitor.Replanted)
                    elementConsumer.consumptionRate = oxyfernOxygenPerSecond / 50f;
                else
                    elementConsumer.consumptionRate = oxyfernOxygenPerSecond / 200f;
                return false;
            }
            else
                Helpers.Print("Oxyfern_SetConsumptionRate elementConsumer was null");
            return true;
        }
    }
}