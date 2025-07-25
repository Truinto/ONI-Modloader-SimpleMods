﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using KSerialization;
using UnityEngine;

#pragma warning disable CS0649

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    public class ConduitDispenserOptional : KMonoBehaviour, IConduitDispenser
    {
        [SerializeField]
        public ConduitType conduitType;

        [SerializeField]
        public CellOffset conduitOffset;

        [SerializeField]
        public SimHashes[] elementFilter = [];

        [SerializeField]
        public int storageIndex;

        [SerializeField]
        public bool invertElementFilter;

        [SerializeField]
        public bool alwaysDispense = true;

        [SerializeField]
        public bool isOn = true;

        [SerializeField]
        public bool blocked;

        [SerializeField]
        public bool empty = true;

        [SerializeField]
        public bool useSecondaryOutput;

        [MyCmpReq]
        private Operational operational = null!;

        private Storage? _storage;

        [MyCmpReq]
        private Building building = null!;

        private HandleVector<int>.Handle partitionerEntry;

        private int utilityCell = -1;

        private int RoundRobinIndex;

        private FlowUtilityNetwork.NetworkItem? networkItem;

        public Storage Storage => this._storage ??= GetComponents<Storage>()[this.storageIndex];

        public ConduitType ConduitType => this.conduitType;

        public ConduitFlow.ConduitContents ConduitContents => GetConduitManager().GetContents(this.utilityCell);

        public bool IsConnected
        {
            get
            {
                GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType == ConduitType.Gas) ? 12 : 16];
                if (gameObject != null)
                {
                    return gameObject.GetComponent<BuildingComplete>() != null;
                }
                return false;
            }
        }

        public void SetConduitData(ConduitType type)
        {
            this.conduitType = type;
        }

        public ConduitFlow GetConduitManager()
        {
            return this.conduitType switch
            {
                ConduitType.Gas => Game.Instance.gasConduitFlow,
                ConduitType.Liquid => Game.Instance.liquidConduitFlow,
                _ => throw new Exception("Cannot get valid ConduitType"),
            };
        }

        private void OnConduitConnectionChanged(object? data)
        {
            base.Trigger((int)GameHashes.ConduitConnectionChanged, this.IsConnected);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.utilityCell = GetOutputCell(this.conduitType);
            var layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType == ConduitType.Gas) ? 12 : 16];
            this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, OnConduitConnectionChanged);
            GetConduitManager().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Dispense); //ConduitFlowPriority.LastPostUpdate?
            OnConduitConnectionChanged(null);

            this.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Source, this.utilityCell, base.gameObject);
            Conduit.GetNetworkManager(this.conduitType).AddToNetworks(this.utilityCell, this.networkItem, true);
        }

        public override void OnCleanUp()
        {
            GetConduitManager().RemoveConduitUpdater(ConduitUpdate);
            GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
            base.OnCleanUp();

            Conduit.GetNetworkManager(this.conduitType).RemoveFromNetworks(this.utilityCell, this.networkItem, is_endpoint: true);
        }

        public void SetOnState(bool onState)
        {
            this.isOn = onState;
        }

        private void ConduitUpdate(float dt)
        {
            //this.operational.SetFlag(ConduitDispenser.outputConduitFlag, this.IsConnected);
            this.blocked = false;
            if (this.isOn)
            {
                Dispense(dt);
            }
        }

        private void Dispense(float dt)
        {
            if (!this.operational.IsOperational && !this.alwaysDispense)
            {
                return;
            }
            if (this.building.Def.CanMove)
            {
                this.utilityCell = GetOutputCell(GetConduitManager().conduitType);
            }
            var primaryElement = FindSuitableElement();
            if (primaryElement != null)
            {
                primaryElement.KeepZeroMassObject = true;
                this.empty = false;
                float num = GetConduitManager().AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                if (num > 0f)
                {
                    int num2 = (int)(num / primaryElement.Mass * (float)primaryElement.DiseaseCount);
                    primaryElement.ModifyDiseaseCount(-num2, "ConduitDispenser.ConduitUpdate");
                    primaryElement.Mass -= num;
                    this.Storage.Trigger((int)GameHashes.OnStorageChange, primaryElement.gameObject);
                }
                else
                {
                    this.blocked = true;
                }
            }
            else
            {
                this.empty = true;
            }
        }

        private PrimaryElement? FindSuitableElement()
        {
            List<GameObject> items = this.Storage.items;
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                int index = (i + this.RoundRobinIndex) % count;
                if (items[index] == null)
                {
                    items.RemoveAt(index);
                    count = items.Count;
                    continue;
                }
                var primaryElement = items[index].GetComponent<PrimaryElement>();
                if (primaryElement == null || primaryElement.Mass <= 0f)
                    continue;
                if (this.ConduitType != primaryElement.Element.GetConduitType())
                    continue;
                if (this.elementFilter.Length == 0 
                    || (!this.invertElementFilter && IsFilteredElement(primaryElement.ElementID)) 
                    || (this.invertElementFilter && !IsFilteredElement(primaryElement.ElementID)))
                {
                    this.RoundRobinIndex = (i + 1) % count;
                    return primaryElement;
                }
            }
            return null;
        }

        private bool IsFilteredElement(SimHashes element)
        {
            for (int i = 0; i != this.elementFilter!.Length; i++)
            {
                if (this.elementFilter[i] == element || this.elementFilter[i] == SimHashes.Void)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetOutputCell(ConduitType outputConduitType)
        {
            return this.building.GetCellWithOffset(this.conduitOffset);
        }

        public void AssignPort(PortDisplayInfo port)
        {
            this.conduitType = port.type;
            this.conduitOffset = port.offset;
            this.elementFilter = port.filters;
            this.storageIndex = port.StorageIndex;
        }
    }
}
