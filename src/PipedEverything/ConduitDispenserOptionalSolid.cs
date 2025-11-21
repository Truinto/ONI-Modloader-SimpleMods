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
        public Tag[] tagFilter = [];

        [SerializeField]
        public bool alwaysDispense = true;

        [SerializeField]
        public int storageIndex;

        [MyCmpReq]
        private Operational operational = null!;

        [MyCmpReq]
        private Building building = null!;

        private HandleVector<int>.Handle partitionerEntry;

        private int utilityCell = -1;

        private FlowUtilityNetwork.NetworkItem? networkItem;

        private int round_robin_index;

        private Storage? _storage;
        public Storage Storage => this._storage ??= GetComponents<Storage>()[this.storageIndex];

        public ConduitType ConduitType => ConduitType.Solid;

        public SolidConduitFlow.ConduitContents ConduitContents => GetConduitFlow().GetContents(this.utilityCell);

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

        private void OnConduitConnectionChanged(object? data)
        {
            BoxingTrigger((int)GameHashes.ConduitConnectionChanged, this.IsConnected);
        }

        private void ConduitUpdate(float dt)
        {
            //this.operational.SetFlag(SolidConduitDispenser.outputConduitFlag, this.IsConnected);
            if (this.operational.IsOperational || this.alwaysDispense)
            {
                SolidConduitFlow conduitFlow = GetConduitFlow();
                if (conduitFlow.HasConduit(this.utilityCell) && conduitFlow.IsConduitEmpty(this.utilityCell))
                {
                    var pickupable = FindSuitableItem();
                    if (pickupable != null)
                    {
                        if (pickupable.PrimaryElement.Mass > FumiKMod.SolidMaxMass)
                            pickupable = pickupable.Take(FumiKMod.SolidMaxMass) ?? pickupable;
                        conduitFlow.AddPickupable(this.utilityCell, pickupable);
                    }
                }
            }
            this.Storage.storageNetworkID = GetConnectedNetworkID();
        }

        private Pickupable? FindSuitableItem()
        {
            var items = this.Storage.items;
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                int index = (i + this.round_robin_index) % count;
                if (items[index] == null)
                {
                    items.RemoveAt(index);
                    count = items.Count;
                    continue;
                }
                var primaryElement = items[index].GetComponent<PrimaryElement>();
                if (primaryElement == null || primaryElement.Mass <= 0f)
                    continue;
                var pickupable = primaryElement.GetComponent<Pickupable>();
                if (pickupable == null)
                    continue;
                if (this.tagFilter.Length != 0 && !items[index].HasAnyTags(this.tagFilter))
                    continue;

                this.round_robin_index = (i + 1) % count;
                return pickupable;
            }
            return null;
        }

        private int GetConnectedNetworkID()
        {
            var gameObject = Grid.Objects[this.utilityCell, 20];
            return gameObject?.GetComponent<SolidConduit>()?.GetNetwork()?.id ?? -1;
        }

        private int GetOutputCell()
        {
            return this.building.GetCellWithOffset(this.conduitOffset);
        }

        public void AssignPort(PortDisplayInfo port)
        {
            this.conduitOffset = port.offset;
            this.tagFilter = port.filterTags;
            this.storageIndex = port.StorageIndex;
        }
    }
}
