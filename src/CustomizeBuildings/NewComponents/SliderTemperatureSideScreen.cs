﻿using KSerialization;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace CustomizeBuildings
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class SliderTemperatureSideScreen : KMonoBehaviour, IThresholdSwitch
    {
        [Serialize] public float SetTemperature = 273.15f + 80f;
        [Serialize] public bool Switch;

        public LocString TextLogic; //Send <b><style="logic_on">Green Signal</style></b> if:

        public float Threshold { get => this.SetTemperature; set { this.SetTemperature = value; Update(); } }
        public bool ActivateAboveThreshold { get => this.Switch; set => this.Switch = value; }
        public float CurrentValue { get; set; }
        public virtual float RangeMin => 0f;
        public virtual float RangeMax => 2273.15f;
        public LocString Title => STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;
        public LocString ThresholdValueName => STRINGS.UI.NEWBUILDCATEGORIES.TEMPERATURE.NAME;
        public string AboveToolTip => STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.ABOVE_BUTTON;
        public string BelowToolTip => STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.BELOW_BUTTON;
        public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.InputField;
        public int IncrementScale => 1;
        public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(this.RangeMax);
        public string Format(float value, bool units) => GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units, false);
        public float GetRangeMaxInputField() => GameUtil.GetConvertedTemperature(this.RangeMax, false);
        public float GetRangeMinInputField() => GameUtil.GetConvertedTemperature(this.RangeMin, false);
        public float ProcessedInputValue(float input) => GameUtil.GetTemperatureConvertedToKelvin(input);
        public float ProcessedSliderValue(float input) => Mathf.Round(input);
        public LocString ThresholdValueUnits() => Helpers.GetTemperatureUnit();

        public override void OnSpawn()
        {
            base.OnSpawn();
            Update();
        }

        public virtual void Update()
        {
        }

        //[OnDeserialized]
        //public virtual void OnDeserialized()
        //{
        //    Update();
        //}
    }

    /// <summary>
    /// Overwriting strings of hidden text fields.
    /// </summary>
    [HarmonyPatch(typeof(ThresholdSwitchSideScreen), nameof(ThresholdSwitchSideScreen.UpdateLabels))]
    public static class ThresholdSwitchSideScreen_FixTitle
    {
        public static void Postfix(ThresholdSwitchSideScreen __instance)
        {
            try
            {
                var locTexts = __instance.transform.GetComponentsInChildren<LocText>();
                var loc1 = default(LocString);

                var slider = __instance.target?.GetComponent<SliderTemperatureSideScreen>();
                if (slider != null)                
                    loc1 = slider.TextLogic;

                var slider2 = __instance.target?.GetComponent<AirConditionerSliders>();
                if (slider2 != null)
                    loc1 = slider2.TextLogic;

                locTexts[1].SetText(loc1 ?? STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.ACTIVATE_IF);
            }
            catch (Exception) { }
        }
    }
}
