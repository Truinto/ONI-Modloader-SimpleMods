using UnityEngine;
using Common;

namespace PipedEverything
{
    // can't be stored in components. It somehow gets reset before it's used
    // Serialization doesn't seem to help at all
    public class PortDisplayInfo
    {
        public readonly ConduitType type;
        public readonly CellOffset offset;
        public readonly bool input;
        public readonly Color color;
        public readonly SimHashes[] filter;
        public readonly int StorageIndex;
        public readonly int StorageCapacity;

        public PortDisplayInfo(SimHashes[] filter, ConduitType type, CellOffset offset, bool input, Color? color, int? storageIndex, int? storageCapacity)
        {
            this.filter = filter;
            this.type = type;
            this.offset = offset;
            this.input = input;
            this.color = color ?? Color.white;
            this.StorageIndex = storageIndex ?? 0;
            this.StorageCapacity = storageCapacity ?? (type == ConduitType.Gas ? 2 : 100);

            //var resources = BuildingCellVisualizerResources.Instance();
            //var ioColors = type == ConduitType.Gas ? resources.gasIOColours : resources.liquidIOColours;
            //var colorSet = input ? ioColors.input : ioColors.output;
            //this.color = colorSet.connected;
        }
    }
}
