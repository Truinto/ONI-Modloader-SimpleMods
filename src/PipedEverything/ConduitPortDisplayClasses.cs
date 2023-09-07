using UnityEngine;
using Common;

namespace PipedEverything
{
    public class PortDisplayInput : DisplayConduitPortInfo
    {
        public PortDisplayInput(SimHashes[] filter, ConduitType type, CellOffset offset, Color? color = null) : base(filter, type, offset, true, color) { }
    }

    public class PortDisplayOutput : DisplayConduitPortInfo
    {
        public PortDisplayOutput(SimHashes[] filter, ConduitType type, CellOffset offset, Color? color = null) : base(filter, type, offset, false, color) { }
    }


    // can't be stored in components. It somehow gets reset before it's used
    // Serialization doesn't seem to help at all
    public class DisplayConduitPortInfo
    {
        public readonly ConduitType type;
        public readonly CellOffset offset;
        public readonly bool input;
        public readonly Color color;
        public readonly SimHashes[] filter;

        public DisplayConduitPortInfo(SimHashes[] filter, ConduitType type, CellOffset offset, bool input, Color? color)
        {
            this.filter = filter;
            this.type = type;
            this.offset = offset;
            this.input = input;

            // assign port colors
            if (color != null)
            {
                this.color = color ?? Color.white;
            }
            else
            {
                // none given. Use defaults
                var resources = BuildingCellVisualizerResources.Instance();
                var ioColors = type == ConduitType.Gas ? resources.gasIOColours : resources.liquidIOColours;
                var colorSet = input ? ioColors.input : ioColors.output;

                this.color = colorSet.connected;
            }
        }
    }
}
