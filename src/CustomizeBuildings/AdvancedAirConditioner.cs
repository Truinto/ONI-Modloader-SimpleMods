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
    //[HarmonyPatch(typeof(PlantablePlot), "RegisterWithPlant")]
    public class TEST_3
    {
        public static bool Prefix(PlantablePlot __instance)
        {
            if (__instance.gameObject.name == FlowerVaseConfig.ID)
            {
                Debug.Log("TEST_3 prevented FlowerVase link.");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(LiquidConditionerConfig), "ConfigureBuildingTemplate")]
    public class TEST_1
    {
        public static bool Prefix(GameObject go)
        {
            go.AddOrGet<SpaceHeater>().targetTemperature = 274.15f + 50f;

            go.AddOrGet<LoopingSounds>();
            var airConditioner = go.AddOrGet<AdvancedAirConditioner>();
            airConditioner.temperatureDelta = -14f;
            airConditioner.maxEnvironmentDelta = -50f;
            Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
            storage.showInUI = true;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = CustomizeBuildingsState.StateManager.State.PipeLiquidMaxPressure * CustomizeBuildingsState.StateManager.State.PipeThroughputPercent;

            return false;
        }
    }

    [HarmonyPatch(typeof(AirConditionerConfig), "ConfigureBuildingTemplate")]
    public class TEST_2
    {
        public static void Prefix(GameObject go)
        {
            go.AddOrGet<SpaceHeater>().targetTemperature = 274.15f + 50f;
        }
    }


    [HarmonyPatch(typeof(AirConditioner), "UpdateState")]
    public class Patch_AirConditioner
    {
        public static bool Prefix(float dt, AirConditioner __instance)
        {
            var aac = __instance as AdvancedAirConditioner;
            if (aac != null)
            {
                aac.UpdateState(dt);
                return false;
            }

            return true;
        }
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class AdvancedAirConditioner : AirConditioner
    {
#pragma warning disable CS0649
        [MyCmpReq] private ConduitConsumer consumer;
        [MyCmpGet] private OccupyArea occupyArea;
        [MyCmpReq] private BuildingComplete building;
#pragma warning restore CS0649

        public FastSetter _lastEnvTemp = typeof(AirConditioner).CreateSetterProperty("lastEnvTemp");
        public FastSetter _lastGasTemp = typeof(AirConditioner).CreateSetterProperty("lastGasTemp");
        public FastSetter _lowTempLag = typeof(AirConditioner).CreateSetter("lowTempLag");

        protected override void OnSpawn()
        {
            cooledAirOutputCell = building.GetUtilityOutputCell();
            structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
        }

        #region Buttons/Sliders
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
        }

        private void OnRefreshUserMenu(object data)
        {
            Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("", "Button Text", new System.Action(this.ButtonClick), tooltipText: "tooltip button"), 2f);
            Game.Instance.userMenu.AddSlider(this.gameObject, new UserMenu.SliderInfo()
            {
                toolTip = "toolTip",
                toolTipMin = "toolTipMin",
                toolTipMax = "toolTipMax",
                minLimit = 0f,
                maxLimit = 300f,
                onMinChange = new System.Action<MinMaxSlider>(SliderChanged)
            });
        }

        public void SliderChanged(MinMaxSlider slider)
        {

        }

        public void ButtonClick()
        {
            KMod.Manager.Dialog(null, "Title", "Button clicked");
        }
        #endregion


        #region 1-1 copy
        private int cooledAirOutputCell = -1;
        private int cellCount;
        private float envTemp;
        private float lowTempLag;
        private float lastSampleTime = -1f;
        private HandleVector<int>.Handle structureTemperature;
        private static readonly Func<int, object, bool> UpdateStateCbDelegate = (int cell, object data) => AdvancedAirConditioner.UpdateStateCb(cell, data);
        private static bool UpdateStateCb(int cell, object data)
        {
            var airConditioner = (AdvancedAirConditioner)data;
            airConditioner.cellCount++;
            airConditioner.envTemp += Grid.Temperature[cell];
            return true;
        }
        #endregion

        public bool useTarget = true;
        public bool useBuildingMax = true;
        public bool useMassReduction = true;
        public float targetTemperature = 274.15f;
        public float buildingMaxTemperature = 274.15f + 150f;
        public float DPUMax = 1e10f;

        public void UpdateState(float dt)
        {
            bool isSatisfied = this.consumer.IsSatisfied;
            this.envTemp = 0f;
            this.cellCount = 0;
            if (this.occupyArea != null && base.gameObject != null)
            {
                this.occupyArea.TestArea(Grid.PosToCell(base.gameObject), this, AdvancedAirConditioner.UpdateStateCbDelegate);
                this.envTemp /= (float)this.cellCount;
            }
            _lastEnvTemp(this, this.envTemp);   //this.lastEnvTemp = this.envTemp;

            List<GameObject> items = this.storage.items;
            for (int i = 0; i < items.Count; i++)
            {
                PrimaryElement element = items[i].GetComponent<PrimaryElement>();
                if (element.Mass > 0f && (!isLiquidConditioner && element.Element.IsGas || isLiquidConditioner && element.Element.IsLiquid))
                {
                    isSatisfied = true;
                    _lastGasTemp(this, element.Temperature);    //this.lastGasTemp = storage.Temperature;

                    float temperatureNew = element.Temperature + this.temperatureDelta;
                    if (this.useTarget) temperatureNew = this.targetTemperature; //#
                    if (temperatureNew < 1f)
                    {
                        temperatureNew = 1f;
                        this.lowTempLag = Mathf.Min(this.lowTempLag + dt / 5f, 1f);
                    }
                    else
                    {
                        this.lowTempLag = Mathf.Min(this.lowTempLag - dt / 5f, 0f);
                    }
                    _lowTempLag(this, this.lowTempLag);

                    ConduitFlow conduit = this.isLiquidConditioner ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
                    float mass_max = element.Mass;
                    float temperatureCooled = temperatureNew - element.Temperature;

                    if (this.useMassReduction)
                    {
                        // mass cannot be greater than the pipe size minus pipe content
                        //mass_max = Math.Min(mass_max, this.consumer.consumptionRate - conduit.GetContents(this.cooledAirOutputCell).mass);

                        // max of mass that could be moved with the DPUMax
                        float mass_DPU = this.DPUMax / (temperatureCooled * element.Element.specificHeatCapacity);
                        mass_max = Math.Min(mass_max, mass_DPU);
                    }

                    float mass_output = conduit.AddElement(this.cooledAirOutputCell, element.ElementID, mass_max, temperatureNew, element.DiseaseIdx, element.DiseaseCount);
                    element.KeepZeroMassObject = true;
                    float diseasePercent = mass_output / element.Mass;
                    int diseaseCount = (int)((float)element.DiseaseCount * diseasePercent);
                    element.Mass -= mass_output;
                    element.ModifyDiseaseCount(-diseaseCount, "AirConditioner.UpdateState");

                    float DPU_removed = temperatureCooled * element.Element.specificHeatCapacity * mass_output;
                    float display_dt = (this.lastSampleTime > 0f) ? (Time.time - this.lastSampleTime) : 1f;
                    this.lastSampleTime = Time.time;
                    GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, -DPU_removed, STRINGS.BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, display_dt);
                    break;
                }
            }
            if (Time.time - this.lastSampleTime > 2f)
            {
                GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, 0f, STRINGS.BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, Time.time - this.lastSampleTime);
                this.lastSampleTime = Time.time;
            }
            this.operational.SetActive(isSatisfied, false);
            this.Trigger((int)GameHashes.ActiveChanged, this);  //this.UpdateStatus();
        }

    }
}