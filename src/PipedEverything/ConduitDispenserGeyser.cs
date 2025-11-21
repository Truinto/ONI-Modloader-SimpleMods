using Common;
using Klei;
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

        private ElementEmitter? ElementEmitter;
        private PortDisplayController? Controller;
        private PortDisplay2? Port;
        private HandleVector<int>.Handle PartitionerEntry;
        private int UtilityCell;
        private int NextStorageIndex;
        private FlowUtilityNetwork.NetworkItem? NetworkItem;
        private int Cooldown;

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
        }

        public override void OnCleanUp()
        {
            // conduit remove
            GetConduitManager().RemoveConduitUpdater(ConduitUpdate);
            GameScenePartitioner.Instance.Free(ref this.PartitionerEntry);
            base.OnCleanUp();
            Conduit.GetNetworkManager(this.ConduitType).RemoveFromNetworks(this.UtilityCell, this.NetworkItem, is_endpoint: true);
        }

        private void OnConduitConnectionChanged(object? data)
        {
            BoxingTrigger((int)GameHashes.ConduitConnectionChanged, IsConnected());
        }

        public void Sim200ms(float dt)
        {
            // null checks
            if (this.ElementEmitter == null)
                throw new NullReferenceException(nameof(this.ElementEmitter));
            if (!this.ElementEmitter.simActive)
            {
                this.Cooldown = 0;
                return;
            }
            if (!Sim.IsValidHandle(this.ElementEmitter.simHandle))
                return;
            if (this.Port == null)
                throw new NullReferenceException(nameof(this.Port));

            bool usePort = this.Port.IsConnected() && !this.Port.Storage.IsFull();

            // prevent flickering
            if (usePort)
            {
                if (this.Cooldown > 0)
                {
                    this.Cooldown--;
                    usePort = false;
                }
            }
            else
            {
                this.Cooldown = 20;
            }

            if (usePort)
            {
                var emitterOutput = this.ElementEmitter.outputElement;

                // disable emission
                if (this.ElementEmitter.emissionFrequency > 0f)
                {
                    this.ElementEmitter.emissionFrequency = 0f;
                    int game_cell = Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), (int)emitterOutput.outputElementOffset.x, (int)emitterOutput.outputElementOffset.y);
                    SimMessages.ModifyElementEmitter(this.ElementEmitter.simHandle, game_cell, 1, SimHashes.Vacuum, 0f, 0f, 0f, this.ElementEmitter.maxPressure, byte.MaxValue, 0);
                }
                // store in storage
                else
                {
                    float emit_temperature = emitterOutput.minOutputTemperature == 0f ? this.ElementEmitter.GetComponent<PrimaryElement>().Temperature : emitterOutput.minOutputTemperature;
                    this.Port.TryStore(emitterOutput.elementHash, emitterOutput.massGenerationRate * dt, emit_temperature, emitterOutput.addedDiseaseIdx, emitterOutput.addedDiseaseCount);
                }
            }
            else
            {
                // enable emission
                if (this.ElementEmitter.emissionFrequency < 1f)
                {
                    this.ElementEmitter.emissionFrequency = 1f;
                    var emitterOutput = this.ElementEmitter.outputElement;
                    int game_cell = Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), (int)emitterOutput.outputElementOffset.x, (int)emitterOutput.outputElementOffset.y);
                    float emit_temperature = emitterOutput.minOutputTemperature == 0f ? this.ElementEmitter.GetComponent<PrimaryElement>().Temperature : emitterOutput.minOutputTemperature;
                    SimMessages.ModifyElementEmitter(this.ElementEmitter.simHandle, game_cell, this.ElementEmitter.emitRange, emitterOutput.elementHash, this.ElementEmitter.emissionFrequency, emitterOutput.massGenerationRate, emit_temperature, this.ElementEmitter.maxPressure, emitterOutput.addedDiseaseIdx, emitterOutput.addedDiseaseCount);

                    this.Port.TryStore(emitterOutput.elementHash, emitterOutput.massGenerationRate * dt, emit_temperature, emitterOutput.addedDiseaseIdx, emitterOutput.addedDiseaseCount);
                }
            }
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
                throw new NullReferenceException(nameof(this.Port));
            if (this.ElementEmitter == null)
                throw new NullReferenceException(nameof(this.ElementEmitter));

            // put element on conduit
            var element = findElement();
            if (element != null)
            {
                element.KeepZeroMassObject = true;
                switch (GetConduitManager())
                {
                    case ConduitFlow pipeFlow:
                        float massAddedToPipe;
                        if (PipedEverythingState.StateManager.State.GeyserPipesUnlimited)
                            massAddedToPipe = ConduitAddElementOverpressure(pipeFlow, this.UtilityCell, element.ElementID, element.Mass, element.Temperature, element.DiseaseIdx, element.DiseaseCount, this.ElementEmitter.outputElement.massGenerationRate);
                        else
                            massAddedToPipe = pipeFlow.AddElement(this.UtilityCell, element.ElementID, element.Mass, element.Temperature, element.DiseaseIdx, element.DiseaseCount);
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
                            float mass = FumiKMod.SolidMaxMass;
                            if (PipedEverythingState.StateManager.State.GeyserPipesUnlimited)
                                mass = Mathf.Max(mass, this.ElementEmitter.outputElement.massGenerationRate);

                            var pickupable = element.GetComponent<Pickupable>();
                            if (pickupable != null)
                                solidFlow.AddPickupable(this.UtilityCell, pickupable.Take(mass) ?? pickupable);
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
                    if (this.TagFilter.Length != 0 && !items[index].HasAnyTags(this.TagFilter))
                        continue;

                    this.NextStorageIndex = (i + 1) % count;
                    return primaryElement;
                }
                return null;
            }
        }

        /// <summary>
        /// Overload the input node. This won't increase the whole pipe's volume. For that you could patch <see cref="ConduitFlow.ConduitContents.GetEffectiveCapacity(float)"/>.<br/>
        /// Loading a save will sanities the node, deleting some mass. To fix patch <see cref="ConduitFlow.OnDeserialized"/>.
        /// </summary>
        public static float ConduitAddElementOverpressure(ConduitFlow instance, int cell_idx, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, float massOverpressure)
        {
            // check connected
            if (instance.grid[cell_idx].conduitIdx == -1)
                return 0f;

            // check pipe empty or same element
            var contents = instance.GetConduit(cell_idx).GetContents(instance);
            if (contents.element != element && contents.element != SimHashes.Vacuum && mass > 0f)
                return 0f;

            // get transfer mass
            float transferMass = Mathf.Min(mass, Mathf.Max(massOverpressure, instance.MaxMass) - contents.mass);
            if (transferMass <= 0f)
                return 0f;

            // transfer properties
            contents.temperature = GameUtil.GetFinalTemperature(temperature, transferMass, contents.temperature, contents.mass);
            contents.AddMass(transferMass);
            contents.element = element;
            contents.ConsolidateMass();
            float percent = transferMass / mass;
            int transferDisease = (int)(percent * disease_count);
            if (transferDisease > 0)
            {
                var diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, transferDisease, contents.diseaseIdx, contents.diseaseCount);
                contents.diseaseIdx = diseaseInfo.idx;
                contents.diseaseCount = diseaseInfo.count;
            }

            // set output
            instance.SetContents(cell_idx, contents);
            return transferMass;
        }
    }
}
