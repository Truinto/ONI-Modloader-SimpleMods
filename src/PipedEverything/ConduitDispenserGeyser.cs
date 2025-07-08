using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    public class ConduitDispenserGeyser : KMonoBehaviour, ISim200ms
    {
        [SerializeField]
        public ConduitType ConduitType;
        [SerializeField]
        public Tag[] TagFilter = [];
        [SerializeField]
        public SimHashes OutputElement = SimHashes.Void;

        private bool realIsBlocked;
        private ElementEmitter? ElementEmitter;
        private PortDisplayController? Controller;
        private PortDisplay2? Port;
        private HandleVector<int>.Handle PartitionerEntry;
        private int UtilityCell;
        private int NextStorageIndex;
        private FlowUtilityNetwork.NetworkItem? NetworkItem;

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.ElementEmitter = GetComponent<ElementEmitter>() ?? throw new NullReferenceException(nameof(this.ElementEmitter));
            this.Controller = GetComponent<PortDisplayController>() ?? throw new NullReferenceException(nameof(this.Controller));
            if (this.ConduitType == ConduitType.None)
                this.ConduitType = this.Controller.outputPorts.FirstOrDefault()?.type ?? throw new NullReferenceException(nameof(this.ConduitType));
            this.Port = this.Controller.GetPort(false, this.ConduitType, this.OutputElement) ?? throw new NullReferenceException(nameof(this.Port));
            this.UtilityCell = this.GetCellWithOffset(this.Port.offset);

            // conduit add
            var layer = GameScenePartitioner.Instance.objectLayers[GetLayer()];
            this.PartitionerEntry = GameScenePartitioner.Instance.Add("ConduitDispenserGeyser.OnSpawn", base.gameObject, this.UtilityCell, layer, OnConduitConnectionChanged);
            GetConduitManager().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Dispense); //ConduitFlowPriority.LastPostUpdate?
            OnConduitConnectionChanged(null);
            this.NetworkItem = new FlowUtilityNetwork.NetworkItem(this.ConduitType, Endpoint.Source, this.UtilityCell, base.gameObject);
            Conduit.GetNetworkManager(this.ConduitType).AddToNetworks(this.UtilityCell, this.NetworkItem, true);

            Subscribe((int)GameHashes.EmitterBlocked, OnEmitterBlocked);
            Subscribe((int)GameHashes.EmitterUnblocked, OnEmitterUnblocked);
        }

        public override void OnCleanUp()
        {
            Unsubscribe((int)GameHashes.EmitterBlocked, OnEmitterBlocked);
            Unsubscribe((int)GameHashes.EmitterUnblocked, OnEmitterUnblocked);

            // conduit remove
            GetConduitManager().RemoveConduitUpdater(ConduitUpdate);
            GameScenePartitioner.Instance.Free(ref this.PartitionerEntry);
            base.OnCleanUp();
            Conduit.GetNetworkManager(this.ConduitType).RemoveFromNetworks(this.UtilityCell, this.NetworkItem, is_endpoint: true);
        }

        /// <summary>
        /// Remember the real blocked state, by listening to events other than this component.
        /// </summary>
        private void OnEmitterBlocked(object? obj)
        {
            if (!ReferenceEquals(this, obj))
                realIsBlocked = true;
        }
        private void OnEmitterUnblocked(object? obj)
        {
            if (!ReferenceEquals(this, obj))
                realIsBlocked = false;
        }

        private void OnConduitConnectionChanged(object? data)
        {
            Trigger((int)GameHashes.ConduitConnectionChanged, IsConnected());
        }

        public void Sim200ms(float dt)
        {
            // null checks
            if (this.ElementEmitter == null)
                throw new NullReferenceException(nameof(this.ElementEmitter));
            if (!this.ElementEmitter.simActive)
                return;
            if (this.Port == null)
                throw new NullReferenceException(nameof(this.Port));

            bool usePort = this.Port.IsConnected() && !this.Port.Storage.IsFull();
            if (usePort)
            {
                // disable emission
                if (this.ElementEmitter.emissionFrequency > 0f)
                {
                    this.ElementEmitter.emissionFrequency = 0f;
                    SimMessages.ModifyElementEmitter(ElementEmitter.simHandle, 0, 0, SimHashes.Vacuum, 0f, 0f, 0f, 0f, byte.MaxValue, 0);
                }

                // store in storage
                var emitterOutput = this.ElementEmitter.outputElement;
                float emit_temperature = emitterOutput.minOutputTemperature == 0f ? ElementEmitter.GetComponent<PrimaryElement>().Temperature : emitterOutput.minOutputTemperature;
                this.Port.TryStore(emitterOutput.elementHash, emitterOutput.massGenerationRate * dt, emit_temperature, emitterOutput.addedDiseaseIdx, emitterOutput.addedDiseaseCount);
                if (this.ElementEmitter.isEmitterBlocked)
                {
                    realIsBlocked = true;
                    this.ElementEmitter.isEmitterBlocked = false;
                    Trigger((int)GameHashes.EmitterUnblocked, this);
                }
            }
            else
            {
                // enable emission
                if (this.ElementEmitter.emissionFrequency < 1f)
                {
                    this.ElementEmitter.emissionFrequency = 1f;
                    var emitterOutput = ElementEmitter.outputElement;
                    int game_cell = Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), (int)emitterOutput.outputElementOffset.x, (int)emitterOutput.outputElementOffset.y);
                    float emit_temperature = emitterOutput.minOutputTemperature == 0f ? ElementEmitter.GetComponent<PrimaryElement>().Temperature : emitterOutput.minOutputTemperature;
                    SimMessages.ModifyElementEmitter(ElementEmitter.simHandle, game_cell, ElementEmitter.emitRange, emitterOutput.elementHash, ElementEmitter.emissionFrequency, emitterOutput.massGenerationRate, emit_temperature, ElementEmitter.maxPressure, emitterOutput.addedDiseaseIdx, emitterOutput.addedDiseaseCount);
                }
                if (this.ElementEmitter.isEmitterBlocked != realIsBlocked)
                {
                    this.ElementEmitter.isEmitterBlocked = realIsBlocked;
                    if (realIsBlocked)
                        Trigger((int)GameHashes.EmitterBlocked, this);
                    else
                        Trigger((int)GameHashes.EmitterUnblocked, this);
                }
            }

            // TODO: fix state being Overpressure (idle in 1.4 cycles); Emitting Steam: Infinity t
        }

        public bool IsConnected() => Grid.Objects[this.UtilityCell, GetLayer()]?.GetComponent<BuildingComplete>() != null;

        public int GetLayer() => this.ConduitType switch
        {
            ConduitType.Gas => 12,
            ConduitType.Liquid => 16,
            _ => 20,
        };

        public IConduitFlow GetConduitManager() => this.ConduitType switch
        {
            ConduitType.Gas => Game.Instance.gasConduitFlow,
            ConduitType.Liquid => Game.Instance.liquidConduitFlow,
            _ => Game.Instance.solidConduitFlow,
        };

        private void ConduitUpdate(float dt)
        {
            if (this.Port == null)
                return;

            // put element on conduit
            var element = findElement();
            if (element != null)
            {
                element.KeepZeroMassObject = true;
                switch (GetConduitManager())
                {
                    case ConduitFlow pipeFlow:
                        float massAddedToPipe = pipeFlow.AddElement(this.UtilityCell, element.ElementID, element.Mass, element.Temperature, element.DiseaseIdx, element.DiseaseCount);
                        if (massAddedToPipe > 0f)
                        {
                            int diseaseAddedToPipe = (int)(massAddedToPipe / element.Mass * element.DiseaseCount);
                            element.ModifyDiseaseCount(-diseaseAddedToPipe, "ConduitDispenserGeyser.ConduitUpdate");
                            element.Mass -= massAddedToPipe;
                            this.Port.Storage.Trigger((int)GameHashes.OnStorageChange, element.gameObject);
                        }
                        break;

                    case SolidConduitFlow solidFlow:
                        if (solidFlow.HasConduit(this.UtilityCell) && solidFlow.IsConduitEmpty(this.UtilityCell))
                        {
                            var pickupable = element.GetComponent<Pickupable>();
                            if (pickupable != null)
                                solidFlow.AddPickupable(this.UtilityCell, pickupable.Take(20f));
                        }
                        break;
                }
            }

            //// update storage network; this is only relevant for duplicant accessible storages
            //if (this.ConduitType == ConduitType.Solid)
            //    this.Port.Storage.storageNetworkID = Grid.Objects[this.UtilityCell, 20]?.GetComponent<SolidConduit>()?.GetNetwork()?.id ?? -1;

            PrimaryElement? findElement()
            {
                List<GameObject> items = this.Port.Storage.items;
                int count = items.Count;
                for (int i = 0; i < count; i++)
                {
                    int index = (i + this.NextStorageIndex) % count;
                    var primaryElement = items[index].GetComponent<PrimaryElement>();
                    if (primaryElement == null || primaryElement.Mass <= 0f)
                        continue;
                    if (this.ConduitType != primaryElement.Element.GetConduitType())
                        continue;
                    if (this.TagFilter.Length != 0 && !items[index].HasAnyTags(this.TagFilter))
                        continue;

                    this.NextStorageIndex = (i + 1) % count;
                    return primaryElement;
                }
                return null;
            }
        }
    }
}
