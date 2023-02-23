using Common;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomizeBuildings
{
    public class UserControlledTransferArm : KMonoBehaviour, IUserControlledCapacity
    {
        [Serialize]
        public int UserValue = 99;
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

        protected override void OnSpawn()
        {
            base.OnSpawn();
            UserMaxCapacity = UserValue;
        }

        protected override void OnCleanUp()
        {
            visualizer = null;
            transferArm = null;
            base.OnCleanUp();
        }

        public float AmountStored
        {
            get
            {
                return UserValue;
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
                return UserValue;
            }

            set
            {
                UserValue = Helpers.MinMax(Min, (int)value, Max);
                transferArm.pickupRange = UserValue;
                visualizer.x = -UserValue;
                visualizer.y = -UserValue;
                visualizer.width = UserValue * 2 + 1;
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
