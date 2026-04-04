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
            bool seal_container = CustomizeBuildingsState.Instance.SealInsulateStorages;

            if (def.PrefabID == NoseconeHarvestConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.DrillConeKG);

            else if (def.PrefabID is ModularLaunchpadPortGasConfig.ID or ModularLaunchpadPortGasUnloaderConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.RocketPortGas);

            else if (def.PrefabID is ModularLaunchpadPortLiquidConfig.ID or ModularLaunchpadPortLiquidUnloaderConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.RocketPortLiquid);

            else if (def.PrefabID is ModularLaunchpadPortSolidConfig.ID or ModularLaunchpadPortSolidUnloaderConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.RocketPortSolid);

            else if (def.PrefabID == StorageLockerConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.LockerKG);

            else if (def.PrefabID == StorageLockerSmartConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.LockerSmartKG);

            else if (def.PrefabID == StorageTileConfig.ID)
            {
                setStorage(CustomizeBuildingsState.Instance.StorageTileKG);
                var tileDef = def.BuildingComplete.GetDef<StorageTile.Def>();
                if (tileDef != null)
                    tileDef.MaxCapacity = CustomizeBuildingsState.Instance.StorageTileKG;
            }
            else if (def.PrefabID == RationBoxConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.RationBoxKG);

            else if (def.PrefabID == RefrigeratorConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.FridgeKG);

            else if (def.PrefabID == CreatureFeederConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.CritterFeederKG);

            else if (def.PrefabID == FishFeederConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.FishFeederKG);

            else if (def.PrefabID == LiquidBottlerConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.BottleFillerKG); //200f

            else if (def.PrefabID == GasBottlerConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.CanisterFillerKG); //25f

            else if (def.PrefabID == BottleEmptierConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.BottleEmptierKG); //200f

            else if (def.PrefabID == BottleEmptierGasConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.CanisterEmptierKG); //200f

            else if (def.PrefabID == BottleEmptierConduitLiquidConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.BottleEmptierConduitKG); //200f

            else if (def.PrefabID == BottleEmptierConduitGasConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.CanisterEmptierConduitKG); //200f

            else if (def.PrefabID == SolidConduitInboxConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.ConveyorLoaderKG);

            else if (def.PrefabID == SolidConduitOutboxConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.ConveyorReceptacleKG);

            else if (def.PrefabID == CO2EngineConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.CO2EngineKG); //100f

            else if (def.PrefabID == SugarEngineConfig.ID)
                setStorage(CustomizeBuildingsState.Instance.SugarEngineKG); //450f

            else if (def.PrefabID is SteamEngineConfig.ID or SteamEngineClusterConfig.ID) //900f, 150f
                setStorage(CustomizeBuildingsState.Instance.SteamEngineKG);

            else if (def.PrefabID == KeroseneEngineClusterSmallConfig.ID) //450f
                setStorage(CustomizeBuildingsState.Instance.SmallPetroleumEngineKG);

            else if (def.PrefabID == HEPEngineConfig.ID) //4000f
                setRads(CustomizeBuildingsState.Instance.HEPEngineStorage);

            else if (def.PrefabID is LiquidFuelTankClusterConfig.ID or LiquidFuelTankClusterConfig.ID) //900f
                setStorage(CustomizeBuildingsState.Instance.LiquidFuelTankKG);

            else if (def.PrefabID == SmallOxidizerTankConfig.ID) //450f
                setStorage(CustomizeBuildingsState.Instance.SmallOxidizerTankKG);

            else if (def.PrefabID is OxidizerTankConfig.ID or OxidizerTankClusterConfig.ID) //2700f, 900f
                setStorage(CustomizeBuildingsState.Instance.LargeSolidOxidizerTankKG);

            else if (def.PrefabID is OxidizerTankLiquidConfig.ID or OxidizerTankLiquidClusterConfig.ID) //2700f, 450f
                setStorage(CustomizeBuildingsState.Instance.LiquidOxidizerTankKG);

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
            return CustomizeBuildingsState.Instance.ConveyorLoaderAcceptLiquidsGas;
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
            return CustomizeBuildingsState.Instance.ConveyorLoaderHasSlider;
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
            return CustomizeBuildingsState.Instance.ConveyorReceptacleHasSlider;
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
            if (CustomizeBuildingsState.Instance.AirfilterDropsCanisters)
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
            return CustomizeBuildingsState.Instance.RailgunMaxLaunch != 200f;
        }

        public static bool Prefix(ref float __result)
        {
            __result = CustomizeBuildingsState.Instance.RailgunMaxLaunch;
            return false;
        }
    }

    [HarmonyPatch(typeof(RailGunConfig), nameof(RailGunConfig.DoPostConfigureComplete))]
    public class Railgun_MaxLaunch2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.Instance.RailgunMaxLaunch != 200f;
        }

        public static void Postfix(GameObject go)
        {
            var particleStorage = go.AddOrGet<HighEnergyParticleStorage>();
            particleStorage.capacity = CustomizeBuildingsState.Instance.RailgunMaxLaunch * 1.05f;

            go.AddOrGet<Storage>().capacityKg = Math.Max(1200f, CustomizeBuildingsState.Instance.RailgunMaxLaunch * 2f);
        }
    }
}
