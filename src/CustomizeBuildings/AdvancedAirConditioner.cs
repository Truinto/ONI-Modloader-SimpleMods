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
    //[HarmonyPatch(typeof(PlantablePlot), "RegisterWithPlant")]
    public class TEST_3
    {
        public static bool Prefix(PlantablePlot __instance)
        {
            if (__instance.PrefabID() == FlowerVaseConfig.ID)
            {
                Debug.Log($"TEST_3 prevented link to {__instance.PrefabID()}");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(LiquidConditionerConfig), "ConfigureBuildingTemplate")]
    public class LiquidConditionerConfig_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AirConditionerAbsoluteOutput;
        }

        public static void Postfix(GameObject go)
        {
            go.AddOrGet<AirConditionerSliders>();
            go.AddOrGet<Storage>().capacityKg = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure * 2f;
        }
    }

    [HarmonyPatch(typeof(AirConditionerConfig), "ConfigureBuildingTemplate")]
    public class AirConditionerConfig_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AirConditionerAbsoluteOutput;
        }

        public static void Postfix(GameObject go)
        {
            go.AddOrGet<AirConditionerSliders>();
            go.AddOrGet<Storage>().capacityKg = CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure * 2f;
        }
    }

    [HarmonyPatch(typeof(AirConditioner), "UpdateState")]
    public class AirConditioner_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AirConditionerAbsoluteOutput;
        }

        public static Func<int, object, bool> UpdateStateCbDelegate = (Func<int, object, bool>)AccessTools.Field(typeof(AirConditioner), "UpdateStateCbDelegate").GetValue(null);

        [HarmonyPriority(Priority.Low)]
        public static bool Prefix(float dt, AirConditioner __instance, ref float ___envTemp, ref int ___cellCount, ref float ___lowTempLag, ref int ___cooledAirOutputCell,
            ref float ___lastSampleTime, ref HandleVector<int>.Handle ___structureTemperature)
        {
            var consumer = __instance.GetComponent<ConduitConsumer>();
            var occupyArea = __instance.GetComponent<OccupyArea>();
            var operational = __instance.GetComponent<Operational>();
            var storage = __instance.GetComponent<Storage>();
            var slider = __instance.GetComponent<AirConditionerSliders>();

            bool isSatisfied = consumer.IsSatisfied;
            ___envTemp = 0f;
            ___cellCount = 0;
            if (occupyArea != null && __instance.gameObject != null)
            {
                occupyArea.TestArea(Grid.PosToCell(__instance.gameObject), __instance, UpdateStateCbDelegate);
                ___envTemp /= (float)___cellCount;
            }
            Access.lastEnvTemp(__instance, ___envTemp);
            List<GameObject> items = storage.items;
            for (int i = 0; i < items.Count; i++)
            {
                PrimaryElement element = items[i].GetComponent<PrimaryElement>();
                if (element.Mass > 0f && (__instance.isLiquidConditioner && element.Element.IsLiquid || !__instance.isLiquidConditioner && element.Element.IsGas))
                {
                    isSatisfied = true;
                    Access.lastGasTemp(__instance, element.Temperature);
                    float temperatureNew = element.Temperature + __instance.temperatureDelta;
                    if (slider != null)
                    {
                        slider.CurrentTemperature = element.Temperature;
                        if (slider.SetTemperature >= 1f)
                            temperatureNew = slider.SetTemperature; //# target temperature
                    }
                    if (temperatureNew < 1f)
                    {
                        temperatureNew = 1f;
                        ___lowTempLag = Mathf.Min(___lowTempLag + dt / 5f, 1f);
                    }
                    else
                    {
                        ___lowTempLag = Mathf.Min(___lowTempLag - dt / 5f, 0f);
                    }

                    float temperatureCooled = temperatureNew - element.Temperature;
                    float mass_max = element.Mass;
                    if (slider != null && temperatureCooled != 0f && slider.SetTemperature >= 1f)
                    {
                        float mass_DPU = slider.SetDPU / (Math.Abs(temperatureCooled) * element.Element.specificHeatCapacity);
                        mass_max = Math.Min(element.Mass, mass_DPU);
#if DEBUG
                        if (mass_max < 0f)
                            Debug.Log("mass_max is less than 0!");
#endif
                        mass_max = Math.Max(mass_max, 0f);
                    }

                    ConduitFlow conduit = __instance.isLiquidConditioner ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
                    float mass_output = conduit.AddElement(___cooledAirOutputCell, element.ElementID, mass_max, temperatureNew, element.DiseaseIdx, element.DiseaseCount); //#
                    element.KeepZeroMassObject = true;
                    float diseasePercent = mass_output / element.Mass;
                    int diseaseCount = (int)((float)element.DiseaseCount * diseasePercent);
                    element.Mass -= mass_output;
                    element.ModifyDiseaseCount(-diseaseCount, "AirConditioner.UpdateState");
                    float DPU_removed = temperatureCooled * element.Element.specificHeatCapacity * mass_output;

                    if (slider != null)
                    {
                        slider.CurrentDPU = DPU_removed;
                    }

                    float display_dt = (___lastSampleTime > 0f) ? (Time.time - ___lastSampleTime) : 1f;
                    ___lastSampleTime = Time.time;
                    GameComps.StructureTemperatures.ProduceEnergy(___structureTemperature, Math.Max(4f, -DPU_removed), STRINGS.BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, display_dt);
                    break;
                }
            }

            if (Time.time - ___lastSampleTime > 2f)
            {
                GameComps.StructureTemperatures.ProduceEnergy(___structureTemperature, 0f, STRINGS.BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, Time.time - ___lastSampleTime);
                ___lastSampleTime = Time.time;
            }
            operational.SetActive(isSatisfied, false);
            Access.UpdateStatus(__instance);

            return false;
        }
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class AirConditionerSliders : KMonoBehaviour, IUserControlledCapacity, IThresholdSwitch, ISim200ms    //IActivationRangeTarget
    {
        #region OnSpawn
        private EnergyConsumer energyConsumer;
        private float factorDPU;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            energyConsumer = this.GetComponent<EnergyConsumer>();
            if (CustomizeBuildingsState.StateManager.State.AirConditionerAbsolutePowerFactor <= 0f)
                CustomizeBuildingsState.StateManager.State.AirConditionerAbsolutePowerFactor = 0.001f;
            factorDPU = energyConsumer.BaseWattsNeededWhenActive / (MaxDPU * CustomizeBuildingsState.StateManager.State.AirConditionerAbsolutePowerFactor);
        }
        #endregion

        #region IUserControlledCapacity
        public float CurrentDPU;
        public const float MaxDPU = 10000f;
        [Serialize] public float SetDPU = 100f;

        LocString IUserControlledCapacity.CapacityUnits => "kDPU";
        float IUserControlledCapacity.UserMaxCapacity
        {
            get { return SetDPU; }
            set { SetDPU = value; }
        }
        float IUserControlledCapacity.AmountStored => this.CurrentDPU;
        float IUserControlledCapacity.MinCapacity => 0f;
        float IUserControlledCapacity.MaxCapacity => MaxDPU;
        bool IUserControlledCapacity.WholeValues => true;
        #endregion

        #region IThresholdSwitch
        public float CurrentTemperature;
        [Serialize] public float SetTemperature;
        [Serialize] public bool Switch;

        float IThresholdSwitch.Threshold { get => this.SetTemperature; set => this.SetTemperature = value; }
        bool IThresholdSwitch.ActivateAboveThreshold { get => this.Switch; set => this.Switch = value; }
        float IThresholdSwitch.CurrentValue => this.CurrentTemperature;
        float IThresholdSwitch.RangeMin => 0f;
        float IThresholdSwitch.RangeMax => 1273.15f;
        LocString IThresholdSwitch.Title => STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;
        LocString IThresholdSwitch.ThresholdValueName => STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;
        string IThresholdSwitch.AboveToolTip => "Above";
        string IThresholdSwitch.BelowToolTip => "Below";
        ThresholdScreenLayoutType IThresholdSwitch.LayoutType => ThresholdScreenLayoutType.InputField;
        int IThresholdSwitch.IncrementScale => 1;
        NonLinearSlider.Range[] IThresholdSwitch.GetRanges => NonLinearSlider.GetDefaultRange(((IThresholdSwitch)this).RangeMax);
        float IThresholdSwitch.GetRangeMinInputField() => GameUtil.GetConvertedTemperature(((IThresholdSwitch)this).RangeMin, false);
        float IThresholdSwitch.GetRangeMaxInputField() => GameUtil.GetConvertedTemperature(((IThresholdSwitch)this).RangeMax, false);
        LocString IThresholdSwitch.ThresholdValueUnits() => Helpers.GetTemperatureUnit();
        string IThresholdSwitch.Format(float value, bool units) => GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units, false);
        float IThresholdSwitch.ProcessedSliderValue(float input) => Mathf.Round(input);
        float IThresholdSwitch.ProcessedInputValue(float input) => GameUtil.GetTemperatureConvertedToKelvin(input);
        #endregion

        #region TooHot; Power Consumption
        public void Sim200ms(float dt)
        {
            // works in principle: 1) should average, so it doesn't flicker so much 2) should consider overheat material bonuses
            //bool isTooHot = TooHotFast();
            //this.operational.SetFlag(hotFlag, !isTooHot);
            //if (isTooHot && handle == Guid.Empty)
            //    handle = selectable.AddStatusItem(Db.Get().BuildingStatusItems.CoolingStalledHotEnv);
            //else if (handle != Guid.Empty)
            //    handle = selectable.RemoveStatusItem(handle);

            energyConsumer.BaseWattageRating = Math.Min(energyConsumer.BaseWattsNeededWhenActive, 100f + (Math.Abs(CurrentDPU) * factorDPU));
            //Helpers.Print($" dt={dt}, CurrentDPU={CurrentDPU}, factorDPU={factorDPU}, Rating={energyConsumer.BaseWattageRating}");
        }

        public bool TooHot()
        {
            this.monitorCells.Clear();
            int cell = Grid.PosToCell(this.transform.GetPosition());
            GameUtil.GetNonSolidCells(cell, radius, this.monitorCells);
            int count = 0;
            float summ = 0f;
            for (int i = 0; i < this.monitorCells.Count; i++)
            {
                if (Grid.Mass[this.monitorCells[i]] > minimumCellMass)
                {
                    count++;
                    summ += Grid.Temperature[this.monitorCells[i]];
                }
            }
            return count <= 0 || summ / count >= this.hotTemperature;
        }

        public bool TooHotFast()
        {
            int cell = Grid.PosToCell(this.transform.GetPosition());
            return Grid.Mass[cell] <= 0 || Grid.Temperature[cell] >= this.hotTemperature;
        }

        public float hotTemperature = 273.15f + 60f;
        public float lastMass;

