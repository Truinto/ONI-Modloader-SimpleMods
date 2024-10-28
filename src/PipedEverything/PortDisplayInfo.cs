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
        public readonly Color background;
        public readonly Color border;
        public readonly SimHashes[] filters;
        public readonly Tag[] filterTags;
        public readonly int StorageIndex;
        public readonly float StorageCapacity;

        public PortDisplayInfo(SimHashes[] filters, Tag[] filterTags, ConduitType type, CellOffset offset, bool input, Color32? color, Color32? background, Color32? border, int? storageIndex, float? storageCapacity)
        {
            this.filters = filters;
            this.filterTags = filterTags;
            this.type = type;
            this.offset = offset;
            this.input = input;
            this.color = color ?? Color.white;
            this.background = background ?? Color.black;
            this.border = border ?? this.color;
            this.StorageIndex = storageIndex ?? 0;
            this.StorageCapacity = storageCapacity ?? (type == ConduitType.Gas ? 10f : 400f);
        }
    }
}
