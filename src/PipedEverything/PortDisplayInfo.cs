using UnityEngine;
using Common;

namespace PipedEverything
{
    public class PortDisplayInfo
    {
        public readonly ConduitType type;
        public readonly CellOffset offset;
        public readonly bool input;
        public readonly Color color;
        public readonly SimHashes[] filter;
        public readonly int StorageIndex;
        public readonly float StorageCapacity;

        public PortDisplayInfo(SimHashes[] filter, ConduitType type, CellOffset offset, bool input, Color? color, int? storageIndex, float? storageCapacity)
        {
            this.filter = filter;
            this.type = type;
            this.offset = offset;
            this.input = input;
            this.color = color ?? Color.white;
            this.StorageIndex = storageIndex ?? 0;
            this.StorageCapacity = storageCapacity ?? (type == ConduitType.Gas ? 2f : 200f);
        }
    }
}
