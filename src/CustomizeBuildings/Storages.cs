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
                setStorage(CustomizeBuildingsState.StateManager.State.DrillConeKG);

            else if (def.PrefabID is ModularLaunchpadPortGasConfig.ID or ModularLaunchpadPortGasUnloaderConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.RocketPortGas);

            else if (def.PrefabID is ModularLaunchpadPortLiquidConfig.ID or ModularLaunchpadPortLiquidUnloaderConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.RocketPortLiquid);

            else if (def.PrefabID is ModularLaunchpadPortSolidConfig.ID or ModularLaunchpadPortSolidUnloaderConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.RocketPortSolid);

            else if (def.PrefabID == StorageLockerConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.LockerKG);

            else if (def.PrefabID == StorageLockerSmartConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.LockerSmartKG);

            else if (def.PrefabID == StorageTileConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.StorageTileKG);

            else if (def.PrefabID == RationBoxConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.RationBoxKG);

            else if (def.PrefabID == RefrigeratorConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.FridgeKG);

            else if (def.PrefabID == CreatureFeederConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.CritterFeederKG);

            else if (def.PrefabID == FishFeederConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.FishFeederKG);

            else if (def.PrefabID == LiquidBottlerConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.BottleFillerKG); //200f

            else if (def.PrefabID == GasBottlerConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.CanisterFillerKG); //25f

            else if (def.PrefabID == BottleEmptierConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.BottleEmptierKG); //200f

            else if (def.PrefabID == BottleEmptierGasConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.CanisterEmptierKG); //200f

            else if (def.PrefabID == BottleEmptierConduitLiquidConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.BottleEmptierConduitKG); //200f

            else if (def.PrefabID == BottleEmptierConduitGasConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.CanisterEmptierConduitKG); //200f

            else if (def.PrefabID == SolidConduitInboxConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.ConveyorLoaderKG);

            else if (def.PrefabID == SolidConduitOutboxConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.ConveyorReceptacleKG);

            else if (def.PrefabID == CO2EngineConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.CO2EngineKG); //100f

            else if (def.PrefabID == SugarEngineConfig.ID)
                setStorage(CustomizeBuildingsState.StateManager.State.SugarEngineKG); //450f

            else if (def.PrefabID is SteamEngineConfig.ID or SteamEngineClusterConfig.ID) //900f, 150f
                setStorage(CustomizeBuildingsState.StateManager.State.SteamEngineKG);

            else if (def.PrefabID == KeroseneEngineClusterSmallConfig.ID) //450f
                setStorage(CustomizeBuildingsState.StateManager.State.SmallPetroleumEngineKG);

            else if (def.PrefabID == HEPEngineConfig.ID) //4000f
                setRads(CustomizeBuildingsState.StateManager.State.HEPEngineStorage);

            else if (def.PrefabID is LiquidFuelTankClusterConfig.ID or LiquidFuelTankClusterConfig.ID) //900f
                setStorage(CustomizeBuildingsState.StateManager.State.LiquidFuelTankKG);

            else if (def.PrefabID == SmallOxidizerTankConfig.ID) //450f
                setStorage(CustomizeBuildingsState.StateManager.State.SmallOxidizerTankKG);

            else if (def.PrefabID is OxidizerTankConfig.ID or OxidizerTankClusterConfig.ID) //2700f, 900f
                setStorage(CustomizeBuildingsState.StateManager.State.LargeSolidOxidizerTankKG);

            else if (def.PrefabID is OxidizerTankLiquidConfig.ID or OxidizerTankLiquidClusterConfig.ID) //2700f, 450f
                setStorage(CustomizeBuildingsState.StateManager.State.LiquidOxidizerTankKG);

            else if (def.PrefabID == SpiceGrinderConfig.ID)
                foreach (var spiceStorage in def.BuildingComplete.GetComponents<Storage>())
                    spiceStorage.SetDefaultStoredItemModifiers(GourmetCookingStationConfig.GourmetCookingStationStoredItemModifiers);

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

                foreach (var delivery in def.BuildingComplete.GetComponents<ManualDeliveryKG>())
                {
                    delivery.capacity = value;
                    delivery.refillMass = value;
                }

                foreach (var consumer in def.BuildingComplete.GetComponents<ConduitConsumer>())
                    consumer.capacityKG = value;

                foreach (var consumer in def.BuildingComplete.GetComponents<SolidConduitConsumer>())
                    consumer.capacityKG = value;

                foreach (var tank in def.BuildingComplete.GetComponents<FuelTank>())
                {
                    tank.targetFillMass = value;
                    tank.physicalFuelCapacity = value;
                }

                foreach (var consumer in def.BuildingComplete.GetComponents<OxidizerTank>())
                    consumer.maxFillMass = value;

                foreach (var consumer in def.BuildingComplete.GetComponents<Bottler>())
                    consumer.userMaxCapacity = value;
            }

            void setRads(float value)
            {
                var pStorage = def.BuildingComplete.GetComponent<HighEnergyParticleStorage>();
                if (pStorage != null)
                    pStorage.capacity = value;
                var radTank = def.BuildingComplete.GetComponent<HEPFuelTank>();
                if (radTank != null)
                    radTank.physicalFuelCapacity = value;
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
