using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;

namespace CustomizeBuildings
{
    //[HarmonyPatch(typeof(IceCooledFan.States), "InitializeStates")]
    //class IceCooledFanM
    //{
    //    private bool Prepare()
    //    {
    //        return true;
    //    }
    //    private bool Prefix(IceCooledFan.States __instance, Operational ___operational, out StateMachine.BaseState default_state)
    //    {
    //        if (operational == null)
    //        {
    //            operational = ___operational;
    //        }
    //        default_state = (StateMachine.BaseState)this.off;
    //        this.off
    //            .EventTransition(GameHashes.OperationalChanged, this.on, (smi => this.operational.IsOperational)); // (GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State)
    //        this.on
    //            .EventTransition(GameHashes.OperationalChanged, this.off, (smi => !this.operational.IsOperational)) // (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.Transition.ConditionCallback)
    //            .DefaultState(this.on.waiting);
    //        this.on.waiting
    //            .EventTransition(GameHashes.OnStorageChange, this.on.working_pre, (smi => smi.EnvironmentNeedsCooling() && smi.master.HasMaterial() && smi.EnvironmentHighEnoughPressure())); // (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.Transition.ConditionCallback)
    //        this.on.working_pre
    //            .PlayAnim("working_pre")
    //            .OnAnimQueueComplete(this.on.working);
    //        this.on.working
    //            .Enter((smi => this.operational.SetActive(true, false))) // (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State.Callback)
    //            .QueueAnim("working_loop", true, null)
    //            .EventTransition(GameHashes.OnStorageChange, this.on.working_pst, (smi => !(smi.EnvironmentNeedsCooling() && smi.master.HasMaterial() && smi.EnvironmentHighEnoughPressure()))) // (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.Transition.ConditionCallback)
    //            .Update("IceCooledFanCooling", ((smi, dt) => this.DoCooling(dt, __instance)), UpdateRate.SIM_200ms, false) // (System.Action<IceCooledFan.StatesInstance, float>)
    //            .Exit((smi => this.operational.SetActive(false, false))); // (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State.Callback)
    //        this.on.working_pst
    //            .PlayAnim("working_pst")
    //            .OnAnimQueueComplete(this.on.waiting);
    //        return false;
    //    }   //smi.master.operational --> this.operational
    //    public Operational operational = null;
    //    public void DoCooling(float dt, object instance)
    //    {
    //        AccessTools.Method(typeof(IceCooledFan), "DoCooling").Invoke(instance, new object[1] { dt });
    //    }
    //    public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State off;
    //    public IceCooledFanM.OnStates on;
    //    public class OnStates : GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State
    //    {
    //        public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State waiting;
    //        public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State working_pre;
    //        public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State working;
    //        public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State working_pst;
    //    }
    //}

