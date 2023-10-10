using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;
using KSerialization;
using Common;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(LiquidValveConfig), nameof(LiquidValveConfig.ConfigureBuildingTemplate))]
    public class LiquidValveConfig_ConfigureBuildingTemplate2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.ValveEnableTemperatureFilter;
        }

        public static void Prefix(GameObject go)
        {
            go.AddOrGet<ValveBaseTemperature>();
        }
    }

    [HarmonyPatch(typeof(GasValveConfig), nameof(GasValveConfig.ConfigureBuildingTemplate))]
    public class GasValveConfig_ConfigureBuildingTemplate2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.ValveEnableTemperatureFilter;
        }

        public static void Prefix(GameObject go)
        {
            go.AddOrGet<ValveBaseTemperature>();
        }
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class ValveBaseTemperature : ValveBase
    {
        private static LocString textLogic = Helpers.StringsAddShort("Let material through if:", "ValveTemperatureLogic");
        [MyCmpAdd] private SliderTemperatureSideScreen temperatureSlider;

        public override void OnSpawn()
        {
            var building = base.GetComponent<Building>();
            this.inputCell = building.GetUtilityInputCell();
            this.outputCell = building.GetUtilityOutputCell();
            Conduit.GetFlowManager(this.conduitType).AddConduitUpdater(ConduitUpdate2);
            UpdateAnim();
            OnCmpEnable();
            temperatureSlider.TextLogic = textLogic;
        }

        public override void OnCleanUp()
        {
            Game.Instance.accumulators.Remove(this.flowAccumulator);
            Conduit.GetFlowManager(this.conduitType).RemoveConduitUpdater(ConduitUpdate2);
        }

        public void ConduitUpdate2(float dt)
        {
            float mass_moved = 0f;
            var flowManager = Conduit.GetFlowManager(this.conduitType);
            var conduit = flowManager.GetConduit(this.inputCell);
            if (!flowManager.HasConduit(this.inputCell) || !flowManager.HasConduit(this.outputCell))
                goto exit;

            var contents = conduit.GetContents(flowManager);
            if (contents.mass <= 0f)
                goto exit;

            temperatureSlider.CurrentValue = contents.temperature;

            if (temperatureSlider.Switch ? contents.temperature <= temperatureSlider.SetTemperature : contents.temperature > temperatureSlider.SetTemperature)
                goto exit;

            float mass_want = Mathf.Min(contents.mass, this.currentFlow * dt);
            if (mass_want > 0f)
            {
                int disease_count = (int)(mass_want / contents.mass * (float)contents.diseaseCount);
                mass_moved = flowManager.AddElement(this.outputCell, contents.element, mass_want, contents.temperature, contents.diseaseIdx, disease_count);
                Game.Instance.accumulators.Accumulate(this.flowAccumulator, mass_moved);
                if (mass_moved > 0f)
                {
                    flowManager.RemoveElement(this.inputCell, mass_moved);
                }
            }

        exit:
            OnMassTransfer(mass_moved);
            UpdateAnim();
        }
    }


    // TODO: FilterableCustomTagSelection
    // IThresholdSwitch -> for temperature
    // Filterable or FlatTagFilterable -> for material (refactor SkillStationSideScreen.cs)
    // Filterable -> solo select
    // TreeFilterable -> multi select, but storage is forced
    // FlatTagFilterable -> no groups

    /// <summary>
    /// 
    /// </summary>
    [SerializationConfig(MemberSerialization.OptIn)]
    public class FilterableCustomTagSelection
    {
        public static Dictionary<string, Dictionary<Tag, HashSet<Tag>>> Tags = new();

        [SerializeField]
        public string Key;

        [SerializeField]
        public bool MultiSelect;

        [Serialize]
        public HashSet<Tag> SelectedTags = new();

        public Dictionary<Tag, HashSet<Tag>> GetTags()
        {
            if (Tags.TryGetValue(Key, out var tags))
                return tags;
            return new();
        }
    }

    //[HarmonyPatch]
    public static class Patch_FilterableCustomTagSelection
    {
        [HarmonyPatch(typeof(FilterSideScreen), nameof(FilterSideScreen.IsValidForTarget))]
        [HarmonyPostfix]
        public static void Postfix1()
        {

        }

        [HarmonyPatch(typeof(FilterSideScreen), nameof(FilterSideScreen.SetTarget))]
        [HarmonyPostfix]
        public static void Postfix2()
        {

        }
    }
}