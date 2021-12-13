using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(Assets), nameof(Assets.AddBuildingDef))]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class Storages
    {
        public static bool Prepare()
        {
            return true; // todo create storage global flag
        }

        public static void Prefix(BuildingDef def)
        {
            bool seal_container = CustomizeBuildingsState.StateManager.State.SealInsulateStorages;

            if (def.PrefabID == NoseconeHarvestConfig.ID)
            {
                var storage = def.BuildingComplete.GetComponent<Storage>();
                storage.capacityKg = CustomizeBuildingsState.StateManager.State.DrillConeKG;
                var manualDeliveryKG = def.BuildingComplete.GetComponent<ManualDeliveryKG>();
                //manualDeliveryKG.minimumMass = storage.capacityKg;
                manualDeliveryKG.capacity = storage.capacityKg;
                manualDeliveryKG.refillMass = storage.capacityKg;
            }

            else if (def.PrefabID == ModularLaunchpadPortGasConfig.ID || def.PrefabID == ModularLaunchpadPortGasUnloaderConfig.ID)
            {
                ConduitConsumer conduitConsumer = def.BuildingComplete.GetComponent<ConduitConsumer>();
                if (conduitConsumer != null)
                    conduitConsumer.capacityKG = CustomizeBuildingsState.StateManager.State.RocketPortGas;
                foreach (var storage in def.BuildingComplete.GetComponents<Storage>())
                    storage.capacityKg = CustomizeBuildingsState.StateManager.State.RocketPortGas;
            }

            else if (def.PrefabID == ModularLaunchpadPortLiquidConfig.ID || def.PrefabID == ModularLaunchpadPortLiquidUnloaderConfig.ID)
            {
                ConduitConsumer conduitConsumer = def.BuildingComplete.GetComponent<ConduitConsumer>();
                if (conduitConsumer != null)
                    conduitConsumer.capacityKG = CustomizeBuildingsState.StateManager.State.RocketPortLiquid;
                foreach (var storage in def.BuildingComplete.GetComponents<Storage>())
                    storage.capacityKg = CustomizeBuildingsState.StateManager.State.RocketPortLiquid;
            }

            else if (def.PrefabID == ModularLaunchpadPortSolidConfig.ID || def.PrefabID == ModularLaunchpadPortSolidUnloaderConfig.ID)
            {
                var solidConduitConsumer = def.BuildingComplete.GetComponent<SolidConduitConsumer>();
                if (solidConduitConsumer != null)
                    solidConduitConsumer.capacityKG = CustomizeBuildingsState.StateManager.State.RocketPortSolid;
                foreach (var storage in def.BuildingComplete.GetComponents<Storage>())
                    storage.capacityKg = CustomizeBuildingsState.StateManager.State.RocketPortSolid;
            }

            else if (def.PrefabID == StorageLockerConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.LockerKG);

            else if (def.PrefabID == StorageLockerSmartConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.LockerSmartKG);

            else if (def.PrefabID == RationBoxConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.RationBoxKG);

            else if (def.PrefabID == RefrigeratorConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.FridgeKG);

            else if (def.PrefabID == CreatureFeederConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.CritterFeederKG);

            else if (def.PrefabID == FishFeederConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.FishFeederKG);

            else if (def.PrefabID == GasBottlerConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.CanisterFillerKG);

            else if (def.PrefabID == SolidConduitInboxConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.ConveyorLoaderKG);

            else if (def.PrefabID == SolidConduitOutboxConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.ConveyorReceptacleKG);

            void setStorage(float value)
            {
                if ((def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid) && value > 100000f)
                    value = 100000f;

                foreach (var storage in def.BuildingComplete.GetComponents<Storage>())
                {
                    storage.capacityKg = value;
                    if (seal_container)
                        storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
                }

                foreach (var consumer in def.BuildingComplete.GetComponents<ConduitConsumer>())
                    consumer.capacityKG = value;
            }
        }
    }

    [HarmonyPatch(typeof(SolidConduitInboxConfig), "DoPostConfigureComplete")]
    public class SolidConduitInbox_AcceptAny
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.ConveyorLoaderAcceptLiquidsGas;
        }

        public static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.storageFilters.AddRange(TUNING.STORAGEFILTERS.LIQUIDS);
            storage.storageFilters.AddRange(TUNING.STORAGEFILTERS.GASES);

            //Debug.Log($"Debug: version={UnityEngine.Application.version} unityVersion={UnityEngine.Application.unityVersion}");
        }
    }

    [HarmonyPatch(typeof(SolidConduitInboxConfig), "DoPostConfigureComplete")]
    public class SolidConduitInboxConfig_DoPostConfigureComplete
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.ConveyorLoaderHasSlider;
        }

        public static void Postfix(GameObject go)
        {
            go.AddOrGet<UserControlledConduitInbox>();
        }
    }

    [HarmonyPatch(typeof(SolidConduitOutboxConfig), "ConfigureBuildingTemplate")]
    public class SolidConduitOutboxConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.ConveyorReceptacleHasSlider;
        }

        public static void Postfix(GameObject go)
        {
            go.AddOrGet<UserControlledStorage>();
        }
    }

    [HarmonyPatch(typeof(Storage), "OnQueueDestroyObject")]
    public class Storage_OnQueueDestroyObject
    {
        public static void Prefix(Storage __instance)
        {
            if (CustomizeBuildingsState.StateManager.State.AirfilterDropsCanisters)
            {
                __instance.DropAll(false, false, new Vector3(), true);
            }
        }
    }

    [HarmonyPatch(typeof(RailGun), nameof(RailGun.MaxLaunchMass), MethodType.Getter)]
    public class Railgun_MaxLaunch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RailgunMaxLaunch != 200f;
        }

        public static bool Prefix(ref float __result)
        {
            __result = CustomizeBuildingsState.StateManager.State.RailgunMaxLaunch;
            return false;
        }
    }

    [HarmonyPatch(typeof(RailGunConfig), nameof(RailGunConfig.DoPostConfigureComplete))]
    public class Railgun_MaxLaunch2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.RailgunMaxLaunch != 200f;
        }

        public static void Postfix(GameObject go)
        {
            var particleStorage = go.AddOrGet<HighEnergyParticleStorage>();
            particleStorage.capacity = CustomizeBuildingsState.StateManager.State.RailgunMaxLaunch * 1.05f;

            go.AddOrGet<Storage>().capacityKg = Math.Max(1200f, CustomizeBuildingsState.StateManager.State.RailgunMaxLaunch * 2f);
        }
    }
}