    //// possible solution2: adapt implementation from pump, which seems generic
    ////go.AddOrGet<LogicOperationalController>(); //?
    ////go.AddOrGet<EnergyConsumer>(); //?
    ////-- in config
    //typeof(IceCooledFanConfig), "ConfigureBuildingTemplate"
    //private static bool Prefix(GameObject go, Tag prefab_tag)
    //{
    //    float capacity = 500f;
    //    Storage storage1 = go.AddComponent<Storage>();
    //    storage1.capacityKg = capacity;
    //    storage1.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    //    Storage storage2 = go.AddComponent<Storage>();
    //    storage2.capacityKg = capacity;
    //    storage2.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    //    go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = 273.15f;
    //    go.AddOrGet<LoopingSounds>();
    //    Prioritizable.AddRef(go);
    //    IceCooledFanM iceFan = go.AddOrGet<IceCooledFanM>();
    //    iceFan.iceStorage = storage1;
    //    iceFan.liquidStorage = storage2;
    //    ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
    //    manualDeliveryKg.SetStorage(storage1);
    //    manualDeliveryKg.requestedItemTag = GameTags.IceOre;
    //    manualDeliveryKg.capacity = capacity;
    //    manualDeliveryKg.refillMass = capacity * 0.2f;
    //    manualDeliveryKg.minimumMass = 10f;
    //    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    //    return false;
    //}
    //typeof (IceCooledFanConfig), "DoPostConfigureComplete"
    //private static void Postfix(GameObject go)
    //{
    //    go.AddOrGetDef<OperationalController.Def>();    // has game state machine
    //}
    ////---- 
    //public class IceCooledFanM : KMonoBehaviour, ISim1000ms
    //{
    //    //public static readonly Operational.Flag PumpableFlag = new Operational.Flag("cooling", Operational.Flag.Type.Requirement);
    //    [MyCmpGet]
    //    private Operational operational;
    //    [SerializeField]
    //    public Storage iceStorage;
    //    [SerializeField]
    //    public Storage liquidStorage;
    //    public float coolingRate = 32f;
    //    public float targetTemperature = 278.15f;
    //    public float minCooledTemperature = 278.15f;
    //    public float minEnvironmentMass = 0.25f;
    //    public Vector2I minCoolingRange = new Vector2I(-2, 0);
    //    public Vector2I maxCoolingRange = new Vector2I(2, 4);
    //    public Tag consumptionTag = GameTags.IceOre;
    //    protected override void OnSpawn()
    //    {
    //        base.OnSpawn();
    //        this.GetComponent<ManualDeliveryKG>().SetStorage(this.iceStorage);
    //    }
    //    protected override void OnCleanUp()
    //    {
    //        base.OnCleanUp();
    //    }
    //    public void Sim1000ms(float dt)
    //    {
    //        if (this.operational.IsOperational && this.IsRequirementOK())
    //        {
    //            this.operational.SetActive(true, false);
    //            this.DoCooling(dt);
    //        }
    //        else
    //        {
    //            this.operational.SetActive(false, false);
    //        }
    //        UpdateUnworkableStatusItems();
    //    }
    //    private bool IsRequirementOK()
    //    {
    //        return this.EnvironmentNeedsCooling() && this.EnvironmentHighEnoughPressure() && HasMaterial();
    //        //this.operational.SetFlag(IceCooledFanM.PumpableFlag, !this.storage.IsFull() && flag);
    //    }
    //    public bool EnvironmentNeedsCooling()
    //    {
    //        int cell = Grid.PosToCell(this.transform.GetPosition());
    //        for (int y = this.minCoolingRange.y; y < this.maxCoolingRange.y; ++y)
    //        {
    //            for (int x = this.minCoolingRange.x; x < this.maxCoolingRange.x; ++x)
    //            {
    //                CellOffset offset = new CellOffset(x, y);
    //                int index = Grid.OffsetCell(cell, offset);
    //                if ((double)Grid.Temperature[index] > (double)this.minCooledTemperature)
    //                    return true;
    //            }
    //        }
    //        return false;
    //    }
    //    public bool EnvironmentHighEnoughPressure()
    //    {
    //        int cell = Grid.PosToCell(this.transform.GetPosition());
    //        for (int y = this.minCoolingRange.y; y < this.maxCoolingRange.y; ++y)
    //        {
    //            for (int x = this.minCoolingRange.x; x < this.maxCoolingRange.x; ++x)
    //            {
    //                CellOffset offset = new CellOffset(x, y);
    //                int index = Grid.OffsetCell(cell, offset);
    //                if ((double)Grid.Mass[index] >= (double)this.minEnvironmentMass)
    //                    return true;
    //            }
    //        }
    //        return false;
    //    }
    //    public bool HasMaterial()
    //    {
    //        //this.UpdateMeter();
    //        return (double)this.iceStorage.MassStored() > 0.0;
    //    }
    //    private void UpdateUnworkableStatusItems()
    //    {
    //        KSelectable component = this.GetComponent<KSelectable>();
    //        if (!this.EnvironmentNeedsCooling())
    //        {
    //            if (!component.HasStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther))
    //                component.AddStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther, (object)null);
    //        }
    //        else if (component.HasStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther))
    //            component.RemoveStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther, false);
    //        if (!this.EnvironmentHighEnoughPressure())
    //        {
    //            if (!component.HasStatusItem(Db.Get().BuildingStatusItems.UnderPressure))
    //                component.AddStatusItem(Db.Get().BuildingStatusItems.UnderPressure, (object)null);
    //        }
    //        else if (component.HasStatusItem(Db.Get().BuildingStatusItems.UnderPressure))
    //            component.RemoveStatusItem(Db.Get().BuildingStatusItems.UnderPressure, false);
    //    }
    //    private void DoCooling(float dt)
    //    {
    //        float kilowatts = this.coolingRate * dt;
    //        foreach (GameObject gameObject in this.iceStorage.items)
    //            GameUtil.DeltaThermalEnergy(gameObject.GetComponent<PrimaryElement>(), kilowatts, this.targetTemperature);
    //        for (int count = this.iceStorage.items.Count; count > 0; --count)
    //        {
    //            GameObject item_go = this.iceStorage.items[count - 1];
    //            if ((UnityEngine.Object)item_go != (UnityEngine.Object)null && (double)item_go.GetComponent<PrimaryElement>().Temperature > (double)item_go.GetComponent<PrimaryElement>().Element.highTemp && item_go.GetComponent<PrimaryElement>().Element.HasTransitionUp)
    //            {
    //                PrimaryElement component = item_go.GetComponent<PrimaryElement>();
    //                this.iceStorage.AddLiquid(component.Element.highTempTransitionTarget, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, false, true);
    //                this.iceStorage.ConsumeIgnoringDisease(item_go);
    //            }
    //        }
    //        for (int count = this.iceStorage.items.Count; count > 0; --count)
    //        {
    //            GameObject go = this.iceStorage.items[count - 1];
    //            if ((UnityEngine.Object)go != (UnityEngine.Object)null && (double)go.GetComponent<PrimaryElement>().Temperature >= (double)this.targetTemperature)
    //                this.iceStorage.Transfer(go, this.liquidStorage, true, true);
    //        }
    //        if (!this.liquidStorage.IsEmpty())
    //            this.liquidStorage.DropAll(false, false, new Vector3(1f, 0.0f, 0.0f), true);
    //        this.UpdateMeter();
    //    }
    //}

