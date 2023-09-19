using STRINGS;
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
    public class ConduitConsumerOptional : KMonoBehaviour, IConduitConsumer
    {
        private float[] capacities = Array.Empty<float>();

        [SerializeField]
        public CellOffset conduitOffset;

        [SerializeField]
        public SimHashes[] elementFilter = Array.Empty<SimHashes>();

        [SerializeField]
        public ConduitType conduitType;

        [SerializeField]
        public bool ignoreMinMassCheck;

        //[SerializeField]
        //public Tag capacityTag = GameTags.Any;

        [SerializeField]
        public float capacityKG = float.PositiveInfinity;

        [SerializeField]
        public int storageIndex;

        [SerializeField]
        public bool forceAlwaysSatisfied;

        [SerializeField]
        public bool alwaysConsume;

        [SerializeField]
        public bool keepZeroMassObject = true;

        [SerializeField]
        public bool useSecondaryInput;

        [SerializeField]
        public bool isOn = true;

        [NonSerialized]
        public bool isConsuming = true;

        [NonSerialized]
        public bool consumedLastTick = true;

        [MyCmpReq]
        public Operational operational;

        [MyCmpReq]
        private Building building;

        public Operational.State OperatingRequirement = Operational.State.None;

        public ISecondaryInput targetSecondaryInput;

        private Storage _storage;

        [MyCmpGet]
        private BuildingComplete m_buildingComplete;

        private int utilityCell = -1;

        private FlowUtilityNetwork.NetworkItem networkItem;

        public float consumptionRate = float.PositiveInfinity;

        public SimHashes lastConsumedElement = SimHashes.Vacuum;

        private HandleVector<int>.Handle partitionerEntry;

        private bool satisfied;

        public ConduitConsumer.WrongElementResult wrongElementResult = ConduitConsumer.WrongElementResult.Destroy;

        public Storage Storage => this._storage ??= GetComponents<Storage>()[this.storageIndex];

        public ConduitType ConduitType => this.conduitType;

        public bool IsConnected
        {
            get
            {
                if (Grid.Objects[this.utilityCell, (this.conduitType == ConduitType.Gas) ? 12 : 16] != null)
                {
                    return this.m_buildingComplete != null;
                }
                return false;
            }
        }

        public bool CanConsume
        {
            get
            {
                bool result = false;
                if (this.IsConnected)
                {
                    result = this.GetConduitManager().GetContents(this.utilityCell).mass > 0f;
                }
                return result;
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
                if (this.Storage == null)
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

        public ConduitType TypeOfConduit => this.conduitType;

        public bool IsAlmostEmpty
        {
            get
            {
                if (!this.ignoreMinMassCheck)
                {
                    return this.MassAvailable < this.ConsumptionRate * 30f;
                }
                return false;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (!this.ignoreMinMassCheck)
                {
                    if (this.MassAvailable != 0f)
                    {
                        return this.MassAvailable < this.ConsumptionRate;
                    }
                    return true;
                }
                return false;
            }
        }

        public float ConsumptionRate => this.consumptionRate;

        public bool IsSatisfied
        {
            get
            {
                if (!this.satisfied)
                {
                    return !this.isConsuming;
                }
                return true;
            }
            set
            {
                this.satisfied = value || this.forceAlwaysSatisfied;
            }
        }

        public float MassAvailable
        {
            get
            {
                ConduitFlow conduitManager = this.GetConduitManager();
                int inputCell = this.GetInputCell(conduitManager.conduitType);
                return conduitManager.GetContents(inputCell).mass;
            }
        }

        public void SetConduitData(ConduitType type)
        {
            this.conduitType = type;
        }

        private ConduitFlow GetConduitManager()
        {
            return this.conduitType switch
            {
                ConduitType.Gas => Game.Instance.gasConduitFlow,
                ConduitType.Liquid => Game.Instance.liquidConduitFlow,
                _ => null,
            };
        }

        private int GetInputCell(ConduitType inputConduitType)
        {
            return this.building.GetCellWithOffset(this.conduitOffset);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            GameScheduler.Instance.Schedule("PlumbingTutorial", 2f, delegate
            {
                Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing);
            });
            ConduitFlow conduitManager = this.GetConduitManager();
            this.utilityCell = this.GetInputCell(conduitManager.conduitType);
            ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType == ConduitType.Gas) ? 12 : 16];
            this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, OnConduitConnectionChanged);
            this.GetConduitManager().AddConduitUpdater(ConduitUpdate);
            this.OnConduitConnectionChanged(null);

            this.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Sink, this.utilityCell, base.gameObject);
            Conduit.GetNetworkManager(this.conduitType).AddToNetworks(this.utilityCell, this.networkItem, true);
        }

        public override void OnCleanUp()
        {
            this.GetConduitManager().RemoveConduitUpdater(ConduitUpdate);
            GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
            base.OnCleanUp();

            Conduit.GetNetworkManager(this.conduitType).RemoveFromNetworks(this.utilityCell, this.networkItem, is_endpoint: true);
        }

        private void OnConduitConnectionChanged(object data)
        {
            base.Trigger(-2094018600, this.IsConnected);
        }

        public void SetOnState(bool onState)
        {
            this.isOn = onState;
        }

        private void ConduitUpdate(float dt)
        {
            if (this.isConsuming && this.isOn)
            {
                ConduitFlow conduitManager = this.GetConduitManager();
                this.Consume(dt, conduitManager);
            }
        }

        private void Consume(float dt, ConduitFlow conduit_mgr)
        {
            this.IsSatisfied = false;
            this.consumedLastTick = false;
            if (this.building.Def.CanMove)
            {
                this.utilityCell = this.GetInputCell(conduit_mgr.conduitType);
            }
            if (!this.IsConnected)
            {
                return;
            }

            ConduitFlow.ConduitContents contents = conduit_mgr.GetContents(this.utilityCell);
            if (contents.mass <= 0f)
            {
                return;
            }

            this.IsSatisfied = true;

            if (!this.alwaysConsume && !this.operational.MeetsRequirements(this.OperatingRequirement))
                return;

            float a = this.ConsumptionRate * dt;
            a = Mathf.Min(a, CapacityForElement(contents.element));
            if (a <= 0f)
                return;
            Element element = ElementLoader.FindElementByHash(contents.element);
            if (contents.element != this.lastConsumedElement)
            {
                DiscoveredResources.Instance.Discover(element.tag, element.materialCategory);
            }
            float num = 0f;
            if (a > 0f)
            {
                ConduitFlow.ConduitContents conduitContents = conduit_mgr.RemoveElement(this.utilityCell, a);
                num = conduitContents.mass;
                this.lastConsumedElement = conduitContents.element;
            }

            if (num <= 0f)
                return;

            bool isAllowedElement = this.elementFilter.Contains(element.id);
            if (this.elementFilter.Length > 0 && !isAllowedElement)
            {
                base.Trigger(-794517298, new BuildingHP.DamageSourceInfo
                {
                    damage = 1,
                    source = BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
                    popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
                });
            }

            this.consumedLastTick = true;
            if (isAllowedElement || this.wrongElementResult == ConduitConsumer.WrongElementResult.Store || contents.element == SimHashes.Vacuum || this.elementFilter.Length == 0)
            {
                int disease_count = (int)((float)contents.diseaseCount * (num / contents.mass));
                Element element2 = ElementLoader.FindElementByHash(contents.element);
                switch (this.conduitType)
                {
                    case ConduitType.Liquid:
                        if (element2.IsLiquid)
                        {
                            this.Storage.AddLiquid(contents.element, num, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, do_disease_transfer: false);
                        }
                        else
                        {
                            Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element2.id);
                        }
                        break;
                    case ConduitType.Gas:
                        if (element2.IsGas)
                        {
                            this.Storage.AddGasChunk(contents.element, num, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, do_disease_transfer: false);
                        }
                        else
                        {
                            Debug.LogWarning("Gas conduit consumer consuming non gas: " + element2.id);
                        }
                        break;
                }
            }
            else if (this.wrongElementResult == ConduitConsumer.WrongElementResult.Dump)
            {
                int disease_count2 = (int)((float)contents.diseaseCount * (num / contents.mass));
                SimMessages.AddRemoveSubstance(Grid.PosToCell(base.transform.GetPosition()), contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num, contents.temperature, contents.diseaseIdx, disease_count2);
            }

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

        public void AssignPort(PortDisplayInfo port)
        {
            this.conduitType = port.type;
            this.conduitOffset = port.offset;
            this.elementFilter = port.filter;
            this.storageIndex = port.StorageIndex;

            this.capacities = new float[this.elementFilter.Length];
            this.capacityKG = port.StorageCapacity;
        }
    }
}
