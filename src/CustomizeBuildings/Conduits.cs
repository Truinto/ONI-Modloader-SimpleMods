using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;

namespace CustomizeBuildings
{
    public class WarpConduitSender_Patch : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == WarpConduitSenderConfig.ID;
        }

        public void Edit(BuildingDef def)
        {
            var sender = def.BuildingComplete.GetComponent<WarpConduitSender>();
            sender.gasStorage.capacityKg = CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure;
            sender.liquidStorage.capacityKg = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure;
            sender.solidStorage.capacityKg = CustomizeBuildingsState.StateManager.State.ConveyorRailPackageSize * 5f;
        }

        public void Undo(BuildingDef def)
        {
            throw new NotImplementedException();
        }
    }

    public class ConductionPanel_Patch : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == ContactConductivePipeBridgeConfig.ID && CustomizeBuildingsState.StateManager.State.ConductivePanelPressure != 10f;
        }

        public void Edit(BuildingDef def)
        {
            var storage = def.BuildingComplete.GetComponent<Storage>();
            storage.capacityKg = CustomizeBuildingsState.StateManager.State.ConductivePanelPressure;

            var conduitConsumer = def.BuildingComplete.GetComponent<ConduitConsumer>();
            conduitConsumer.capacityKG = CustomizeBuildingsState.StateManager.State.ConductivePanelPressure;

            var conductiveDef = def.BuildingComplete.GetDef<ContactConductivePipeBridge.Def>();
            conductiveDef.pumpKGRate = CustomizeBuildingsState.StateManager.State.ConductivePanelPressure;
        }

        public void Undo(BuildingDef def)
        {
            throw new NotImplementedException();
        }
    }

    [HarmonyPatch(typeof(ConduitFlow), nameof(ConduitFlow.AddElement))]
    public class ConduitFlow_AddElement
    {
        public static float limit = 100000f;
        public static void Prefix(ref float mass)
        {
            mass = Math.Min(mass, limit);
        }
    }

    [HarmonyPatch(typeof(Game), "OnPrefabInit")]
    public class Game_OnPrefabInit
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10f || CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1f;
        }

        public static void Postfix(Game __instance)
        {
            if (CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10f)
                AccessTools.Field(typeof(ConduitFlow), "MaxMass").SetValue(__instance.liquidConduitFlow, CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure);

            if (CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1f)
                AccessTools.Field(typeof(ConduitFlow), "MaxMass").SetValue(__instance.gasConduitFlow, CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure);
        }
    }

    [HarmonyPatch(typeof(LiquidValveConfig), "ConfigureBuildingTemplate")]
    public class LiquidValveConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10f
                && !LiquidValveConfig_ConfigureBuildingTemplate2.Prepare();
        }

        public static void Postfix(GameObject go)
        {
            if (go.GetComponent<ValvePressure>() == null)
            {
                var valveBase = go.AddOrGet<ValveBase>();
                if (valveBase != null)
                    valveBase.maxFlow = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure;
            }
        }
    }

    [HarmonyPatch(typeof(GasValveConfig), "ConfigureBuildingTemplate")]
    public class GasValveConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1f
                && !GasValveConfig_ConfigureBuildingTemplate2.Prepare();
        }

        public static void Postfix(GameObject go)
        {
            if (go.GetComponent<ValvePressure>() == null)
            {
                var valveBase = go.AddOrGet<ValveBase>();
                if (valveBase != null)
                    valveBase.maxFlow = CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure;
            }
        }
    }

    [HarmonyPatch(typeof(LiquidLogicValveConfig), "ConfigureBuildingTemplate")]
    public class LiquidLogicValveConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10f;
        }

        public static void Postfix(GameObject go)
        {
            go.AddOrGet<OperationalValve>().maxFlow = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure;
        }
    }

    [HarmonyPatch(typeof(GasLogicValveConfig), "ConfigureBuildingTemplate")]
    public class GasLogicValveConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1f;
        }

        public static void Postfix(GameObject go)
        {
            go.AddOrGet<OperationalValve>().maxFlow = CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure;
        }
    }

    [HarmonyPatch(typeof(LiquidConditionerConfig), "ConfigureBuildingTemplate")]
    public class LiquidConditionerConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure != 10f || CustomizeBuildingsState.StateManager.State.PipeThroughputPercent != 1.0f;
        }

        public static void Postfix(GameObject go)
        {
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure * CustomizeBuildingsState.StateManager.State.PipeThroughputPercent;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = conduitConsumer.consumptionRate * 2f;
        }
    }

    [HarmonyPatch(typeof(AirConditionerConfig), "ConfigureBuildingTemplate")]
    public class AirConditionerConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure != 1f || CustomizeBuildingsState.StateManager.State.PipeThroughputPercent != 1.0f;
        }

        public static void Postfix(GameObject go)
        {
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure * CustomizeBuildingsState.StateManager.State.PipeThroughputPercent;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = conduitConsumer.consumptionRate * 2f;
        }
    }

    [HarmonyPatch(typeof(LiquidPumpConfig), "DoPostConfigureComplete")]
    public class LiquidPumpConfig_DoPostConfigureComplete
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidPump != 10f;
        }

        public static void Postfix(GameObject go)
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
    public class GasPumpConfig_DoPostConfigureComplete
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeGasPump != 0.5f;
        }

        public static void Postfix(GameObject go)
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

    [HarmonyPatch(typeof(LiquidMiniPumpConfig), "DoPostConfigureComplete")]
    public class LiquidMiniPumpConfig_DoPostConfigureComplete
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeLiquidPumpMini != 1f;
        }

        public static void Postfix(GameObject go)
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

            storage.capacityKg = CustomizeBuildingsState.StateManager.State.PipeLiquidPumpMini * 2;

            elementConsumer.consumptionRate = CustomizeBuildingsState.StateManager.State.PipeLiquidPumpMini;
        }
    }

    [HarmonyPatch(typeof(GasMiniPumpConfig), "DoPostConfigureComplete")]
    public class GasMiniPumpConfig_DoPostConfigureComplete
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeGasPumpMini != 0.05f;
        }

        public static void Postfix(GameObject go)
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

            storage.capacityKg = CustomizeBuildingsState.StateManager.State.PipeGasPumpMini * 2;

            elementConsumer.consumptionRate = CustomizeBuildingsState.StateManager.State.PipeGasPumpMini;
        }
    }

    [HarmonyPatch(typeof(SolidConduitDispenser), "ConduitUpdate")]
    public class SolidConduitDispenser_ConduitUpdate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.ConveyorRailPackageSize != 20f;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
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