using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomizeBuildings
{
    public class UserControlledTransferArm : KMonoBehaviour, IUserControlledCapacity
    {
        public int Max = 10;
        public int Min = 1;
        public LocString Unit = "";
        [MyCmpGet]
        private StationaryChoreRangeVisualizer visualizer;
        [MyCmpGet]
        private SolidTransferArm transferArm;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        public float AmountStored
        {
            get
            {
                return transferArm.pickupRange;
            }
        }

        public LocString CapacityUnits
        {
            get
            {
                return Unit;
            }
        }

        public float MaxCapacity
        {
            get
            {
                return Max;
            }
        }

        public float MinCapacity
        {
            get
            {
                return Min;
            }
        }

        public float UserMaxCapacity
        {
            get
            {
                return transferArm.pickupRange;
            }

            set
            {
                transferArm.pickupRange = Math.Min(Max, (int)value);
                visualizer.x = -transferArm.pickupRange;
                visualizer.y = -transferArm.pickupRange;
                visualizer.width = transferArm.pickupRange * 2 + 1;
                visualizer.height = visualizer.width;
            }
        }

        public bool WholeValues
        {
            get
            {
                return true;
            }
        }
    }
    
}
