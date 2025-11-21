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
        public CellOffset conduitOffset;

        [SerializeField]
        public Tag[] tagFilter = [];

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

        public bool IsConnected => Grid.Objects[this.utilityCell, 20]?.GetComponent<BuildingComplete>() != null;

        private SolidConduitFlow GetConduitFlow()
        {
            return Game.Instance.solidConduitFlow;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.utilityCell = GetInputCell();
            ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[20];
            this.partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, OnConduitConnectionChanged);
            GetConduitFlow().AddConduitUpdater(ConduitUpdate);
            OnConduitConnectionChanged(null);

            this.networkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Sink, this.utilityCell, base.gameObject);
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
            this.consuming = this.consuming && this.IsConnected;
            BoxingTrigger((int)GameHashes.ConduitConnectionChanged, this.IsConnected);
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
                    float remainingSpace = CapacityForElement(pickupable);
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

        private float CapacityForElement(Pickupable element)
        {
            Tag tag;
            if (this.tagFilter.Length == 0)
                tag = element?.PrimaryElement?.ElementID.ToTag() ?? default;
            else
                tag = GetMatch(element?.GetComponent<KPrefabID>());
            if (!tag.IsValid)
                return 0f;

            var storage = this.Storage;
            float capacityElement = this.capacityKG;
            float capacityStorage = this.Storage.capacityKg;
            int count = storage.items.Count;
            for (int i = 0; i < count; i++)
            {
                var item = storage.items[i];
                if (item == null)
                {
                    storage.items.RemoveAt(i);
                    count = storage.items.Count;
                    continue;
                }

                var element2 = item.GetComponent<PrimaryElement>();
                if (element2 == null)
                    continue;
                capacityStorage -= element2.Mass;
                if (item.HasTag(tag))
                    capacityElement -= element2.Mass;
            }

            return Mathf.Min(capacityElement, capacityStorage);
        }

        private Tag GetMatch(KPrefabID? prefabID)
        {
            if (prefabID == null)
                return default;
            foreach (var tag in this.tagFilter)
            {
                if (tag.IsTag(prefabID.PrefabTag))
                    return tag;
                foreach (var ptag in prefabID.Tags)
                    if (tag.IsTag(ptag))
                        return tag;
            }
            return default;
        }

        private int GetConnectedNetworkID()
        {
            return Grid.Objects[this.utilityCell, 20]
                ?.GetComponent<SolidConduit>()
                ?.GetNetwork()
                ?.id ?? -1;
        }

        private int GetInputCell()
        {
            return this.building.GetCellWithOffset(this.conduitOffset);
        }

        public void AssignPort(PortDisplayInfo port)
        {
            this.conduitOffset = port.offset;
            this.tagFilter = port.filterTags;
            this.storageIndex = port.StorageIndex;
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