    [HarmonyPatch(typeof(IceCooledFanConfig), "CreateBuildingDef")]
    internal class IceCooledFanPre
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeIceCooledFan;
        }

        private static void Postfix(BuildingDef __result)
        {
            __result.ExhaustKilowattsWhenActive = -58f;
            __result.SelfHeatKilowattsWhenActive = -6f;
            __result.RequiresPowerInput = true;
            __result.EnergyConsumptionWhenActive = 120f;
        }
    }


    [HarmonyPatch(typeof(IceCooledFanConfig), "ConfigureBuildingTemplate")]
    internal class IceCooledFanMid
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeIceCooledFan;
        }

        private static bool Prefix(GameObject go, Tag prefab_tag)
        {
            Storage storage1 = go.AddOrGet<Storage>();
            storage1.capacityKg = 50f;
            storage1.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);

            go.AddOrGet<MassiveHeatSink>();
            go.AddOrGetDef<PoweredActiveController.Def>();

            go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = 273.15f;
            go.AddOrGet<LoopingSounds>();
            Prioritizable.AddRef(go);
            ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage1);
            manualDeliveryKg.RequestedItemTag = GameTags.IceOre;
            manualDeliveryKg.capacity = storage1.capacityKg;
            manualDeliveryKg.refillMass = storage1.capacityKg * 0.2f;
            manualDeliveryKg.MinimumMass = 10f;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
                { new ElementConverter.ConsumedElement(GameTags.IceOre, 0.01f) };
            elementConverter.outputElements = new ElementConverter.OutputElement[]
                { new ElementConverter.OutputElement(0.01f, SimHashes.Water, 278.15f, false, true, 0.0f, 0.5f, 1f, byte.MaxValue, 0) };

            ElementDropper elementDropper = go.AddOrGet<ElementDropper>();
            elementDropper.emitMass = 10f;
            elementDropper.emitTag = new Tag("Water");
            elementDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);
            return false;
        }
    }

    [HarmonyPatch(typeof(IceCooledFanConfig), "DoPostConfigureComplete")]
    internal class IceCooledFanPost
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeIceCooledFan;
        }

        private static bool Prefix(GameObject go)
        {
            return false;
        }
    }

}