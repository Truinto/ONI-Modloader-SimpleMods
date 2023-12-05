using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using HarmonyLib;
using KSerialization;
using UnityEngine;
using Common;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(LiquidReservoirConfig), nameof(LiquidReservoirConfig.CreateBuildingDef))]
    public class LiquidReservoirConfig_CreateBuildingDef
    {
        public static void Postfix(ref BuildingDef __result)
        {
            if (CustomizeBuildingsState.StateManager.State.ReservoirNoGround)
            {
                __result.BuildLocationRule = BuildLocationRule.Anywhere;
                __result.ContinuouslyCheckFoundation = false;
            }
        }
    }
    [HarmonyPatch(typeof(LiquidReservoirConfig), nameof(LiquidReservoirConfig.ConfigureBuildingTemplate))]
    public class LiquidReservoirConfig_ConfigureBuildingTemplate
    {
        public static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = Math.Min((float)CustomizeBuildingsState.StateManager.State.LiquidReservoirKG, (float)99000f);
            if (CustomizeBuildingsState.StateManager.State.SealInsulateStorages)
                storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.capacityKG = storage.capacityKg;

            if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
            {
                storage.allowItemRemoval = true;
                storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
                storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
                storage.onlyTransferFromLowerPriority = true;
                storage.showInUI = true;
                storage.showDescriptor = true;
                go.AddOrGet<StorageLocker>();
                //go.AddOrGet<Automatable>();
                go.AddOrGet<ReservoirToggleSideScreen>();
                go.AddOrGet<ReservoirToggleSideScreen_Store>();
                Prioritizable.AddRef(go);

                UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
                UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireOutputs>());
            }
        }
    }

    [HarmonyPatch(typeof(GasReservoirConfig), nameof(GasReservoirConfig.CreateBuildingDef))]
    public class GasReservoirConfig_CreateBuildingDef
    {
        public static void Postfix(ref BuildingDef __result)
        {
            if (!CustomizeBuildingsState.StateManager.State.ReservoirNoGround) return;
            __result.BuildLocationRule = BuildLocationRule.Anywhere;
            __result.ContinuouslyCheckFoundation = false;

        }
    }
    [HarmonyPatch(typeof(GasReservoirConfig), nameof(GasReservoirConfig.ConfigureBuildingTemplate))]
    public class GasReservoirConfig_ConfigureBuildingTemplate
    {
        public static void Postfix(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = Math.Min((float)CustomizeBuildingsState.StateManager.State.GasReservoirKG, (float)99000f);
            if (CustomizeBuildingsState.StateManager.State.SealInsulateStorages)
                storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.capacityKG = storage.capacityKg;

            if (CustomizeBuildingsState.StateManager.State.ReservoirManualDelivery)
            {
                storage.allowItemRemoval = true;
                storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
                storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
                storage.onlyTransferFromLowerPriority = true;
                storage.showInUI = true;
                storage.showDescriptor = true;
                go.AddOrGet<StorageLocker>();
                //go.AddOrGet<Automatable>();
                go.AddOrGet<ReservoirToggleSideScreen>();
                go.AddOrGet<ReservoirToggleSideScreen_Store>();
                Prioritizable.AddRef(go);
                UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
                UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireOutputs>());
            }
        }
    }

    [HarmonyPatch(typeof(FetchManager), nameof(FetchManager.IsFetchablePickup))]
    public static class Reservoir_FetchManager_Patch
    {
        // note: fetches are organized by destination; source is located in a storage if "pickup.storage != null"
        // fetches are announced by FilteredStorage.OnFilterChanged
        public static void Postfix(Pickupable pickup, FetchChore chore, Storage destination, ref bool __result)
        {
            if (!__result)
                return;
            __result = destination.GetComponent<ReservoirToggleSideScreen_Store>()?.Enabled != false;
        }
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class ReservoirToggleSideScreen_Store : ButtonToggleSideScreen
    {
        public static ButtonMenuTextOverride Text = new()
        {
            Text = Helpers.StringsAddShort("[x] Allow Duplicant to store material", "ReservoirToggleSideScreen_Store1"),
            CancelText = Helpers.StringsAddShort("[ ] Allow Duplicant to store material", "ReservoirToggleSideScreen_Store2"),
            ToolTip = Helpers.StringsAddShort("Currently allowed", "ButtonToggleToolTip"),
            CancelToolTip = Helpers.StringsAddShort("Currently disallowed", "ButtonToggleCancelToolTip")
        };

        public override ButtonMenuTextOverride GetButtonTextOverride() => Text;

        public override void Update()
        {
        }
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class ReservoirToggleSideScreen : ButtonToggleSideScreen
    {
        [MyCmpGet] private Storage storage;

        public static ButtonMenuTextOverride Text = new()
        {
            Text = Helpers.StringsAddShort("[x] Allow Duplicant to remove material", "ReservoirToggleSideScreen_Remove1"),
            CancelText = Helpers.StringsAddShort("[ ] Allow Duplicant to remove material", "ReservoirToggleSideScreen_Remove2"),
            ToolTip = Helpers.StringsAddShort("Currently allowed", "ButtonToggleToolTip"),
            CancelToolTip = Helpers.StringsAddShort("Currently disallowed", "ButtonToggleCancelToolTip")
        };

        public override ButtonMenuTextOverride GetButtonTextOverride() => Text;

        public override void Update()
        {
            // update items; see also GasBottler
            if (storage != null)
            {
                storage.allowItemRemoval = this.Enabled;
                foreach (GameObject item in storage.items)
                    item?.Trigger((int)GameHashes.OnStorageInteracted, storage);
            }
        }
    }

    public class ReservoirToggleSideScreenAlt : KMonoBehaviour, ICheckboxListGroupControl //, INToggleSideScreenControl, ISidescreenButtonControl
    {
        public string Title => "Duplicant Access";

        public string Description => "Control how dupes interact with this reservoir.";

        public int CheckboxSideScreenSortOrder() => 0;

        public bool SidescreenEnabled() => true;

        public ICheckboxListGroupControl.ListGroup[] GetData()
        {
            return new ICheckboxListGroupControl.ListGroup[] {
                new("Allow removal", new ICheckboxListGroupControl.CheckboxItem[]
                {
                    new() { isOn = true, text = "text", overrideLinkActions = s => true }
                })
            };
        }
    }

}