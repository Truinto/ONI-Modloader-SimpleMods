
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;*/
using System;
using KSerialization;
using UnityEngine;

public class LiquidReservoirSmart : KMonoBehaviour, IUserControlledCapacity, ISim1000ms
{
    //private static readonly EventSystem.IntraObjectHandler<LiquidReservoirSmart> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<LiquidReservoirSmart>((component, data) => component.OnStorageChange(data));

    [MyCmpGet]
    private LogicPorts ports;
    private MeterController meter;
    [MyCmpGet]
    private Storage storage;
    [MyCmpGet]
    private Operational operational;
    [Serialize]
    private float userMaxCapacity = float.PositiveInfinity;
    private LoggerFS log;
    protected FilteredStorage filteredStorage;

    protected override void OnPrefabInit()
    {
        this.Initialize(true);
    }

    protected void Initialize(bool use_logic_meter)
    {
        base.OnPrefabInit();
        this.log = new LoggerFS(nameof(StorageLocker), 35);
        this.filteredStorage = new FilteredStorage((KMonoBehaviour)this, (Tag[])null, (Tag[])null, (IUserControlledCapacity)this, use_logic_meter, Db.Get().ChoreTypes.StorageFetch);
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        this.filteredStorage.FilterChanged();
        this.ports = this.gameObject.GetComponent<LogicPorts>();
        this.meter = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[2]
        {
            "meter_fill",
            "meter_OL"
        });
        //this.Subscribe<LiquidReservoirSmart>((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
        this.OnStorageChange((object) null);
    }

    private void UpdateLogic(bool on)
    {
        this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, !on ? 0 : 1);
        /*bool flag = this.filteredStorage.IsFull();
        this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, !flag ? 0 : 1);
        this.filteredStorage.SetLogicMeter(flag);*/
    }

    private void OnStorageChange(object data)
    {
        float percent = this.AmountStored / this.UserMaxCapacity;
        this.UpdateLogic(percent >= 1);
        this.meter.SetPositionPercent(Mathf.Clamp01(percent));
    }

    protected override void OnCleanUp()
    {
        this.filteredStorage.CleanUp();
    }

    public void Sim1000ms(float dt)
    {
        OnStorageChange((object) null);
    }

    public float UserMaxCapacity
    {
        get
        {
            return Mathf.Min(this.userMaxCapacity, this.GetComponent<Storage>().capacityKg);
        }
        set
        {
            this.userMaxCapacity = value;
            this.filteredStorage.FilterChanged();
        }
    }

    public float AmountStored
    {
        get
        {
            return this.GetComponent<Storage>().MassStored();
        }
    }

    public float MinCapacity
    {
        get
        {
            return 0.0f;
        }
    }

    public float MaxCapacity
    {
        get
        {
            return this.GetComponent<Storage>().capacityKg;
        }
    }

    public bool WholeValues
    {
        get
        {
            return false;
        }
    }

    public LocString CapacityUnits
    {
        get
        {
            return GameUtil.GetCurrentMassUnit(false);
        }
    }
}