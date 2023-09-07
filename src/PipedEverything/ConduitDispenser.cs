using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System.Linq;

namespace PipedEverything
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ConduitDispenser2 : PortConduitDispenserBase
    {

    }

    // Don't add this directly as components to GameObjects
    // Use a subclass to inherit this and doesn't add anything
    // This is to avoid crashes on loading savegames due to class name clashes
    [SerializationConfig(MemberSerialization.OptIn)]
    public class PortConduitDispenserBase : KMonoBehaviour, ISaveLoadable
    {
        [SerializeField]
        public CellOffset conduitOffset;

        [SerializeField]
        public ConduitType conduitType;

        [SerializeField]
        public SimHashes[] elementFilter = null;

        [SerializeField]
        public bool invertElementFilter;

        [SerializeField]
        public bool alwaysDispense = true;

        [SerializeField]
        public bool SkipSetOperational = true;

        private static readonly Operational.Flag outputConduitFlag = new("output_conduit", Operational.Flag.Type.Functional);

        private FlowUtilityNetwork.NetworkItem networkItem;

        [MyCmpReq]
        readonly private Operational operational;

        [MyCmpReq]
        public Storage storage;

        private HandleVector<int>.Handle partitionerEntry;

        private int utilityCell = -1;

        private int elementOutputOffset;

        public void AssignPort(DisplayConduitPortInfo port)
        {
            this.conduitType = port.type;
            this.conduitOffset = port.offset;
            this.elementFilter = port.filter;
        }

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
                return GetConduitManager().GetContents(this.utilityCell);
            }
        }

        public int UtilityCell
        {
            get
            {
                return this.utilityCell;
            }
        }

        public bool IsConnected
        {
            get
            {
                GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 16 : 12];
                return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
            }
        }

        public void SetConduitData(ConduitType type)
        {
            this.conduitType = type;
        }

        public ConduitFlow GetConduitManager()
        {
            if (this.conduitType == ConduitType.Gas)            
                return Game.Instance.gasConduitFlow;            
            if (this.conduitType == ConduitType.Liquid)            
                return Game.Instance.liquidConduitFlow;            
            return null;
        }

        private void OnConduitConnectionChanged(object data)
        {
            base.Trigger((int)GameHashes.ConduitConnectionChanged, this.IsConnected);
        }

        internal virtual CellOffset GetUtilityCellOffset()
        {
            return new CellOffset(0, 1);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            this.utilityCell = this.GetCellWithOffset(this.conduitOffset);
            IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
            this.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Source, this.utilityCell, base.gameObject);
            networkManager.AddToNetworks(this.utilityCell, this.networkItem, true);

            ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
            this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, new Action<object>(OnConduitConnectionChanged));
            GetConduitManager().AddConduitUpdater(new Action<float>(ConduitUpdate), ConduitFlowPriority.LastPostUpdate);
            OnConduitConnectionChanged(null);
        }

        public override void OnCleanUp()
        {
            IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
            networkManager.RemoveFromNetworks(this.utilityCell, this.networkItem, true);

            GetConduitManager().RemoveConduitUpdater(new Action<float>(ConduitUpdate));
            GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
            base.OnCleanUp();
        }

        public virtual void ConduitUpdate(float dt)
        {
            if (!SkipSetOperational)
            {
                this.operational.SetFlag(ConduitDispenser.outputConduitFlag, this.IsConnected);
            }
            if (this.operational.IsOperational || this.alwaysDispense)
            {
                PrimaryElement primaryElement = FindSuitableElement();
                if (primaryElement != null)
                {
                    primaryElement.KeepZeroMassObject = true;
                    ConduitFlow conduitManager = GetConduitManager();
                    float num = conduitManager.AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                    if (num > 0f)
                    {
                        float num2 = num / primaryElement.Mass;
                        int num3 = (int)(num2 * (float)primaryElement.DiseaseCount);
                        primaryElement.ModifyDiseaseCount(-num3, "CustomConduitDispenser.ConduitUpdate");
                        primaryElement.Mass -= num;
                        base.Trigger(-1697596308, primaryElement.gameObject);
                    }
                }
            }
        }

        protected virtual PrimaryElement FindSuitableElement()
        {
            List<GameObject> items = this.storage.items;
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                int index = (i + this.elementOutputOffset) % count;
                PrimaryElement component = items[index].GetComponent<PrimaryElement>();
                if (component != null && component.Mass > 0f && ((this.conduitType != ConduitType.Liquid) ? component.Element.IsGas : component.Element.IsLiquid) && (this.elementFilter == null || this.elementFilter.Length == 0 || (!this.invertElementFilter && IsFilteredElement(component.ElementID)) || (this.invertElementFilter && !IsFilteredElement(component.ElementID))))
                {
                    this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
                    return component;
                }
            }
            return null;
        }

        private bool IsFilteredElement(SimHashes element)
        {
            for (int num = 0; num < this.elementFilter.Length; num++)
            {
                if (this.elementFilter[num] == element)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
