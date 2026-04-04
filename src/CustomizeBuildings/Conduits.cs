using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;

namespace CustomizeBuildings
{
    public class Bottler_Patch : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            if (id is BottleEmptierConduitLiquidConfig.ID && CustomizeBuildingsState.Instance.PipeLiquidMaxPressure != 10f)
                return true;
            if (id is BottleEmptierConduitGasConfig.ID && CustomizeBuildingsState.Instance.PipeGasMaxPressure != 1f)
                return true;
            return false;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var emptier = def.BuildingComplete.GetComponent<BottleEmptier>();
            if (emptier != null)
            {
                emptier.emptyRate =
                    emptier.isGasEmptier
                    ? CustomizeBuildingsState.Instance.PipeGasMaxPressure * CustomizeBuildingsState.Instance.PipeThroughputPercent
                    : CustomizeBuildingsState.Instance.PipeLiquidMaxPressure * CustomizeBuildingsState.Instance.PipeThroughputPercent;
            }
        }
    }

    public class WarpConduitSender_Patch : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == WarpConduitSenderConfig.ID;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var sender = def.BuildingComplete.GetComponent<WarpConduitSender>();
            sender.gasStorage.capacityKg = CustomizeBuildingsState.Instance.PipeGasMaxPressure * 2f;
            sender.liquidStorage.capacityKg = CustomizeBuildingsState.Instance.PipeLiquidMaxPressure * 2f;
            sender.solidStorage.capacityKg = CustomizeBuildingsState.Instance.ConveyorRailPackageSize * 5f;
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
            return CustomizeBuildingsState.Instance.PipeLiquidMaxPressure != 10f || CustomizeBuildingsState.Instance.PipeGasMaxPressure != 1f;
        }

        public static void Postfix(Game __instance)
        {
            if (CustomizeBuildingsState.Instance.PipeLiquidMaxPressure != 10f)
                __instance.liquidConduitFlow.MaxMass = CustomizeBuildingsState.Instance.PipeLiquidMaxPressure;

            if (CustomizeBuildingsState.Instance.PipeGasMaxPressure != 1f)
                __instance.gasConduitFlow.MaxMass = CustomizeBuildingsState.Instance.PipeGasMaxPressure;
        }
    }

    [HarmonyPatch(typeof(LiquidValveConfig), "ConfigureBuildingTemplate")]
    public class LiquidValveConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeLiquidMaxPressure != 10f;
        }

        public static void Postfix(GameObject go)
        {
            var valveBase = go.AddOrGet<ValveBase>();
            if (valveBase != null)
                valveBase.maxFlow = CustomizeBuildingsState.Instance.PipeLiquidMaxPressure;
        }
    }

    [HarmonyPatch(typeof(GasValveConfig), "ConfigureBuildingTemplate")]
    public class GasValveConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeGasMaxPressure != 1f;
        }

        public static void Postfix(GameObject go)
        {
            var valveBase = go.AddOrGet<ValveBase>();
            if (valveBase != null)
                valveBase.maxFlow = CustomizeBuildingsState.Instance.PipeGasMaxPressure;

        }
    }

    [HarmonyPatch(typeof(LiquidLogicValveConfig), "ConfigureBuildingTemplate")]
    public class LiquidLogicValveConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeLiquidMaxPressure != 10f;
        }

        public static void Postfix(GameObject go)
        {
            go.AddOrGet<OperationalValve>().maxFlow = CustomizeBuildingsState.Instance.PipeLiquidMaxPressure;
        }
    }

    [HarmonyPatch(typeof(GasLogicValveConfig), "ConfigureBuildingTemplate")]
    public class GasLogicValveConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeGasMaxPressure != 1f;
        }

        public static void Postfix(GameObject go)
        {
            go.AddOrGet<OperationalValve>().maxFlow = CustomizeBuildingsState.Instance.PipeGasMaxPressure;
        }
    }

    [HarmonyPatch(typeof(LiquidConditionerConfig), "ConfigureBuildingTemplate")]
    public class LiquidConditionerConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeLiquidMaxPressure != 10f || CustomizeBuildingsState.Instance.PipeThroughputPercent != 1.0f;
        }

        public static void Postfix(GameObject go)
        {
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = CustomizeBuildingsState.Instance.PipeLiquidMaxPressure * CustomizeBuildingsState.Instance.PipeThroughputPercent;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = conduitConsumer.consumptionRate * 2f;
        }
    }

    [HarmonyPatch(typeof(AirConditionerConfig), "ConfigureBuildingTemplate")]
    public class AirConditionerConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeGasMaxPressure != 1f || CustomizeBuildingsState.Instance.PipeThroughputPercent != 1.0f;
        }

        public static void Postfix(GameObject go)
        {
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = CustomizeBuildingsState.Instance.PipeGasMaxPressure * CustomizeBuildingsState.Instance.PipeThroughputPercent;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = conduitConsumer.consumptionRate * 2f;
        }
    }

    public class ConductivePanelMod : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id is ContactConductivePipeBridgeConfig.ID
                && (CustomizeBuildingsState.Instance.PipeLiquidMaxPressure != 10f
                    || CustomizeBuildingsState.Instance.PipeThroughputPercent != 1.0f);
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var pipeDef = def.BuildingComplete.GetDef<ContactConductivePipeBridge.Def>();
            pipeDef.pumpKGRate = CustomizeBuildingsState.Instance.PipeLiquidMaxPressure * CustomizeBuildingsState.Instance.PipeThroughputPercent;

            //var storage = def.BuildingComplete.GetComponent<Storage>();
            //storage.capacityKg = CustomizeBuildingsState.Instance.PipeLiquidMaxPressure * CustomizeBuildingsState.Instance.PipeThroughputPercent;

            var conduitConsumer = def.BuildingComplete.GetComponent<ConduitConsumer>();
            if (conduitConsumer != null)
                conduitConsumer.capacityKG = CustomizeBuildingsState.Instance.PipeLiquidMaxPressure * CustomizeBuildingsState.Instance.PipeThroughputPercent;
        }
    }

    public class SolidLimitValvelMod : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id is SolidLimitValveConfig.ID
                && (CustomizeBuildingsState.Instance.ConveyorMeterLimit != 500f);
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var limitValve = def.BuildingComplete.GetComponent<LimitValve>();
            limitValve.maxLimitKg = CustomizeBuildingsState.Instance.ConveyorMeterLimit;
        }
    }

    [HarmonyPatch(typeof(MorbRoverMaker.Def), nameof(MorbRoverMaker.Def.GetConduitMaxPackageMass))]
    public class MorbRoverMaker_GetConduitMaxPackageMass
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeGasMaxPressure != 1f
                    || CustomizeBuildingsState.Instance.PipeThroughputPercent != 1.0f;
        }

        public static bool Prefix(ref float __result)
        {
            __result = CustomizeBuildingsState.Instance.PipeGasMaxPressure * CustomizeBuildingsState.Instance.PipeThroughputPercent;
            return false;
        }
    }

    [HarmonyPatch(typeof(LiquidPumpConfig), "DoPostConfigureComplete")]
    public class LiquidPumpConfig_DoPostConfigureComplete
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeLiquidPump != 10f;
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

            storage.capacityKg = CustomizeBuildingsState.Instance.PipeLiquidPump * 2;

            elementConsumer.consumptionRate = CustomizeBuildingsState.Instance.PipeLiquidPump;
        }
    }

    [HarmonyPatch(typeof(GasPumpConfig), "DoPostConfigureComplete")]
    public class GasPumpConfig_DoPostConfigureComplete
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeGasPump != 0.5f;
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

            storage.capacityKg = CustomizeBuildingsState.Instance.PipeGasPump * 2;

            elementConsumer.consumptionRate = CustomizeBuildingsState.Instance.PipeGasPump;
        }
    }

    [HarmonyPatch(typeof(LiquidMiniPumpConfig), "DoPostConfigureComplete")]
    public class LiquidMiniPumpConfig_DoPostConfigureComplete
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeLiquidPumpMini != 1f;
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

            storage.capacityKg = CustomizeBuildingsState.Instance.PipeLiquidPumpMini * 2;

            elementConsumer.consumptionRate = CustomizeBuildingsState.Instance.PipeLiquidPumpMini;
        }
    }

    [HarmonyPatch(typeof(GasMiniPumpConfig), "DoPostConfigureComplete")]
    public class GasMiniPumpConfig_DoPostConfigureComplete
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.PipeGasPumpMini != 0.05f;
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

            storage.capacityKg = CustomizeBuildingsState.Instance.PipeGasPumpMini * 2;

            elementConsumer.consumptionRate = CustomizeBuildingsState.Instance.PipeGasPumpMini;
        }
    }

    [HarmonyPatch(typeof(SolidConduitDispenser), "ConduitUpdate")]
    public class SolidConduitDispenser_ConduitUpdate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.ConveyorRailPackageSize != 20f;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            float newMass = CustomizeBuildingsState.Instance.ConveyorRailPackageSize;
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
