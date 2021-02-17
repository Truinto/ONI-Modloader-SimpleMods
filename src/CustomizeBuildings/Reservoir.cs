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
            if (CustomizeBuildingsState.StateManager.State.ReservoirNoGround)
            {
                __result.BuildLocationRule = BuildLocationRule.Anywhere;
                __result.ContinuouslyCheckFoundation = false;
            }
        }
    }
    [HarmonyPatch(typeof(LiquidReservoirConfig), "ConfigureBuildingTemplate")]
    internal class LiquidReservoirConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = Math.Min((float)CustomizeBuildingsState.StateManager.State.LiquidReservoirKG, (float)99000f);
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.capacityKG = storage.capacityKg;

            if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
            {
                storage.allowItemRemoval = true;
                storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
                storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
                storage.showInUI = true;
                storage.showDescriptor = true;
                go.AddOrGet<StorageLocker>();
                Prioritizable.AddRef(go);
            }
        }
    }


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
            storage.capacityKg = Math.Min((float)CustomizeBuildingsState.StateManager.State.GasReservoirKG, (float)99000f);
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.capacityKG = storage.capacityKg;

            if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
            {
                storage.allowItemRemoval = true;
                storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
                storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
                storage.showInUI = true;
                storage.showDescriptor = true;
                go.AddOrGet<StorageLocker>();
                Prioritizable.AddRef(go);
            }
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