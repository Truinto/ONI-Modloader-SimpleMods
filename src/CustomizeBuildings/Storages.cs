using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace CustomizeBuildings
{
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

    [HarmonyPatch(typeof(StorageLockerConfig), "ConfigureBuildingTemplate")]
    public class StorageLockerConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.LockerKG;
        }
    }

    [HarmonyPatch(typeof(StorageLockerSmartConfig), "DoPostConfigureComplete")]
    public class StorageLockerSmartConfig_DoPostConfigureComplete
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.LockerSmartKG;
        }
    }

    [HarmonyPatch(typeof(RationBoxConfig), "ConfigureBuildingTemplate")]
    public class RationBoxConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.RationBoxKG;
        }
    }

    [HarmonyPatch(typeof(RefrigeratorConfig), "DoPostConfigureComplete")]
    public class RefrigeratorConfig_DoPostConfigureComplete
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.FridgeKG;
        }
    }

    [HarmonyPatch(typeof(CreatureFeederConfig), "ConfigureBuildingTemplate")]
    public class CreatureFeederConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.CritterFeederKG;
        }
    }

    [HarmonyPatch(typeof(FishFeederConfig), "ConfigureBuildingTemplate")]
    public class FishFeederConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.FishFeederKG;
        }
    }

    [HarmonyPatch(typeof(GasBottlerConfig), "ConfigureBuildingTemplate")]
    public class GasBottlerConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.CanisterFillerKG != 10f;
        }

        private static void Postfix(GameObject go)
        {
            var conduit = go.AddOrGet<ConduitConsumer>();
            conduit.storage.capacityKg = CustomizeBuildingsState.StateManager.State.CanisterFillerKG;
            conduit.capacityKG = CustomizeBuildingsState.StateManager.State.CanisterFillerKG;
        }
    }

    [HarmonyPatch(typeof(SolidConduitInboxConfig), "DoPostConfigureComplete")]
    public class SolidConduitInboxConfig_DoPostConfigureComplete
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.ConveyorLoaderKG;
            if (CustomizeBuildingsState.StateManager.State.ConveyorLoaderHasSlider)
                go.AddOrGet<UserControlledConduitInbox>();
        }
    }

    [HarmonyPatch(typeof(SolidConduitOutboxConfig), "ConfigureBuildingTemplate")]
    public class SolidConduitOutboxConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.ConveyorReceptacleKG;
            if (CustomizeBuildingsState.StateManager.State.ConveyorReceptacleHasSlider)
                go.AddOrGet<UserControlledStorage>();
        }
    }

    [HarmonyPatch(typeof(Storage), "OnQueueDestroyObject")]
    public class Storage_OnQueueDestroyObject
    {
        private static void Prefix(Storage __instance)
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

            go.AddOrGet<Storage>().capacityKg = Math.Min(1200f, CustomizeBuildingsState.StateManager.State.RailgunMaxLaunch * 2f);
        }
    }
}