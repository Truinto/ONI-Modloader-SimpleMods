using Harmony;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;

namespace CustomizeBuildings
{

    [HarmonyPatch(typeof(Game), "OnPrefabInit")]
    internal class Game_OnPrefabInit
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10 || CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1;
        }

        internal static void Postfix(Game __instance)
        {
            if (CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10)
                AccessTools.Field(typeof(ConduitFlow), "MaxMass").SetValue(__instance.liquidConduitFlow, CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure);

            if (CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1)
                AccessTools.Field(typeof(ConduitFlow), "MaxMass").SetValue(__instance.gasConduitFlow, CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure);
        }
    }

    [HarmonyPatch(typeof(LiquidValveConfig), "ConfigureBuildingTemplate")]
    internal class LiquidValveConfig_ConfigureBuildingTemplate
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10;
        }

        internal static void Postfix(GameObject go)
        {
            go.AddOrGet<ValveBase>().maxFlow = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure;
        }
    }

    [HarmonyPatch(typeof(GasValveConfig), "ConfigureBuildingTemplate")]
    internal class GasValveConfig_ConfigureBuildingTemplate
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1;
        }

        internal static void Postfix(GameObject go)
        {
            go.AddOrGet<ValveBase>().maxFlow = CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure;
        }
    }

    [HarmonyPatch(typeof(LiquidLogicValveConfig), "ConfigureBuildingTemplate")]
    internal class LiquidLogicValveConfig_ConfigureBuildingTemplate
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10;
        }

        internal static void Postfix(GameObject go)
        {
            go.AddOrGet<OperationalValve>().maxFlow = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure;
        }
    }

    [HarmonyPatch(typeof(GasLogicValveConfig), "ConfigureBuildingTemplate")]
    internal class GasLogicValveConfig_ConfigureBuildingTemplate
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1;
        }

        internal static void Postfix(GameObject go)
        {
            go.AddOrGet<OperationalValve>().maxFlow = CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure;
        }
    }

    [HarmonyPatch(typeof(LiquidConditionerConfig), "ConfigureBuildingTemplate")]
    internal class LiquidConditionerConfig_ConfigureBuildingTemplate
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10;
        }

        internal static void Postfix(GameObject go)
        {
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = conduitConsumer.consumptionRate * 2f;
        }
    }
    
    [HarmonyPatch(typeof(AirConditionerConfig), "ConfigureBuildingTemplate")]
    internal class AirConditionerConfig_ConfigureBuildingTemplate
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1;
        }

        internal static void Postfix(GameObject go)
        {
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = conduitConsumer.consumptionRate * 2f;
        }
    }

    [HarmonyPatch(typeof(LiquidPumpConfig), "DoPostConfigureComplete")]
    internal class LiquidPumpConfig_DoPostConfigureComplete
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidPump != 10f;
        }

        private static void Postfix(GameObject go)
        {
            ElementConsumer elementConsumer = go.GetComponent<ElementConsumer>();
            if (elementConsumer == null)
            {
                Debug.LogWarning("LiquidPumpConfig_DoPostConfigureComplete elementConsumer was null");
                return;
            }

            Storage storage = go.GetComponent<Storage>();
            if (storage == null)
            {
                Debug.LogWarning("LiquidPumpConfig_DoPostConfigureComplete storage was null");
                return;
            }

            storage.capacityKg = CustomizeBuildingsState.StateManager.State.PipeLiquidPump * 2;

            elementConsumer.consumptionRate = CustomizeBuildingsState.StateManager.State.PipeLiquidPump;
        }
    }

    [HarmonyPatch(typeof(GasPumpConfig), "DoPostConfigureComplete")]
    internal class GasPumpConfig_DoPostConfigureComplete
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeGasPump != 0.5f;
        }

        private static void Postfix(GameObject go)
        {
            ElementConsumer elementConsumer = go.GetComponent<ElementConsumer>();
            if (elementConsumer == null)
            {
                Debug.LogWarning("GasPumpConfig_DoPostConfigureComplete elementConsumer was null");
                return;
            }

            Storage storage = go.GetComponent<Storage>();
            if (storage == null)
            {
                Debug.LogWarning("GasPumpConfig_DoPostConfigureComplete storage was null");
                return;
            }

            storage.capacityKg = CustomizeBuildingsState.StateManager.State.PipeGasPump * 2;

            elementConsumer.consumptionRate = CustomizeBuildingsState.StateManager.State.PipeGasPump;
        }
    }

    [HarmonyPatch(typeof(SolidConduitDispenser), "ConduitUpdate")]
    internal class SolidConduitDispenser_ConduitUpdate
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.ConveyorRailPackageSize != 20f;
        } 

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            float newMass = CustomizeBuildingsState.StateManager.State.ConveyorRailPackageSize;
            List<CodeInstruction> list = instr.ToList<CodeInstruction>();

            // if ((double) suitableItem.PrimaryElement.Mass > 20.0)
            // suitableItem = suitableItem.Take(20f);

            foreach (CodeInstruction line in list)
            {
                if (line.opcode == OpCodes.Ldc_R4 && (float)line.operand == 20f)    //IL_0079: ldc.r4       20
                {
                    line.operand = newMass;
                }
            }

            return list;
        }
    }
    
}