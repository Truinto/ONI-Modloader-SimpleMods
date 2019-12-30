// Decompiled with JetBrains decompiler
// Type: ConduitDispenser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3EAEEF34-517A-43A2-A9FE-A8421AAC144C
// Assembly location: D:\Programme\Steam\SteamApps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;
using System;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitDispenser2 : KMonoBehaviour, ISaveLoadable, ISecondaryOutput
{
    private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);
    [SerializeField]
    public SimHashes[] elementFilter = (SimHashes[])null;
    [SerializeField]
    public bool invertElementFilter = false;
    [SerializeField]
    public bool alwaysDispense = false;
    private int utilityCell = -1;
    public ConduitPortInfo secondaryOutput;
    [MyCmpReq]
    private Building building;
    private int elementOutputOffset = 0;
    [SerializeField]
    public ConduitType conduitType;
    [MyCmpReq]
    private Operational operational;
    [MyCmpReq]
    public Storage storage;
    private HandleVector<int>.Handle partitionerEntry;

    public ConduitType TypeOfConduit
    {
        get
        {
            return this.conduitType;
        }
    }

    public ConduitFlow.ConduitContents ConduitContents
    {
        get
        {
            return this.GetConduitManager().GetContents(this.utilityCell);
        }
    }

    public void SetConduitData(ConduitType type)
    {
        this.conduitType = type;
    }

    public ConduitFlow GetConduitManager()
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

    private void OnConduitConnectionChanged(object data)
    {
        this.Trigger(-2094018600, (object)this.IsConnected);
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        this.utilityCell = Grid.OffsetCell(this.building.NaturalBuildingCell(), secondaryOutput.offset); //this.GetComponent<Building>().GetUtilityOutputCell();
        this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", (object)this.gameObject, this.utilityCell, GameScenePartitioner.Instance.objectLayers[this.conduitType != ConduitType.Gas ? 16 : 12], new System.Action<object>(this.OnConduitConnectionChanged));
        this.GetConduitManager().AddConduitUpdater(new System.Action<float>(this.ConduitUpdate), ConduitFlowPriority.Last);
        this.OnConduitConnectionChanged((object)null);
    }

    protected override void OnCleanUp()
    {
        this.GetConduitManager().RemoveConduitUpdater(new System.Action<float>(this.ConduitUpdate));
        GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
        base.OnCleanUp();
    }

    private void ConduitUpdate(float dt)
    {
        this.operational.SetFlag(ConduitDispenser2.outputConduitFlag, this.IsConnected);
        if (!this.operational.IsOperational && !this.alwaysDispense)
            return;
        PrimaryElement suitableElement = this.FindSuitableElement();
        if ((UnityEngine.Object)suitableElement != (UnityEngine.Object)null)
        {
            suitableElement.KeepZeroMassObject = true;
            float num1 = this.GetConduitManager().AddElement(this.utilityCell, suitableElement.ElementID, suitableElement.Mass, suitableElement.Temperature, suitableElement.DiseaseIdx, suitableElement.DiseaseCount);
            if ((double)num1 > 0.0)
            {
                int num2 = (int)((double)(num1 / suitableElement.Mass) * (double)suitableElement.DiseaseCount);
                suitableElement.ModifyDiseaseCount(-num2, "ConduitDispenser.ConduitUpdate");
                suitableElement.Mass -= num1;
                this.Trigger(-1697596308, (object)suitableElement.gameObject);
            }
        }
    }

    private PrimaryElement FindSuitableElement()
    {
        List<GameObject> items = this.storage.items;
        int count = items.Count;
        for (int index1 = 0; index1 < count; ++index1)
        {
            int index2 = (index1 + this.elementOutputOffset) % count;
            PrimaryElement component = items[index2].GetComponent<PrimaryElement>();
            if ((UnityEngine.Object)component != (UnityEngine.Object)null && (double)component.Mass > 0.0 && ((this.conduitType != ConduitType.Liquid ? (component.Element.IsGas ? 1 : 0) : (component.Element.IsLiquid ? 1 : 0)) != 0 && (this.elementFilter == null || this.elementFilter.Length == 0 || !this.invertElementFilter && this.IsFilteredElement(component.ElementID) || this.invertElementFilter && !this.IsFilteredElement(component.ElementID))))
            {
                this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
                return component;
            }
        }
        return (PrimaryElement)null;
    }

    private bool IsFilteredElement(SimHashes element)
    {
        for (int index = 0; index != this.elementFilter.Length; ++index)
        {
            if (this.elementFilter[index] == element)
                return true;
        }
        return false;
    }

    ConduitType ISecondaryOutput.GetSecondaryConduitType()
    {
        return this.secondaryOutput.conduitType;
    }

    CellOffset ISecondaryOutput.GetSecondaryConduitOffset()
    {
        return this.secondaryOutput.offset;
    }

    public bool IsConnected
    {
        get
        {
            GameObject gameObject = Grid.Objects[this.utilityCell, this.conduitType != ConduitType.Gas ? 16 : 12];
            return (UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)gameObject.GetComponent<BuildingComplete>() != (UnityEngine.Object)null;
        }
    }
}