#pragma warning disable
        private Guid handle;
        [MyCmpReq] private KSelectable selectable;
        [MyCmpReq] private Operational operational;
#pragma warning restore
        private static readonly Operational.Flag hotFlag = new Operational.Flag("TooHot", Operational.Flag.Type.Requirement);
        private const int radius = 2;
        private const float minimumCellMass = 0;
        private List<int> monitorCells = new List<int>();
        #endregion
    }


    [HarmonyPatch(typeof(SpaceHeaterConfig), "ConfigureBuildingTemplate")]
    public class SpaceHeater_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SpaceHeaterTargetTemperature;
        }

        public static void Postfix(GameObject go) => go.AddOrGet<SpaceHeaterSlider>();
    }

    [HarmonyPatch(typeof(LiquidHeaterConfig), "ConfigureBuildingTemplate")]
    public class SpaceHeater_Patch2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.SpaceHeaterTargetTemperature;
        }

        public static void Postfix(GameObject go) => go.AddOrGet<SpaceHeaterSlider>();
    }

    public class SpaceHeaterSlider : KMonoBehaviour, IThresholdSwitch, ISim1000ms
    {
        private SpaceHeater spaceHeater;

        #region IThresholdSwitch
        public float CurrentTemperature;
        [Serialize] public float SetTemperature = 273.15f + 80f;
        [Serialize] public bool Switch;

        float IThresholdSwitch.Threshold
        {
            get => this.SetTemperature;
            set
            {
                this.SetTemperature = value;
                this.UpdateThreshold();
            }
        }
        bool IThresholdSwitch.ActivateAboveThreshold { get => this.Switch; set => this.Switch = value; }
        float IThresholdSwitch.CurrentValue => this.CurrentTemperature;
        float IThresholdSwitch.RangeMin => 0f;
        float IThresholdSwitch.RangeMax => 1273.15f;
        LocString IThresholdSwitch.Title => STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;
        LocString IThresholdSwitch.ThresholdValueName => STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;
        string IThresholdSwitch.AboveToolTip => "Above";
        string IThresholdSwitch.BelowToolTip => "Below";
        ThresholdScreenLayoutType IThresholdSwitch.LayoutType => ThresholdScreenLayoutType.InputField;
        int IThresholdSwitch.IncrementScale => 1;
        NonLinearSlider.Range[] IThresholdSwitch.GetRanges => NonLinearSlider.GetDefaultRange(((IThresholdSwitch)this).RangeMax);
        float IThresholdSwitch.GetRangeMinInputField() => GameUtil.GetConvertedTemperature(((IThresholdSwitch)this).RangeMin, false);
        float IThresholdSwitch.GetRangeMaxInputField() => GameUtil.GetConvertedTemperature(((IThresholdSwitch)this).RangeMax, false);
        LocString IThresholdSwitch.ThresholdValueUnits() => Helpers.GetTemperatureUnit();
        string IThresholdSwitch.Format(float value, bool units) => GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units, false);
        float IThresholdSwitch.ProcessedSliderValue(float input) => Mathf.Round(input);
        float IThresholdSwitch.ProcessedInputValue(float input) => GameUtil.GetTemperatureConvertedToKelvin(input);
        #endregion

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.spaceHeater = this.GetComponent<SpaceHeater>();
            this.UpdateThreshold();
        }

        public void UpdateThreshold()
        {
            if (this.spaceHeater != null)
            {
                spaceHeater.targetTemperature = this.SetTemperature;
            }
        }

        public void Sim1000ms(float dt)
        {
            this.CurrentTemperature = Grid.Temperature[Grid.PosToCell(this)];
        }
    }

}