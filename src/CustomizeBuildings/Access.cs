using HarmonyLib;
using UnityEngine;
using System;
using Common;
using static HarmonyLib.AccessTools;
using Klei.AI;
using System.Reflection;

namespace CustomizeBuildings
{
    public class Access
    {
        // Property Examples:
        //public static Func<AirConditioner, float> Get_lastEnvTemp = MethodDelegate<Func<AirConditioner, float>>(PropertyGetter(typeof(AirConditioner), nameof(AirConditioner.lastEnvTemp)));
        //public static Action<AirConditioner, float> Set_lastEnvTemp = MethodDelegate<Action<AirConditioner, float>>(PropertySetter(typeof(AirConditioner), nameof(AirConditioner.lastEnvTemp)));

        public static Action<AirConditioner, float> lastEnvTemp;
        public static Action<AirConditioner, float> lastGasTemp;
        public static Action<AirConditioner> UpdateStatus;

        static Access()
        {
            // no inline definitions, so we get more meaningful debug expections
            try
            {
                lastEnvTemp = MethodDelegate<Action<AirConditioner, float>>(PropertySetter(typeof(AirConditioner), nameof(AirConditioner.lastEnvTemp)));
                lastGasTemp = MethodDelegate<Action<AirConditioner, float>>(PropertySetter(typeof(AirConditioner), nameof(AirConditioner.lastGasTemp)));

                UpdateStatus = MethodDelegate<Action<AirConditioner>>(Method(typeof(AirConditioner), "UpdateStatus"));

                foreach (var field in typeof(Access).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (field.GetValue(null) == null)
                        Helpers.Print($"Error: Field Access.{field.Name} is null!");
                }
            }
            catch (Exception e)
            {
                Helpers.Print(e.ToString());
            }
        }
    }
}