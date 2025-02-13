using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;
using KSerialization;
using Common;
using Shared;

namespace CustomizeBuildings
{
    public class AdvancedConditionerMod : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id is AirConditionerConfig.ID or LiquidConditionerConfig.ID
                && CustomizeBuildingsState.StateManager.State.AirConditionerAbsoluteOutput;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            def.BuildingComplete.AddOrGet<AirConditionerSliders>().TextLogic = Helpers.StringsGetLocShort("AdvancedConditionerModLogic");
            def.BuildingComplete.AddOrGet<Storage>().capacityKg = 2f *
                (def.PrefabID is AirConditionerConfig.ID ?
                CustomizeBuildingsState.StateManager.State.PipeGasMaxPressure :
                CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure);
        }
    }

    [HarmonyPatch(typeof(AirConditioner), nameof(AirConditioner.UpdateState))]
    [HarmonyPriority(Priority.Low)]
    public class AirConditioner_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AirConditionerAbsoluteOutput;
        }

        public static Func<int, object, bool> UpdateStateCbDelegate = (Func<int, object, bool>)AccessTools.Field(typeof(AirConditioner), "UpdateStateCbDelegate").GetValue(null);

        // old patch
        public static bool Prefix_OFFLINE(float dt, AirConditioner __instance, ref float ___envTemp, ref int ___cellCount, ref float ___lowTempLag, ref int ___cooledAirOutputCell,
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
            __instance.lastEnvTemp = ___envTemp;
            List<GameObject> items = storage.items;
            for (int i = 0; i < items.Count; i++)
            {
                PrimaryElement element = items[i].GetComponent<PrimaryElement>();
                if (element.Mass > 0f && (__instance.isLiquidConditioner && element.Element.IsLiquid || !__instance.isLiquidConditioner && element.Element.IsGas))
                {
                    isSatisfied = true;
                    __instance.lastGasTemp = element.Temperature;
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
                            Helpers.Print("mass_max is less than 0!");
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
            __instance.UpdateStatus();

            return false;
        }

        // copy of original
        private static void UpdateState(float dt, AirConditioner __instance)
        {
            bool isSatisfied = __instance.consumer.IsSatisfied;
            __instance.envTemp = 0f;
            __instance.cellCount = 0;
            if (__instance.occupyArea != null && __instance.gameObject != null)
            {
                __instance.occupyArea.TestArea(Grid.PosToCell(__instance.gameObject), __instance, AirConditioner.UpdateStateCbDelegate);
                __instance.envTemp /= __instance.cellCount;
            }
            __instance.lastEnvTemp = __instance.envTemp;
            List<GameObject> items = __instance.storage.items;
            for (int i = 0; i < items.Count; i++)
            {
                PrimaryElement element = items[i].GetComponent<PrimaryElement>();
                if (element.Mass > 0f && (__instance.isLiquidConditioner && element.Element.IsLiquid || !__instance.isLiquidConditioner && element.Element.IsGas))
                {
                    isSatisfied = true;
                    __instance.lastGasTemp = element.Temperature;
                    float temperatureNew = element.Temperature + __instance.temperatureDelta;
                    if (temperatureNew < 1f)
                    {
                        temperatureNew = 1f;
                        __instance.lowTempLag = Mathf.Min(__instance.lowTempLag + dt / 5f, 1f);
                    }
                    else
                    {
                        __instance.lowTempLag = Mathf.Min(__instance.lowTempLag - dt / 5f, 0f);
                    }
                    float mass_output = (__instance.isLiquidConditioner ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow).AddElement(__instance.cooledAirOutputCell, element.ElementID, element.Mass, temperatureNew, element.DiseaseIdx, element.DiseaseCount);
                    element.KeepZeroMassObject = true;
                    float diseasePercent = mass_output / element.Mass;
                    int diseaseCount = (int)(element.DiseaseCount * diseasePercent);
                    element.Mass -= mass_output;
                    element.ModifyDiseaseCount(-diseaseCount, "AirConditioner.UpdateState");
                    float DPU_removed = (temperatureNew - element.Temperature) * element.Element.specificHeatCapacity * mass_output;
                    float display_dt = (__instance.lastSampleTime > 0f) ? (Time.time - __instance.lastSampleTime) : 1f;
                    __instance.lastSampleTime = Time.time;
                    GameComps.StructureTemperatures.ProduceEnergy(__instance.structureTemperature, 0f - DPU_removed, STRINGS.BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, display_dt);
                    break;
                }
            }

            if (Time.time - __instance.lastSampleTime > 2f)
            {
                GameComps.StructureTemperatures.ProduceEnergy(__instance.structureTemperature, 0f, STRINGS.BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, Time.time - __instance.lastSampleTime);
                __instance.lastSampleTime = Time.time;
            }
            __instance.operational.SetActive(isSatisfied);
            __instance.UpdateStatus();
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(ConduitFlow), nameof(ConduitFlow.AddElement));
            data.Offset(-5).NameLocal("num_temperatureNew");
            data.Offset(5);
            data.ReplaceCall(patchConduit);

            return data;

            static float patchConduit(ConduitFlow instance, int cell_idx, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, [LocalParameter("num_temperatureNew")] ref float temperatureNew, [LocalParameter(indexByType: 0)] PrimaryElement primaryElement, AirConditioner __instance)
            {
                // temperatureNew = temperature; mass_max = mass;

                var slider = __instance.GetComponent<AirConditionerSliders>();
                if (slider == null) // if there are mods (HVAC) that re-use AirConditioner without the slider: do operation as normal
                {
                    return instance.AddElement(cell_idx, element, mass, temperature, disease_idx, disease_count);
                }
                else
                {
                    slider.CurrentTemperature = primaryElement.Temperature;
                    if (slider.SetTemperature >= 1f)
                        temperatureNew = slider.SetTemperature; //# target temperature

                    float temperatureCooled = temperatureNew - primaryElement.Temperature;
                    if (temperatureCooled != 0f && slider.SetTemperature >= 1f)
                    {
                        float mass_DPU = slider.SetDPU / (Math.Abs(temperatureCooled) * primaryElement.Element.specificHeatCapacity);
                        mass = Math.Max(0f, Math.Min(primaryElement.Mass, mass_DPU));
                    }

                    float mass_output = instance.AddElement(cell_idx, element, mass, temperatureNew, disease_idx, disease_count);

                    float DPU_removed = temperatureCooled * primaryElement.Element.specificHeatCapacity * mass_output;
                    slider.CurrentDPU = DPU_removed;

                    return mass_output;
                }
            }
        }
    }

    [HarmonyPatch(typeof(AirConditioner), nameof(AirConditioner.UpdateState))]
    public static class AirConditioner_Patch2
    {
        public static float factor = CustomizeBuildingsState.StateManager.State.AirConditionerHeatEfficiency;

        public static bool Prepare()
        {
            factor = CustomizeBuildingsState.StateManager.State.AirConditionerHeatEfficiency;
            return factor != 1f || CustomizeBuildingsState.StateManager.State.AirConditionerAbsoluteOutput;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var data = new TranspilerTool(instructions, generator, original);

            data.Seek(typeof(AirConditioner), nameof(AirConditioner.structureTemperature));
            data++;
            data.InsertAfter(patch);

            return data;

            static float patch(float __stack, AirConditioner __instance)
            {
                if (__instance.temperatureDelta > 0f) // HVAC patch
                    return __stack;
                return Math.Min(-4f, __stack) * factor; // -4 is negated in code; this ensures the building emits at least 4 units of heat
            }
        }
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class AirConditionerSliders : KMonoBehaviour, IUserControlledCapacity, IThresholdSwitch, ISim200ms    //IActivationRangeTarget
    {
        public LocString TextLogic;

        #region OnSpawn
        private EnergyConsumer energyConsumer;
        private float factorDPU = 1f;

        public override void OnSpawn()
        {
            base.OnSpawn();
            energyConsumer = this.GetComponent<EnergyConsumer>();
            if (CustomizeBuildingsState.StateManager.State.AirConditionerAbsolutePowerFactor <= 0f)
                CustomizeBuildingsState.StateManager.State.AirConditionerAbsolutePowerFactor = 0.0001f;
            factorDPU = energyConsumer.WattsNeededWhenActive / (10000f * CustomizeBuildingsState.StateManager.State.AirConditionerAbsolutePowerFactor);
        }
        #endregion

        #region IUserControlledCapacity
        public float CurrentDPU;
        public const float MaxDPU = 100000f;
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

            if (energyConsumer != null)
                energyConsumer.BaseWattageRating = Math.Min(energyConsumer.WattsNeededWhenActive, 100f + (Math.Abs(CurrentDPU) * factorDPU));
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
        private static readonly Operational.Flag hotFlag = new("TooHot", Operational.Flag.Type.Requirement);
        private const int radius = 2;
        private const float minimumCellMass = 0;
        private List<int> monitorCells = new();
        #endregion
    }

    public class SpaceHeaterSliderMod : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id is SpaceHeaterConfig.ID or LiquidHeaterConfig.ID or "GasRefrigerationUnit" or "LiquidRefrigerationUnit"
                && CustomizeBuildingsState.StateManager.State.SpaceHeaterTargetTemperature;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var slider = def.BuildingComplete.AddOrGet<SpaceHeaterSlider>();
            slider.TextLogic = Helpers.StringsGetLocShort("SpaceHeaterSliderLogic");
            slider.SetTemperature = def.PrefabID is "GasRefrigerationUnit" or "LiquidRefrigerationUnit" ? 16f : 273.15f + 80f;
        }
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class SpaceHeaterSlider : SliderTemperatureSideScreen, ISim1000ms
    {
        [MyCmpGet] private SpaceHeater spaceHeater;
        [MyCmpGet] private MinimumOperatingTemperature minTemp;

        public override void Update()
        {
            if (spaceHeater != null)
                spaceHeater.targetTemperature = SetTemperature;
            if (minTemp != null)
                minTemp.minimumTemperature = SetTemperature;
        }

        public void Sim1000ms(float dt)
        {
            CurrentValue = Grid.Temperature[Grid.PosToCell(this)];
        }
    }

}
