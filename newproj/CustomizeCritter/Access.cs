using HarmonyLib;
using UnityEngine;
using System;
using Common;
using static HarmonyLib.AccessTools;
using Klei.AI;
using System.Reflection;

namespace CustomizeCritter
{
    public class Access
    {
        public static FieldRef<TemperatureVulnerable, float> internalTemperatureLethal_Low;
        public static FieldRef<TemperatureVulnerable, float> internalTemperatureWarning_Low;
        public static FieldRef<TemperatureVulnerable, float> internalTemperatureWarning_High;
        public static FieldRef<TemperatureVulnerable, float> internalTemperatureLethal_High;

        static Access()
        {
            // no inline definitions, so we get more meaningful debug expections
            try
            {
                internalTemperatureLethal_Low = FieldRefAccess<TemperatureVulnerable, float>("internalTemperatureLethal_Low");
                internalTemperatureWarning_Low = FieldRefAccess<TemperatureVulnerable, float>("internalTemperatureWarning_Low");
                internalTemperatureWarning_High = FieldRefAccess<TemperatureVulnerable, float>("internalTemperatureWarning_High");
                internalTemperatureLethal_High = FieldRefAccess<TemperatureVulnerable, float>("internalTemperatureLethal_High");
            }
            catch (Exception e)
            {
                Helpers.PrintDebug(e.ToString());
            }
        }
    }
}