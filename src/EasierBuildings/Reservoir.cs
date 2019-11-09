using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace CustomizeBuildings
{


    [HarmonyPatch(typeof(LiquidReservoirConfig), "CreateBuildingDef")]
    internal class LiquidReservoirConfig_CreateBuildingDef
    {
        private static void Postfix(ref BuildingDef __result)
        {
            if (!CustomizeBuildingsState.StateManager.State.ReservoirNoGround) return;
            __result.BuildLocationRule = BuildLocationRule.Anywhere;
            __result.ContinuouslyCheckFoundation = false;
        }
    }
    [HarmonyPatch(typeof(LiquidReservoirConfig), "ConfigureBuildingTemplate")]
    internal class LiquidReservoirConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = Math.Min((float)CustomizeBuildingsState.StateManager.State.LiquidReservoirKG, (float)100000f);
            //storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.capacityKG = storage.capacityKg;

            if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
            {
                storage.allowItemRemoval = true;
                storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
                storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
                storage.showInUI = true;
                storage.showDescriptor = true;
                storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
                Prioritizable.AddRef(go);
            }
        }
    }
    [HarmonyPatch(typeof(LiquidReservoirConfig), "DoPostConfigureComplete")]
    internal class LiquidReservoirConfig_DoPostConfigureComplete
    {
        private static void Postfix(GameObject go)
        {
            if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
            {
                GeneratedBuildings.RegisterLogicPorts(go, ReservoirPort.OUTPUT_PORT);
                go.AddOrGet<ReservoirPort>();
            }
        }
    }
    //[HarmonyPatch(typeof(LiquidReservoirConfig), "DoPostConfigurePreview")]
    //internal class LiquidReservoirConfig_DoPostConfigurePreview
    //{
    //    private static void Postfix(GameObject go)
    //    {
    //        if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
    //        {
    //            //GeneratedBuildings.RegisterLogicPorts(go, ReservoirPort.OUTPUT_PORT);
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(LiquidReservoirConfig), "DoPostConfigureUnderConstruction")]
    //internal class LiquidReservoirConfig_DoPostConfigureUnderConstruction
    //{
    //    private static void Postfix(GameObject go)
    //    {
    //        if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
    //        {
    //            GeneratedBuildings.RegisterLogicPorts(go, ReservoirPort.OUTPUT_PORT);
    //        }
    //    }
    //}



    [HarmonyPatch(typeof(GasReservoirConfig), "CreateBuildingDef")]
    internal class GasReservoirConfig_CreateBuildingDef
    {
        private static void Postfix(ref BuildingDef __result)
        {
            if (!CustomizeBuildingsState.StateManager.State.ReservoirNoGround) return;
            __result.BuildLocationRule = BuildLocationRule.Anywhere;
            __result.ContinuouslyCheckFoundation = false;
            
        }
    }
    [HarmonyPatch(typeof(GasReservoirConfig), "ConfigureBuildingTemplate")]
    internal class GasReservoirConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = Math.Min((float)CustomizeBuildingsState.StateManager.State.GasReservoirKG, (float)100000f);
            //storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.capacityKG = storage.capacityKg;

            if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
            {
                storage.allowItemRemoval = true;
                storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
                storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
                storage.showInUI = true;
                storage.showDescriptor = true;
                storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
                go.AddOrGet<StorageLocker>();
                Prioritizable.AddRef(go);
            }
        }
    }
    [HarmonyPatch(typeof(GasReservoirConfig), "DoPostConfigureComplete")]
    internal class GasReservoirConfig_DoPostConfigureComplete
    {
        private static void Postfix(GameObject go)
        {
            if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
            {
                GeneratedBuildings.RegisterLogicPorts(go, ReservoirPort.OUTPUT_PORT);
                go.AddOrGet<ReservoirPort>();
            }
        }
    }
    //[HarmonyPatch(typeof(GasReservoirConfig), "DoPostConfigurePreview")]
    //internal class GasReservoirConfig_DoPostConfigurePreview
    //{
    //    private static void Postfix(GameObject go)
    //    {
    //        if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
    //        {
    //            //GeneratedBuildings.RegisterLogicPorts(go, ReservoirPort.OUTPUT_PORT);
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(GasReservoirConfig), "DoPostConfigureUnderConstruction")]
    //internal class GasReservoirConfig_DoPostConfigureUnderConstruction
    //{
    //    private static void Postfix(GameObject go)
    //    {
    //        if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
    //        {
    //            GeneratedBuildings.RegisterLogicPorts(go, ReservoirPort.OUTPUT_PORT);
    //        }
    //    }
    //}
    

    public class ReservoirPort : StorageLocker, ISim1000ms
    {
        public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1),
            STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT_ACTIVE,
            STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT_INACTIVE, false, false);
        [MyCmpGet]
        private LogicPorts port;

        public void Sim1000ms(float dt)
        {
            UpdateLogic();
        }

        private void UpdateLogic()
        {
            float percent = this.AmountStored / this.UserMaxCapacity;
            this.port.SendSignal(FilteredStorage.FULL_PORT_ID, percent >= 1 ? 1 : 0);
        }
    }

    public class SweepOnlyNoFetchAtAll : KMonoBehaviour
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            Storage storage = this.GetComponent<Storage>();
            if (storage != null)
            {
                storage.Subscribe(644822890, new System.Action<object>(this.FetchMarkedChanged));
            }
        }

        protected override void OnCleanUp()
        {
            Storage storage = this.GetComponent<Storage>();
            if (storage != null)
            {
                storage.Unsubscribe(644822890, new System.Action<object>(this.FetchMarkedChanged));
            }
        }

        private void FetchMarkedChanged(object data)
        {
            Storage storage = this.GetComponent<Storage>();
            if (storage != null)
            {
                if (storage.GetOnlyFetchMarkedItems())
                {
                    storage.allowItemRemoval = false;
                    storage.fetchCategory = Storage.FetchCategory.Building;
                }
                else
                {
                    storage.allowItemRemoval = true;
                    storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
                }
            }
        }

    }
}