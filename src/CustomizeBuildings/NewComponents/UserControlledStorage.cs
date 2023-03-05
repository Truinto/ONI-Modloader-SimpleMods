using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;
using KSerialization;

namespace CustomizeBuildings
{
    public class UserControlledStorage : KMonoBehaviour, IUserControlledCapacity
    {
        [Serialize]
        public float userMaxCapacity = float.PositiveInfinity;
#pragma warning disable 649
        [MyCmpReq]
        private Storage storage;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            storage.capacityKg = this.UserMaxCapacity;
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        public float AmountStored
        {
            get
            {
                return this.storage.MassStored();
            }
        }

        public LocString CapacityUnits
        {
            get
            {
                return GameUtil.GetCurrentMassUnit(false);
            }
        }

        public float MaxCapacity
        {
            get
            {
                return CustomizeBuildingsState.StateManager.State.IUserControlledMax;
            }
        }

        public float MinCapacity
        {
            get
            {
                return 0f;
            }
        }

        public float UserMaxCapacity
        {
            get
            {
                return Mathf.Min(this.userMaxCapacity, CustomizeBuildingsState.StateManager.State.IUserControlledMax);
            }
            set
            {
                this.userMaxCapacity = value;
                this.storage.capacityKg = value;
            }
        }

        public bool WholeValues
        {
            get
            {
                return false;
            }
        }
    }
}
