using UnityEngine;
using Common;

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    public class PortDisplay2 : KMonoBehaviour
    {
        private GameObject portObject;

        // The cache for last location/color.
        // The default values doesn't matter and will be overwritten on first call.
        // However there is a theoredical risk that no default value can cause a crash, hence setting them to something.
        [SerializeField]
        private int lastUtilityCell = -1;

        [SerializeField]
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

        public void AssignPort(DisplayConduitPortInfo port)
        {
            this.type = port.type;
            this.offset = port.offset;
            this.input = port.input;
            this.color = port.color;
            this.sprite = GetSprite();
            this.filter = port.filter;
        }

        public void Draw(GameObject obj, BuildingCellVisualizer visualizer, bool force)
        {
            int utilityCell = visualizer.building.GetCellWithOffset(this.offset);

            // redraw if anything changed
            if (force || utilityCell != this.lastUtilityCell || color != this.lastColor)
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
            int layer = this.type == ConduitType.Gas ? 12 : this.type == ConduitType.Liquid ? 16 : 20;
            return Grid.Objects[this.lastUtilityCell, layer] != null;
        }
    }
}
