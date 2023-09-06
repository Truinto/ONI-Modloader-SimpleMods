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
        public int UserValue = 4;
        public int Max = 10;
        public int Min = 1;
        public LocString Unit = "";
        [MyCmpGet]
        private RangeVisualizer visualizer;
        [MyCmpGet]
        private SolidTransferArm transferArm;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            UserMaxCapacity = UserValue;
            visualizer.BlockingCb = SolidTransferArmConfig_Patches.BlockingCbx;
        }

        public override void OnCleanUp()
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

                if (UserValue > 30)
                {
                    visualizer.RangeMin.x = -1;
                    visualizer.RangeMin.y = -1;
                    visualizer.RangeMax.x = 1;
                    visualizer.RangeMax.y = 1;
                }
                else
                {
                    visualizer.RangeMin.x = -UserValue;
                    visualizer.RangeMin.y = -UserValue;
                    visualizer.RangeMax.x = UserValue;
                    visualizer.RangeMax.y = UserValue;
                }
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
