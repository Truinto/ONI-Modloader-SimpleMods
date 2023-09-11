using UnityEngine;
using Common;
using static LogicPorts;
using System.Linq;

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    public class PortDisplay2 : KMonoBehaviour
    {
        private GameObject portObject;

        private int lastUtilityCell = -1;

        private Color lastColor = Color.black;

        [SerializeField]
        internal ConduitType type;

        [SerializeField]
        internal CellOffset offset;

        [SerializeField]
        internal bool input;

        [SerializeField]
        internal Color32 color;

        [SerializeField]
        internal Sprite sprite;

        [SerializeField]
        internal SimHashes[] filter;

        [SerializeField]
        internal int storageIndex;

        [SerializeField]
        internal int storageCapacity;

        private Storage storage;

        public Storage Storage => storage ??= GetComponents<Storage>()[this.storageIndex];

        public void AssignPort(PortDisplayInfo port)
        {
            this.type = port.type;
            this.offset = port.offset;
            this.input = port.input;
            this.color = port.color;
            this.sprite = GetSprite();
            this.filter = port.filter;
            this.storageIndex = port.StorageIndex;
            this.storageCapacity = port.StorageCapacity;
        }

        public void Draw(GameObject obj, BuildingCellVisualizer visualizer, bool force)
        {
            int utilityCell = visualizer.building.GetCellWithOffset(this.offset);

            // redraw if anything changed
            if (force || utilityCell != this.lastUtilityCell || this.color != this.lastColor)
            {
                this.lastColor = color;
                this.lastUtilityCell = utilityCell;
                visualizer.DrawUtilityIcon(utilityCell, this.sprite, ref portObject, color, Color.white);
            }
        }

        private Sprite GetSprite()
        {
            var resources = BuildingCellVisualizerResources.Instance();
            if (input)
            {
                if (this.type == ConduitType.Gas)
                {
                    return resources.gasInputIcon;
                }
                else if (this.type == ConduitType.Liquid || this.type == ConduitType.Solid)
                {
                    return resources.liquidInputIcon;
                }
            }
            else
            {
                if (this.type == ConduitType.Gas)
                {
                    return resources.gasOutputIcon;
                }
                else if (this.type == ConduitType.Liquid || this.type == ConduitType.Solid)
                {
                    return resources.liquidOutputIcon;
                }
            }

            return null;
        }

        internal void DisableIcons()
        {
            if (this.portObject != null)
            {
                if (this.portObject != null && this.portObject.activeInHierarchy)
                {
                    this.portObject.SetActive(false);
                }
            }
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
            if (this.portObject != null)
            {
                UnityEngine.Object.Destroy(this.portObject);
            }
        }

        public bool IsConnected()
        {
            if (this.lastUtilityCell < 0)
                this.lastUtilityCell = this.GetCellWithOffset(this.offset);

            int layer = this.type == ConduitType.Gas ? 12 : this.type == ConduitType.Liquid ? 16 : 20;
            return Grid.Objects[this.lastUtilityCell, layer] != null;
        }

        public float GetCapacity(SimHashes element)
        {
            if (!filter.Contains(element))
                return 0f;

            float capacityElement = this.storageCapacity;
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

        public bool IsBlocked()
        {
            float capacityElement = this.storageCapacity;
            foreach (var item in this.Storage.items)
            {
                if (item == null)
                    continue;

                var element2 = item.GetComponent<PrimaryElement>();
                if (this.filter.Contains(element2.ElementID) && element2.Mass > capacityElement)
                    return true;
            }
            return false;
        }

        public bool TryStore(Element element, float mass, float temperature)
        {
            //if (GetCapacity(element.id) < 0)
            //    return false;

            if (element.IsGas)
                this.Storage.AddGasChunk(element.id, mass, temperature, 0, 0, keep_zero_mass: true);
            else if (element.IsLiquid)
                this.Storage.AddLiquid(element.id, mass, temperature, 0, 0, keep_zero_mass: true);

            return true;
        }
    }
}
