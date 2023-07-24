using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using HarmonyLib;
using KSerialization;
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
                go.AddOrGet<Automatable>();
                // FetchChore..ctor handles fetch precondition, which is set in AutomatableSideScreen
                // a second toggle for storage.allowItemRemoval is required
                go.AddComponent<ReservoirToggleSideScreen>();
                Prioritizable.AddRef(go);

                //SetOnlyFetchMarkedItems - Sweep only
            }
        }
    }

    public class ReservoirToggleSideScreen2 : KMonoBehaviour, ICheckboxListGroupControl //, INToggleSideScreenControl, ISidescreenButtonControl
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

    [SerializationConfig(MemberSerialization.OptIn)]
    public class ReservoirToggleSideScreen : KMonoBehaviour, ISidescreenButtonControl
    {
        [Serialize]
        public bool Enabled = false;
        public int SortOrder = 20;

        public ButtonMenuTextOverride Text { get; set; } = new()
        {
            Text = "[x] Allow Duplicant to remove material",
            CancelText = "[ ] Allow Duplicant to remove material",
            ToolTip = "Currently allowed",
            CancelToolTip = "Currently disallowed"
        };

        public string SidescreenButtonText => Enabled ? Text.Text : Text.CancelText;
        public string SidescreenButtonTooltip => Enabled ? Text.ToolTip : Text.CancelToolTip;

        public int ButtonSideScreenSortOrder() => SortOrder;
        public int HorizontalGroupID() => -1;
        public bool SidescreenEnabled() => true;
        public bool SidescreenButtonInteractable() => true;
        public void SetButtonTextOverride(ButtonMenuTextOverride textOverride) => Text = textOverride;

        public void OnSidescreenButtonPressed()
        {
            Enabled = !Enabled;
            UpdateEnabled();
        }

        public void UpdateEnabled()
        {
            // update items; see also GasBottler
            var storage = this.GetComponent<Storage>();
            storage.allowItemRemoval = Enabled;
            foreach (GameObject item in storage.items)
                item?.Trigger((int)GameHashes.OnStorageInteracted, storage);
        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
        }

        [OnDeserialized]
        private void OnDeserialized()
        {
            UpdateEnabled();
        }

        public static readonly EventSystem.IntraObjectHandler<ReservoirToggleSideScreen> OnCopySettingsDelegate = new((ReservoirToggleSideScreen target, object source) =>
        {
            var component = ((GameObject)source).GetComponent<ReservoirToggleSideScreen>();
            if (component != null)
            {
                target.Enabled = component.Enabled;
            }
        });
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
                go.AddOrGet<Automatable>();
                go.AddComponent<ReservoirToggleSideScreen>();
                Prioritizable.AddRef(go);
            }
        }
    }

    public class SweepOnlyNoFetchAtAll : KMonoBehaviour
    {
        public override void OnSpawn()
        {
            base.OnSpawn();
            Storage storage = this.GetComponent<Storage>();
            if (storage != null)
            {
                storage.Subscribe(644822890, new System.Action<object>(this.FetchMarkedChanged));
            }
        }

        public override void OnCleanUp()
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