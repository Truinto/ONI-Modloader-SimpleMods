using Harmony;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;
using KSerialization;

namespace CustomizeBuildings
{
    //[HarmonyPatch(typeof(ValveBase), "ConduitUpdate")]
    public class Valve_PressureMode
    {
        public static bool Prefix(float dt, ValveBase __instance, float ___currentFlow, int ___outputCell, int ___inputCell, HandleVector<int>.Handle ___flowAccumulator)
        {
            ConduitFlow flowManager = Conduit.GetFlowManager(__instance.conduitType);
            if (!flowManager.HasConduit(___inputCell) || !flowManager.HasConduit(___outputCell))
            {
                __instance.UpdateAnim();
            }
            else
            {
                ConduitFlow.ConduitContents input_content = flowManager.GetConduit(___inputCell).GetContents(flowManager);
                ConduitFlow.ConduitContents output_content = flowManager.GetConduit(___outputCell).GetContents(flowManager);

                float mass_input = Mathf.Min(input_content.mass, ___currentFlow * dt);
                float mass_output = output_content.mass;

                float mass_limit = Mathf.Max(___currentFlow - mass_output, 0);  // mass on output cannot exceed flow setting
                mass_input = Mathf.Min(mass_input, mass_limit);                 // set new input mass

                if (mass_input > 0f)
                {
                    float disease_percent = mass_input / input_content.mass;
                    int disease_count = (int)(disease_percent * input_content.diseaseCount);
                    float mass_moved = flowManager.AddElement(___outputCell, input_content.element, mass_input, input_content.temperature, input_content.diseaseIdx, disease_count);
                    Game.Instance.accumulators.Accumulate(___flowAccumulator, mass_moved);
                    if (mass_moved > 0f)
                    {
                        flowManager.RemoveElement(___inputCell, mass_moved);
                    }
                }
                __instance.UpdateAnim();
            }

            return false;
        }
    }

    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(typeof(LiquidValveConfig), nameof(LiquidValveConfig.ConfigureBuildingTemplate))]
    public class LiquidValveConfig_ConfigureBuildingTemplate2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeValvePressureButtonShow;
        }

        public static bool Prefix(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            var valveBase = go.AddOrGet<ValvePressure>();
            valveBase.conduitType = ConduitType.Liquid;
            valveBase.maxFlow = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure;
            valveBase.animFlowRanges = new ValveBase.AnimRangeInfo[]
            {
                new ValveBase.AnimRangeInfo(3f, "lo"),
                new ValveBase.AnimRangeInfo(7f, "med"),
                new ValveBase.AnimRangeInfo(10f, "hi")
            };
            go.AddOrGet<Valve>();
            Workable workable = go.AddOrGet<Workable>();
            workable.workTime = 5f;

            return false;
        }
    }

    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(typeof(GasValveConfig), nameof(GasValveConfig.ConfigureBuildingTemplate))]
    public class GasValveConfig_ConfigureBuildingTemplate2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.PipeValvePressureButtonShow;
        }

        public static bool Prefix(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            var valveBase = go.AddOrGet<ValvePressure>();
            valveBase.conduitType = ConduitType.Gas;
            valveBase.maxFlow = CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure;
            valveBase.animFlowRanges = new ValveBase.AnimRangeInfo[]
            {
                new ValveBase.AnimRangeInfo(3f, "lo"),
                new ValveBase.AnimRangeInfo(7f, "med"),
                new ValveBase.AnimRangeInfo(10f, "hi")
            };
            go.AddOrGet<Valve>();
            Workable workable = go.AddOrGet<Workable>();
            workable.workTime = 5f;

            return false;
        }
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    //[AddComponentMenu("KMonoBehaviour/scripts/ValveBase")]
    public class ValvePressure : ValveBase, IActivationRangeTarget
    {
        protected override void OnPrefabInit()
        {
            //base.OnPrefabInit();
            this.Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu); //#
            this.flowAccumulator = Game.Instance.accumulators.Add("Flow", this);
        }

        protected override void OnSpawn()
        {
            //base.OnSpawn();
            Building component = base.GetComponent<Building>();
            this.inputCell = component.GetUtilityInputCell();
            this.outputCell = component.GetUtilityOutputCell();
            Conduit.GetFlowManager(this.conduitType).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default); //#
            this.UpdateAnim();
            this.OnCmpEnable();
        }

        protected override void OnCleanUp()
        {
            this.Unsubscribe((int)GameHashes.RefreshUserMenu);
            Game.Instance.accumulators.Remove(this.flowAccumulator);
            Conduit.GetFlowManager(this.conduitType).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate)); //#
            //base.OnCleanUp();
        }

        public override void UpdateAnim()
        {
            float actual_flow = Game.Instance.accumulators.GetAverageRate(this.flowAccumulator);
            bool flag = actual_flow > 0f;
            if (flag)
            {
                for (int i = 0; i < this.animFlowRanges.Length; i++)
                {
                    bool flag2 = actual_flow <= this.animFlowRanges[i].minFlow;
                    if (flag2)
                    {
                        bool flag3 = this.curFlowIdx != i;
                        if (flag3)
                        {
                            this.curFlowIdx = i;
                            this.controller.Play(this.animFlowRanges[i].animName, (actual_flow <= 0f) ? KAnim.PlayMode.Once : KAnim.PlayMode.Loop, 1f, 0f);
                        }
                        break;
                    }
                }
            }
            else
            {
                this.controller.Play("off", KAnim.PlayMode.Once, 1f, 0f);
            }
        }

        private void ConduitUpdate(float dt)
        {
            ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
            if (!flowManager.HasConduit(this.inputCell) || !flowManager.HasConduit(this.outputCell))
            {
                this.UpdateAnim();
            }
            else
            {
                ConduitFlow.ConduitContents input_content = flowManager.GetConduit(this.inputCell).GetContents(flowManager);
                ConduitFlow.ConduitContents output_content = flowManager.GetConduit(this.outputCell).GetContents(flowManager);

                float mass_input = Mathf.Min(input_content.mass, this.CurrentFlow * dt);
                float mass_output = output_content.mass;

                if (limitPressure)
                {
                    float mass_limit = Mathf.Max(this.CurrentFlow - mass_output, 0);  // mass on output cannot exceed flow setting
                    mass_input = Mathf.Min(mass_input, mass_limit);                   // set new input mass
                }

                if (mass_input > 0f)
                {
                    float disease_percent = mass_input / input_content.mass;
                    int disease_count = (int)(disease_percent * input_content.diseaseCount);
                    float mass_moved = flowManager.AddElement(this.outputCell, input_content.element, mass_input, input_content.temperature, input_content.diseaseIdx, disease_count);
                    Game.Instance.accumulators.Accumulate(this.flowAccumulator, mass_moved);
                    if (mass_moved > 0f)
                    {
                        flowManager.RemoveElement(this.inputCell, mass_moved);
                    }
                }
                this.UpdateAnim();
            }
        }

        private void OnRefreshUserMenu(object data)
        {
            Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("", limitPressure ? "Pressure Limited" : "Pressure Unlimited", new System.Action(this.ButtonPressureMode), tooltipText: "Click to toggle pressure mode."), 10f);
        }

        private void ButtonPressureMode()
        {
            limitPressure = !limitPressure;
        }

        [SerializeField]
        private bool limitPressure;

        private int inputCell;
        private int outputCell;
        private int curFlowIdx = -1;


        public float ActivateValue { get; set; }
        public float DeactivateValue { get; set; }
        public float MinValue => 0f;
        public float MaxValue => 1273.15f;
        public bool UseWholeNumbers => false;
        public string ActivationRangeTitleText => "ActivationRangeTitleText";
        public string ActivateSliderLabelText => "ActivateSliderLabelText";
        public string DeactivateSliderLabelText => "DeactivateSliderLabelText";
        public string ActivateTooltip => "ActivateTooltip";
        public string DeactivateTooltip => "DeactivateTooltip";
    }

}