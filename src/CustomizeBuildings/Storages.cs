using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace CustomizeBuildings
{

    [HarmonyPatch(typeof(StorageLockerConfig), "ConfigureBuildingTemplate")]
    internal class StorageLockerConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.LockerKG;
        }
    }

    [HarmonyPatch(typeof(StorageLockerSmartConfig), "DoPostConfigureComplete")]
    internal class StorageLockerSmartConfig_DoPostConfigureComplete
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.LockerSmartKG;
        }
    }

    [HarmonyPatch(typeof(RationBoxConfig), "ConfigureBuildingTemplate")]
    internal class RationBoxConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.RationBoxKG;
        }
    }

    [HarmonyPatch(typeof(RefrigeratorConfig), "DoPostConfigureComplete")]
    internal class RefrigeratorConfig_DoPostConfigureComplete
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.FridgeKG;
        }
    }

    [HarmonyPatch(typeof(CreatureFeederConfig), "ConfigureBuildingTemplate")]
    internal class CreatureFeederConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.CritterFeederKG;
        }
    }

    [HarmonyPatch(typeof(FishFeederConfig), "ConfigureBuildingTemplate")]
    internal class FishFeederConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.FishFeederKG;
        }
    }

    [HarmonyPatch(typeof(GasBottlerConfig), "ConfigureBuildingTemplate")]
    internal class GasBottlerConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            GasBottler gasBottler = go.AddOrGet<GasBottler>();
            Storage storage = gasBottler.storage;
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.CanisterFillerKG;
        }
    }

    [HarmonyPatch(typeof(SolidConduitInboxConfig), "DoPostConfigureComplete")]
    internal class SolidConduitInboxConfig_DoPostConfigureComplete
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.ConveyorLoaderKG;
            if (CustomizeBuildingsState.StateManager.State.AutoSweeperSlider)
                go.AddOrGet<UserControlledConduitInbox>();
        }
    }

    [HarmonyPatch(typeof(SolidConduitOutboxConfig), "ConfigureBuildingTemplate")]
    internal class SolidConduitOutboxConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)CustomizeBuildingsState.StateManager.State.ConveyorReceptacleKG;
        }
    }

    [HarmonyPatch(typeof(Storage), "OnQueueDestroyObject")]
    internal class Storage_OnQueueDestroyObject
    {
        private static void Prefix(Storage __instance)
        {
            if (CustomizeBuildingsState.StateManager.State.AirfilterDropsCanisters)
            {
                __instance.DropAll(false, false, new Vector3(), true);
            }
        }
    }
}