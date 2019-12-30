// Decompiled with JetBrains decompiler
// Type: ConduitConsumer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3EAEEF34-517A-43A2-A9FE-A8421AAC144C
// Assembly location: D:\Programme\Steam\SteamApps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class ConduitConsumer2 : KMonoBehaviour, ISecondaryInput
{
    public static readonly Operational.Flag elementRequirementFlag = new Operational.Flag("elementRequired", Operational.Flag.Type.Requirement);
    [SerializeField]
    public bool ignoreMinMassCheck = false;
    [SerializeField]
    public Tag capacityTag = GameTags.Any;
    [SerializeField]
    public float capacityKG = float.PositiveInfinity;
    [SerializeField]
    public bool forceAlwaysSatisfied = false;
    [SerializeField]
    public bool alwaysConsume = false;
    [SerializeField]
    public bool keepZeroMassObject = true;
    [SerializeField]
    public bool useSecondaryInput = false;
    [NonSerialized]
    public bool isConsuming = true;
    private int utilityCell = -1;
    public ConduitPortInfo secondaryInput;
    public float consumptionRate = float.PositiveInfinity;
    public SimHashes lastConsumedElement = SimHashes.Vacuum;
    private bool satisfied = false;
    public ConduitConsumer2.WrongElementResult wrongElementResult = ConduitConsumer2.WrongElementResult.Destroy;
    [SerializeField]
    public ConduitType conduitType;
    [MyCmpReq]
    public Operational operational;
    [MyCmpReq]
    private Building building;
    [MyCmpGet]
    public Storage storage;
    private HandleVector<int>.Handle partitionerEntry;

    public bool IsConnected
    {
        get
        {
            GameObject gameObject = Grid.Objects[this.utilityCell, this.conduitType != ConduitType.Gas ? 16 : 12];
            return (UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)gameObject.GetComponent<BuildingComplete>() != (UnityEngine.Object)null;
        }
    }

    public bool CanConsume
    {
        get
        {
            bool flag = false;
            if (this.IsConnected)
                flag = (double)this.GetConduitManager().GetContents(this.utilityCell).mass > 0.0;
            return flag;
        }
    }

    public float stored_mass
    {
        get
        {
            return !((UnityEngine.Object)this.storage == (UnityEngine.Object)null) ? (!(this.capacityTag != GameTags.Any) ? this.storage.MassStored() : this.storage.GetMassAvailable(this.capacityTag)) : 0.0f;
        }
    }

    public float space_remaining_kg
    {
        get
        {
            float b = this.capacityKG - this.stored_mass;
            return !((UnityEngine.Object)this.storage == (UnityEngine.Object)null) ? Mathf.Min(this.storage.RemainingCapacity(), b) : b;
        }
    }

    public void SetConduitData(ConduitType type)
    {
        this.conduitType = type;
    }

    public ConduitType TypeOfConduit
    {
        get
        {
            return this.conduitType;
        }
    }

    public bool IsAlmostEmpty
    {
        get
        {
            return !this.ignoreMinMassCheck && (double)this.MassAvailable < (double)this.ConsumptionRate * 30.0;
        }
    }

    public bool IsEmpty
    {
        get
        {
            return !this.ignoreMinMassCheck && ((double)this.MassAvailable == 0.0 || (double)this.MassAvailable < (double)this.ConsumptionRate);
        }
    }

    public float ConsumptionRate
    {
        get
        {
            return this.consumptionRate;
        }
    }

    public bool IsSatisfied
    {
        get
        {
            return this.satisfied || !this.isConsuming;
        }
        set
        {
            this.satisfied = value || this.forceAlwaysSatisfied;
        }
    }

    private ConduitFlow GetConduitManager()
    {
        switch (this.conduitType)
        {
            case ConduitType.Gas:
                return Game.Instance.gasConduitFlow;
            case ConduitType.Liquid:
                return Game.Instance.liquidConduitFlow;
            default:
                return (ConduitFlow)null;
        }
    }

    public float MassAvailable
    {
        get
        {
            return this.GetConduitManager().GetContents(this.GetInputCell()).mass;
        }
    }

    private int GetInputCell()
    {
        //if (this.useSecondaryInput)
        //    return Grid.OffsetCell(this.building.NaturalBuildingCell(), this.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset());
        //return this.building.GetUtilityInputCell();
        return Grid.OffsetCell(this.building.NaturalBuildingCell(), secondaryInput.offset);
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        this.utilityCell = this.GetInputCell();
        this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", (object)this.gameObject, this.utilityCell, GameScenePartitioner.Instance.objectLayers[this.conduitType != ConduitType.Gas ? 16 : 12], new System.Action<object>(this.OnConduitConnectionChanged));
        this.GetConduitManager().AddConduitUpdater(new System.Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
        this.OnConduitConnectionChanged((object)null);
    }

    protected override void OnCleanUp()
    {
        this.GetConduitManager().RemoveConduitUpdater(new System.Action<float>(this.ConduitUpdate));
        GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
        base.OnCleanUp();
    }

    private void OnConduitConnectionChanged(object data)
    {
        this.Trigger(-2094018600, (object)this.IsConnected);
    }

    private void ConduitUpdate(float dt)
    {
        if (!this.isConsuming)
            return;
        ConduitFlow conduitManager = this.GetConduitManager();
        this.Consume(dt, conduitManager);
    }

    private void Consume(float dt, ConduitFlow conduit_mgr)
    {
        this.IsSatisfied = false;
        if (this.building.Def.CanMove)
            this.utilityCell = this.GetInputCell();
        if (!this.IsConnected)
            return;
        ConduitFlow.ConduitContents contents = conduit_mgr.GetContents(this.utilityCell);
        if ((double)contents.mass <= 0.0)
            return;
        this.IsSatisfied = true;
        if (!this.alwaysConsume && !this.operational.IsOperational)
            return;
        float delta = Mathf.Min(this.ConsumptionRate * dt, this.space_remaining_kg);
        float mass = 0.0f;
        if ((double)delta > 0.0)
        {
            ConduitFlow.ConduitContents conduitContents = conduit_mgr.RemoveElement(this.utilityCell, delta);
            mass = conduitContents.mass;
            this.lastConsumedElement = conduitContents.element;
        }
        bool flag = ElementLoader.FindElementByHash(contents.element).HasTag(this.capacityTag);
        if ((double)mass > 0.0 && this.capacityTag != GameTags.Any && !flag)
            this.Trigger(-794517298, (object)new BuildingHP.DamageSourceInfo()
            {
                damage = 1,
                source = (string)BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
                popString = (string)UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
            });
        if (flag || this.wrongElementResult == ConduitConsumer2.WrongElementResult.Store || (contents.element == SimHashes.Vacuum || this.capacityTag == GameTags.Any))
        {
            if ((double)mass <= 0.0)
                return;
            int disease_count = (int)((double)contents.diseaseCount * ((double)mass / (double)contents.mass));
            Element elementByHash = ElementLoader.FindElementByHash(contents.element);
            switch (this.conduitType)
            {
                case ConduitType.Gas:
                    if (elementByHash.IsGas)
                    {
                        this.storage.AddGasChunk(contents.element, mass, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
                        break;
                    }
                    Debug.LogWarning((object)("Gas conduit consumer consuming non gas: " + elementByHash.id.ToString()));
                    break;
                case ConduitType.Liquid:
                    if (elementByHash.IsLiquid)
                    {
                        this.storage.AddLiquid(contents.element, mass, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
                        break;
                    }
                    Debug.LogWarning((object)("Liquid conduit consumer consuming non liquid: " + elementByHash.id.ToString()));
                    break;
            }
        }
        else
        {
            if ((double)mass <= 0.0 || this.wrongElementResult != ConduitConsumer2.WrongElementResult.Dump)
                return;
            int disease_count = (int)((double)contents.diseaseCount * ((double)mass / (double)contents.mass));
            SimMessages.AddRemoveSubstance(Grid.PosToCell(this.transform.GetPosition()), contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, mass, contents.temperature, contents.diseaseIdx, disease_count, true, -1);
        }
    }

    ConduitType ISecondaryInput.GetSecondaryConduitType()
    {
        return this.secondaryInput.conduitType;
    }

    CellOffset ISecondaryInput.GetSecondaryConduitOffset()
    {
        return this.secondaryInput.offset;
    }

    public enum WrongElementResult
    {
        Destroy,
        Dump,
        Store,
    }
}
