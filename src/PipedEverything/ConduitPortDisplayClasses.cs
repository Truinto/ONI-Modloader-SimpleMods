using UnityEngine;
using Common;

namespace PipedEverything
{
    public class PortDisplayInput : DisplayConduitPortInfo
    {
        public PortDisplayInput(ConduitType type, CellOffset offset, CellOffset? offsetFlipped = null, Color? color = null) : base(type, offset, offsetFlipped, true, color) { }
    }

    public class PortDisplayOutput : DisplayConduitPortInfo
    {
        public PortDisplayOutput(ConduitType type, CellOffset offset, CellOffset? offsetFlipped = null, Color? color = null) : base(type, offset, offsetFlipped, false, color) { }
    }


    // can't be stored in components. It somehow gets reset before it's used
    // Serialization doesn't seem to help at all
    public abstract class DisplayConduitPortInfo
    {
        public readonly ConduitType type;
        public readonly CellOffset offset;
        public readonly CellOffset offsetFlipped;
        public readonly bool input;
        public readonly Color color;

        public DisplayConduitPortInfo(ConduitType type, CellOffset offset, CellOffset? offsetFlipped, bool input, Color? color)
        {
            this.type = type;
            this.offset = offset;
            this.input = input;

            this.offsetFlipped = offsetFlipped ?? offset;

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
