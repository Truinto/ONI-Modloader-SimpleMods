using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSerialization;
using UnityEngine;
using Common;

#pragma warning disable CS0649

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    public class ConduitDispenserOptionalSolid : KMonoBehaviour, IConduitDispenser
    {
        [SerializeField]
        public CellOffset conduitOffset;

        [SerializeField]
        public SimHashes[] elementFilter;

        [SerializeField]
        public bool alwaysDispense = true;

        [SerializeField]
        public int storageIndex;

        [MyCmpReq]
        private Operational operational;

        private Storage _storage;

        [MyCmpReq]
        private Building building;

        private HandleVector<int>.Handle partitionerEntry;

        private int utilityCell = -1;

        private FlowUtilityNetwork.NetworkItem networkItem;

        private bool dispensing;

        private int round_robin_index;

        public Storage Storage => this._storage ??= GetComponents<Storage>()[this.storageIndex];

        public ConduitType ConduitType => ConduitType.Solid;

        public SolidConduitFlow.ConduitContents ConduitContents => GetConduitFlow().GetContents(this.utilityCell);

        public bool IsDispensing => this.dispensing;

        public bool IsConnected
        {
            get
            {
                GameObject gameObject = Grid.Objects[this.utilityCell, 20];
                if (gameObject != null)
                {
                    return gameObject.GetComponent<BuildingComplete>() != null;
                }
                return false;
            }
        }

        public SolidConduitFlow GetConduitFlow()
        {
            return Game.Instance.solidConduitFlow;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.utilityCell = GetOutputCell();
            ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[20];
            this.partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, OnConduitConnectionChanged);
            GetConduitFlow().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Dispense);
            OnConduitConnectionChanged(null);

            this.networkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Source, this.utilityCell, base.gameObject);
            Game.Instance.solidConduitSystem.AddToNetworks(this.utilityCell, this.networkItem, true);
        }

        public override void OnCleanUp()
        {
            GetConduitFlow().RemoveConduitUpdater(ConduitUpdate);
            GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
            base.OnCleanUp();

            Game.Instance.solidConduitSystem.RemoveFromNetworks(this.utilityCell, this.networkItem, is_endpoint: true);
        }

        private void OnConduitConnectionChanged(object data)
        {
            this.dispensing = this.dispensing && this.IsConnected;
            base.Trigger(-2094018600, this.IsConnected);
        }

        private void ConduitUpdate(float dt)
        {
            bool flag = false;
            //this.operational.SetFlag(SolidConduitDispenser.outputConduitFlag, this.IsConnected);
            if (this.operational.IsOperational || this.alwaysDispense)
            {
                SolidConduitFlow conduitFlow = GetConduitFlow();
                if (conduitFlow.HasConduit(this.utilityCell) && conduitFlow.IsConduitEmpty(this.utilityCell))
                {
                    Pickupable pickupable = FindSuitableItem();
                    if ((bool)pickupable)
                    {
                        if (pickupable.PrimaryElement.Mass > 20f)
                        {
                            pickupable = pickupable.Take(20f);
                        }
                        conduitFlow.AddPickupable(this.utilityCell, pickupable);
                        flag = true;
                    }
                }
            }
            this.Storage.storageNetworkID = GetConnectedNetworkID();
            this.dispensing = flag;
        }

        private Pickupable FindSuitableItem()
        {
            var list = this.Storage.items;
            var list3 = new List<GameObject>(list.Count);
            foreach (var item in list)
            {
                var element2 = item.GetComponent<PrimaryElement>();
                if (this.elementFilter.Length == 0 && element2.Element.IsSolid || this.elementFilter.Contains(element2.ElementID))
                    list3.Add(item);
            }

            if (list3.Count < 1)
                return null;

            this.round_robin_index %= list3.Count;
            return list3[this.round_robin_index++]?.GetComponent<Pickupable>();
        }

        private int GetConnectedNetworkID()
        {
            GameObject gameObject = Grid.Objects[this.utilityCell, 20];
            SolidConduit solidConduit = ((gameObject != null) ? gameObject.GetComponent<SolidConduit>() : null);
            return ((solidConduit != null) ? solidConduit.GetNetwork() : null)?.id ?? (-1);
        }

        private int GetOutputCell()
        {
            return this.building.GetCellWithOffset(this.conduitOffset);
        }

        public void AssignPort(PortDisplayInfo port)
        {
            this.conduitOffset = port.offset;
            this.elementFilter = port.filter;
            this.storageIndex = port.StorageIndex;
        }
    }
}
