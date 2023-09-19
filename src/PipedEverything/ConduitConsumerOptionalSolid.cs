using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Common;

#pragma warning disable CS0649

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    public class ConduitConsumerOptionalSolid : KMonoBehaviour, IConduitConsumer, ISecondaryInput
    {
        [SerializeField]
        private float[] capacities = Array.Empty<float>();

        [SerializeField]
        public CellOffset conduitOffset;

        [SerializeField]
        public SimHashes[] elementFilter = Array.Empty<SimHashes>();

        [SerializeField]
        public float capacityKG = float.PositiveInfinity;

        [SerializeField]
        public bool alwaysConsume = true;

        [SerializeField]
        public int storageIndex;

        [MyCmpReq]
        private Operational operational;

        [MyCmpReq]
        private Building building;

        private Storage _storage;

        private HandleVector<int>.Handle partitionerEntry;

        private int utilityCell = -1;

        private FlowUtilityNetwork.NetworkItem networkItem;

        private bool consuming;

        public Storage Storage => this._storage ??= GetComponents<Storage>()[this.storageIndex];

        public ConduitType ConduitType => ConduitType.Solid;

        public bool IsConsuming => this.consuming;

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

        public float stored_mass
        {
            get
            {
                if (this.Storage == null)
                    return 0f;

                float mass = 0f;
                for (int i = 0; i < this.Storage.items.Count; i++)
                {
                    var gameObject = this.Storage.items[i];
                    if (gameObject == null)
                        continue;

                    var element = gameObject.GetComponent<PrimaryElement>();
                    if (this.elementFilter.Contains(element.ElementID))
                        mass += element.Mass;
                }

                return mass;
            }
        }

        public float space_remaining_kg
        {
            get
            {
                if (this.Storage == null || this.capacities.Length == 0)
                    return this.capacityKG;

                this.capacities.Fill(this.capacityKG);
                float capacityStorage = this.Storage.capacityKg;
                for (int i = 0; i < this.Storage.items.Count; i++)
                {
                    var gameObject = this.Storage.items[i];
                    if (gameObject == null)
                        continue;

                    var element = gameObject.GetComponent<PrimaryElement>();
                    capacityStorage -= element.Mass;
                    int index = Array.IndexOf(this.elementFilter, element.ElementID);
                    if (index < 0)
                        continue;

                    this.capacities[index] -= element.Mass;
                }

                return Mathf.Min(this.capacities.Min(), capacityStorage);
            }
        }

        private SolidConduitFlow GetConduitFlow()
        {
            return Game.Instance.solidConduitFlow;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.utilityCell = this.GetInputCell();
            ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[20];
            this.partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, OnConduitConnectionChanged);
            this.GetConduitFlow().AddConduitUpdater(ConduitUpdate);
            this.OnConduitConnectionChanged(null);

            this.networkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Sink, this.utilityCell, base.gameObject);
            Game.Instance.solidConduitSystem.AddToNetworks(this.utilityCell, this.networkItem, true);
        }

        public override void OnCleanUp()
        {
            this.GetConduitFlow().RemoveConduitUpdater(ConduitUpdate);
            GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
            base.OnCleanUp();

            Game.Instance.solidConduitSystem.RemoveFromNetworks(this.utilityCell, this.networkItem, is_endpoint: true);
        }

        private void OnConduitConnectionChanged(object data)
        {
            this.consuming = this.consuming && this.IsConnected;
            base.Trigger(-2094018600, this.IsConnected);
        }

        private void ConduitUpdate(float dt)
        {
            bool flag = false;
            if (this.IsConnected)
            {
                var conduitFlow = GetConduitFlow();
                var contents = conduitFlow.GetContents(this.utilityCell);
                if (contents.pickupableHandle.IsValid() && (this.alwaysConsume || this.operational.IsOperational))
                {
                    var pickupable = conduitFlow.GetPickupable(contents.pickupableHandle);
                    float remainingSpace = CapacityForElement(pickupable.PrimaryElement.ElementID);
                    if (remainingSpace > 0f)
                    {
                        if (pickupable.PrimaryElement.Mass <= remainingSpace || pickupable.PrimaryElement.Mass > Mathf.Min(this.Storage.capacityKg, this.capacityKG))
                        {
                            var pickupable2 = conduitFlow.RemovePickupable(this.utilityCell);
                            if ((bool)pickupable2)
                            {
                                this.Storage.Store(pickupable2.gameObject, hide_popups: true);
                                flag = true;
                            }
                        }
                    }
                }
            }

            if (this.Storage != null)
            {
                this.Storage.storageNetworkID = GetConnectedNetworkID();
            }
            this.consuming = flag;
        }

        private float CapacityForElement(SimHashes element)
        {
            if (elementFilter.Length == 0)
                return this.Storage.RemainingCapacity();

            if (!elementFilter.Contains(element))
                return 0f;

            float capacityElement = this.capacityKG;
            float capacityStorage = this.Storage.capacityKg;
            foreach (var item in this.Storage.items)
            {
                if (item == null)
                    continue;

                var element2 = item.GetComponent<PrimaryElement>();
                capacityStorage -= element2.Mass;
                if (element == element2.ElementID)
                    capacityElement -= element2.Mass;
            }

            return Mathf.Min(capacityElement, capacityStorage);
        }

        private int GetConnectedNetworkID()
        {
            GameObject gameObject = Grid.Objects[this.utilityCell, 20];
            SolidConduit solidConduit = ((gameObject != null) ? gameObject.GetComponent<SolidConduit>() : null);
            return ((solidConduit != null) ? solidConduit.GetNetwork() : null)?.id ?? (-1);
        }

        private int GetInputCell()
        {
            return this.building.GetCellWithOffset(this.conduitOffset);
        }

        public void AssignPort(PortDisplayInfo port)
        {
            this.conduitOffset = port.offset;
            this.elementFilter = port.filter;
            this.storageIndex = port.StorageIndex;

            this.capacities = new float[this.elementFilter.Length];
            this.capacityKG = port.StorageCapacity;
        }

        public bool HasSecondaryConduitType(ConduitType type)
        {
            return type == ConduitType.Solid;
        }

        public CellOffset GetSecondaryConduitOffset(ConduitType type)
        {
            return this.conduitOffset;
        }
    }
}
